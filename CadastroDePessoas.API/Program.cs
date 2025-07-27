using CadastroDePessoas.API.Configuracoes;
using CadastroDePessoas.API.Filtros;
using CadastroDePessoas.API.Middlewares;
using CadastroDePessoas.IoC;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExcecaoFiltro>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AdicionarSwagger();
builder.Services.AdicionarCorsConfiguracao(builder.Configuration);
builder.Services.AdicionarJwtAutenticacao(builder.Configuration);
builder.Services.AdicionarDatabase(builder.Configuration);

builder.Services.AdicionarInfraestrutura(builder.Configuration);
builder.Services.AdicionarMediatR();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next.Invoke();
});

app.UseMiddleware<CorsPreflightMiddleware>();

app.UsarCorsConfiguracao(app.Environment);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseMiddleware<SecurityMiddleware>();

app.InicializarDatabase();
app.UsarSwagger();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();