import React from 'react';
import {
    Table,
    Thead,
    Tbody,
    Tr,
    Th,
    Td,
    TableContainer,
    Badge,
    Box,
    Text,
    Input,
    InputGroup,
    InputLeftElement,
    Flex,
    Select,
    Button,
} from '@chakra-ui/react';
import { FiSearch, FiPlus } from 'react-icons/fi';
import { Link } from 'react-router-dom';
import { formatarData, formatarCPF } from '../../utils/formatters';
import ActionButtons from '../ui/ActionButtons';
import Loading from '../ui/Loading';

const PessoasList = ({
    pessoas = [],
    isLoading,
    onDelete
}) => {
    const [filtro, setFiltro] = React.useState('');
    const [filtroSexo, setFiltroSexo] = React.useState('');

    const pessoasFiltradas = React.useMemo(() => {
        if (!filtro && !filtroSexo) return pessoas;

        return pessoas.filter(pessoa => {
            const matchFiltro = !filtro ||
                pessoa.nome.toLowerCase().includes(filtro.toLowerCase()) ||
                pessoa.cpf.includes(filtro) ||
                pessoa.email?.toLowerCase().includes(filtro.toLowerCase());

            const matchSexo = !filtroSexo ||
                pessoa.sexo?.toString() === filtroSexo;

            return matchFiltro && matchSexo;
        });
    }, [pessoas, filtro, filtroSexo]);

    if (isLoading) {
        return <Loading />;
    }

    if (pessoas.length === 0) {
        return (
            <Box textAlign="center" py={10}>
                <Text fontSize="lg" mb={4}>Nenhuma pessoa cadastrada</Text>
                <Button
                    as={Link}
                    to="/pessoas/criar"
                    colorScheme="blue"
                    leftIcon={<FiPlus />}
                >
                    Cadastrar Pessoa
                </Button>
            </Box>
        );
    }

    return (
        <Box>
            <Flex
                mb={6}
                direction={{ base: 'column', md: 'row' }}
                justify="space-between"
                align={{ base: 'stretch', md: 'center' }}
                gap={4}
            >
                <InputGroup maxW={{ base: 'full', md: '320px' }}>
                    <InputLeftElement pointerEvents="none">
                        <FiSearch color="gray.300" />
                    </InputLeftElement>
                    <Input
                        placeholder="Buscar por nome, CPF ou e-mail"
                        value={filtro}
                        onChange={(e) => setFiltro(e.target.value)}
                    />
                </InputGroup>

                <Flex gap={4} align="center">
                    <Select
                        placeholder="Filtrar por sexo"
                        maxW="200px"
                        value={filtroSexo}
                        onChange={(e) => setFiltroSexo(e.target.value)}
                    >
                        <option value="0">Masculino</option>
                        <option value="1">Feminino</option>
                        <option value="2">Outro</option>
                    </Select>

                    <Button
                        as={Link}
                        to="/pessoas/criar"
                        colorScheme="blue"
                        leftIcon={<FiPlus />}
                        size={{ base: 'sm', md: 'md' }}
                    >
                        Nova Pessoa
                    </Button>
                </Flex>
            </Flex>

            <TableContainer>
                <Table variant="simple">
                    <Thead>
                        <Tr>
                            <Th>Nome</Th>
                            <Th>CPF</Th>
                            <Th>Data de Nascimento</Th>
                            <Th>Sexo</Th>
                            <Th>Ações</Th>
                        </Tr>
                    </Thead>
                    <Tbody>
                        {pessoasFiltradas.map((pessoa) => (
                            <Tr key={pessoa.id}>
                                <Td fontWeight="medium">{pessoa.nome}</Td>
                                <Td>{formatarCPF(pessoa.cpf)}</Td>
                                <Td>{formatarData(pessoa.dataNascimento)}</Td>
                                <Td>
                                    {pessoa.sexo === 0 && <Badge colorScheme="blue">Masculino</Badge>}
                                    {pessoa.sexo === 1 && <Badge colorScheme="pink">Feminino</Badge>}
                                    {pessoa.sexo === 2 && <Badge colorScheme="purple">Outro</Badge>}
                                    {pessoa.sexo === null && <Badge colorScheme="gray">Não informado</Badge>}
                                </Td>
                                <Td>
                                    <ActionButtons
                                        id={pessoa.id}
                                        onDelete={onDelete}
                                    />
                                </Td>
                            </Tr>
                        ))}
                    </Tbody>
                </Table>
            </TableContainer>
        </Box>
    );
};

export default PessoasList;