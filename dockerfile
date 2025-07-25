FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Instalar Node.js
RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash -
RUN apt-get install -y nodejs

# Copiar solution e projetos
COPY *.sln ./
COPY CadastroDePessoas.API/*.csproj ./CadastroDePessoas.API/
COPY CadastroDePessoas.Application/*.csproj ./CadastroDePessoas.Application/
COPY CadastroDePessoas.Domain/*.csproj ./CadastroDePessoas.Domain/
COPY CadastroDePessoas.Infraestructure/*.csproj ./CadastroDePessoas.Infraestructure/
COPY CadastroDePessoas.IoC/*.csproj ./CadastroDePessoas.IoC/

# Restaurar dependências
RUN dotnet restore

# Copiar código fonte
COPY . ./

# Construir front-end React
WORKDIR /app/cadastro-de-pessoas-web
RUN npm ci --only=production
RUN npm run build

# Publicar aplicação .NET
WORKDIR /app
RUN dotnet publish CadastroDePessoas.API/CadastroDePessoas.API.csproj -c Release -o /app/publish

# Copiar build do React para wwwroot
RUN cp -r cadastro-de-pessoas-web/build/* /app/publish/wwwroot/

# Imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/publish .

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:$PORT
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CadastroDePessoas.API.dll"]