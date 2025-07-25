import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/layout/Layout';
import Login from './pages/auth/Login';
import Home from './pages/Home';
import ListarPessoas from './pages/pessoas/ListarPessoas';
import CriarPessoa from './pages/pessoas/CriarPessoa';
import EditarPessoa from './pages/pessoas/EditarPessoa';
import DetalhesPessoa from './pages/pessoas/DetalhesPessoa';
import NotFound from './pages/NotFound';
import { useAuth } from './hooks/useAuth';

const ProtectedRoute = ({ children }) => {
    const { authenticated } = useAuth();

    if (!authenticated) {
        return <Navigate to="/login" replace />;
    }

    return children;
};

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/login" element={<Login />} />

            <Route path="/" element={
                <ProtectedRoute>
                    <Layout />
                </ProtectedRoute>
            }>
                <Route index element={<Home />} />
                <Route path="pessoas">
                    <Route index element={<ListarPessoas />} />
                    <Route path="criar" element={<CriarPessoa />} />
                    <Route path="editar/:id" element={<EditarPessoa />} />
                    <Route path=":id" element={<DetalhesPessoa />} />
                </Route>
            </Route>

            <Route path="*" element={<NotFound />} />
        </Routes>
    );
};

export default AppRoutes;