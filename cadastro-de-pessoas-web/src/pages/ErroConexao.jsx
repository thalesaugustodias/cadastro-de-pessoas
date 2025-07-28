import React from 'react';
import { FiWifiOff } from 'react-icons/fi';
import ErrorPage from '../components/errors/ErrorPage';
import { useLocation } from 'react-router-dom';

const ErroConexao = () => {
    const location = useLocation();
    const isOffline = location.state?.offline || false;

    return (
        <ErrorPage
            title="Erro de Conexão"
            message={
                isOffline 
                    ? "Parece que você está offline ou o servidor não está respondendo. Verifique sua conexão de internet e tente novamente."
                    : "Não foi possível conectar ao servidor. Verifique sua conexão ou tente novamente mais tarde."
            }
            icon={FiWifiOff}
            statusCode=""
        />
    );
};

export default ErroConexao;