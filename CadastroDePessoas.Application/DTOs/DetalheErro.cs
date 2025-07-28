using CadastroDePessoas.Application.DTOs;

namespace CadastroDePessoas.Application.DTOs
{
    public class DetalheErro
    {
        public int Linha { get; set; }
        public string Mensagem { get; set; }
        public Dictionary<string, string> ValoresOriginais { get; set; } = new Dictionary<string, string>();
        public string RegistroOriginal => ValoresOriginais != null && ValoresOriginais.Count > 0 ? string.Join(",", ValoresOriginais.Values) : "";
    }
}