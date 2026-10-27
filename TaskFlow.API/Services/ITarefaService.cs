using TaskFlow.API.Models.DTOs;

namespace TaskFlow.API.Services;

public interface ITarefaService
{
    Task<IEnumerable<TarefaResponseDto>> ListarAsync();
    Task<TarefaResponseDto> ObterPorIdAsync(int id);
    Task<TarefaResponseDto> CriarAsync(CriarTarefaDto dto);
    Task<TarefaResponseDto> AtualizarAsync(int id, AtualizarTarefaDto dto);
    Task ExcluirAsync(int id);
    Task<TarefaResponseDto> IniciarAsync(int id);
    Task<TarefaResponseDto> ConcluirAsync(int id);
}
