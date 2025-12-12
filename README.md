📘 Blog API & Frontend (Full-Stack)

Projeto Full-Stack desenvolvido com API em .NET 8, Frontend em React (TypeScript) e banco de dados PostgreSQL. O ambiente é totalmente conteinerizado utilizando Docker e Docker Compose.

🛠 Tecnologias Utilizadas

Componente | Tecnologia | Versão | Notas
Backend API | .NET | 8.0 | Arquitetura em Camadas, CQRS (MediatR), JWT
Banco de Dados | PostgreSQL | Definido no Docker | Persistência via Entity Framework Core
Frontend | React (TypeScript) | Definido no Docker | Interface do usuário, consumo da API
ORM | Entity Framework Core | - | Mapeamento Objeto-Relacional
Conteinerização | Docker / Docker Compose | - | Orquestração dos serviços

Usuário Administrador criado por Seeder

O backend cria automaticamente um usuário administrador para acesso inicial ao sistema:

{
  "username": "Admin",
  "email": "Admin@gmail.com.br",
  "password": "Admin122025"
}


Esse usuário recebe a role Admin, definida no enum PerfilDeAcessoBlog.

CRUD de Usuários

A criação de usuários já atribui a role definida no momento do cadastro. No backend, os endpoints de listar, editar e excluir usuários estão implementados. O frontend ainda não possui todas as telas finalizadas para esse fluxo.

🚀 1. Execução Full-Stack (Ambiente Completo)

Modo recomendado para subir a aplicação completa (API + Frontend + Banco) com um único comando Docker.

A. Local de Execução

Abra o terminal e navegue até a raiz do projeto, onde está o arquivo docker-compose.yml:

cd C:\Users\User\desktop\dev.net\blog.api

B. Comando de Inicialização
docker-compose up --build

C. Endereços de Acesso

Componente | URL | Descrição
Frontend | http://localhost:5173/
 | Tela inicial
Login | http://localhost:5173/login
 | Acesso direto ao login
API (Swagger) | http://localhost
:<PORTA_DA_API>/swagger | Testes interativos da API

Observações:
A porta padrão do frontend é 5173.
A porta backend é definida no docker-compose.yml (comum: 5000 ou 5001).

💻 2. Execução Apenas do Backend (Desenvolvimento Local)

A. Iniciar o Banco de Dados
docker-compose up db

B. Iniciar a API
cd Blog.Api.API
dotnet run

C. Acesso ao Swagger
http://localhost:5160/swagger/index.html


🗃 Detalhes da Arquitetura

A API segue Clean Architecture com CQRS via MediatR.

Camadas:
API: Controladores, configuração, autenticação e Swagger
Application: Handlers, validações, lógica e interfaces
Domain: Entidades e regras de domínio
Infrastructure: Persistência, repositórios e EF Core

Swagger permite visualizar e testar todos os endpoints, incluindo rotas protegidas via Bearer Token.

🛑 Encerrar Containers

Para parar tudo:

docker-compose down
