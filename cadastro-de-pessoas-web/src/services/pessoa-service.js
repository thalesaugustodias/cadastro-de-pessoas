import api, { resolveApiPath } from './api';

export const pessoaService = {
    listar: async () => {
        const response = await api.get(resolveApiPath('pessoas'));
        return response.data;
    },

    obterPorId: async (id) => {
        const response = await api.get(resolveApiPath(`pessoas/${id}`));
        return response.data;
    },

    criar: async (pessoa) => {
        const response = await api.post(resolveApiPath('pessoas'), pessoa);
        return response.data;
    },

    atualizar: async (pessoa) => {
        const response = await api.put(resolveApiPath('pessoas'), pessoa);
        return response.data;
    },

    remover: async (id) => {
        const response = await api.delete(resolveApiPath(`pessoas/${id}`));
        return response.data;
    },
};