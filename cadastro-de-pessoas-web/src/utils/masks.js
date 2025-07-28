/**
 * Aplica máscara de CPF: 000.000.000-00
 * @param {string} value
 * @returns {string}
 */
export const maskCPF = (value) => {
    if (!value) return '';
    
    const digits = value.replace(/\D/g, '').substring(0, 11);
    
    return digits
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d)/, '$1.$2')
        .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
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

    value = value.replace(/\D/g, '');

    if (value.length <= 10) {
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{4})(\d)/, '$1-$2');
    } else {
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{5})(\d)/, '$1-$2')
            .substring(0, 15);
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

    value = value.replace(/\D/g, '');

    if (value.length <= 10) {
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{4})(\d)/, '$1-$2');
    } else {
        return value
            .replace(/(\d{2})(\d)/, '($1) $2')
            .replace(/(\d{5})(\d)/, '$1-$2')
            .substring(0, 15);
    }
};

/**
 * Aplica máscara de CEP: 00000-000
 * @param {string} value
 * @returns {string}
 */
export const maskCEP = (value) => {
    if (!value) return '';
    
    const digits = value.replace(/\D/g, '').substring(0, 8);
    
    return digits
        .replace(/(\d{5})(\d{1,3})/, '$1-$2');
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
    
    const onlyDigits = value.replace(/\D/g, '');

    const numberValue = parseFloat(onlyDigits) / 100;
    
    return numberValue.toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
};