import React, { useState } from 'react';
import {
    Box,
    Heading,
    Text,
    Button,
    VStack,
    HStack,
    Card,
    CardBody,
    Icon,
    Input,
    Alert,
    AlertIcon,
    Progress,
    Table,
    Thead,
    Tbody,
    Tr,
    Th,
    Td,
    Badge,
} from '@chakra-ui/react';
import { FiUpload, FiFile, FiCheck, FiX, FiDownload } from 'react-icons/fi';
import { useNotification } from '../../hooks/useNotification';

const ImportarDados = () => {
    const { showSuccess, showError } = useNotification();
    const [file, setFile] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const [importResult, setImportResult] = useState(null);

    const handleFileSelect = (event) => {
        const selectedFile = event.target.files[0];
        if (selectedFile) {
            if (selectedFile.type === 'text/csv' || selectedFile.name.endsWith('.csv')) {
                setFile(selectedFile);
            } else {
                showError('Por favor, selecione apenas arquivos CSV');
            }
        }
    };

    const handleImport = async () => {
        if (!file) {
            showError('Selecione um arquivo CSV');
            return;
        }

        setIsLoading(true);

        // Simular importação
        setTimeout(() => {
            setImportResult({
                total: 150,
                success: 145,
                errors: 5,
                details: [
                    { linha: 12, erro: 'CPF inválido: 123.456.789-00' },
                    { linha: 34, erro: 'Email já cadastrado: joao@email.com' },
                    { linha: 67, erro: 'Data de nascimento inválida' },
                    { linha: 89, erro: 'Campo nome obrigatório' },
                    { linha: 112, erro: 'CEP não encontrado: 12345-999' },
                ]
            });
            setIsLoading(false);
            showSuccess('Importação concluída! 145 registros importados com sucesso.');
        }, 3000);
    };

    const downloadTemplate = () => {
        const csvContent = 'data:text/csv;charset=utf-8,Nome,Email,CPF,DataNascimento,Telefone,CEP,Logradouro,Numero,Complemento,Bairro,Cidade,Estado\n"João Silva","joao@email.com","123.456.789-01","1990-01-01","(11) 99999-9999","01310-100","Av. Paulista","1000","Apto 101","Bela Vista","São Paulo","SP"';
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement('a');
        link.setAttribute('href', encodedUri);
        link.setAttribute('download', 'template-importacao.csv');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    };

    return (
        <Box p={8}>
            <VStack spacing={8} align="stretch">
                {/* Header */}
                <Box>
                    <Heading size="lg" mb={2} color="gray.800">
                        Importar Dados
                    </Heading>
                    <Text color="gray.600">
                        Importe dados em lote através de arquivo CSV
                    </Text>
                </Box>

                {/* Template Download */}
                <Card>
                    <CardBody p={6}>
                        <HStack justify="space-between" align="center">
                            <Box>
                                <Heading size="md" mb={2}>
                                    Template de Importação
                                </Heading>
                                <Text color="gray.600">
                                    Baixe o modelo CSV com as colunas corretas
                                </Text>
                            </Box>
                            <Button
                                leftIcon={<FiDownload />}
                                variant="outline"
                                colorScheme="blue"
                                onClick={downloadTemplate}
                            >
                                Baixar Template
                            </Button>
                        </HStack>
                    </CardBody>
                </Card>

                {/* Upload Area */}
                <Card>
                    <CardBody p={6}>
                        <VStack spacing={6}>
                            <Box
                                border="2px dashed"
                                borderColor={file ? 'green.300' : 'gray.300'}
                                borderRadius="xl"
                                p={8}
                                textAlign="center"
                                bg={file ? 'green.50' : 'gray.50'}
                                w="100%"
                                transition="all 0.2s"
                            >
                                <Icon
                                    as={file ? FiCheck : FiUpload}
                                    boxSize="12"
                                    color={file ? 'green.500' : 'gray.400'}
                                    mb={4}
                                />
                                <Heading size="md" mb={2} color="gray.700">
                                    {file ? file.name : 'Selecione o arquivo CSV'}
                                </Heading>
                                <Text color="gray.500" mb={4}>
                                    {file 
                                        ? `Arquivo selecionado (${(file.size / 1024).toFixed(1)} KB)`
                                        : 'Arraste e solte ou clique para selecionar'
                                    }
                                </Text>
                                <Input
                                    type="file"
                                    accept=".csv"
                                    onChange={handleFileSelect}
                                    display="none"
                                    id="file-upload"
                                />
                                <Button
                                    as="label"
                                    htmlFor="file-upload"
                                    variant={file ? 'success' : 'outline'}
                                    leftIcon={<FiFile />}
                                    cursor="pointer"
                                >
                                    {file ? 'Alterar Arquivo' : 'Selecionar Arquivo'}
                                </Button>
                            </Box>

                            {file && (
                                <Button
                                    onClick={handleImport}
                                    variant="primary"
                                    size="lg"
                                    leftIcon={<FiUpload />}
                                    isLoading={isLoading}
                                    loadingText="Importando..."
                                    w="full"
                                >
                                    Iniciar Importação
                                </Button>
                            )}
                        </VStack>
                    </CardBody>
                </Card>

                {/* Progress */}
                {isLoading && (
                    <Card>
                        <CardBody p={6}>
                            <VStack spacing={4}>
                                <Text fontWeight="600">Processando arquivo...</Text>
                                <Progress
                                    size="lg"
                                    isIndeterminate
                                    colorScheme="blue"
                                    w="100%"
                                    borderRadius="full"
                                />
                            </VStack>
                        </CardBody>
                    </Card>
                )}

                {/* Results */}
                {importResult && (
                    <Card>
                        <CardBody p={6}>
                            <VStack spacing={6} align="stretch">
                                <Heading size="md">Resultado da Importação</Heading>

                                <HStack spacing={6}>
                                    <Badge colorScheme="blue" p={2} borderRadius="md">
                                        Total: {importResult.total}
                                    </Badge>
                                    <Badge colorScheme="green" p={2} borderRadius="md">
                                        Sucesso: {importResult.success}
                                    </Badge>
                                    <Badge colorScheme="red" p={2} borderRadius="md">
                                        Erros: {importResult.errors}
                                    </Badge>
                                </HStack>

                                {importResult.errors > 0 && (
                                    <Box>
                                        <Heading size="sm" mb={4}>Erros Encontrados:</Heading>
                                        <Table variant="modern" size="sm">
                                            <Thead>
                                                <Tr>
                                                    <Th>Linha</Th>
                                                    <Th>Erro</Th>
                                                </Tr>
                                            </Thead>
                                            <Tbody>
                                                {importResult.details.map((error, index) => (
                                                    <Tr key={index}>
                                                        <Td>{error.linha}</Td>
                                                        <Td>{error.erro}</Td>
                                                    </Tr>
                                                ))}
                                            </Tbody>
                                        </Table>
                                    </Box>
                                )}
                            </VStack>
                        </CardBody>
                    </Card>
                )}

                {/* Instructions */}
                <Alert status="info" borderRadius="xl">
                    <AlertIcon />
                    <Box>
                        <Text fontWeight="600" mb={2}>Instruções:</Text>
                        <Text fontSize="sm">
                            1. Baixe o template CSV<br />
                            2. Preencha os dados seguindo o formato<br />
                            3. Selecione o arquivo preenchido<br />
                            4. Clique em "Iniciar Importação"<br />
                            5. Aguarde o processamento
                        </Text>
                    </Box>
                </Alert>
            </VStack>
        </Box>
    );
};

export default ImportarDados;