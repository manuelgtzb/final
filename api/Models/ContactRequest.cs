using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace LandingFinal.Api.Models;

public sealed class ContactRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    [EmailAddress]
    [StringLength(254)]
    public string? Email { get; set; }

    [StringLength(30)]
    [RegularExpression(@"^[0-9+() .-]{7,30}$", ErrorMessage = "Phone has an invalid format.")]
    public string? Phone { get; set; }

    [Required]
    [StringLength(3000, MinimumLength = 10)]
    public string? Project { get; set; }

    [Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "Budget must be a positive number.")]
    public decimal Budget { get; set; }

    [Required]
    [RegularExpression("^(MXN|USD)$", ErrorMessage = "Currency must be MXN or USD.")]
    public string? Currency { get; set; }

    [Required]
    [StringLength(2048)]
    public string? TurnstileToken { get; set; }

    public void Normalize()
    {
        Name = Name?.Trim();
        Email = NullIfWhiteSpace(Email)?.ToLowerInvariant();
        Phone = NullIfWhiteSpace(Phone);
        Project = Project?.Trim();
        Currency = Currency?.Trim().ToUpperInvariant();
        TurnstileToken = TurnstileToken?.Trim();
    }

    public string CreateFingerprint()
    {
        var canonicalValue = string.Join('\n',
            Name,
            Email,
            Phone,
            Project,
            Budget.ToString("0.00", CultureInfo.InvariantCulture),
            Currency);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalValue)));
    }

    private static string? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public static class ContactRequestValidator
{
    public static Dictionary<string, string[]> Validate(ContactRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);

        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Phone))
        {
            results.Add(new ValidationResult(
                "At least one of email or phone is required.",
                [nameof(request.Email), nameof(request.Phone)]));
        }

        return results
            .SelectMany(result => result.MemberNames.DefaultIfEmpty("request")
                .Select(member => new { Member = ToCamelCase(member), Message = result.ErrorMessage ?? "Invalid value." }))
            .GroupBy(item => item.Member)
            .ToDictionary(
                group => group.Key,
                group => group.Select(item => item.Message).Distinct().ToArray());
    }

    private static string ToCamelCase(string value) =>
        string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value[1..];
}
