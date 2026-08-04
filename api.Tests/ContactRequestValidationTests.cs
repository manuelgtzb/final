using LandingFinal.Api.Models;

namespace LandingFinal.Api.Tests;

public sealed class ContactRequestValidationTests
{
    [Fact]
    public void Validate_AcceptsACompleteRequestWithEmailOnly()
    {
        var request = ValidRequest();
        request.Phone = null;

        var errors = ContactRequestValidator.Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RequiresEmailOrPhone()
    {
        var request = ValidRequest();
        request.Email = null;
        request.Phone = null;

        var errors = ContactRequestValidator.Validate(request);

        Assert.Contains("email", errors.Keys);
        Assert.Contains("phone", errors.Keys);
    }

    [Theory]
    [InlineData(0, "USD")]
    [InlineData(-1, "MXN")]
    [InlineData(100, "EUR")]
    public void Validate_RejectsInvalidBudgetOrCurrency(decimal budget, string currency)
    {
        var request = ValidRequest();
        request.Budget = budget;
        request.Currency = currency;

        var errors = ContactRequestValidator.Validate(request);

        Assert.NotEmpty(errors);
    }

    private static ContactRequest ValidRequest() => new()
    {
        Name = "Ada Lovelace",
        Email = "ada@example.com",
        Project = "Necesito un sitio web para mi empresa.",
        Budget = 1500,
        Currency = "USD",
        TurnstileToken = "valid-test-token"
    };
}
