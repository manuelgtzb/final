using System.Globalization;
using System.Threading.RateLimiting;
using LandingFinal.Api.Models;
using LandingFinal.Api.Options;
using LandingFinal.Api.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;

const string NextCorsPolicy = "NextApp";
const string ContactRateLimitPolicy = "ContactForm";

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>()?
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray() ?? [];

if (allowedOrigins.Length == 0 && builder.Environment.IsDevelopment())
{
    allowedOrigins = ["http://localhost:3000"];
}

if (allowedOrigins.Length == 0 || allowedOrigins.Any(origin => !Uri.TryCreate(origin, UriKind.Absolute, out _)))
{
    throw new InvalidOperationException("AllowedOrigins must contain valid absolute URLs.");
}

if (!builder.Environment.IsDevelopment())
{
    string[] productionOrigins = ["https://romalabs.xyz", "https://www.romalabs.xyz"];
    if (!allowedOrigins.ToHashSet(StringComparer.OrdinalIgnoreCase).SetEquals(productionOrigins))
    {
        throw new InvalidOperationException(
            "Production CORS origins must be https://romalabs.xyz and https://www.romalabs.xyz.");
    }
}
var portValue = builder.Configuration["PORT"];
if (!string.IsNullOrWhiteSpace(portValue))
{
    if (!int.TryParse(portValue, NumberStyles.None, CultureInfo.InvariantCulture, out var port) ||
        port is < 1 or > 65535)
    {
        throw new InvalidOperationException("PORT must be a number between 1 and 65535.");
    }

    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.Configure<ResendOptions>(builder.Configuration.GetSection("Resend"));
builder.Services.Configure<ContactEmailOptions>(builder.Configuration.GetSection("ContactEmail"));
builder.Services.Configure<TurnstileOptions>(builder.Configuration.GetSection("Turnstile"));

builder.Services.AddHttpClient("Resend", client =>
{
    client.BaseAddress = new Uri("https://api.resend.com/");
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddHttpClient("Turnstile", client =>
{
    client.BaseAddress = new Uri("https://challenges.cloudflare.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddTransient<IContactEmailService, ContactEmailService>();
builder.Services.AddTransient<ITurnstileService, TurnstileService>();
builder.Services.AddSingleton<IdempotencyService>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


builder.Services.AddCors(options =>
{
    options.AddPolicy(NextCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "OPTIONS")
            .WithHeaders("Content-Type", "Idempotency-Key");
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            code = "rate_limited",
            message = "Too many contact requests. Please try again later."
        }, cancellationToken);
    };

    options.AddPolicy(ContactRateLimitPolicy, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: GetClientIp(httpContext),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors(NextCorsPolicy);
app.UseRateLimiter();

app.MapGet("/", () => Results.Redirect("/api/health"));

app.MapGet("/api/health", () => new
{
    status = "ok",
    service = "LandingFinal.Api",
    timestamp = DateTimeOffset.UtcNow
});

app.MapGet("/api/content", () => new LandingContent(
    "Landing Final",
    "Base lista con ASP.NET Core y Next.js.",
    ["API .NET 8", "Frontend Next.js", "CORS configurado para desarrollo local"]));

app.MapPost("/api/contact", async (
    HttpContext httpContext,
    ContactRequest request,
    ITurnstileService turnstileService,
    IContactEmailService emailService,
    IdempotencyService idempotencyService) =>
{
    request.Normalize();
    var validationErrors = ContactRequestValidator.Validate(request);
    if (validationErrors.Count > 0)
    {
        return Results.ValidationProblem(validationErrors);
    }

    var idempotencyHeader = httpContext.Request.Headers["Idempotency-Key"].ToString();
    if (!Guid.TryParse(idempotencyHeader, out var idempotencyKey))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["idempotencyKey"] = ["A valid Idempotency-Key header is required."]
        });
    }

    var execution = await idempotencyService.ExecuteAsync(
        idempotencyKey,
        request.CreateFingerprint(),
        async () =>
        {
            var turnstileStatus = await turnstileService.ValidateAsync(
                request.TurnstileToken!,
                GetClientIp(httpContext),
                httpContext.RequestAborted);

            if (turnstileStatus == TurnstileValidationStatus.Invalid)
            {
                return new ContactOperationResult(400, "turnstile_invalid", "Human verification failed. Please try again.");
            }

            if (turnstileStatus == TurnstileValidationStatus.Unavailable)
            {
                return new ContactOperationResult(500, "verification_unavailable", "Human verification is temporarily unavailable.");
            }

            var submittedAt = DateTimeOffset.UtcNow;
            var emailResult = await emailService.SendAsync(
                request,
                submittedAt,
                idempotencyKey,
                httpContext.RequestAborted);

            return emailResult.Status switch
            {
                ContactEmailSendStatus.Sent => new ContactOperationResult(
                    200,
                    "sent",
                    "Project request sent successfully.",
                    emailResult.EmailId),
                ContactEmailSendStatus.RateLimited => new ContactOperationResult(
                    429,
                    "rate_limited",
                    "The email provider is temporarily rate limited. Please try again later."),
                _ => new ContactOperationResult(
                    500,
                    "delivery_failed",
                    "The project request could not be delivered.")
            };
        });

    var result = execution.Result;
    return Results.Json(new
    {
        success = result.StatusCode == 200,
        code = result.Code,
        message = result.Message,
        emailId = result.StatusCode == 200 ? result.EmailId : null,
        duplicate = execution.IsDuplicate
    }, statusCode: result.StatusCode);
})
.RequireRateLimiting(ContactRateLimitPolicy);

app.Run();

static string GetClientIp(HttpContext context)
{
    var address = context.Connection.RemoteIpAddress;
    return address?.IsIPv4MappedToIPv6 == true
        ? address.MapToIPv4().ToString()
        : address?.ToString() ?? "unknown";
}

record LandingContent(string Title, string Subtitle, string[] Highlights);
