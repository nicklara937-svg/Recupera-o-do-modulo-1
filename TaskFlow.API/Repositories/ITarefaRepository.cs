using TaskFlow.API.Models.Entities;

namespace TaskFlow.API.Repositories;

public interface ITarefaRepository
{
    Task<IEnumerable<Tarefa>> ListarAsync();
    Task<Tarefa?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Tarefa tarefa);
    Task AtualizarAsync(Tarefa tarefa);
    Task RemoverAsync(Tarefa tarefa);
}
