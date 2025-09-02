import { observer } from 'mobx-react-lite';
import { useEffect } from 'react';
import ApiEndpoints from './components/ApiEndpoints';
import Header from './components/Header';
import HeroSection from './components/HeroSection';
import ProtectedData from './components/ProtectedData';
import TechnicalImplementation from './components/TechnicalImplementation';
import UserInformation from './components/UserInformation';
import { useAuthStore } from './stores';

const App = observer(() => {
    const authStore = useAuthStore();

    useEffect(() => {
        // Check if user is already authenticated on app load
        authStore.checkAuth();
    }, [authStore]);

    return (
        <div className="min-h-screen bg-gray-900 text-white">
            <Header />

            {/* Main Content */}
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <div className="space-y-8">
                    <HeroSection />
                    <UserInformation />
                    <ProtectedData />
                    <ApiEndpoints />
                    <TechnicalImplementation />
                </div>
            </main>
        </div>
    );
});

export default App;
