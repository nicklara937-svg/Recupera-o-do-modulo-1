using TaskFlow.API.Models.Entities;
using TaskFlow.API.Models.Enums;

namespace TaskFlow.API.Repositories;

public interface ITarefaRepository
{
    Task<IEnumerable<Tarefa>> ListarAsync(StatusTarefa? status = null);
    Task<Tarefa?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Tarefa tarefa);
    Task AtualizarAsync(Tarefa tarefa);
    Task RemoverAsync(Tarefa tarefa);
}
