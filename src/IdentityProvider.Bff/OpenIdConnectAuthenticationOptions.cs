using Microsoft.AspNetCore.Authentication;

namespace IdentityProvider.Bff;

public class OpenIdConnectAuthenticationOptions : RemoteAuthenticationOptions
{
    /// <summary>
    /// Gets or sets the Authority to use when making OpenIdConnect calls.
    /// </summary>
    public string? Authority { get; set; }

    /// <summary>
    /// Gets or sets the authorization endpoint URL. If not specified, will use Authority + "/connect/authorize".
    /// </summary>
    public string? AuthorizationEndpoint { get; set; } = "/connect/authorize";

    /// <summary>
    /// Gets or sets the token endpoint URL. If not specified, will use Authority + "/connect/token".
    /// </summary>
    public string? TokenEndpoint { get; set; } = "/connect/token";

    /// <summary>
    /// Gets or sets the userinfo endpoint URL. If not specified, will use Authority + "/connect/userinfo".
    /// </summary>
    public string? UserInfoEndpoint { get; set; } = "/connect/userinfo";

    /// <summary>
    /// Gets or sets the JWKS URI. If not specified, will use Authority + "/.well-known/jwks".
    /// </summary>
    public string? JwksUri { get; set; } = "/.well-known/jwks";

    /// <summary>
    /// Gets or sets the 'client_id'.
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the 'client_secret'.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the response type for the authorization request.
    /// </summary>
    public string ResponseType { get; set; } = "code";

    /// <summary>
    /// Gets or sets the space-separated list of scopes to request.
    /// </summary>
    public string Scope { get; set; } = "openid profile";

    /// <summary>
    /// Gets or sets whether to require HTTPS metadata.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// Gets or sets the redirect path for sign-in.
    /// </summary>
    public string SignInPath { get; set; } = "/signin-oidc";

    /// <summary>
    /// Gets or sets the redirect path for sign-out.
    /// </summary>
    public string SignOutPath { get; set; } = "/signout-oidc";

    /// <summary>
    /// Gets or sets the data format used to protect state data.
    /// </summary>
    public ISecureDataFormat<AuthenticationProperties>? StateDataFormat { get; set; }
}
