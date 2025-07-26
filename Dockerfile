# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar todos os arquivos do repositório para o diretório /app
COPY . ./

# Navegar para o diretório onde está o .sln
WORKDIR /app/CadastroDePessoas.API
RUN dotnet restore CadastroDePessoas.sln

# Fazer build da aplicação
RUN dotnet publish CadastroDePessoas.API.csproj -c Release -o /app/out

# Imagem de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Criar diretório para o SQLite
RUN mkdir -p /var/lib/render/app-data

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
ENTRYPOINT ["dotnet", "CadastroDePessoas.API.dll"]