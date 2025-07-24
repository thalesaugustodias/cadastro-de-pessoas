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
                    // Poderia verificar o token no backend se necess�rio
                    setUser({ nome: 'Usu�rio Autenticado' }); // Exemplo simples
                    setLoading(false);
                } catch (error) {
                    logout();
                    setLoading(false);
                }
            } else {
                setLoading(false);
            }
        };

        validateToken();
    }, [token]);

    const login = async (email, senha) => {
        try {
            const response = await authService.login(email, senha);
            const { token } = response;

            localStorage.setItem('token', token);
            setToken(token);
            setUser({ nome: 'Usu�rio Autenticado' }); // Em uma aplica��o real, voc� decodificaria o token ou buscaria os dados do usu�rio

            return { success: true };
        } catch (error) {
            return {
                success: false,
                message: error.response?.data?.detail || 'Erro ao realizar login'
            };
        }
    };

    const logout = () => {
        localStorage.removeItem('token');
        setToken(null);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{
            user,
            token,
            authenticated: !!token,
            loading,
            login,
            logout
        }}>
            {children}
        </AuthContext.Provider>
    );
};
