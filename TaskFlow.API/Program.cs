using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.Repositories;
using TaskFlow.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + serialização dos enums como texto (ex.: "EmAndamento")
builder.Services.AddControllers()
    .AddJsonOptions(opcoes => opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Entity Framework Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(opcoes =>
    opcoes.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeção de dependência das camadas (ciclo de vida Scoped: uma instância por requisição)
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<ITarefaService, TarefaService>();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
