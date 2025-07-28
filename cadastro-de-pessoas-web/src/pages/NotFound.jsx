import React from 'react';
import { FiAlertTriangle } from 'react-icons/fi';
import ErrorPage from '../components/errors/ErrorPage';
import { useLocation } from 'react-router-dom';

const NotFound = () => {
    const location = useLocation();
    const resource = location.state?.resource || 'página';

    return (
        <ErrorPage
            title="Página não encontrada"
            message={`A ${resource} que você está procurando não existe ou foi movida.`}
            icon={FiAlertTriangle}
            statusCode="404"
        />
    );
};

export default NotFound;