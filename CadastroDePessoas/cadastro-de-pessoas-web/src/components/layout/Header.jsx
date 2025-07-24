import React from 'react';
import {
    Box,
    Flex,
    Text,
    IconButton,
    useColorModeValue,
    HStack,
    Menu,
    MenuButton,
    MenuList,
    MenuItem,
    Avatar,
    Image
} from '@chakra-ui/react';

import { HamburgerIcon } from '@chakra-ui/icons';
import { useAuth } from '../../hooks/useAuth';

const Header = ({ onSidebarToggle }) => {
    const { user, logout } = useAuth();

    return (
        <Box
            as="header"
            bg={useColorModeValue('white', 'gray.900')}
            px={4}
            py={2}
            boxShadow="sm"
            position="fixed"
            top={0}
            left={0}
            right={0}
            zIndex={100}
            height="60px"
        >
            <Flex h="100%" alignItems="center" justifyContent="space-between">
                <Flex alignItems="center">
                    <IconButton
                        aria-label="Menu"
                        icon={<HamburgerIcon />}
                        variant="ghost"
                        onClick={onSidebarToggle}
                        mr={4}
                        display={{ base: 'flex', md: 'none' }}
                    />

                    <Image
                        src="/assets/logo.png"
                        alt="Logo"
                        height="30px"
                        mr={3}
                    />

                    <Text
                        fontSize="xl"
                        fontWeight="bold"
                        color="brand.500"
                        display={{ base: 'none', md: 'flex' }}
                    >
                        Cadastro de Pessoas
                    </Text>
                </Flex>

                <HStack spacing={3}>
                    <Menu>
                        <MenuButton
                            as={Box}
                            rounded="full"
                            cursor="pointer"
                        >
                            <Avatar size="sm" name={user?.nome} />
                        </MenuButton>
                        <MenuList>
                            <MenuItem onClick={logout}>Sair</MenuItem>
                        </MenuList>
                    </Menu>
                </HStack>
            </Flex>
        </Box>
    );
};

export default Header;