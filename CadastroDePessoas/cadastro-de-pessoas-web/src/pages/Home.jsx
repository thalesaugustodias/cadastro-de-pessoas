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
    VStack,
    HStack,
    Progress,
    Badge,
    Avatar,
    Grid,
    GridItem,
} from '@chakra-ui/react';
import { keyframes } from '@emotion/react';
import { 
    FiUsers, 
    FiUserPlus, 
    FiCalendar, 
    FiActivity,
    FiClock,
    FiUpload,
    FiDownload,
} from 'react-icons/fi';
import { Link } from 'react-router-dom';
import { pessoaService } from '../services/pessoa-service';
import { formatarData } from '../utils/formatters';
import { useAuth } from '../hooks/useAuth';
import Loading from '../components/ui/Loading';

// Animação para contadores
const countUp = keyframes`
    0% { opacity: 0; transform: translateY(10px); }
    100% { opacity: 1; transform: translateY(0); }
`;

const StatCard = ({ title, value, icon, color, helpText, isLoading }) => (
    <Card 
        variant="elevated"
        _hover={{
            transform: 'translateY(-8px)',
            boxShadow: '0 20px 40px 0 rgba(0, 0, 0, 0.1)',
        }}
        transition="all 0.3s ease"
    >
        <CardBody p={6}>
            <Stat>
                <HStack justify="space-between" align="start" mb={4}>
                    <Box>
                        <StatLabel 
                            fontSize="sm" 
                            fontWeight="600" 
                            color="gray.500"
                            textTransform="uppercase"
                            letterSpacing="wide"
                        >
                            {title}
                        </StatLabel>
                    </Box>
                    <Flex
                        w="12"
                        h="12"
                        align="center"
                        justify="center"
                        rounded="xl"
                        bg={`linear-gradient(135deg, ${color}.400, ${color}.600)`}
                        boxShadow={`0 8px 25px 0 ${color === 'brand' ? 'rgba(30, 119, 243, 0.3)' : 'rgba(16, 185, 129, 0.3)'}`}
                    >
                        <Icon as={icon} boxSize="6" color="white" />
                    </Flex>
                </HStack>
                
                <StatNumber 
                    fontSize="3xl" 
                    fontWeight="bold"
                    color="gray.800"
                    animation={`${countUp} 0.6s ease-out`}
                >
                    {isLoading ? <Progress size="sm" isIndeterminate /> : value}
                </StatNumber>
                
                {helpText && (
                    <StatHelpText 
                        fontSize="sm" 
                        color="gray.600"
                        mb={0}
                        mt={3}
                    >
                        {helpText}
                    </StatHelpText>
                )}
            </Stat>
        </CardBody>
    </Card>
);

const QuickActionCard = ({ title, description, icon, color, to, onClick }) => (
    <Card
        variant="outline"
        cursor="pointer"
        as={to ? Link : Box}
        to={to}
        onClick={onClick}
        _hover={{
            borderColor: `${color}.300`,
            boxShadow: `0 8px 25px 0 rgba(30, 119, 243, 0.15)`,
            transform: 'translateY(-4px)',
        }}
        transition="all 0.3s ease"
    >
        <CardBody p={6}>
            <VStack spacing={4} align="center" textAlign="center">
                <Flex
                    w="16"
                    h="16"
                    align="center"
                    justify="center"
                    rounded="2xl"
                    bg={`linear-gradient(135deg, ${color}.400, ${color}.600)`}
                    boxShadow={`0 8px 25px 0 ${color === 'brand' ? 'rgba(30, 119, 243, 0.3)' : 'rgba(16, 185, 129, 0.3)'}`}
                >
                    <Icon as={icon} boxSize="8" color="white" />
                </Flex>
                
                <Box>
                    <Heading size="md" mb={2} color="gray.800">
                        {title}
                    </Heading>
                    <Text fontSize="sm" color="gray.600">
                        {description}
                    </Text>
                </Box>
            </VStack>
        </CardBody>
    </Card>
);

const WelcomeCard = ({ user }) => (
    <Card 
        bg="linear-gradient(135deg, #1e77f3 0%, #26ece9 100%)"
        color="white"
        mb={8}
    >
        <CardBody p={8}>
            <HStack spacing={6} align="center">
                <Avatar 
                    size="xl" 
                    name={user?.nome || 'Usuário'} 
                    bg="whiteAlpha.300"
                    color="white"
                    border="4px solid"
                    borderColor="whiteAlpha.300"
                />
                
                <Box flex="1">
                    <Heading size="lg" mb={2}>
                        Olá, {user?.nome || 'Usuário'}! ??
                    </Heading>
                    <Text fontSize="lg" opacity="0.9" mb={3}>
                        Bem-vindo ao sistema de cadastro
                    </Text>
                    <Text fontSize="sm" opacity="0.8">
                        Gerencie cadastros de forma simples e eficiente
                    </Text>
                </Box>
                
                <VStack spacing={2} align="end">
                    <Badge 
                        bg="whiteAlpha.200" 
                        color="white" 
                        px={3} 
                        py={1}
                        borderRadius="full"
                    >
                        Administrador
                    </Badge>
                    <Text fontSize="sm" opacity="0.8">
                        Sistema em operação
                    </Text>
                </VStack>
            </HStack>
        </CardBody>
    </Card>
);

const RecentActivity = ({ pessoas }) => {
    const recentPeople = pessoas
        .sort((a, b) => new Date(b.dataCadastro) - new Date(a.dataCadastro))
        .slice(0, 5);

    return (
        <Card h="fit-content">
            <CardBody p={6}>
                <HStack justify="space-between" align="center" mb={6}>
                    <Heading size="md" color="gray.800">
                        Cadastros Recentes
                    </Heading>
                    <Icon as={FiActivity} color="gray.400" />
                </HStack>
                
                <VStack spacing={4} align="stretch">
                    {recentPeople.map((pessoa, index) => (
                        <HStack key={pessoa.id} spacing={4}>
                            <Avatar 
                                size="sm" 
                                name={pessoa.nome}
                                bg="brand.500"
                                color="white"
                            />
                            <Box flex="1">
                                <Text fontWeight="600" fontSize="sm" color="gray.800">
                                    {pessoa.nome}
                                </Text>
                                <Text fontSize="xs" color="gray.500">
                                    {formatarData(pessoa.dataCadastro)}
                                </Text>
                            </Box>
                            <Badge 
                                colorScheme="green" 
                                variant="subtle"
                                fontSize="xs"
                            >
                                Novo
                            </Badge>
                        </HStack>
                    ))}
                    
                    {recentPeople.length === 0 && (
                        <Text color="gray.500" textAlign="center" py={4}>
                            Nenhum cadastro encontrado
                        </Text>
                    )}
                </VStack>
            </CardBody>
        </Card>
    );
};

const Home = () => {
    const { user } = useAuth();
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

    // Cadastros dos últimos 30 dias
    const ultimosMeses = new Date();
    ultimosMeses.setDate(ultimosMeses.getDate() - 30);
    const cadastrosRecentes = pessoas.filter(p => 
        new Date(p.dataCadastro) >= ultimosMeses
    ).length;

    if (isLoading) {
        return <Loading />;
    }

    if (error) {
        return (
            <Box textAlign="center" py={20}>
                <Icon as={FiActivity} boxSize="16" color="gray.300" mb={6} />
                <Heading size="md" color="gray.600" mb={4}>
                    Ops! Algo deu errado
                </Heading>
                <Text color="gray.500" mb={6}>{error}</Text>
                <Button 
                    variant="primary" 
                    onClick={() => window.location.reload()}
                    leftIcon={<FiClock />}
                >
                    Tentar novamente
                </Button>
            </Box>
        );
    }

    return (
        <Box p={8}>
            {/* Card de boas-vindas */}
            <WelcomeCard user={user} />

            {/* Estatísticas principais */}
            <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} spacing={6} mb={8}>
                <StatCard
                    title="Total de Pessoas"
                    value={totalPessoas.toLocaleString()}
                    icon={FiUsers}
                    color="brand"
                    helpText="cadastradas no sistema"
                    isLoading={isLoading}
                />

                <StatCard
                    title="Novos Cadastros"
                    value={cadastrosRecentes}
                    icon={FiUserPlus}
                    color="success"
                    helpText="nos últimos 30 dias"
                    isLoading={isLoading}
                />

                <StatCard
                    title="Média de Idade"
                    value={mediaIdade ? `${mediaIdade} anos` : 'N/A'}
                    icon={FiCalendar}
                    color="warning"
                    helpText="dos cadastrados"
                    isLoading={isLoading}
                />
            </SimpleGrid>

            {/* Grid principal */}
            <Grid templateColumns={{ base: '1fr', lg: '2fr 1fr' }} gap={8}>
                {/* Ações rápidas */}
                <GridItem>
                    <Heading size="md" mb={6} color="gray.800">
                        Ações Rápidas
                    </Heading>
                    <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
                        <QuickActionCard
                            title="Gerenciar Pessoas"
                            description="Visualizar, editar e excluir cadastros"
                            icon={FiUsers}
                            color="brand"
                            to="/pessoas"
                        />

                        <QuickActionCard
                            title="Nova Pessoa"
                            description="Adicionar um novo cadastro"
                            icon={FiUserPlus}
                            color="success"
                            to="/pessoas/criar"
                        />

                        <QuickActionCard
                            title="Importar Dados"
                            description="Upload de arquivo em lote"
                            icon={FiUpload}
                            color="accent"
                            to="/pessoas/importar"
                        />

                        <QuickActionCard
                            title="Exportar Dados"
                            description="Exportar em PDF ou Excel"
                            icon={FiDownload}
                            color="warning"
                            to="/pessoas/exportar"
                        />
                    </SimpleGrid>
                </GridItem>

                {/* Atividade recente */}
                <GridItem>
                    <RecentActivity pessoas={pessoas} />
                </GridItem>
            </Grid>
        </Box>
    );
};

export default Home;