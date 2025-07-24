import React from 'react';
import {
    AlertDialog,
    AlertDialogBody,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogContent,
    AlertDialogOverlay,
    Button,
} from '@chakra-ui/react';

const Confirm = ({
    isOpen,
    onClose,
    onConfirm,
    title = 'Confirma��o',
    message = 'Tem certeza que deseja realizar esta a��o?',
    confirmText = 'Confirmar',
    cancelText = 'Cancelar',
    isLoading = false,
}) => {
    const cancelRef = React.useRef();

    return (
        <AlertDialog
            isOpen={isOpen}
            leastDestructiveRef={cancelRef}
            onClose={onClose}
            isCentered
        >
            <AlertDialogOverlay>
                <AlertDialogContent>
                    <AlertDialogHeader fontSize="lg" fontWeight="bold">
                        {title}
                    </AlertDialogHeader>

                    <AlertDialogBody>
                        {message}
                    </AlertDialogBody>

                    <AlertDialogFooter>
                        <Button ref={cancelRef} onClick={onClose} isDisabled={isLoading}>
                            {cancelText}
                        </Button>
                        <Button
                            colorScheme="red"
                            onClick={onConfirm}
                            ml={3}
                            isLoading={isLoading}
                        >
                            {confirmText}
                        </Button>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialogOverlay>
        </AlertDialog>
    );
};

export default Confirm;