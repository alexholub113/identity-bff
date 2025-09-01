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
        this.setLoading(true);
        this.setError('');
        
        try {
            // Fetch weather forecast data from the protected endpoint
            const weatherData = await ApiService.get<WeatherForecast[]>('/WeatherForecast');
            
            runInAction(() => {
                this.setWeatherForecast(weatherData);
            });

            // Create mock protected data that includes the weather info
            const mockData: ProtectedDataItem = {
                userId: userId,
                timestamp: new Date().toISOString(),
                secureData: `Weather forecast data retrieved successfully. ${weatherData.length} entries found.`,
                userPermissions: userRoles,
                sessionInfo: {
                    loginTime: new Date(Date.now() - Math.random() * 3600000).toISOString(),
                    expiresAt: new Date(Date.now() + 3600000).toISOString()
                }
            };
            
            runInAction(() => {
                this.setProtectedData(mockData);
            });
        } catch (err: any) {
            runInAction(() => {
                if (err.message.includes('401') || err.message.includes('status: 401')) {
                    this.setError('Authentication required. Please log in to access protected data.');
                } else {
                    this.setError('Failed to fetch protected data. Please try again.');
                }
            });
            console.error('Protected data fetch error:', err);
        } finally {
            runInAction(() => {
                this.setLoading(false);
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
                if (err.message.includes('401') || err.message.includes('status: 401')) {
                    this.setError('Authentication required. Please log in to access weather data.');
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

    clearData() {
        this.protectedData = null;
        this.weatherForecast = [];
        this.error = '';
    }
}
