using Microsoft.AspNetCore.Routing;

namespace IdentityProvider.Bff.Endpoints;

/// <summary>
/// Defines a contract for minimal API endpoints that can be automatically discovered and registered.
/// </summary>
internal interface IAuthEndpoint
{
    /// <summary>
    /// Maps the endpoint to the specified route builder with the desired HTTP method, route pattern, and configuration.
    /// </summary>
    /// <param name="app">The endpoint route builder to map the endpoint to. This can be a <see cref="Microsoft.AspNetCore.Builder.WebApplication"/> or a <see cref="Microsoft.AspNetCore.Routing.RouteGroupBuilder"/>.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}