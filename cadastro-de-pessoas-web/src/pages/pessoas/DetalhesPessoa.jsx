import React from 'react';
import { Box, Heading, Text, Breadcrumb, BreadcrumbItem, BreadcrumbLink, Flex, Icon } from '@chakra-ui/react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { FiHome, FiUsers, FiUser } from 'react-icons/fi';
import PessoaItem from '../../components/pessoas/PessoaItem';
import { pessoaService } from '../../services/pessoa-service';
import { useNotification } from '../../hooks/useNotification';
import Loading from '../../components/ui/Loading';

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
        <Box p={6}>
            {/* Breadcrumb de navegação */}
            <Breadcrumb mb={6} fontSize="sm" separator=">" color="gray.500">
                <BreadcrumbItem>
                    <BreadcrumbLink as={Link} to="/" display="flex" alignItems="center">
                        <Icon as={FiHome} mr={1} />
                        Início
                    </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbItem>
                    <BreadcrumbLink as={Link} to="/pessoas" display="flex" alignItems="center">
                        <Icon as={FiUsers} mr={1} />
                        Pessoas
                    </BreadcrumbLink>
                </BreadcrumbItem>
                <BreadcrumbItem isCurrentPage fontWeight="semibold">
                    <Flex align="center">
                        <Icon as={FiUser} mr={1} />
                        <Text>Detalhes da Pessoa</Text>
                    </Flex>
                </BreadcrumbItem>
            </Breadcrumb>

            {/* Título da página */}
            <Heading as="h1" size="lg" mb={6} color="gray.700" display="flex" alignItems="center">
                <Icon as={FiUser} mr={2} color="brand.500" />
                Detalhes da Pessoa
            </Heading>

            {isLoading ? (
                <Loading text="Carregando detalhes da pessoa..." />
            ) : (
                <PessoaItem
                    pessoa={pessoa}
                    isLoading={false}
                    onDelete={handleDelete}
                />
            )}
        </Box>
    );
};

export default DetalhesPessoa;