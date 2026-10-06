using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolManagement.Core.Abstractions;
using SchoolManagement.Core.Configuration;

namespace SchoolManagement.Infrastructure.Email;

/// <summary>
/// Sends transactional email via the Brevo (Sendinblue) v3 API.
/// If no API key / sender is configured, it no-ops and returns false so that
/// account creation still succeeds (credentials are shown on screen instead).
/// </summary>
public sealed class BrevoEmailSender : IEmailSender
{
    private readonly HttpClient _http;
    private readonly BrevoOptions _options;
    private readonly ILogger<BrevoEmailSender> _logger;

    public BrevoEmailSender(HttpClient http, IOptions<BrevoOptions> options, ILogger<BrevoEmailSender> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendCredentialsAsync(string toEmail, string toName, string username, string tempPassword, string schoolName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.SenderEmail))
        {
            _logger.LogInformation("Brevo not configured; skipping credentials email to {Email}.", toEmail);
            return false;
        }
        if (string.IsNullOrWhiteSpace(toEmail))
            return false;

        var loginUrl = string.IsNullOrWhiteSpace(_options.LoginUrl) ? "" : _options.LoginUrl;
        var html =
            $"<div style=\"font-family:Arial,sans-serif;font-size:15px;color:#334155\">" +
            $"<h2 style=\"color:#0f172a\">{System.Net.WebUtility.HtmlEncode(schoolName)}</h2>" +
            $"<p>Hello {System.Net.WebUtility.HtmlEncode(toName)},</p>" +
            $"<p>An account has been created for you. Use the credentials below to sign in:</p>" +
            $"<table style=\"border-collapse:collapse\">" +
            $"<tr><td style=\"padding:4px 12px 4px 0\"><b>Username</b></td><td>{System.Net.WebUtility.HtmlEncode(username)}</td></tr>" +
            $"<tr><td style=\"padding:4px 12px 4px 0\"><b>Temporary password</b></td><td>{System.Net.WebUtility.HtmlEncode(tempPassword)}</td></tr>" +
            (string.IsNullOrEmpty(loginUrl) ? "" : $"<tr><td style=\"padding:4px 12px 4px 0\"><b>Login</b></td><td><a href=\"{loginUrl}\">{loginUrl}</a></td></tr>") +
            $"</table>" +
            $"<p style=\"color:#64748b\">Please change your password after your first login.</p>" +
            $"</div>";

        var payload = new
        {
            sender = new { name = _options.SenderName, email = _options.SenderEmail },
            to = new[] { new { email = toEmail, name = string.IsNullOrWhiteSpace(toName) ? toEmail : toName } },
            subject = $"Your {schoolName} login details",
            htmlContent = html,
        };

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            req.Headers.Add("api-key", _options.ApiKey);
            req.Headers.Add("accept", "application/json");
            req.Content = JsonContent.Create(payload);

            using var res = await _http.SendAsync(req, ct);
            if (res.IsSuccessStatusCode)
                return true;

            var body = await res.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Brevo send failed ({Status}): {Body}", (int)res.StatusCode, body);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Brevo send threw for {Email}.", toEmail);
            return false;
        }
    }
}
