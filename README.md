# ✅ TaskFlow API

## 🎯 Sobre o Projeto

A **TaskFlow API** é uma Web API RESTful desenvolvida com **.NET**, **Entity Framework Core** e **SQL Server** para o gerenciamento e acompanhamento de tarefas de uma equipe. Ela substitui o controle por planilhas e mensagens espalhadas, permitindo **criar, consultar, atualizar, excluir, iniciar e concluir tarefas**, além de **filtrar por status**.

Projeto de Recuperação - Módulo 01 (Desenvolvedor(a) Back End .NET).

## 🛠 Tecnologias Utilizadas

- .NET 10 (ASP.NET Core Web API) e C#
- Entity Framework Core 10 (Fluent API + Migrations)
- SQL Server
- Swagger / OpenAPI (Swashbuckle)
- Git e GitHub (commits semânticos e branches por funcionalidade)

## ✔️ Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server (local, LocalDB ou Docker)
- Ferramenta do EF Core: `dotnet tool install --global dotnet-ef`

## 🚀 Como Executar o Projeto

### 1. Clonar o repositório

```bash
git clone <URL-DO-SEU-REPOSITORIO>
cd TaskFlow
```

### 2. Subir um SQL Server (exemplo com Docker)

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=TaskFlow@123" \
  -p 1433:1433 --name taskflow-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

> Já tem SQL Server instalado? Basta ajustar a connection string (passo 3).

### 3. Configurar a Connection String

Edite `TaskFlow.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=TaskFlowDb;User Id=sa;Password=TaskFlow@123;TrustServerCertificate=True;"
}
```

Alternativas comuns:

| Cenário | Connection string |
|---|---|
| SQL Server Express (Windows Auth) | `Server=localhost\\SQLEXPRESS;Database=TaskFlowDb;Trusted_Connection=True;TrustServerCertificate=True;` |
| LocalDB (Windows) | `Server=(localdb)\\MSSQLLocalDB;Database=TaskFlowDb;Trusted_Connection=True;` |

### 4. Criar o banco de dados

**Opção A - Migrations do EF Core (recomendada):**

```bash
dotnet ef database update --project TaskFlow.API
```

**Opção B - Script SQL:** execute `Database/script.sql` no SSMS / Azure Data Studio ou via `sqlcmd`:

```bash
sqlcmd -S localhost,1433 -U sa -P "TaskFlow@123" -C -i Database/script.sql
```

*(Opcional)* Para popular dados de exemplo, execute `Database/seed.sql`.

### 5. Executar a aplicação

```bash
dotnet run --project TaskFlow.API
```

### 6. Acessar o Swagger

Abra **http://localhost:5080/swagger** no navegador.

## 🏗 Arquitetura

O projeto segue arquitetura em camadas com responsabilidades isoladas e fluxo **Controller → Service → Repository**:

```
TaskFlow/
 ├── Database/
 │    ├── script.sql                 --> Criação do banco (equivalente às migrations)
 │    └── seed.sql                   --> Carga inicial opcional
 └── TaskFlow.API/
      ├── Controllers/               --> Rotas HTTP e Status Codes
      ├── Services/                  --> Regras de negócio e validações
      ├── Repositories/              --> Acesso a dados (EF Core / DbContext)
      ├── Models/
      │    ├── Entities/             --> Tarefa, Chamado, Categoria, Interacao
      │    ├── Enums/                --> StatusTarefa
      │    └── DTOs/                 --> Contratos de entrada/saída da API
      ├── Exceptions/                --> Exceções de domínio (404 / 400)
      ├── Middlewares/               --> ExceptionHandlingMiddleware (erros globais)
      ├── Data/                      --> AppDbContext e Migrations
      └── Program.cs                 --> Pipeline e Injeção de Dependência
```

| Camada | Responsabilidade |
|---|---|
| **Controller** | Recebe a requisição, delega ao Service e devolve o Status Code adequado |
| **Service** | Valida dados, aplica as regras de negócio (ex.: transições de status) |
| **Repository** | Única camada que acessa o banco, via EF Core |
| **Middleware** | Captura exceções não tratadas e responde JSON padronizado, sem stack trace |
| **Program.cs** | Registra as dependências (`AddScoped`) e monta a pipeline |

## 🗃 Modelo de Dados

- **Tarefa** (`Id`, `Titulo`, `Descricao`, `Status`, `DataCriacao`, `DataConclusao`)
- **Chamado** (`Id`, `Titulo`, `Descricao`, `DataCriacao`, `TarefaId`, `CategoriaId`)
- **Categoria** (`Id`, `Nome`)
- **Interacao** (`Id`, `Mensagem`, `DataCriacao`, `ChamadoId`)

Relacionamentos: **Tarefa 1:N Chamado**, **Categoria 1:N Chamado** (opcional) e **Chamado 1:N Interacao**.

Status da tarefa: `Pendente` → `EmAndamento` → `Concluida`. Datas são armazenadas em **UTC**.

## 📌 Endpoints

| Método | Rota | Descrição | Sucesso | Erros |
|---|---|---|---|---|
| POST | `/api/tarefas` | Cria tarefa (status `Pendente`) | 201 | 400 |
| GET | `/api/tarefas` | Lista todas as tarefas | 200 | - |
| GET | `/api/tarefas?status=Pendente` | Filtra por status | 200 | 400 |
| GET | `/api/tarefas/{id}` | Consulta uma tarefa | 200 | 404 |
| PUT | `/api/tarefas/{id}` | Atualiza título e descrição | 200 | 400, 404 |
| DELETE | `/api/tarefas/{id}` | Exclui a tarefa | 204 | 404 |
| PATCH | `/api/tarefas/{id}/iniciar` | `Pendente` → `EmAndamento` | 200 | 400, 404 |
| PATCH | `/api/tarefas/{id}/concluir` | `EmAndamento` → `Concluida` (+ `DataConclusao`) | 200 | 400, 404 |

Valores aceitos em `status`: `Pendente`, `EmAndamento`, `Concluida` (sem diferenciar maiúsculas/minúsculas ou acentos).

## 📏 Regras de Negócio

- Toda tarefa nasce com status **Pendente** e `DataCriacao` preenchida automaticamente.
- `PUT` altera **somente** `Titulo` e `Descricao`; `DataCriacao` e `DataConclusao` não podem ser alteradas manualmente.
- **Iniciar** só é permitido para tarefas `Pendente`.
- **Concluir** só é permitido para tarefas `EmAndamento` e preenche `DataConclusao` automaticamente.
- Transições inválidas retornam **400 Bad Request** com mensagem explicativa (validação na camada Service).

## 🧪 Exemplos de Utilização

**Criar tarefa**

```bash
curl -X POST http://localhost:5080/api/tarefas \
  -H "Content-Type: application/json" \
  -d '{"titulo": "Estudar Entity Framework", "descricao": "Revisar migrations"}'
```

```json
{
  "id": 1,
  "titulo": "Estudar Entity Framework",
  "descricao": "Revisar migrations",
  "status": "Pendente",
  "dataCriacao": "2026-10-26T15:30:00Z",
  "dataConclusao": null
}
```

**Iniciar e concluir**

```bash
curl -X PATCH http://localhost:5080/api/tarefas/1/iniciar
curl -X PATCH http://localhost:5080/api/tarefas/1/concluir
```

**Filtrar por status**

```bash
curl "http://localhost:5080/api/tarefas?status=Concluida"
```

**Atualizar / Excluir**

```bash
curl -X PUT http://localhost:5080/api/tarefas/1 \
  -H "Content-Type: application/json" \
  -d '{"titulo": "Novo título", "descricao": "Nova descrição"}'

curl -X DELETE http://localhost:5080/api/tarefas/1
```

## 🛡 Tratamento de Erros

Todos os erros seguem o mesmo formato JSON (sem stack trace). Exemplo de transição inválida (`400`):

```json
{
  "status": 400,
  "mensagem": "Não é possível concluir a tarefa 1: o status atual é 'Pendente'. Apenas tarefas EmAndamento podem ser concluídas.",
  "caminho": "/api/tarefas/1/concluir",
  "timestamp": "2026-10-26T15:31:00Z"
}
```

| Situação | Status |
|---|---|
| Recurso inexistente | 404 Not Found |
| Dados inválidos / regra de negócio violada | 400 Bad Request |
| Erro inesperado (detalhes apenas no log do servidor) | 500 Internal Server Error |

## 🌿 Versionamento

Fluxo com branches por funcionalidade integradas em `develop` e liberadas na `main`:

- `feature/estrutura-inicial`, `feature/entidades-ef-migrations`, `feature/crud-tarefas`
- `feature/controle-status`, `feature/filtro-status`, `feature/middleware-erros`
- `docs/readme`

Commits semânticos (`feat:`, `fix:`, `docs:`, `chore:`, `refactor:`).

## 👥 Autoria
Nicolas Lara

