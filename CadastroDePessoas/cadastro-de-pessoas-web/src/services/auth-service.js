import api from './api';

export const authService = {
    login: async (email, senha) => {
        // ?? FIX: Endpoint correto para V1 da API
        const response = await api.post('/v1/auth/login', { email, senha });
        return response.data;
    },

    loginV2: async (email, senha) => {
        // Endpoint para V2 da API com informações extras
        const response = await api.post('/v2/auth/login', { email, senha });
        return response.data;
    },

    logout: async () => {
        try {
            await api.post('/v1/auth/logout');
        } catch (error) {
            // Ignora erros de logout, apenas remove token local
            console.warn('Erro no logout:', error);
        }
    },

    verifyToken: async () => {
        // Verifica se o token é válido no backend
        const response = await api.get('/v1/auth/verify');
        return response.data;
    },

    // Utilitário para decodificar JWT (básico)
    decodeToken: (token) => {
        try {
            const payload = token.split('.')[1];
            const decodedPayload = atob(payload);
            return JSON.parse(decodedPayload);
        } catch (error) {
            console.error('Erro ao decodificar token:', error);
            return null;
        }
    }
};