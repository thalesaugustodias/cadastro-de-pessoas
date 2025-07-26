# Cadastro de Pessoas - Aplicação Fullstack

O projeto Cadastro de Pessoas é uma solução completa de gerenciamento de cadastros desenvolvida com .NET 8 (backend) e React 18 (frontend). Esta aplicação demonstra boas práticas de desenvolvimento, arquitetura limpa e utilização de tecnologias modernas.

## 🏗️ Estrutura do Projeto

O projeto segue uma arquitetura em camadas com separação clara de responsabilidades:

### Backend (.NET 8)
- **CadastroDePessoas.API**: Camada de apresentação/controllers
- **CadastroDePessoas.Application**: Camada de aplicação com serviços e DTOs
- **CadastroDePessoas.Domain**: Entidades e regras de negócio
- **CadastroDePessoas.Infraestructure**: Persistência e implementações externas
- **CadastroDePessoas.IoC**: Configuração de injeção de dependências

### Frontend (React 18)
- Aplicação React moderna usando Vite como bundler
- Interface com Chakra UI para componentes responsivos
- Formulários com react-hook-form e validação com Yup

## 🚀 Como Executar o Projeto

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v16 ou superior)
- [Docker](https://www.docker.com/) (opcional, para Redis)

### Backend (.NET API)

1. **Clone o repositório**:
   ```bash
   git clone https://github.com/thalesaugustodias/desafio-stefanini.git
   cd desafio-stefanini/CadastroDePessoas
   ```

2. **Restaure as dependências**:
   ```bash
   dotnet restore
   ```

3. **Execute o projeto API**:
   ```bash
   cd CadastroDePessoas.API
   dotnet run
   ```

   O backend estará disponível em `https://localhost:5001`

### Frontend (React)

1. **Navegue até a pasta do frontend**:
   ```bash
   cd cadastro-de-pessoas-web
   ```

2. **Instale as dependências**:
   ```bash
   npm install
   ```

3. **Execute o frontend**:
   ```bash
   npm run dev
   ```

   O frontend estará disponível em `http://localhost:3001`

## 🗄️ Banco de Dados

A aplicação utiliza SQLite por padrão para facilitar a execução sem configurações adicionais. O banco de dados é criado automaticamente na primeira execução.

### Usuários padrão
- **Admin**: admin@exemplo.com / Admin@123
- **Usuário**: user@teste.com / User@123

### Resetar o banco de dados
Para limpar e recriar o banco de dados com os dados iniciais, acesse:
- `https://localhost:5001/api/v1/health/reset-database` (POST)

## 🔄 Cache com Redis (Opcional)

A aplicação pode usar Redis como provedor de cache ou a memória local. Para habilitar o Redis:

### 1. Execute o Redis com Docker
```bash
docker run --name redis-cache -p 6379:6379 -d redis
```

### 2. Configure a aplicação
No arquivo `appsettings.json` ou `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cadastro_pessoas.db",
    "Redis": "localhost:6379"
  },
  "UseRedisCache": "true"
}
```

- `UseRedisCache`: Define se o Redis será usado como cache
  - `true`: Usa Redis para cache (requer Redis em execução)
  - `false`: Usa cache em memória (padrão, não requer configuração adicional)

## 📝 API Documentation

A documentação da API está disponível via Swagger:
- `https://localhost:5001/swagger`

## 🧪 Testes

Para executar os testes automatizados:

```bash
dotnet test
```

## 🤝 Contribuição

Contribuições são bem-vindas! Para contribuir:
1. Faça um fork do projeto
2. Crie uma branch com sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo LICENSE para mais detalhes.