using Microsoft.AspNetCore.Authentication;
using MinimalEndpoints.Abstractions;

namespace IdentityProvider.Bff.Service.Endpoints.Auth;

public class LogoutEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync();
            return Results.Ok(new { Message = "Logged out successfully" });
        })
        .WithTags("Authentication");
    }
}