using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Identity.Bff.Api.Experimental;

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

        AuthenticationProperties? originalProperties = null;

        // Validate state parameter (basic CSRF protection)
        if (!string.IsNullOrEmpty(state) && Options.StateDataFormat != null)
        {
            try
            {
                originalProperties = Options.StateDataFormat.Unprotect(state);
                if (originalProperties == null)
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
            new Claim(ClaimTypes.NameIdentifier, "alexholub"), // This would come from the token
            new Claim(ClaimTypes.Name, "Alex Holub"),
            new Claim(ClaimTypes.Email, "alexholub@example.com"),
            new Claim("sub", "alexholub123")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        // Preserve the original redirect URI from the authentication properties
        var authenticationProperties = new AuthenticationProperties
        {
            IssuedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
            RedirectUri = originalProperties?.RedirectUri ?? "/" // Use the original return URL or default to home
        };

        var ticket = new AuthenticationTicket(principal, authenticationProperties, Scheme.Name);

        Logger.LogInformation("Authentication successful for authorization code: {Code}, redirecting to: {RedirectUri}",
            authorizationCode, authenticationProperties.RedirectUri);

        return Task.FromResult(HandleRequestResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        // This is called when we need to redirect the user to the IdP for authentication

        // Instead of redirecting to a URL, we need to submit a form to the authorization endpoint
        var formHtml = BuildAuthorizationForm(properties);

        Logger.LogInformation("Submitting form to authorization endpoint");

        // Return an HTML form that auto-submits to the authorization endpoint
        Response.ContentType = "text/html";
        return Response.WriteAsync(formHtml);
    }

    private string BuildAuthorizationForm(AuthenticationProperties properties)
    {
        var options = Options;

        // Build the callback URL (where the IdP will redirect back to)
        var callbackUrl = BuildRedirectUri(options.CallbackPath);

        // Generate state parameter for CSRF protection
        var state = Options.StateDataFormat?.Protect(properties) ?? Guid.NewGuid().ToString();

        // Determine the authorization endpoint
        var authorizationEndpoint = $"{options.Authority?.TrimEnd('/')}/{options.AuthorizationEndpoint?.TrimStart('/') ?? "connect/authorize"}";

        // Build HTML form with auto-submit
        var formHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Redirecting to Identity Provider</title>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            display: flex; 
            justify-content: center; 
            align-items: center; 
            height: 100vh; 
            margin: 0; 
            background-color: #f5f5f5; 
        }}
        .container {{ 
            text-align: center; 
            padding: 20px; 
            background: white; 
            border-radius: 8px; 
            box-shadow: 0 2px 10px rgba(0,0,0,0.1); 
        }}
        .spinner {{ 
            border: 4px solid #f3f3f3; 
            border-top: 4px solid #3498db; 
            border-radius: 50%; 
            width: 40px; 
            height: 40px; 
            animation: spin 2s linear infinite; 
            margin: 20px auto; 
        }}
        @keyframes spin {{ 
            0% {{ transform: rotate(0deg); }} 
            100% {{ transform: rotate(360deg); }} 
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Redirecting to Identity Provider</h2>
        <div class=""spinner""></div>
        <p>Please wait while we redirect you to the authentication provider...</p>
        <noscript>
            <p><strong>JavaScript is disabled.</strong> Please click the button below to continue:</p>
            <input type=""submit"" value=""Continue to Identity Provider"" />
        </noscript>
    </div>
    
    <form id=""authForm"" method=""post"" action=""{authorizationEndpoint}"">
        <input type=""hidden"" name=""client_id"" value=""{System.Net.WebUtility.HtmlEncode(options.ClientId ?? "")}"" />
        <input type=""hidden"" name=""response_type"" value=""{System.Net.WebUtility.HtmlEncode(options.ResponseType)}"" />
        <input type=""hidden"" name=""scope"" value=""{System.Net.WebUtility.HtmlEncode(options.Scope)}"" />
        <input type=""hidden"" name=""redirect_uri"" value=""{System.Net.WebUtility.HtmlEncode(callbackUrl)}"" />
        <input type=""hidden"" name=""state"" value=""{System.Net.WebUtility.HtmlEncode(state)}"" />
    </form>
    
    <script>
        // Auto-submit the form after a short delay
        setTimeout(function() {{
            document.getElementById('authForm').submit();
        }}, 1000);
    </script>
</body>
</html>";

        return formHtml;
    }
}
