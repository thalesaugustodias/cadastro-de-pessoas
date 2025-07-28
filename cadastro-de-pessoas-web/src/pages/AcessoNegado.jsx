import React from 'react';
import { FiLock } from 'react-icons/fi';
import ErrorPage from '../components/errors/ErrorPage';
import { useLocation } from 'react-router-dom';

const AcessoNegado = () => {
    const location = useLocation();
    const from = location.state?.from || '';
    const recurso = location.state?.resource || 'este recurso';

    return (
        <ErrorPage
            title="Acesso Negado"
            message={`Você não tem permissão para acessar ${recurso}. Entre em contato com o administrador se acredita que isso é um erro.`}
            icon={FiLock}
            statusCode="403"
        />
    );
};

export default AcessoNegado;