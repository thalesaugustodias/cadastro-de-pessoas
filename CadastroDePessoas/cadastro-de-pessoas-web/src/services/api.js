import axios from 'axios';

const getBaseURL = () => {
    // No Vite, variáveis de ambiente são acessadas via import.meta.env
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

api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

api.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        if (error.response && error.response.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default api;