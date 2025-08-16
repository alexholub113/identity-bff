using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IdentityProvider.Bff;

internal class OpenIdConnectAuthenticationHandler(IOptionsMonitor<OpenIdConnectAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IDataProtectionProvider dataProtection) : RemoteAuthenticationHandler<OpenIdConnectAuthenticationOptions>(options, logger, encoder)
{
    private readonly IDataProtectionProvider _dataProtection = dataProtection;

    protected override Task InitializeHandlerAsync()
    {
        // Initialize the StateDataFormat if not already set
        if (Options.StateDataFormat == null)
        {
            var dataProtector = _dataProtection.CreateProtector(
                typeof(OpenIdConnectAuthenticationHandler).FullName!,
                Scheme.Name,
                "state");

            Options.StateDataFormat = new SimplePropertiesDataFormat(dataProtector);
        }

        return base.InitializeHandlerAsync();
    }

    protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        // This is called when the IdP redirects back to our callback URL
        var query = Request.Query;

        // Check for error response from IdP
        if (query.ContainsKey("error"))
        {
            var error = query["error"].ToString();
            var errorDescription = query["error_description"].ToString();
            Logger.LogWarning("Authentication error: {Error} - {Description}", error, errorDescription);

            return Task.FromResult(HandleRequestResult.Fail($"Authentication failed: {error}"));
        }

        // Check for authorization code
        if (!query.ContainsKey("code"))
        {
            Logger.LogWarning("No authorization code received");
            return Task.FromResult(HandleRequestResult.Fail("No authorization code received"));
        }

        var authorizationCode = query["code"].ToString();
        var state = query["state"].ToString();

        // Validate state parameter (basic CSRF protection)
        if (!string.IsNullOrEmpty(state) && Options.StateDataFormat != null)
        {
            try
            {
                var protectedProperties = Options.StateDataFormat.Unprotect(state);
                if (protectedProperties == null)
                {
                    Logger.LogWarning("Invalid state parameter");
                    return Task.FromResult(HandleRequestResult.Fail("Invalid state parameter"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to unprotect state parameter");
                return Task.FromResult(HandleRequestResult.Fail("Invalid state parameter"));
            }
        }

        // TODO: In a real implementation, you would:
        // 1. Exchange the authorization code for tokens by calling the token endpoint
        // 2. Validate the tokens (signature, expiration, issuer, audience)
        // 3. Extract claims from the ID token

        // For now, let's create a minimal successful authentication result
        // In a real scenario, you'd get these claims from the ID token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"), // This would come from the token
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "user@example.com"),
            new Claim("sub", "user123")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        var authenticationProperties = new AuthenticationProperties
        {
            IssuedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        var ticket = new AuthenticationTicket(principal, authenticationProperties, Scheme.Name);

        Logger.LogInformation("Authentication successful for authorization code: {Code}", authorizationCode);

        return Task.FromResult(HandleRequestResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // This is called when we need to redirect the user to the IdP for authentication

        // Build the authorization URL
        var authorizationUrl = BuildAuthorizationUrl(properties);

        Logger.LogInformation("Redirecting to authorization endpoint: {Url}", authorizationUrl);

        // Redirect to the IdP
        Response.Redirect(authorizationUrl);

        return Task.CompletedTask;
    }

    private string BuildAuthorizationUrl(AuthenticationProperties properties)
    {
        var options = Options;

        // Build the callback URL (where the IdP will redirect back to)
        // For development, ensure we use HTTP if that's what we want
        var callbackUrl = BuildRedirectUri(options.CallbackPath);

        // Generate state parameter for CSRF protection
        var state = Options.StateDataFormat?.Protect(properties) ?? Guid.NewGuid().ToString();

        // Determine the authorization endpoint
        var authorizationEndpoint = $"{options.Authority?.TrimEnd('/')}/{options.AuthorizationEndpoint?.TrimStart('/') ?? "connect/authorize"}";

        // Build authorization URL with required parameters
        var authorizationUrl = $"{authorizationEndpoint}" +
            $"?client_id={Uri.EscapeDataString(options.ClientId ?? "")}" +
            $"&response_type={Uri.EscapeDataString(options.ResponseType)}" +
            $"&scope={Uri.EscapeDataString(options.Scope)}" +
            $"&redirect_uri={Uri.EscapeDataString(callbackUrl)}" +
            $"&state={Uri.EscapeDataString(state)}";

        return authorizationUrl;
    }

    private string BuildCallbackUrl(string callbackPath)
    {
        var request = Request;
        var scheme = request.Scheme;
        var host = request.Host;

        var baseUrl = $"{scheme}://{host}";

        // Ensure callback path starts with /
        if (!callbackPath.StartsWith('/'))
        {
            callbackPath = "/" + callbackPath;
        }

        var callbackUrl = $"{baseUrl}{callbackPath}";

        return callbackUrl;
    }
}
