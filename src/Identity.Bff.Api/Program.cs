using Identity.Bff.Api.Configuration;
using Identity.Bff.Api.Experimental;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Configure authentication from appsettings.json
var authConfig = builder.Configuration.GetSection(AuthenticationConfiguration.SectionName)
    .Get<AuthenticationConfiguration>() ?? throw new InvalidOperationException($"Authentication configuration section '{AuthenticationConfiguration.SectionName}' is missing or invalid.");

// Configure CORS from appsettings.json
var corsConfig = builder.Configuration.GetSection(CorsConfiguration.SectionName)
    .Get<CorsConfiguration>() ?? throw new InvalidOperationException($"CORS configuration section '{CorsConfiguration.SectionName}' is missing or invalid.");

// Add CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (corsConfig.AllowedOrigins?.Length > 0)
        {
            policy.WithOrigins(corsConfig.AllowedOrigins);
        }
        else
        {
            policy.AllowAnyOrigin();
        }

        policy.AllowAnyMethod()
              .AllowAnyHeader();

        if (corsConfig.AllowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

builder.Services.AddHttpClient();

var authenticationBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // Use the provided scheme name for challenge when using custom handler
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
});
authenticationBuilder.AddCookie();

if (authConfig.UseExperimentalLogic)
{
    authenticationBuilder.AddScheme<OpenIdConnectAuthenticationOptions, OpenIdConnectAuthenticationHandler>(
        OpenIdConnectDefaults.AuthenticationScheme,
        "OpenID Connect",
        options =>
        {
            options.Authority = authConfig.Authority;
            options.ClientId = authConfig.ClientId;
            options.ClientSecret = authConfig.ClientSecret;
            options.ResponseType = authConfig.ResponseType;
            options.Scope = authConfig.Scope;
            options.CallbackPath = authConfig.CallbackPath;
            options.RequireHttpsMetadata = authConfig.RequireHttpsMetadata;
            options.CallbackPath = authConfig.CallbackPath;
            options.SaveTokens = true;
        });
}
else
{
    authenticationBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, "OpenID Connect", options =>
    {
        // Basic OIDC configuration
        options.Authority = authConfig.Authority;
        options.ClientId = authConfig.ClientId;
        options.ClientSecret = authConfig.ClientSecret;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.ResponseType = authConfig.ResponseType;

        // Scopes
        options.Scope.Clear();
        if (!string.IsNullOrEmpty(authConfig.Scope))
        {
            foreach (var scope in authConfig.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                options.Scope.Add(scope);
            }
        }
        else
        {
            options.Scope.Add("openid");
            options.Scope.Add("profile");
        }

        options.RequireHttpsMetadata = authConfig.RequireHttpsMetadata;
        options.CallbackPath = authConfig.CallbackPath;
        options.SignedOutCallbackPath = "/signout-callback-oidc";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
    });
}

WebApplication app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use session before authentication
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Use CORS
app.UseCors("AllowFrontend");

app.Run();