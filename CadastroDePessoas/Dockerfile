# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar os arquivos csproj e restaurar dependências
COPY CadastroDePessoas/CadastroDePessoas.API/*.csproj CadastroDePessoas/CadastroDePessoas.API/
COPY CadastroDePessoas/CadastroDePessoas.Application/*.csproj CadastroDePessoas/CadastroDePessoas.Application/
COPY CadastroDePessoas/CadastroDePessoas.Domain/*.csproj CadastroDePessoas/CadastroDePessoas.Domain/
COPY CadastroDePessoas/CadastroDePessoas.Infraestructure/*.csproj CadastroDePessoas/CadastroDePessoas.Infraestructure/
COPY CadastroDePessoas/CadastroDePessoas.IoC/*.csproj CadastroDePessoas/CadastroDePessoas.IoC/
COPY CadastroDePessoas/*.sln CadastroDePessoas/
RUN dotnet restore CadastroDePessoas/CadastroDePessoas.sln

# Copiar todo o resto e fazer build
COPY . ./
RUN dotnet publish CadastroDePessoas/CadastroDePessoas.API/CadastroDePessoas.API.csproj -c Release -o out

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