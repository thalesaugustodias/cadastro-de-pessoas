import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { ChakraProvider } from '@chakra-ui/react';
import { AuthProvider } from './contexts/AuthContext';
import { NotificationProvider } from './contexts/NotificationContext';
import theme from './theme';
import AppRoutes from './routes.jsx';

const App = () => {
    return (
        <ChakraProvider theme={theme}>
            <Router>
                <AuthProvider>
                    <NotificationProvider>
                        <AppRoutes />
                    </NotificationProvider>
                </AuthProvider>
            </Router>
        </ChakraProvider>
    );
};

export default App;