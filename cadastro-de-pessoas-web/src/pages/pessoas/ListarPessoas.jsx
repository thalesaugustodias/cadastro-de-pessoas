import React from 'react';
import { Box, Heading, useDisclosure } from '@chakra-ui/react';
import PessoasList from '../../components/pessoas/PessoasList';
import Confirm from '../../components/ui/Confirm';
import { pessoaService } from '../../services/pessoa-service';
import { useNotification } from '../../hooks/useNotification';

const ListarPessoas = () => {
    const [pessoas, setPessoas] = React.useState([]);
    const [isLoading, setIsLoading] = React.useState(true);
    const [selectedId, setSelectedId] = React.useState(null);
    const [isDeleting, setIsDeleting] = React.useState(false);
    const { isOpen, onOpen, onClose } = useDisclosure();
    const { showSuccess, showError } = useNotification();

    React.useEffect(() => {
        fetchPessoas();
    }, []);

    const fetchPessoas = async () => {
        try {
            setIsLoading(true);
            const data = await pessoaService.listar();
            setPessoas(data);
        } catch (error) {
            console.error('Erro ao buscar pessoas:', error);
            showError('Não foi possível carregar a lista de pessoas');
        } finally {
            setIsLoading(false);
        }
    };

    const handleDeleteClick = (id) => {
        setSelectedId(id);
        onOpen();
    };

    const handleDelete = async () => {
        if (!selectedId) return;

        try {
            setIsDeleting(true);
            await pessoaService.remover(selectedId);
            setPessoas(pessoas.filter(p => p.id !== selectedId));
            showSuccess('Pessoa excluída com sucesso!');
        } catch (error) {
            console.error('Erro ao excluir pessoa:', error);
            showError('Erro ao excluir pessoa');
        } finally {
            setIsDeleting(false);
            onClose();
            setSelectedId(null);
        }
    };

    return (
        <Box p={6}>
            <Heading as="h1" size="lg" mb={6}>
                Gerenciar Pessoas
            </Heading>

            <PessoasList
                pessoas={pessoas}
                isLoading={isLoading}
                onDelete={handleDeleteClick}
            />

            <Confirm
                isOpen={isOpen}
                onClose={onClose}
                onConfirm={handleDelete}
                title="Excluir Pessoa"
                message="Tem certeza que deseja excluir esta pessoa? Esta ação não pode ser desfeita."
                isLoading={isDeleting}
            />
        </Box>
    );
};

export default ListarPessoas;