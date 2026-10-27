using TaskFlow.API.Models.Enums;

namespace TaskFlow.API.Models.Entities;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataConclusao { get; set; }

    // Relacionamento 1:N -> uma Tarefa possui vários Chamados
    public ICollection<Chamado> Chamados { get; set; } = new List<Chamado>();
}
