import React, { useState, useEffect } from 'react';
import '../styles/icon-fixes.css';
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
    useColorModeValue,
    useDisclosure,
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalBody,
    ModalCloseButton,
    ModalFooter,
    AvatarGroup,
    Divider,
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
    FiFilePlus,
    FiPieChart,
    FiTrendingUp,
    FiAlertCircle,
    FiCheckCircle,
    FiInfo,
    FiRefreshCw,
} from 'react-icons/fi';
import { Link } from 'react-router-dom';
import { pessoaService } from '../services/pessoa-service';
import { formatarData } from '../utils/formatters';
import { useAuth } from '../hooks/useAuth';
import { useNotification } from '../hooks/useNotification';
import Loading from '../components/ui/Loading';

const countUp = keyframes`
    0% { opacity: 0; transform: translateY(10px); }
    100% { opacity: 1; transform: translateY(0); }
`;

const pulse = keyframes`
    0% { transform: scale(1); }
    50% { transform: scale(1.05); }
    100% { transform: scale(1); }
`;

const StatCard = ({ title, value, icon, color, helpText, isLoading }) => {
    const bgGradient = useColorModeValue(
        `linear-gradient(135deg, ${color}.400 0%, ${color}.600 100%)`,
        `linear-gradient(135deg, ${color}.600 0%, ${color}.800 100%)`
    );
    
    const boxShadow = useColorModeValue(
        `0 8px 25px 0 ${color === 'brand' ? 'rgba(30, 119, 243, 0.25)' : 
                        color === 'success' ? 'rgba(16, 185, 129, 0.25)' : 
                        'rgba(245, 158, 11, 0.25)'}`,
        'none'
    );
    
    return (
        <Card 
            variant="elevated"
            _hover={{
                transform: 'translateY(-8px)',
                boxShadow: 'xl',
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
                            rounded="full"
                            bg={bgGradient}
                            boxShadow={boxShadow}
                            animation={isLoading ? `${pulse} 1.5s ease-in-out infinite` : 'none'}
                        >
                            <Icon as={icon} boxSize="6" color="white" style={{ color: 'white' }} />
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
};

const QuickActionCard = ({ title, description, icon, color, to, onClick }) => {
    const bgGradient = useColorModeValue(
        `linear-gradient(135deg, ${color}.400 0%, ${color}.600 100%)`,
        `linear-gradient(135deg, ${color}.600 0%, ${color}.800 100%)`
    );
    
    const boxShadow = useColorModeValue(
        `0 8px 25px 0 ${color === 'brand' ? 'rgba(30, 119, 243, 0.25)' : 
                        color === 'success' ? 'rgba(16, 185, 129, 0.25)' : 
                        color === 'warning' ? 'rgba(245, 158, 11, 0.25)' : 
                        'rgba(124, 58, 237, 0.25)'}`,
        'none'
    );
    
    return (
        <Card
            variant="outline"
            cursor="pointer"
            as={to ? Link : Box}
            to={to}
            onClick={onClick}
            _hover={{
                borderColor: `${color}.300`,
                boxShadow: 'lg',
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
                        rounded="full"
                        bg={bgGradient}
                        boxShadow={boxShadow}
                    >
                        <Icon as={icon} boxSize="8" color="white" style={{ color: 'white !important' }} />
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
};

const WelcomeCard = ({ user, stats }) => {
    const cardBg = useColorModeValue(
        "linear-gradient(135deg, #1e77f3 0%, #26ece9 100%)",
        "linear-gradient(135deg, #134a99 0%, #1d8d8b 100%)"
    );
    
    const currentHour = new Date().getHours();
    let greeting = "Olá";
    
    if (currentHour < 12) {
        greeting = "Bom dia";
    } else if (currentHour < 18) {
        greeting = "Boa tarde";
    } else {
        greeting = "Boa noite";
    }
    
    return (
        <Card 
            bg={cardBg}
            color="white"
            mb={8}
            boxShadow="xl"
        >
            <CardBody p={8}>
                <HStack key="welcome-header" spacing={6} align="center">
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
                            {greeting}, {user?.nome || 'Usuário'}! 👋
                        </Heading>
                        <Text fontSize="lg" opacity="0.9" mb={3}>
                            Bem-vindo ao sistema de cadastro
                        </Text>
                        <Text fontSize="sm" opacity="0.8">
                            {stats 
                                ? `Você tem acesso a ${stats.totalPessoas} cadastros no sistema`
                                : 'Gerencie cadastros de forma simples e eficiente'}
                        </Text>
                    </Box>
                    
                    <VStack spacing={2} align="end">
                        <Badge 
                            bg="whiteAlpha.300" 
                            color="white" 
                            px={3} 
                            py={1}
                            borderRadius="full"
                        >
                            Administrador
                        </Badge>
                        <HStack key="welcome-date" spacing={2} mt={2}>
                            <Icon as={FiClock} boxSize="3" color="white" style={{ color: 'white' }} />
                            <Text fontSize="sm" opacity="0.8">
                                {new Date().toLocaleDateString('pt-BR', { 
                                    weekday: 'long', 
                                    year: 'numeric', 
                                    month: 'long', 
                                    day: 'numeric' 
                                })}
                            </Text>
                        </HStack>
                    </VStack>
                </HStack>
            </CardBody>
        </Card>
    );
};

const RecentActivity = ({ pessoas, isLoading }) => {
    const { isOpen, onOpen, onClose } = useDisclosure();
    const [selectedPerson, setSelectedPerson] = useState(null);
    
    const recentPeople = React.useMemo(() => {
        if (!pessoas || pessoas.length === 0) return [];
        
        return pessoas
            .slice()
            .sort((a, b) => new Date(b.dataCadastro || b.DataCadastro) - new Date(a.dataCadastro || a.DataCadastro))
            .slice(0, 5);
    }, [pessoas]);
        
    const handleViewDetails = (pessoa) => {
        setSelectedPerson(pessoa);
        onOpen();
    };
    
    const getProp = (pessoa, lowerProp, upperProp) => {
        return pessoa[lowerProp] !== undefined ? pessoa[lowerProp] : pessoa[upperProp];
    };
    
    return (
        <>
            <Card h="fit-content" boxShadow="sm">
                <CardBody p={6}>
                    <HStack justify="space-between" align="center" mb={6}>
                        <Heading size="md" color="gray.800">
                            Cadastros Recentes
                        </Heading>
                        <Icon as={FiActivity} color="gray.400" style={{ color: 'var(--chakra-colors-gray-400)' }} />
                    </HStack>
                    
                    <VStack spacing={4} align="stretch">
                        {isLoading ? (
                            <Box py={10} textAlign="center">
                                <Progress size="xs" isIndeterminate colorScheme="blue" mb={6} />
                                <Text color="gray.500">Carregando cadastros recentes...</Text>
                            </Box>
                        ) : recentPeople.length > 0 ? (
                            recentPeople.map((pessoa, index) => {
                                const id = getProp(pessoa, "id", "Id");
                                const nome = getProp(pessoa, "nome", "Nome");
                                const email = getProp(pessoa, "email", "Email");
                                const dataCadastro = getProp(pessoa, "dataCadastro", "DataCadastro");
                                
                                return (
                                    <HStack key={id || `pessoa-${index}`} spacing={4}>
                                        <Avatar 
                                            size="sm" 
                                            name={nome}
                                            bg={`hsl(${Math.floor(Math.random() * 360)}, 70%, 50%)`}
                                            color="white"
                                        />
                                        <Box flex="1">
                                            <Text fontWeight="600" fontSize="sm" color="gray.800">
                                                {nome}
                                            </Text>
                                            <HStack spacing={2} mt={1}>
                                                <Icon as={FiCalendar} boxSize="3" color="gray.500" />
                                                <Text fontSize="xs" color="gray.500">
                                                    {formatarData(dataCadastro)}
                                                </Text>
                                            </HStack>
                                        </Box>
                                        <Box>
                                            <HStack spacing={1}>
                                                <Button 
                                                    size="sm" 
                                                    variant="ghost" 
                                                    colorScheme="blue"
                                                    onClick={() => handleViewDetails(pessoa)}
                                                    aria-label="Ver detalhes"
                                                >
                                                    <Icon as={FiInfo} />
                                                </Button>
                                                <Button
                                                    as={Link}
                                                    to={`/pessoas/${id}`}
                                                    size="sm"
                                                    variant="outline"
                                                    colorScheme="green"
                                                    aria-label="Ver perfil completo"
                                                >
                                                    <Icon as={FiUserPlus} />
                                                </Button>
                                            </HStack>
                                        </Box>
                                    </HStack>
                                );
                            })
                        ) : (
                            <Box textAlign="center" py={6}>
                                <Icon as={FiUsers} boxSize="8" color="gray.300" mb={3} />
                                <Text color="gray.500" fontWeight="medium">
                                    Nenhum cadastro encontrado
                                </Text>
                                <Text fontSize="sm" color="gray.400" mt={1}>
                                    Adicione novos cadastros para vê-los aqui
                                </Text>
                                <Button 
                                    as={Link}
                                    to="/pessoas/criar"
                                    size="sm"
                                    leftIcon={<FiUserPlus />}
                                    variant="outline"
                                    colorScheme="brand"
                                    mt={4}
                                >
                                    Novo Cadastro
                                </Button>
                            </Box>
                        )}
                    </VStack>
                </CardBody>
            </Card>
            
            {selectedPerson && (
                <Modal isOpen={isOpen} onClose={onClose} size="md">
                    <ModalOverlay backdropFilter="blur(5px)" />
                    <ModalContent>
                        <ModalHeader>Detalhes da Pessoa</ModalHeader>
                        <ModalCloseButton />
                        <ModalBody pb={6}>
                            <VStack spacing={4} align="start">
                                <HStack w="100%" spacing={4}>
                                    <Avatar 
                                        size="xl" 
                                        name={getProp(selectedPerson, "nome", "Nome")}
                                        bg={`hsl(${Math.floor(Math.random() * 360)}, 70%, 50%)`}
                                        color="white"
                                    />
                                    <Box>
                                        <Heading size="md">{getProp(selectedPerson, "nome", "Nome")}</Heading>
                                        <Text color="gray.500">{getProp(selectedPerson, "email", "Email") || 'Email não disponível'}</Text>
                                        <Badge colorScheme="green" mt={1}>
                                            ID: {(getProp(selectedPerson, "id", "Id") || "").substring(0, 8)}...
                                        </Badge>
                                    </Box>
                                </HStack>
                                
                                <Box w="100%" pt={2}>
                                    <SimpleGrid columns={2} spacing={4}>
                                        <Box>
                                            <Text fontSize="sm" color="gray.500">CPF</Text>
                                            <Text fontWeight="medium">
                                                {getProp(selectedPerson, "cpf", "CPF") || 'Não informado'}
                                            </Text>
                                        </Box>
                                        <Box>
                                            <Text fontSize="sm" color="gray.500">Telefone</Text>
                                            <Text fontWeight="medium">
                                                {getProp(selectedPerson, "telefone", "Telefone") || 'Não informado'}
                                            </Text>
                                        </Box>
                                        <Box>
                                            <Text fontSize="sm" color="gray.500">Nacionalidade</Text>
                                            <Text fontWeight="medium">
                                                {getProp(selectedPerson, "nacionalidade", "Nacionalidade") || 'Não informada'}
                                            </Text>
                                        </Box>
                                        <Box>
                                            <Text fontSize="sm" color="gray.500">Idade</Text>
                                            <Text fontWeight="medium">
                                                {getProp(selectedPerson, "idade", "Idade") || 'Não informada'}
                                            </Text>
                                        </Box>
                                    </SimpleGrid>
                                    
                                    {(selectedPerson.endereco || selectedPerson.Endereco) && (
                                        <Box mt={4}>
                                            <Text fontSize="sm" color="gray.500" mb={1}>Endereço</Text>
                                            <Text fontWeight="medium">
                                                {getProp(selectedPerson, "enderecoCompleto", "EnderecoCompleto")}
                                            </Text>
                                        </Box>
                                    )}
                                </Box>
                            </VStack>
                        </ModalBody>
                        <ModalFooter>
                            <Button 
                                as={Link} 
                                to={`/pessoas/${getProp(selectedPerson, "id", "Id")}`}
                                colorScheme="blue" 
                                mr={3}
                                leftIcon={<FiFilePlus />}
                            >
                                Ver Perfil Completo
                            </Button>
                            <Button onClick={onClose}>Fechar</Button>
                        </ModalFooter>
                    </ModalContent>
                </Modal>
            )}
        </>
    );
};

const StatisticsCard = ({ pessoas }) => {
    const sexoStats = React.useMemo(() => {
        if (!pessoas || pessoas.length === 0) return { masculino: 0, feminino: 0, outros: 0 };
        
        const masculino = pessoas.filter(p => (p.sexo === 0 || p.Sexo === 0)).length;
        const feminino = pessoas.filter(p => (p.sexo === 1 || p.Sexo === 1)).length;
        const outros = pessoas.filter(p => 
            (p.sexo !== 0 && p.sexo !== 1 && p.sexo !== undefined) || 
            (p.Sexo !== 0 && p.Sexo !== 1 && p.Sexo !== undefined)
        ).length;
        
        return { masculino, feminino, outros };
    }, [pessoas]);
    
    return (
        <Card h="fit-content">
            <CardBody p={6}>
                <HStack justify="space-between" mb={6}>
                    <Heading size="md" color="gray.800">
                        Estatísticas
                    </Heading>
                    <Icon as={FiPieChart} color="gray.400" style={{ color: 'var(--chakra-colors-gray-400)' }} />
                </HStack>
                
                <VStack spacing={4} align="stretch">
                    <Box>
                        <HStack key="masculino-stat" justify="space-between" mb={2}>
                            <Text fontSize="sm" fontWeight="medium" color="gray.600">Masculino</Text>
                            <Text fontSize="sm" fontWeight="bold">{sexoStats.masculino}</Text>
                        </HStack>
                        <Progress 
                            value={pessoas.length > 0 ? (sexoStats.masculino / pessoas.length) * 100 : 0} 
                            size="sm" 
                            colorScheme="blue" 
                            borderRadius="full" 
                        />
                    </Box>
                    
                    <Box>
                        <HStack key="feminino-stat" justify="space-between" mb={2}>
                            <Text fontSize="sm" fontWeight="medium" color="gray.600">Feminino</Text>
                            <Text fontSize="sm" fontWeight="bold">{sexoStats.feminino}</Text>
                        </HStack>
                        <Progress 
                            value={pessoas.length > 0 ? (sexoStats.feminino / pessoas.length) * 100 : 0} 
                            size="sm" 
                            colorScheme="pink" 
                            borderRadius="full" 
                        />
                    </Box>
                    
                    <Box>
                        <HStack key="outros-stat" justify="space-between" mb={2}>
                            <Text fontSize="sm" fontWeight="medium" color="gray.600">Outros</Text>
                            <Text fontSize="sm" fontWeight="bold">{sexoStats.outros}</Text>
                        </HStack>
                        <Progress 
                            value={pessoas.length > 0 ? (sexoStats.outros / pessoas.length) * 100 : 0} 
                            size="sm" 
                            colorScheme="purple" 
                            borderRadius="full" 
                        />
                    </Box>
                    
                    <Divider my={2} />
                    
                    <Box>
                        <AvatarGroup size="sm" max={5} mt={2}>
                            {pessoas.slice(0, 5).map((pessoa, index) => (
                                <Avatar
                                    key={pessoa.Id || pessoa.id || `avatar-${index}`}
                                    name={pessoa.Nome || pessoa.nome || `Pessoa ${index + 1}`}
                                    bg={`hsl(${Math.floor(Math.random() * 360)}, 70%, 50%)`}
                                />
                            ))}
                        </AvatarGroup>
                        <Text fontSize="xs" color="gray.500" mt={2}>
                            {pessoas.length} pessoas no total
                        </Text>
                    </Box>
                </VStack>
            </CardBody>
        </Card>
    );
};

const Home = () => {
    const { user } = useAuth();
    const { showSuccess, showError } = useNotification();
    const [pessoas, setPessoas] = React.useState([]);
    const [isLoading, setIsLoading] = React.useState(true);
    const [error, setError] = React.useState(null);
    const [refreshTrigger, setRefreshTrigger] = React.useState(0);

    React.useEffect(() => {
        const fetchPessoas = async () => {
            try {
                setIsLoading(true);
                const data = await pessoaService.listar();
                setPessoas(data || []);
                setError(null);
            } catch (error) {
                console.error('Erro ao buscar pessoas:', error);
                setError('Não foi possível carregar os dados. Tente novamente mais tarde.');
            } finally {
                setIsLoading(false);
            }
        };

        fetchPessoas();
    }, [refreshTrigger]);

    const handleRefresh = () => {
        setRefreshTrigger(prev => prev + 1);
        showSuccess("Dados atualizados");
    };

    const getProp = (pessoa, lowerProp, upperProp) => {
        if (!pessoa) return null;
        return pessoa[lowerProp] !== undefined ? pessoa[lowerProp] : pessoa[upperProp];
    };

    const ultimoCadastro = React.useMemo(() => {
        if (!pessoas || pessoas.length === 0) return null;
        
        return [...pessoas].sort((a, b) => {
            const dateA = new Date(getProp(a, "dataCadastro", "DataCadastro") || 0);
            const dateB = new Date(getProp(b, "dataCadastro", "DataCadastro") || 0);
            return dateB - dateA;
        })[0];
    }, [pessoas]);

    const calcularIdade = (dataNascimento) => {
        if (!dataNascimento) return 0;
        
        const hoje = new Date();
        const nascimento = new Date(dataNascimento);
        if (isNaN(nascimento.getTime())) return 0;
        
        let idade = hoje.getFullYear() - nascimento.getFullYear();
        const m = hoje.getMonth() - nascimento.getMonth();

        if (m < 0 || (m === 0 && hoje.getDate() < nascimento.getDate())) {
            idade--;
        }
        return idade;
    };

    const totalPessoas = pessoas.length;
    
    const mediaIdade = React.useMemo(() => {
        if (pessoas.length === 0) return 0;
        
        let sum = 0;
        let count = 0;
        
        for (const p of pessoas) {
            const idade = getProp(p, "idade", "Idade") || 
                calcularIdade(getProp(p, "dataNascimento", "DataNascimento"));
            
            if (idade > 0) {
                sum += idade;
                count++;
            }
        }
        
        return count > 0 ? Math.round(sum / count) : 0;
    }, [pessoas]);

    const cadastrosRecentes = React.useMemo(() => {
        if (pessoas.length === 0) return 0;
        
        const ultimosMeses = new Date();
        ultimosMeses.setDate(ultimosMeses.getDate() - 30);
        
        return pessoas.filter(p => {
            const dataCadastro = new Date(getProp(p, "dataCadastro", "DataCadastro") || 0);
            return dataCadastro >= ultimosMeses;
        }).length;
    }, [pessoas]);

    if (isLoading && pessoas.length === 0) {
        return <Loading />;
    }

    if (error && pessoas.length === 0) {
        return (
            <Box textAlign="center" py={20}>
                <Icon as={FiAlertCircle} boxSize="16" color="red.300" mb={6} />
                <Heading size="md" color="gray.600" mb={4}>
                    Ops! Algo deu errado
                </Heading>
                <Text color="gray.500" mb={6}>{error}</Text>
                <Button 
                    variant="primary" 
                    onClick={handleRefresh}
                    leftIcon={<FiRefreshCw />}
                >
                    Tentar novamente
                </Button>
            </Box>
        );
    }

    const stats = {
        totalPessoas,
        cadastrosRecentes,
        mediaIdade
    };

    return (
        <Box p={8}>
            <WelcomeCard user={user} stats={stats} />
            
            <HStack key="page-header" justify="space-between" mb={6}>
                <Heading size="lg" color="gray.800">
                    Visão Geral
                </Heading>
                <Button 
                    leftIcon={<Icon as={FiRefreshCw} style={{ color: 'inherit' }} />}
                    onClick={handleRefresh}
                    variant="outline"
                    size="sm"
                    isLoading={isLoading}
                >
                    Atualizar
                </Button>
            </HStack>

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
                    icon={FiTrendingUp}
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

            <Grid templateColumns={{ base: '1fr', lg: '2fr 1fr' }} gap={8}>
                <GridItem>
                    <Heading size="md" mb={6} color="gray.800">
                        Ações Rápidas
                    </Heading>
                    <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6} mb={8}>
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
                    
                    {ultimoCadastro && (
                        <Card mb={8} bg="blue.50" boxShadow="sm">
                            <CardBody p={6}>
                                <HStack key="last-reg-header" justify="space-between" mb={4}>
                                    <Heading size="sm" color="blue.700">
                                        <HStack key="heading-content">
                                            <Icon as={FiCheckCircle} />
                                            <Text>Último Cadastro</Text>
                                        </HStack>
                                    </Heading>
                                    <Badge colorScheme="blue">
                                        {formatarData(getProp(ultimoCadastro, "dataCadastro", "DataCadastro"))}
                                    </Badge>
                                </HStack>
                                <HStack key="last-reg-content" spacing={4}>
                                    <Avatar 
                                        size="md" 
                                        name={getProp(ultimoCadastro, "nome", "Nome")} 
                                        bg="blue.500"
                                    />
                                    <Box>
                                        <Text fontWeight="bold">{getProp(ultimoCadastro, "nome", "Nome")}</Text>
                                        <Text fontSize="sm">{getProp(ultimoCadastro, "email", "Email")}</Text>
                                    </Box>
                                    <Button 
                                        as={Link}
                                        to={`/pessoas/${getProp(ultimoCadastro, "id", "Id")}`}
                                        ml="auto"
                                        size="sm"
                                        colorScheme="blue"
                                        variant="outline"
                                    >
                                        Ver Detalhes
                                    </Button>
                                </HStack>
                            </CardBody>
                        </Card>
                    )}
                </GridItem>

                <GridItem>
                    <VStack spacing={8} align="stretch">
                        <RecentActivity pessoas={pessoas} isLoading={isLoading} />
                        
                        <StatisticsCard pessoas={pessoas} />
                        
                        <Card>
                            <CardBody p={6}>
                                <Heading size="sm" mb={4}>Status do Sistema</Heading>
                                <VStack spacing={3} align="stretch">
                                    <HStack key="api-status" justify="space-between">
                                        <Text fontSize="sm">API</Text>
                                        <Badge colorScheme="green">Online</Badge>
                                    </HStack>
                                    <HStack key="db-status" justify="space-between">
                                        <Text fontSize="sm">Banco de Dados</Text>
                                        <Badge colorScheme="green">Conectado</Badge>
                                    </HStack>
                                    <HStack key="login-status" justify="space-between">
                                        <Text fontSize="sm">Último Login</Text>
                                        <Text fontSize="xs">{formatarData(new Date())}</Text>
                                    </HStack>
                                </VStack>
                            </CardBody>
                        </Card>
                    </VStack>
                </GridItem>
            </Grid>
        </Box>
    );
};

export default Home;