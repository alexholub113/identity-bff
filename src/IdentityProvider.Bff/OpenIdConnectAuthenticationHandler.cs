using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityProvider.Bff;

internal class OpenIdConnectAuthenticationHandler(IOptionsMonitor<OpenIdConnectAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IDataProtectionProvider dataProtection, IHttpClientFactory httpClientFactory) : RemoteAuthenticationHandler<OpenIdConnectAuthenticationOptions>(options, logger, encoder)
{
    private readonly IDataProtectionProvider _dataProtection = dataProtection;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

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

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        // This is called when the IdP redirects back to our callback URL
        var query = Request.Query;

        // Check for error response from IdP
        if (query.ContainsKey("error"))
        {
            var error = query["error"].ToString();
            var errorDescription = query["error_description"].ToString();
            Logger.LogWarning("Authentication error: {Error} - {Description}", error, errorDescription);

            return HandleRequestResult.Fail($"Authentication failed: {error}");
        }

        // Check for authorization code
        if (!query.ContainsKey("code"))
        {
            Logger.LogWarning("No authorization code received");
            return HandleRequestResult.Fail("No authorization code received");
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
                    return HandleRequestResult.Fail("Invalid state parameter");
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to unprotect state parameter");
                return HandleRequestResult.Fail("Invalid state parameter");
            }
        }

        // Exchange authorization code for tokens
        try
        {
            var tokenResponse = await ExchangeCodeForTokensAsync(authorizationCode, originalProperties);
            if (tokenResponse == null)
            {
                Logger.LogWarning("Failed to exchange authorization code for tokens");
                return HandleRequestResult.Fail("Failed to exchange authorization code for tokens");
            }

            // Extract claims from the ID token
            var claims = ExtractClaimsFromIdToken(tokenResponse.IdToken);
            if (claims == null || !claims.Any())
            {
                Logger.LogWarning("Failed to extract claims from ID token");
                return HandleRequestResult.Fail("Failed to extract claims from ID token");
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);

            // Store tokens in authentication properties for later use
            var authenticationProperties = new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                RedirectUri = originalProperties?.RedirectUri ?? "/"
            };

            // Store tokens securely in authentication properties
            authenticationProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = tokenResponse.AccessToken },
                new AuthenticationToken { Name = "id_token", Value = tokenResponse.IdToken },
                new AuthenticationToken { Name = "token_type", Value = tokenResponse.TokenType },
                new AuthenticationToken { Name = "expires_at", Value = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o") }
            });

            var ticket = new AuthenticationTicket(principal, authenticationProperties, Scheme.Name);

            Logger.LogInformation("Authentication successful for authorization code: {Code}, user: {UserId}",
                authorizationCode, claims.FirstOrDefault(c => c.Type == "sub")?.Value);

            return HandleRequestResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during token exchange");
            return HandleRequestResult.Fail("Authentication failed");
        }
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

    private async Task<TokenResponse?> ExchangeCodeForTokensAsync(string authorizationCode, AuthenticationProperties? originalProperties)
    {
        try
        {
            var options = Options;
            var tokenEndpoint = $"{options.Authority?.TrimEnd('/')}/{options.TokenEndpoint?.TrimStart('/') ?? "connect/token"}";
            var callbackUrl = BuildRedirectUri(options.CallbackPath);

            var httpClient = _httpClientFactory.CreateClient();

            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = authorizationCode,
                ["redirect_uri"] = callbackUrl,
                ["client_id"] = options.ClientId ?? ""
            };

            // Add client secret if configured
            if (!string.IsNullOrEmpty(options.ClientSecret))
            {
                tokenRequest["client_secret"] = options.ClientSecret;
            }

            var content = new FormUrlEncodedContent(tokenRequest);

            Logger.LogDebug("Exchanging authorization code for tokens at: {TokenEndpoint}", tokenEndpoint);

            var response = await httpClient.PostAsync(tokenEndpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Logger.LogWarning("Token exchange failed with status {StatusCode}: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Logger.LogDebug("Token exchange successful");
            return tokenResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception during token exchange");
            return null;
        }
    }

    private List<Claim>? ExtractClaimsFromIdToken(string idToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // For demo purposes, we'll read the token without validation
            // In production, you should validate the signature, issuer, audience, etc.
            var jwt = tokenHandler.ReadJwtToken(idToken);

            var claims = new List<Claim>();

            // Extract standard OIDC claims
            foreach (var claim in jwt.Claims)
            {
                // Map some claims to standard ClaimTypes
                var claimType = claim.Type switch
                {
                    "sub" => ClaimTypes.NameIdentifier,
                    "email" => ClaimTypes.Email,
                    "name" => ClaimTypes.Name,
                    "given_name" => ClaimTypes.GivenName,
                    "family_name" => ClaimTypes.Surname,
                    _ => claim.Type
                };

                claims.Add(new Claim(claimType, claim.Value));

                // Also keep the original claim type for OIDC compatibility
                if (claimType != claim.Type)
                {
                    claims.Add(new Claim(claim.Type, claim.Value));
                }
            }

            Logger.LogDebug("Extracted {ClaimCount} claims from ID token", claims.Count);
            return claims;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to extract claims from ID token");
            return null;
        }
    }

    private class TokenResponse
    {
        public string AccessToken { get; set; } = "";
        public string TokenType { get; set; } = "";
        public int ExpiresIn { get; set; }
        public string IdToken { get; set; } = "";
        public string? Scope { get; set; }
    }
}
