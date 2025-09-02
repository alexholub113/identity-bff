using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using MinimalEndpoints.Abstractions;

namespace Identity.Bff.Api.Endpoints.Auth;

public class LogoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout", async (HttpContext context, string? returnUrl = null, ILogger<LogoutEndpoint> logger = null!) =>
        {
            try
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Ok(new { Message = "Already logged out" });
                }

                logger?.LogInformation("User {User} initiating logout", context.User.Identity?.Name);

                // Validate return URL
                var validatedReturnUrl = ValidateLogoutReturnUrl(returnUrl);

                // Sign out from the local application
                await context.SignOutAsync("Cookies");

                // Also sign out from the OIDC provider (federated logout)
                var properties = new AuthenticationProperties
                {
                    RedirectUri = validatedReturnUrl
                };

                await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, properties);

                logger?.LogInformation("User logout completed, redirecting to: {ReturnUrl}", validatedReturnUrl);

                return Results.Ok(new { Message = "Logged out successfully", RedirectUrl = validatedReturnUrl });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error during logout");
                return Results.Problem("Logout failed", statusCode: 500);
            }
        })
        .WithTags("Authentication")
        .WithSummary("Logout from both local application and identity provider")
        .WithDescription("Signs out the user from both the local application and the external identity provider (federated logout).");

        // Alternative GET endpoint for simple logout links
        app.MapGet("/auth/logout", async (HttpContext context, string? returnUrl = null, ILogger<LogoutEndpoint> logger = null!) =>
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                var validReturnUrl = ValidateLogoutReturnUrl(returnUrl);
                return Results.Redirect(validReturnUrl);
            }

            // For GET requests, redirect to logout page or perform logout directly
            logger?.LogInformation("GET logout request for user {User}", context.User.Identity?.Name);

            var validatedReturnUrl = ValidateLogoutReturnUrl(returnUrl);

            await context.SignOutAsync("Cookies");

            var properties = new AuthenticationProperties
            {
                RedirectUri = validatedReturnUrl
            };

            await context.SignOutAsync("oidc", properties);

            return Results.Empty; // The SignOutAsync will handle the redirect
        })
        .WithTags("Authentication")
        .WithSummary("Logout via GET request")
        .WithDescription("Alternative logout endpoint for simple logout links.");
    }

    private static string ValidateLogoutReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return "/";
        }

        // Only allow relative URLs for logout to prevent open redirects
        if (returnUrl.StartsWith("/") && !returnUrl.StartsWith("//"))
        {
            return returnUrl;
        }

        return "/";
    }
}