import axios from 'axios';

// Servi�o para consulta de CEP via ViaCEP
export const cepService = {
    buscarCep: async (cep) => {
        try {
            // Remove caracteres n�o num�ricos
            const cepLimpo = cep.replace(/\D/g, '');
            
            // Verifica se o CEP tem 8 d�gitos
            if (cepLimpo.length !== 8) {
                throw new Error('CEP deve ter 8 d�gitos');
            }

            const response = await axios.get(`https://viacep.com.br/ws/${cepLimpo}/json/`);
            
            if (response.data.erro) {
                throw new Error('CEP n�o encontrado');
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