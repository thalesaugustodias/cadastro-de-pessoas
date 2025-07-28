import React from 'react';
import {
    Box,
    Flex,
    Text,
    IconButton,
    HStack,
    Menu,
    MenuButton,
    MenuList,
    MenuItem,
    Avatar,
    Tooltip,
    useBreakpointValue,
    Divider,
} from '@chakra-ui/react';
import { Link } from 'react-router-dom';
import { HamburgerIcon, ChevronDownIcon } from '@chakra-ui/icons';
import { FiUser, FiLogOut } from 'react-icons/fi';
import { useAuth } from '../../hooks/useAuth';

const Header = ({ onSidebarToggle }) => {
    const { user, logout } = useAuth();
    const isMobile = useBreakpointValue({ base: true, md: false });

    return (
        <Box
            as="header"
            bg="white"
            px={6}
            py={4}
            boxShadow="0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)"
            position="fixed"
            top={0}
            left={0}
            right={0}
            zIndex={1000}
            height="72px"
            borderBottom="1px solid"
            borderColor="gray.100"
        >
            <Flex h="100%" alignItems="center" justifyContent="space-between">
                {/* Logo e Menu Mobile */}
                <Flex alignItems="center">
                    {isMobile && (
                        <IconButton
                            aria-label="Abrir menu"
                            icon={<HamburgerIcon />}
                            variant="ghost"
                            onClick={onSidebarToggle}
                            mr={4}
                            size="lg"
                            _hover={{
                                bg: 'gray.100',
                                transform: 'scale(1.05)',
                            }}
                        />
                    )}

                    <Flex alignItems="center">
                        <Box
                            w="40px"
                            h="40px"
                            bg="linear-gradient(135deg, #1e77f3 0%, #26ece9 100%)"
                            borderRadius="xl"
                            display="flex"
                            alignItems="center"
                            justifyContent="center"
                            mr={3}
                            boxShadow="0 4px 12px 0 rgba(30, 119, 243, 0.3)"
                        >
                            <Text
                                color="white"
                                fontSize="xl"
                                fontWeight="bold"
                            >
                                CP
                            </Text>
                        </Box>

                        <Box>
                            <Text
                                fontSize="xl"
                                fontWeight="bold"
                                color="gray.800"
                                lineHeight="1.2"
                            >
                                Cadastro de Pessoas
                            </Text>
                            <Text
                                fontSize="sm"
                                color="gray.500"
                                display={{ base: 'none', lg: 'block' }}
                            >
                                Sistema de Gerenciamento
                            </Text>
                        </Box>
                    </Flex>
                </Flex>

                {/* Menu do usuário */}
                <HStack spacing={4}>
                    <Menu>
                        <MenuButton
                            as={Flex}
                            alignItems="center"
                            cursor="pointer"
                            bg="gray.50"
                            p={2}
                            borderRadius="xl"
                            _hover={{
                                bg: 'gray.100',
                                transform: 'scale(1.02)',
                            }}
                            transition="all 0.2s"
                        >
                            <HStack spacing={3}>
                                <Avatar 
                                    size="sm" 
                                    name={user?.nome || 'Usuário'}
                                    bg="brand.500"
                                    color="white"
                                    border="2px solid"
                                    borderColor="white"
                                    shadow="md"
                                />
                                {!isMobile && (
                                    <Box textAlign="left">
                                        <Text 
                                            fontSize="sm" 
                                            fontWeight="600" 
                                            color="gray.800"
                                            lineHeight="1.2"
                                        >
                                            {user?.nome || 'Usuário'}
                                        </Text>
                                        <Text 
                                            fontSize="xs" 
                                            color="gray.500"
                                        >
                                            {user?.email || 'usuario@exemplo.com'}
                                        </Text>
                                    </Box>
                                )}
                                <ChevronDownIcon color="gray.400" />
                            </HStack>
                        </MenuButton>
                        
                        <MenuList
                            bg="white"
                            border="1px solid"
                            borderColor="gray.200"
                            borderRadius="xl"
                            boxShadow="xl"
                            py={4}
                            minW="240px"
                        >
                            <Box px={4} pb={3}>
                                <Text 
                                    fontSize="sm" 
                                    fontWeight="600" 
                                    color="gray.800"
                                >
                                    {user?.nome || 'Usuário'}
                                </Text>
                                <Text 
                                    fontSize="xs" 
                                    color="gray.500"
                                >
                                    {user?.email || 'usuario@exemplo.com'}
                                </Text>
                            </Box>
                            
                            <Divider />
                            
                            <MenuItem
                                icon={<FiUser />}
                                fontWeight="500"
                                py={3}
                                _hover={{ bg: 'gray.50' }}
                                as={Link}
                                to="/perfil"
                            >
                                Meu Perfil
                            </MenuItem>
                            
                            <Divider />
                            
                            <MenuItem
                                icon={<FiLogOut />}
                                fontWeight="500"
                                py={3}
                                color="danger.600"
                                _hover={{ bg: 'danger.50', color: 'danger.700' }}
                                onClick={logout}
                            >
                                Sair da Conta
                            </MenuItem>
                        </MenuList>
                    </Menu>
                </HStack>
            </Flex>
        </Box>
    );
};

export default Header;