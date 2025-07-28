import React, { useState, useRef } from 'react';
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
    useDisclosure,
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalBody,
    ModalFooter,
    ModalCloseButton,
    Tooltip,
    Code,
    FormControl,
    FormLabel,
    Switch,
    Tabs,
    TabList,
    TabPanels,
    Tab,
    TabPanel,
    Accordion,
    AccordionItem,
    AccordionButton,
    AccordionPanel,
    AccordionIcon,
} from '@chakra-ui/react';
import { FiUpload, FiFile, FiCheck, FiX, FiDownload, FiAlertCircle, FiRefreshCw, FiInfo, FiList, FiTable, FiFileText } from 'react-icons/fi';
import { useNotification } from '../../hooks/useNotification';
import { pessoaService } from '../../services/pessoa-service';
import { saveAs } from 'file-saver';

const ImportarDados = () => {
    const { showSuccess, showError, showInfo } = useNotification();
    const [file, setFile] = useState(null);
    const [fileContents, setFileContents] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [isDownloading, setIsDownloading] = useState(false);
    const [importResult, setImportResult] = useState(null);
    const [importarParcialmente, setImportarParcialmente] = useState(true);
    const [isGeneratingErrorsFile, setIsGeneratingErrorsFile] = useState(false);
    const fileInputRef = useRef(null);
    const { isOpen, onOpen, onClose } = useDisclosure();
    const [templateType, setTemplateType] = useState("excel");

    const handleFileSelect = async (event) => {
        const selectedFile = event.target.files[0];
        if (selectedFile) {
            const fileExtension = selectedFile.name.split('.').pop().toLowerCase();
            
            if (['xlsx', 'xls', 'csv'].includes(fileExtension)) {
                setFile(selectedFile);
                
                if (fileExtension === 'csv') {
                    try {
                        const text = await selectedFile.text();
                        const lines = text.split('\n');
                        setFileContents(lines);
                    } catch (error) {
                        console.error('Erro ao ler arquivo:', error);
                        setFileContents([]);
                    }
                } else {
                    setFileContents([]);
                }
            } else {
                showError('Por favor, selecione apenas arquivos Excel (.xlsx, .xls) ou CSV (.csv)');
            }
        }
    };

    const handleImport = async () => {
        if (!file) {
            showError('Selecione um arquivo para importar');
            return;
        }

        setIsLoading(true);
        setImportResult(null);

        try {
            const result = await pessoaService.importarArquivo(file, importarParcialmente);
            setImportResult(result);
            
            const sucesso = result.sucesso !== undefined ? result.sucesso : result.Sucesso;
            const erros = result.erros !== undefined ? result.erros : result.Erros;
            const total = result.total !== undefined ? result.total : result.Total;
            
            if (sucesso > 0) {
                if (erros > 0) {
                    showInfo(`Importação parcial: ${sucesso} registros importados com sucesso e ${erros} erros.`, 'Importação Concluída');
                } else {
                    showSuccess(`Importação concluída! ${sucesso} registros importados com sucesso.`);
                }
            } else if (erros > 0) {
                showError(`Falha na importação: ${erros} erros encontrados.`);
            }
            
            try {
                await pessoaService.limparCache();
            } catch (error) {
                console.error('Erro ao limpar cache:', error);
            }
        } catch (error) {
            console.error('Erro durante a importação:', error);
            showError('Ocorreu um erro durante a importação. Verifique o formato do arquivo e tente novamente.');
        } finally {
            setIsLoading(false);
        }
    };

    const downloadTemplate = async () => {
        setIsDownloading(true);
        try {
            let blob;
            let filename;
            
            if (templateType === "excel") {
                blob = await pessoaService.downloadTemplateExcel();
                filename = 'template-importacao.xlsx';
            } else {
                blob = await pessoaService.downloadTemplateCsv();
                filename = 'template-importacao.csv';
            }
            
            saveAs(blob, filename);
            showSuccess(`Template ${templateType === "excel" ? "Excel" : "CSV"} baixado com sucesso!`);
        } catch (error) {
            console.error('Erro ao baixar template:', error);
            showError(`Não foi possível baixar o template ${templateType === "excel" ? "Excel" : "CSV"}. Tente novamente mais tarde.`);
        } finally {
            setIsDownloading(false);
        }
    };
    
    const downloadErrorsFile = async () => {
        if (!importResult || !getDetalhesErro().length) {
            showError('Não há erros para exportar');
            return;
        }
        
        setIsGeneratingErrorsFile(true);
        try {
            const errorsJson = JSON.stringify(getDetalhesErro());
            
            const isExcelFile = file && (file.name.endsWith('.xlsx') || file.name.endsWith('.xls'));
            
            let blob;
            let filename;
            
            if (isExcelFile) {
                blob = await pessoaService.downloadErrorsExcel(errorsJson);
                filename = 'registros-com-erro.xlsx';
            } else {
                blob = await pessoaService.downloadErrorsCsv(errorsJson);
                filename = 'registros-com-erro.csv';
            }
            
            saveAs(blob, filename);
            showSuccess(`Arquivo com erros gerado com sucesso!`);
        } catch (error) {
            console.error('Erro ao gerar arquivo de erros:', error);
            showError('Não foi possível gerar o arquivo com os erros.');
        } finally {
            setIsGeneratingErrorsFile(false);
        }
    };
    
    const resetImport = () => {
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
        setFile(null);
        setFileContents([]);
        setImportResult(null);
    };

    const formatarMensagemErro = (erro) => {
        const mensagem = erro.mensagem || erro.Mensagem || '';
        
        return mensagem
            .replace('Validation failed: -- ', '')
            .replace('CPF: ', 'CPF - ');
    };

    const getValorProblematico = (erro) => {
        const mensagem = erro.mensagem || erro.Mensagem || '';
        
        const match = mensagem.match(/'([^']+)'/);
        if (match && match[1]) {
            return match[1];
        }
        
        if (erro.valoresOriginais && Object.keys(erro.valoresOriginais).length > 0) {
            const campo = identificarCampoProblematico(mensagem);
            if (campo && erro.valoresOriginais[campo]) {
                return erro.valoresOriginais[campo];
            }
        }
        
        return "Valor não disponível";
    };

    const extrairValorDaLinha = (linhaNumero, campo) => {
        if (!fileContents || fileContents.length <= linhaNumero) return "Valor não disponível";
        
        try {
            const header = fileContents[0].split(',');
            const linha = fileContents[linhaNumero].split(',');
            
            const indice = header.findIndex(h => 
                h.trim().toLowerCase() === campo.toLowerCase() || 
                h.trim().replace(/"/g, '').toLowerCase() === campo.toLowerCase()
            );
            
            if (indice === -1) return "Campo não encontrado";
            
            let valor = linha[indice];
            if (valor) {
                valor = valor.replace(/"/g, '').trim();
                return valor || "Valor vazio";
            }
            
            return "Valor não disponível";
        } catch (error) {
            console.error('Erro ao extrair valor da linha:', error);
            return "Erro ao ler valor";
        }
    };

    const identificarCampoProblematico = (erroTexto) => {
        if (!erroTexto) return 'Desconhecido';
        
        if (erroTexto.includes('CPF')) return 'CPF';
        if (erroTexto.includes('nome') || erroTexto.includes('Nome')) return 'Nome';
        if (erroTexto.includes('data de nascimento') || erroTexto.includes('Data de nascimento')) return 'DataNascimento';
        if (erroTexto.includes('e-mail') || erroTexto.includes('Email')) return 'Email';
        if (erroTexto.includes('sexo') || erroTexto.includes('Sexo')) return 'Sexo';
        
        return 'Dados inválidos';
    };

    const visualizarErros = () => {
        onOpen();
    };

    const getDetalhesErro = () => {
        if (!importResult) return [];
        
        return importResult.detalhes || importResult.Detalhes || [];
    };
    
    const getRegistroCompletoComErro = (erro) => {
        if (erro.valoresOriginais && Object.keys(erro.valoresOriginais).length > 0) {
            return Object.entries(erro.valoresOriginais)
                .map(([campo, valor]) => `${campo}: ${valor}`)
                .join(', ');
        }
        
        return "Registro completo não disponível";
    };

    const getFileTypeDisplay = () => {
        if (!file) return "";
        
        const extension = file.name.split('.').pop().toLowerCase();
        switch (extension) {
            case 'xlsx':
                return "Excel (.xlsx)";
            case 'xls':
                return "Excel (.xls)";
            case 'csv':
                return "CSV (.csv)";
            default:
                return extension.toUpperCase();
        }
    };

    return (
        <Box p={8}>
            <VStack spacing={8} align="stretch">
                <Box>
                    <Heading size="lg" mb={2} color="gray.800">
                        Importar Dados
                    </Heading>
                    <Text color="gray.600">
                        Importe dados em lote através de arquivo Excel ou CSV
                    </Text>
                </Box>

                <Card>
                    <CardBody p={6}>
                        <VStack spacing={4}>
                            <Heading size="md" mb={2}>
                                Template de Importação
                            </Heading>
                            
                            <Tabs variant="soft-rounded" colorScheme="blue" width="100%" onChange={(index) => setTemplateType(index === 0 ? "excel" : "csv")}>
                                <TabList justifyContent="center">
                                    <Tab><Icon as={FiTable} mr={2} /> Excel</Tab>
                                    <Tab><Icon as={FiFileText} mr={2} /> CSV</Tab>
                                </TabList>
                                <TabPanels mt={4}>
                                    <TabPanel>
                                        <Text textAlign="center" color="gray.600" mb={4}>
                                            Baixe o modelo Excel com formatação e exemplos.
                                            <br />Este formato é mais fácil de trabalhar e evita problemas de importação.
                                        </Text>
                                    </TabPanel>
                                    <TabPanel>
                                        <Text textAlign="center" color="gray.600" mb={4}>
                                            Baixe o modelo CSV para compatibilidade com outros sistemas.
                                            <br />Recomendamos usar o formato Excel sempre que possível.
                                        </Text>
                                    </TabPanel>
                                </TabPanels>
                            </Tabs>
                            
                            <Button
                                leftIcon={<FiDownload />}
                                colorScheme="blue"
                                onClick={downloadTemplate}
                                isLoading={isDownloading}
                                loadingText="Baixando..."
                                size="lg"
                                width="100%"
                                maxW="400px"
                            >
                                Baixar Template {templateType === "excel" ? "Excel" : "CSV"}
                            </Button>
                        </VStack>
                    </CardBody>
                </Card>

                <Card>
                    <CardBody p={6}>
                        <VStack spacing={6}>
                            <FormControl display="flex" alignItems="center" justifyContent="flex-end">
                                <FormLabel htmlFor="importar-parcialmente" mb="0" fontSize="sm">
                                    Importar registros válidos mesmo com erros
                                </FormLabel>
                                <Switch 
                                    id="importar-parcialmente" 
                                    isChecked={importarParcialmente}
                                    onChange={(e) => setImportarParcialmente(e.target.checked)}
                                    colorScheme="green"
                                />
                            </FormControl>
                            
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
                                    {file ? file.name : 'Selecione o arquivo Excel ou CSV'}
                                </Heading>
                                <Text color="gray.500" mb={4}>
                                    {file 
                                        ? `Arquivo ${getFileTypeDisplay()} selecionado (${(file.size / 1024).toFixed(1)} KB)`
                                        : 'Arraste e solte ou clique para selecionar'
                                    }
                                </Text>
                                <Input
                                    type="file"
                                    accept=".xlsx,.xls,.csv"
                                    onChange={handleFileSelect}
                                    display="none"
                                    id="file-upload"
                                    ref={fileInputRef}
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

                {importResult && (
                    <Card>
                        <CardBody p={6}>
                            <VStack spacing={6} align="stretch">
                                <Heading size="md">Resultado da Importação</Heading>

                                <HStack spacing={6}>
                                    <Badge colorScheme="blue" p={2} borderRadius="md">
                                        Total: {importResult.total || importResult.Total}
                                    </Badge>
                                    <Badge colorScheme="green" p={2} borderRadius="md">
                                        Sucesso: {importResult.sucesso || importResult.Sucesso}
                                    </Badge>
                                    <Badge colorScheme="red" p={2} borderRadius="md">
                                        Erros: {importResult.erros || importResult.Erros}
                                    </Badge>
                                </HStack>

                                {(importResult.erros > 0 || importResult.Erros > 0) && (
                                    <Box textAlign="center">
                                        <Alert status="warning" borderRadius="md">
                                            <AlertIcon />
                                            <Box>
                                                <Text fontWeight="semibold">
                                                    Foram encontrados erros durante a importação. 
                                                </Text>
                                                <Text fontSize="sm" mt={1}>
                                                    {importarParcialmente 
                                                        ? "Os registros válidos foram importados com sucesso."
                                                        : "Nenhum registro foi importado devido aos erros."}
                                                </Text>
                                            </Box>
                                        </Alert>
                                        <HStack justify="center" spacing={4} mt={4}>
                                            <Button 
                                                colorScheme="blue" 
                                                leftIcon={<FiAlertCircle />}
                                                onClick={visualizarErros}
                                            >
                                                Ver Detalhes dos Erros
                                            </Button>
                                            <Button
                                                colorScheme="green"
                                                leftIcon={<FiDownload />}
                                                onClick={downloadErrorsFile}
                                                isLoading={isGeneratingErrorsFile}
                                                loadingText="Gerando arquivo..."
                                            >
                                                Baixar Registros com Erro
                                            </Button>
                                        </HStack>
                                    </Box>
                                )}
                                
                                {(importResult.sucesso > 0 || importResult.Sucesso > 0) && (
                                    <Box textAlign="center">
                                        <Alert status="success" borderRadius="md">
                                            <AlertIcon />
                                            <Text>
                                                {importResult.sucesso || importResult.Sucesso} registros foram importados com sucesso.
                                            </Text>
                                        </Alert>
                                    </Box>
                                )}
                                
                                <HStack justify="center" mt={2}>
                                    <Button 
                                        variant="ghost" 
                                        leftIcon={<FiRefreshCw />}
                                        onClick={resetImport}
                                        size="sm"
                                    >
                                        Nova Importação
                                    </Button>
                                </HStack>
                            </VStack>
                        </CardBody>
                    </Card>
                )}

                <Alert status="info" borderRadius="xl">
                    <AlertIcon />
                    <Box>
                        <Text fontWeight="600" mb={2}>Instruções:</Text>
                        <Text fontSize="sm">
                            1. Baixe o template Excel ou CSV<br />
                            2. Preencha os dados seguindo o formato<br />
                            3. Selecione o arquivo preenchido<br />
                            4. Clique em "Iniciar Importação"<br />
                            5. Aguarde o processamento
                        </Text>
                    </Box>
                </Alert>

                <Alert status="warning" borderRadius="xl">
                    <AlertIcon />
                    <Box>
                        <Text fontWeight="600" mb={2}>Atenção ao formato:</Text>
                        <Text fontSize="sm">
                            • <strong>Recomendamos usar Excel</strong> (.xlsx) para evitar problemas de importação<br />
                            • Cada registro deve estar em uma linha separada<br />
                            • Datas devem estar no formato AAAA-MM-DD (ex: 1990-01-31)<br />
                            • CPF deve ser único (não pode já existir no sistema)<br />
                            • O template Excel já está pré-formatado com exemplos
                        </Text>
                    </Box>
                </Alert>
            </VStack>
            
            {importResult && (
                <Modal isOpen={isOpen} onClose={onClose} size="5xl">
                    <ModalOverlay backdropFilter="blur(5px)" />
                    <ModalContent>
                        <ModalHeader>
                            <HStack>
                                <Text>Detalhes dos Erros</Text>
                                <Badge colorScheme="red" ml={2}>
                                    {(importResult.erros || importResult.Erros)} erros encontrados
                                </Badge>
                                <Tooltip label="Cada erro está associado a uma linha específica no arquivo. A linha 1 é o cabeçalho." fontSize="sm">
                                    <span><Icon as={FiInfo} color="blue.400" /></span>
                                </Tooltip>
                            </HStack>
                        </ModalHeader>
                        <ModalCloseButton />
                        <ModalBody pb={6}>
                            {getDetalhesErro().length > 0 ? (
                                <VStack spacing={4} align="stretch">
                                    <Alert status="info" size="sm">
                                        <AlertIcon />
                                        <VStack align="start" spacing={1}>
                                            <Text fontSize="sm" fontWeight="medium">
                                                Corrija os erros abaixo no seu arquivo e importe novamente.
                                            </Text>
                                            <Text fontSize="xs">
                                                A linha indicada corresponde à posição no arquivo (linha 1 = cabeçalho).
                                                Você pode baixar apenas os registros com erro para corrigir e reimportar.
                                            </Text>
                                        </VStack>
                                    </Alert>
                                    
                                    <Table variant="simple" size="sm">
                                        <Thead>
                                            <Tr>
                                                <Th>Linha</Th>
                                                <Th>Campo</Th>
                                                <Th>Valor</Th>
                                                <Th>Problema</Th>
                                            </Tr>
                                        </Thead>
                                        <Tbody>
                                            {getDetalhesErro().map((erro, index) => {
                                                const linha = erro.linha || erro.Linha || (index + 2);
                                                const mensagemErro = formatarMensagemErro(erro);
                                                const campoProblema = identificarCampoProblematico(mensagemErro);
                                                const valorProblematico = getValorProblematico(erro);
                                                
                                                return (
                                                    <Tr key={index} _hover={{ bg: "gray.50" }}>
                                                        <Td>
                                                            <Badge colorScheme="red">{linha}</Badge>
                                                        </Td>
                                                        <Td>
                                                            <Badge colorScheme="blue">{campoProblema}</Badge>
                                                        </Td>
                                                        <Td>
                                                            <Code colorScheme="red">{valorProblematico}</Code>
                                                        </Td>
                                                        <Td>{mensagemErro}</Td>
                                                    </Tr>
                                                );
                                            })}
                                        </Tbody>
                                    </Table>
                                    
                                    <Accordion allowToggle mt={4}>
                                        <AccordionItem>
                                            <h2>
                                                <AccordionButton>
                                                    <Box flex="1" textAlign="left" fontWeight="medium">
                                                        <Icon as={FiList} mr={2} /> Detalhes completos dos registros com erro
                                                    </Box>
                                                    <AccordionIcon />
                                                </AccordionButton>
                                            </h2>
                                            <AccordionPanel pb={4}>
                                                {getDetalhesErro().map((erro, index) => {
                                                    const linha = erro.linha || erro.Linha || (index + 2);
                                                    return (
                                                        <Box key={index} mb={4} p={3} borderRadius="md" borderWidth="1px">
                                                            <HStack mb={2}>
                                                                <Badge colorScheme="red">Linha {linha}</Badge>
                                                                <Badge colorScheme="orange">
                                                                    {formatarMensagemErro(erro)}
                                                                </Badge>
                                                            </HStack>
                                                            <Code w="100%" p={2} fontSize="xs" display="block" whiteSpace="pre-wrap">
                                                                {getRegistroCompletoComErro(erro)}
                                                            </Code>
                                                        </Box>
                                                    );
                                                })}
                                            </AccordionPanel>
                                        </AccordionItem>
                                    </Accordion>
                                    
                                    <Alert status="warning" size="sm">
                                        <AlertIcon />
                                        <Box>
                                            <Text fontWeight="bold" fontSize="sm">Dicas para correção:</Text>
                                            <Text fontSize="sm">
                                                • CPF: Deve conter 11 dígitos numéricos e ser válido<br />
                                                • Data de Nascimento: Deve estar no formato AAAA-MM-DD<br />
                                                • Email: Verifique se está em formato válido (exemplo@dominio.com)<br />
                                                • Campos obrigatórios: Nome, CPF e Data de Nascimento não podem estar vazios
                                            </Text>
                                        </Box>
                                    </Alert>
                                </VStack>
                            ) : (
                                <Text>Não há detalhes adicionais sobre os erros.</Text>
                            )}
                        </ModalBody>
                        <ModalFooter>
                            <HStack spacing={3}>
                                <Button 
                                    colorScheme="green" 
                                    leftIcon={<FiDownload />}
                                    onClick={downloadErrorsFile}
                                    isLoading={isGeneratingErrorsFile}
                                >
                                    Baixar Registros com Erro
                                </Button>
                                <Button onClick={onClose}>Fechar</Button>
                            </HStack>
                        </ModalFooter>
                    </ModalContent>
                </Modal>
            )}
        </Box>
    );
};

export default ImportarDados;