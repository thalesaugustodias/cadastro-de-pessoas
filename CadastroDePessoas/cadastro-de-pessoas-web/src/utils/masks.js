// Funções utilitárias para aplicar máscaras em campos de texto

/**
 * Aplica máscara de CPF: 000.000.000-00
 * @param {string} value
 * @returns {string}
 */
export function maskCPF(value) {
  return value
    .replace(/\D/g, '')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})(\d{1,2})$/, '$1-$2')
    .slice(0, 14);
}

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
export function maskPhone(value) {
  const cleaned = value.replace(/\D/g, '');
  if (cleaned.length > 10) {
    return cleaned
      .replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
      .slice(0, 15);
  }
  return cleaned
    .replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3')
    .slice(0, 14);
}

/**
 * Aplica máscara de CEP: 00000-000
 * @param {string} value
 * @returns {string}
 */
export function maskCEP(value) {
  return value
    .replace(/\D/g, '')
    .replace(/(\d{5})(\d{1,3})$/, '$1-$2')
    .slice(0, 9);
}

export const mascaraData = (value) => {
    return value
        .replace(/\D/g, '')
        .replace(/(\d{2})(\d)/, '$1/$2')
        .replace(/(\d{2})(\d)/, '$1/$2')
        .replace(/(\d{4})\d+?$/, '$1');
};