import React, { createContext, useState } from 'react';
import { useToast } from '@chakra-ui/react';

export const NotificationContext = createContext();

export const NotificationProvider = ({ children }) => {
    const toast = useToast();
    const [notifications, setNotifications] = useState([]);

    const showSuccess = (message, title = 'Sucesso') => {
        toast({
            title,
            description: message,
            status: 'success',
            duration: 5000,
            isClosable: true,
            position: 'top-right',
        });
    };

    const showError = (message, title = 'Erro') => {
        toast({
            title,
            description: message,
            status: 'error',
            duration: 5000,
            isClosable: true,
            position: 'top-right',
        });
    };

    const showWarning = (message, title = 'Atenção') => {
        toast({
            title,
            description: message,
            status: 'warning',
            duration: 5000,
            isClosable: true,
            position: 'top-right',
        });
    };

    const showInfo = (message, title = 'Informação') => {
        toast({
            title,
            description: message,
            status: 'info',
            duration: 5000,
            isClosable: true,
            position: 'top-right',
        });
    };

    return (
        <NotificationContext.Provider value={{
            notifications,
            showSuccess,
            showError,
            showWarning,
            showInfo
        }}>
            {children}
        </NotificationContext.Provider>
    );
};