using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Data;
using TaskFlow.API.Models.Entities;
using TaskFlow.API.Models.Enums;

namespace TaskFlow.API.Repositories;

/// <summary>
/// Camada de acesso a dados: conversa exclusivamente com o banco via EF Core.
/// Nenhuma regra de negócio deve ser implementada aqui.
/// </summary>
public class TarefaRepository : ITarefaRepository
{
    private readonly AppDbContext _context;

    public TarefaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tarefa>> ListarAsync(StatusTarefa? status = null)
    {
        var consulta = _context.Tarefas.AsNoTracking();

        // Filtro opcional por status (RF08) executado no banco (WHERE)
        if (status.HasValue)
        {
            consulta = consulta.Where(t => t.Status == status.Value);
        }

        return await consulta
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task<Tarefa?> ObterPorIdAsync(int id)
    {
        return await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AdicionarAsync(Tarefa tarefa)
    {
        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Tarefa tarefa)
    {
        _context.Tarefas.Update(tarefa);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Tarefa tarefa)
    {
        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();
    }
}
