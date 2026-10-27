namespace TaskFlow.API.Exceptions;

/// <summary>
/// Lançada quando uma regra de negócio ou validação é violada (resulta em HTTP 400).
/// </summary>
public class RegraNegocioException : Exception
{
    public RegraNegocioException(string mensagem) : base(mensagem)
    {
    }
}
