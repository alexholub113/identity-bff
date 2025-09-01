export class ApiService {
    private static getBaseUrl(): string {
        const baseUrl = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7108';
        return baseUrl;
    }

    private static async request<T>(
        url: string,
        options: RequestInit = {}
    ): Promise<T> {
        const response = await fetch(`${this.getBaseUrl()}${url}`, {
            ...options,
            credentials: 'include', // Important for cookie-based authentication
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const contentType = response.headers.get('content-type');
        if (contentType && contentType.includes('application/json')) {
            return response.json();
        }
        
        return response.text() as unknown as T;
    }

    static async get<T>(url: string): Promise<T> {
        return this.request<T>(url, { method: 'GET' });
    }

    static async post<T>(url: string, data?: any): Promise<T> {
        return this.request<T>(url, {
            method: 'POST',
            body: data ? JSON.stringify(data) : undefined,
        });
    }
}

export interface AuthStatus {
    isAuthenticated: boolean;
    name?: string;
    authenticationType?: string;
    claimsCount: number;
}

export interface UserProfile {
    sub?: string;
    id?: string;
    email?: string;
    name?: string;
    roles?: string[];
    [key: string]: any;
}

export interface WeatherForecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary?: string;
}
