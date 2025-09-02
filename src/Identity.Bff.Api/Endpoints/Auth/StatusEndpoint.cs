using MinimalEndpoints.Abstractions;

namespace Identity.Bff.Api.Endpoints.Auth;

public class StatusEndpoint : IEndpoint
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
