using IdentityProvider.Bff.Endpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        services.AddAuthEndpoints();

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