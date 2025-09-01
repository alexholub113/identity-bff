import { AuthStore } from './AuthStore';
import { DataStore } from './DataStore';

export class RootStore {
    authStore: AuthStore;
    dataStore: DataStore;

    constructor() {
        this.authStore = new AuthStore();
        this.dataStore = new DataStore();
    }
}

// Create a singleton instance of the root store
export const rootStore = new RootStore();
