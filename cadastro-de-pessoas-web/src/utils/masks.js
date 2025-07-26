// Funções utilitárias para aplicar máscaras em campos de texto

/**
 * Aplica máscara de CPF: 000.000.000-00
 * @param {string} value
 * @returns {string}
 */
export const maskCPF = (value) => {
    if (!value) return '';
    
    return value
        .replace(/\D/g, '') // Remove tudo que não é dígito
        .replace(/(\d{3})(\d)/, '$1.$2') // Coloca um ponto entre o terceiro e o quarto dígitos
        .replace(/(\d{3})(\d)/, '$1.$2') // Coloca um ponto entre o terceiro e o quarto dígitos
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2'); // Coloca um hífen entre o terceiro e o quarto dígitos
};

/**
 * Aplica máscara de CNPJ: 00.000.000/0000-00
 * @param {string} value
 * @returns {string}
 */
export function maskCNPJ(value) {
  return value
    .replace(/\D/g, '')
    .replace(/(\d{2})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1/$2')
    .replace(/(\d{4})(\d{1,2})$/, '$1-$2')
    .slice(0, 18);
}

/**
 * Aplica máscara de telefone: (00) 00000-0000 ou (00) 0000-0000
 * @param {string} value
 * @returns {string}
 */
export const maskPhone = (value) => {
    if (!value) return '';
    
    value = value.replace(/\D/g, ''); // Remove tudo que não é dígito
    
    if (value.length <= 10) {
        // Telefone fixo: (00) 0000-0000
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{4})(\d)/, '$1-$2');
    } else {
        // Celular: (00) 00000-0000
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{5})(\d)/, '$1-$2')
            .substring(0, 15); // Limita a 15 caracteres
    }
};

/**
 * Aplica máscara de telefone: (00) 00000-0000 ou (00) 0000-0000
 * Alias para maskPhone para compatibilidade
 * @param {string} value
 * @returns {string}
 */
export const maskTelefone = (value) => {
    if (!value) return '';
    
    value = value.replace(/\D/g, ''); // Remove tudo que não é dígito
    
    if (value.length <= 10) {
        // Telefone fixo: (00) 0000-0000
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{4})(\d)/, '$1-$2');
    } else {
        // Celular: (00) 00000-0000
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{5})(\d)/, '$1-$2')
            .substring(0, 15); // Limita a 15 caracteres
    }
};

/**
 * Aplica máscara de CEP: 00000-000
 * @param {string} value
 * @returns {string}
 */
export const maskCEP = (value) => {
    if (!value) return '';
    
    return value
        .replace(/\D/g, '') // Remove tudo que não é dígito
        .replace(/(\d{5})(\d)/, '$1-$2') // Coloca um hífen entre o quinto e o sexto dígitos
        .substring(0, 9); // Limita a 9 caracteres
};

export const unMask = (value) => {
    if (!value) return '';
    return value.replace(/\D/g, '');
};

export const maskRG = (value) => {
    if (!value) return '';
    
    return value
        .replace(/\D/g, '')
        .replace(/(\d{2})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1})$/, '$1-$2')
        .substring(0, 12);
};

export const maskMoney = (value) => {
    if (!value) return '';
    
    // Remove tudo que não é dígito
    const onlyDigits = value.replace(/\D/g, '');
    
    // Converte para número e divide por 100 para ter os centavos
    const numberValue = parseFloat(onlyDigits) / 100;
    
    // Formata como moeda brasileira
    return numberValue.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
};