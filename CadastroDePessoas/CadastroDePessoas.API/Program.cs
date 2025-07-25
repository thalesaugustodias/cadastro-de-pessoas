using CadastroDePessoas.API.Configuracoes;
using CadastroDePessoas.API.Filtros;
using CadastroDePessoas.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExcecaoFiltro>();
})
.AddFluentValidation();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AdicionarSwagger();

builder.Services.AdicionarCorsConfiguracao(builder.Configuration);

builder.Services.AdicionarJwtAutenticacao(builder.Configuration);

builder.Services.AdicionarDatabase(builder.Configuration);

builder.Services.AdicionarInfraestrutura(builder.Configuration);

builder.Services.AdicionarMediatR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.InicializarDatabase();

app.UsarSwagger();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UsarCorsConfiguracao();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();