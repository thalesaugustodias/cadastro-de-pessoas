import React, { useState } from 'react';
import {
    Box,
    Heading,
    Text,
    Button,
    VStack,
    HStack,
    Card,
    CardBody,
    Switch,
    Select,
    Input,
    FormControl,
    FormLabel,
    Alert,
    AlertIcon,
    Divider,
    Badge,
    Grid,
    GridItem,
} from '@chakra-ui/react';
import { FiSave, FiSettings, FiBell, FiDatabase, FiShield } from 'react-icons/fi';
import { useNotification } from '../../hooks/useNotification';

const Configuracoes = () => {
    const { showSuccess, showError } = useNotification();
    const [isLoading, setIsLoading] = useState(false);
    
    const [settings, setSettings] = useState({
        // Notificações
        emailNotifications: true,
        systemNotifications: false,
        exportNotifications: true,
        
        // Sistema
        itemsPerPage: '10',
        dateFormat: 'dd/MM/yyyy',
        autoSave: true,

        // Segurança
        sessionTimeout: '60',
        requirePasswordChange: false,
        twoFactorAuth: false,
        
        // Backup
        autoBackup: true,
        backupFrequency: 'weekly',
        retentionDays: '30',
    });

    const handleSettingChange = (key, value) => {
        setSettings(prev => ({
            ...prev,
            [key]: value
        }));
    };

    const handleSave = async () => {
        setIsLoading(true);
        
        try {
            // Simular salvamento
            await new Promise(resolve => setTimeout(resolve, 1500));
            
            // Salvar no localStorage para persistir
            localStorage.setItem('systemSettings', JSON.stringify(settings));
            
            showSuccess('Configurações salvas com sucesso!');
        } catch (error) {
            showError('Erro ao salvar configurações');
        } finally {
            setIsLoading(false);
        }
    };

    const resetToDefaults = () => {
        setSettings({
            emailNotifications: true,
            systemNotifications: false,
            exportNotifications: true,
            itemsPerPage: '10',
            dateFormat: 'dd/MM/yyyy',
            autoSave: true,
            sessionTimeout: '60',
            requirePasswordChange: false,
            twoFactorAuth: false,
            autoBackup: true,
            backupFrequency: 'weekly',
            retentionDays: '30',
        });
        showSuccess('Configurações restauradas para os valores padrão');
    };

    return (
        <Box p={8}>
            <VStack spacing={8} align="stretch">
                {/* Header */}
                <Box>
                    <Heading size="lg" mb={2} color="gray.800">
                        Configurações do Sistema
                    </Heading>
                    <Text color="gray.600">
                        Personalize as preferências e configurações do sistema
                    </Text>
                </Box>

                <Grid templateColumns={{ base: '1fr', lg: '1fr 1fr' }} gap={8}>
                    {/* Coluna Esquerda */}
                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            {/* Notificações */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiBell />
                                            <Heading size="md">Notificações</Heading>
                                        </HStack>
                                        
                                        <VStack spacing={4} align="stretch">
                                            <HStack justify="space-between">
                                                <Text>Notificações por e-mail</Text>
                                                <Switch
                                                    isChecked={settings.emailNotifications}
                                                    onChange={(e) => handleSettingChange('emailNotifications', e.target.checked)}
                                                    colorScheme="blue"
                                                />
                                            </HStack>
                                            
                                            <HStack justify="space-between">
                                                <Text>Notificações do sistema</Text>
                                                <Switch
                                                    isChecked={settings.systemNotifications}
                                                    onChange={(e) => handleSettingChange('systemNotifications', e.target.checked)}
                                                    colorScheme="blue"
                                                />
                                            </HStack>
                                            
                                            <HStack justify="space-between">
                                                <Text>Notificações de exportação</Text>
                                                <Switch
                                                    isChecked={settings.exportNotifications}
                                                    onChange={(e) => handleSettingChange('exportNotifications', e.target.checked)}
                                                    colorScheme="blue"
                                                />
                                            </HStack>
                                        </VStack>
                                    </VStack>
                                </CardBody>
                            </Card>

                            {/* Sistema */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiSettings />
                                            <Heading size="md">Sistema</Heading>
                                        </HStack>
                                        
                                        <FormControl>
                                            <FormLabel>Itens por página</FormLabel>
                                            <Select
                                                value={settings.itemsPerPage}
                                                onChange={(e) => handleSettingChange('itemsPerPage', e.target.value)}
                                            >
                                                <option value="5">5 itens</option>
                                                <option value="10">10 itens</option>
                                                <option value="25">25 itens</option>
                                                <option value="50">50 itens</option>
                                            </Select>
                                        </FormControl>
                                        
                                        <FormControl>
                                            <FormLabel>Formato de data</FormLabel>
                                            <Select
                                                value={settings.dateFormat}
                                                onChange={(e) => handleSettingChange('dateFormat', e.target.value)}
                                            >
                                                <option value="dd/MM/yyyy">DD/MM/AAAA</option>
                                                <option value="MM/dd/yyyy">MM/DD/AAAA</option>
                                                <option value="yyyy-MM-dd">AAAA-MM-DD</option>
                                            </Select>
                                        </FormControl>
                                        
                                        <HStack justify="space-between">
                                            <Text>Salvamento automático</Text>
                                            <Switch
                                                isChecked={settings.autoSave}
                                                onChange={(e) => handleSettingChange('autoSave', e.target.checked)}
                                                colorScheme="blue"
                                            />
                                        </HStack>
                                    </VStack>
                                </CardBody>
                            </Card>
                        </VStack>
                    </GridItem>

                    {/* Coluna Direita */}
                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            {/* Segurança */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiShield />
                                            <Heading size="md">Segurança</Heading>
                                        </HStack>
                                        
                                        <FormControl>
                                            <FormLabel>Timeout da sessão (minutos)</FormLabel>
                                            <Select
                                                value={settings.sessionTimeout}
                                                onChange={(e) => handleSettingChange('sessionTimeout', e.target.value)}
                                            >
                                                <option value="30">30 minutos</option>
                                                <option value="60">1 hora</option>
                                                <option value="120">2 horas</option>
                                                <option value="240">4 horas</option>
                                            </Select>
                                        </FormControl>
                                        
                                        <HStack justify="space-between">
                                            <Text>Exigir troca de senha</Text>
                                            <Switch
                                                isChecked={settings.requirePasswordChange}
                                                onChange={(e) => handleSettingChange('requirePasswordChange', e.target.checked)}
                                                colorScheme="blue"
                                            />
                                        </HStack>
                                        
                                        <HStack justify="space-between">
                                            <VStack align="start" spacing={0}>
                                                <Text>Autenticação de dois fatores</Text>
                                                <Badge colorScheme="orange" size="sm">Em breve</Badge>
                                            </VStack>
                                            <Switch
                                                isChecked={settings.twoFactorAuth}
                                                onChange={(e) => handleSettingChange('twoFactorAuth', e.target.checked)}
                                                colorScheme="blue"
                                                isDisabled
                                            />
                                        </HStack>
                                    </VStack>
                                </CardBody>
                            </Card>

                            {/* Backup */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack>
                                            <FiDatabase />
                                            <Heading size="md">Backup</Heading>
                                        </HStack>
                                        
                                        <HStack justify="space-between">
                                            <Text>Backup automático</Text>
                                            <Switch
                                                isChecked={settings.autoBackup}
                                                onChange={(e) => handleSettingChange('autoBackup', e.target.checked)}
                                                colorScheme="blue"
                                            />
                                        </HStack>
                                        
                                        <FormControl>s
                                            <FormLabel>Frequência do backup</FormLabel>
                                            <Select
                                                value={settings.backupFrequency}
                                                onChange={(e) => handleSettingChange('backupFrequency', e.target.value)}
                                                isDisabled={!settings.autoBackup}
                                            >
                                                <option value="daily">Diário</option>
                                                <option value="weekly">Semanal</option>
                                                <option value="monthly">Mensal</option>
                                            </Select>
                                        </FormControl>
                                        
                                        <FormControl>
                                            <FormLabel>Retenção (dias)</FormLabel>
                                            <Input
                                                type="number"
                                                value={settings.retentionDays}
                                                onChange={(e) => handleSettingChange('retentionDays', e.target.value)}
                                                min="1"
                                                max="365"
                                            />
                                        </FormControl>
                                    </VStack>
                                </CardBody>
                            </Card>
                        </VStack>
                    </GridItem>
                </Grid>

                <Divider />

                {/* Ações */}
                <HStack justify="space-between">
                    <Button
                        variant="outline"
                        onClick={resetToDefaults}
                    >
                        Restaurar Padrões
                    </Button>
                    
                    <Button
                        variant="primary"
                        leftIcon={<FiSave />}
                        onClick={handleSave}
                        isLoading={isLoading}
                        loadingText="Salvando..."
                    >
                        Salvar Configurações
                    </Button>
                </HStack>

                <Alert status="info" borderRadius="xl">
                    <AlertIcon />
                    <Box>
                        <Text fontWeight="600" mb={2}>Informações:</Text>
                        <Text fontSize="sm">
                            • As configurações são salvas localmente no seu navegador<br />
                            • Algumas funcionalidades podem estar em desenvolvimento<br />
                            • Alterações de segurança podem exigir nova autenticação
                        </Text>
                    </Box>
                </Alert>
            </VStack>
        </Box>
    );
};

export default Configuracoes;