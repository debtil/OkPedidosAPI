# OkPedidosAPI

A **OkPedidosAPI** é uma aplicação desenvolvida para gerenciar pedidos de um
sistema de entregas de marmitas.
Ela é construída em **.NET 9.0**, utiliza **PostgreSQL** como banco de dados e
roda via **Docker**.

------------------------------------------------------------------------

## Funcionalidades

-   **Gerenciamento de Pedidos:** Cadastro e controle dos pedidos de
    marmitas.
-   **Autenticação:** Sistema de login e gerenciamento de usuários.
-   **Database:** Conexão com PostgreSQL, configurado para ser acessado via
    Docker.

------------------------------------------------------------------------

## Tecnologias Utilizadas

-   **.NET 9.0** — Framework principal da API.
-   **PostgreSQL** — Banco de dados relacional para persistência.
-   **Docker** — Facilita o deploy e a padronização do ambiente.

------------------------------------------------------------------------

## Configuração

### Configurações de Banco de Dados

A aplicação conecta-se a um PostgreSQL.
Adicione no arquivo .env:

``ConnectionStrings__DefaultConnection=Host=seu_host;Port=5432;Database=okpedidosdb;Username=okpedidosdb_user;Password=your_password``

Certifique-se de adicionar corretamente as variáveis de ambiente no
sistema onde a API será executada.

------------------------------------------------------------------------

### Configuração do Ambiente Local

Clonando o Repositório

``git clone https://github.com/debtil/OkPedidosAPI.git``

**Criando o Arquivo .env**

Crie um arquivo .env na raiz do projeto e adicione a connection string
usada pelo banco de dados.

------------------------------------------------------------------------

### Docker

Para rodar a aplicação com Docker:

``docker-compose up``

Isso iniciará os serviços necessários e exporá a API na porta 5003.

------------------------------------------------------------------------

### Comandos Importantes

**Rodando Docker com Build**

``docker-compose up –build``

**Rodando Migrações**

``dotnet ef migrations add InitialCreate``
``dotnet ef database update``

**Acessando Localmente**

Após iniciar, a API ficará disponível em:

``http://localhost:5003``
