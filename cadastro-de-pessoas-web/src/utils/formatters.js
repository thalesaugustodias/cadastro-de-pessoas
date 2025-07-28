export const formatarData = (data) => {
    if (!data) return '';

    const date = new Date(data);
    return date.toLocaleDateString('pt-BR');
};

export const formatarCPF = (cpf) => {
    if (!cpf) return '';

    cpf = cpf.replace(/\D/g, '');

    if (cpf.length !== 11) {
        return cpf;
    }

    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
};

/**
 * Formata um endereço completo como uma string legível
 * @param {Object} endereco - Objeto de endereço
 * @returns {string} - Endereço formatado
 */
export const formatarEndereco = (endereco) => {
    if (!endereco) return '';
    
    const logradouro = endereco.Logradouro || endereco.logradouro || '';
    const numero = endereco.Numero || endereco.numero || '';
    const complemento = endereco.Complemento || endereco.complemento || '';
    const bairro = endereco.Bairro || endereco.bairro || '';
    const cidade = endereco.Cidade || endereco.cidade || '';
    const estado = endereco.Estado || endereco.estado || '';
    const cep = endereco.CEP || endereco.cep || '';
    
    if (logradouro && numero) {
        return `${logradouro}, ${numero}${complemento ? ', ' + complemento : ''}, ${bairro}, ${cidade} - ${estado}, ${cep}`;
    }
    
    return '';
};