namespace Identity.Bff.Api.Configuration;

public class CorsConfiguration
{
    public const string SectionName = "Cors";

    /// <summary>
    /// Allowed origins for CORS requests
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether to allow credentials in CORS requests
    /// </summary>
    public bool AllowCredentials { get; set; } = true;
}