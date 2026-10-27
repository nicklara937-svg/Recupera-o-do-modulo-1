/* =====================================================================
   TaskFlow API - Carga inicial OPCIONAL (útil para a demonstração)
   Execute somente depois de criar o banco (script.sql ou migrations).
   ===================================================================== */

USE [TaskFlowDb];
GO

INSERT INTO [Tarefas] ([Titulo], [Descricao], [Status], [DataCriacao], [DataConclusao])
VALUES
    (N'Configurar ambiente de desenvolvimento', N'Instalar .NET SDK, SQL Server e ferramentas do EF Core', N'Concluida', DATEADD(DAY, -5, SYSUTCDATETIME()), DATEADD(DAY, -4, SYSUTCDATETIME())),
    (N'Modelar o banco de dados', N'Definir entidades e relacionamentos da TaskFlow API', N'EmAndamento', DATEADD(DAY, -3, SYSUTCDATETIME()), NULL),
    (N'Escrever documentação do projeto', N'Completar o README com instruções de execução', N'Pendente', DATEADD(DAY, -1, SYSUTCDATETIME()), NULL),
    (N'Gravar vídeo de apresentação', NULL, N'Pendente', SYSUTCDATETIME(), NULL);
GO
