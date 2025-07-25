import React from 'react';
import {
    Box,
    Heading,
    Text,
    SimpleGrid,
    Stat,
    StatLabel,
    StatNumber,
    StatHelpText,
    Icon,
    Flex,
    Button,
    Card,
    CardBody,
    useColorModeValue,
} from '@chakra-ui/react';
import { FiUsers, FiUserPlus, FiCalendar } from 'react-icons/fi';
import { Link } from 'react-router-dom';
import { pessoaService } from '../services/pessoa-service';
import { formatarData } from '../utils/formatters';
import Loading from '../components/ui/Loading';

const StatCard = ({ title, value, icon, color, helpText }) => (
    <Card>
        <CardBody>
            <Stat>
                <Flex justifyContent="space-between" alignItems="center">
                    <Box>
                        <StatLabel fontSize="sm" fontWeight="medium" color="gray.500">
                            {title}
                        </StatLabel>
                        <StatNumber fontSize="3xl" fontWeight="bold">
                            {value}
                        </StatNumber>
                        {helpText && (
                            <StatHelpText fontSize="sm" color="gray.500">
                                {helpText}
                            </StatHelpText>
                        )}
                    </Box>
                    <Flex
                        w="12"
                        h="12"
                        align="center"
                        justify="center"
                        rounded="full"
                        bg={color + '.100'}
                    >
                        <Icon as={icon} boxSize="6" color={color + '.500'} />
                    </Flex>
                </Flex>
            </Stat>
        </CardBody>
    </Card>
);

const Home = () => {
    const [pessoas, setPessoas] = React.useState([]);
    const [isLoading, setIsLoading] = React.useState(true);
    const [error, setError] = React.useState(null);

    React.useEffect(() => {
        const fetchPessoas = async () => {
            try {
                const data = await pessoaService.listar();
                setPessoas(data);
            } catch (error) {
                console.error('Erro ao buscar pessoas:', error);
                setError('Não foi possível carregar os dados. Tente novamente mais tarde.');
            } finally {
                setIsLoading(false);
            }
        };

        fetchPessoas();
    }, []);

    // Calcular estatísticas
    const totalPessoas = pessoas.length;
    const ultimoCadastro = pessoas.length > 0
        ? pessoas.sort((a, b) => new Date(b.dataCadastro) - new Date(a.dataCadastro))[0]
        : null;
    const ultimaAtualizacao = pessoas
        .filter(p => p.dataAtualizacao)
        .sort((a, b) => new Date(b.dataAtualizacao) - new Date(a.dataAtualizacao))[0];

    // Calcular média de idade
    const calcularIdade = (dataNascimento) => {
        const hoje = new Date();
        const nascimento = new Date(dataNascimento);
        let idade = hoje.getFullYear() - nascimento.getFullYear();
        const m = hoje.getMonth() - nascimento.getMonth();

        if (m < 0 || (m === 0 && hoje.getDate() < nascimento.getDate())) {
            idade--;
        }

        return idade;
    };

    const mediaIdade = pessoas.length > 0
        ? Math.round(pessoas.reduce((acc, p) => acc + calcularIdade(p.dataNascimento), 0) / pessoas.length)
        : 0;

    if (isLoading) {
        return <Loading />;
    }

    if (error) {
        return (
            <Box textAlign="center" py={10}>
                <Text color="red.500" mb={4}>{error}</Text>
                <Button colorScheme="blue" onClick={() => window.location.reload()}>
                    Tentar novamente
                </Button>
            </Box>
        );
    }

    return (
        <Box>
            <Box mb={8}>
                <Heading as="h1" size="lg" mb={2}>
                    Dashboard
                </Heading>
                <Text color="gray.600">
                    Bem-vindo ao sistema de cadastro de pessoas
                </Text>
            </Box>

            <SimpleGrid columns={{ base: 1, md: 3 }} spacing={6} mb={8}>
                <StatCard
                    title="Total de Pessoas"
                    value={totalPessoas}
                    icon={FiUsers}
                    color="blue"
                />

                <StatCard
                    title="Último Cadastro"
                    value={ultimoCadastro ? ultimoCadastro.nome : 'Nenhum'}
                    icon={FiUserPlus}
                    color="green"
                    helpText={ultimoCadastro ? formatarData(ultimoCadastro.dataCadastro) : ''}
                />

                <StatCard
                    title="Média de Idade"
                    value={mediaIdade || 'N/A'}
                    icon={FiCalendar}
                    color="purple"
                    helpText={mediaIdade ? 'anos' : 'Sem dados suficientes'}
                />
            </SimpleGrid>

            <Flex justifyContent="center" mt={10}>
                <Button
                    as={Link}
                    to="/pessoas"
                    size="lg"
                    colorScheme="blue"
                    leftIcon={<FiUsers />}
                    mr={4}
                >
                    Gerenciar Pessoas
                </Button>

                <Button
                    as={Link}
                    to="/pessoas/criar"
                    size="lg"
                    colorScheme="green"
                    leftIcon={<FiUserPlus />}
                >
                    Nova Pessoa
                </Button>
            </Flex>
        </Box>
    );
};

export default Home;