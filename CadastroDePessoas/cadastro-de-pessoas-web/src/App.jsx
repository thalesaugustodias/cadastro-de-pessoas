import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { NotificationProvider } from './contexts/NotificationContext';
import AppRoutes from './routes';

const App = () => {
    return (
        <Router>
            <AuthProvider>
                <NotificationProvider>
                    <AppRoutes />
                </NotificationProvider>
            </AuthProvider>
        </Router>
    );
};

export default App;