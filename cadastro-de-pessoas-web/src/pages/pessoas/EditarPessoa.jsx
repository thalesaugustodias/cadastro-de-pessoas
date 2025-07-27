import React from 'react';
import { Box, Heading } from '@chakra-ui/react';
import { useParams, useNavigate } from 'react-router-dom';
import PessoaForm from '../../components/pessoas/PessoaForm';
import Loading from '../../components/ui/Loading';
import { pessoaService } from '../../services/pessoa-service';
import { useNotification } from '../../hooks/useNotification';

const EditarPessoa = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { showSuccess, showError } = useNotification();
    const [pessoa, setPessoa] = React.useState(null);
    const [isLoading, setIsLoading] = React.useState(true);
    const [isSubmitting, setIsSubmitting] = React.useState(false);

    React.useEffect(() => {
        const fetchPessoa = async () => {
            try {
                const data = await pessoaService.obterPorId(id);
                setPessoa(data);
            } catch (error) {
                console.error('Erro ao buscar pessoa:', error);
                showError('Não foi possível carregar os dados da pessoa');
                navigate('/pessoas');
            } finally {
                setIsLoading(false);
            }
        };

        fetchPessoa();
    }, [id, navigate, showError]);

    const handleSubmit = async (data) => {
        try {
            setIsSubmitting(true);
            await pessoaService.atualizar(data);
            showSuccess('Pessoa atualizada com sucesso!');
            navigate(`/pessoas/${id}`);
        } catch (error) {
            console.error('Erro ao atualizar pessoa:', error);

            if (error.response?.data?.errors) {
                const erros = error.response.data.errors;
                const mensagens = Object.values(erros).flat();
                mensagens.forEach(msg => showError(msg));
            } else {
                showError('Erro ao atualizar pessoa');
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    if (isLoading) {
        return <Loading />;
    }

    return (
        <Box p={6}>
            <Heading as="h1" size="lg" mb={6}>
                Editar Pessoa
            </Heading>

            <PessoaForm
                initialData={pessoa}
                onSubmit={handleSubmit}
                isLoading={isSubmitting}
                isEdit={true}
            />
        </Box>
    );
};

export default EditarPessoa;