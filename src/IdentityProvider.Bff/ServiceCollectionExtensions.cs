using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IdentityProvider.Bff;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRemoteAuthentication(
        this IServiceCollection services,
        string authenticationScheme,
        string? displayName,
        Action<OpenIdConnectAuthenticationOptions> configureOptions,
        bool useCustomHandler = false)
    {
        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // Use the provided scheme name for challenge when using custom handler
            options.DefaultChallengeScheme = useCustomHandler ? authenticationScheme : OpenIdConnectDefaults.AuthenticationScheme;
        });
        authenticationBuilder.AddCookie();

        if (useCustomHandler)
        {
            authenticationBuilder.AddScheme<OpenIdConnectAuthenticationOptions, OpenIdConnectAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
        else
        {
            var openIdConnectAuthenticationOptions = new OpenIdConnectAuthenticationOptions();
            configureOptions?.Invoke(openIdConnectAuthenticationOptions);
            authenticationBuilder.AddOpenIdConnect(options =>
            {
                options.Authority = openIdConnectAuthenticationOptions.Authority;
                options.ClientId = openIdConnectAuthenticationOptions.ClientId;
                options.ClientSecret = openIdConnectAuthenticationOptions.ClientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.Code;
            });
        }

        return services;
    }
}