import { makeAutoObservable, runInAction } from 'mobx';
import { ApiService, AuthStatus, UserProfile } from '../services/apiService';

export interface User {
    id: string;
    email: string;
    name: string;
    roles?: string[];
}

export class AuthStore {
    user: User | null = null;
    isLoading = false;

    constructor() {
        makeAutoObservable(this);
    }

    get isAuthenticated(): boolean {
        return !!this.user;
    }

    setLoading(loading: boolean) {
        this.isLoading = loading;
    }

    setUser(user: User | null) {
        this.user = user;
    }

    /**
     * Creates a login URL with the return URL as a query parameter
     * @param returnUrl - URL to redirect to after successful authentication (defaults to current page)
     */
    private createLoginUrl(returnUrl?: string): string {
        const currentUrl = returnUrl || window.location.href;
        const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7108';
        const loginUrl = new URL(`/auth/login`, baseUrl);
        loginUrl.searchParams.set('returnUrl', currentUrl);
        return loginUrl.toString();
    }

    /**
     * Redirects to authentication with an optional return URL  
     * @param returnUrl - URL to redirect to after successful authentication
     */
    async loginWithExternalProvider(returnUrl?: string): Promise<void> {
        this.setLoading(true);
        
        try {
            // Redirect to the auth login endpoint which will handle external provider authentication
            // Include return URL so user can be redirected back to the appropriate page after auth
            window.location.href = this.createLoginUrl(returnUrl);
        } catch (error) {
            console.error('External login failed:', error);
            throw new Error('External authentication failed. Please try again.');
        } finally {
            runInAction(() => {
                this.setLoading(false);
            });
        }
    }

    async logout(): Promise<void> {
        this.setLoading(true);
        
        try {
            await ApiService.post('/auth/logout');
            
            runInAction(() => {
                this.setUser(null);
            });
            
        } catch (error) {
            console.error('Logout failed:', error);
            // Even if logout fails, clear local state
            runInAction(() => {
                this.setUser(null);
            });
        } finally {
            runInAction(() => {
                this.setLoading(false);
            });
        }
    }

    async checkAuth(): Promise<void> {
        this.setLoading(true);
        
        try {
            const authStatus = await ApiService.get<AuthStatus>('/auth/status');
            
            if (authStatus.isAuthenticated && authStatus.name) {
                // Get user profile information
                try {
                    const profile = await ApiService.get<UserProfile>('/auth/profile');
                    
                    const user: User = {
                        id: profile.sub || profile.id || 'unknown',
                        email: profile.email || authStatus.name,
                        name: profile.name || authStatus.name,
                        roles: profile.roles || ['user']
                    };
                    
                    runInAction(() => {
                        this.setUser(user);
                    });
                } catch (profileError) {
                    console.error('Failed to fetch profile:', profileError);
                    
                    // Fallback to basic user info from auth status
                    const user: User = {
                        id: 'unknown',
                        email: authStatus.name || 'unknown@example.com',
                        name: authStatus.name || 'Unknown User',
                        roles: ['user']
                    };
                    
                    runInAction(() => {
                        this.setUser(user);
                    });
                }
            } else {
                runInAction(() => {
                    this.setUser(null);
                });
            }
        } catch (error) {
            console.error('Auth check failed:', error);
            runInAction(() => {
                this.setUser(null);
            });
        } finally {
            runInAction(() => {
                this.setLoading(false);
            });
        }
    }
}
