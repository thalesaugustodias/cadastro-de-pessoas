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
} from '@chakra-ui/react';
import { 
    FiHeart,
    FiZap,
    FiShield,
    FiCode,
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
                            Sistema de gerenciamento de cadastros
                        </Text>
                    </VStack>                   
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
                            Desenvolvido por Thales Augusto Dias - Desafio Stefanini
                        </Text>                       
                    </HStack>

                    <HStack spacing={6} fontSize="sm">
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
                </Flex>
            </VStack>
        </Box>
    );
};

export default Footer;