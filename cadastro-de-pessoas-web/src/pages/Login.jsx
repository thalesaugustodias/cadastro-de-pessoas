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
    Image,
    useColorModeValue,
} from '@chakra-ui/react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { useNotification } from '../hooks/useNotification';

const schema = yup.object().shape({
    email: yup.string().email('Digite um e-mail válido').required('O e-mail é obrigatório'),
    senha: yup.string().required('A senha é obrigatória'),
});

const Login = () => {
    const { login } = useAuth();
    const { showError } = useNotification();
    const navigate = useNavigate();
    const [isLoading, setIsLoading] = React.useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm({
        resolver: yupResolver(schema),
    });

    const onSubmit = async (data) => {
        setIsLoading(true);

        try {
            const result = await login(data.email, data.senha);

            if (result.success) {
                navigate('/');
            } else {
                showError(result.message || 'Credenciais inválidas');
            }
        } catch (error) {
            showError('Erro ao realizar login');
            console.error(error);
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
                    <VStack spacing={6} align="stretch">
                        <Box textAlign="center">
                            <Heading as="h1" size="xl" mb={2}>
                                Bem-vindo
                            </Heading>
                            <Text color="gray.500">
                                Entre com suas credenciais para acessar o sistema
                            </Text>
                        </Box>

                        <Box as="form" onSubmit={handleSubmit(onSubmit)}>
                            <VStack spacing={4}>
                                <FormControl isInvalid={errors.email}>
                                    <FormLabel htmlFor="email">E-mail</FormLabel>
                                    <Input
                                        id="email"
                                        type="email"
                                        placeholder="seu@email.com"
                                        {...register('email')}
                                    />
                                    <FormErrorMessage>{errors.email?.message}</FormErrorMessage>
                                </FormControl>

                                <FormControl isInvalid={errors.senha}>
                                    <FormLabel htmlFor="senha">Senha</FormLabel>
                                    <Input
                                        id="senha"
                                        type="password"
                                        placeholder="Sua senha"
                                        {...register('senha')}
                                    />
                                    <FormErrorMessage>{errors.senha?.message}</FormErrorMessage>
                                </FormControl>

                                <Button
                                    type="submit"
                                    colorScheme="blue"
                                    size="lg"
                                    width="full"
                                    mt={4}
                                    isLoading={isLoading}
                                >
                                    Entrar
                                </Button>
                            </VStack>
                        </Box>

                        <Text fontSize="sm" color="gray.500" textAlign="center">
                            Use o usuário padrão: admin@exemplo.com / Admin@123
                        </Text>
                    </VStack>
                </Box>
            </Container>
        </Box>
    );
};

export default Login;