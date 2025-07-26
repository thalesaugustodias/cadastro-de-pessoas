FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar os arquivos csproj e restaurar dependências
COPY CadastroDePessoas.API/*.csproj CadastroDePessoas.API/
COPY CadastroDePessoas.Application/*.csproj CadastroDePessoas.Application/
COPY CadastroDePessoas.Domain/*.csproj CadastroDePessoas.Domain/
COPY CadastroDePessoas.Infraestructure/*.csproj CadastroDePessoas.Infraestructure/
COPY CadastroDePessoas.IoC/*.csproj CadastroDePessoas.IoC/
COPY *.sln .
RUN dotnet restore

# Copiar todo o resto e fazer build
COPY . ./
RUN dotnet publish CadastroDePessoas.API/CadastroDePessoas.API.csproj -c Release -o out

# Build da imagem de runtime
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