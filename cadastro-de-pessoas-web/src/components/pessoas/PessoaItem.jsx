import React from 'react';
import {
    Box,
    Heading,
    Text,
    Divider,
    SimpleGrid,
    Badge,
    HStack,
    Button,
    Flex,
    useDisclosure,
} from '@chakra-ui/react';

import { FiEdit, FiTrash2, FiArrowLeft } from 'react-icons/fi';
import { Link } from 'react-router-dom';
import { formatarData, formatarCPF } from '../../utils/formatters';
import Confirm from '../ui/Confirm';
import Loading from '../ui/Loading';

const InfoItem = ({ label, value, placeholder = 'N�o informado' }) => (
    <Box mb={4}>
        <Text fontSize="sm" color="gray.500" fontWeight="medium">
            {label}
        </Text>
        <Text fontSize="md">{value || placeholder}</Text>
    </Box>
);

const PessoaItem = ({
    pessoa,
    isLoading,
    onDelete
}) => {
    const { isOpen, onOpen, onClose } = useDisclosure();
    const [isDeleting, setIsDeleting] = React.useState(false);

    const handleDelete = async () => {
        setIsDeleting(true);
        await onDelete(pessoa.id);
        setIsDeleting(false);
        onClose();
    };

    if (isLoading) {
        return <Loading />;
    }

    if (!pessoa) {
        return (
            <Box textAlign="center" py={10}>
                <Text fontSize="lg" mb={4}>Pessoa não encontrada</Text>
                <Button
                    as={Link}
                    to="/pessoas"
                    colorScheme="blue"
                    leftIcon={<FiArrowLeft />}
                >
                    Voltar para a lista
                </Button>
            </Box>
        );
    }

    return (
        <Box>
            <Flex
                justify="space-between"
                align={{ base: 'flex-start', md: 'center' }}
                direction={{ base: 'column', md: 'row' }}
                gap={4}
                mb={6}
            >
                <Box>
                    <Heading as="h2" size="lg">
                        {pessoa.nome}
                    </Heading>
                    <HStack mt={2} spacing={2}>
                        {pessoa.sexo === 0 && <Badge colorScheme="blue">Masculino</Badge>}
                        {pessoa.sexo === 1 && <Badge colorScheme="pink">Feminino</Badge>}
                        {pessoa.sexo === 2 && <Badge colorScheme="purple">Outro</Badge>}
                        {pessoa.sexo === null && <Badge colorScheme="gray">Sexo não informado</Badge>}

                        <Badge colorScheme="green">{pessoa.idade} anos</Badge>
                    </HStack>
                </Box>

                <HStack spacing={4}>
                    <Button
                        as={Link}
                        to="/pessoas"
                        leftIcon={<FiArrowLeft />}
                        variant="outline"
                    >
                        Voltar
                    </Button>

                    <Button
                        as={Link}
                        to={`/pessoas/editar/${pessoa.id}`}
                        leftIcon={<FiEdit />}
                        colorScheme="green"
                    >
                        Editar
                    </Button>

                    <Button
                        leftIcon={<FiTrash2 />}
                        colorScheme="red"
                        onClick={onOpen}
                    >
                        Excluir
                    </Button>
                </HStack>
            </Flex>

            <Divider mb={6} />

            <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
                <Box>
                    <Heading as="h3" size="md" mb={4}>
                        Informações Pessoais
                    </Heading>

                    <InfoItem
                        label="CPF"
                        value={formatarCPF(pessoa.cpf)}
                    />

                    <InfoItem
                        label="Data de Nascimento"
                        value={formatarData(pessoa.dataNascimento)}
                    />

                    <InfoItem
                        label="E-mail"
                        value={pessoa.email}
                    />

                    <InfoItem
                        label="Naturalidade"
                        value={pessoa.naturalidade}
                    />

                    <InfoItem
                        label="Nacionalidade"
                        value={pessoa.nacionalidade}
                    />
                </Box>

                <Box>
                    <Heading as="h3" size="md" mb={4}>
                        Informações Adicionais
                    </Heading>

                    <InfoItem
                        label="Endereço"
                        value={pessoa.endereco}
                    />

                    <InfoItem
                        label="Data de Cadastro"
                        value={formatarData(pessoa.dataCadastro)}
                    />

                    {pessoa.dataAtualizacao && (
                        <InfoItem
                            label="Última Atualização"
                            value={formatarData(pessoa.dataAtualizacao)}
                        />
                    )}
                </Box>
            </SimpleGrid>

            <Confirm
                isOpen={isOpen}
                onClose={onClose}
                onConfirm={handleDelete}
                title="Excluir Pessoa"
                message={`Tem certeza que deseja excluir ${pessoa.nome}? Esta ação não pode ser desfeita.`}
                isLoading={isDeleting}
            />
        </Box>
    );
};

export default PessoaItem;