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
                    if (authService.isTokenExpired(token)) {
                        // Apenas faça logout se o token estiver realmente expirado
                        console.log("Token expirado, fazendo logout");
                        authService.logout();
                        setAuthenticated(false);
                        setUser(null);
                        setLoading(false);
                        return;
                    }

                    // Tente obter os dados do usuário do localStorage primeiro para evitar
                    // requisições desnecessárias ao recarregar a página
                    const storedUser = authService.getCurrentUser();
                    if (storedUser) {
                        setAuthenticated(true);
                        setUser(storedUser);
                        setLoading(false);
                        return;
                    }

                    // Se não tiver usuário no localStorage, então faça a requisição
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
                    // Em caso de erro de conexão, mantenha o usuário logado se tiver dados no localStorage
                    const storedUser = authService.getCurrentUser();
                    if (storedUser) {
                        setAuthenticated(true);
                        setUser(storedUser);
                    } else {
                        authService.logout();
                        setAuthenticated(false);
                        setUser(null);
                    }
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
