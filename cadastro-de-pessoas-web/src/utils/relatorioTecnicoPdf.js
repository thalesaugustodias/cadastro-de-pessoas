import jsPDF from 'jspdf';

const COLORS = {
    primary: [30, 119, 243],
    accent: [38, 236, 233],
    dark: [30, 30, 45],
    gray: [100, 100, 110],
    lightGray: [240, 241, 245],
    white: [255, 255, 255],
    sectionBg: [245, 248, 255],
    borderBlue: [180, 210, 250],
};

const PAGE_W = 210;
const PAGE_H = 297;
const MARGIN_L = 18;
const MARGIN_R = 18;
const CONTENT_W = PAGE_W - MARGIN_L - MARGIN_R;

function addWrappedText(doc, text, x, y, maxWidth, lineHeight, options = {}) {
    const lines = doc.splitTextToSize(text, maxWidth);
    lines.forEach((line) => {
        if (options.align === 'center') {
            doc.text(line, x, y, { align: 'center' });
        } else {
            doc.text(line, x, y);
        }
        y += lineHeight;
    });
    return y;
}

function drawHRule(doc, y, color = COLORS.lightGray) {
    doc.setDrawColor(...color);
    doc.setLineWidth(0.3);
    doc.line(MARGIN_L, y, PAGE_W - MARGIN_R, y);
    return y + 4;
}

function addPageIfNeeded(doc, y, needed = 20) {
    if (y + needed > PAGE_H - 18) {
        doc.addPage();
        return 18;
    }
    return y;
}

export function gerarRelatorioTecnicoPdf() {
    const doc = new jsPDF({ unit: 'mm', format: 'a4', orientation: 'portrait' });

    // ── Cover gradient-like header ──────────────────────────────────────────
    doc.setFillColor(...COLORS.primary);
    doc.rect(0, 0, PAGE_W, 38, 'F');
    doc.setFillColor(...COLORS.accent);
    doc.rect(0, 34, PAGE_W, 4, 'F');

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(15);
    doc.setTextColor(...COLORS.white);
    doc.text('Relatório Técnico', MARGIN_L, 15);

    doc.setFontSize(10);
    doc.setFont('helvetica', 'normal');
    doc.text('Ajuste nos Fluxos de Disputa da Timeline', MARGIN_L, 23);

    // metadata strip
    doc.setFontSize(8);
    doc.setTextColor(200, 225, 255);
    const meta = [
        'Commit: d758978',
        'Branch: feature/SDA-56-ajustes-de-timeline',
        'Data: 09 de Março de 2026',
    ];
    doc.text(meta.join('   |   '), MARGIN_L, 31);

    // ── Author badge ────────────────────────────────────────────────────────
    doc.setFillColor(...COLORS.sectionBg);
    doc.roundedRect(MARGIN_L, 42, CONTENT_W, 11, 2, 2, 'F');
    doc.setDrawColor(...COLORS.borderBlue);
    doc.setLineWidth(0.4);
    doc.roundedRect(MARGIN_L, 42, CONTENT_W, 11, 2, 2, 'S');
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(8.5);
    doc.setTextColor(...COLORS.primary);
    doc.text('Autor:', MARGIN_L + 4, 49.5);
    doc.setFont('helvetica', 'normal');
    doc.setTextColor(...COLORS.dark);
    doc.text('Thales Augusto De Lima Dias  ·  10-03-2026', MARGIN_L + 18, 49.5);

    // ── Files altered ────────────────────────────────────────────────────────
    let y = 61;
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(8.5);
    doc.setTextColor(...COLORS.gray);
    doc.text('ARQUIVOS ALTERADOS', MARGIN_L, y);
    y += 5;

    const files = [
        'TimelineWorkflow.cs',
        'CustomerServiceTimelineBuilder.cs',
        'CustomerServiceTimelineBuilderTests.cs',
    ];
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(8.5);
    doc.setTextColor(...COLORS.dark);
    files.forEach((f) => {
        doc.setFillColor(...COLORS.lightGray);
        doc.roundedRect(MARGIN_L, y - 3.5, CONTENT_W, 6, 1.5, 1.5, 'F');
        doc.text(`• ${f}`, MARGIN_L + 4, y);
        y += 7;
    });

    y = drawHRule(doc, y + 1, COLORS.borderBlue);

    // ── Helper: Section heading ──────────────────────────────────────────────
    const sectionHeading = (title, cy) => {
        cy = addPageIfNeeded(doc, cy, 16);
        doc.setFillColor(...COLORS.primary);
        doc.roundedRect(MARGIN_L, cy, CONTENT_W, 8, 2, 2, 'F');
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(10);
        doc.setTextColor(...COLORS.white);
        doc.text(title, MARGIN_L + 4, cy + 5.5);
        return cy + 12;
    };

    const subHeading = (title, cy) => {
        cy = addPageIfNeeded(doc, cy, 14);
        doc.setFillColor(...COLORS.sectionBg);
        doc.roundedRect(MARGIN_L, cy, CONTENT_W, 7, 1.5, 1.5, 'F');
        doc.setDrawColor(...COLORS.borderBlue);
        doc.setLineWidth(0.3);
        doc.line(MARGIN_L, cy, MARGIN_L, cy + 7);
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(9);
        doc.setTextColor(...COLORS.primary);
        doc.text(title, MARGIN_L + 5, cy + 4.8);
        return cy + 11;
    };

    const bodyText = (text, cy, indent = 0) => {
        cy = addPageIfNeeded(doc, cy, 10);
        doc.setFont('helvetica', 'normal');
        doc.setFontSize(9);
        doc.setTextColor(...COLORS.dark);
        const lines = doc.splitTextToSize(text, CONTENT_W - indent);
        lines.forEach((line) => {
            cy = addPageIfNeeded(doc, cy, 6);
            doc.text(line, MARGIN_L + indent, cy);
            cy += 5.5;
        });
        return cy + 1;
    };

    const bulletItem = (label, text, cy) => {
        cy = addPageIfNeeded(doc, cy, 10);
        doc.setFillColor(...COLORS.accent);
        doc.circle(MARGIN_L + 3.5, cy - 1, 1, 'F');
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(9);
        doc.setTextColor(...COLORS.primary);
        doc.text(label, MARGIN_L + 7, cy);
        if (text) {
            doc.setFont('helvetica', 'normal');
            doc.setTextColor(...COLORS.dark);
            const labelWidth = doc.getTextWidth(label) + 8 + MARGIN_L;
            const remaining = PAGE_W - MARGIN_R - labelWidth;
            if (remaining > 20) {
                const lines = doc.splitTextToSize(text, remaining - 2);
                doc.text(lines[0], labelWidth + 2, cy);
                cy += 5.5;
                for (let i = 1; i < lines.length; i++) {
                    cy = addPageIfNeeded(doc, cy, 6);
                    doc.text(lines[i], MARGIN_L + 7, cy);
                    cy += 5.5;
                }
            } else {
                cy += 5.5;
                cy = bodyText(text, cy, 7);
            }
        } else {
            cy += 6;
        }
        return cy;
    };

    // ══════════════════════════════════════════════════════════════════════════
    // SECTION 1 — Contexto
    // ══════════════════════════════════════════════════════════════════════════
    y = sectionHeading('1. Contexto', y);
    y = bodyText(
        'Com a arquitetura de timeline entregue no SDA-1490 (substituição do CustomerServiceTimelineProvider ' +
        'pelo modelo declarativo TimelineWorkflow + CustomerServiceTimelineBuilder), a estrutura geral da ' +
        'timeline passou a funcionar corretamente para a maioria dos estágios. No entanto, dois cenários de ' +
        'fluxo de disputa específicos apresentavam etapas que não apareciam na timeline mesmo após os eventos ' +
        'corretos serem registrados.',
        y,
    );
    y = bodyText(
        'Não se trata de uma regressão causada por alguma mudança recente — os passos em questão simplesmente ' +
        'nunca foram incluídos na definição dos workflows correspondentes.',
        y,
    );

    y += 2;

    // ══════════════════════════════════════════════════════════════════════════
    // SECTION 2 — Dois Cenários Distintos
    // ══════════════════════════════════════════════════════════════════════════
    y = sectionHeading('2. Dois Cenários Distintos', y);

    // 2.1
    y = subHeading('Cenário 1 — Disputa de Reembolso após Rejeição de Proposta Parcial', y);
    y = bodyText(
        'Fluxo: devolução aprovada → reembolso parcial proposto pelo seller → cliente recusa a proposta → ' +
        'atendimento entra em estado InRefundDispute.',
        y,
    );
    y = bodyText(
        'O passo esperado na timeline era "Reembolso parcial recusado" (PartialRefundRejected), posicionado ' +
        'entre o estágio de disputa (InRefundDispute) e a entrada de reembolso negado (RefundDenied). Esse ' +
        'passo existe no EventToStepMap — o evento CustomerRejectedPartialRefundProposal está corretamente ' +
        'mapeado para PartialRefundRejected — mas o step não estava declarado na lista de passos do workflow ' +
        'InRefundDispute.',
        y,
    );
    y = bodyText(
        'Como consequência, mesmo que o evento fosse registrado, o passo era simplesmente ignorado na ' +
        'montagem da timeline.',
        y,
    );
    y = bulletItem(
        'Correção:',
        'PartialRefundRejected foi incluído na lista de steps do workflow InRefundDispute, na posição ' +
        'correta entre InRefundDispute e RefundDenied.',
        y,
    );

    y += 3;

    // 2.2
    y = subHeading('Cenário 2 — Disputa de Devolução após Negação com Proposta de Reembolso Parcial', y);
    y = bodyText(
        'Fluxo: devolução negada com proposta de reembolso parcial pelo seller → cliente contesta → ' +
        'atendimento entra em estado InReturnDispute.',
        y,
    );
    y = bodyText(
        'Nesse fluxo, o passo "Reembolso parcial oferecido" (PartialRefundProposed) precisava aparecer na ' +
        'timeline para indicar que, antes da disputa, havia uma proposta de reembolso parcial em aberto. ' +
        'O evento ReturnDeniedWithPartialRefundProposal está mapeado corretamente para PartialRefundProposed. ' +
        'O problema era duplo:',
        y,
    );
    y = bulletItem('1.', 'PartialRefundProposed não estava na lista de steps do workflow InReturnDispute.', y);
    y = bulletItem(
        '2.',
        'O passo corrente do estágio (InReturnDispute) não tinha evento registrado diretamente. O ' +
        'CustomerServiceTimelineBuilder filtrava passos sem evento registrado em cenários de disputa, ' +
        'fazendo o passo corrente sumir da timeline.',
        y,
    );
    y = bulletItem(
        'Correção:',
        'PartialRefundProposed foi inserido no workflow InReturnDispute. Foi adicionado o flag ' +
        'currentStepAlwaysVisible nos estágios InRefundDispute e InReturnDispute no ' +
        'CustomerServiceTimelineBuilder, garantindo que o passo corrente seja sempre exibido ' +
        'independentemente de haver evento registrado para ele.',
        y,
    );

    y += 2;

    // ══════════════════════════════════════════════════════════════════════════
    // SECTION 3 — Causa-Raiz
    // ══════════════════════════════════════════════════════════════════════════
    y = addPageIfNeeded(doc, y, 50);
    y = sectionHeading('3. Causa-Raiz', y);
    y = bodyText(
        'Ambos os problemas têm a mesma origem: a definição dos workflows em TimelineWorkflow.cs para os ' +
        'estágios de disputa estava incompleta. Os steps existiam no enum TimelineStep e os mappings ' +
        'existiam no EventToStepMap, mas eles não foram incluídos nas listas de passos dos respectivos ' +
        'workflows quando os estágios foram inicialmente definidos.',
        y,
    );
    y = bodyText(
        'O segundo problema (passo corrente oculto) é uma consequência do comportamento de filtragem do ' +
        'CustomerServiceTimelineBuilder, que por padrão omite passos sem eventos ocorridos — comportamento ' +
        'correto para a maioria dos estágios, mas que precisava de exceção explícita para os estágios de ' +
        'disputa, onde o passo corrente representa o estado agregado, não uma ação pontual.',
        y,
    );

    y += 2;

    // ══════════════════════════════════════════════════════════════════════════
    // SECTION 4 — Arquivos Alterados
    // ══════════════════════════════════════════════════════════════════════════
    y = addPageIfNeeded(doc, y, 80);
    y = sectionHeading('4. Arquivos Alterados', y);

    y = subHeading('FCXLABS.CustomerService.Domain/Models/TimelineWorkflow.cs', y);
    y = bodyText('Adicionados dois steps às listas de seus respectivos workflows:', y);
    y = bulletItem('•', 'PartialRefundRejected ao workflow do estágio InRefundDispute.', y);
    y = bulletItem('•', 'PartialRefundProposed ao workflow do estágio InReturnDispute.', y);

    y += 2;
    y = subHeading('FCXLABS.CustomerService.Application/Services/CustomerServiceTimelineBuilder.cs', y);
    y = bodyText(
        'Adicionada a verificação currentStepAlwaysVisible para os estágios InRefundDispute e ' +
        'InReturnDispute. A condição de filtro passou a respeitar esse flag, garantindo que o passo ' +
        'corrente seja sempre incluído na timeline desses estágios mesmo na ausência de evento registrado.',
        y,
    );

    y += 2;
    y = subHeading('FCXLABS.CustomerService.Tests/Services/CustomerServiceTimelineBuilderTests.cs', y);
    y = bodyText('Adicionados dois testes unitários cobrindo os novos cenários:', y);
    y = bulletItem(
        '•',
        'InRefundDispute_AfterApprovedReturnAndRejectedPartialRefund_Should_Include_PartialRefundRejected — ' +
        'verifica que a timeline em disputa de reembolso contém o passo de rejeição da proposta parcial e ' +
        'não exibe prematuramente o passo de reembolso negado.',
        y,
    );
    y += 1;
    y = bulletItem(
        '•',
        'InReturnDispute_AfterDeniedReturnWithPartialRefund_Should_Include_PartialRefundProposed — ' +
        'verifica que a timeline em disputa de devolução exibe o passo de proposta de reembolso parcial ' +
        'e não exibe o passo de devolução negada simples.',
        y,
    );

    y += 2;

    // ══════════════════════════════════════════════════════════════════════════
    // SECTION 5 — Impacto
    // ══════════════════════════════════════════════════════════════════════════
    y = addPageIfNeeded(doc, y, 45);
    y = sectionHeading('5. Impacto', y);
    y = bulletItem(
        '•',
        'Nenhuma lógica de negócio foi alterada. Os handlers responsáveis por registrar os eventos ' +
        '(RejectRefundProposalHandler, RejectReturnDenialHandler) já estavam corretos.',
        y,
    );
    y = bulletItem(
        '•',
        'A correção é restrita à definição dos workflows e à lógica de exibição do builder.',
        y,
    );
    y = bulletItem('•', '283/283 testes unitários passando após o ajuste.', y);

    // ── Footer on every page ─────────────────────────────────────────────────
    const totalPages = doc.getNumberOfPages();
    for (let p = 1; p <= totalPages; p++) {
        doc.setPage(p);
        doc.setFillColor(...COLORS.primary);
        doc.rect(0, PAGE_H - 12, PAGE_W, 12, 'F');
        doc.setFont('helvetica', 'normal');
        doc.setFontSize(7.5);
        doc.setTextColor(...COLORS.white);
        doc.text(
            'Thales Augusto De Lima Dias  ·  10-03-2026  ·  Relatório Técnico — SDA-56',
            MARGIN_L,
            PAGE_H - 5.5,
        );
        doc.text(`${p} / ${totalPages}`, PAGE_W - MARGIN_R, PAGE_H - 5.5, { align: 'right' });
    }

    doc.save('relatorio-tecnico-sda-56.pdf');
}
