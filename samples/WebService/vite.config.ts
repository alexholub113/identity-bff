import react from '@vitejs/plugin-react'
import { defineConfig, loadEnv } from 'vite'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '')
    
    return {
        plugins: [react()],
        server: {
            port: 3000,
            open: true,
            proxy: {
                '/api': {
                    target: env.VITE_API_BASE_URL || 'https://localhost:7108',
                    changeOrigin: true,
                    secure: false
                }
            }
        }
    }
})
