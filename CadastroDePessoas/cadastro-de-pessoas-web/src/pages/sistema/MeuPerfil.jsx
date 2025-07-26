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
} from '@chakra-ui/react';
import { FiSave, FiUser, FiLock, FiMail, FiCalendar } from 'react-icons/fi';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useAuth } from '../../hooks/useAuth';
import { useNotification } from '../../hooks/useNotification';
import { authService } from '../../services/auth-service';
import { formatarData } from '../../utils/formatters';

const perfilSchema = yup.object().shape({
    nome: yup.string().required('Nome � obrigat�rio').min(2, 'Nome deve ter pelo menos 2 caracteres'),
    email: yup.string().email('E-mail inv�lido').required('E-mail � obrigat�rio'),
});

const senhaSchema = yup.object().shape({
    senhaAtual: yup.string().required('Senha atual � obrigat�ria'),
    novaSenha: yup.string()
        .required('Nova senha � obrigat�ria')
        .min(6, 'Senha deve ter pelo menos 6 caracteres')
        .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])/, 
                'Senha deve conter: 1 min�scula, 1 mai�scula, 1 n�mero e 1 caractere especial'),
    confirmarSenha: yup.string()
        .required('Confirma��o de senha � obrigat�ria')
        .oneOf([yup.ref('novaSenha')], 'Senhas n�o coincidem'),
});

const MeuPerfil = () => {
    const { user, updateUser } = useAuth();
    const { showSuccess, showError } = useNotification();
    const [isLoadingPerfil, setIsLoadingPerfil] = useState(false);
    const [isLoadingSenha, setIsLoadingSenha] = useState(false);
    const [profileData, setProfileData] = useState(null);

    const perfilForm = useForm({
        resolver: yupResolver(perfilSchema),
        defaultValues: {
            nome: '',
            email: '',
        }
    });

    const senhaForm = useForm({
        resolver: yupResolver(senhaSchema),
    });

    useEffect(() => {
        loadProfile();
    }, []);

    const loadProfile = async () => {
        try {
            const response = await authService.getProfile();
            if (response.success) {
                setProfileData(response.user);
                perfilForm.reset({
                    nome: response.user.nome,
                    email: response.user.email,
                });
            }
        } catch (error) {
            console.error('Erro ao carregar perfil:', error);
            // Usar dados do contexto como fallback
            if (user) {
                perfilForm.reset({
                    nome: user.nome || '',
                    email: user.email || '',
                });
            }
        }
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
                updateUser(response.user); // Atualizar contexto
                showSuccess('Perfil atualizado com sucesso!');
            } else {
                showError(response.message || 'Erro ao atualizar perfil');
            }
        } catch (error) {
            const errorMsg = error.response?.data?.message || 'Erro ao atualizar perfil';
            showError(errorMsg);
        } finally {
            setIsLoadingPerfil(false);
        }
    };

    const onUpdateSenha = async (data) => {
        setIsLoadingSenha(true);
        
        try {
            // Por enquanto, simular atualiza��o da senha
            await new Promise(resolve => setTimeout(resolve, 1500));
            
            // TODO: Implementar endpoint de mudan�a de senha no backend
            // await authService.changePassword(data);
            
            showSuccess('Senha alterada com sucesso!');
            senhaForm.reset();
        } catch (error) {
            showError('Erro ao alterar senha. Verifique a senha atual.');
        } finally {
            setIsLoadingSenha(false);
        }
    };

    const displayUser = profileData || user || {};

    return (
        <Box p={8}>
            <VStack spacing={8} align="stretch">
                {/* Header */}
                <Box>
                    <Heading size="lg" mb={2} color="gray.800">
                        Meu Perfil
                    </Heading>
                    <Text color="gray.600">
                        Gerencie suas informa��es pessoais e configura��es de conta
                    </Text>
                </Box>

                <Grid templateColumns={{ base: '1fr', lg: '1fr 1fr' }} gap={8}>
                    {/* Informa��es do Perfil */}
                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            {/* Avatar e Info B�sica */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4}>
                                        <Avatar 
                                            size="2xl" 
                                            name={displayUser.nome || 'Usu�rio'} 
                                            bg="brand.500"
                                            color="white"
                                        />
                                        <VStack spacing={1}>
                                            <Heading size="md">{displayUser.nome || 'Usu�rio'}</Heading>
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
                                                <Text fontSize="sm" color="gray.500">�ltimo acesso</Text>
                                                <Text fontSize="sm" fontWeight="600">Hoje</Text>
                                            </VStack>
                                        </HStack>
                                    </VStack>
                                </CardBody>
                            </Card>

                            {/* Editar Perfil */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiUser />
                                            <Heading size="md">Informa��es Pessoais</Heading>
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
                                                    Salvar Altera��es
                                                </Button>
                                            </VStack>
                                        </Box>
                                    </VStack>
                                </CardBody>
                            </Card>
                        </VStack>
                    </GridItem>

                    {/* Seguran�a */}
                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            {/* Alterar Senha */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiLock />
                                            <Heading size="md">Seguran�a</Heading>
                                        </HStack>

                                        <Alert status="info" borderRadius="lg" size="sm">
                                            <AlertIcon />
                                            <Text fontSize="sm">
                                                Funcionalidade de altera��o de senha em desenvolvimento
                                            </Text>
                                        </Alert>

                                        <Box as="form" onSubmit={senhaForm.handleSubmit(onUpdateSenha)}>
                                            <VStack spacing={4}>
                                                <FormControl isInvalid={senhaForm.formState.errors.senhaAtual}>
                                                    <FormLabel>Senha Atual</FormLabel>
                                                    <Input
                                                        {...senhaForm.register('senhaAtual')}
                                                        type="password"
                                                        placeholder="Digite sua senha atual"
                                                        isDisabled
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
                                                        isDisabled
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
                                                        isDisabled
                                                    />
                                                    <FormErrorMessage>
                                                        {senhaForm.formState.errors.confirmarSenha?.message}
                                                    </FormErrorMessage>
                                                </FormControl>

                                                <Button
                                                    type="submit"
                                                    variant="secondary"
                                                    leftIcon={<FiLock />}
                                                    isLoading={isLoadingSenha}
                                                    loadingText="Alterando..."
                                                    w="full"
                                                    isDisabled
                                                >
                                                    Alterar Senha
                                                </Button>
                                            </VStack>
                                        </Box>
                                    </VStack>
                                </CardBody>
                            </Card>

                            {/* Informa��es da Conta */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <Heading size="md">Informa��es da Conta</Heading>
                                        
                                        <VStack spacing={3} align="stretch">
                                            <HStack justify="space-between">
                                                <Text color="gray.600">Status da conta:</Text>
                                                <Badge colorScheme="green">Ativa</Badge>
                                            </HStack>
                                            
                                            <HStack justify="space-between">
                                                <Text color="gray.600">Tipo de usu�rio:</Text>
                                                <Badge colorScheme="blue">Administrador</Badge>
                                            </HStack>
                                            
                                            <HStack justify="space-between">
                                                <Text color="gray.600">�ltimo login:</Text>
                                                <Text fontSize="sm">Hoje �s {new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })}</Text>
                                            </HStack>
                                            
                                            <HStack justify="space-between">
                                                <Text color="gray.600">ID do usu�rio:</Text>
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
                        <Text fontWeight="600" mb={2}>Dicas de Seguran�a:</Text>
                        <Text fontSize="sm">
                            � Mantenha suas informa��es sempre atualizadas<br />
                            � Use um e-mail v�lido para recupera��o de conta<br />
                            � N�o compartilhe suas credenciais de acesso<br />
                            � Em caso de problemas, entre em contato com o suporte
                        </Text>
                    </Box>
                </Alert>
            </VStack>
        </Box>
    );
};

export default MeuPerfil;