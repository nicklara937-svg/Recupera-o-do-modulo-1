namespace TaskFlow.API.Models.Entities;

public class Chamado
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; }

    // N:1 -> cada Chamado pertence a uma Tarefa
    public int TarefaId { get; set; }
    public Tarefa? Tarefa { get; set; }

    // N:1 opcional -> cada Chamado pode ter uma Categoria
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    // 1:N -> um Chamado possui várias Interações
    public ICollection<Interacao> Interacoes { get; set; } = new List<Interacao>();
}
