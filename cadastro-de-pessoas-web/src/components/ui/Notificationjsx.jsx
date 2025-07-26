import React from 'react';
import {
    Alert,
    AlertIcon,
    AlertTitle,
    AlertDescription,
    CloseButton
} from '@chakra-ui/react';

const Notification = ({
    status = 'info',
    title,
    message,
    isClosable = true,
    onClose
}) => {
    return (
        <Alert status={status} borderRadius="md" mb={4}>
            <AlertIcon />
            {title && <AlertTitle mr={2}>{title}</AlertTitle>}
            <AlertDescription>{message}</AlertDescription>
            {isClosable && <CloseButton position="absolute" right="8px" top="8px" onClick={onClose} />}
        </Alert>
    );
};

export default Notification;