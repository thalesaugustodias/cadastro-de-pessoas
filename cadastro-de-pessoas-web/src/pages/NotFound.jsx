import React from 'react';
import { Box, Heading, Text, Button, VStack, useColorModeValue } from '@chakra-ui/react';
import { FiArrowLeft } from 'react-icons/fi';
import { Link } from 'react-router-dom';

const NotFound = () => {
    return (
        <Box
            minH="100vh"
            display="flex"
            alignItems="center"
            justifyContent="center"
            bg={useColorModeValue('gray.50', 'gray.800')}
            px={4}
        >
            <VStack spacing={6} textAlign="center">
                <Heading as="h1" size="4xl" colorScheme="blue">
                    404
                </Heading>

                <Heading as="h2" size="xl">
                    Página não encontrada
                </Heading>

                <Text fontSize="lg" maxW="md">
                    A página que você está procurando pode ter sido removida,
                    renomeada ou está temporariamente indisponível.
                </Text>

                <Button
                    as={Link}
                    to="/"
                    leftIcon={<FiArrowLeft />}
                    colorScheme="blue"
                    size="lg"
                >
                    Voltar para o início
                </Button>
            </VStack>
        </Box>
    );
};

export default NotFound;