namespace TaskFlow.API.Exceptions;

/// <summary>
/// Lançada quando um recurso solicitado não existe (resulta em HTTP 404).
/// </summary>
public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}
