namespace RedeCaixaUtilitario.Domain.Model;

public class Usuario
{
    public string Matricula { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    public string Logar() => Senha;
}
