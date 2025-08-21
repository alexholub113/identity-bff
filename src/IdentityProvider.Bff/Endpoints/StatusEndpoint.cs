using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IdentityProvider.Bff.Endpoints;

public class StatusEndpoint : IAuthEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/status", (HttpContext context) =>
        {
            var user = context.User;

            return Results.Ok(new
            {
                IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
                user.Identity?.Name,
                user.Identity?.AuthenticationType,
                ClaimsCount = user.Claims.Count()
            });
        })
        .WithTags("Authentication");
    }
}
