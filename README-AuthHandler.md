# OpenID Connect Authentication Handler - Minimal Implementation

This is a minimal implementation of an OpenID Connect authentication handler for learning purposes. 

## What's Implemented

### 1. Basic OpenID Connect Flow
- **Challenge**: Redirects users to the IdP's authorization endpoint
- **Callback Handling**: Processes the authorization code when the IdP redirects back
- **State Protection**: Basic CSRF protection using encrypted state parameters

### 2. Current Limitations (TODOs for learning)
This is a minimal implementation. Here's what's missing that you can implement step by step:

- **Token Exchange**: Currently doesn't exchange authorization code for tokens
- **Token Validation**: No JWT validation (signature, expiration, issuer, audience)
- **Real Claims**: Uses hardcoded claims instead of extracting from ID token
- **Userinfo Endpoint**: Doesn't call the userinfo endpoint for additional claims
- **Refresh Tokens**: No token refresh mechanism
- **Discovery Document**: Doesn't use OpenID Connect discovery document

## How to Test

### 1. Start Your IdP Server
Make sure your IdentityProvider.Server.Api is running on `https://localhost:5001`

### 2. Start the BFF Service
```bash
cd sample/IdentityProvider.Bff.Service
dotnet run
```

### 3. Test the Authentication Flow

1. **Check Initial Status**
   ```
   GET https://localhost:7001/auth/status
   ```
   Should return `IsAuthenticated: false`

2. **Trigger Login**
   ```
   GET https://localhost:7001/auth/login
   ```
   This will:
   - Redirect you to your IdP at `https://localhost:5001/connect/authorize`
   - Your IdP will redirect to its login page
   - After login, IdP redirects back to `https://localhost:7001/signin-oidc`
   - Our handler processes the callback and creates authentication

3. **Check Protected Endpoint**
   ```
   GET https://localhost:7001/auth/profile
   ```
   Should return user information and claims

## Configuration

In `Program.cs`:

```csharp
builder.Services.AddRemoteAuthentication("oidc", "OpenID Connect", options =>
{
    options.Authority = "https://localhost:5001"; // Your IdP URL
    options.ClientId = "bff-client";
    options.ClientSecret = "secret";
    options.CallbackPath = "/signin-oidc"; // Where IdP redirects back
}, useCustomHandler: true);
```

## Next Steps for Learning

1. **Implement Token Exchange**: 
   - Call the token endpoint with the authorization code
   - Get access_token, id_token, and optionally refresh_token

2. **Add JWT Validation**:
   - Validate JWT signature using IdP's public keys
   - Validate issuer, audience, expiration

3. **Extract Real Claims**:
   - Parse claims from the ID token
   - Optionally call userinfo endpoint for additional claims

4. **Add Discovery Support**:
   - Fetch IdP configuration from `/.well-known/openid_configuration`
   - Use discovered endpoints instead of hardcoded URLs

5. **Implement Logout**:
   - Support RP-initiated logout
   - Handle logout callbacks from IdP

## File Structure

- `OpenIdConnectAuthenticationHandler.cs` - Main authentication handler
- `OpenIdConnectAuthenticationOptions.cs` - Configuration options
- `ServiceCollectionExtensions.cs` - DI registration
- `AuthTestEndpoint.cs` - Test endpoints for authentication flow
