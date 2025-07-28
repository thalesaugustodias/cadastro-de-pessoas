import React from 'react';
import {
    Box,
    Heading,
    Text,
    Divider,
    SimpleGrid,
    Badge,
    HStack,
    Button,
    Flex,
    useDisclosure,
    Icon,
    VStack,
    Card,
    CardHeader,
    CardBody,
    Tooltip,
    Grid,
    GridItem,
} from '@chakra-ui/react';

import { 
    FiEdit, 
    FiTrash2, 
    FiArrowLeft, 
    FiUser, 
    FiMail, 
    FiPhone, 
    FiMapPin, 
    FiCalendar, 
    FiFlag, 
    FiClock,
    FiHeart,
    FiFileText
} from 'react-icons/fi';
import { Link } from 'react-router-dom';
import { formatarData, formatarCPF, formatarEndereco } from '../../utils/formatters';
import Confirm from '../ui/Confirm';
import Loading from '../ui/Loading';

const InfoItem = ({ label, value, icon, placeholder = 'Não informado' }) => (
    <Box mb={4}>
        <HStack spacing={2} mb={1}>
            {icon && <Icon as={icon} color="brand.500" />}
            <Text fontSize="sm" color="gray.600" fontWeight="medium">
                {label}
            </Text>
        </HStack>
        <Text fontSize="md" fontWeight={value ? "normal" : "light"} pl={icon ? "6" : "0"}>
            {value || placeholder}
        </Text>
    </Box>
);

const PessoaItem = ({
    pessoa,
    isLoading,
    onDelete
}) => {
    const { isOpen, onOpen, onClose } = useDisclosure();
    const [isDeleting, setIsDeleting] = React.useState(false);

    const handleDelete = async () => {
        setIsDeleting(true);
        await onDelete(pessoa.Id);
        setIsDeleting(false);
        onClose();
    };

    if (isLoading) {
        return <Loading />;
    }

    if (!pessoa) {
        return (
            <Box textAlign="center" py={10}>
                <Text fontSize="lg" mb={4}>Pessoa não encontrada</Text>
                <Button
                    as={Link}
                    to="/pessoas"
                    colorScheme="blue"
                    leftIcon={<FiArrowLeft />}
                >
                    Voltar para a lista
                </Button>
            </Box>
        );
    }

    return (
        <Box>
            {/* Cabeçalho com foto, nome e ações */}
            <Card mb={6} variant="outline" shadow="sm">
                <CardBody>
                    <Flex
                        justify="space-between"
                        align={{ base: 'flex-start', md: 'center' }}
                        direction={{ base: 'column', md: 'row' }}
                        gap={4}
                    >
                        <Flex align="center" gap={4}>
                            <Flex
                                bg="brand.50" 
                                w="80px" 
                                h="80px" 
                                borderRadius="full" 
                                align="center" 
                                justify="center"
                                border="3px solid"
                                borderColor="brand.100"
                            >
                                <Icon as={FiUser} boxSize="40px" color="brand.500" />
                            </Flex>
                            
                            <Box>
                                <Heading as="h2" size="lg" color="gray.800">
                                    {pessoa.Nome}
                                </Heading>
                                <HStack mt={2} spacing={2} flexWrap="wrap">
                                    {pessoa.Sexo === 0 && 
                                        <Badge colorScheme="blue" variant="solid" px={2} py={1} borderRadius="full">
                                            Masculino
                                        </Badge>
                                    }
                                    {pessoa.Sexo === 1 && 
                                        <Badge colorScheme="pink" variant="solid" px={2} py={1} borderRadius="full">
                                            Feminino
                                        </Badge>
                                    }
                                    {pessoa.Sexo === 2 && 
                                        <Badge colorScheme="purple" variant="solid" px={2} py={1} borderRadius="full">
                                            Outro
                                        </Badge>
                                    }
                                    {pessoa.Sexo === null && 
                                        <Badge colorScheme="gray" variant="solid" px={2} py={1} borderRadius="full">
                                            Sexo não informado
                                        </Badge>
                                    }

                                    <Badge colorScheme="green" variant="solid" px={2} py={1} borderRadius="full">
                                        {pessoa.Idade} anos
                                    </Badge>
                                    
                                    <Badge colorScheme="cyan" variant="solid" px={2} py={1} borderRadius="full">
                                        {pessoa.Nacionalidade || "Brasileiro"}
                                    </Badge>
                                </HStack>
                            </Box>
                        </Flex>

                        <HStack spacing={4} mt={{ base: 4, md: 0 }}>
                            <Button
                                as={Link}
                                to="/pessoas"
                                leftIcon={<FiArrowLeft />}
                                variant="outline"
                                size="sm"
                            >
                                Voltar
                            </Button>

                            <Tooltip label="Editar pessoa" placement="top">
                                <Button
                                    as={Link}
                                    to={`/pessoas/editar/${pessoa.Id}`}
                                    leftIcon={<FiEdit />}
                                    colorScheme="green"
                                    size="sm"
                                >
                                    Editar
                                </Button>
                            </Tooltip>

                            <Tooltip label="Excluir pessoa" placement="top">
                                <Button
                                    leftIcon={<FiTrash2 />}
                                    colorScheme="red"
                                    onClick={onOpen}
                                    size="sm"
                                >
                                    Excluir
                                </Button>
                            </Tooltip>
                        </HStack>
                    </Flex>
                </CardBody>
            </Card>

            {/* Cartões de informações */}
            <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6} mb={6}>
                <Card variant="outline" shadow="sm">
                    <CardHeader borderBottom="1px solid" borderColor="gray.100" bg="gray.50" py={3} px={5}>
                        <Flex align="center" gap={2}>
                            <Icon as={FiFileText} color="brand.500" />
                            <Heading as="h3" size="sm" fontWeight="600" color="gray.700">
                                Informações Pessoais
                            </Heading>
                        </Flex>
                    </CardHeader>
                    <CardBody pt={4}>
                        <Grid templateColumns={{ base: "1fr", lg: "repeat(2, 1fr)" }} gap={4}>
                            <GridItem>
                                <InfoItem
                                    label="CPF"
                                    value={formatarCPF(pessoa.CPF)}
                                    icon={FiFileText}
                                />
                            </GridItem>
                            <GridItem>
                                <InfoItem
                                    label="E-mail"
                                    value={pessoa.Email}
                                    icon={FiMail}
                                />
                            </GridItem>
                            <GridItem>
                                <InfoItem
                                    label="Telefone"
                                    value={pessoa.Telefone}
                                    icon={FiPhone}
                                />
                            </GridItem>
                            <GridItem>
                                <InfoItem
                                    label="Data de Nascimento"
                                    value={formatarData(pessoa.DataNascimento)}
                                    icon={FiCalendar}
                                />
                            </GridItem>
                            <GridItem>
                                <InfoItem
                                    label="Naturalidade"
                                    value={pessoa.Naturalidade}
                                    icon={FiMapPin}
                                />
                            </GridItem>
                            <GridItem>
                                <InfoItem
                                    label="Nacionalidade"
                                    value={pessoa.Nacionalidade}
                                    icon={FiFlag}
                                />
                            </GridItem>
                        </Grid>
                    </CardBody>
                </Card>

                <Card variant="outline" shadow="sm">
                    <CardHeader borderBottom="1px solid" borderColor="gray.100" bg="gray.50" py={3} px={5}>
                        <Flex align="center" gap={2}>
                            <Icon as={FiMapPin} color="brand.500" />
                            <Heading as="h3" size="sm" fontWeight="600" color="gray.700">
                                Localização e Cadastro
                            </Heading>
                        </Flex>
                    </CardHeader>
                    <CardBody pt={4}>
                        <InfoItem
                            label="Endereço Completo"
                            value={pessoa.EnderecoCompleto || formatarEndereco(pessoa.Endereco)}
                            icon={FiMapPin}
                        />

                        {pessoa.Endereco && (
                            <Grid templateColumns={{ base: "1fr", lg: "repeat(2, 1fr)" }} gap={4} mt={4}>
                                <GridItem>
                                    <InfoItem
                                        label="CEP"
                                        value={pessoa.Endereco.CEP}
                                    />
                                </GridItem>
                                <GridItem>
                                    <InfoItem
                                        label="Estado"
                                        value={pessoa.Endereco.Estado}
                                    />
                                </GridItem>
                            </Grid>
                        )}

                        <Divider my={4} />
                        
                        <Grid templateColumns={{ base: "1fr", lg: "repeat(2, 1fr)" }} gap={4}>
                            <GridItem>
                                <InfoItem
                                    label="Data de Cadastro"
                                    value={formatarData(pessoa.DataCadastro)}
                                    icon={FiClock}
                                />
                            </GridItem>
                            <GridItem>
                                {pessoa.DataAtualizacao && (
                                    <InfoItem
                                        label="Última Atualização"
                                        value={formatarData(pessoa.DataAtualizacao)}
                                        icon={FiClock}
                                    />
                                )}
                            </GridItem>
                        </Grid>
                    </CardBody>
                </Card>
            </SimpleGrid>

            {/* Confirmação de exclusão */}
            <Confirm
                isOpen={isOpen}
                onClose={onClose}
                onConfirm={handleDelete}
                title="Excluir Pessoa"
                message={
                    <Box textAlign="center">
                        <Text mb={4}>
                            Tem certeza que deseja excluir <Text as="span" fontWeight="bold">{pessoa.Nome}</Text>?
                        </Text>
                        <Text fontSize="sm" color="red.500">
                            Esta ação não pode ser desfeita.
                        </Text>
                    </Box>
                }
                isLoading={isDeleting}
            />
        </Box>
    );
};

export default PessoaItem;