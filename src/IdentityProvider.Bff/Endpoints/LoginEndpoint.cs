using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace IdentityProvider.Bff.Endpoints;

public class LoginEndpoint : IAuthEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/login", async (HttpContext context, string? returnUrl = null, ILogger<LoginEndpoint> logger = null!) =>
        {
            try
            {
                // Generate and store state parameter for CSRF protection
                var state = GenerateSecureRandomString(32);
                var properties = new AuthenticationProperties
                {
                    Items = { ["state"] = state }
                };

                // Validate and set return URL
                var validatedReturnUrl = ValidateReturnUrl(context, returnUrl, logger);
                properties.RedirectUri = validatedReturnUrl;

                logger?.LogInformation("Initiating login for return URL: {ReturnUrl}", validatedReturnUrl);

                // Store state in session for validation on callback
                context.Session.SetString("oauth_state", state);

                // Trigger authentication challenge
                await context.ChallengeAsync("oidc", properties);
                return Results.Empty;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error during login initiation");
                return Results.Problem("Login initiation failed", statusCode: 500);
            }
        })
        .WithTags("Authentication")
        .WithSummary("Initiate secure login flow with CSRF protection")
        .WithDescription("Redirects to the external identity provider for authentication with state parameter for CSRF protection.");
    }

    private static string ValidateReturnUrl(HttpContext context, string? returnUrl, ILogger? logger)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return "/";
        }

        // Check if it's a relative URL (safe)
        if (returnUrl.StartsWith("/") && !returnUrl.StartsWith("//"))
        {
            return returnUrl;
        }

        // Check if it's an absolute URL with allowed host
        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
        {
            if (IsAllowedHost(uri.Host, context.Request.Host.Host))
            {
                return returnUrl;
            }

            logger?.LogWarning("Rejected return URL with disallowed host: {Host}", uri.Host);
        }

        logger?.LogWarning("Invalid return URL rejected: {ReturnUrl}", returnUrl);
        return "/";
    }

    private static bool IsAllowedHost(string host, string requestHost)
    {
        // Allow same host as request
        if (string.Equals(host, requestHost, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Add your allowed hosts here for security
        var allowedHosts = new[] { "localhost", "127.0.0.1" };
        return allowedHosts.Contains(host, StringComparer.OrdinalIgnoreCase);
    }

    private static string GenerateSecureRandomString(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "")[..length];
    }
}
