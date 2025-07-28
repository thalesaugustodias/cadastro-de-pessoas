using CadastroDePessoas.Application.DTOs;
using CadastroDePessoas.Application.Interfaces;
using CadastroDePessoas.Application.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CadastroDePessoas.API.Controllers
{
    [ApiController]
    [Route("api/v1/importacao")]
    [Authorize]
    public class ImportacaoController(ImportacaoService importacaoService,IServiceCache serviceCache) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ImportarArquivo(IFormFile arquivo, [FromQuery] bool importarParcialmente = true)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest(new { message = "Nenhum arquivo fornecido" });
            }

            string extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (extensao != ".xlsx" && extensao != ".xls" && extensao != ".csv")
            {
                return BadRequest(new { message = "Apenas arquivos Excel (.xlsx, .xls) ou CSV (.csv) são aceitos" });
            }

            try
            {
                using var stream = arquivo.OpenReadStream();
                ImportacaoResultado resultado;
                
                if (extensao == ".csv")
                {
                    resultado = await importacaoService.ImportarCsv(stream, importarParcialmente);
                }
                else
                {
                    resultado = await importacaoService.ImportarExcel(stream, importarParcialmente);
                }
                
                if (resultado.Sucesso > 0)
                {
                    await serviceCache.RemoverAsync("pessoas_lista");
                }
                
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao importar arquivo", detail = ex.Message });
            }
        }
        
        [HttpGet("template/excel")]
        public async Task<IActionResult> DownloadTemplateExcel()
        {
            try
            {
                var bytes = await importacaoService.GerarTemplateExcel();
                return File(
                    bytes, 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    "template-importacao.xlsx"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar template Excel", detail = ex.Message });
            }
        }
        
        [HttpGet("erros-excel")]
        public IActionResult GerarExcelComErros([FromQuery] string errosJson)
        {
            if (string.IsNullOrEmpty(errosJson))
            {
                return BadRequest(new { message = "Dados de erro não fornecidos" });
            }

            try
            {
                var erros = System.Text.Json.JsonSerializer.Deserialize<List<DetalheErro>>(
                    errosJson, 
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (erros == null || erros.Count == 0)
                {
                    return BadRequest(new { message = "Nenhum erro encontrado nos dados fornecidos" });
                }

                using var workbook = new XLWorkbook();
                var worksheet = workbook.AddWorksheet("Registros com Erro");
                
                if (erros[0].ValoresOriginais?.Count > 0)
                {
                    var cabecalhos = erros[0].ValoresOriginais.Keys.ToList();
                    
                    cabecalhos.Add("MensagemErro");
                    
                    for (int i = 0; i < cabecalhos.Count; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = cabecalhos[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    
                    int linha = 2;
                    foreach (var erro in erros)
                    {
                        if (erro.ValoresOriginais?.Count > 0)
                        {
                            int col = 1;
                            foreach (var key in cabecalhos.Take(cabecalhos.Count - 1))
                            {
                                worksheet.Cell(linha, col).Value = erro.ValoresOriginais.TryGetValue(key, out var value) ? value : "";
                                col++;
                            }
                            
                            worksheet.Cell(linha, col).Value = erro.Mensagem;
                            worksheet.Cell(linha, col).Style.Font.FontColor = XLColor.Red;
                            
                            linha++;
                        }
                    }
                    
                    worksheet.Columns().AdjustToContents();
                }
                
                using var ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;
                
                return File(
                    ms.ToArray(), 
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                    "registros-com-erro.xlsx"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar Excel de erros", detail = ex.Message });
            }
        }
        
        [HttpGet("erros-csv")]
        public IActionResult GerarCsvComErros([FromQuery] string errosJson)
        {
            if (string.IsNullOrEmpty(errosJson))
            {
                return BadRequest(new { message = "Dados de erro não fornecidos" });
            }

            try
            {
                var erros = System.Text.Json.JsonSerializer.Deserialize<List<DetalheErro>>(
                    errosJson, 
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (erros == null || erros.Count == 0)
                {
                    return BadRequest(new { message = "Nenhum erro encontrado nos dados fornecidos" });
                }

                var linhasCsv = new List<string>();
                
                if (erros[0].ValoresOriginais?.Count > 0)
                {
                    var cabecalhos = erros[0].ValoresOriginais.Keys.ToList();
                    cabecalhos.Add("MensagemErro");
                    
                    linhasCsv.Add(string.Join(",", cabecalhos.Select(k => $"\"{k}\"")));
                    
                    foreach (var erro in erros)
                    {
                        if (erro.ValoresOriginais?.Count > 0)
                        {
                            var valores = erro.ValoresOriginais.Values
                                .Select(v => $"\"{v.Replace("\"", "\"\"")}\"")
                                .ToList();
                            
                            valores.Add($"\"{erro.Mensagem.Replace("\"", "\"\"")}\"");
                            
                            linhasCsv.Add(string.Join(",", valores));
                        }
                    }
                }

                var csvContent = string.Join("\r\n", linhasCsv);
                var bytes = Encoding.UTF8.GetBytes(csvContent);

                return File(bytes, "text/csv", "registros-com-erro.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar CSV de erros", detail = ex.Message });
            }
        }
    }
}