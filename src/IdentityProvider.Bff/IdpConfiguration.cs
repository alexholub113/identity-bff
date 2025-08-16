namespace IdentityProvider.Bff;

public class IdpConfiguration
{
    public const string SectionName = "IdP";

    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string[] Scopes { get; set; } = ["openid", "profile"];
    public string CallbackPath { get; set; } = "/signin-oidc";
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";
    public bool RequireHttpsMetadata { get; set; } = true;

    public bool UseCustomAuthenticationHandler { get; set; } = false;
}