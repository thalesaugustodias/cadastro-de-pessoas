import React, { useState, useEffect } from 'react';
import {
    Box,
    Heading,
    Text,
    Button,
    VStack,
    HStack,
    Card,
    CardBody,
    Input,
    FormControl,
    FormLabel,
    FormErrorMessage,
    Avatar,
    Alert,
    AlertIcon,
    Divider,
    Grid,
    GridItem,
    Badge,
    useDisclosure,
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalBody,
    ModalFooter,
    ModalCloseButton,
    useToast,
    Spinner,
    Center,
    Icon,
} from '@chakra-ui/react';
import { FiSave, FiUser, FiLock, FiRefreshCw, FiAlertCircle } from 'react-icons/fi';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useAuth } from '../../hooks/useAuth';
import { useNotification } from '../../hooks/useNotification';
import { authService } from '../../services/auth-service';
import { formatarData } from '../../utils/formatters';

const perfilSchema = yup.object().shape({
    nome: yup.string().required('Nome é obrigatório').min(2, 'Nome deve ter pelo menos 2 caracteres'),
    email: yup.string().email('E-mail inválido').required('E-mail é obrigatório'),
});

const senhaSchema = yup.object().shape({
    senhaAtual: yup.string().required('Senha atual é obrigatória'),
    novaSenha: yup.string()
        .required('Nova senha é obrigatória')
        .min(6, 'Senha deve ter pelo menos 6 caracteres')
        .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])/, 
                'Senha deve conter: 1 minúscula, 1 maiúscula, 1 número e 1 caractere especial'),
    confirmarSenha: yup.string()
        .required('Confirmação de senha é obrigatória')
        .oneOf([yup.ref('novaSenha')], 'Senhas não coincidem'),
});

const MeuPerfil = () => {
    const { user, updateUser, authenticated, logout } = useAuth();
    const { showSuccess, showError } = useNotification();
    const toast = useToast();
    const [isLoadingPerfil, setIsLoadingPerfil] = useState(false);
    const [isLoadingSenha, setIsLoadingSenha] = useState(false);
    const [isLoadingData, setIsLoadingData] = useState(true);
    const [profileData, setProfileData] = useState(null);
    const [authError, setAuthError] = useState(false);
    const { isOpen, onOpen, onClose } = useDisclosure();
    const [senhaAlteradaComSucesso, setSenhaAlteradaComSucesso] = useState(false);
    const [debugInfo, setDebugInfo] = useState(null);

    const perfilForm = useForm({
        resolver: yupResolver(perfilSchema),
        defaultValues: {
            nome: user?.nome || '',
            email: user?.email || '',
        }
    });

    const senhaForm = useForm({
        resolver: yupResolver(senhaSchema),
    });

    useEffect(() => {
        if (user) {
            perfilForm.reset({
                nome: user.nome || '',
                email: user.email || '',
            });
        }
    }, [user]);

    useEffect(() => {
        loadProfile();
    }, []);

    const loadProfile = async () => {
        // Usar o valor booleano 'authenticated' em vez de chamar isAuthenticated como função
        if (!authenticated) {
            setAuthError(true);
            setIsLoadingData(false);
            return;
        }

        setIsLoadingData(true);
        try {
            const response = await authService.getProfile();
            if (response.success) {
                setProfileData(response.user);
                perfilForm.reset({
                    nome: response.user.nome,
                    email: response.user.email,
                });
                setAuthError(false);
            }
        } catch (error) {
            console.error('Erro ao carregar perfil:', error);
            
            // Captura informações de debug para ajudar no diagnóstico
            if (error.response?.data?.debug) {
                setDebugInfo(error.response.data.debug);
            }
            
            // Se o erro for 401 (Unauthorized) ou específico sobre token inválido
            if (error.response?.status === 401 || 
                error.response?.data?.message?.includes('token')) {
                setAuthError(true);
                toast({
                    title: "Sessão expirada",
                    description: "Sua sessão expirou. Por favor, faça login novamente.",
                    status: "warning",
                    duration: 5000,
                    isClosable: true,
                    position: "top-right"
                });
            } else if (error.response?.status === 400) {
                // Problema com o token ou claims
                setAuthError(true);
                toast({
                    title: "Problema de autenticação",
                    description: error.response.data.message || "Problema com seu token de autenticação. Faça login novamente.",
                    status: "error",
                    duration: 5000,
                    isClosable: true,
                    position: "top-right"
                });
            } else {
                // Usar os dados do usuário do localStorage como fallback
                if (user) {
                    perfilForm.reset({
                        nome: user.nome || '',
                        email: user.email || '',
                    });
                }
            }
        } finally {
            setIsLoadingData(false);
        }
    };

    const handleRelogin = () => {
        logout();
        window.location.href = '/login';
    };

    const onUpdatePerfil = async (data) => {
        setIsLoadingPerfil(true);
        
        try {
            const response = await authService.updateProfile({
                nome: data.nome,
                email: data.email
            });

            if (response.success) {
                setProfileData(response.user);
                updateUser(response.user);
                showSuccess('Perfil atualizado com sucesso!');
            } else {
                showError(response.message || 'Erro ao atualizar perfil');
            }
        } catch (error) {
            const errorMsg = error.response?.data?.message || 'Erro ao atualizar perfil';
            showError(errorMsg);
            
            if (error.response?.status === 401) {
                setAuthError(true);
            }
        } finally {
            setIsLoadingPerfil(false);
        }
    };

    const onUpdateSenha = async (data) => {
        setIsLoadingSenha(true);
        
        try {
            const response = await authService.changePassword({
                senhaAtual: data.senhaAtual,
                novaSenha: data.novaSenha,
                confirmarSenha: data.confirmarSenha
            });

            if (response.success) {
                showSuccess('Senha alterada com sucesso!');
                senhaForm.reset();
                setSenhaAlteradaComSucesso(true);
                setTimeout(() => {
                    onClose();
                    setSenhaAlteradaComSucesso(false);
                }, 2000);
            } else {
                showError(response.message || 'Erro ao alterar senha');
            }
        } catch (error) {
            const errorMsg = error.message || error.response?.data?.message || 'Erro ao alterar senha. Verifique a senha atual.';
            showError(errorMsg);
            
            if (error.response?.status === 401) {
                setAuthError(true);
            }
        } finally {
            setIsLoadingSenha(false);
        }
    };

    const displayUser = profileData || user || {};

    if (isLoadingData) {
        return (
            <Center py={20}>
                <VStack spacing={6}>
                    <Spinner size="xl" color="blue.500" thickness="4px" />
                    <Text color="gray.600">Carregando dados do perfil...</Text>
                </VStack>
            </Center>
        );
    }

    if (authError) {
        return (
            <Box p={8}>
                <Card mb={8} bg="red.50">
                    <CardBody p={6}>
                        <VStack spacing={6} align="center">
                            <Icon as={FiAlertCircle} boxSize={16} color="red.500" />
                            <Heading size="md" textAlign="center">Sessão Expirada</Heading>
                            <Text textAlign="center">
                                Sua sessão expirou ou o token de autenticação é inválido. 
                                Por favor, faça login novamente para acessar seu perfil.
                            </Text>

                            {debugInfo && (
                                <Alert status="info" variant="subtle">
                                    <AlertIcon />
                                    <Text fontSize="sm" fontFamily="mono">
                                        {debugInfo}
                                    </Text>
                                </Alert>
                            )}

                            <Button 
                                colorScheme="blue" 
                                leftIcon={<FiRefreshCw />}
                                onClick={handleRelogin}
                            >
                                Fazer Login Novamente
                            </Button>
                        </VStack>
                    </CardBody>
                </Card>
            </Box>
        );
    }

    return (
        <Box p={8}>
            <VStack spacing={8} align="stretch">
                <Box>
                    <Heading size="lg" mb={2} color="gray.800">
                        Meu Perfil
                    </Heading>
                    <Text color="gray.600">
                        Gerencie suas informações pessoais e configurações de conta
                    </Text>
                </Box>

                <Grid templateColumns={{ base: '1fr', lg: '1fr 1fr' }} gap={8}>
                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4}>
                                        <Avatar 
                                            size="2xl" 
                                            name={displayUser.nome || 'Usuário'}
                                            bg="brand.500"
                                            color="white"
                                        />
                                        <VStack spacing={1}>
                                            <Heading size="md">{displayUser.nome || 'Usuário'}</Heading>
                                            <Text color="gray.600">{displayUser.email || 'usuario@exemplo.com'}</Text>
                                            <Badge colorScheme="green" mt={2}>
                                                Conta Ativa
                                            </Badge>
                                        </VStack>
                                        
                                        <HStack spacing={4} mt={4}>
                                            <VStack spacing={0}>
                                                <Text fontSize="sm" color="gray.500">Membro desde</Text>
                                                <Text fontSize="sm" fontWeight="600">
                                                    {profileData?.dataCadastro 
                                                        ? formatarData(profileData.dataCadastro)
                                                        : formatarData(new Date())
                                                    }
                                                </Text>
                                            </VStack>
                                            <Divider orientation="vertical" h="10" />
                                            <VStack spacing={0}>
                                                <Text fontSize="sm" color="gray.500">Último acesso</Text>
                                                <Text fontSize="sm" fontWeight="600">Hoje</Text>
                                            </VStack>
                                        </HStack>
                                    </VStack>
                                </CardBody>
                            </Card>

                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiUser />
                                            <Heading size="md">Informações Pessoais</Heading>
                                        </HStack>

                                        <Box as="form" onSubmit={perfilForm.handleSubmit(onUpdatePerfil)}>
                                            <VStack spacing={4}>
                                                <FormControl isInvalid={perfilForm.formState.errors.nome}>
                                                    <FormLabel>Nome Completo</FormLabel>
                                                    <Input
                                                        {...perfilForm.register('nome')}
                                                        placeholder="Seu nome completo"
                                                    />
                                                    <FormErrorMessage>
                                                        {perfilForm.formState.errors.nome?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <FormControl isInvalid={perfilForm.formState.errors.email}>
                                                    <FormLabel>E-mail</FormLabel>
                                                    <Input
                                                        {...perfilForm.register('email')}
                                                        placeholder="seu@email.com"
                                                        type="email"
                                                    />
                                                    <FormErrorMessage>
                                                        {perfilForm.formState.errors.email?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <Button
                                                    type="submit"
                                                    variant="primary"
                                                    leftIcon={<FiSave />}
                                                    isLoading={isLoadingPerfil}
                                                    loadingText="Salvando..."
                                                    w="full"
                                                >
                                                    Salvar Alterações
                                                </Button>
                                            </VStack>
                                        </Box>
                                    </VStack>
                                </CardBody>
                            </Card>
                        </VStack>
                    </GridItem>

                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiLock />
                                            <Heading size="md">Segurança</Heading>
                                        </HStack>

                                        <Button
                                            variant="secondary"
                                            leftIcon={<FiLock />}
                                            onClick={onOpen}
                                            w="full"
                                        >
                                            Alterar Senha
                                        </Button>

                                        <Modal isOpen={isOpen} onClose={onClose}>
                                            <ModalOverlay />
                                            <ModalContent>
                                                <ModalHeader>Alterar Senha</ModalHeader>
                                                <ModalCloseButton />
                                                <ModalBody>
                                                    {senhaAlteradaComSucesso ? (
                                                        <Alert status="success" borderRadius="md">
                                                            <AlertIcon />
                                                            Senha alterada com sucesso!
                                                        </Alert>
                                                    ) : (
                                                        <Box as="form" id="change-password-form" onSubmit={senhaForm.handleSubmit(onUpdateSenha)}>
                                                            <VStack spacing={4}>
                                                                <FormControl isInvalid={senhaForm.formState.errors.senhaAtual}>
                                                                    <FormLabel>Senha Atual</FormLabel>
                                                                    <Input
                                                                        {...senhaForm.register('senhaAtual')}
                                                                        type="password"
                                                                        placeholder="Digite sua senha atual"
                                                                    />
                                                                    <FormErrorMessage>
                                                                        {senhaForm.formState.errors.senhaAtual?.message}
                                                                    </FormErrorMessage>
                                                                </FormControl>

                                                                <FormControl isInvalid={senhaForm.formState.errors.novaSenha}>
                                                                    <FormLabel>Nova Senha</FormLabel>
                                                                    <Input
                                                                        {...senhaForm.register('novaSenha')}
                                                                        type="password"
                                                                        placeholder="Digite a nova senha"
                                                                    />
                                                                    <FormErrorMessage>
                                                                        {senhaForm.formState.errors.novaSenha?.message}
                                                                    </FormErrorMessage>
                                                                </FormControl>

                                                                <FormControl isInvalid={senhaForm.formState.errors.confirmarSenha}>
                                                                    <FormLabel>Confirmar Nova Senha</FormLabel>
                                                                    <Input
                                                                        {...senhaForm.register('confirmarSenha')}
                                                                        type="password"
                                                                        placeholder="Confirme a nova senha"
                                                                    />
                                                                    <FormErrorMessage>
                                                                        {senhaForm.formState.errors.confirmarSenha?.message}
                                                                    </FormErrorMessage>
                                                                </FormControl>
                                                            </VStack>
                                                        </Box>
                                                    )}
                                                </ModalBody>

                                                <ModalFooter>
                                                    {!senhaAlteradaComSucesso && (
                                                        <>
                                                            <Button variant="outline" mr={3} onClick={onClose}>
                                                                Cancelar
                                                            </Button>
                                                            <Button 
                                                                type="submit"
                                                                form="change-password-form"
                                                                variant="primary"
                                                                leftIcon={<FiLock />}
                                                                isLoading={isLoadingSenha}
                                                                loadingText="Alterando..."
                                                            >
                                                                Alterar Senha
                                                            </Button>
                                                        </>
                                                    )}
                                                </ModalFooter>
                                            </ModalContent>
                                        </Modal>
                                    </VStack>
                                </CardBody>
                            </Card>
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <Heading size="md">Informações da Conta</Heading>

                                        <VStack spacing={3} align="stretch">
                                            <HStack justify="space-between">
                                                <Text color="gray.600">Status da conta:</Text>
                                                <Badge colorScheme="green">Ativa</Badge>
                                            </HStack>
                                            

                                            <HStack justify="space-between">
                                                <Text color="gray.600">Tipo de usuário:</Text>
                                                <Badge colorScheme="blue">Administrador</Badge>
                                            </HStack>
                                            

                                            <HStack justify="space-between">
                                                <Text color="gray.600">Último login:</Text>
                                                <Text fontSize="sm">Hoje às {new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}</Text>
                                            </HStack>
                                            

                                            <HStack justify="space-between">
                                                <Text color="gray.600">ID do usuário:</Text>
                                                <Text fontSize="xs" fontFamily="mono">
                                                    {displayUser.id ? displayUser.id.toString().substring(0, 8) + '...' : 'N/A'}
                                                </Text>
                                            </HStack>
                                        </VStack>
                                    </VStack>
                                </CardBody>
                            </Card>
                        </VStack>
                    </GridItem>
                </Grid>

                <Alert status="info" borderRadius="xl">
                    <AlertIcon />
                    <Box>
                        <Text fontWeight="600" mb={2}>Dicas de Segurança:</Text>
                        <Text fontSize="sm">
                            • Mantenha suas informações sempre atualizadas<br />
                            • Use um e-mail válido para recuperação de conta<br />
                            • Não compartilhe suas credenciais de acesso<br />
                            • Em caso de problemas, entre em contato com o suporte
                        </Text>
                    </Box>
                </Alert>
            </VStack>
        </Box>
    );
};

export default MeuPerfil;