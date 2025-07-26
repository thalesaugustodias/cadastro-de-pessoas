import api from './api';

export const authService = {
    login: async (email, senha) => {
        try {
            const response = await api.post('/api/v1/auth/login', {
                email,
                senha
            });

            if (response.data.success && response.data.token) {
                localStorage.setItem('token', response.data.token);
                localStorage.setItem('user', JSON.stringify(response.data.user));
                
                return {
                    success: true,
                    token: response.data.token,
                    user: response.data.user,
                    message: response.data.message
                };
            }

            return {
                success: false,
                message: response.data.message || 'Erro no login'
            };
        } catch (error) {
            
            if (error.response?.data?.message) {
                return {
                    success: false,
                    message: error.response.data.message
                };
            }

            if (error.response?.status === 401) {
                return {
                    success: false,
                    message: 'Email ou senha incorretos'
                };
            }

            return {
                success: false,
                message: 'Erro de conex�o. Verifique se o servidor est� rodando.'
            };
        }
    },

    register: async (nome, email, senha) => {
        try {
            const response = await api.post('/api/v1/Auth/register', {
                nome,
                email,
                senha
            });
            if (response.data.success && response.data.token) {
                localStorage.setItem('token', response.data.token);
                localStorage.setItem('user', JSON.stringify(response.data.user));
                return response.data;
            }
            return {
                success: false,
                message: response.data.message || 'Erro no registro'
            };
        } catch (error) {
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            if (error.response?.status === 401) {
                throw new Error('Erro ao registrar. Verifique as credenciais.');
            }
            throw new Error('Erro de conex�o. Verifique se o servidor est� rodando.');
        }
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        return Promise.resolve();
    },

    verifyToken: async () => {
        try {
            const response = await api.get('/api/v1/auth/verify');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    getProfile: async () => {
        try {
            const response = await api.get('/api/v1/auth/profile');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    updateProfile: async (profileData) => {
        try {
            const response = await api.put('/api/v1/auth/profile', profileData);
            
            if (response.data.success) {
                localStorage.setItem('user', JSON.stringify(response.data.user));
            }
            
            return response.data;
        } catch (error) {
            throw error;
        }
    },   

    getCurrentUser: () => {
        try {
            const userStr = localStorage.getItem('user');
            return userStr ? JSON.parse(userStr) : null;
        } catch (error) {
            console.error('Erro ao recuperar usu�rio:', error);
            return null;
        }
    },

    getToken: () => {
        return localStorage.getItem('token');
    },

    isAuthenticated: () => {
        const token = localStorage.getItem('token');
        return !!token && !authService.isTokenExpired(token);
    },

    decodeToken: (token) => {
        try {
            if (!token) return null;
            
            const parts = token.split('.');
            if (parts.length !== 3) {
                throw new Error('Token JWT inv�lido');
            }

            const payload = parts[1];
            
            const paddedPayload = payload + '=='.substring(0, (4 - payload.length % 4) % 4);
            
            const decodedPayload = atob(paddedPayload.replace(/-/g, '+').replace(/_/g, '/'));
            
            return JSON.parse(decodedPayload);
        } catch (error) {
            console.error('Erro ao decodificar token:', error);
            return null;
        }
    },

    isTokenExpired: (token) => {
        try {
            const decodedToken = authService.decodeToken(token);
            if (!decodedToken || !decodedToken.exp) {
                return true;
            }
            
            const currentTime = Date.now() / 1000;
            return decodedToken.exp < currentTime;
        } catch (error) {
            return true;
        }
    }
};

export const healthService = {
    checkHealth: async () => {
        try {
            const response = await api.get('/api/v1/health');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    checkDatabase: async () => {
        try {
            const response = await api.get('/api/v1/health/database');
            return response.data;
        } catch (error) {
            throw error;
        }
    }    
};