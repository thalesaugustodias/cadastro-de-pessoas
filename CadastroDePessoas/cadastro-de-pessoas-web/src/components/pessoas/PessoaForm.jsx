import React from 'react';
import {
    Box,
    Button,
    FormControl,
    FormLabel,
    FormErrorMessage,
    Input,
    Select,
    SimpleGrid,
    VStack,
    HStack,
} from '@chakra-ui/react';

import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { maskCPF } from '../../utils/masks';  // Corrigido: maskCPF
import { validarCPF } from '../../utils/validators';

const schema = yup.object().shape({
    nome: yup.string().required('O nome é obrigatório'),
    email: yup.string().email('Digite um e-mail válido').nullable(),
    dataNascimento: yup
        .date()
        .typeError('Data inválida')
        .max(new Date(), 'A data de nascimento não pode ser no futuro')
        .min(new Date(1900, 0, 1), 'A data de nascimento deve ser após 01/01/1900')
        .required('A data de nascimento é obrigatória'),
    cpf: yup
        .string()
        .required('O CPF é obrigatório')
        .test('is-cpf', 'CPF inválido', (value) => validarCPF(value || '')),
    sexo: yup.string().nullable(),
    naturalidade: yup.string().nullable(),
    nacionalidade: yup.string().nullable(),
    endereco: yup.string().nullable(),
});

const PessoaForm = ({ initialData = {}, onSubmit, isLoading, isEdit = false }) => {
    const {
        register,
        handleSubmit,
        formState: { errors },
        setValue,
        watch,
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {
            nome: initialData.nome || '',
            email: initialData.email || '',
            dataNascimento: initialData.dataNascimento
                ? new Date(initialData.dataNascimento).toISOString().split('T')[0]
                : '',
            cpf: initialData.cpf || '',
            sexo: initialData.sexo?.toString() || '',
            naturalidade: initialData.naturalidade || '',
            nacionalidade: initialData.nacionalidade || '',
            endereco: initialData.endereco || '',
        },
    });

    const cpf = watch('cpf');

    // Aplicar máscara de CPF
    React.useEffect(() => {
        if (cpf) {
            setValue('cpf', maskCPF(cpf));  // Corrigido: maskCPF
        }
    }, [cpf, setValue]);

    const submitHandler = (data) => {
        // Remover formatação do CPF
        data.cpf = data.cpf.replace(/\D/g, '');

        // Converter sexo para número se estiver preenchido
        if (data.sexo) {
            data.sexo = parseInt(data.sexo);
        } else {
            data.sexo = null;
        }

        // Remover campos vazios
        Object.keys(data).forEach(key => {
            if (data[key] === '') {
                data[key] = null;
            }
        });

        // Se for edição, incluir o ID
        if (isEdit) {
            data.id = initialData.id;
        }

        onSubmit(data);
    };

    return (
        <Box as="form" onSubmit={handleSubmit(submitHandler)}>
            <VStack spacing={6} align="stretch">
                <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
                    <FormControl isInvalid={errors.nome}>
                        <FormLabel htmlFor="nome">Nome*</FormLabel>
                        <Input id="nome" {...register('nome')} />
                        <FormErrorMessage>{errors.nome?.message}</FormErrorMessage>
                    </FormControl>

                    <FormControl isInvalid={errors.email}>
                        <FormLabel htmlFor="email">E-mail</FormLabel>
                        <Input id="email" type="email" {...register('email')} />
                        <FormErrorMessage>{errors.email?.message}</FormErrorMessage>
                    </FormControl>
                </SimpleGrid>

                <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
                    <FormControl isInvalid={errors.cpf}>
                        <FormLabel htmlFor="cpf">CPF*</FormLabel>
                        <Input
                            id="cpf"
                            {...register('cpf')}
                            disabled={isEdit} // CPF não deve ser editável na edição
                        />
                        <FormErrorMessage>{errors.cpf?.message}</FormErrorMessage>
                    </FormControl>

                    <FormControl isInvalid={errors.dataNascimento}>
                        <FormLabel htmlFor="dataNascimento">Data de Nascimento*</FormLabel>
                        <Input
                            id="dataNascimento"
                            type="date"
                            {...register('dataNascimento')}
                        />
                        <FormErrorMessage>{errors.dataNascimento?.message}</FormErrorMessage>
                    </FormControl>
                </SimpleGrid>

                <SimpleGrid columns={{ base: 1, md: 3 }} spacing={6}>
                    <FormControl isInvalid={errors.sexo}>
                        <FormLabel htmlFor="sexo">Sexo</FormLabel>
                        <Select id="sexo" {...register('sexo')} placeholder="Selecione">
                            <option value="0">Masculino</option>
                            <option value="1">Feminino</option>
                            <option value="2">Outro</option>
                        </Select>
                        <FormErrorMessage>{errors.sexo?.message}</FormErrorMessage>
                    </FormControl>

                    <FormControl isInvalid={errors.naturalidade}>
                        <FormLabel htmlFor="naturalidade">Naturalidade</FormLabel>
                        <Input id="naturalidade" {...register('naturalidade')} />
                        <FormErrorMessage>{errors.naturalidade?.message}</FormErrorMessage>
                    </FormControl>

                    <FormControl isInvalid={errors.nacionalidade}>
                        <FormLabel htmlFor="nacionalidade">Nacionalidade</FormLabel>
                        <Input id="nacionalidade" {...register('nacionalidade')} />
                        <FormErrorMessage>{errors.nacionalidade?.message}</FormErrorMessage>
                    </FormControl>
                </SimpleGrid>

                <FormControl isInvalid={errors.endereco}>
                    <FormLabel htmlFor="endereco">Endereço</FormLabel>
                    <Input id="endereco" {...register('endereco')} />
                    <FormErrorMessage>{errors.endereco?.message}</FormErrorMessage>
                </FormControl>

                <HStack justify="flex-end" spacing={4} pt={4}>
                    <Button
                        variant="outline"
                        colorScheme="gray"
                        as={isEdit ? 'a' : 'button'}
                        href={isEdit ? `/pessoas/${initialData.id}` : '/pessoas'}
                    >
                        Cancelar
                    </Button>
                    <Button
                        type="submit"
                        colorScheme="blue"
                        isLoading={isLoading}
                    >
                        {isEdit ? 'Atualizar' : 'Cadastrar'}
                    </Button>
                </HStack>
            </VStack>
        </Box>
    );
};

export default PessoaForm;