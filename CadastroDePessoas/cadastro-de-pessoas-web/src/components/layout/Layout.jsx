import React, { useState } from 'react';
import { Box, Flex, useDisclosure } from '@chakra-ui/react';
import { Outlet } from 'react-router-dom';
import Header from './Header';
import SideBar from './SideBar';  // Corrigido: SideBar com 'B' maiúsculo
import Footer from './Footer';

const Layout = () => {
    const { isOpen, onToggle } = useDisclosure();

    return (
        <Flex direction="column" minH="100vh">
            <Header onSidebarToggle={onToggle} />

            <Flex flex="1" mt="60px">
                <SideBar isOpen={isOpen} />

                <Box
                    flex="1"
                    ml={{ base: 0, md: '64' }}
                    p={5}
                    bg="gray.50"
                    minH="calc(100vh - 60px)"
                    transition="margin-left 0.3s"
                >
                    <Box
                        bg="white"
                        borderRadius="lg"
                        boxShadow="sm"
                        p={6}
                        height="100%"
                    >
                        <Outlet />
                    </Box>
                </Box>
            </Flex>

            <Footer />
        </Flex>
    );
};

export default Layout;