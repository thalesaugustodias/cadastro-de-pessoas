import React, { useState } from 'react';
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
    Icon,
    InputGroup,
    InputRightElement,
    IconButton,
    Alert,
    AlertIcon,
    Heading,
    Divider,
} from '@chakra-ui/react';

import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { FiSearch, FiUser, FiMapPin } from 'react-icons/fi';
import { maskCPF, maskCEP, maskTelefone } from '../../utils/masks';
import { validarCPF } from '../../utils/validators';
import { cepService } from '../../services/cep-service';
import { useNotification } from '../../hooks/useNotification';
import { useNavigate } from 'react-router-dom';

const schema = yup.object().shape({
    nome: yup.string().required('O nome é obrigatório'),
    email: yup.string().email('Digite um e-mail válido').nullable(),
    telefone: yup.string().nullable(),
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

    endereco: yup.object().shape({
        cep: yup.string().nullable(),
        logradouro: yup.string().nullable(),
        numero: yup.string().nullable(),
        complemento: yup.string().nullable(),
        bairro: yup.string().nullable(),
        cidade: yup.string().nullable(),
        estado: yup.string().nullable(),
    }).nullable(),
});

const PessoaForm = ({ initialData = {}, onSubmit, isLoading, isEdit = false }) => {
    const { showError, showSuccess } = useNotification();
    const [isLoadingCep, setIsLoadingCep] = useState(false);
    const navigate = useNavigate();

    const {
        register,
        handleSubmit,
        formState: { errors },
        setValue,
        watch,
        getValues,
    } = useForm({
        resolver: yupResolver(schema),
        defaultValues: {
            nome: initialData.Nome || '',
            email: initialData.Email || '',
            telefone: initialData.Telefone || '',
            dataNascimento: initialData.DataNascimento
                ? new Date(initialData.DataNascimento).toISOString().split('T')[0]
                : '',
            cpf: initialData.CPF || '',
            sexo: initialData.Sexo?.toString() || '',
            naturalidade: initialData.Naturalidade || '',
            nacionalidade: initialData.Nacionalidade || '',
            endereco: {
                cep: initialData.Endereco?.CEP || '',
                logradouro: initialData.Endereco?.Logradouro || '',
                numero: initialData.Endereco?.Numero || '',
                complemento: initialData.Endereco?.Complemento || '',
                bairro: initialData.Endereco?.Bairro || '',
                cidade: initialData.Endereco?.Cidade || '',
                estado: initialData.Endereco?.Estado || '',
            },
        },
    });

    const onSubmitForm = (data) => {
        if (isEdit && initialData.Id) {
            data.Id = initialData.Id;
        }
        
        const transformedData = {
            Id: data.Id,
            Nome: data.nome,
            Email: data.email,
            Telefone: data.telefone,
            DataNascimento: data.dataNascimento,
            CPF: data.cpf,
            Sexo: data.sexo ? parseInt(data.sexo) : null,
            Naturalidade: data.naturalidade,
            Nacionalidade: data.nacionalidade,
            Endereco: data.endereco ? {
                Id: data.endereco.id || (initialData.Endereco ? initialData.Endereco.Id : null),
                CEP: data.endereco.cep,
                Logradouro: data.endereco.logradouro,
                Numero: data.endereco.numero,
                Complemento: data.endereco.complemento,
                Bairro: data.endereco.bairro,
                Cidade: data.endereco.cidade,
                Estado: data.endereco.estado
            } : null
        };
        
        onSubmit(transformedData);
    };
    
    const cpf = watch('cpf');
    const telefone = watch('telefone');
    const cep = watch('endereco.cep');

    React.useEffect(() => {
        if (cpf) {
            setValue('cpf', maskCPF(cpf));
        }
    }, [cpf, setValue]);

    React.useEffect(() => {
        if (telefone) {
            setValue('telefone', maskTelefone(telefone));
        }
    }, [telefone, setValue]);

    React.useEffect(() => {
        if (cep) {
            setValue('endereco.cep', maskCEP(cep));
        }
    }, [cep, setValue]);

    const buscarCep = async () => {
        const cepValue = getValues('endereco.cep');
        
        if (!cepValue || cepValue.replace(/\D/g, '').length !== 8) {
            showError('Digite um CEP válido com 8 dígitos');
            return;
        }

        setIsLoadingCep(true);

        try {
            const result = await cepService.buscarCep(cepValue);
            
            if (result.success) {
                setValue('endereco.logradouro', result.data.logradouro);
                setValue('endereco.bairro', result.data.bairro);
                setValue('endereco.cidade', result.data.cidade);
                setValue('endereco.estado', result.data.estado);
                setValue('endereco.complemento', result.data.complemento);
                
                showSuccess('CEP encontrado!');
                
                document.getElementById('endereco.numero')?.focus();
            } else {
                showError(result.message);
            }
        } catch (error) {
            showError('Erro ao consultar CEP');
        } finally {
            setIsLoadingCep(false);
        }
    };

    const submitHandler = (data) => {
        data.cpf = data.cpf.replace(/\D/g, '');
        
        if (data.telefone) {
            data.telefone = data.telefone.replace(/\D/g, '');
        }

        if (data.endereco?.cep) {
            data.endereco.cep = data.endereco.cep.replace(/\D/g, '');
        }

        if (data.sexo) {
            data.sexo = parseInt(data.sexo);
        } else {
            data.sexo = null;
        }

        if (data.endereco) {
            Object.keys(data.endereco).forEach(key => {
                if (data.endereco[key] === '') {
                    data.endereco[key] = null;
                }
            });
            
            const hasEnderecoData = Object.values(data.endereco).some(value => value !== null && value !== '');
            if (!hasEnderecoData) {
                data.endereco = null;
            }
        }

        Object.keys(data).forEach(key => {
            if (key !== 'endereco' && data[key] === '') {
                data[key] = null;
            }
        });

        if (isEdit) {
            data.id = initialData.id;
        }

        onSubmit(data);
    };

    return (
        <Box as="form" onSubmit={handleSubmit(submitHandler)}>
            <VStack spacing={8} align="stretch">
                {/* Dados Pessoais */}
                <Box>
                    <HStack mb={4}>
                        <Icon as={FiUser} color="brand.500" />
                        <Heading size="md" color="gray.800">Dados Pessoais</Heading>
                    </HStack>
                    
                    <VStack spacing={6} align="stretch">
                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
                            <FormControl isInvalid={errors.nome}>
                                <FormLabel htmlFor="nome">Nome Completo*</FormLabel>
                                <Input id="nome" {...register('nome')} />
                                <FormErrorMessage>{errors.nome?.message}</FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={errors.email}>
                                <FormLabel htmlFor="email">E-mail</FormLabel>
                                <Input id="email" type="email" {...register('email')} />
                                <FormErrorMessage>{errors.email?.message}</FormErrorMessage>
                            </FormControl>
                        </SimpleGrid>

                        <SimpleGrid columns={{ base: 1, md: 3 }} spacing={6}>
                            <FormControl isInvalid={errors.cpf}>
                                <FormLabel htmlFor="cpf">CPF*</FormLabel>
                                <Input
                                    id="cpf"
                                    {...register('cpf')}
                                    disabled={isEdit}
                                    placeholder="000.000.000-00"
                                    maxLength={14}
                                />
                                <FormErrorMessage>{errors.cpf?.message}</FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={errors.telefone}>
                                <FormLabel htmlFor="telefone">Telefone</FormLabel>
                                <Input
                                    id="telefone"
                                    {...register('telefone')}
                                    placeholder="(00) 00000-0000"
                                    maxLength={15}
                                />
                                <FormErrorMessage>{errors.telefone?.message}</FormErrorMessage>
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
                                <Input 
                                    id="naturalidade" 
                                    {...register('naturalidade')} 
                                    placeholder="Cidade de nascimento"
                                />
                                <FormErrorMessage>{errors.naturalidade?.message}</FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={errors.nacionalidade}>
                                <FormLabel htmlFor="nacionalidade">Nacionalidade</FormLabel>
                                <Input 
                                    id="nacionalidade" 
                                    {...register('nacionalidade')} 
                                    placeholder="País de origem"
                                />
                                <FormErrorMessage>{errors.nacionalidade?.message}</FormErrorMessage>
                            </FormControl>
                        </SimpleGrid>
                    </VStack>
                </Box>

                <Divider />

                {/* Endereço */}
                <Box>
                    <HStack mb={4}>
                        <Icon as={FiMapPin} color="brand.500" />
                        <Heading size="md" color="gray.800">Endereço</Heading>
                    </HStack>

                    <VStack spacing={6} align="stretch">
                        <SimpleGrid columns={{ base: 1, md: 3 }} spacing={6}>
                            <FormControl isInvalid={errors.endereco?.cep}>
                                <FormLabel htmlFor="endereco.cep">CEP</FormLabel>
                                <InputGroup>
                                    <Input
                                        id="endereco.cep"
                                        {...register('endereco.cep')}
                                        placeholder="00000-000"
                                        maxLength={9}
                                    />
                                    <InputRightElement>
                                        <IconButton
                                            aria-label="Buscar CEP"
                                            icon={<FiSearch />}
                                            size="sm"
                                            variant="ghost"
                                            onClick={buscarCep}
                                            isLoading={isLoadingCep}
                                        />
                                    </InputRightElement>
                                </InputGroup>
                                <FormErrorMessage>{errors.endereco?.cep?.message}</FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={errors.endereco?.estado}>
                                <FormLabel htmlFor="endereco.estado">Estado</FormLabel>
                                <Input
                                    id="endereco.estado"
                                    {...register('endereco.estado')}
                                    placeholder="SP"
                                />
                                <FormErrorMessage>{errors.endereco?.estado?.message}</FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={errors.endereco?.cidade}>
                                <FormLabel htmlFor="endereco.cidade">Cidade</FormLabel>
                                <Input
                                    id="endereco.cidade"
                                    {...register('endereco.cidade')}
                                    placeholder="São Paulo"
                                />
                                <FormErrorMessage>{errors.endereco?.cidade?.message}</FormErrorMessage>
                            </FormControl>
                        </SimpleGrid>

                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
                            <FormControl isInvalid={errors.endereco?.logradouro}>
                                <FormLabel htmlFor="endereco.logradouro">Logradouro</FormLabel>
                                <Input
                                    id="endereco.logradouro"
                                    {...register('endereco.logradouro')}
                                    placeholder="Rua, Avenida, etc."
                                />
                                <FormErrorMessage>{errors.endereco?.logradouro?.message}</FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={errors.endereco?.bairro}>
                                <FormLabel htmlFor="endereco.bairro">Bairro</FormLabel>
                                <Input
                                    id="endereco.bairro"
                                    {...register('endereco.bairro')}
                                    placeholder="Nome do bairro"
                                />
                                <FormErrorMessage>{errors.endereco?.bairro?.message}</FormErrorMessage>
                            </FormControl>
                        </SimpleGrid>

                        <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
                            <FormControl isInvalid={errors.endereco?.numero}>
                                <FormLabel htmlFor="endereco.numero">Número</FormLabel>
                                <Input
                                    id="endereco.numero"
                                    {...register('endereco.numero')}
                                    placeholder="123"
                                />
                                <FormErrorMessage>{errors.endereco?.numero?.message}</FormErrorMessage>
                            </FormControl>

                            <FormControl isInvalid={errors.endereco?.complemento}>
                                <FormLabel htmlFor="endereco.complemento">Complemento</FormLabel>
                                <Input
                                    id="endereco.complemento"
                                    {...register('endereco.complemento')}
                                    placeholder="Apto, Bloco, etc."
                                />
                                <FormErrorMessage>{errors.endereco?.complemento?.message}</FormErrorMessage>
                            </FormControl>
                        </SimpleGrid>
                    </VStack>
                </Box>

                <Alert status="info" borderRadius="lg">
                    <AlertIcon />
                    <Box>
                        <strong>Dica:</strong> Digite o CEP e clique no ícone de busca para preencher automaticamente o endereço.
                    </Box>
                </Alert>

                <HStack justify="flex-end" spacing={4} pt={4}>
                    <Button
                        variant="outline"
                        colorScheme="gray"
                        onClick={() => navigate('/pessoas')}
                    >
                        Cancelar
                    </Button>
                    <Button
                        type="submit"
                        variant="primary"
                        isLoading={isLoading}
                        onClick={handleSubmit(onSubmitForm)}
                    >
                        {isEdit ? 'Atualizar' : 'Cadastrar'}
                    </Button>
                </HStack>
            </VStack>
        </Box>
    );
};

export default PessoaForm;