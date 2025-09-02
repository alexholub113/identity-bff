const TechnicalImplementation = () => {
    return (
        <div className="bg-gray-800 rounded-xl shadow-xl p-6 border border-gray-700">
            <h3 className="text-xl font-semibold text-white mb-4 flex items-center">
                <span className="w-6 h-6 bg-purple-500 rounded-full flex items-center justify-center text-xs mr-2">⚙️</span>
                Implementation Highlights
            </h3>
            
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <div className="space-y-4">
                    <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                        <h4 className="font-semibold text-blue-400 mb-2">🔐 Security Features</h4>
                        <ul className="text-sm text-gray-300 space-y-1">
                            <li>• HTTP-only cookies for session management</li>
                            <li>• CSRF protection with state parameter</li>
                            <li>• No tokens exposed to JavaScript</li>
                            <li>• Secure HTTPS communication</li>
                            <li>• Automatic 401 response handling</li>
                        </ul>
                    </div>
                    
                    <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                        <h4 className="font-semibold text-green-400 mb-2">🏗️ Architecture</h4>
                        <ul className="text-sm text-gray-300 space-y-1">
                            <li>• Backend-for-Frontend (BFF) pattern</li>
                            <li>• ASP.NET Core with Minimal APIs</li>
                            <li>• Custom OpenID Connect handler</li>
                            <li>• Environment-based configuration</li>
                            <li>• RESTful API design</li>
                        </ul>
                    </div>
                </div>
                
                <div className="space-y-4">
                    <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                        <h4 className="font-semibold text-purple-400 mb-2">⚡ Frontend Stack</h4>
                        <ul className="text-sm text-gray-300 space-y-1">
                            <li>• React 18 with TypeScript</li>
                            <li>• MobX for reactive state management</li>
                            <li>• Vite for fast development builds</li>
                            <li>• Tailwind CSS for styling</li>
                            <li>• API proxy for seamless integration</li>
                        </ul>
                    </div>
                    
                    <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                        <h4 className="font-semibold text-yellow-400 mb-2">🔄 OAuth2 Flow</h4>
                        <ul className="text-sm text-gray-300 space-y-1">
                            <li>• Authorization Code with PKCE</li>
                            <li>• Dynamic return URL support</li>
                            <li>• Federated logout capability</li>
                            <li>• Scope-based access control</li>
                            <li>• Session expiration handling</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default TechnicalImplementation;
