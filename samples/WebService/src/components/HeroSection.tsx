const HeroSection = () => {
    return (
        <div className="bg-gradient-to-r from-gray-800 to-gray-700 rounded-xl shadow-xl p-8">
            <div className="text-center mb-6">
                <h2 className="text-3xl font-bold text-white mb-3">
                    OAuth2 + OpenID Connect + BFF Pattern
                </h2>
                <p className="text-gray-300 text-lg max-w-3xl mx-auto">
                    A complete implementation showcasing modern authentication patterns with ASP.NET Core backend 
                    and React frontend using the Backend-for-Frontend (BFF) security pattern.
                </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <div className="text-blue-400 font-semibold text-sm mb-2">🔐 AUTHENTICATION</div>
                    <h3 className="font-medium text-white mb-2">OAuth2 + OIDC</h3>
                    <p className="text-gray-400 text-sm">Industry standard authentication with authorization code flow and PKCE</p>
                </div>
                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <div className="text-green-400 font-semibold text-sm mb-2">🛡️ SECURITY</div>
                    <h3 className="font-medium text-white mb-2">BFF Pattern</h3>
                    <p className="text-gray-400 text-sm">Backend-for-Frontend with HTTP-only cookies and CSRF protection</p>
                </div>
                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <div className="text-purple-400 font-semibold text-sm mb-2">⚡ TECHNOLOGY</div>
                    <h3 className="font-medium text-white mb-2">Modern Stack</h3>
                    <p className="text-gray-400 text-sm">ASP.NET Core, React, TypeScript, MobX, Tailwind CSS</p>
                </div>
            </div>
        </div>
    );
};

export default HeroSection;
