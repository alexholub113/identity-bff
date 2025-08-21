# Authentication Security Best Practices

This document outlines the security best practices implemented in the IdentityProvider.Bff authentication endpoints.

## 🛡️ Security Features Implemented

### 1. CSRF Protection
- **State Parameter**: Each login request generates a cryptographically secure random state parameter
- **Session Validation**: State is stored in server-side session and validated on callback
- **Secure Random Generation**: Uses `RandomNumberGenerator` for cryptographically secure randomness

### 2. Open Redirect Protection
- **URL Validation**: All return URLs are validated before use
- **Allowlist Approach**: Only specific hosts are allowed for external redirects
- **Relative URL Support**: Supports safe relative URLs (starting with `/` but not `//`)
- **Default Fallback**: Invalid URLs default to safe home page (`/`)

### 3. Session Security
- **HTTP-Only Cookies**: Session cookies are not accessible to JavaScript
- **SameSite Protection**: Uses `SameSite=Lax` to prevent CSRF attacks
- **Secure Policy**: Adapts to request scheme (HTTP/HTTPS)
- **Timeout**: Sessions expire after 30 minutes of inactivity

### 4. CORS Configuration
- **Explicit Origins**: Only specific frontend origins are allowed
- **Credentials Support**: Properly configured for cookie-based authentication
- **Method Restrictions**: Could be limited to specific methods if needed

### 5. Security Headers (Production)
- **X-Content-Type-Options**: Prevents MIME type sniffing
- **X-Frame-Options**: Prevents clickjacking attacks
- **X-XSS-Protection**: Enables browser XSS protection
- **Referrer-Policy**: Controls referrer information leakage
- **Content-Security-Policy**: Basic CSP to prevent XSS

### 6. Federated Logout
- **Local Logout**: Signs out from local application
- **IdP Logout**: Also signs out from external identity provider
- **Return URL Validation**: Validates logout return URLs

## 🔒 OAuth2/OIDC Best Practices

### Authorization Code Flow
- ✅ Uses Authorization Code flow (most secure for server-side apps)
- ✅ Includes `state` parameter for CSRF protection
- ✅ Uses `code` response type
- ✅ Supports PKCE (if configured in IdP)

### Scope Management
- ✅ Requests minimal required scopes (`openid profile email`)
- ✅ Configurable scope requirements
- ✅ Proper scope validation

### Token Handling
- ✅ Tokens stored server-side only
- ✅ No tokens exposed to frontend JavaScript
- ✅ HTTP-only cookies for session management
- ✅ Automatic token refresh handled by middleware

### Callback Security
- ✅ Uses ASP.NET Core's built-in OIDC middleware for callbacks
- ✅ Automatic state validation
- ✅ Proper error handling
- ✅ No custom callback implementation (reduces attack surface)

## 🎯 BFF Pattern Compliance

### Backend-for-Frontend Principles
- ✅ **Token Isolation**: Access tokens never reach the frontend
- ✅ **Session-Based**: Uses session cookies instead of bearer tokens
- ✅ **Proxy Pattern**: API acts as proxy to external services
- ✅ **Single Domain**: All authentication handled on same domain

### Security Benefits
- ✅ **No Token Storage**: Frontend never stores sensitive tokens
- ✅ **Automatic Refresh**: Token refresh handled transparently
- ✅ **CSRF Protection**: Session-based auth provides CSRF protection
- ✅ **XSS Mitigation**: HTTP-only cookies prevent XSS token theft

## 🚀 Production Considerations

### HTTPS Requirements
- **Production**: Must use HTTPS for all authentication flows
- **Development**: HTTP allowed for testing only
- **Certificate Validation**: Enable `RequireHttpsMetadata` in production

### Configuration Security
- **Secret Management**: Use secure secret storage (Azure Key Vault, etc.)
- **Environment Variables**: Don't store secrets in config files
- **Rotation**: Implement regular secret rotation

### Monitoring & Logging
- **Authentication Events**: Log all authentication attempts
- **Failed Logins**: Monitor failed login attempts
- **Security Events**: Alert on security-related events
- **Audit Trail**: Maintain audit logs for compliance

### Rate Limiting
- **Login Attempts**: Implement rate limiting on login endpoints
- **Callback Validation**: Rate limit callback processing
- **Token Refresh**: Limit token refresh requests

## 📋 Security Checklist

### Configuration
- [ ] HTTPS enabled in production
- [ ] Secure random keys generated
- [ ] Proper CORS configuration
- [ ] Session timeout configured
- [ ] Security headers enabled

### Authentication Flow
- [ ] State parameter validation
- [ ] Return URL validation
- [ ] Proper error handling
- [ ] Federated logout implemented
- [ ] Session security configured

### Monitoring
- [ ] Authentication logging enabled
- [ ] Failed login monitoring
- [ ] Security event alerting
- [ ] Audit trail implementation

### Dependencies
- [ ] Regular security updates
- [ ] Vulnerability scanning
- [ ] Dependency audit
- [ ] Security testing

## 🔍 Common Vulnerabilities Mitigated

1. **Cross-Site Request Forgery (CSRF)**
   - Mitigated by: State parameter, SameSite cookies

2. **Open Redirect Attacks**
   - Mitigated by: Return URL validation, allowlist

3. **Session Hijacking**
   - Mitigated by: HTTP-only cookies, secure flags, timeout

4. **Cross-Site Scripting (XSS)**
   - Mitigated by: HTTP-only cookies, CSP headers, no token exposure

5. **Clickjacking**
   - Mitigated by: X-Frame-Options header

6. **Token Theft**
   - Mitigated by: Server-side token storage, BFF pattern

7. **Man-in-the-Middle (MITM)**
   - Mitigated by: HTTPS requirement, secure cookies

This implementation follows industry best practices for OAuth2/OIDC authentication in a BFF architecture.
