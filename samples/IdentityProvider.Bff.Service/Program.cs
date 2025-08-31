using IdentityProvider.Bff;
using IdentityProvider.Bff.Service.Configuration;
using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddAuthorization();

// Add session support for state management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Add CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "https://localhost:3000") // Frontend URLs
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Important for cookie-based authentication
    });
});

// Configure authentication from appsettings.json
var authConfig = builder.Configuration.GetSection(AuthenticationConfiguration.SectionName)
    .Get<AuthenticationConfiguration>();

if (authConfig == null)
{
    throw new InvalidOperationException($"Authentication configuration section '{AuthenticationConfiguration.SectionName}' is missing or invalid.");
}

// Validate required configuration
if (string.IsNullOrEmpty(authConfig.Authority))
    throw new InvalidOperationException("Authentication:Authority is required.");
if (string.IsNullOrEmpty(authConfig.ClientId))
    throw new InvalidOperationException("Authentication:ClientId is required.");
if (string.IsNullOrEmpty(authConfig.ClientSecret))
    throw new InvalidOperationException("Authentication:ClientSecret is required.");

builder.Services.AddRemoteAuthentication(
    authenticationScheme: authConfig.Scheme,
    displayName: authConfig.DisplayName,
    options =>
    {
        options.Authority = authConfig.Authority;
        options.ClientId = authConfig.ClientId;
        options.ClientSecret = authConfig.ClientSecret;
        options.ResponseType = authConfig.ResponseType;
        options.Scope = authConfig.Scope;
        options.CallbackPath = authConfig.CallbackPath;
        options.RequireHttpsMetadata = authConfig.RequireHttpsMetadata;
    },
    useCustomHandler: authConfig.UseCustomHandler);

WebApplication app = builder.Build();

app.UseAuthEndpoints();
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Add security headers for production
    app.Use(async (context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";
        await next();
    });
}

// Use session before authentication
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Use CORS
app.UseCors("AllowFrontend");

app.Run();