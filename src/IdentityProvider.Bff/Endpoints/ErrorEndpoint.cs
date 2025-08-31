using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace IdentityProvider.Bff.Endpoints;

public class ErrorEndpoint : IAuthEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/error", (HttpContext context, string? message = null, ILogger<ErrorEndpoint> logger = null!) =>
        {
            var errorMessage = message ?? "An authentication error occurred.";

            // Log the error for debugging purposes
            logger?.LogError("Authentication error: {ErrorMessage}", errorMessage);

            // For API requests, return JSON
            if (IsApiRequest(context))
            {
                return Results.Problem(
                    title: "Authentication Error",
                    detail: errorMessage,
                    statusCode: 401
                );
            }

            // For browser requests, return HTML with error information
            var html = GenerateErrorPageHtml(errorMessage);
            return Results.Content(html, "text/html");
        })
        .WithTags("Authentication")
        .WithSummary("Handle authentication errors")
        .WithDescription("Displays authentication error information for both API and browser requests.");
    }

    private static bool IsApiRequest(HttpContext context)
    {
        // Check if the request accepts JSON or if it's explicitly an API call
        var acceptHeader = context.Request.Headers.Accept.ToString();
        return acceptHeader.Contains("application/json") ||
               context.Request.Path.StartsWithSegments("/api") ||
               context.Request.Headers.ContainsKey("X-Requested-With");
    }

    private static string GenerateErrorPageHtml(string errorMessage)
    {
        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Authentication Error</title>
                <style>
                    body {
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                        max-width: 600px;
                        margin: 100px auto;
                        padding: 20px;
                        background-color: #f5f5f5;
                    }
                    .error-container {
                        background: white;
                        padding: 40px;
                        border-radius: 8px;
                        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                        text-align: center;
                    }
                    .error-icon {
                        font-size: 48px;
                        color: #dc3545;
                        margin-bottom: 20px;
                    }
                    h1 {
                        color: #dc3545;
                        margin-bottom: 20px;
                    }
                    .error-message {
                        background-color: #f8d7da;
                        border: 1px solid #f5c6cb;
                        color: #721c24;
                        padding: 15px;
                        border-radius: 4px;
                        margin: 20px 0;
                        word-break: break-word;
                    }
                    .actions {
                        margin-top: 30px;
                    }
                    .btn {
                        display: inline-block;
                        padding: 10px 20px;
                        margin: 0 10px;
                        text-decoration: none;
                        border-radius: 4px;
                        font-weight: 500;
                        transition: background-color 0.2s;
                    }
                    .btn-primary {
                        background-color: #007bff;
                        color: white;
                    }
                    .btn-primary:hover {
                        background-color: #0056b3;
                    }
                    .btn-secondary {
                        background-color: #6c757d;
                        color: white;
                    }
                    .btn-secondary:hover {
                        background-color: #545b62;
                    }
                    .timestamp {
                        color: #6c757d;
                        font-size: 0.9em;
                        margin-top: 20px;
                    }
                </style>
            </head>
            <body>
                <div class="error-container">
                    <div class="error-icon">⚠️</div>
                    <h1>Authentication Error</h1>
                    <p>We encountered an issue while trying to authenticate you.</p>
                    
                    <div class="error-message">
                        {{System.Net.WebUtility.HtmlEncode(errorMessage)}}
                    </div>
                    
                    <div class="timestamp">
                        Error occurred at: {{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}} UTC
                    </div>
                </div>
                
                <script>
                    // Auto-refresh after 5 minutes in case it's a temporary issue
                    setTimeout(() => {
                        if (confirm('This error page will refresh automatically. Click OK to refresh now, or Cancel to stay on this page.')) {
                            window.location.reload();
                        }
                    }, 300000); // 5 minutes
                </script>
            </body>
            </html>
            """;
    }
}
