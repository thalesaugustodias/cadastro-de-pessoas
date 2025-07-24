import api from './api';

export const authService = {
    login: async (email, senha) => {
        const response = await api.post('/usuarios/login', { email, senha });
        return response.data;
    },
};