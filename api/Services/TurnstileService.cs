using System.Net.Http.Json;
using System.Text.Json.Serialization;
using LandingFinal.Api.Options;
using Microsoft.Extensions.Options;

namespace LandingFinal.Api.Services;

public enum TurnstileValidationStatus
{
    Valid,
    Invalid,
    Unavailable
}

public interface ITurnstileService
{
    Task<TurnstileValidationStatus> ValidateAsync(
        string token,
        string? remoteIp,
        CancellationToken cancellationToken);
}

public sealed class TurnstileService(
    IHttpClientFactory httpClientFactory,
    IOptions<TurnstileOptions> options,
    ILogger<TurnstileService> logger) : ITurnstileService
{
    public async Task<TurnstileValidationStatus> ValidateAsync(
        string token,
        string? remoteIp,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.Value.SecretKey))
        {
            logger.LogError("Turnstile secret key is not configured.");
            return TurnstileValidationStatus.Unavailable;
        }

        var fields = new Dictionary<string, string>
        {
            ["secret"] = options.Value.SecretKey,
            ["response"] = token,
            ["idempotency_key"] = Guid.NewGuid().ToString()
        };

        if (!string.IsNullOrWhiteSpace(remoteIp))
        {
            fields["remoteip"] = remoteIp;
        }

        try
        {
            var client = httpClientFactory.CreateClient("Turnstile");
            using var response = await client.PostAsync(
                "turnstile/v0/siteverify",
                new FormUrlEncodedContent(fields),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Turnstile returned HTTP {StatusCode}.", (int)response.StatusCode);
                return TurnstileValidationStatus.Unavailable;
            }

            var result = await response.Content.ReadFromJsonAsync<TurnstileResponse>(cancellationToken: cancellationToken);
            if (result?.Success == true)
            {
                return TurnstileValidationStatus.Valid;
            }

            var errorCodes = result?.ErrorCodes ?? [];
            logger.LogWarning("Turnstile rejected a token. Codes: {ErrorCodes}", string.Join(",", errorCodes));

            return errorCodes.Contains("internal-error", StringComparer.OrdinalIgnoreCase)
                ? TurnstileValidationStatus.Unavailable
                : TurnstileValidationStatus.Invalid;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogError("Turnstile validation timed out.");
            return TurnstileValidationStatus.Unavailable;
        }
        catch (HttpRequestException exception)
        {
            logger.LogError(exception, "Turnstile could not be reached.");
            return TurnstileValidationStatus.Unavailable;
        }
    }

    private sealed record TurnstileResponse(
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("error-codes")] string[]? ErrorCodes);
}
