// Script para converter ícones do Remix para ícones do Chakra UI
// Mapeamento de ícones Remix para ícones Chakra UI equivalentes
const iconMapping = {
  RiTimeLine: 'TimeIcon',
  RiPulseLine: 'InfoIcon',
  RiCalendarLine: 'CalendarIcon',
  RiInformationLine: 'InfoOutlineIcon',
  RiUserAddLine: 'AddIcon',
  RiFileAddLine: 'InfoIcon',
  RiPieChartLine: 'InfoOutlineIcon',
  RiAlertLine: 'WarningIcon',
  RiRefreshLine: 'RepeatIcon',
  RiArrowUpLine: 'ArrowUpIcon',
  RiUploadLine: 'UploadIcon',
  RiDownloadLine: 'DownloadIcon',
  RiCheckboxCircleLine: 'CheckCircleIcon'
};

// Exemplo de como substituir no código:
// 1. Encontrar todas as instâncias de ícones Remix (RiXxxLine)
// 2. Substituir pelo equivalente do Chakra UI
// 3. Certificar-se de que todos os ícones necessários estão importados do @chakra-ui/icons

/*
Exemplo de substituição:

Original:
<Icon as={RiTimeLine} w="100%" h="100%" />

Substituído:
<Icon as={TimeIcon} w="100%" h="100%" />
*/
