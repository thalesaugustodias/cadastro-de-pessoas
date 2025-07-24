import React from 'react';
import {
    Box,
    Flex,
    VStack,
    Text,
    Icon,
    useColorModeValue,
    Divider
} from '@chakra-ui/react';

import { Link, useLocation } from 'react-router-dom';
import { FiHome, FiUsers, FiPlusCircle, FiList } from 'react-icons/fi';

const SidebarItem = ({ icon, children, to, isActive }) => {
    return (
        <Flex
            align="center"
            px="4"
            py="3"
            my="1"
            borderRadius="md"
            role="group"
            cursor="pointer"
            color={isActive ? 'white' : 'gray.600'}
            bg={isActive ? 'brand.500' : 'transparent'}
            _hover={{
                bg: isActive ? 'brand.600' : 'gray.100',
                color: isActive ? 'white' : 'brand.500',
            }}
            as={Link}
            to={to}
            w="100%"
        >
            <Icon
                mr="4"
                fontSize="16"
                as={icon}
            />
            <Text fontSize="sm" fontWeight="medium">{children}</Text>
        </Flex>
    );
};

const Sidebar = ({ isOpen }) => {
    const location = useLocation();

    return (
        <Box
            position="fixed"
            left={0}
            w={{ base: 'full', md: '64' }}
            top="60px"
            bottom={0}
            bg={useColorModeValue('white', 'gray.900')}
            borderRightWidth="1px"
            borderRightColor={useColorModeValue('gray.200', 'gray.700')}
            display={{ base: isOpen ? 'block' : 'none', md: 'block' }}
            transition="transform 0.3s"
            zIndex={90}
            overflowY="auto"
            pb="10"
        >
            <VStack align="stretch" mt="6" px="4" spacing="1">
                <SidebarItem
                    icon={FiHome}
                    to="/"
                    isActive={location.pathname === '/'}
                >
                    Início
                </SidebarItem>

                <Box mt="6" mb="2">
                    <Text
                        px="4"
                        fontSize="xs"
                        fontWeight="semibold"
                        textTransform="uppercase"
                        letterSpacing="wider"
                        color="gray.500"
                    >
                        Gerenciamento
                    </Text>
                    <Divider mt="2" />
                </Box>

                <SidebarItem
                    icon={FiUsers}
                    to="/pessoas"
                    isActive={location.pathname === '/pessoas'}
                >
                    Pessoas
                </SidebarItem>

                <SidebarItem
                    icon={FiPlusCircle}
                    to="/pessoas/criar"
                    isActive={location.pathname === '/pessoas/criar'}
                >
                    Nova Pessoa
                </SidebarItem>
            </VStack>
        </Box>
    );
};

export default Sidebar;