# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar todos os arquivos do reposit�rio para o diret�rio /app
COPY . ./

# Listar arquivos para debug
RUN ls -R /app

# Restaurar depend�ncias apenas do projeto API
RUN dotnet restore CadastroDePessoas.API/CadastroDePessoas.API.csproj

# Fazer build da aplica��o
RUN dotnet publish CadastroDePessoas.API/CadastroDePessoas.API.csproj -c Release -o out

# Imagem de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Criar diret�rio para o SQLite
RUN mkdir -p /var/lib/render/app-data

# Configurar vari�veis de ambiente
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
ENTRYPOINT ["dotnet", "CadastroDePessoas.API.dll"]