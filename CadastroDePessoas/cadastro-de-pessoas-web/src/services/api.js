import axios from 'axios';

const getBaseURL = () => {
    // No Vite, vari�veis de ambiente s�o acessadas via import.meta.env
    const isDevelopment = import.meta.env.DEV;
    
    if (isDevelopment) {
        return import.meta.env.VITE_API_URL || 'https://localhost:5001/api';
    }
    
    return '/api';
};

const api = axios.create({
    baseURL: getBaseURL(),
    headers: {
        'Content-Type': 'application/json',
    },
});

// ?? Interceptor de REQUEST - Adicionar token
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        
        // Log para debug (remover em produ��o)
        if (import.meta.env.DEV) {
            console.log(`?? API Request: ${config.method?.toUpperCase()} ${config.url}`, {
                headers: config.headers,
                data: config.data
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
        // Log para debug (remover em produ��o)
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
                    // Token inv�lido ou expirado
                    console.warn('?? Token inv�lido - redirecionando para login');
                    localStorage.removeItem('token');
                    
                    // S� redireciona se n�o estiver j� na p�gina de login
                    if (window.location.pathname !== '/login') {
                        window.location.href = '/login';
                    }
                    break;
                    
                case 403:
                    console.warn('?? Acesso negado');
                    break;
                    
                case 404:
                    console.warn('?? Recurso n�o encontrado');
                    break;
                    
                case 422:
                    console.warn('?? Dados inv�lidos:', data);
                    break;
                    
                case 500:
                    console.error('?? Erro interno do servidor');
                    break;
                    
                default:
                    console.error(`?? Erro HTTP ${status}:`, data);
            }
        } else if (error.request) {
            // Erro de rede
            console.error('?? Erro de rede - Servidor indispon�vel');
        }

        return Promise.reject(error);
    }
);

export default api;