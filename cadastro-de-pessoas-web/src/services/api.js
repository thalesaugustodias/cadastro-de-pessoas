import axios from 'axios';

const getBaseURL = () => {
    const isDevelopment = import.meta.env.DEV;
    
    if (isDevelopment) {
        return import.meta.env.VITE_API_URL || 'https://localhost:5001/api/v1';
    }
    
    return import.meta.env.VITE_API_URL || 'https://cadastro-de-pessoas-vina.onrender.com/api/v1';
};

const api = axios.create({
    baseURL: getBaseURL(),
    headers: {
        'Content-Type': 'application/json; charset=utf-8',
    },
    timeout: 30000,
});

// Endpoints que não precisam de token
const publicEndpoints = [
    '/api/v1/Auth/login',
    '/api/v1/Auth/register',
    '/api/v1/Auth/logout',
    '/api/v1/Auth/reset-admin',
    '/health',
    '/health/database',
    '/health/reset-database'
];

api.interceptors.request.use(
    (config) => {
        const isPublicEndpoint = publicEndpoints.some(endpoint => 
            config.url?.includes(endpoint)
        );
        
        if (!isPublicEndpoint) {
            const token = localStorage.getItem('token');
            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
            }
        }
        
        if (import.meta.env.DEV) {
            console.log(`?? API Request: ${config.method?.toUpperCase()} ${config.url}`, {
                headers: config.headers,
                data: config.data,
                isPublic: isPublicEndpoint
            });
        }
        
        return config;
    },
    (error) => {
        console.error('? Request Error:', error);
        return Promise.reject(error);
    }
);

api.interceptors.response.use(
    (response) => {
        if (import.meta.env.DEV) {
            console.log(`? API Response: ${response.status}`, response.data);
        }
        
        return response;
    },
    (error) => {
        if (import.meta.env.DEV) {
            console.error(`? API Error: ${error.response?.status}`, {
                url: error.config?.url,
                method: error.config?.method,
                data: error.response?.data,
                message: error.message
            });
        }

        if (error.response) {
            const { status, data } = error.response;
            
            switch (status) {
                case 401:
                    if (!error.config?.url?.includes('/auth/login')) {
                        console.warn('Token inválido - redirecionando para login');
                        localStorage.removeItem('token');
                        localStorage.removeItem('user');
                        
                        if (window.location.pathname !== '/login') {
                            window.location.href = '/login';
                        }
                    }
                    break;
                    
                case 403:
                    console.warn('Acesso negado');
                    break;
                    
                case 404:
                    console.warn('Recurso não encontrado');
                    break;
                    
                case 422:
                    console.warn('Dados inválidos:', data);
                    break;
                    
                case 500:
                    console.error('Erro interno do servidor');
                    break;
                    
                default:
                    console.error(`Erro HTTP ${status}:`, data);
            }
        } else if (error.request) {
            console.error('Erro de rede - Servidor indisponível');
        }

        return Promise.reject(error);
    }
);

export default api;