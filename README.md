# E-commerce Microservices

## Descrição do Projeto
Este projeto é uma aplicação de **microserviços** para gerenciamento de estoque e vendas de produtos em uma plataforma de e-commerce. Ele utiliza .NET 9, C#, Entity Framework, RabbitMQ, JWT para autenticação e SQL Server como banco de dados.

A arquitetura é composta por:

- **Microserviço de Estoque (`Inventory`)**: gerencia produtos e quantidade em estoque.
- **Microserviço de Vendas (`Sales`)**: gerencia pedidos e valida disponibilidade do estoque.
- **API Gateway (`Gateway`)**: centraliza o acesso às APIs dos microserviços.
- **RabbitMQ**: comunicação assíncrona entre microserviços.
- **Autenticação JWT**: protege os endpoints, garantindo que apenas usuários autorizados possam executar ações.

---

## Arquitetura


- O **Gateway** direciona requisições para o microserviço correto.
- O **Sales** cria pedidos e publica eventos de venda no **RabbitMQ**.
- O **Inventory** consome os eventos do **RabbitMQ** para atualizar o estoque.
- A autenticação JWT garante que apenas usuários autenticados possam acessar os endpoints.

---

## Tecnologias Utilizadas

- .NET 9 (C#)
- Entity Framework Core
- SQL Server
- RabbitMQ
- JWT para autenticação
- Serilog para logging
- Docker / Docker Compose
- Swagger para documentação das APIs

---

## Funcionalidades

### Microserviço de Estoque (Inventory)
- Cadastro de produtos (nome, descrição, preço, quantidade)
- Consulta de produtos
- Atualização automática do estoque ao receber evento de venda

### Microserviço de Vendas (Sales)
- Criação de pedidos
- Validação de estoque antes de confirmar o pedido
- Notificação de venda via RabbitMQ

### Comum
- Autenticação via JWT
- API Gateway com roteamento centralizado
- Logging centralizado via Serilog
- Swagger para testes de endpoints

---

## Requisitos

- Docker e Docker Compose
- .NET 9 SDK
- SQL Server (via Docker)
- RabbitMQ (via Docker)
