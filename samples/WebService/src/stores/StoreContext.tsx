import React, { createContext, ReactNode, useContext } from 'react';
import { RootStore, rootStore } from './RootStore';

const StoreContext = createContext<RootStore>(rootStore);

interface StoreProviderProps {
    children: ReactNode;
}

export const StoreProvider: React.FC<StoreProviderProps> = ({ children }) => {
    return (
        <StoreContext.Provider value={rootStore}>
            {children}
        </StoreContext.Provider>
    );
};

export const useStore = (): RootStore => {
    const store = useContext(StoreContext);
    if (!store) {
        throw new Error('useStore must be used within a StoreProvider');
    }
    return store;
};

// Convenience hooks for individual stores
export const useAuthStore = () => useStore().authStore;
export const useDataStore = () => useStore().dataStore;
