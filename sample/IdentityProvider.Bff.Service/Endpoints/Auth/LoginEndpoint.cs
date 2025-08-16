using Microsoft.AspNetCore.Authentication;
using MinimalEndpoints.Abstractions;

namespace IdentityProvider.Bff.Service.Endpoints.Auth;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/login", async (HttpContext context) =>
        {
            // This will trigger the authentication challenge and redirect to your IdP
            await context.ChallengeAsync("oidc");
        })
        .WithTags("Authentication");
    }
}
