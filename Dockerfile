# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar arquivos de solução e projeto primeiro para melhor cache do Docker
COPY *.sln ./
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

# Restaurar dependências
RUN dotnet restore CadastroDePessoas.sln

# Copiar código fonte
COPY . ./

# Fazer build da aplicação
RUN dotnet publish CadastroDePessoas.API/CadastroDePessoas.API.csproj -c Release -o out

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