namespace TaskFlow.API.Models.Enums;

/// <summary>
/// Ciclo de vida de uma tarefa: Pendente -> EmAndamento -> Concluida.
/// </summary>
public enum StatusTarefa
{
    Pendente = 0,
    EmAndamento = 1,
    Concluida = 2
}
