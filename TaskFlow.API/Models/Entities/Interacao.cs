namespace TaskFlow.API.Models.Entities;

public class Interacao
{
    public int Id { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }

    public int ChamadoId { get; set; }
    public Chamado? Chamado { get; set; }
}
