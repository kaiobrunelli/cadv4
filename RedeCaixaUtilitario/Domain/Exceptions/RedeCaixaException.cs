namespace RedeCaixaUtilitario.Domain.Exceptions;

public class RedeCaixaException : Exception
{
    public RedeCaixaException(string mensagem) : base(mensagem)
    {
    }
}
