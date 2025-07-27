import api from './api';

export const pessoaService = {
    listar: async () => {
        const response = await api.get('/api/v1/pessoas');
        return response.data;
    },

    obterPorId: async (id) => {
        const response = await api.get(`/api/v1/pessoas/${id}`);
        return response.data;
    },

    criar: async (pessoa) => {
        const response = await api.post('/api/v1/pessoas', pessoa);
        return response.data;
    },

    atualizar: async (pessoa) => {
        const response = await api.put('/api/v1/pessoas', pessoa);
        return response.data;
    },

    remover: async (id) => {
        const response = await api.delete(`/api/v1/pessoas/${id}`);
        return response.data;
    },
};