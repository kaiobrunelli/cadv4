using Microsoft.JSInterop;

namespace PlataformaNotificacao.UI.Servicos;

public class ServicoUsuarioUI
{
    private class UsuarioMock
    {
        public string Nome { get; set; } = "";
        public string Matricula { get; set; } = "";
        public int Unidade { get; set; }
    }

    private readonly UsuarioMock UsuarioTeste = new()
    {
        Nome = "Kaio Brunelli",
        Matricula = "c151896",
        Unidade = 7175,
    };

    private string _matricula;

    public ServicoUsuarioUI()
    {
        _matricula = UsuarioTeste.Matricula;
    }

    public string Matricula => _matricula;

    public string Nome => _matricula == UsuarioTeste.Matricula
        ? UsuarioTeste.Nome
        : _matricula switch
        {
            "c123456" => "Bruno Costa",
            "c123457" => "Carla Mendes",
            "c123000" => "Diego Santos",
            "c123001" => "Elena Ferreira",
            _         => _matricula
        };

    public string Iniciais => _matricula switch
    {
        "c151896" => "KB",
        "c123456" => "BC",
        "c123457" => "CM",
        "c123000" => "DS",
        "c123001" => "EF",
        _         => "?"
    };

    public string Cargo => _matricula switch
    {
        "c151896" => "Analista Sênior",
        "c123456" => "Gestor",
        "c123457" => "Analista Júnior",
        "c123000" => "Coordenador",
        "c123001" => "Diretora Financeira",
        _         => ""
    };

    public int UnidadeUsuario => ModoGigovForcado
        ? CodigoGigovTeste
        : _matricula == UsuarioTeste.Matricula
            ? UsuarioTeste.Unidade
            : _matricula switch
            {
                "c123457" => 7105,
                _         => 7175,
            };

    public bool EhGigov => UnidadeUsuario != 7175;

    public const int CodigoGigovTeste = 1201;
    private const string ChaveModoTeste = "cad_modo_teste_gigov";

    public bool ModoGigovForcado { get; private set; }

    public async Task CarregarModoTesteAsync(IJSRuntime js)
    {
        var valor = await js.InvokeAsync<string?>("localStorage.getItem", ChaveModoTeste);
        ModoGigovForcado = valor == "1";
    }

    public async Task AlternarModoTesteAsync(IJSRuntime js)
    {
        ModoGigovForcado = !ModoGigovForcado;
        await js.InvokeVoidAsync("localStorage.setItem", ChaveModoTeste, ModoGigovForcado ? "1" : "0");
    }

    public string Cor => _matricula switch
    {
        "c151896" => "#005CA9",
        "c123456" => "#065F46",
        "c123457" => "#7C3AED",
        "c123000" => "#B45309",
        "c123001" => "#BE185D",
        _         => "#6B7280"
    };

    public event Func<Task>? AoMudar;

    public async Task MudarParaAsync(string matricula)
    {
        _matricula = matricula;
        if (AoMudar is not null)
            await AoMudar.Invoke();
    }
}
