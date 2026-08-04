using LandingFinal.Api.Email;
using LandingFinal.Api.Models;

namespace LandingFinal.Api.Tests;

public sealed class ContactEmailTemplateTests
{
    [Fact]
    public void Build_EscapesAllUserContentInHtml()
    {
        var request = new ContactRequest
        {
            Name = "<script>alert('name')</script>",
            Email = "safe@example.com",
            Phone = "+52 833 123 4567",
            Project = "<img src=x onerror=alert('project')>",
            Budget = 500,
            Currency = "USD",
            TurnstileToken = "token"
        };

        var content = ContactEmailTemplate.Build(
            request,
            DateTimeOffset.Parse("2026-08-03T12:00:00Z"),
            "https://example.com/logo.png");

        Assert.DoesNotContain("<script>", content.Html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<img src=x", content.Html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("&lt;script&gt;", content.Html);
        Assert.Contains("&lt;img", content.Html);
        Assert.Contains("SOLICITUD DE PROYECTO", content.Html);
        Assert.Contains("<script>alert('name')</script>", content.Text);
    }
}
