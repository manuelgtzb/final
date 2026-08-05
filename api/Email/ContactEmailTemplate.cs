using System.Globalization;
using System.Text.Encodings.Web;
using LandingFinal.Api.Models;

namespace LandingFinal.Api.Email;

public sealed record ContactEmailContent(string Html, string Text);

public static class ContactEmailTemplate
{
    public static ContactEmailContent Build(ContactRequest request, DateTimeOffset submittedAt, string logoUrl)
    {
        var encoder = HtmlEncoder.Default;
        var safeName = encoder.Encode(request.Name ?? string.Empty);
        var safeEmail = encoder.Encode(request.Email ?? "No proporcionado");
        var safePhone = encoder.Encode(request.Phone ?? "No proporcionado");
        var safeProject = EncodeMultiline(request.Project ?? string.Empty, encoder);
        var safeBudget = encoder.Encode($"{request.Budget:N2} {request.Currency}");
        var safeDate = encoder.Encode(submittedAt.UtcDateTime.ToString("dd/MM/yyyy HH:mm 'UTC'", CultureInfo.InvariantCulture));
        var safeLogoUrl = Uri.TryCreate(logoUrl, UriKind.Absolute, out var logoUri) && logoUri.Scheme == Uri.UriSchemeHttps
            ? encoder.Encode(logoUri.ToString())
            : null;

        var brand = safeLogoUrl is not null
            ? $"<img src=\"{safeLogoUrl}\" width=\"150\" alt=\"ROMA LABS\" style=\"display:block;width:150px;max-width:100%;height:auto;border:0;\" />"
            : "<div style=\"font-family:Arial,sans-serif;font-size:22px;line-height:28px;font-weight:700;color:#f4f0e8;\">ROMA LABS</div>";

        var html = $$"""
            <!doctype html>
            <html lang="es">
              <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width,initial-scale=1">
                <meta name="color-scheme" content="dark">
                <meta name="supported-color-schemes" content="dark">
                <style>
                  :root { color-scheme: dark; supported-color-schemes: dark; }
                  @media (prefers-color-scheme: dark) {
                    .email-background { background-color:#0b0c0e !important; background-image:linear-gradient(#0b0c0e,#0b0c0e) !important; }
                    .email-card { background-color:#17181b !important; background-image:linear-gradient(#17181b,#17181b) !important; }
                    .email-text { color:#f4f0e8 !important; }
                  }
                  [data-ogsc] .email-background, [data-ogsc].email-background { background-color:#0b0c0e !important; background-image:linear-gradient(#0b0c0e,#0b0c0e) !important; }
                  [data-ogsc] .email-card, [data-ogsc].email-card { background-color:#17181b !important; background-image:linear-gradient(#17181b,#17181b) !important; }
                  [data-ogsc] .email-text, [data-ogsc].email-text { color:#f4f0e8 !important; }
                </style>
              </head>
              <body class="email-background" bgcolor="#0b0c0e" style="margin:0;padding:0;background-color:#0b0c0e;background-image:linear-gradient(#0b0c0e,#0b0c0e);color:#f4f0e8;">
                <table class="email-background" role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" bgcolor="#0b0c0e" style="width:100%;background-color:#0b0c0e;background-image:linear-gradient(#0b0c0e,#0b0c0e);">
                  <tr>
                    <td align="center" style="padding:32px 16px;">
                      <table class="email-card" role="presentation" width="640" cellspacing="0" cellpadding="0" border="0" bgcolor="#17181b" style="width:100%;max-width:640px;background-color:#17181b;background-image:linear-gradient(#17181b,#17181b);border:1px solid #d7aa55;">
                        <tr><td style="padding:30px 32px 22px;border-bottom:1px solid #d7aa55;">{{brand}}</td></tr>
                        <tr>
                          <td class="email-text" style="padding:32px;color:#f4f0e8;">
                            <div style="font-family:Arial,sans-serif;font-size:11px;line-height:16px;font-weight:700;color:#d7aa55;">NUEVO CONTACTO</div>
                            <h1 class="email-text" style="margin:8px 0 28px;font-family:Arial,sans-serif;font-size:27px;line-height:34px;font-weight:700;color:#f4f0e8;">SOLICITUD DE PROYECTO</h1>
                            {{Row("NOMBRE", safeName)}}
                            {{Row("EMAIL", safeEmail)}}
                            {{Row("TELÉFONO", safePhone)}}
                            {{Row("NECESIDAD", safeProject)}}
                            {{Row("PRESUPUESTO", safeBudget)}}
                            {{Row("FECHA", safeDate, isLast: true)}}
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:18px 32px;background-color:#17181b;background-image:linear-gradient(#17181b,#17181b);border-top:1px solid #333438;font-family:Arial,sans-serif;font-size:11px;line-height:17px;color:#8f9298;">
                            Enviado de forma segura desde el formulario de Roma Labs.
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </body>
            </html>
            """;

        var text = $$"""
            ROMA LABS
            SOLICITUD DE PROYECTO

            Nombre: {{request.Name}}
            Email: {{request.Email ?? "No proporcionado"}}
            Teléfono: {{request.Phone ?? "No proporcionado"}}
            Necesidad:
            {{request.Project}}

            Presupuesto: {{request.Budget:N2}} {{request.Currency}}
            Fecha: {{submittedAt.UtcDateTime:dd/MM/yyyy HH:mm}} UTC
            """;

        return new ContactEmailContent(html, text);
    }

    private static string Row(string label, string value, bool isLast = false) => $$"""
        <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="width:100%;{{(isLast ? string.Empty : "border-bottom:1px solid #333438;")}}">
          <tr>
            <td width="135" valign="top" style="width:135px;padding:15px 12px 15px 0;font-family:Arial,sans-serif;font-size:11px;line-height:18px;font-weight:700;color:#d7aa55;">{{label}}</td>
            <td class="email-text" valign="top" style="padding:15px 0;font-family:Arial,sans-serif;font-size:15px;line-height:23px;color:#f4f0e8;word-break:break-word;">{{value}}</td>
          </tr>
        </table>
        """;

    private static string EncodeMultiline(string value, HtmlEncoder encoder) =>
        encoder.Encode(value).Replace("\r\n", "<br>", StringComparison.Ordinal).Replace("\n", "<br>", StringComparison.Ordinal);
}
