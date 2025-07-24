import React from 'react';
import { Box, Heading } from '@chakra-ui/react';
import { useNavigate } from 'react-router-dom';
import PessoaForm from '../../components/pessoas/PessoaForm';
import { pessoaService } from '../../services/pessoa-service';
import { useNotification } from '../../hooks/useNotification';

const CriarPessoa = () => {
    const [isLoading, setIsLoading] = React.useState(false);
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotification();

    const handleSubmit = async (data) => {
        try {
            setIsLoading(true);
            await pessoaService.criar(data);
            showSuccess('Pessoa cadastrada com sucesso!');
            navigate('/pessoas');
        } catch (error) {
            console.error('Erro ao cadastrar pessoa:', error);

            // Verificar se é um erro de validação do servidor
            if (error.response?.data?.errors) {
                const erros = error.response.data.errors;
                const mensagens = Object.values(erros).flat();
                mensagens.forEach(msg => showError(msg));
            } else {
                showError('Erro ao cadastrar pessoa');
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Box>
            <Heading as="h1" size="lg" mb={6}>
                Cadastrar Nova Pessoa
            </Heading>

            <PessoaForm
                onSubmit={handleSubmit}
                isLoading={isLoading}
            />
        </Box>
    );
};

export default CriarPessoa;