import { observer } from 'mobx-react-lite';
import { useAuthStore, useDataStore } from '../stores';

const AuthFlowDemo = observer(() => {
    const dataStore = useDataStore();
    const authStore = useAuthStore();

    const handleTryProtectedCall = async () => {
        await dataStore.fetchWeatherForecast();
    };

    return (
        <div>
            {/* Demo Actions */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <h4 className="text-white font-medium mb-2 flex items-center">
                        <span className="mr-2">🌤️</span>
                        Weather Forecast API
                    </h4>
                    <p className="text-gray-400 text-sm mb-3">
                        GET /WeatherForecast - Requires valid authentication
                    </p>
                    <button
                        onClick={handleTryProtectedCall}
                        disabled={dataStore.isLoading}
                        className="w-full px-4 py-2 bg-gradient-to-r from-blue-600 to-blue-700 text-white rounded-lg hover:from-blue-700 hover:to-blue-800 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 focus:ring-offset-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 font-medium"
                    >
                        {dataStore.isLoading ? (
                            <span className="flex items-center justify-center">
                                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></div>
                                Calling API...
                            </span>
                        ) : (
                            'Try Protected API Call'
                        )}
                    </button>
                </div>

                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <h4 className="text-white font-medium mb-2 flex items-center">
                        <span className="mr-2">👤</span>
                        User Profile API
                    </h4>
                    <p className="text-gray-400 text-sm mb-3">
                        GET /auth/profile - Returns user information
                    </p>
                    <button
                        onClick={() => authStore.loginWithExternalProvider()}
                        disabled={authStore.isLoading}
                        className="w-full px-4 py-2 bg-gradient-to-r from-green-600 to-green-700 text-white rounded-lg hover:from-green-700 hover:to-green-800 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 focus:ring-offset-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 font-medium"
                    >
                        {authStore.isLoading ? (
                            <span className="flex items-center justify-center">
                                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></div>
                                Redirecting...
                            </span>
                        ) : (
                            'Start OAuth2 Flow'
                        )}
                    </button>
                </div>
            </div>

            {/* Flow Explanation */}
            <div className="bg-gradient-to-r from-blue-900/50 to-purple-900/50 rounded-lg p-4 border border-blue-700/50">
                <h4 className="text-blue-300 font-medium mb-2 flex items-center">
                    <span className="mr-2">🔄</span>
                    How the Authentication Flow Works:
                </h4>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                    <div>
                        <p className="text-blue-200 font-medium mb-1">Protected API Call:</p>
                        <ul className="text-blue-300 space-y-1 list-disc list-inside">
                            <li>If authenticated: Returns weather data ✅</li>
                            <li>If not authenticated: Auto-redirects to login 🔄</li>
                            <li>No manual login step needed!</li>
                            <li>Seamless user experience</li>
                        </ul>
                    </div>
                    <div>
                        <p className="text-green-200 font-medium mb-1">Manual OAuth2 Flow:</p>
                        <ul className="text-green-300 space-y-1 list-disc list-inside">
                            <li>Alternative way to authenticate</li>
                            <li>Redirects to Identity Provider</li>
                            <li>User authenticates manually</li>
                            <li>Returns with valid session</li>
                        </ul>
                    </div>
                </div>
                <div className="mt-3 p-3 bg-green-900/30 border border-green-600/50 rounded-lg">
                    <p className="text-green-200 text-sm">
                        <span className="font-medium">💡 Try it:</span> Click "Try Protected API Call" without logging in first. 
                        You'll be automatically redirected to authenticate, then returned to see the weather data!
                    </p>
                </div>
            </div>
        </div>
    );
});

export default AuthFlowDemo;
