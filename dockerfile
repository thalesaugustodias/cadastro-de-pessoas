FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Instalar Node.js
RUN apt-get update
RUN apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_18.x | bash -
RUN apt-get install -y nodejs

# Copiar arquivos do projeto e restaurar dependências
COPY . ./
RUN dotnet restore "CadastroDePessoas.sln"

# Publicar o projeto .NET
WORKDIR /app/src/CadastroPessoas.API
RUN dotnet publish -c Release -o /app/publish

# Construir o front-end React
WORKDIR /app/cadastro-de-pessoas-web
RUN npm install
RUN npm run build
RUN cp -r build/* /app/publish/wwwroot/

# Configurar a imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/publish .

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:$PORT

# Iniciar a aplicação
ENTRYPOINT ["dotnet", "CadastroPessoas.API.dll"]