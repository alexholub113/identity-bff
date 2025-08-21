using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IdentityProvider.Bff.Endpoints;

public class ProfileEndpoint : IAuthEndpoint
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
