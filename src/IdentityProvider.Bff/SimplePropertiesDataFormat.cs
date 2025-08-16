using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace IdentityProvider.Bff;

/// <summary>
/// Simple data format for protecting authentication properties
/// </summary>
internal class SimplePropertiesDataFormat(IDataProtector dataProtector) : ISecureDataFormat<AuthenticationProperties>
{
    private readonly IDataProtector _dataProtector = dataProtector;

    public string Protect(AuthenticationProperties data)
    {
        var json = JsonSerializer.Serialize(data.Items);
        var protectedData = _dataProtector.Protect(json);
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(protectedData));
    }

    public string Protect(AuthenticationProperties data, string? purpose)
    {
        return Protect(data);
    }

    public AuthenticationProperties? Unprotect(string? protectedText)
    {
        return Unprotect(protectedText, null);
    }

    public AuthenticationProperties? Unprotect(string? protectedText, string? purpose)
    {
        if (string.IsNullOrEmpty(protectedText))
            return null;

        try
        {
            var bytes = Convert.FromBase64String(protectedText);
            var protectedData = System.Text.Encoding.UTF8.GetString(bytes);
            var json = _dataProtector.Unprotect(protectedData);
            var items = JsonSerializer.Deserialize<Dictionary<string, string?>>(json);

            var properties = new AuthenticationProperties();
            if (items != null)
            {
                foreach (var item in items)
                {
                    properties.Items[item.Key] = item.Value;
                }
            }

            return properties;
        }
        catch
        {
            return null;
        }
    }
}
