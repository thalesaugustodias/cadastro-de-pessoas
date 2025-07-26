import axios from 'axios';

export const cepService = {
    buscarCep: async (cep) => {
        try {
            const cepLimpo = cep.replace(/\D/g, '');
            
            if (cepLimpo.length !== 8) {
                throw new Error('CEP deve ter 8 dígitos');
            }

            const response = await axios.get(`https://viacep.com.br/ws/${cepLimpo}/json/`);
            
            if (response.data.erro) {
                throw new Error('CEP não encontrado');
            }

            return {
                success: true,
                data: {
                    cep: response.data.cep,
                    logradouro: response.data.logradouro,
                    bairro: response.data.bairro,
                    cidade: response.data.localidade,
                    estado: response.data.uf,
                    complemento: response.data.complemento || ''
                }
            };
        } catch (error) {
            return {
                success: false,
                message: error.message || 'Erro ao consultar CEP'
            };
        }
    }
};

export default cepService;