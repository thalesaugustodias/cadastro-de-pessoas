import React from 'react';
import { FiAlertOctagon } from 'react-icons/fi';
import ErrorPage from '../components/errors/ErrorPage';
import { useLocation } from 'react-router-dom';

const ErroServidor = () => {
    const location = useLocation();
    const statusCode = location.state?.statusCode || 500;
    const errorMessage = location.state?.errorMessage || 'Ocorreu um erro interno no servidor.';

    return (
        <ErrorPage
            title="Erro do Servidor"
            message={`${errorMessage} Nossa equipe foi notificada e está trabalhando para resolver o problema. Por favor, tente novamente mais tarde.`}
            icon={FiAlertOctagon}
            statusCode={statusCode}
        />
    );
};

export default ErroServidor;