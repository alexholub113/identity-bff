import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import { useAuthStore, useDataStore } from '../stores';

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
            <div className="bg-white rounded-lg shadow p-6">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Protected Data</h3>
                <p className="text-gray-600">Please log in to view protected data and weather information.</p>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Protected Data Section */}
            <div className="bg-white rounded-lg shadow p-6">
                <div className="flex justify-between items-center mb-4">
                    <h3 className="text-lg font-semibold text-gray-900">Protected Data</h3>
                    {dataStore.isLoading && (
                        <div className="text-sm text-blue-600">Loading...</div>
                    )}
                </div>

                {dataStore.error && (
                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
                        {dataStore.error}
                    </div>
                )}

                {dataStore.protectedData && (
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700">User ID</label>
                            <p className="text-sm text-gray-900">{dataStore.protectedData.userId}</p>
                        </div>
                        
                        <div>
                            <label className="block text-sm font-medium text-gray-700">Secure Data</label>
                            <p className="text-sm text-gray-900">{dataStore.protectedData.secureData}</p>
                        </div>
                        
                        <div>
                            <label className="block text-sm font-medium text-gray-700">User Permissions</label>
                            <p className="text-sm text-gray-900">{dataStore.protectedData.userPermissions.join(', ')}</p>
                        </div>
                        
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Login Time</label>
                                <p className="text-sm text-gray-900">
                                    {new Date(dataStore.protectedData.sessionInfo.loginTime).toLocaleString()}
                                </p>
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700">Session Expires</label>
                                <p className="text-sm text-gray-900">
                                    {new Date(dataStore.protectedData.sessionInfo.expiresAt).toLocaleString()}
                                </p>
                            </div>
                        </div>
                        
                        <div>
                            <label className="block text-sm font-medium text-gray-700">Last Updated</label>
                            <p className="text-sm text-gray-900">
                                {new Date(dataStore.protectedData.timestamp).toLocaleString()}
                            </p>
                        </div>
                    </div>
                )}
            </div>

            {/* Weather Forecast Section */}
            <div className="bg-white rounded-lg shadow p-6">
                <div className="flex justify-between items-center mb-4">
                    <h3 className="text-lg font-semibold text-gray-900">Weather Forecast (Protected API)</h3>
                    <button
                        onClick={handleRefreshWeather}
                        disabled={dataStore.isLoading}
                        className="px-3 py-1 bg-blue-600 text-white text-sm rounded hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
                    >
                        {dataStore.isLoading ? 'Loading...' : 'Refresh'}
                    </button>
                </div>

                {dataStore.weatherForecast.length > 0 ? (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Date
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Temperature (°C)
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Temperature (°F)
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Summary
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {dataStore.weatherForecast.map((forecast, index) => (
                                    <tr key={index} className="hover:bg-gray-50">
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                            {new Date(forecast.date).toLocaleDateString()}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                            {forecast.temperatureC}°C
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                            {forecast.temperatureF}°F
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                            {forecast.summary || 'N/A'}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <p className="text-gray-600">No weather data available.</p>
                )}
            </div>
        </div>
    );
});

export default ProtectedData;
