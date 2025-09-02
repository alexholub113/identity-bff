import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { useAuthStore, useDataStore } from '../stores';
import AuthFlowDemo from './AuthFlowDemo';
import ErrorDisplay from './ErrorDisplay';
import WeatherTable from './WeatherTable';

const ProtectedData = observer(() => {
    const dataStore = useDataStore();
    const authStore = useAuthStore();

    useEffect(() => {
        if (authStore.isAuthenticated && authStore.user) {
            dataStore.fetchProtectedData(authStore.user.id, authStore.user.roles);
        } else {
            dataStore.clearData();
        }
    }, [authStore.isAuthenticated, authStore.user, dataStore]);

    const handleRefreshWeather = () => {
        if (authStore.isAuthenticated) {
            dataStore.fetchWeatherForecast();
        }
    };

    if (!authStore.isAuthenticated) {
        return (
            <div className="bg-gray-800 rounded-xl shadow-xl p-6 border border-gray-700">
                <h3 className="text-xl font-semibold text-white mb-4 flex items-center">
                    <span className="w-6 h-6 bg-yellow-500 rounded-full flex items-center justify-center text-xs mr-2">🔒</span>
                    Protected Resources Demo
                </h3>
                
                <div className="text-center py-6 mb-6">
                    <div className="w-16 h-16 bg-gray-700 rounded-full flex items-center justify-center mx-auto mb-4">
                        <span className="text-2xl">🔐</span>
                    </div>
                    <p className="text-gray-300 mb-2 text-lg font-medium">Experience the Authentication Flow</p>
                    <p className="text-gray-400 mb-6">Try accessing protected endpoints to see how OAuth2 authentication works</p>
                </div>

                <AuthFlowDemo />

                {/* Error Display */}
                {dataStore.error && (
                    <div className="mt-4">
                        <ErrorDisplay error={dataStore.error} />
                    </div>
                )}
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Protected Data Section */}
            <div className="bg-gray-800 rounded-xl shadow-xl p-6 border border-gray-700">
                <div className="flex justify-between items-center mb-6">
                    <h3 className="text-xl font-semibold text-white flex items-center">
                        <span className="w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center text-xs mr-2">🔒</span>
                        Protected User Data
                    </h3>
                    {dataStore.isLoading && (
                        <div className="flex items-center space-x-2">
                            <div className="w-4 h-4 border-2 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
                            <span className="text-sm text-blue-400">Loading...</span>
                        </div>
                    )}
                </div>

                {dataStore.error && (
                    <div className="mb-4">
                        <ErrorDisplay error={dataStore.error} />
                    </div>
                )}

                {dataStore.protectedData && (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                            <label className="block text-sm font-medium text-gray-400 mb-1">User ID</label>
                            <p className="text-white font-mono text-sm">{dataStore.protectedData.userId}</p>
                        </div>
                        
                        <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                            <label className="block text-sm font-medium text-gray-400 mb-1">Secure Data</label>
                            <p className="text-white font-mono text-sm">{dataStore.protectedData.secureData}</p>
                        </div>
                        
                        <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                            <label className="block text-sm font-medium text-gray-400 mb-1">Permissions</label>
                            <p className="text-white font-mono text-sm">{dataStore.protectedData.userPermissions.join(', ')}</p>
                        </div>
                        
                        <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                            <label className="block text-sm font-medium text-gray-400 mb-1">Login Time</label>
                            <p className="text-white font-mono text-sm">
                                {new Date(dataStore.protectedData.sessionInfo.loginTime).toLocaleString()}
                            </p>
                        </div>
                        
                        <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                            <label className="block text-sm font-medium text-gray-400 mb-1">Session Expires</label>
                            <p className="text-white font-mono text-sm">
                                {new Date(dataStore.protectedData.sessionInfo.expiresAt).toLocaleString()}
                            </p>
                        </div>
                        
                        <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                            <label className="block text-sm font-medium text-gray-400 mb-1">Last Updated</label>
                            <p className="text-white font-mono text-sm">
                                {new Date(dataStore.protectedData.timestamp).toLocaleString()}
                            </p>
                        </div>
                    </div>
                )}
            </div>

            {/* Weather Forecast Section */}
            <div className="bg-gray-800 rounded-xl shadow-xl p-6 border border-gray-700">
                <div className="flex justify-between items-center mb-6">
                    <h3 className="text-xl font-semibold text-white flex items-center">
                        <span className="w-6 h-6 bg-green-500 rounded-full flex items-center justify-center text-xs mr-2">🌤️</span>
                        Weather Forecast API
                    </h3>
                    <button
                        onClick={handleRefreshWeather}
                        disabled={dataStore.isLoading}
                        className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 focus:ring-offset-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors font-medium"
                    >
                        {dataStore.isLoading ? (
                            <span className="flex items-center">
                                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin mr-2"></div>
                                Loading...
                            </span>
                        ) : (
                            <span className="flex items-center">
                                🔄 Refresh Data
                            </span>
                        )}
                    </button>
                </div>

                <WeatherTable />
            </div>
        </div>
    );
});

export default ProtectedData;
