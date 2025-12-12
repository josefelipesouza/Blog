📘 Blog API & Frontend (Full-Stack)

Projeto Full-Stack desenvolvido com API em .NET 8, Frontend em React (TypeScript) e banco de dados PostgreSQL. Todo o ambiente é totalmente conteinerizado utilizando Docker e Docker Compose.

🛠 Tecnologias Utilizadas
Componente	Tecnologia	Versão	Notas
Backend API	.NET	8.0	Arquitetura em Camadas, CQRS (MediatR), JWT
Banco de Dados	PostgreSQL	Definido no Docker	Persistência via Entity Framework Core
Frontend	React (TypeScript)	Definido no Docker	Interface do usuário, consumo da API
ORM	Entity Framework Core	-	Mapeamento Objeto-Relacional
Conteinerização	Docker / Docker Compose	-	Orquestração dos serviços
🚀 1. Execução Full-Stack (Ambiente Completo)

Modo recomendado para subir a aplicação completa (API + Frontend + Banco) com um único comando Docker.

A. Local de Execução

Abra o terminal e navegue até a raiz do projeto, onde está o arquivo docker-compose.yml.

Exemplo:

cd C:\Users\User\desktop\dev.net\blog.api

B. Comando de Inicialização

Execute:

docker-compose up --build

C. Endereços de Acesso

Após a inicialização, utilize:

Componente	URL de Acesso	Descrição
Frontend	http://localhost:5173/
	Tela inicial
Login do Frontend	http://localhost:5173/login
	Acesso direto ao login
API (Swagger)	http://localhost
:<PORTA_DA_API>/swagger	UI interativa para testar a API

Observação:
A porta padrão do frontend é 5173.
A porta da API é definida no docker-compose.yml (geralmente 5000 ou 5001).

💻 2. Execução Apenas do Backend (Desenvolvimento Local da API)

Ideal para trabalhar exclusivamente na API utilizando hot reload.

A. Iniciar o Banco de Dados

No diretório raiz do projeto:

docker-compose up db

B. Iniciar a API (.NET 8)

Navegue até o diretório da API:

cd Blog.Api.API


Execute:

dotnet run

C. Endereço de Acesso ao Swagger

O Swagger sobe na porta configurada no launchSettings.json ou appsettings.json.

Acesso:

http://localhost:5160/swagger/index.html

🗃 Detalhes Técnicos da Arquitetura

A API utiliza arquitetura limpa em camadas com CQRS via MediatR.

Estrutura:

API: Controladores, configuração, autenticação e Swagger.

Application: Handlers do MediatR, validações, lógica de negócio e interfaces.

Domain: Entidades, regras e invariantes de domínio.

Infrastructure: Repositórios, serviços e persistência via EF Core no PostgreSQL.

Uso do Swagger

O Swagger oferece:

Documentação completa dos endpoints de CRUD e autenticação.

Botão Authorize para inserir o Bearer Token.

Testes de rotas protegidas após fazer login ou registro.

🛑 Parar e Limpar Containers

Para encerrar todos os serviços de forma limpa:

docker-compose down
