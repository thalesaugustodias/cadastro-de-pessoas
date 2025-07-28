namespace CadastroDePessoas.Application.DTOs
{
    public class ImportacaoResultado
    {
        public int Total { get; set; }
        public int Sucesso { get; set; }
        public int Erros { get; set; }
        public List<DetalheErro> Detalhes { get; set; }
    }
}