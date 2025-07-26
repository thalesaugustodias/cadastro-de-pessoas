import api from './api';

export const pessoaService = {
    listar: async () => {
        const response = await api.get('/pessoas');
        return response.data;
    },

    obterPorId: async (id) => {
        const response = await api.get(`/pessoas/${id}`);
        return response.data;
    },

    criar: async (pessoa) => {
        const response = await api.post('/pessoas', pessoa);
        return response.data;
    },

    atualizar: async (pessoa) => {
        const response = await api.put('/pessoas', pessoa);
        return response.data;
    },

    remover: async (id) => {
        const response = await api.delete(`/pessoas/${id}`);
        return response.data;
    },
};