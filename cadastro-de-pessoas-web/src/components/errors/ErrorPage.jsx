import React from 'react';
import {
    Box,
    Heading,
    Text,
    Button,
    VStack,
    HStack,
    useColorModeValue,
    Icon,
    Container,
    Image,
} from '@chakra-ui/react';
import { FiAlertTriangle, FiHome } from 'react-icons/fi';
import { Link } from 'react-router-dom';

const ErrorPage = ({ 
    title = "Página não encontrada", 
    message = "A página que você está procurando não existe ou foi movida.", 
    icon = FiAlertTriangle,
    statusCode = "404",
    showHomeButton = true,
    showBackButton = true,
    customButtons = null,
    illustration = null
}) => {
    const bgColor = useColorModeValue('gray.50', 'gray.900');
    const cardBg = useColorModeValue('white', 'gray.800');

    return (
        <Box 
            minH="100vh" 
            bg={bgColor} 
            py={20} 
            px={4} 
            display="flex" 
            alignItems="center" 
            justifyContent="center"
        >
            <Container maxW="container.md">
                <Box
                    bg={cardBg}
                    rounded="xl"
                    shadow="lg"
                    overflow="hidden"
                    p={{ base: 6, md: 10 }}
                >
                    <VStack spacing={8} align="center" textAlign="center">
                        {statusCode && (
                            <Heading 
                                size="4xl" 
                                fontWeight="bold" 
                                color="brand.500"
                                opacity={0.6}
                            >
                                {statusCode}
                            </Heading>
                        )}

                        <Icon as={icon} boxSize={16} color="red.500" />

                        {illustration && (
                            <Box maxW="300px" mx="auto" my={4}>
                                <Image src={illustration} alt="Ilustração de erro" />
                            </Box>
                        )}

                        <VStack spacing={2}>
                            <Heading size="xl">{title}</Heading>
                            <Text fontSize="lg" color="gray.600">
                                {message}
                            </Text>
                        </VStack>

                        <HStack spacing={4} pt={4}>
                            {customButtons ? (
                                customButtons
                            ) : (
                                <>
                                    {showHomeButton && (
                                        <Button
                                            as={Link}
                                            to="/"
                                            colorScheme="brand"
                                            leftIcon={<FiHome />}
                                            size="lg"
                                        >
                                            Voltar para o Início
                                        </Button>
                                    )}
                                    {showBackButton && (
                                        <Button
                                            onClick={() => window.history.back()}
                                            variant="outline"
                                            size="lg"
                                        >
                                            Voltar
                                        </Button>
                                    )}
                                </>
                            )}
                        </HStack>
                    </VStack>
                </Box>
            </Container>
        </Box>
    );
};

export default ErrorPage;