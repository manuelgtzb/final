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
            ? $"<img src=\"{safeLogoUrl}\" width=\"574\" alt=\"ROMA LABS\" style=\"display:block;width:100%;max-width:574px;height:auto;margin:0 auto;border:0;outline:none;text-decoration:none;\" />"
            : "<div class=\"email-light\" style=\"font-family:Arial,sans-serif;font-size:26px;line-height:32px;font-weight:700;color:#f4f0e8;-webkit-text-fill-color:#f4f0e8;\">ROMA LABS</div>";

        var html = $$"""
            <!doctype html>
            <html lang="es">
              <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width,initial-scale=1">
                <meta name="color-scheme" content="dark">
                <meta name="supported-color-schemes" content="dark">
                <style>
                  :root {
                    color-scheme: dark;
                    supported-color-schemes: dark;
                  }

                  .email-background {
                    background-color: #090a0c !important;
                    background-image: linear-gradient(#090a0c, #090a0c) !important;
                  }

                  .email-card,
                  .email-cell {
                    background-color: #17181b !important;
                    background-image: linear-gradient(#17181b, #17181b) !important;
                  }

                  .email-header,
                  .email-footer {
                    background-color: #101114 !important;
                    background-image: linear-gradient(#101114, #101114) !important;
                  }

                  .email-light {
                    color: #f4f0e8 !important;
                    -webkit-text-fill-color: #f4f0e8 !important;
                  }

                  .email-muted {
                    color: #9b9da3 !important;
                    -webkit-text-fill-color: #9b9da3 !important;
                  }

                  .email-gold {
                    color: #d7aa55 !important;
                    -webkit-text-fill-color: #d7aa55 !important;
                  }

                  @media only screen and (max-width: 640px) {
                    .email-shell { padding: 16px 10px !important; }
                    .email-header { padding: 18px !important; }
                    .email-content { padding: 26px 20px !important; }
                    .email-label { width: 105px !important; }
                  }

                  [data-ogsc] .email-background { background:#090a0c !important; }
                  [data-ogsc] .email-card,
                  [data-ogsc] .email-cell { background:#17181b !important; }
                  [data-ogsc] .email-header,
                  [data-ogsc] .email-footer { background:#101114 !important; }
                  [data-ogsc] .email-light { color:#f4f0e8 !important; }
                  [data-ogsc] .email-muted { color:#9b9da3 !important; }
                  [data-ogsc] .email-gold { color:#d7aa55 !important; }
                </style>
              </head>
              <body class="email-background" bgcolor="#090a0c" style="margin:0;padding:0;background-color:#090a0c;background-image:linear-gradient(#090a0c,#090a0c);color:#f4f0e8;">
                <table class="email-background" role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" bgcolor="#090a0c" style="width:100%;background-color:#090a0c;background-image:linear-gradient(#090a0c,#090a0c);">
                  <tr>
                    <td class="email-shell email-background" align="center" bgcolor="#090a0c" style="padding:32px 16px;background-color:#090a0c;background-image:linear-gradient(#090a0c,#090a0c);">
                      <table class="email-card" role="presentation" width="640" cellspacing="0" cellpadding="0" border="0" bgcolor="#17181b" style="width:100%;max-width:640px;background-color:#17181b;background-image:linear-gradient(#17181b,#17181b);border:1px solid #d7aa55;border-collapse:separate;">
                        <tr>
                          <td class="email-header" bgcolor="#101114" style="padding:24px 32px;background-color:#101114;background-image:linear-gradient(#101114,#101114);border-bottom:1px solid #d7aa55;">
                            {{brand}}
                          </td>
                        </tr>
                        <tr>
                          <td class="email-content email-cell" bgcolor="#17181b" style="padding:32px;background-color:#17181b;background-image:linear-gradient(#17181b,#17181b);color:#f4f0e8;-webkit-text-fill-color:#f4f0e8;">
                            <div class="email-gold" style="font-family:Arial,sans-serif;font-size:11px;line-height:16px;font-weight:700;letter-spacing:1.5px;color:#d7aa55;-webkit-text-fill-color:#d7aa55;">NUEVO CONTACTO</div>
                            <h1 class="email-light" style="margin:8px 0 28px;font-family:Arial,sans-serif;font-size:27px;line-height:34px;font-weight:700;color:#f4f0e8;-webkit-text-fill-color:#f4f0e8;">SOLICITUD DE PROYECTO</h1>
                            {{Row("NOMBRE", safeName)}}
                            {{Row("EMAIL", safeEmail)}}
                            {{Row("TELÉFONO", safePhone)}}
                            {{Row("NECESIDAD", safeProject)}}
                            {{Row("PRESUPUESTO", safeBudget)}}
                            {{Row("FECHA", safeDate, isLast: true)}}
                          </td>
                        </tr>
                        <tr>
                          <td class="email-footer email-muted" bgcolor="#101114" style="padding:18px 32px;background-color:#101114;background-image:linear-gradient(#101114,#101114);border-top:1px solid #333438;font-family:Arial,sans-serif;font-size:11px;line-height:17px;color:#9b9da3;-webkit-text-fill-color:#9b9da3;">
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
        <table class="email-cell" role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" bgcolor="#17181b" style="width:100%;background-color:#17181b;background-image:linear-gradient(#17181b,#17181b);{{(isLast ? string.Empty : "border-bottom:1px solid #333438;")}}">
          <tr>
            <td class="email-label email-gold" width="135" valign="top" bgcolor="#17181b" style="width:135px;padding:15px 12px 15px 0;background-color:#17181b;background-image:linear-gradient(#17181b,#17181b);font-family:Arial,sans-serif;font-size:11px;line-height:18px;font-weight:700;color:#d7aa55;-webkit-text-fill-color:#d7aa55;">{{label}}</td>
            <td class="email-light" valign="top" bgcolor="#17181b" style="padding:15px 0;background-color:#17181b;background-image:linear-gradient(#17181b,#17181b);font-family:Arial,sans-serif;font-size:15px;line-height:23px;color:#f4f0e8;-webkit-text-fill-color:#f4f0e8;word-break:break-word;">{{value}}</td>
          </tr>
        </table>
        """;

    private static string EncodeMultiline(string value, HtmlEncoder encoder) =>
        encoder.Encode(value)
            .Replace("\r\n", "<br>", StringComparison.Ordinal)
            .Replace("\n", "<br>", StringComparison.Ordinal);
}