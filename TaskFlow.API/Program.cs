using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.Middlewares;
using TaskFlow.API.Models.DTOs;
using TaskFlow.API.Repositories;
using TaskFlow.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + serialização dos enums como texto (ex.: "EmAndamento")
builder.Services.AddControllers()
    .AddJsonOptions(opcoes => opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        // Erros de validação de entrada (DataAnnotations) seguem o mesmo padrão JSON do middleware
        opcoes.InvalidModelStateResponseFactory = contexto =>
        {
            var detalhes = contexto.ModelState.Values
                .SelectMany(valor => valor.Errors)
                .Select(erro => string.IsNullOrWhiteSpace(erro.ErrorMessage) ? "Valor inválido." : erro.ErrorMessage)
                .ToList();

            var resposta = new ErroResponse(
                StatusCodes.Status400BadRequest,
                "Os dados enviados são inválidos.",
                detalhes,
                contexto.HttpContext.Request.Path.ToString(),
                DateTime.UtcNow);

            return new BadRequestObjectResult(resposta);
        };
    });

// Entity Framework Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(opcoes =>
    opcoes.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeção de dependência das camadas (ciclo de vida Scoped: uma instância por requisição)
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<ITarefaService, TarefaService>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Deve ser o primeiro da pipeline para capturar exceções de todas as etapas seguintes
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
