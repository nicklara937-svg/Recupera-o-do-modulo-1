using TaskFlow.API.Exceptions;
using TaskFlow.API.Models.DTOs;

namespace TaskFlow.API.Middlewares;

/// <summary>
/// Middleware global de tratamento de erros (RNF03).
/// Captura qualquer exceção não tratada e devolve um JSON padronizado,
/// sem expor stack trace ou detalhes internos ao cliente.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception excecao)
        {
            await TratarExcecaoAsync(context, excecao);
        }
    }

    private async Task TratarExcecaoAsync(HttpContext context, Exception excecao)
    {
        var (status, mensagem) = excecao switch
        {
            NaoEncontradoException => (StatusCodes.Status404NotFound, excecao.Message),
            RegraNegocioException => (StatusCodes.Status400BadRequest, excecao.Message),
            _ => (StatusCodes.Status500InternalServerError,
                  "Ocorreu um erro interno ao processar a requisição. Tente novamente mais tarde.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            // O stack trace fica apenas no log do servidor, nunca na resposta
            _logger.LogError(excecao, "Erro não tratado em {Metodo} {Caminho}",
                context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning("Requisição rejeitada em {Metodo} {Caminho}: {Mensagem}",
                context.Request.Method, context.Request.Path, mensagem);
        }

        if (context.Response.HasStarted)
        {
            _logger.LogWarning("A resposta já havia sido iniciada; não foi possível escrever o erro padronizado.");
            return;
        }

        context.Response.StatusCode = status;

        var erro = new ErroResponse(
            status,
            mensagem,
            null,
            context.Request.Path.ToString(),
            DateTime.UtcNow);

        await context.Response.WriteAsJsonAsync(erro);
    }
}
