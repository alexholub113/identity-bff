/**
 * Authentication utilities for handling login redirects and return URLs
 */

/**
 * Gets the current return URL from the URL parameters or defaults to current page
 */
export function getCurrentReturnUrl(): string {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('returnUrl') || window.location.pathname;
}

/**
 * Creates a login URL with return URL for deep linking scenarios
 * @param targetPage - The page to redirect to after authentication
 * @param baseUrl - Base URL for the application (defaults to current origin)
 */
export function createReturnUrl(targetPage: string, baseUrl?: string): string {
    const base = baseUrl || window.location.origin;
    return `${base}${targetPage}`;
}

/**
 * Checks if the current page requires authentication and redirects if needed
 * @param authStore - The authentication store instance
 * @param protectedPaths - Array of paths that require authentication
 */
export function checkAuthenticationRequired(
    isAuthenticated: boolean, 
    protectedPaths: string[] = ['/dashboard', '/profile', '/settings']
): boolean {
    const currentPath = window.location.pathname;
    return !isAuthenticated && protectedPaths.some(path => currentPath.startsWith(path));
}

/**
 * Example usage for protected route component:
 * 
 * ```typescript
 * // In a component that requires authentication
 * const ProtectedComponent = () => {
 *     const authStore = useAuthStore();
 *     
 *     useEffect(() => {
 *         if (checkAuthenticationRequired(authStore.isAuthenticated)) {
 *             // Redirect to login but return to current page after auth
 *             authStore.loginWithExternalProvider(window.location.href);
 *         }
 *     }, [authStore.isAuthenticated]);
 *     
 *     if (!authStore.isAuthenticated) {
 *         return <div>Redirecting to login...</div>;
 *     }
 *     
 *     return <div>Protected content here</div>;
 * };
 * ```
 */
