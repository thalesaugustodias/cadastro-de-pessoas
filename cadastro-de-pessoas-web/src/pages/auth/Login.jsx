import React from 'react';
import {
    Box,
    Button,
    FormControl,
    FormLabel,
    FormErrorMessage,
    Input,
    VStack,
    Heading,
    Text,
    Container,
    Alert,
    AlertIcon,
    AlertDescription,
    HStack,
    Divider,
    Tabs,
    TabList,
    TabPanels,
    Tab,
    TabPanel,
    Icon,
    Stack,
    Flex,
} from '@chakra-ui/react';
import { keyframes } from '@emotion/react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { useNotification } from '../../hooks/useNotification';
import { authService } from '../../services/auth-service';
import { 
    FiMail, 
    FiLock, 
    FiUser, 
    FiLogIn,
    FiUserPlus,
} from 'react-icons/fi';

const float = keyframes`
    0%, 100% { transform: translateY(0px); }
    50% { transform: translateY(-10px); }
`;

const loginSchema = yup.object().shape({
    email: yup.string().email('Digite um e-mail válido').required('O e-mail é obrigatório'),
    senha: yup.string().required('A senha é obrigatória'),
});

const registerSchema = yup.object().shape({
    nome: yup.string().required('O nome é obrigatório').min(2, 'Nome deve ter pelo menos 2 caracteres'),
    email: yup.string().email('Digite um e-mail válido').required('O e-mail é obrigatório'),
    senha: yup.string()
        .required('A senha é obrigatória')
        .min(6, 'Senha deve ter pelo menos 6 caracteres')
        .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])/, 
                'Senha deve conter: 1 minúscula, 1 maiúscula, 1 número e 1 caractere especial'),
});

const Login = () => {
    const { login } = useAuth();
    const { showError, showSuccess } = useNotification();
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = React.useState(false);
    const [loginError, setLoginError] = React.useState('');

    const loginForm = useForm({
        resolver: yupResolver(loginSchema),
        defaultValues: {
            email: 'admin@exemplo.com',
            senha: 'Admin@123'
        }
    });

    const registerForm = useForm({
        resolver: yupResolver(registerSchema),
    });

    const onLogin = async (data) => {
        setIsLoading(true);
        setLoginError('');

        try {
            const result = await login(data.email, data.senha);

            if (result.success) {
                showSuccess(result.message || 'Login realizado com sucesso!');
                navigate('/');
            } else {
                const errorMsg = result.message || 'Credenciais inválidas';
                setLoginError(errorMsg);
                showError(errorMsg);
            }
        } catch (error) {
            const errorMsg = 'Erro ao realizar login. Verifique sua conexão.';
            setLoginError(errorMsg);
            showError(errorMsg);
        } finally {
            setIsLoading(false);
        }
    };

    const onRegister = async (data) => {
        setIsLoading(true);

        try {
            const result = await authService.register(data.nome, data.email, data.senha);
            showSuccess('Usuário criado com sucesso! Agora você pode fazer login.');
            registerForm.reset();
        } catch (error) {
            const errorMsg = error.response?.data?.errors 
                ? Object.values(error.response.data.errors).flat().join(', ')
                : error.response?.data?.message || 'Erro ao criar usuário';
            showError(errorMsg);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Box
            minH="100vh"
            bg="linear-gradient(135deg, #667eea 0%, #764ba2 100%)"
            position="relative"
            overflow="hidden"
        >
            {/* Elementos decorativos de fundo */}
            <Box
                position="absolute"
                top="10%"
                left="10%"
                w="200px"
                h="200px"
                borderRadius="full"
                bg="whiteAlpha.100"
                animation={`${float} 6s ease-in-out infinite`}
            />
            <Box
                position="absolute"
                top="60%"
                right="15%"
                w="150px"
                h="150px"
                borderRadius="full"
                bg="whiteAlpha.100"
                animation={`${float} 4s ease-in-out infinite reverse`}
            />
            
            <Flex
                align="center"
                justify="center"
                minH="100vh"
                py={12}
                px={4}
            >
                <Container maxW="lg">
                    {/* Logo e título */}
                    <VStack spacing={8} mb={10}>
                        <VStack spacing={2}>
                            <Heading 
                                size="2xl" 
                                color="white" 
                                textAlign="center"
                                fontWeight="700"
                            >
                                Cadastro de Pessoas
                            </Heading>
                            <Text 
                                color="whiteAlpha.800" 
                                fontSize="lg"
                                textAlign="center"
                                maxW="md"
                            >
                                Sistema de gerenciamento de cadastros
                            </Text>
                        </VStack>
                    </VStack>

                    {/* Card principal */}
                    <Box
                        bg="white"
                        p={10}
                        borderRadius="3xl"
                        boxShadow="0 25px 50px -12px rgba(0, 0, 0, 0.25)"
                        border="1px solid"
                        borderColor="whiteAlpha.200"
                        backdropFilter="blur(10px)"
                    >
                        <Tabs variant="modern" isFitted>
                            <TabList mb={8} borderColor="gray.200">
                                <Tab
                                    _selected={{
                                        color: 'brand.500',
                                        borderColor: 'brand.500',
                                        fontWeight: '700',
                                    }}
                                    fontSize="lg"
                                    py={4}
                                >
                                    <HStack spacing={2}>
                                        <Icon as={FiLogIn} />
                                        <Text>Entrar</Text>
                                    </HStack>
                                </Tab>
                                <Tab
                                    _selected={{
                                        color: 'brand.500',
                                        borderColor: 'brand.500',
                                        fontWeight: '700',
                                    }}
                                    fontSize="lg"
                                    py={4}
                                >
                                    <HStack spacing={2}>
                                        <Icon as={FiUserPlus} />
                                        <Text>Cadastrar</Text>
                                    </HStack>
                                </Tab>
                            </TabList>
                            
                            <TabPanels>
                                <TabPanel p={0}>
                                    <VStack spacing={8} align="stretch">
                                        <Box textAlign="center">
                                            <Heading as="h1" size="xl" mb={3} color="gray.800">
                                                Bem-vindo de volta
                                            </Heading>
                                            <Text color="gray.600" fontSize="lg">
                                                Entre com suas credenciais para continuar
                                            </Text>
                                        </Box>

                                        {loginError && (
                                            <Alert 
                                                status="error" 
                                                borderRadius="xl"
                                                border="1px solid"
                                                borderColor="danger.200"
                                            >
                                                <AlertIcon />
                                                <AlertDescription fontWeight="500">
                                                    {loginError}
                                                </AlertDescription>
                                            </Alert>
                                        )}

                                        <Box as="form" onSubmit={loginForm.handleSubmit(onLogin)}>
                                            <VStack spacing={6}>
                                                <FormControl isInvalid={loginForm.formState.errors.email}>
                                                    <FormLabel color="gray.700">E-mail</FormLabel>
                                                    <Box position="relative">
                                                        <Icon
                                                            as={FiMail}
                                                            position="absolute"
                                                            left="4"
                                                            top="50%"
                                                            transform="translateY(-50%)"
                                                            color="gray.400"
                                                            zIndex="1"
                                                        />
                                                        <Input
                                                            type="email"
                                                            placeholder="seu@email.com"
                                                            size="lg"
                                                            pl="12"
                                                            {...loginForm.register('email')}
                                                        />
                                                    </Box>
                                                    <FormErrorMessage>
                                                        {loginForm.formState.errors.email?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <FormControl isInvalid={loginForm.formState.errors.senha}>
                                                    <FormLabel color="gray.700">Senha</FormLabel>
                                                    <Box position="relative">
                                                        <Icon
                                                            as={FiLock}
                                                            position="absolute"
                                                            left="4"
                                                            top="50%"
                                                            transform="translateY(-50%)"
                                                            color="gray.400"
                                                            zIndex="1"
                                                        />
                                                        <Input
                                                            type="password"
                                                            placeholder="Sua senha"
                                                            size="lg"
                                                            pl="12"
                                                            {...loginForm.register('senha')}
                                                        />
                                                    </Box>
                                                    <FormErrorMessage>
                                                        {loginForm.formState.errors.senha?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <Button
                                                    type="submit"
                                                    variant="primary"
                                                    size="lg"
                                                    width="full"
                                                    isLoading={isLoading}
                                                    loadingText="Entrando..."
                                                    leftIcon={<FiLogIn />}
                                                >
                                                    Entrar na Conta
                                                </Button>
                                            </VStack>
                                        </Box>

                                        <Alert 
                                            status="info" 
                                            borderRadius="xl"
                                            bg="blue.50"
                                            borderColor="blue.200"
                                            border="1px solid"
                                        >
                                            <AlertIcon color="blue.500" />
                                            <Box>
                                                <Text fontSize="sm" fontWeight="600" color="blue.800">
                                                    Usuario de demonstração:
                                                </Text>
                                                <Text fontSize="sm" color="blue.700" mt={1}>
                                                    admin@exemplo.com / Admin@123
                                                </Text>
                                            </Box>
                                        </Alert>
                                    </VStack>
                                </TabPanel>

                                {/* REGISTER TAB */}
                                <TabPanel p={0}>
                                    <VStack spacing={8} align="stretch">
                                        <Box textAlign="center">
                                            <Heading as="h1" size="xl" mb={3} color="gray.800">
                                                Criar nova conta
                                            </Heading>
                                            <Text color="gray.600" fontSize="lg">
                                                Cadastre-se para acessar o sistema
                                            </Text>
                                        </Box>

                                        <Box as="form" onSubmit={registerForm.handleSubmit(onRegister)}>
                                            <VStack spacing={6}>
                                                <FormControl isInvalid={registerForm.formState.errors.nome}>
                                                    <FormLabel color="gray.700">Nome Completo</FormLabel>
                                                    <Box position="relative">
                                                        <Icon
                                                            as={FiUser}
                                                            position="absolute"
                                                            left="4"
                                                            top="50%"
                                                            transform="translateY(-50%)"
                                                            color="gray.400"
                                                            zIndex="1"
                                                        />
                                                        <Input
                                                            placeholder="Seu nome completo"
                                                            size="lg"
                                                            pl="12"
                                                            {...registerForm.register('nome')}
                                                        />
                                                    </Box>
                                                    <FormErrorMessage>
                                                        {registerForm.formState.errors.nome?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <FormControl isInvalid={registerForm.formState.errors.email}>
                                                    <FormLabel color="gray.700">E-mail</FormLabel>
                                                    <Box position="relative">
                                                        <Icon
                                                            as={FiMail}
                                                            position="absolute"
                                                            left="4"
                                                            top="50%"
                                                            transform="translateY(-50%)"
                                                            color="gray.400"
                                                            zIndex="1"
                                                        />
                                                        <Input
                                                            type="email"
                                                            placeholder="seu@email.com"
                                                            size="lg"
                                                            pl="12"
                                                            {...registerForm.register('email')}
                                                        />
                                                    </Box>
                                                    <FormErrorMessage>
                                                        {registerForm.formState.errors.email?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <FormControl isInvalid={registerForm.formState.errors.senha}>
                                                    <FormLabel color="gray.700">Senha</FormLabel>
                                                    <Box position="relative">
                                                        <Icon
                                                            as={FiLock}
                                                            position="absolute"
                                                            left="4"
                                                            top="50%"
                                                            transform="translateY(-50%)"
                                                            color="gray.400"
                                                            zIndex="1"
                                                        />
                                                        <Input
                                                            type="password"
                                                            placeholder="MinhaSenh@123"
                                                            size="lg"
                                                            pl="12"
                                                            {...registerForm.register('senha')}
                                                        />
                                                    </Box>
                                                    <FormErrorMessage>
                                                        {registerForm.formState.errors.senha?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <Button
                                                    type="submit"
                                                    variant="success"
                                                    size="lg"
                                                    width="full"
                                                    isLoading={isLoading}
                                                    loadingText="Criando conta..."
                                                    leftIcon={<FiUserPlus />}
                                                >
                                                    Criar Conta
                                                </Button>
                                            </VStack>
                                        </Box>

                                        <Alert 
                                            status="warning" 
                                            borderRadius="xl"
                                            bg="warning.50"
                                            borderColor="warning.200"
                                            border="1px solid"
                                        >
                                            <AlertIcon color="warning.500" />
                                            <Box>
                                                <Text fontSize="sm" fontWeight="600" color="warning.800">
                                                    Requisitos para senha:
                                                </Text>
                                                <Text fontSize="sm" color="warning.700" mt={1}>
                                                     • Mínimo 6 caracteres<br />
                                                     • 1 letra minúscula e 1 maiúscula<br />
                                                     • 1 número e 1 caractere especial (@$!%*?&)
                                                </Text>
                                            </Box>
                                        </Alert>
                                    </VStack>
                                </TabPanel>
                            </TabPanels>
                        </Tabs>
                    </Box>
                </Container>
            </Flex>
        </Box>
    );
};

export default Login;