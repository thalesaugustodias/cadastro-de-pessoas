import React from 'react';
import { Flex, Button, IconButton, Tooltip } from '@chakra-ui/react';
import { FiEdit, FiTrash2, FiEye } from 'react-icons/fi';
import { Link } from 'react-router-dom';

const ActionButtons = ({
    id,
    onDelete,
    showView = true,
    showEdit = true,
    showDelete = true,
    baseRoute = '/pessoas'
}) => {
    return (
        <Flex gap={2} justify="flex-end">
            {showView && (
                <Tooltip label="Visualizar">
                    <IconButton
                        as={Link}
                        to={`${baseRoute}/${id}`}
                        icon={<FiEye />}
                        colorScheme="blue"
                        variant="ghost"
                        size="sm"
                        aria-label="Visualizar"
                    />
                </Tooltip>
            )}

            {showEdit && (
                <Tooltip label="Editar">
                    <IconButton
                        as={Link}
                        to={`${baseRoute}/editar/${id}`}
                        icon={<FiEdit />}
                        colorScheme="green"
                        variant="ghost"
                        size="sm"
                        aria-label="Editar"
                    />
                </Tooltip>
            )}

            {showDelete && (
                <Tooltip label="Excluir">
                    <IconButton
                        icon={<FiTrash2 />}
                        colorScheme="red"
                        variant="ghost"
                        size="sm"
                        aria-label="Excluir"
                        onClick={() => onDelete(id)}
                    />
                </Tooltip>
            )}
        </Flex>
    );
};

export default ActionButtons;