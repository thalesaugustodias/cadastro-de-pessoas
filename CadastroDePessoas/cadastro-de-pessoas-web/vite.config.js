import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    esbuild: {
        loader: 'jsx',
        include: /src\/.*\.[jt]sx?$/,
        exclude: []
    },
    optimizeDeps: {
        esbuildOptions: {
            loader: {
                '.js': 'jsx',
                '.ts': 'tsx',
            },
        },
    },
    define: {
        // Definir variáveis globais se necessário
        global: 'globalThis',
    },
    server: {
        port: 3001,
        host: true,
        proxy: {
            '/api': {
                target: 'https://localhost:5001',
                changeOrigin: true,
                secure: false
            }
        }
    },
    build: {
        outDir: 'build',
        sourcemap: false
    }
})