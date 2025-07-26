import React, { createContext, useState, useEffect } from 'react';
import { authService } from '../services/auth-service';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [authenticated, setAuthenticated] = useState(false);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const checkAuth = async () => {
            const token = authService.getToken();
            
            if (token) {
                try {
                    // Verificar se o token não está expirado
                    if (authService.isTokenExpired(token)) {
                        authService.logout();
                        setAuthenticated(false);
                        setUser(null);
                        setLoading(false);
                        return;
                    }

                    // Verificar token no backend
                    const response = await authService.verifyToken();
                    if (response.valid) {
                        setAuthenticated(true);
                        setUser(response.user);
                    } else {
                        authService.logout();
                        setAuthenticated(false);
                        setUser(null);
                    }
                } catch (error) {
                    console.error('Erro ao verificar token:', error);
                    authService.logout();
                    setAuthenticated(false);
                    setUser(null);
                }
            }
            
            setLoading(false);
        };

        checkAuth();
    }, []);

    const login = async (email, senha) => {
        try {
            const response = await authService.login(email, senha);
            
            if (response.success) {
                setAuthenticated(true);
                setUser(response.user);
            }
            
            return response;
        } catch (error) {
            console.error('Erro no login:', error);
            return {
                success: false,
                message: 'Erro de conexão. Tente novamente.'
            };
        }
    };

    const logout = () => {
        authService.logout();
        setAuthenticated(false);
        setUser(null);
    };

    const updateUser = (userData) => {
        setUser(userData);
    };

    const value = {
        authenticated,
        user,
        loading,
        login,
        logout,
        updateUser
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};
