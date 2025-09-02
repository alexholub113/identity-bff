import { makeAutoObservable, runInAction } from 'mobx';
import { ApiService, WeatherForecast } from '../services/apiService';

export interface ProtectedDataItem {
    userId: string;
    timestamp: string;
    secureData: string;
    userPermissions: string[];
    sessionInfo: {
        loginTime: string;
        expiresAt: string;
    };
}

export class DataStore {
    protectedData: ProtectedDataItem | null = null;
    weatherForecast: WeatherForecast[] = [];
    isLoading = false;
    error = '';

    constructor() {
        makeAutoObservable(this);
    }

    setLoading(loading: boolean) {
        this.isLoading = loading;
    }

    setError(error: string) {
        this.error = error;
    }

    setProtectedData(data: ProtectedDataItem | null) {
        this.protectedData = data;
    }

    setWeatherForecast(data: WeatherForecast[]) {
        this.weatherForecast = data;
    }

    async fetchProtectedData(userId: string, userRoles: string[] = []): Promise<void> {
        // First fetch the weather data
        await this.fetchWeatherForecast();
        
        // If we got here, the weather fetch was successful, so create mock protected data
        if (this.weatherForecast.length > 0 && !this.error) {
            const mockData: ProtectedDataItem = {
                userId: userId,
                timestamp: new Date().toISOString(),
                secureData: `Weather forecast data retrieved successfully. ${this.weatherForecast.length} entries found.`,
                userPermissions: userRoles,
                sessionInfo: {
                    loginTime: new Date(Date.now() - Math.random() * 3600000).toISOString(),
                    expiresAt: new Date(Date.now() + 3600000).toISOString()
                }
            };
            
            runInAction(() => {
                this.setProtectedData(mockData);
            });
        }
    }

    async fetchWeatherForecast(): Promise<void> {
        this.setLoading(true);
        this.setError('');
        
        try {
            const weatherData = await ApiService.get<WeatherForecast[]>('/WeatherForecast');
            
            runInAction(() => {
                this.setWeatherForecast(weatherData);
            });
        } catch (err: any) {
            runInAction(() => {
                if (err.message.includes('401') || err.message.includes('status: 401') || 
                    err.message.includes('302') || err.message.includes('status: 302')) {
                    // Instead of showing error, automatically trigger authentication
                    this.setError('Redirecting to login...');
                    this.triggerAuthenticationChallenge();
                } else {
                    this.setError('Failed to fetch weather data. Please try again.');
                }
            });
            console.error('Weather forecast fetch error:', err);
        } finally {
            runInAction(() => {
                this.setLoading(false);
            });
        }
    }

    private triggerAuthenticationChallenge(): void {
        // Redirect to the auth login endpoint which will handle the OAuth2 challenge
        const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7108';
        const currentUrl = window.location.href;
        const loginUrl = new URL(`/auth/login`, baseUrl);
        loginUrl.searchParams.set('returnUrl', currentUrl);
        
        // Trigger the authentication challenge by redirecting to login
        window.location.href = loginUrl.toString();
    }

    clearData() {
        this.protectedData = null;
        this.weatherForecast = [];
        this.error = '';
    }
}
