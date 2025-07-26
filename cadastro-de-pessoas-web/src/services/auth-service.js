import api from './api';

export const authService = {
    login: async (email, senha) => {
        try {
            const response = await api.post('/auth/login', {
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
            console.error('Erro no login:', error);
            
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
                message: 'Erro de conexão. Verifique se o servidor está rodando.'
            };
        }
    },

    register: async (nome, email, senha) => {
        try {
            const response = await api.post('/auth/register', {
                nome,
                email,
                senha
            });

            return response.data;
        } catch (error) {
            if (error.response?.data?.message) {
                throw new Error(error.response.data.message);
            }
            throw error;
        }
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        return Promise.resolve();
    },

    verifyToken: async () => {
        try {
            const response = await api.get('/auth/verify');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    getProfile: async () => {
        try {
            const response = await api.get('/auth/profile');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    updateProfile: async (profileData) => {
        try {
            const response = await api.put('/auth/profile', profileData);
            
            if (response.data.success) {
                // Atualizar dados do usuário no localStorage
                localStorage.setItem('user', JSON.stringify(response.data.user));
            }
            
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    resetAdmin: async () => {
        try {
            const response = await api.post('/auth/reset-admin');
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
            console.error('Erro ao recuperar usuário:', error);
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

    // Função para decodificar JWT token
    decodeToken: (token) => {
        try {
            if (!token) return null;
            
            // JWT tem 3 partes separadas por pontos: header.payload.signature
            const parts = token.split('.');
            if (parts.length !== 3) {
                throw new Error('Token JWT inválido');
            }

            // Decodificar o payload (segunda parte)
            const payload = parts[1];
            
            // Adicionar padding se necessário para o base64url
            const paddedPayload = payload + '=='.substring(0, (4 - payload.length % 4) % 4);
            
            // Decodificar de base64url para string
            const decodedPayload = atob(paddedPayload.replace(/-/g, '+').replace(/_/g, '/'));
            
            // Converter string JSON para objeto
            return JSON.parse(decodedPayload);
        } catch (error) {
            console.error('Erro ao decodificar token:', error);
            return null;
        }
    },

    // Função para verificar se o token está expirado
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
            const response = await api.get('/health');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    checkDatabase: async () => {
        try {
            const response = await api.get('/health/database');
            return response.data;
        } catch (error) {
            throw error;
        }
    },

    resetDatabase: async () => {
        try {
            const response = await api.post('/health/reset-database');
            return response.data;
        } catch (error) {
            throw error;
        }
    }
};