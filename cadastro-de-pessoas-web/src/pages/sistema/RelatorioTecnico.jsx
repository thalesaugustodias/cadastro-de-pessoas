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
    Badge,
    Divider,
    Icon,
    List,
    ListItem,
    ListIcon,
    Alert,
    AlertIcon,
} from '@chakra-ui/react';
import { FiDownload, FiFileText, FiCheckCircle } from 'react-icons/fi';
import { gerarRelatorioTecnicoPdf } from '../../utils/relatorioTecnicoPdf';
import { useNotification } from '../../hooks/useNotification';

const MetaBadge = ({ label, value }) => (
    <HStack>
        <Text fontSize="sm" color="gray.500" fontWeight="600" minW="80px">{label}:</Text>
        <Badge colorScheme="blue" px={2} py={0.5} borderRadius="md" fontFamily="mono" fontSize="xs">
            {value}
        </Badge>
    </HStack>
);

const SectionBlock = ({ title, children }) => (
    <Box>
        <HStack mb={3}>
            <Box w="3px" h="20px" bg="linear-gradient(180deg, #1e77f3 0%, #26ece9 100%)" borderRadius="full" />
            <Heading size="sm" color="gray.800">{title}</Heading>
        </HStack>
        <Box pl={4}>{children}</Box>
    </Box>
);

const RelatorioTecnico = () => {
    const [isGenerating, setIsGenerating] = useState(false);
    const { showSuccess, showError } = useNotification();

    const handleDownload = async () => {
        setIsGenerating(true);
        try {
            await new Promise(resolve => setTimeout(resolve, 300));
            gerarRelatorioTecnicoPdf();
            showSuccess('PDF gerado e baixado com sucesso!');
        } catch (err) {
            console.error('Erro ao gerar PDF:', err);
            showError('Erro ao gerar o PDF. Tente novamente.');
        } finally {
            setIsGenerating(false);
        }
    };

    return (
        <Box p={8}>
            <VStack spacing={8} align="stretch">
                {/* Header */}
                <Box>
                    <HStack mb={2} spacing={3}>
                        <Icon as={FiFileText} boxSize={6} color="blue.500" />
                        <Heading size="lg" color="gray.800">
                            Relatório Técnico
                        </Heading>
                    </HStack>
                    <Text color="gray.600">
                        Ajuste nos Fluxos de Disputa da Timeline
                    </Text>
                </Box>

                {/* Metadata card */}
                <Card>
                    <CardBody p={6}>
                        <VStack align="stretch" spacing={3}>
                            <Heading size="sm" color="gray.700" mb={1}>Informações do Commit</Heading>
                            <MetaBadge label="Commit" value="d758978" />
                            <MetaBadge label="Branch" value="feature/SDA-56-ajustes-de-timeline" />
                            <MetaBadge label="Data" value="09 de Março de 2026" />
                            <Divider />
                            <Heading size="sm" color="gray.700" mt={1} mb={1}>Arquivos Alterados</Heading>
                            <List spacing={1}>
                                {[
                                    'TimelineWorkflow.cs',
                                    'CustomerServiceTimelineBuilder.cs',
                                    'CustomerServiceTimelineBuilderTests.cs',
                                ].map(f => (
                                    <ListItem key={f} fontSize="sm" color="gray.600">
                                        <ListIcon as={FiCheckCircle} color="blue.400" />
                                        {f}
                                    </ListItem>
                                ))}
                            </List>
                            <Divider />
                            <HStack>
                                <Text fontSize="sm" color="gray.500" fontWeight="600">Autor:</Text>
                                <Text fontSize="sm" color="gray.700">Thales Augusto De Lima Dias</Text>
                                <Badge colorScheme="green" fontSize="xs">10-03-2026</Badge>
                            </HStack>
                        </VStack>
                    </CardBody>
                </Card>

                {/* Report preview */}
                <Card>
                    <CardBody p={6}>
                        <VStack align="stretch" spacing={6}>
                            <Heading size="sm" color="gray.700">Conteúdo do Relatório</Heading>

                            <SectionBlock title="Contexto">
                                <Text fontSize="sm" color="gray.600">
                                    Com a arquitetura de timeline entregue no SDA-1490, dois cenários de fluxo de
                                    disputa apresentavam etapas que não apareciam na timeline. Os passos existiam no
                                    enum <Badge colorScheme="purple" fontSize="xs">TimelineStep</Badge> e nos
                                    mappings, mas não foram incluídos nas definições dos workflows correspondentes.
                                </Text>
                            </SectionBlock>

                            <Divider />

                            <SectionBlock title="Cenário 1 — Disputa de Reembolso">
                                <Text fontSize="sm" color="gray.600" mb={2}>
                                    Fluxo: devolução aprovada → reembolso parcial proposto → cliente recusa →{' '}
                                    <Badge colorScheme="orange" fontSize="xs">InRefundDispute</Badge>
                                </Text>
                                <Alert status="success" borderRadius="lg" py={2}>
                                    <AlertIcon />
                                    <Text fontSize="sm">
                                        <strong>PartialRefundRejected</strong> adicionado ao workflow{' '}
                                        <strong>InRefundDispute</strong>.
                                    </Text>
                                </Alert>
                            </SectionBlock>

                            <Divider />

                            <SectionBlock title="Cenário 2 — Disputa de Devolução">
                                <Text fontSize="sm" color="gray.600" mb={2}>
                                    Fluxo: devolução negada com proposta de reembolso → cliente contesta →{' '}
                                    <Badge colorScheme="orange" fontSize="xs">InReturnDispute</Badge>
                                </Text>
                                <Alert status="success" borderRadius="lg" py={2}>
                                    <AlertIcon />
                                    <Text fontSize="sm">
                                        <strong>PartialRefundProposed</strong> adicionado ao workflow{' '}
                                        <strong>InReturnDispute</strong>. Flag{' '}
                                        <Badge colorScheme="purple" fontSize="xs">currentStepAlwaysVisible</Badge>{' '}
                                        adicionado nos dois estágios de disputa.
                                    </Text>
                                </Alert>
                            </SectionBlock>

                            <Divider />

                            <SectionBlock title="Impacto">
                                <List spacing={1}>
                                    {[
                                        'Nenhuma lógica de negócio alterada.',
                                        'Correção restrita a TimelineWorkflow.cs e CustomerServiceTimelineBuilder.cs.',
                                        '283/283 testes unitários passando após o ajuste.',
                                    ].map(item => (
                                        <ListItem key={item} fontSize="sm" color="gray.600">
                                            <ListIcon as={FiCheckCircle} color="green.400" />
                                            {item}
                                        </ListItem>
                                    ))}
                                </List>
                            </SectionBlock>
                        </VStack>
                    </CardBody>
                </Card>

                {/* Download button */}
                <Card>
                    <CardBody p={6}>
                        <VStack spacing={4} align="stretch">
                            <HStack justify="space-between" align="center">
                                <Box>
                                    <Heading size="sm" color="gray.700" mb={1}>Baixar Relatório em PDF</Heading>
                                    <Text fontSize="sm" color="gray.500">
                                        Documento formatado pronto para impressão e arquivo.
                                    </Text>
                                </Box>
                                <Badge colorScheme="blue" p={2} borderRadius="lg" fontSize="xs">
                                    PDF · A4
                                </Badge>
                            </HStack>
                            <Button
                                onClick={handleDownload}
                                variant="primary"
                                size="lg"
                                leftIcon={<FiDownload />}
                                isLoading={isGenerating}
                                loadingText="Gerando PDF..."
                                w="full"
                            >
                                Baixar PDF
                            </Button>
                        </VStack>
                    </CardBody>
                </Card>
            </VStack>
        </Box>
    );
};

export default RelatorioTecnico;
