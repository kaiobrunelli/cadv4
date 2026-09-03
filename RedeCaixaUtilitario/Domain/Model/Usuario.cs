namespace RedeCaixaUtilitario.Domain.Model;

// Versão mínima do Usuario usado pra logar no SIAPF (o original vive em
// Utilitarios.Model no projeto "XP Metodo nvoo/RedeCaixaUtilitario", que não
// está disponível aqui). Matricula/Senha chegam por chamada — quem clicou em
// "Validar" no CAD — em vez de uma conta de serviço fixa.
public class Usuario
{
    public string Matricula { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;

    // Valor colocado no campo de senha da tela de logon do SIAPF. Não há
    // transformação conhecida (o Usuario.Logar() original não estava
    // disponível pra conferir) — aqui é a senha informada, sem alteração.
    public string Logar() => Senha;
}
