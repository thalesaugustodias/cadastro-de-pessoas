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