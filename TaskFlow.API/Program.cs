using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Controllers + serialização dos enums como texto (ex.: "EmAndamento")
builder.Services.AddControllers()
    .AddJsonOptions(opcoes => opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
