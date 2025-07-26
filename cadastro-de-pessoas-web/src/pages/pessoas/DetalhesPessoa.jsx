import React from 'react';
import { Box } from '@chakra-ui/react';
import { useParams, useNavigate } from 'react-router-dom';
import PessoaItem from '../../components/pessoas/PessoaItem';
import { pessoaService } from '../../services/pessoa-service';
import { useNotification } from '../../hooks/useNotification';

const DetalhesPessoa = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotification();
    const [pessoa, setPessoa] = React.useState(null);
    const [isLoading, setIsLoading] = React.useState(true);

    React.useEffect(() => {
        fetchPessoa();
    }, [id]);

    const fetchPessoa = async () => {
        try {
            setIsLoading(true);
            const data = await pessoaService.obterPorId(id);
            setPessoa(data);
        } catch (error) {
            console.error('Erro ao buscar pessoa:', error);
            showError('Não foi possível carregar os dados da pessoa');
        } finally {
            setIsLoading(false);
        }
    };

    const handleDelete = async (id) => {
        try {
            await pessoaService.remover(id);
            showSuccess('Pessoa excluída com sucesso!');
            navigate('/pessoas');
        } catch (error) {
            console.error('Erro ao excluir pessoa:', error);
            showError('Erro ao excluir pessoa');
            return false;
        }
    };

    return (
        <Box>
            <PessoaItem
                pessoa={pessoa}
                isLoading={isLoading}
                onDelete={handleDelete}
            />
        </Box>
    );
};

export default DetalhesPessoa;