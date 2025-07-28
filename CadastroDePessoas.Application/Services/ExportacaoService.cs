using System.Text;
using CadastroDePessoas.Application.DTOs.Pessoa;
using CadastroDePessoas.Domain.Interfaces;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CadastroDePessoas.Application.Services
{
    public class ExportacaoService
    {
        private readonly IRepositorioPessoa _repositorioPessoa;

        public ExportacaoService(IRepositorioPessoa repositorioPessoa)
        {
            _repositorioPessoa = repositorioPessoa;
        }

        public async Task<byte[]> ExportarParaExcel(string[] campos)
        {
            var pessoas = await _repositorioPessoa.ObterTodosAsync();
            var pessoasDto = pessoas.Select(p => new PessoaDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Sexo = p.Sexo,
                Email = p.Email,
                DataNascimento = p.DataNascimento,
                Naturalidade = p.Naturalidade,
                Nacionalidade = p.Nacionalidade,
                CPF = p.CPF,
                Telefone = p.Telefone,
                DataCadastro = p.DataCadastro,
                DataAtualizacao = p.DataAtualizacao,
                Endereco = p.Endereco != null ? new DTOs.Endereco.EnderecoDTO
                {
                    Id = p.Endereco.Id,
                    CEP = p.Endereco.CEP,
                    Logradouro = p.Endereco.Logradouro,
                    Numero = p.Endereco.Numero,
                    Complemento = p.Endereco.Complemento,
                    Bairro = p.Endereco.Bairro,
                    Cidade = p.Endereco.Cidade,
                    Estado = p.Endereco.Estado
                } : null
            }).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Pessoas");

            // Adicionar cabeçalhos
            int colIndex = 1;
            foreach (var campo in campos)
            {
                worksheet.Cell(1, colIndex).Value = ObterNomeCampo(campo);
                colIndex++;
            }

            // Adicionar dados
            int rowIndex = 2;
            foreach (var pessoa in pessoasDto)
            {
                colIndex = 1;
                foreach (var campo in campos)
                {
                    worksheet.Cell(rowIndex, colIndex).Value = ObterValorCampo(pessoa, campo);
                    colIndex++;
                }
                rowIndex++;
            }

            // Formatar cabeçalhos
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
            
            // Ajustar largura das colunas
            worksheet.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> ExportarParaPdf(string[] campos)
        {
            var pessoas = await _repositorioPessoa.ObterTodosAsync();
            var pessoasDto = pessoas.Select(p => new PessoaDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Sexo = p.Sexo,
                Email = p.Email,
                DataNascimento = p.DataNascimento,
                Naturalidade = p.Naturalidade,
                Nacionalidade = p.Nacionalidade,
                CPF = p.CPF,
                Telefone = p.Telefone,
                DataCadastro = p.DataCadastro,
                DataAtualizacao = p.DataAtualizacao,
                Endereco = p.Endereco != null ? new DTOs.Endereco.EnderecoDTO
                {
                    Id = p.Endereco.Id,
                    CEP = p.Endereco.CEP,
                    Logradouro = p.Endereco.Logradouro,
                    Numero = p.Endereco.Numero,
                    Complemento = p.Endereco.Complemento,
                    Bairro = p.Endereco.Bairro,
                    Cidade = p.Endereco.Cidade,
                    Estado = p.Endereco.Estado
                } : null
            }).ToList();

            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4.Rotate());
            var writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            // Adicionar título
            var titulo = new Paragraph("Relatório de Pessoas Cadastradas", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(titulo);

            // Adicionar data de geração
            var dataGeracao = new Paragraph($"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", new Font(Font.FontFamily.HELVETICA, 10))
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingAfter = 20
            };
            document.Add(dataGeracao);

            // Criar tabela
            var table = new PdfPTable(campos.Length)
            {
                WidthPercentage = 100
            };

            // Configurar cabeçalhos
            var headerFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.WHITE);
            foreach (var campo in campos)
            {
                var cell = new PdfPCell(new Phrase(ObterNomeCampo(campo), headerFont))
                {
                    BackgroundColor = new BaseColor(30, 119, 243),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 5
                };
                table.AddCell(cell);
            }

            // Adicionar dados
            var dataFont = new Font(Font.FontFamily.HELVETICA, 9);
            foreach (var pessoa in pessoasDto)
            {
                foreach (var campo in campos)
                {
                    var cell = new PdfPCell(new Phrase(ObterValorCampo(pessoa, campo), dataFont))
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 4
                    };
                    table.AddCell(cell);
                }
            }

            document.Add(table);
            document.Close();
            return ms.ToArray();
        }

        public async Task<byte[]> GerarTemplateCsv()
        {
            var cabecalhos = "Nome,Email,CPF,DataNascimento,Telefone,Sexo,Naturalidade,Nacionalidade,CEP,Logradouro,Numero,Complemento,Bairro,Cidade,Estado";
            var exemplo = "\"João Silva\",\"joao@email.com\",\"123.456.789-01\",\"1990-01-01\",\"(11) 99999-9999\",\"0\",\"São Paulo\",\"Brasil\",\"01310-100\",\"Av. Paulista\",\"1000\",\"Apto 101\",\"Bela Vista\",\"São Paulo\",\"SP\"";
            
            var conteudo = $"{cabecalhos}\n{exemplo}";
            return Encoding.UTF8.GetBytes(conteudo);
        }

        private string ObterNomeCampo(string campo)
        {
            return campo switch
            {
                "nome" => "Nome",
                "email" => "E-mail",
                "cpf" => "CPF",
                "dataNascimento" => "Data de Nascimento",
                "telefone" => "Telefone",
                "sexo" => "Sexo",
                "naturalidade" => "Naturalidade",
                "nacionalidade" => "Nacionalidade",
                "endereco.cep" => "CEP",
                "endereco.logradouro" => "Logradouro",
                "endereco.numero" => "Número",
                "endereco.complemento" => "Complemento",
                "endereco.bairro" => "Bairro",
                "endereco.cidade" => "Cidade",
                "endereco.estado" => "Estado",
                _ => campo
            };
        }

        private string ObterValorCampo(PessoaDTO pessoa, string campo)
        {
            return campo switch
            {
                "nome" => pessoa.Nome ?? "",
                "email" => pessoa.Email ?? "",
                "cpf" => pessoa.CPF ?? "",
                "dataNascimento" => pessoa.DataNascimento.ToString("dd/MM/yyyy"),
                "telefone" => pessoa.Telefone ?? "",
                "sexo" => pessoa.SexoDescricao ?? "",
                "naturalidade" => pessoa.Naturalidade ?? "",
                "nacionalidade" => pessoa.Nacionalidade ?? "",
                "endereco.cep" => pessoa.Endereco?.CEP ?? "",
                "endereco.logradouro" => pessoa.Endereco?.Logradouro ?? "",
                "endereco.numero" => pessoa.Endereco?.Numero ?? "",
                "endereco.complemento" => pessoa.Endereco?.Complemento ?? "",
                "endereco.bairro" => pessoa.Endereco?.Bairro ?? "",
                "endereco.cidade" => pessoa.Endereco?.Cidade ?? "",
                "endereco.estado" => pessoa.Endereco?.Estado ?? "",
                _ => ""
            };
        }
    }
}