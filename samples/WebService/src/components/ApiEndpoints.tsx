const ApiEndpoints = () => {
    return (
        <div className="bg-gray-800 rounded-xl shadow-xl p-6 border border-gray-700">
            <h3 className="text-xl font-semibold text-white mb-4 flex items-center">
                <span className="w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center text-xs mr-2">API</span>
                Available Endpoints
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-3">
                    <div className="flex items-center space-x-3 p-3 bg-gray-900 rounded-lg border border-gray-600">
                        <span className="px-2 py-1 bg-green-600 text-white text-xs rounded font-mono">GET</span>
                        <code className="text-gray-300">/auth/status</code>
                        <span className="text-gray-400 text-sm">Check auth state</span>
                    </div>
                    <div className="flex items-center space-x-3 p-3 bg-gray-900 rounded-lg border border-gray-600">
                        <span className="px-2 py-1 bg-green-600 text-white text-xs rounded font-mono">GET</span>
                        <code className="text-gray-300">/auth/login</code>
                        <span className="text-gray-400 text-sm">Initiate OAuth flow</span>
                    </div>
                    <div className="flex items-center space-x-3 p-3 bg-gray-900 rounded-lg border border-gray-600">
                        <span className="px-2 py-1 bg-red-600 text-white text-xs rounded font-mono">POST</span>
                        <code className="text-gray-300">/auth/logout</code>
                        <span className="text-gray-400 text-sm">End session</span>
                    </div>
                </div>
                <div className="space-y-3">
                    <div className="flex items-center space-x-3 p-3 bg-gray-900 rounded-lg border border-gray-600">
                        <span className="px-2 py-1 bg-green-600 text-white text-xs rounded font-mono">GET</span>
                        <code className="text-gray-300">/auth/profile</code>
                        <span className="text-gray-400 text-sm">User profile 🔒</span>
                    </div>
                    <div className="flex items-center space-x-3 p-3 bg-gray-900 rounded-lg border border-gray-600">
                        <span className="px-2 py-1 bg-green-600 text-white text-xs rounded font-mono">GET</span>
                        <code className="text-gray-300">/WeatherForecast</code>
                        <span className="text-gray-400 text-sm">Protected data 🔒</span>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ApiEndpoints;
