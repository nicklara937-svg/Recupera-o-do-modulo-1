using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.Models.DTOs;

/// <summary>
/// Dados aceitos na criação de uma tarefa. Status e DataCriacao são definidos pelo sistema.
/// </summary>
public class CriarTarefaDto
{
    [Required(ErrorMessage = "O título da tarefa é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? Descricao { get; set; }
}
