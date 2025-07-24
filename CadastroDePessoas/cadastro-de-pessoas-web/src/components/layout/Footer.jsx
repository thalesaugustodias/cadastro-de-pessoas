import React from 'react';
import { Box, Text, useColorModeValue } from '@chakra-ui/react';

const Footer = () => {
    return (
        <Box
            as="footer"
            mt="auto"
            py={4}
            textAlign="center"
            bg={useColorModeValue('white', 'gray.900')}
            borderTopWidth="1px"
            borderTopColor={useColorModeValue('gray.200', 'gray.700')}
        >
            <Text fontSize="sm" color="gray.500">
                © {new Date().getFullYear()} Cadastro de Pessoas. Todos os direitos reservados.
            </Text>
        </Box>
    );
};

export default Footer;