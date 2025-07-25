using CadastroDePessoas.API.Configuracoes;
using CadastroDePessoas.API.Filtros;
using CadastroDePessoas.IoC;

var builder = WebApplication.CreateBuilder(args);

// Configuração de controllers sem AddFluentValidation (deprecated)
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExcecaoFiltro>();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AdicionarSwagger();
builder.Services.AdicionarCorsConfiguracao(builder.Configuration);
builder.Services.AdicionarJwtAutenticacao(builder.Configuration);
builder.Services.AdicionarDatabase(builder.Configuration);

// Injeção de dependências seguindo Clean Architecture
builder.Services.AdicionarInfraestrutura(builder.Configuration);
builder.Services.AdicionarMediatR();

var app = builder.Build();

// Pipeline de configuração
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

// Servir arquivos estáticos (React build)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UsarCorsConfiguracao();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback para SPA
app.MapFallbackToFile("index.html");

app.Run();