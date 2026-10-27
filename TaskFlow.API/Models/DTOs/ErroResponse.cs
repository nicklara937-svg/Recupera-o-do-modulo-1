using System.Text.Json.Serialization;

namespace TaskFlow.API.Models.DTOs;

/// <summary>
/// Formato padronizado de resposta para qualquer erro retornado pela API.
/// </summary>
public record ErroResponse(
    int Status,
    string Mensagem,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IEnumerable<string>? Detalhes,
    string Caminho,
    DateTime Timestamp);
