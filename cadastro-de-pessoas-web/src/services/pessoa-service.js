import api, { resolveApiPath } from './api';

export const pessoaService = {
    listar: async () => {
        try {
            const response = await api.get(resolveApiPath('Pessoas'));
            return response.data;
        } catch (error) {
            console.error('Erro ao listar pessoas:', error);
            throw error;
        }
    },

    obterPorId: async (id) => {
        try {
            const response = await api.get(resolveApiPath(`Pessoas/${id}`));
            return response.data;
        } catch (error) {
            console.error(`Erro ao obter pessoa com ID ${id}:`, error);
            throw error;
        }
    },

    criar: async (pessoa) => {
        try {
            const response = await api.post(resolveApiPath('Pessoas'), pessoa);
            return response.data;
        } catch (error) {
            console.error('Erro ao criar pessoa:', error);
            throw error;
        }
    },

    atualizar: async (pessoa) => {
        try {
            const response = await api.put(resolveApiPath('Pessoas'), pessoa);
            return response.data;
        } catch (error) {
            console.error(`Erro ao atualizar pessoa com ID ${pessoa.id}:`, error);
            throw error;
        }
    },

    remover: async (id) => {
        try {
            const response = await api.delete(resolveApiPath(`Pessoas/${id}`));
            return response.data;
        } catch (error) {
            console.error(`Erro ao remover pessoa com ID ${id}:`, error);
            throw error;
        }
    },
    
    exportarExcel: async (campos) => {
        try {
            const response = await api.post(
                resolveApiPath('Pessoas/exportar/excel'), 
                campos, 
                { responseType: 'blob' }
            );
            return response.data;
        } catch (error) {
            console.error('Erro ao exportar para Excel:', error);
            throw error;
        }
    },

    exportarPdf: async (campos) => {
        try {
            const response = await api.post(
                resolveApiPath('Pessoas/exportar/pdf'), 
                campos, 
                { responseType: 'blob' }
            );
            return response.data;
        } catch (error) {
            console.error('Erro ao exportar para PDF:', error);
            throw error;
        }
    },

    downloadTemplateExcel: async () => {
        try {
            const response = await api.get(
                resolveApiPath('Importacao/template/excel'), 
                { responseType: 'blob' }
            );
            return response.data;
        } catch (error) {
            console.error('Erro ao baixar template Excel:', error);
            throw error;
        }
    },
    
    downloadTemplateCsv: async () => {
        try {
            const response = await api.get(
                resolveApiPath('Pessoas/importar/template'), 
                { responseType: 'blob' }
            );
            return response.data;
        } catch (error) {
            console.error('Erro ao baixar template CSV:', error);
            throw error;
        }
    },

    importarArquivo: async (arquivo, importarParcialmente = true) => {
        try {
            const formData = new FormData();
            formData.append('arquivo', arquivo);
            
            const response = await api.post(
                resolveApiPath(`Importacao?importarParcialmente=${importarParcialmente}`), 
                formData, 
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                }
            );
            return response.data;
        } catch (error) {
            console.error('Erro ao importar arquivo:', error);
            throw error;
        }
    },
    
    downloadErrorsExcel: async (errorsJson) => {
        try {
            const response = await api.get(
                resolveApiPath(`Importacao/erros-excel?errosJson=${encodeURIComponent(errorsJson)}`), 
                { responseType: 'blob' }
            );
            return response.data;
        } catch (error) {
            console.error('Erro ao baixar Excel de erros:', error);
            throw error;
        }
    },
    
    downloadErrorsCsv: async (errorsJson) => {
        try {
            const response = await api.get(
                resolveApiPath(`Importacao/erros-csv?errosJson=${encodeURIComponent(errorsJson)}`), 
                { responseType: 'blob' }
            );
            return response.data;
        } catch (error) {
            console.error('Erro ao baixar CSV de erros:', error);
            throw error;
        }
    },

    limparCache: async () => {
        try {
            const response = await api.post(resolveApiPath('Pessoas/limpar-cache'));
            return response.data;
        } catch (error) {
            console.error('Erro ao limpar cache:', error);
            throw error;
        }
    }
};