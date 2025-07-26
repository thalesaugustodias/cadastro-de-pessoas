import axios from 'axios';

const getBaseURL = () => {
    // No Vite, variáveis de ambiente são acessadas via import.meta.env
    const isDevelopment = import.meta.env.DEV;
    
    if (isDevelopment) {
        return import.meta.env.VITE_API_URL || 'https://localhost:5001/api/v1';
    }
    
    return '/api/v1';
};

const api = axios.create({
    baseURL: getBaseURL(),
    headers: {
        'Content-Type': 'application/json; charset=utf-8',
    },
});

// Endpoints que não precisam de token
const publicEndpoints = [
    '/auth/login',
    '/auth/register',
    '/auth/logout',
    '/auth/reset-admin',
    '/health',
    '/health/database',
    '/health/reset-database'
];

// ?? Interceptor de REQUEST - Adicionar token apenas quando necessário
api.interceptors.request.use(
    (config) => {
        // Verificar se é um endpoint público
        const isPublicEndpoint = publicEndpoints.some(endpoint => 
            config.url?.includes(endpoint)
        );
        
        // Só adicionar token se não for endpoint público
        if (!isPublicEndpoint) {
            const token = localStorage.getItem('token');
            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
            }
        }
        
        // Log para debug (remover em produção)
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

// ?? Interceptor de RESPONSE - Tratar respostas e erros
api.interceptors.response.use(
    (response) => {
        // Log para debug (remover em produção)
        if (import.meta.env.DEV) {
            console.log(`? API Response: ${response.status}`, response.data);
        }
        
        return response;
    },
    (error) => {
        // Log para debug
        if (import.meta.env.DEV) {
            console.error(`? API Error: ${error.response?.status}`, {
                url: error.config?.url,
                method: error.config?.method,
                data: error.response?.data,
                message: error.message
            });
        }

        // Tratar diferentes tipos de erro
        if (error.response) {
            const { status, data } = error.response;
            
            switch (status) {
                case 401:
                    // Token inválido ou expirado - mas só redirecionar se não for tentativa de login
                    if (!error.config?.url?.includes('/auth/login')) {
                        console.warn('?? Token inválido - redirecionando para login');
                        localStorage.removeItem('token');
                        localStorage.removeItem('user');
                        
                        // Só redireciona se não estiver já na página de login
                        if (window.location.pathname !== '/login') {
                            window.location.href = '/login';
                        }
                    }
                    break;
                    
                case 403:
                    console.warn('? Acesso negado');
                    break;
                    
                case 404:
                    console.warn('?? Recurso não encontrado');
                    break;
                    
                case 422:
                    console.warn('?? Dados inválidos:', data);
                    break;
                    
                case 500:
                    console.error('?? Erro interno do servidor');
                    break;
                    
                default:
                    console.error(`?? Erro HTTP ${status}:`, data);
            }
        } else if (error.request) {
            // Erro de rede
            console.error('?? Erro de rede - Servidor indisponível');
        }

        return Promise.reject(error);
    }
);

export default api;