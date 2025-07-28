import React, { useEffect } from 'react';
import { BrowserRouter as Router, useNavigate } from 'react-router-dom';
import { ChakraProvider } from '@chakra-ui/react';
import { AuthProvider } from './contexts/AuthContext';
import { NotificationProvider } from './contexts/NotificationContext';
import { useNotification } from './hooks/useNotification';
import { setupApiErrorHandling } from './services/api';
import theme from './theme';
import AppRoutes from './routes.jsx';
import './styles/icon-fixes.css';

const ApiErrorHandler = () => {
    const navigate = useNavigate();
    const notificationService = useNotification();
    
    useEffect(() => {
        setupApiErrorHandling(notificationService, navigate);
    }, [navigate, notificationService]);
    
    return null;
};

const App = () => {
    return (
        <ChakraProvider theme={theme}>
            <Router>
                <AuthProvider>
                    <NotificationProvider>
                        <ApiErrorHandler />
                        <AppRoutes />
                    </NotificationProvider>
                </AuthProvider>
            </Router>
        </ChakraProvider>
    );
};

export default App;