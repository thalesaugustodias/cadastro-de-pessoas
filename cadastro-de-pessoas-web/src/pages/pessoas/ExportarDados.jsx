import React, { useState, useEffect } from 'react';
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
    Checkbox,
    CheckboxGroup,
    Stack,
    Alert,
    AlertIcon,
    Progress,
    Badge,
    Grid,
    GridItem,
} from '@chakra-ui/react';
import { FiDownload, FiFile, FiFileText } from 'react-icons/fi';
import { useNotification } from '../../hooks/useNotification';
import { pessoaService } from '../../services/pessoa-service';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';

const ExportarDados = () => {
    const { showSuccess, showError } = useNotification();
    const [pessoas, setPessoas] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [exportFormat, setExportFormat] = useState('pdf');
    const [selectedFields, setSelectedFields] = useState([
        'nome', 'email', 'cpf', 'dataNascimento', 'telefone'
    ]);

    const availableFields = [
        { value: 'nome', label: 'Nome' },
        { value: 'email', label: 'E-mail' },
        { value: 'cpf', label: 'CPF' },
        { value: 'dataNascimento', label: 'Data de Nascimento' },
        { value: 'telefone', label: 'Telefone' },
        { value: 'endereco.cep', label: 'CEP' },
        { value: 'endereco.logradouro', label: 'Logradouro' },
        { value: 'endereco.numero', label: 'N�mero' },
        { value: 'endereco.bairro', label: 'Bairro' },
        { value: 'endereco.cidade', label: 'Cidade' },
        { value: 'endereco.estado', label: 'Estado' },
    ];

    useEffect(() => {
        loadPessoas();
    }, []);

    const loadPessoas = async () => {
        try {
            const data = await pessoaService.listar();
            setPessoas(data);
        } catch (error) {
            showError('Erro ao carregar dados');
        }
    };

    const getFieldValue = (pessoa, fieldPath) => {
        const keys = fieldPath.split('.');
        let value = pessoa;
        
        for (const key of keys) {
            value = value?.[key];
        }
        
        if (fieldPath === 'dataNascimento' && value) {
            return new Date(value).toLocaleDateString('pt-BR');
        }
        
        return value || '-';
    };

    const exportToPDF = () => {
        const doc = new jsPDF('l', 'mm', 'a4');
        
        // Título
        doc.setFontSize(18);
        doc.text('Relatório de Pessoas Cadastradas', 14, 20);
        
        doc.setFontSize(10);
        doc.text(`Gerado em: ${new Date().toLocaleDateString('pt-BR')} às ${new Date().toLocaleTimeString('pt-BR')}`, 14, 30);
        doc.text(`Total de registros: ${pessoas.length}`, 14, 35);

        // Preparar dados para a tabela
        const headers = selectedFields.map(field => 
            availableFields.find(f => f.value === field)?.label || field
        );

        const data = pessoas.map(pessoa => 
            selectedFields.map(field => getFieldValue(pessoa, field))
        );

        // Adicionar tabela
        doc.autoTable({
            head: [headers],
            body: data,
            startY: 45,
            styles: {
                fontSize: 8,
                cellPadding: 2,
            },
            headStyles: {
                fillColor: [30, 119, 243],
                textColor: 255,
                fontStyle: 'bold',
            },
            alternateRowStyles: {
                fillColor: [245, 245, 245],
            },
        });

        // Salvar
        doc.save(`pessoas-cadastradas-${new Date().toISOString().split('T')[0]}.pdf`);
    };

    const exportToExcel = async () => {
        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet('Pessoas Cadastradas');

        // Configurar cabeçalhos
        const headers = selectedFields.map(field => 
            availableFields.find(f => f.value === field)?.label || field
        );

        worksheet.addRow(headers);

        // Estilizar cabeçalho
        const headerRow = worksheet.getRow(1);
        headerRow.eachCell((cell) => {
            cell.font = { bold: true, color: { argb: 'FFFFFF' } };
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: '1E77F3' }
            };
            cell.alignment = { vertical: 'middle', horizontal: 'center' };
            cell.border = {
                top: { style: 'thin' },
                left: { style: 'thin' },
                bottom: { style: 'thin' },
                right: { style: 'thin' }
            };
        });

        // Adicionar dados
        pessoas.forEach(pessoa => {
            const row = selectedFields.map(field => getFieldValue(pessoa, field));
            worksheet.addRow(row);
        });

        // Ajustar largura das colunas
        worksheet.columns.forEach(column => {
            column.width = 20;
        });

        // Adicionar bordas nas células de dados
        worksheet.eachRow((row, rowNumber) => {
            if (rowNumber > 1) {
                row.eachCell((cell) => {
                    cell.border = {
                        top: { style: 'thin' },
                        left: { style: 'thin' },
                        bottom: { style: 'thin' },
                        right: { style: 'thin' }
                    };
                });
            }
        });

        // Gerar buffer e fazer download
        const buffer = await workbook.xlsx.writeBuffer();
        const blob = new Blob([buffer], { 
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' 
        });
        
        saveAs(blob, `pessoas-cadastradas-${new Date().toISOString().split('T')[0]}.xlsx`);
    };

    const handleExport = async () => {
        if (selectedFields.length === 0) {
            showError('Selecione pelo menos um campo para exportar');
            return;
        }

        setIsLoading(true);

        try {
            await new Promise(resolve => setTimeout(resolve, 1500));

            if (exportFormat === 'pdf') {
                exportToPDF();
            } else {
                await exportToExcel();
            }

            showSuccess(`Arquivo ${exportFormat.toUpperCase()} exportado com sucesso!`);
        } catch (error) {
            showError('Erro ao exportar arquivo');
            console.error('Erro na exportação:', error);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Box p={8}>
            <VStack spacing={8} align="stretch">
                {/* Header */}
                <Box>
                    <Heading size="lg" mb={2} color="gray.800">
                        Exportar Dados
                    </Heading>
                    <Text color="gray.600">
                        Exporte os dados cadastrados para PDF ou Excel
                    </Text>
                </Box>

                <Grid templateColumns={{ base: '1fr', lg: '1fr 1fr' }} gap={8}>
                    {/* Configurações */}
                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            {/* Formato */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <Heading size="md">Formato de Exportação</Heading>
                                        <HStack spacing={4}>
                                            <Button
                                                variant={exportFormat === 'pdf' ? 'primary' : 'outline'}
                                                leftIcon={<FiFileText />}
                                                onClick={() => setExportFormat('pdf')}
                                                flex="1"
                                            >
                                                PDF
                                            </Button>
                                            <Button
                                                variant={exportFormat === 'excel' ? 'primary' : 'outline'}
                                                leftIcon={<FiFile />}
                                                onClick={() => setExportFormat('excel')}
                                                flex="1"
                                            >
                                                Excel
                                            </Button>
                                        </HStack>
                                    </VStack>
                                </CardBody>
                            </Card>

                            {/* Campos */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <HStack justify="space-between">
                                            <Heading size="md">Campos para Exportar</Heading>
                                            <Badge colorScheme="blue">
                                                {selectedFields.length} selecionados
                                            </Badge>
                                        </HStack>
                                        
                                        <CheckboxGroup
                                            value={selectedFields}
                                            onChange={setSelectedFields}
                                        >
                                            <Stack spacing={3}>
                                                {availableFields.map(field => (
                                                    <Checkbox
                                                        key={field.value}
                                                        value={field.value}
                                                        colorScheme="blue"
                                                    >
                                                        {field.label}
                                                    </Checkbox>
                                                ))}
                                            </Stack>
                                        </CheckboxGroup>
                                    </VStack>
                                </CardBody>
                            </Card>
                        </VStack>
                    </GridItem>

                    {/* Preview/Stats */}
                    <GridItem>
                        <VStack spacing={6} align="stretch">
                            {/* Estatísticas */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4} align="stretch">
                                        <Heading size="md">Resumo da Exportação</Heading>

                                        <HStack justify="space-between">
                                            <Text color="gray.600">Total de registros:</Text>
                                            <Badge colorScheme="blue" p={2}>
                                                {pessoas.length}
                                            </Badge>
                                        </HStack>
                                        
                                        <HStack justify="space-between">
                                            <Text color="gray.600">Campos selecionados:</Text>
                                            <Badge colorScheme="green" p={2}>
                                                {selectedFields.length}
                                            </Badge>
                                        </HStack>
                                        
                                        <HStack justify="space-between">
                                            <Text color="gray.600">Formato:</Text>
                                            <Badge colorScheme="purple" p={2}>
                                                {exportFormat.toUpperCase()}
                                            </Badge>
                                        </HStack>
                                    </VStack>
                                </CardBody>
                            </Card>

                            {/* Botão de Exportar */}
                            <Card>
                                <CardBody p={6}>
                                    <VStack spacing={4}>
                                        <Button
                                            onClick={handleExport}
                                            variant="primary"
                                            size="lg"
                                            leftIcon={<FiDownload />}
                                            isLoading={isLoading}
                                            loadingText="Exportando..."
                                            w="full"
                                            isDisabled={selectedFields.length === 0 || pessoas.length === 0}
                                        >
                                            Exportar {exportFormat.toUpperCase()}
                                        </Button>

                                        {isLoading && (
                                            <Box w="full">
                                                <Text fontSize="sm" color="gray.600" mb={2}>
                                                    Gerando arquivo...
                                                </Text>
                                                <Progress
                                                    size="sm"
                                                    isIndeterminate
                                                    colorScheme="blue"
                                                    borderRadius="full"
                                                />
                                            </Box>
                                        )}
                                    </VStack>
                                </CardBody>
                            </Card>
                        </VStack>
                    </GridItem>
                </Grid>

                {/* Info */}
                <Alert status="info" borderRadius="xl">
                    <AlertIcon />
                    <Box>
                        <Text fontWeight="600" mb={2}>Informações:</Text>
                        <Text fontSize="sm">
                            • PDF: Ideal para visualização e impressão com layout profissional<br />
                            • Excel: Permite edição e análise dos dados com formatação avançada<br />
                            • Selecione os campos que deseja incluir na exportação<br />
                            • O arquivo será baixado automaticamente no seu navegador
                        </Text>
                    </Box>
                </Alert>
            </VStack>
        </Box>
    );
};

export default ExportarDados;