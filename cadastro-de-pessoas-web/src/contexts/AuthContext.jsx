import React, { createContext, useState, useEffect } from 'react';
import { authService } from '../services/auth-service';

export const AuthContext = createContext({
    authenticated: false,
    user: null,
    loading: true,
    login: () => {},
    logout: () => {},
    updateUser: () => {},
    isAuthenticated: () => false
});

export const AuthProvider = ({ children }) => {
    const [authenticated, setAuthenticated] = useState(false);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const checkAuth = async () => {
            setLoading(true);
            
            const token = authService.getToken();
            
            if (token) {
                try {
                    if (authService.isTokenExpired(token)) {
                        
                        authService.logout();
                        setAuthenticated(false);
                        setUser(null);
                        setLoading(false);
                        return;
                    }

                    const storedUser = authService.getCurrentUser();
                    if (storedUser) {
                        setAuthenticated(true);
                        setUser(storedUser);
                        setLoading(false);
                        return;
                    }

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

    const isAuthenticated = () => {
        return authenticated && user !== null;
    };

    const value = {
        authenticated,
        user,
        loading,
        login,
        logout,
        updateUser,
        isAuthenticated
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};
