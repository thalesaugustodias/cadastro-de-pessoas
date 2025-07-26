import React from 'react';
import {
    Box,
    Flex,
    VStack,
    Text,
    Icon,
    Divider,
    Badge,
    Tooltip,
    useBreakpointValue,
    HStack,
} from '@chakra-ui/react';

import { Link, useLocation } from 'react-router-dom';
import { 
    FiHome, 
    FiUsers, 
    FiPlusCircle, 
    FiSettings,
    FiUser,
    FiDownload,
    FiUpload,
} from 'react-icons/fi';

const SidebarItem = ({ icon, children, to, isActive, badge, description }) => {
    const isCollapsed = useBreakpointValue({ base: false, md: false });

    return (
        <Tooltip 
            label={isCollapsed ? children : ''} 
            placement="right"
            isDisabled={!isCollapsed}
        >
            <Flex
                align="center"
                px="4"
                py="3"
                my="1"
                borderRadius="xl"
                role="group"
                cursor="pointer"
                color={isActive ? 'white' : 'gray.600'}
                bg={isActive ? 'linear-gradient(135deg, #1e77f3 0%, #26ece9 100%)' : 'transparent'}
                _hover={{
                    bg: isActive 
                        ? 'linear-gradient(135deg, #1a6dd4 0%, #1fbcba 100%)' 
                        : 'gray.100',
                    color: isActive ? 'white' : 'brand.600',
                    transform: 'translateX(4px)',
                    boxShadow: isActive 
                        ? '0 4px 12px 0 rgba(30, 119, 243, 0.4)' 
                        : '0 2px 8px 0 rgba(0, 0, 0, 0.1)',
                }}
                as={Link}
                to={to}
                w="100%"
                transition="all 0.3s cubic-bezier(.08,.52,.52,1)"
                position="relative"
                overflow="hidden"
            >
                {/* Indicador lateral para item ativo */}
                {isActive && (
                    <Box
                        position="absolute"
                        left="0"
                        top="0"
                        bottom="0"
                        w="4px"
                        bg="white"
                        borderRadius="0 4px 4px 0"
                    />
                )}

                <HStack spacing="4" w="100%">
                    <Icon
                        as={icon}
                        fontSize="20"
                        transition="all 0.2s"
                        _groupHover={{
                            transform: 'scale(1.1)',
                        }}
                    />
                    
                    <Flex justify="space-between" align="center" w="100%">
                        <Box>
                            <Text 
                                fontSize="sm" 
                                fontWeight="600"
                                lineHeight="1.2"
                            >
                                {children}
                            </Text>
                            {description && (
                                <Text 
                                    fontSize="xs" 
                                    opacity="0.8"
                                    mt="1"
                                >
                                    {description}
                                </Text>
                            )}
                        </Box>
                        
                        {badge && (
                            <Badge
                                colorScheme={isActive ? 'whiteAlpha' : 'brand'}
                                borderRadius="full"
                                fontSize="xs"
                                px="2"
                            >
                                {badge}
                            </Badge>
                        )}
                    </Flex>
                </HStack>
            </Flex>
        </Tooltip>
    );
};

const SidebarSection = ({ title, children }) => (
    <Box mb="6">
        <Text
            px="4"
            mb="3"
            fontSize="xs"
            fontWeight="bold"
            textTransform="uppercase"
            letterSpacing="wider"
            color="gray.500"
        >
            {title}
        </Text>
        <VStack align="stretch" spacing="1">
            {children}
        </VStack>
    </Box>
);

const Sidebar = ({ isOpen }) => {
    const location = useLocation();

    return (
        <Box
            position="fixed"
            left={0}
            w={{ base: 'full', md: '280px' }}
            top="72px"
            bottom={0}
            bg="white"
            borderRight="1px solid"
            borderColor="gray.100"
            display={{ base: isOpen ? 'block' : 'none', md: 'block' }}
            transition="all 0.3s cubic-bezier(.08,.52,.52,1)"
            zIndex={900}
            overflowY="auto"
            boxShadow="0 4px 6px -1px rgba(0, 0, 0, 0.1)"
        >
            {/* Gradiente decorativo no topo */}
            <Box
                h="4"
                bg="linear-gradient(90deg, #1e77f3 0%, #26ece9 100%)"
            />

            <VStack align="stretch" p="6" spacing="2">
                {/* Dashboard */}
                <SidebarSection title="Dashboard">
                    <SidebarItem
                        icon={FiHome}
                        to="/"
                        isActive={location.pathname === '/'}
                        description="Visão geral do sistema"
                    >
                        Início
                    </SidebarItem>
                </SidebarSection>

                {/* Gerenciamento de Pessoas */}
                <SidebarSection title="Pessoas">
                    <SidebarItem
                        icon={FiUsers}
                        to="/pessoas"
                        isActive={location.pathname === '/pessoas'}
                        description="Lista completa"
                    >
                        Todas as Pessoas
                    </SidebarItem>

                    <SidebarItem
                        icon={FiPlusCircle}
                        to="/pessoas/criar"
                        isActive={location.pathname === '/pessoas/criar'}
                        description="Adicionar nova pessoa"
                    >
                        Nova Pessoa
                    </SidebarItem>

                    <SidebarItem
                        icon={FiUpload}
                        to="/pessoas/importar"
                        isActive={location.pathname === '/pessoas/importar'}
                        description="Upload em lote"
                    >
                        Importar Dados
                    </SidebarItem>

                    <SidebarItem
                        icon={FiDownload}
                        to="/pessoas/exportar"
                        isActive={location.pathname === '/pessoas/exportar'}
                        description="Exportar PDF/Excel"
                    >
                        Exportar Dados
                    </SidebarItem>
                </SidebarSection>

                <Divider />

                {/* Configurações */}
                <SidebarSection title="Sistema">
                    <SidebarItem
                        icon={FiUser}
                        to="/perfil"
                        isActive={location.pathname === '/perfil'}
                        description="Dados pessoais"
                    >
                        Meu Perfil
                    </SidebarItem>

                    <SidebarItem
                        icon={FiSettings}
                        to="/configuracoes"
                        isActive={location.pathname === '/configuracoes'}
                        description="Preferências do sistema"
                    >
                        Configurações
                    </SidebarItem>
                </SidebarSection>
            </VStack>
        </Box>
    );
};

export default Sidebar;