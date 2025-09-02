import { observer } from 'mobx-react-lite';
import { useDataStore } from '../stores';

const WeatherTable = observer(() => {
    const dataStore = useDataStore();

    if (dataStore.weatherForecast.length === 0) {
        return (
            <div className="text-center py-8">
                <div className="w-16 h-16 bg-gray-700 rounded-full flex items-center justify-center mx-auto mb-4">
                    <span className="text-2xl">📊</span>
                </div>
                <p className="text-gray-400 mb-2">No weather data available</p>
                <p className="text-sm text-gray-500">Click refresh to load weather forecast data</p>
            </div>
        );
    }

    return (
        <div className="overflow-x-auto">
            <table className="min-w-full">
                <thead>
                    <tr className="border-b border-gray-600">
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                            Date
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                            Temperature (°C)
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                            Temperature (°F)
                        </th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">
                            Summary
                        </th>
                    </tr>
                </thead>
                <tbody className="divide-y divide-gray-600">
                    {dataStore.weatherForecast.map((forecast, index) => (
                        <tr key={index} className="hover:bg-gray-700 transition-colors">
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-white font-mono">
                                {new Date(forecast.date).toLocaleDateString()}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-white font-mono">
                                <span className="px-2 py-1 bg-blue-600 rounded text-white text-xs">
                                    {forecast.temperatureC}°C
                                </span>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-white font-mono">
                                <span className="px-2 py-1 bg-orange-600 rounded text-white text-xs">
                                    {forecast.temperatureF}°F
                                </span>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-300">
                                <span className="px-2 py-1 bg-gray-600 rounded text-gray-200 text-xs">
                                    {forecast.summary || 'N/A'}
                                </span>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
});

export default WeatherTable;
