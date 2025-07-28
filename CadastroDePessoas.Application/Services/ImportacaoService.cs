using System.Globalization;
using CadastroDePessoas.Application.CQRS.Comandos.Pessoa.CriarPessoa;
using CadastroDePessoas.Application.DTOs;
using CadastroDePessoas.Domain.Enums;
using CadastroDePessoas.Domain.Interfaces;
using CadastroDePessoas.Domain.Validacoes;
using ClosedXML.Excel;
using MediatR;

namespace CadastroDePessoas.Application.Services
{
    public class ImportacaoService(IMediator mediator, IRepositorioPessoa repositorioPessoa)
    {
        private static readonly string[] DateFormats = { 
            "yyyy-MM-dd", "dd/MM/yyyy", "yyyy/MM/dd", "MM/dd/yyyy", 
            "dd-MM-yyyy", "yyyy.MM.dd", "dd.MM.yyyy" 
        };

        public async Task<ImportacaoResultado> ImportarExcel(Stream excelStream, bool importarParcialmente = true)
        {
            var resultado = new ImportacaoResultado
            {
                Total = 0,
                Sucesso = 0,
                Erros = 0,
                Detalhes = []
            };

            if (excelStream.CanSeek)
            {
                excelStream.Seek(0, SeekOrigin.Begin);
            }

            List<Dictionary<string, string>> registros = new();
            try
            {
                using var workbook = new XLWorkbook(excelStream);
                var worksheet = workbook.Worksheets.FirstOrDefault() ?? 
                    throw new Exception("Arquivo Excel não contém planilhas");
                
                var cabecalhos = new List<string>();
                var primeiraLinha = worksheet.Row(1);
                
                foreach (var cell in primeiraLinha.Cells())
                {
                    if (!string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        cabecalhos.Add(cell.Value.ToString().Trim());
                    }
                }
                
                if (cabecalhos.Count == 0)
                {
                    throw new Exception("Cabeçalhos não encontrados na planilha");
                }
                
                var linhaInicial = 2;
                var ultimaLinha = worksheet.LastRowUsed().RowNumber();
                
                for (int i = linhaInicial; i <= ultimaLinha; i++)
                {
                    var xlRow = worksheet.Row(i);
                    if (xlRow.IsEmpty()) continue;
                    
                    var registro = new Dictionary<string, string>();
                    
                    for (int j = 0; j < cabecalhos.Count; j++)
                    {
                        var coluna = j + 1;
                        var valor = xlRow.Cell(coluna).Value.ToString().Trim();
                        registro[cabecalhos[j]] = valor;
                    }
                    
                    registros.Add(registro);
                }
                
                resultado.Total = registros.Count;
            }
            catch (Exception ex)
            {
                resultado.Erros = 1;
                resultado.Detalhes.Add(new DetalheErro
                {
                    Linha = 0,
                    Mensagem = $"Erro ao ler o arquivo Excel: {ex.Message}"
                });
                return resultado;
            }

            if (!importarParcialmente)
            {
                var resultadoValidacao = await ValidarTodosOsRegistros(registros);
                if (resultadoValidacao.Erros > 0)
                {
                    return resultadoValidacao;
                }
            }

            var cpfsProcessados = new HashSet<string>();

            int linhaProcessada = 1;
            foreach (var registro in registros)
            {
                linhaProcessada++;
                
                try
                {
                    string nome = GetValor(registro, "Nome");
                    string email = GetValor(registro, "Email");
                    string cpf = GetValor(registro, "CPF");
                    string dataNascimentoStr = GetValor(registro, "DataNascimento");
                    string telefone = GetValor(registro, "Telefone");
                    string sexoStr = GetValor(registro, "Sexo");
                    string naturalidade = GetValor(registro, "Naturalidade");
                    string nacionalidade = GetValor(registro, "Nacionalidade");
                    
                    string cep = GetValor(registro, "CEP");
                    string logradouro = GetValor(registro, "Logradouro");
                    string numero = GetValor(registro, "Numero");
                    string complemento = GetValor(registro, "Complemento");
                    string bairro = GetValor(registro, "Bairro");
                    string cidade = GetValor(registro, "Cidade");
                    string estado = GetValor(registro, "Estado");
                    
                    if (string.IsNullOrWhiteSpace(nome))
                    {
                        AdicionarErro(resultado, linhaProcessada, "Nome é obrigatório", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    if (string.IsNullOrWhiteSpace(cpf))
                    {
                        AdicionarErro(resultado, linhaProcessada, "CPF é obrigatório", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    if (!ValidadorCPF.Validar(cpf))
                    {
                        AdicionarErro(resultado, linhaProcessada, $"CPF inválido: '{cpf}'", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    if (await repositorioPessoa.CpfExisteAsync(cpf))
                    {
                        AdicionarErro(resultado, linhaProcessada, $"CPF já cadastrado: {cpf}", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    if (cpfsProcessados.Contains(cpf))
                    {
                        AdicionarErro(resultado, linhaProcessada, $"CPF duplicado no arquivo: {cpf}", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    if (string.IsNullOrWhiteSpace(dataNascimentoStr))
                    {
                        AdicionarErro(resultado, linhaProcessada, "Data de nascimento é obrigatória", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    DateTime dataNascimento;
                    if (!TryParseDate(dataNascimentoStr, out dataNascimento))
                    {
                        AdicionarErro(resultado, linhaProcessada, $"Data de nascimento inválida: '{dataNascimentoStr}'", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
                    {
                        AdicionarErro(resultado, linhaProcessada, $"Email inválido: '{email}'", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    Sexo? sexo = null;
                    if (!string.IsNullOrEmpty(sexoStr) && Enum.TryParse<Sexo>(sexoStr, out var sexoValue))
                    {
                        sexo = sexoValue;
                    }
                    else if (!string.IsNullOrEmpty(sexoStr))
                    {
                        AdicionarErro(resultado, linhaProcessada, $"Valor de sexo inválido: '{sexoStr}'", registro);
                        if (!importarParcialmente) break;
                        continue;
                    }
                    
                    try 
                    {
                        var comando = new CriarPessoaComando
                        {
                            Nome = nome,
                            Email = email,
                            CPF = cpf,
                            Sexo = sexo,
                            DataNascimento = dataNascimento,
                            Telefone = telefone,
                            Naturalidade = naturalidade,
                            Nacionalidade = nacionalidade,
                            Endereco = new EnderecoComando
                            {
                                CEP = cep,
                                Logradouro = logradouro,
                                Numero = numero,
                                Complemento = complemento,
                                Bairro = bairro,
                                Cidade = cidade,
                                Estado = estado
                            }
                        };

                        await mediator.Send(comando);
                        
                        cpfsProcessados.Add(cpf);
                        
                        resultado.Sucesso++;
                    }
                    catch (Exception ex)
                    {
                        AdicionarErro(resultado, linhaProcessada, $"Erro ao criar pessoa: {ex.Message}", registro);
                        if (!importarParcialmente) break;
                    }
                }
                catch (Exception ex)
                {
                    AdicionarErro(resultado, linhaProcessada, $"Erro inesperado: {ex.Message}", registro);
                    if (!importarParcialmente) break;
                }
            }

            return resultado;
        }
        
        public async Task<ImportacaoResultado> ImportarCsv(Stream csvStream, bool importarParcialmente = true)
        {
            using var excelStream = new MemoryStream();
            await ConvertCsvToExcel(csvStream, excelStream);
            
            return await ImportarExcel(excelStream, importarParcialmente);
        }
        
        private async Task ConvertCsvToExcel(Stream csvStream, Stream excelStream)
        {
            if (csvStream.CanSeek)
            {
                csvStream.Seek(0, SeekOrigin.Begin);
            }
            
            using var reader = new StreamReader(csvStream);
            string csvContent = await reader.ReadToEndAsync();
            string[] lines = csvContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            
            if (lines.Length == 0)
            {
                throw new Exception("Arquivo CSV vazio");
            }
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Importação");
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                string[] values = SplitCsvLine(line);
                
                for (int j = 0; j < values.Length; j++)
                {
                    worksheet.Cell(i + 1, j + 1).Value = values[j];
                }
            }
            
            workbook.SaveAs(excelStream);
            
            if (excelStream.CanSeek)
            {
                excelStream.Seek(0, SeekOrigin.Begin);
            }
        }
        
        private static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var currentValue = new System.Text.StringBuilder();
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentValue.ToString().Trim());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }
            
            result.Add(currentValue.ToString().Trim());
            
            return result.ToArray();
        }
        
        private async Task<ImportacaoResultado> ValidarTodosOsRegistros(List<Dictionary<string, string>> registros)
        {
            var resultado = new ImportacaoResultado
            {
                Total = registros.Count,
                Sucesso = 0,
                Erros = 0,
                Detalhes = []
            };
            
            var cpfsProcessados = new HashSet<string>();
            
            int linhaValidacao = 1;
            foreach (var registro in registros)
            {
                linhaValidacao++;
                
                try
                {
                    string nome = GetValor(registro, "Nome");
                    string email = GetValor(registro, "Email");
                    string cpf = GetValor(registro, "CPF");
                    string dataNascimentoStr = GetValor(registro, "DataNascimento");
                    string sexoStr = GetValor(registro, "Sexo");
                    
                    if (string.IsNullOrWhiteSpace(nome))
                    {
                        AdicionarErro(resultado, linhaValidacao, "Nome é obrigatório", registro);
                        continue;
                    }
                    
                    if (string.IsNullOrWhiteSpace(cpf))
                    {
                        AdicionarErro(resultado, linhaValidacao, "CPF é obrigatório", registro);
                        continue;
                    }
                    
                    if (!ValidadorCPF.Validar(cpf))
                    {
                        AdicionarErro(resultado, linhaValidacao, $"CPF inválido: '{cpf}'", registro);
                        continue;
                    }
                    
                    if (await repositorioPessoa.CpfExisteAsync(cpf))
                    {
                        AdicionarErro(resultado, linhaValidacao, $"CPF já cadastrado: {cpf}", registro);
                        continue;
                    }
                    
                    if (!cpfsProcessados.Add(cpf))
                    {
                        AdicionarErro(resultado, linhaValidacao, $"CPF duplicado no arquivo: {cpf}", registro);
                        continue;
                    }
                    
                    if (string.IsNullOrWhiteSpace(dataNascimentoStr))
                    {
                        AdicionarErro(resultado, linhaValidacao, "Data de nascimento é obrigatória", registro);
                        continue;
                    }
                    
                    if (!TryParseDate(dataNascimentoStr, out _))
                    {
                        AdicionarErro(resultado, linhaValidacao, $"Data de nascimento inválida: '{dataNascimentoStr}'", registro);
                        continue;
                    }
                    
                    if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
                    {
                        AdicionarErro(resultado, linhaValidacao, $"Email inválido: '{email}'", registro);
                        continue;
                    }
                    
                    if (!string.IsNullOrEmpty(sexoStr) && !Enum.TryParse<Sexo>(sexoStr, out _))
                    {
                        AdicionarErro(resultado, linhaValidacao, $"Valor de sexo inválido: '{sexoStr}'", registro);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    AdicionarErro(resultado, linhaValidacao, $"Erro inesperado: {ex.Message}", registro);
                }
            }
            
            resultado.Sucesso = resultado.Total - resultado.Erros;
            return resultado;
        }
        
        private static void AdicionarErro(ImportacaoResultado resultado, int linha, string mensagem, Dictionary<string, string> valoresOriginais)
        {
            resultado.Erros++;
            resultado.Detalhes.Add(new DetalheErro
            {
                Linha = linha,
                Mensagem = mensagem,
                ValoresOriginais = valoresOriginais
            });
        }
        
        private static bool TryParseDate(string dateString, out DateTime result)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                result = DateTime.MinValue;
                return false;
            }
            
            dateString = dateString.Trim();
            
            return DateTime.TryParseExact(
                dateString, 
                DateFormats, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out result);
        }
        
        private static string GetValor(Dictionary<string, string> registro, string campo)
        {
            if (registro.TryGetValue(campo, out string valor))
            {
                return valor;
            }
            return string.Empty;
        }
        
        public async Task<byte[]> GerarTemplateExcel()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Template");
            
            string[] cabecalhos = {
                "Nome", "Email", "CPF", "DataNascimento", "Telefone", 
                "Sexo", "Naturalidade", "Nacionalidade", 
                "CEP", "Logradouro", "Numero", "Complemento", 
                "Bairro", "Cidade", "Estado"
            };
            
            for (int i = 0; i < cabecalhos.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = cabecalhos[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            
            string[] exemploValores = {
                "João Silva", "joao@email.com", "12345678909", "1990-01-01", "(11) 98765-4321",
                "0", "São Paulo", "Brasil",
                "01234-567", "Rua Exemplo", "123", "Apto 45",
                "Centro", "São Paulo", "SP"
            };
            
            for (int i = 0; i < exemploValores.Length; i++)
            {
                worksheet.Cell(2, i + 1).Value = exemploValores[i];
            }
            
            string[] notas = {
                "Obrigatório", "Opcional", "Obrigatório", "Obrigatório (AAAA-MM-DD)", "Opcional",
                "0=Masculino, 1=Feminino, 2=Outro", "Opcional", "Opcional",
                "Opcional", "Opcional", "Opcional", "Opcional",
                "Opcional", "Opcional", "Opcional"
            };
            
            for (int i = 0; i < notas.Length; i++)
            {
                var cell = worksheet.Cell(3, i + 1);
                cell.Value = notas[i];
                cell.Style.Font.Italic = true;
                cell.Style.Font.FontColor = XLColor.Gray;
            }
            
            worksheet.Columns().AdjustToContents();
            
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
    }
}