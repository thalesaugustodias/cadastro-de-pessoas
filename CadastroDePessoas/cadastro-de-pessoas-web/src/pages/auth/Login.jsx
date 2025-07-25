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
    Center,
    useColorModeValue,
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
} from '@chakra-ui/react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { useNotification } from '../../hooks/useNotification';
import { authService, healthService } from '../../services/auth-service';

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
    const { showError, showSuccess, showWarning } = useNotification();
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = React.useState(false);
    const [loginError, setLoginError] = React.useState('');

    const loginForm = useForm({
        resolver: yupResolver(loginSchema),
        defaultValues: {
            email: 'admin@exemplo.com', // ?? Pré-preenchido para facilitar teste
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
            console.log('?? Tentando login com:', { email: data.email });
            
            const result = await login(data.email, data.senha);

            if (result.success) {
                showSuccess(result.message || 'Login realizado com sucesso!');
                console.log('? Login bem-sucedido, redirecionando...');
                navigate('/');
            } else {
                const errorMsg = result.message || 'Credenciais inválidas';
                setLoginError(errorMsg);
                showError(errorMsg);
                console.error('? Falha no login:', errorMsg);
            }
        } catch (error) {
            const errorMsg = 'Erro ao realizar login. Verifique sua conexão.';
            setLoginError(errorMsg);
            showError(errorMsg);
            console.error('?? Erro no login:', error);
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

    const resetAdmin = async () => {
        try {
            setIsLoading(true);
            const result = await authService.resetAdmin();
            showWarning(`Admin de emergência criado: ${result.credentials.email} / ${result.credentials.password}`);
        } catch (error) {
            showError('Erro ao criar admin de emergência');
        } finally {
            setIsLoading(false);
        }
    };

    const resetDatabase = async () => {
        if (!window.confirm('?? ATENÇÃO: Isso irá apagar TODOS os dados! Continuar?')) {
            return;
        }

        try {
            setIsLoading(true);
            const result = await healthService.resetDatabase();
            showSuccess('Banco resetado! Use: admin@exemplo.com / Admin@123');
            loginForm.setValue('email', 'admin@exemplo.com');
            loginForm.setValue('senha', 'Admin@123');
        } catch (error) {
            showError('Erro ao resetar banco de dados');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Box
            minH="100vh"
            bg={useColorModeValue('gray.50', 'gray.800')}
            py={12}
        >
            <Container maxW="lg">
                <Center mb={8}>
                    <Heading color="blue.500">
                        Cadastro de Pessoas
                    </Heading>
                </Center>

                <Box
                    bg={useColorModeValue('white', 'gray.700')}
                    p={8}
                    borderRadius="lg"
                    boxShadow="lg"
                >
                    <Tabs isFitted variant="enclosed">
                        <TabList mb="1em">
                            <Tab>?? Login</Tab>
                            <Tab>?? Cadastrar</Tab>
                        </TabList>
                        
                        <TabPanels>
                            {/* LOGIN TAB */}
                            <TabPanel>
                                <VStack spacing={6} align="stretch">
                                    <Box textAlign="center">
                                        <Heading as="h1" size="xl" mb={2}>
                                            Bem-vindo
                                        </Heading>
                                        <Text color="gray.500">
                                            Entre com suas credenciais
                                        </Text>
                                    </Box>

                                    {loginError && (
                                        <Alert status="error" borderRadius="md">
                                            <AlertIcon />
                                            <AlertDescription>{loginError}</AlertDescription>
                                        </Alert>
                                    )}

                                    <Box as="form" onSubmit={loginForm.handleSubmit(onLogin)}>
                                        <VStack spacing={4}>
                                            <FormControl isInvalid={loginForm.formState.errors.email}>
                                                <FormLabel>E-mail</FormLabel>
                                                <Input
                                                    type="email"
                                                    placeholder="seu@email.com"
                                                    {...loginForm.register('email')}
                                                />
                                                <FormErrorMessage>
                                                    {loginForm.formState.errors.email?.message}
                                                </FormErrorMessage>
                                            </FormControl>

                                            <FormControl isInvalid={loginForm.formState.errors.senha}>
                                                <FormLabel>Senha</FormLabel>
                                                <Input
                                                    type="password"
                                                    placeholder="Sua senha"
                                                    {...loginForm.register('senha')}
                                                />
                                                <FormErrorMessage>
                                                    {loginForm.formState.errors.senha?.message}
                                                </FormErrorMessage>
                                            </FormControl>

                                            <Button
                                                type="submit"
                                                colorScheme="blue"
                                                size="lg"
                                                width="full"
                                                isLoading={isLoading}
                                                loadingText="Entrando..."
                                            >
                                                Entrar
                                            </Button>
                                        </VStack>
                                    </Box>

                                    <Alert status="info" borderRadius="md">
                                        <AlertIcon />
                                        <Box>
                                            <Text fontSize="sm">
                                                <strong>Usuários padrão:</strong><br />
                                                • admin@exemplo.com / Admin@123<br />
                                                • user@teste.com / User@123
                                            </Text>
                                        </Box>
                                    </Alert>

                                    <Divider />

                                    <VStack spacing={2}>
                                        <Text fontSize="sm" color="gray.500">
                                            Problemas para entrar?
                                        </Text>
                                        <HStack spacing={2}>
                                            <Button
                                                size="sm"
                                                variant="outline"
                                                colorScheme="orange"
                                                onClick={resetAdmin}
                                                isLoading={isLoading}
                                            >
                                                ?? Admin Emergência
                                            </Button>
                                            <Button
                                                size="sm"
                                                variant="outline"
                                                colorScheme="red"
                                                onClick={resetDatabase}
                                                isLoading={isLoading}
                                            >
                                                ?? Resetar Banco
                                            </Button>
                                        </HStack>
                                    </VStack>
                                </VStack>
                            </TabPanel>

                            {/* REGISTER TAB */}
                            <TabPanel>
                                <VStack spacing={6} align="stretch">
                                    <Box textAlign="center">
                                        <Heading as="h1" size="xl" mb={2}>
                                            Criar Conta
                                        </Heading>
                                        <Text color="gray.500">
                                            Cadastre-se para acessar o sistema
                                        </Text>
                                    </Box>

                                    <Box as="form" onSubmit={registerForm.handleSubmit(onRegister)}>
                                        <VStack spacing={4}>
                                            <FormControl isInvalid={registerForm.formState.errors.nome}>
                                                <FormLabel>Nome Completo</FormLabel>
                                                <Input
                                                    placeholder="Seu nome completo"
                                                    {...registerForm.register('nome')}
                                                />
                                                <FormErrorMessage>
                                                    {registerForm.formState.errors.nome?.message}
                                                </FormErrorMessage>
                                            </FormControl>

                                            <FormControl isInvalid={registerForm.formState.errors.email}>
                                                <FormLabel>E-mail</FormLabel>
                                                <Input
                                                    type="email"
                                                    placeholder="seu@email.com"
                                                    {...registerForm.register('email')}
                                                />
                                                <FormErrorMessage>
                                                    {registerForm.formState.errors.email?.message}
                                                </FormErrorMessage>
                                            </FormControl>

                                            <FormControl isInvalid={registerForm.formState.errors.senha}>
                                                <FormLabel>Senha</FormLabel>
                                                <Input
                                                    type="password"
                                                    placeholder="MinhaSenh@123"
                                                    {...registerForm.register('senha')}
                                                />
                                                <FormErrorMessage>
                                                    {registerForm.formState.errors.senha?.message}
                                                </FormErrorMessage>
                                            </FormControl>

                                            <Button
                                                type="submit"
                                                colorScheme="green"
                                                size="lg"
                                                width="full"
                                                isLoading={isLoading}
                                                loadingText="Criando..."
                                            >
                                                Criar Conta
                                            </Button>
                                        </VStack>
                                    </Box>

                                    <Alert status="warning" borderRadius="md">
                                        <AlertIcon />
                                        <Box>
                                            <Text fontSize="sm">
                                                <strong>Regras para senha:</strong><br />
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
        </Box>
    );
};

export default Login;