using TaskFlow.API.Models.Enums;

namespace TaskFlow.API.Models.DTOs;

public record TarefaResponseDto(
    int Id,
    string Titulo,
    string? Descricao,
    StatusTarefa Status,
    DateTime DataCriacao,
    DateTime? DataConclusao);
