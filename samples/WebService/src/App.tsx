import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import ProtectedData from './components/ProtectedData';
import { useAuthStore } from './stores';

const App = observer(() => {
    const authStore = useAuthStore();

    useEffect(() => {
        // Check if user is already authenticated on app load
        authStore.checkAuth();
    }, [authStore]);

    const handleExternalLogin = async () => {
        try {
            // You can optionally specify a specific return URL
            // For example, to redirect to a specific page after authentication
            // await authStore.loginWithExternalProvider('/dashboard');
            
            // Or use the current page as return URL (default behavior)
            await authStore.loginWithExternalProvider();
        } catch (error) {
            console.error('External login error:', error);
        }
    };

    const handleLogout = async () => {
        try {
            await authStore.logout();
        } catch (error) {
            console.error('Logout error:', error);
        }
    };

    return (
        <div className="min-h-screen bg-gray-100">
            {/* Header */}
            <header className="bg-white shadow-sm border-b">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex justify-between items-center h-16">
                        <h1 className="text-xl font-semibold text-gray-900">
                            IdentityProvider.Bff Demo
                        </h1>                        <div className="flex items-center space-x-4">
                            {/* Authentication Status */}
                            <div className="flex items-center space-x-2">
                                <div className={`w-3 h-3 rounded-full ${authStore.isAuthenticated ? 'bg-green-400' : 'bg-red-400'}`}></div>
                                <span className="text-sm text-gray-600">
                                    {authStore.isAuthenticated ? `Logged in as ${authStore.user?.name || authStore.user?.email}` : 'Not authenticated'}
                                </span>
                            </div>

                            {/* Auth Buttons */}
                            {authStore.isAuthenticated ? (
                                <button
                                    onClick={handleLogout}
                                    disabled={authStore.isLoading}
                                    className="px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 disabled:opacity-50 disabled:cursor-not-allowed"
                                >
                                    {authStore.isLoading ? 'Logging out...' : 'Logout'}
                                </button>
                            ) : (
                                <div className="flex space-x-2">
                                    <button
                                        onClick={handleExternalLogin}
                                        disabled={authStore.isLoading}
                                        className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                                    >
                                        {authStore.isLoading ? 'Connecting...' : 'Login'}
                                    </button>
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <div className="space-y-8">
                    {/* Welcome Section */}
                    <div className="bg-white rounded-lg shadow p-6">
                        <h2 className="text-2xl font-bold text-gray-900 mb-4">
                            IdentityProvider.Bff Service Demo
                        </h2>
                        <p className="text-gray-600 mb-4">
                            This frontend now connects to the real IdentityProvider.Bff.Service API running on port 7108. 
                            It demonstrates authentication flow and accessing protected endpoints like the WeatherForecast API.
                        </p>

                        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                            <h3 className="font-medium text-blue-900 mb-2">API Integration:</h3>
                            <ul className="text-sm text-blue-800 space-y-1">
                                <li>• <strong>Auth Status:</strong> GET /auth/status - Check authentication status</li>
                                <li>• <strong>Login:</strong> GET /auth/login - Initiate login flow</li>
                                <li>• <strong>Logout:</strong> POST /auth/logout - End session</li>
                                <li>• <strong>Profile:</strong> GET /auth/profile - Get user profile (protected)</li>
                                <li>• <strong>Weather:</strong> GET /WeatherForecast - Get weather data (protected)</li>
                            </ul>
                        </div>
                    </div>

                    {/* User Information */}
                    {authStore.isAuthenticated && authStore.user && (
                        <div className="bg-white rounded-lg shadow p-6">
                            <h3 className="text-lg font-semibold text-gray-900 mb-4">User Information</h3>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700">Email</label>
                                    <p className="text-sm text-gray-900">{authStore.user.email}</p>
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700">Name</label>
                                    <p className="text-sm text-gray-900">{authStore.user.name}</p>
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700">Roles</label>
                                    <p className="text-sm text-gray-900">{authStore.user.roles?.join(', ') || 'None'}</p>
                                </div>
                            </div>
                        </div>
                    )}

                    {/* Protected Data Section */}
                    <ProtectedData />

                    {/* Technical Information */}
                    <div className="bg-white rounded-lg shadow p-6">
                        <h3 className="text-lg font-semibold text-gray-900 mb-4">Implementation Notes</h3>
                        <div className="space-y-4 text-sm text-gray-600">
                            <div>
                                <h4 className="font-medium text-gray-900">Current Implementation:</h4>
                                <p>This frontend now connects to the real IdentityProvider.Bff.Service API using cookie-based authentication for the BFF (Backend-for-Frontend) pattern.</p>
                            </div>

                            <div>
                                <h4 className="font-medium text-gray-900">Authentication Flow:</h4>
                                <ul className="list-disc pl-5 space-y-1">
                                    <li><strong>Login:</strong> Redirects to /login with returnUrl parameter</li>
                                    <li><strong>Return URL:</strong> User is redirected back to the original page after authentication</li>
                                    <li><strong>Session:</strong> Uses HTTP-only cookies for secure session management</li>
                                    <li><strong>Status Check:</strong> Calls /status to verify authentication state</li>
                                    <li><strong>Protected Data:</strong> Accesses /WeatherForecast endpoint with automatic auth validation</li>
                                </ul>
                            </div>

                            <div>
                                <h4 className="font-medium text-gray-900">Security Features:</h4>
                                <ul className="list-disc pl-5 space-y-1">
                                    <li>Credentials included in all requests for cookie-based auth</li>
                                    <li>Automatic handling of 401 unauthorized responses</li>
                                    <li>Secure HTTPS proxy to backend service</li>
                                    <li>No sensitive tokens stored in frontend JavaScript</li>
                                    <li>Environment-based API configuration</li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
});

export default App;
