using Microsoft.AspNetCore.Authorization;
using MinimalEndpoints.Abstractions;

namespace IdentityProvider.Bff.Service.Endpoints.Auth;

public class ProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/profile", [Authorize] (HttpContext context) =>
        {
            var user = context.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                return Results.Unauthorized();
            }

            var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();

            return Results.Ok(new
            {
                user.Identity?.IsAuthenticated,
                user.Identity?.Name,
                Claims = claims,
                user.Identity?.AuthenticationType
            });
        })
        .WithTags("Authentication")
        .RequireAuthorization();
    }
}
