using TaskFlow.API.Exceptions;
using TaskFlow.API.Models.DTOs;
using TaskFlow.API.Models.Entities;
using TaskFlow.API.Models.Enums;
using TaskFlow.API.Repositories;

namespace TaskFlow.API.Services;

/// <summary>
/// Camada de regras de negócio: valida dados, controla o ciclo de vida da tarefa
/// e coordena as operações antes de acessar o banco (via Repository).
/// </summary>
public class TarefaService : ITarefaService
{
    private const int TituloMaxLength = 200;
    private const int DescricaoMaxLength = 1000;

    private readonly ITarefaRepository _repository;

    public TarefaService(ITarefaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TarefaResponseDto>> ListarAsync()
    {
        var tarefas = await _repository.ListarAsync();
        return tarefas.Select(ToDto);
    }

    public async Task<TarefaResponseDto> ObterPorIdAsync(int id)
    {
        var tarefa = await BuscarOuFalharAsync(id);
        return ToDto(tarefa);
    }

    public async Task<TarefaResponseDto> CriarAsync(CriarTarefaDto dto)
    {
        ValidarDados(dto.Titulo, dto.Descricao);

        var tarefa = new Tarefa
        {
            Titulo = dto.Titulo.Trim(),
            Descricao = string.IsNullOrWhiteSpace(dto.Descricao) ? null : dto.Descricao.Trim(),
            Status = StatusTarefa.Pendente,   // toda tarefa nasce Pendente
            DataCriacao = DateTime.UtcNow     // preenchida automaticamente
        };

        await _repository.AdicionarAsync(tarefa);
        return ToDto(tarefa);
    }

    public async Task<TarefaResponseDto> AtualizarAsync(int id, AtualizarTarefaDto dto)
    {
        ValidarDados(dto.Titulo, dto.Descricao);

        var tarefa = await BuscarOuFalharAsync(id);

        // Apenas Titulo e Descricao podem ser alterados manualmente
        tarefa.Titulo = dto.Titulo.Trim();
        tarefa.Descricao = string.IsNullOrWhiteSpace(dto.Descricao) ? null : dto.Descricao.Trim();

        await _repository.AtualizarAsync(tarefa);
        return ToDto(tarefa);
    }

    public async Task ExcluirAsync(int id)
    {
        var tarefa = await BuscarOuFalharAsync(id);
        await _repository.RemoverAsync(tarefa);
    }

    public async Task<TarefaResponseDto> IniciarAsync(int id)
    {
        var tarefa = await BuscarOuFalharAsync(id);

        // Regra de negócio (RF06): somente Pendente -> EmAndamento
        if (tarefa.Status != StatusTarefa.Pendente)
        {
            throw new RegraNegocioException(
                $"Não é possível iniciar a tarefa {id}: o status atual é '{tarefa.Status}'. Apenas tarefas Pendentes podem ser iniciadas.");
        }

        tarefa.Status = StatusTarefa.EmAndamento;

        await _repository.AtualizarAsync(tarefa);
        return ToDto(tarefa);
    }

    //__TRANSICOES__

    private async Task<Tarefa> BuscarOuFalharAsync(int id)
    {
        var tarefa = await _repository.ObterPorIdAsync(id);
        return tarefa ?? throw new NaoEncontradoException($"Tarefa com Id {id} não foi encontrada.");
    }

    private static void ValidarDados(string? titulo, string? descricao)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new RegraNegocioException("O título da tarefa é obrigatório.");
        }

        if (titulo.Trim().Length > TituloMaxLength)
        {
            throw new RegraNegocioException($"O título deve ter no máximo {TituloMaxLength} caracteres.");
        }

        if (descricao is not null && descricao.Trim().Length > DescricaoMaxLength)
        {
            throw new RegraNegocioException($"A descrição deve ter no máximo {DescricaoMaxLength} caracteres.");
        }
    }

    private static DateTime AsUtc(DateTime data) => DateTime.SpecifyKind(data, DateTimeKind.Utc);

    private static TarefaResponseDto ToDto(Tarefa tarefa) => new(
        tarefa.Id,
        tarefa.Titulo,
        tarefa.Descricao,
        tarefa.Status,
        AsUtc(tarefa.DataCriacao),
        tarefa.DataConclusao.HasValue ? AsUtc(tarefa.DataConclusao.Value) : (DateTime?)null);
}
