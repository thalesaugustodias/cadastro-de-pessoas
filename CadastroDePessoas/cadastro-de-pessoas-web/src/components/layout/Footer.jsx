import React from 'react';
import { 
    Box, 
    Text, 
    Flex, 
    HStack, 
    VStack,
    Icon,
    Link,
    Divider,
    Badge,
} from '@chakra-ui/react';
import { 
    FiHeart,
    FiCode,
    FiShield,
    FiZap,
} from 'react-icons/fi';

const Footer = () => {
    const currentYear = new Date().getFullYear();

    return (
        <Box
            as="footer"
            mt="auto"
            bg="white"
            borderTop="1px solid"
            borderColor="gray.100"
            py={8}
            px={8}
        >
            <VStack spacing={6}>
                {/* Linha principal */}
                <Flex
                    direction={{ base: 'column', md: 'row' }}
                    justify="space-between"
                    align="center"
                    w="full"
                    maxW="6xl"
                    mx="auto"
                >
                    {/* Logo e descrição */}
                    <VStack spacing={2} align={{ base: 'center', md: 'start' }}>
                        <HStack spacing={3}>
                            <Box
                                w="32px"
                                h="32px"
                                bg="linear-gradient(135deg, #1e77f3 0%, #26ece9 100%)"
                                borderRadius="lg"
                                display="flex"
                                alignItems="center"
                                justifyContent="center"
                            >
                                <Text
                                    color="white"
                                    fontSize="sm"
                                    fontWeight="bold"
                                >
                                    CP
                                </Text>
                            </Box>
                            <Text 
                                fontSize="lg" 
                                fontWeight="bold" 
                                color="gray.800"
                            >
                                Cadastro de Pessoas
                            </Text>
                        </HStack>
                        
                        <Text 
                            fontSize="sm" 
                            color="gray.600"
                            textAlign={{ base: 'center', md: 'left' }}
                        >
                            Sistema moderno e intuitivo para gerenciamento de cadastros
                        </Text>
                    </VStack>

                    {/* Features */}
                    <HStack spacing={8} mt={{ base: 4, md: 0 }}>
                        <VStack spacing={1} align="center">
                            <Icon as={FiZap} color="brand.500" />
                            <Text fontSize="xs" color="gray.500" fontWeight="600">
                                Rápido
                            </Text>
                        </VStack>
                        
                        <VStack spacing={1} align="center">
                            <Icon as={FiShield} color="success.500" />
                            <Text fontSize="xs" color="gray.500" fontWeight="600">
                                Seguro
                            </Text>
                        </VStack>
                        
                        <VStack spacing={1} align="center">
                            <Icon as={FiCode} color="accent.500" />
                            <Text fontSize="xs" color="gray.500" fontWeight="600">
                                Moderno
                            </Text>
                        </VStack>
                    </HStack>
                </Flex>

                <Divider />

                {/* Linha de copyright */}
                <Flex
                    direction={{ base: 'column', md: 'row' }}
                    justify="space-between"
                    align="center"
                    w="full"
                    maxW="6xl"
                    mx="auto"
                    fontSize="sm"
                    color="gray.500"
                >
                    <HStack spacing={2} mb={{ base: 2, md: 0 }}>
                        <Text>
                            © {currentYear} Cadastro de Pessoas.
                        </Text>
                        <Text>
                            Feito com
                        </Text>
                        <Icon as={FiHeart} color="red.500" />
                        <Text>
                            e tecnologia moderna
                        </Text>
                    </HStack>

                    <HStack spacing={4}>
                        <Badge colorScheme="brand" variant="subtle">
                            React 18
                        </Badge>
                        <Badge colorScheme="success" variant="subtle">
                            .NET 8
                        </Badge>
                        <Badge colorScheme="accent" variant="subtle">
                            Chakra UI
                        </Badge>
                    </HStack>
                </Flex>

                {/* Links adicionais */}
                <HStack spacing={6} fontSize="sm">
                    <Link 
                        color="gray.500" 
                        _hover={{ color: 'brand.500' }}
                        fontWeight="500"
                    >
                        Privacidade
                    </Link>
                    <Link 
                        color="gray.500" 
                        _hover={{ color: 'brand.500' }}
                        fontWeight="500"
                    >
                        Termos
                    </Link>
                    <Link 
                        color="gray.500" 
                        _hover={{ color: 'brand.500' }}
                        fontWeight="500"
                    >
                        Suporte
                    </Link>
                    <Link 
                        color="gray.500" 
                        _hover={{ color: 'brand.500' }}
                        fontWeight="500"
                    >
                        Documentação
                    </Link>
                </HStack>
            </VStack>
        </Box>
    );
};

export default Footer;