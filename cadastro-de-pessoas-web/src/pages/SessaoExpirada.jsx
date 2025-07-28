import React from 'react';
import { FiRefreshCw } from 'react-icons/fi';
import { Button } from '@chakra-ui/react';
import ErrorPage from '../components/errors/ErrorPage';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

const SessaoExpirada = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const { logout } = useAuth();
    
    const from = location.state?.from || '/';
    
    const handleLogin = () => {
        logout();
        navigate('/login', { state: { redirectTo: from } });
    };

    const customButtons = (
        <Button
            colorScheme="brand"
            leftIcon={<FiRefreshCw />}
            size="lg"
            onClick={handleLogin}
        >
            Fazer Login Novamente
        </Button>
    );

    return (
        <ErrorPage
            title="Sessão Expirada"
            message="Sua sessão expirou ou foi invalidada. Por favor, faça login novamente para continuar."
            icon={FiRefreshCw}
            statusCode=""
            showHomeButton={false}
            showBackButton={false}
            customButtons={customButtons}
        />
    );
};

export default SessaoExpirada;