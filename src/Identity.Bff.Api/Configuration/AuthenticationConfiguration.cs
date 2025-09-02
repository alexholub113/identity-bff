namespace Identity.Bff.Api.Configuration;

public class AuthenticationConfiguration
{
    public const string SectionName = "Authentication";

    /// <summary>
    /// Gets or sets the Authority to use when making OpenIdConnect calls.
    /// </summary>
    public string Authority { get; set; } = string.Empty;

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
    /// This is where the external identity provider will redirect back to after authentication.
    /// Should match the redirect URI configured in your identity provider.
    /// Default: "/auth/callback" (more explicit than the standard "/signin-oidc")
    /// </summary>
    public string CallbackPath { get; set; } = "/auth/callback";

    /// <summary>
    /// Gets or sets whether to require HTTPS metadata.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to use the experimental authentication logic.
    /// </summary>
    public bool UseExperimentalLogic { get; set; } = false;
}
