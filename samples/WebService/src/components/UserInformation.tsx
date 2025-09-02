import { observer } from 'mobx-react-lite';
import { useAuthStore } from '../stores';

const UserInformation = observer(() => {
    const authStore = useAuthStore();

    if (!authStore.isAuthenticated || !authStore.user) {
        return null;
    }

    return (
        <div className="bg-gray-800 rounded-xl shadow-xl p-6 border border-gray-700">
            <h3 className="text-xl font-semibold text-white mb-4 flex items-center">
                <span className="w-6 h-6 bg-green-500 rounded-full flex items-center justify-center text-xs mr-2">👤</span>
                Authenticated User
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <label className="block text-sm font-medium text-gray-400 mb-1">Email</label>
                    <p className="text-white font-mono text-sm">{authStore.user.email}</p>
                </div>
                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <label className="block text-sm font-medium text-gray-400 mb-1">Name</label>
                    <p className="text-white font-mono text-sm">{authStore.user.name}</p>
                </div>
                <div className="bg-gray-900 rounded-lg p-4 border border-gray-600">
                    <label className="block text-sm font-medium text-gray-400 mb-1">Roles</label>
                    <p className="text-white font-mono text-sm">{authStore.user.roles?.join(', ') || 'None'}</p>
                </div>
            </div>
        </div>
    );
});

export default UserInformation;
