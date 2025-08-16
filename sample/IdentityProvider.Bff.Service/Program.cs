using Asp.Versioning;
using Asp.Versioning.Builder;
using IdentityProvider.Bff;
using IdentityProvider.Bff.Service.Configuration;
using MinimalEndpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader(),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Identity Provider BFF API Sample",
        Version = "v1",
    });
});

builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.Services.AddAuthorization();

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

        // Set custom endpoints if specified
        if (!string.IsNullOrEmpty(authConfig.AuthorizationEndpoint))
            options.AuthorizationEndpoint = authConfig.AuthorizationEndpoint;
        if (!string.IsNullOrEmpty(authConfig.TokenEndpoint))
            options.TokenEndpoint = authConfig.TokenEndpoint;
        if (!string.IsNullOrEmpty(authConfig.UserInfoEndpoint))
            options.UserInfoEndpoint = authConfig.UserInfoEndpoint;
    },
    useCustomHandler: authConfig.UseCustomHandler);

WebApplication app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();