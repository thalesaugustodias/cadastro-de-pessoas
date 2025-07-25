import React, { createContext, useState, useEffect } from 'react';
import { authService } from '../services/auth-service';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(localStorage.getItem('token'));
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const validateToken = async () => {
            if (token) {
                try {
                    // ?? Verificar token no backend
                    const verifyResponse = await authService.verifyToken();
                    
                    if (verifyResponse.valid) {
                        // Decodificar token para obter informações do usuário
                        const decodedToken = authService.decodeToken(token);
                        setUser({
                            id: decodedToken?.sub,
                            email: decodedToken?.email || verifyResponse.user?.email,
                            name: decodedToken?.name || verifyResponse.user?.name || 'Usuário',
                        });
                    } else {
                        logout();
                    }
                } catch (error) {
                    console.error('Token inválido:', error);
                    logout();
                }
            }
            setLoading(false);
        };

        validateToken();
    }, [token]);

    const login = async (email, senha) => {
        try {
            setLoading(true);
            
            // ?? Usar o serviço corrigido
            const response = await authService.login(email, senha);
            const { token: newToken } = response;

            // Armazenar token
            localStorage.setItem('token', newToken);
            setToken(newToken);

            // Decodificar token para obter dados do usuário
            const decodedToken = authService.decodeToken(newToken);
            setUser({
                id: decodedToken?.sub,
                email: decodedToken?.email || email,
                name: decodedToken?.name || 'Usuário',
            });

            return { success: true, message: response.message };
        } catch (error) {
            console.error('Erro no login:', error);
            
            let errorMessage = 'Erro ao realizar login';
            
            if (error.response?.data?.errors) {
                // Erros de validação do FluentValidation
                const errors = Object.values(error.response.data.errors).flat();
                errorMessage = errors.join(', ');
            } else if (error.response?.data?.detail) {
                // Erro específico da API
                errorMessage = error.response.data.detail;
            } else if (error.response?.data?.message) {
                // Mensagem de erro personalizada
                errorMessage = error.response.data.message;
            } else if (error.message) {
                // Erro do axios ou rede
                errorMessage = error.message;
            }

            return {
                success: false,
                message: errorMessage
            };
        } finally {
            setLoading(false);
        }
    };

    const logout = async () => {
        try {
            // Notificar o backend sobre logout
            await authService.logout();
        } catch (error) {
            // Ignora erros de logout
        } finally {
            // Limpar estado local
            localStorage.removeItem('token');
            setToken(null);
            setUser(null);
        }
    };

    const isTokenExpired = () => {
        if (!token) return true;
        
        try {
            const decodedToken = authService.decodeToken(token);
            const currentTime = Date.now() / 1000;
            return decodedToken.exp < currentTime;
        } catch (error) {
            return true;
        }
    };

    return (
        <AuthContext.Provider value={{
            user,
            token,
            authenticated: !!token && !isTokenExpired(),
            loading,
            login,
            logout,
            isTokenExpired
        }}>
            {children}
        </AuthContext.Provider>
    );
};
