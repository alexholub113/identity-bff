import { observer } from 'mobx-react-lite';
import { useAuthStore } from '../stores';

const Header = observer(() => {
    const authStore = useAuthStore();

    const handleExternalLogin = async () => {
        try {
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
        <header className="bg-gray-800 border-b border-gray-700 shadow-lg">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between items-center h-16">
                    <div className="flex items-center space-x-3">
                        <div className="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
                            <span className="text-white font-bold text-sm">BFF</span>
                        </div>
                        <h1 className="text-xl font-bold text-white">
                            Identity Provider & BFF Demo
                        </h1>
                    </div>
                    
                    <div className="flex items-center space-x-4">
                        {/* Authentication Status */}
                        <div className="flex items-center space-x-2">
                            <div className={`w-2 h-2 rounded-full ${authStore.isAuthenticated ? 'bg-green-400' : 'bg-red-400'}`}></div>
                            <span className="text-sm text-gray-300">
                                {authStore.isAuthenticated ? `${authStore.user?.name || authStore.user?.email}` : 'Not authenticated'}
                            </span>
                        </div>

                        {/* Auth Buttons */}
                        {authStore.isAuthenticated ? (
                            <button
                                onClick={handleLogout}
                                disabled={authStore.isLoading}
                                className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 focus:ring-offset-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                            >
                                {authStore.isLoading ? 'Signing out...' : 'Sign Out'}
                            </button>
                        ) : (
                            <button
                                onClick={handleExternalLogin}
                                disabled={authStore.isLoading}
                                className="px-6 py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg hover:from-blue-700 hover:to-purple-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 focus:ring-offset-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 font-medium"
                            >
                                {authStore.isLoading ? 'Connecting...' : 'Sign In with OAuth2'}
                            </button>
                        )}
                    </div>
                </div>
            </div>
        </header>
    );
});

export default Header;
