namespace IdentityProvider.Bff.Service.Configuration;

public class AuthenticationConfiguration
{
    public const string SectionName = "Authentication";

    /// <summary>
    /// Gets or sets the Authority to use when making OpenIdConnect calls.
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authorization endpoint URL. If not specified, will use Authority + "/connect/authorize".
    /// </summary>
    public string? AuthorizationEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the token endpoint URL. If not specified, will use Authority + "/connect/token".
    /// </summary>
    public string? TokenEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the userinfo endpoint URL. If not specified, will use Authority + "/connect/userinfo".
    /// </summary>
    public string? UserInfoEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the 'client_id'.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the 'client_secret'.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the response type for the authorization request.
    /// </summary>
    public string ResponseType { get; set; } = "code";

    /// <summary>
    /// Gets or sets the space-separated list of scopes to request.
    /// </summary>
    public string Scope { get; set; } = "openid profile";

    /// <summary>
    /// Gets or sets the redirect path for sign-in callback.
    /// </summary>
    public string CallbackPath { get; set; } = "/signin-oidc";

    /// <summary>
    /// Gets or sets whether to require HTTPS metadata.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to use the custom authentication handler.
    /// </summary>
    public bool UseCustomHandler { get; set; } = true;

    /// <summary>
    /// Gets or sets the authentication scheme name.
    /// </summary>
    public string Scheme { get; set; } = "oidc";

    /// <summary>
    /// Gets or sets the display name for the authentication scheme.
    /// </summary>
    public string DisplayName { get; set; } = "OpenID Connect";
}
