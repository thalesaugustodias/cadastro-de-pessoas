import React from 'react';
import { Flex, Spinner, Text } from '@chakra-ui/react';

const Loading = ({ text = 'Carregando...' }) => {
    return (
        <Flex direction="column" align="center" justify="center" h="100%" minH="200px">
            <Spinner
                thickness="4px"
                speed="0.65s"
                emptyColor="gray.200"
                color="brand.500"
                size="xl"
            />
            <Text mt={4} fontSize="lg" color="gray.600">
                {text}
            </Text>
        </Flex>
    );
};

export default Loading;