namespace CadastroDePessoas.API.Controllers
{
    public class UpdateProfileRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}