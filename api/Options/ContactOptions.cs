namespace LandingFinal.Api.Options;

public sealed class ResendOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

public sealed class ContactEmailOptions
{
    public string To { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
}

public sealed class TurnstileOptions
{
    public string SecretKey { get; set; } = string.Empty;
}
