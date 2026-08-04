using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text.Json.Serialization;
using LandingFinal.Api.Email;
using LandingFinal.Api.Models;
using LandingFinal.Api.Options;
using Microsoft.Extensions.Options;

namespace LandingFinal.Api.Services;

public enum ContactEmailSendStatus { Sent, RateLimited, Failed }

public sealed record ContactEmailSendResult(ContactEmailSendStatus Status, string? EmailId = null);

public interface IContactEmailService
{
    Task<ContactEmailSendResult> SendAsync(ContactRequest request, DateTimeOffset submittedAt, Guid idempotencyKey, CancellationToken cancellationToken);
}

public sealed class ContactEmailService(
    IHttpClientFactory httpClientFactory,
    IOptions<ResendOptions> resendOptions,
    IOptions<ContactEmailOptions> contactOptions,
    ILogger<ContactEmailService> logger) : IContactEmailService
{
    public async Task<ContactEmailSendResult> SendAsync(
        ContactRequest request,
        DateTimeOffset submittedAt,
        Guid idempotencyKey,
        CancellationToken cancellationToken)
    {
        var resend = resendOptions.Value;
        var contact = contactOptions.Value;

        if (!HasValidConfiguration(resend, contact))
        {
            logger.LogError("Contact email configuration is incomplete or invalid.");
            return new ContactEmailSendResult(ContactEmailSendStatus.Failed);
        }

        var content = ContactEmailTemplate.Build(request, submittedAt, contact.LogoUrl);
        var payload = new ResendEmailRequest(
            contact.From,
            [contact.To],
            "Nueva solicitud de proyecto | Roma Labs",
            content.Html,
            content.Text,
            request.Email);

        using var message = new HttpRequestMessage(HttpMethod.Post, "emails")
        {
            Content = JsonContent.Create(payload)
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", resend.ApiKey);
        message.Headers.TryAddWithoutValidation("Idempotency-Key", $"contact-{idempotencyKey:N}");

        try
        {
            var client = httpClientFactory.CreateClient("Resend");
            using var response = await client.SendAsync(message, cancellationToken);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                logger.LogWarning("Resend rate limited a contact email request.");
                return new ContactEmailSendResult(ContactEmailSendStatus.RateLimited);
            }

            if (!response.IsSuccessStatusCode)
            {
                var providerResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Resend rejected a contact email with status {StatusCode}. Response: {ProviderResponse}",
                    (int)response.StatusCode,
                    providerResponse.Length > 500 ? providerResponse[..500] : providerResponse);
                return new ContactEmailSendResult(ContactEmailSendStatus.Failed);
            }

            var result = await response.Content.ReadFromJsonAsync<ResendEmailResponse>(cancellationToken: cancellationToken);
            return new ContactEmailSendResult(ContactEmailSendStatus.Sent, result?.Id);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError("Resend timed out while sending a contact email.");
            return new ContactEmailSendResult(ContactEmailSendStatus.Failed);
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "Resend could not be reached while sending a contact email.");
            return new ContactEmailSendResult(ContactEmailSendStatus.Failed);
        }
    }

    private static bool HasValidConfiguration(ResendOptions resend, ContactEmailOptions contact)
    {
        if (string.IsNullOrWhiteSpace(resend.ApiKey) ||
            string.IsNullOrWhiteSpace(contact.To) ||
            string.IsNullOrWhiteSpace(contact.From) ||
            !Uri.TryCreate(contact.LogoUrl, UriKind.Absolute, out var logoUri) ||
            logoUri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        try
        {
            _ = new MailAddress(contact.To);
            _ = new MailAddress(contact.From);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private sealed record ResendEmailRequest(
        [property: JsonPropertyName("from")] string From,
        [property: JsonPropertyName("to")] string[] To,
        [property: JsonPropertyName("subject")] string Subject,
        [property: JsonPropertyName("html")] string Html,
        [property: JsonPropertyName("text")] string Text,
        [property: JsonPropertyName("reply_to")]
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ReplyTo);

    private sealed record ResendEmailResponse([property: JsonPropertyName("id")] string Id);
}
