using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Models.DTOs;
using TaskFlow.API.Services;

namespace TaskFlow.API.Controllers;

/// <summary>
/// Camada HTTP: recebe a requisição, delega ao Service e devolve o Status Code adequado.
/// </summary>
[ApiController]
[Route("api/tarefas")]
[Produces("application/json")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaService _service;

    public TarefasController(ITarefaService service)
    {
        _service = service;
    }

    /// <summary>Lista todas as tarefas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TarefaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TarefaResponseDto>>> Listar()
    {
        var tarefas = await _service.ListarAsync();
        return Ok(tarefas);
    }

    /// <summary>Consulta uma tarefa pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponseDto>> ObterPorId(int id)
    {
        var tarefa = await _service.ObterPorIdAsync(id);
        return Ok(tarefa);
    }

    /// <summary>Cria uma nova tarefa (status inicial: Pendente).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TarefaResponseDto>> Criar([FromBody] CriarTarefaDto dto)
    {
        var tarefa = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
    }

    /// <summary>Atualiza título e descrição de uma tarefa.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TarefaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponseDto>> Atualizar(int id, [FromBody] AtualizarTarefaDto dto)
    {
        var tarefa = await _service.AtualizarAsync(id, dto);
        return Ok(tarefa);
    }

    /// <summary>Exclui uma tarefa.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErroResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id)
    {
        await _service.ExcluirAsync(id);
        return NoContent();
    }
}
