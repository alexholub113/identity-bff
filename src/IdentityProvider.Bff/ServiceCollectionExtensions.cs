using IdentityProvider.Bff.Endpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        // Add HTTP client factory for token exchange
        services.AddHttpClient();

        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // Use the provided scheme name for challenge when using custom handler
            options.DefaultChallengeScheme = useCustomHandler ? authenticationScheme : OpenIdConnectDefaults.AuthenticationScheme;
        });
        authenticationBuilder.AddCookie();
        services.AddAuthEndpoints();

        if (useCustomHandler)
        {
            authenticationBuilder.AddScheme<OpenIdConnectAuthenticationOptions, OpenIdConnectAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
        else
        {
            var openIdConnectAuthenticationOptions = new OpenIdConnectAuthenticationOptions();
            configureOptions?.Invoke(openIdConnectAuthenticationOptions);
            authenticationBuilder.AddOpenIdConnect(authenticationScheme, displayName, options =>
            {
                // Basic OIDC configuration
                options.Authority = openIdConnectAuthenticationOptions.Authority;
                options.ClientId = openIdConnectAuthenticationOptions.ClientId;
                options.ClientSecret = openIdConnectAuthenticationOptions.ClientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = openIdConnectAuthenticationOptions.ResponseType;

                // Scopes
                options.Scope.Clear();
                if (!string.IsNullOrEmpty(openIdConnectAuthenticationOptions.Scope))
                {
                    foreach (var scope in openIdConnectAuthenticationOptions.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        options.Scope.Add(scope);
                    }
                }
                else
                {
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                }

                // Security settings
                options.RequireHttpsMetadata = openIdConnectAuthenticationOptions.RequireHttpsMetadata;

                // Callback paths
                if (!string.IsNullOrEmpty(openIdConnectAuthenticationOptions.CallbackPath))
                    options.CallbackPath = openIdConnectAuthenticationOptions.CallbackPath;
                else
                    options.CallbackPath = "/signin-oidc";

                options.SignedOutCallbackPath = "/signout-callback-oidc";

                // Token validation
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
            });
        }

        return services;
    }

    /// <summary>
    /// Maps all registered auth endpoints to the application.
    /// </summary>
    /// <param name="app">The web application to map endpoints to.</param>
    /// <param name="routeGroupBuilder">Optional route group builder for organizing endpoints under a common prefix.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseAuthEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IAuthEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IAuthEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IAuthEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    private static IServiceCollection AddAuthEndpoints(this IServiceCollection services)
    {
        ServiceDescriptor[] serviceDescriptors = typeof(ServiceCollectionExtensions)
            .Assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IAuthEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IAuthEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }
}