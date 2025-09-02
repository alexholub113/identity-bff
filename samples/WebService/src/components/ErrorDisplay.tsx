interface ErrorInfo {
    type: string;
    title: string;
    message: string;
    instructions: string[];
}

interface ErrorDisplayProps {
    error: string;
}

const ErrorDisplay = ({ error }: ErrorDisplayProps) => {
    // Check if error indicates IdP server is not running
    const isIdpServerError = (error: string) => {
        return error.toLowerCase().includes('network') || 
               error.toLowerCase().includes('connection') ||
               error.toLowerCase().includes('fetch') ||
               error.toLowerCase().includes('failed to fetch') ||
               error.toLowerCase().includes('refused') ||
               error.toLowerCase().includes('timeout') ||
               error.toLowerCase().includes('502') ||
               error.toLowerCase().includes('503') ||
               error.toLowerCase().includes('504');
    };

    const getErrorExplanation = (error: string): ErrorInfo => {
        if (isIdpServerError(error)) {
            return {
                type: 'server-error',
                title: 'Identity Provider Server Not Running',
                message: 'The Identity Provider server appears to be offline. Please start the IdP server before testing authentication.',
                instructions: [
                    'Navigate to the idp-server-dotnet project',
                    'Run: dotnet run --project src/IdentityProvider.Server.Api',
                    'Wait for server to start on port 57470',
                    'Then try the authentication flow again'
                ]
            };
        } else if (error.toLowerCase().includes('401') || error.toLowerCase().includes('unauthorized')) {
            return {
                type: 'auth-required',
                title: 'Authentication Required (Expected)',
                message: 'This is the expected behavior! The API correctly rejected the request because you\'re not authenticated.',
                instructions: [
                    'This demonstrates proper security - protected resources require authentication',
                    'Click "Start OAuth2 Flow" to authenticate',
                    'You\'ll be redirected to the Identity Provider',
                    'After login, you can access protected resources'
                ]
            };
        } else {
            return {
                type: 'general-error',
                title: 'Unexpected Error',
                message: error,
                instructions: [
                    'Check that both servers are running:',
                    '• BFF API on port 7108',
                    '• Identity Provider on port 57470',
                    'Check browser console for more details'
                ]
            };
        }
    };

    const errorInfo = getErrorExplanation(error);
    const isServerError = errorInfo.type === 'server-error';
    const isAuthError = errorInfo.type === 'auth-required';
    
    return (
        <div className={`border rounded-lg p-4 ${
            isServerError ? 'bg-orange-900/50 border-orange-600/50' :
            isAuthError ? 'bg-blue-900/50 border-blue-600/50' :
            'bg-red-900/50 border-red-600/50'
        }`}>
            <div className="flex items-start">
                <span className="mr-3 text-2xl">
                    {isServerError ? '🔌' : isAuthError ? '🔒' : '⚠️'}
                </span>
                <div className="flex-1">
                    <h4 className={`font-medium mb-2 ${
                        isServerError ? 'text-orange-200' :
                        isAuthError ? 'text-blue-200' :
                        'text-red-200'
                    }`}>
                        {errorInfo.title}
                    </h4>
                    <p className={`text-sm mb-3 ${
                        isServerError ? 'text-orange-300' :
                        isAuthError ? 'text-blue-300' :
                        'text-red-300'
                    }`}>
                        {errorInfo.message}
                    </p>
                    
                    {errorInfo.instructions && (
                        <div className={`text-xs rounded p-3 ${
                            isServerError ? 'bg-orange-800/30' :
                            isAuthError ? 'bg-blue-800/30' :
                            'bg-red-800/30'
                        }`}>
                            <p className={`font-medium mb-1 ${
                                isServerError ? 'text-orange-200' :
                                isAuthError ? 'text-blue-200' :
                                'text-red-200'
                            }`}>
                                {isServerError ? 'To fix this issue:' : 
                                 isAuthError ? 'Next steps:' : 
                                 'Troubleshooting:'}
                            </p>
                            <ol className={`list-decimal list-inside space-y-1 ${
                                isServerError ? 'text-orange-300' :
                                isAuthError ? 'text-blue-300' :
                                'text-red-300'
                            }`}>
                                {errorInfo.instructions.map((instruction, index) => (
                                    <li key={index}>{instruction}</li>
                                ))}
                            </ol>
                        </div>
                    )}
                    
                    {isServerError && (
                        <div className="mt-3 p-2 bg-gray-700 rounded text-xs font-mono text-gray-300">
                            <strong>Expected servers:</strong><br/>
                            • Identity Provider: http://localhost:57470<br/>
                            • BFF API: http://localhost:7108
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ErrorDisplay;
