using Helpers.RedeCaixa;
using Helpers.RedeCaixa.Enums;
using Helpers.RedeCaixa.RedeCaixaEmulator;
using RedeCaixaUtilitario.Application.Interface;
using RedeCaixaUtilitario.Domain.Exceptions;
using RedeCaixaUtilitario.Domain.Model;
using System.Diagnostics;

namespace RedeCaixaUtilitario.Application;

// Implementação real (não simulada) de ISiapfService — porta só a região
// "Cadastro" (ConsultarCadastroGeral) do projeto original em
// "XP Metodo nvoo/RedeCaixaUtilitario" (Application/SiapfService.cs), mais o
// mínimo de plumbing de terminal 3270 que esse método usa (login, navegação
// de telas, espera de resposta). O ISiapfService real tem muito mais
// métodos (DRP, movimentação financeira, cronograma...) que não são
// necessários aqui — só a consulta usada pra confrontar o FPD do CAD com o
// cadastro do contrato no SIAPF.
public class SiapfService : ISiapfService, IAsyncDisposable
{
    private readonly IRedeCaixa _redeCaixa;
    private Usuario? _usuario;
    private readonly List<string> _telas = new();
    private string _erroRedeCaixa = string.Empty;

    public SiapfService()
    {
        _redeCaixa = new RedeCaixaEmulator();
    }

    public Task<CadastroGeralSiapf> ConsultarCadastroGeralAsync(
        string contrato,
        string contratoDv,
        string matricula,
        string senha,
        CancellationToken cancellationToken = default)
    {
        _usuario = new Usuario { Matricula = matricula, Senha = senha };

        // A navegação de tela é toda síncrona (polling bloqueante no terminal
        // 3270) — roda numa thread separada pra não travar quem chamou.
        return Task.Run(() => ConsultarCadastroGeralInterno(contrato, contratoDv), cancellationToken);
    }

    #region Acesso

    private IRedeCaixa LogarSiapf()
    {
        int tolerancia = 0;
        int toleranciaGeral = 0;
    Inicio:
        if (toleranciaGeral > 30)
        {
            throw new RedeCaixaException("RC01 - Tempo de tolerância excedido. Método: LogarSiapf");
        }

        _redeCaixa.Reconectar();

        while (EstaNaTelaInicial(_redeCaixa))
        {
            if (tolerancia > 60) { toleranciaGeral++; goto Inicio; }
            _redeCaixa.PutString("6", 17, 38);
            _redeCaixa.PutString("5", 17, 41);
            TeclarAguardandoAlteracao(Tecla.Enter);
            Thread.Sleep(100);
            tolerancia++;
        }
        tolerancia = 0;

        while (!_redeCaixa.GetStringArea(2, 1, 80).Contains("6.5")) //Tela de usuário
        {
            if (!_redeCaixa.GetStringArea(16, 1, 80).Contains("SENHA =>") || tolerancia > 60) { toleranciaGeral++; goto Inicio; }

            _redeCaixa.PutString(_usuario!.Matricula, 16, 19);
            _redeCaixa.PutString(_usuario.Logar(), 16, 42);

            TeclarAguardandoAlteracao(Tecla.Enter);

            if (_redeCaixa.GetStringArea(24, 1, 80).Contains("SENHA INVALIDA")) { throw new RedeCaixaException("RC02 - Senha Inválida."); }
            if (_redeCaixa.GetStringArea(24, 1, 80).Contains("INFORME SENHA")) { throw new RedeCaixaException("RC03 - Senha não informada."); }
            if (_redeCaixa.GetStringArea(24, 1, 80).Contains("AGENTE DE RH SE BLOQUEADA")) { throw new RedeCaixaException("RC04 - Sistema já em uso."); }
            if (_redeCaixa.GetStringArea(24, 1, 80).Contains("SIGLA SUSPENSA")) { throw new RedeCaixaException("RC05 - Sigla Suspensa"); }

            tolerancia++;
        }

        return _redeCaixa;
    }

    private void AcessarCer()
    {
        if (!ConferirTela("SSGMS020"))
        {
            LogarSiapf();
        }
        TeclarAguardandoAlteracao(Tecla.Enter);

        if (!ConferirTela("MS001"))
        {
            TeclarAguardandoAlteracao(Tecla.Enter, "MS001");
        }

        _redeCaixa.PutString("4", 17, 50);

        TeclarAguardandoAlteracao(Tecla.Enter);

        //CASO SIAPF ESTEJA INATIVO
        if (_redeCaixa.GetStringArea(1, 1, 80).Trim().Contains("MSGMB003"))
        {
            TeclarAguardandoAlteracao(Tecla.Enter);
            if (_redeCaixa.GetStringArea(24, 1, 80).Trim().Contains("SUBSISTEMA  \" CER \"  INATIVO"))
            {
                throw new RedeCaixaException("RC99 - SIAPF: CER INATIVO");
            }
        }

        if (!ConferirTela("MS020"))
        {
            TeclarAguardandoAlteracao(Tecla.Enter, "APF/CER", "MS020");
        }
    }

    #endregion

    #region Cadastro

    // Porta fiel de RedeCaixaUtilitario.Application.SiapfService.ConsultarCadastroGeral
    // (projeto original, região "Cadastro") — mesma navegação de tela, mesmas
    // coordenadas de captura. `operacao`/`operacaoDv` são o número do
    // contrato consultado.
    private CadastroGeralSiapf ConsultarCadastroGeralInterno(string operacao, string operacaoDv)
    {
        if (!ConferirTela("MS140"))
        {
            if (!ConferirTela("APF/CER", "MS020"))
            {
                AcessarCer();
            }
            _redeCaixa.PutString("01", 4, 13);
            TeclarAguardandoAlteracao(Tecla.Enter, "MB302");
        }

        _redeCaixa.PutString("1", 4, 11);
        _redeCaixa.PutString(operacao, 13, 19);
        _redeCaixa.PutString(operacaoDv, 13, 29);
        TeclarAguardandoAlteracao(Tecla.Enter, "MB010");

        var cadastroGeral = new CadastroGeralSiapf();

        //TELA DADOS GERAIS
        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(4, 17, 23)))
        {
            cadastroGeral.Contrato = _redeCaixa.GetStringArea(4, 17, 23).Trim();
            cadastroGeral.ContratoDv = _redeCaixa.GetStringArea(4, 27, 28).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(4, 44, 50).Trim()))
        {
            cadastroGeral.DadosGerais.ContratoAssociado = _redeCaixa.GetStringArea(4, 44, 50).Trim();
            cadastroGeral.DadosGerais.ContratoAssociadoDv = _redeCaixa.GetStringArea(4, 54, 55).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(4, 69, 75).Trim()))
        {
            cadastroGeral.DadosGerais.ContratoVinculado = _redeCaixa.GetStringArea(4, 69, 75).Trim();
            cadastroGeral.DadosGerais.ContratoVinculadoDv = _redeCaixa.GetStringArea(4, 79, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(2, 70, 75).Trim()))
        {
            cadastroGeral.DadosGerais.ContratoPassivo = _redeCaixa.GetStringArea(2, 70, 75).Trim();
            cadastroGeral.DadosGerais.ContratoPassivoDv = _redeCaixa.GetStringArea(2, 79, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(5, 17, 21).Trim()))
        {
            cadastroGeral.DadosGerais.Tomador = _redeCaixa.GetStringArea(5, 17, 21).Trim();
            cadastroGeral.DadosGerais.TomadorDv = _redeCaixa.GetStringArea(5, 25, 25).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(5, 58, 64)))
        {
            cadastroGeral.DadosGerais.ControleOp = _redeCaixa.GetStringArea(5, 58, 64).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(5, 80, 80).Trim()))
        {
            cadastroGeral.DadosGerais.NaturezaOperacao = _redeCaixa.GetStringArea(5, 80, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(6, 17, 21)))
        {
            cadastroGeral.DadosGerais.CoMutuarioFinal = _redeCaixa.GetStringArea(6, 17, 21).Trim();
            cadastroGeral.DadosGerais.DeMutuarioFinal = _redeCaixa.GetStringArea(6, 62, 62).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(6, 55, 58)))
        {
            cadastroGeral.DadosGerais.Ugc = _redeCaixa.GetStringArea(6, 55, 58).Trim();
            cadastroGeral.DadosGerais.UgcDv = _redeCaixa.GetStringArea(6, 29, 52).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(7, 17, 21)))
        {
            cadastroGeral.DadosGerais.CoAgentePromotorOuParceiro = _redeCaixa.GetStringArea(7, 17, 21).Trim();
            cadastroGeral.DadosGerais.DeAgentePromotorOuParceiro = _redeCaixa.GetStringArea(7, 29, 43).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(7, 55, 58)))
        {
            cadastroGeral.DadosGerais.CoUnidadeMovimento = _redeCaixa.GetStringArea(7, 55, 58).Trim();
            cadastroGeral.DadosGerais.CoUnidadeMovimentoDv = _redeCaixa.GetStringArea(7, 62, 62).Trim();
            cadastroGeral.DadosGerais.DeUnidadeMovimento = _redeCaixa.GetStringArea(7, 66, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(8, 17, 20).Trim()))
        {
            cadastroGeral.DadosGerais.CoOrigemRecursos = _redeCaixa.GetStringArea(8, 17, 20).Trim();
            cadastroGeral.DadosGerais.DeOrigemRecursos = _redeCaixa.GetStringArea(8, 17, 20).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(8, 55, 58)))
        {
            cadastroGeral.DadosGerais.CoGerenciaFilial = _redeCaixa.GetStringArea(8, 55, 58).Trim();
            cadastroGeral.DadosGerais.CoGerenciaFilialDv = _redeCaixa.GetStringArea(8, 62, 62).Trim();
            cadastroGeral.DadosGerais.DeGerenciaFilial = _redeCaixa.GetStringArea(8, 66, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(9, 17, 20).Trim()))
        {
            cadastroGeral.DadosGerais.AnoLimGlob = _redeCaixa.GetStringArea(9, 17, 20).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(9, 55, 58).Trim()))
        {
            cadastroGeral.DadosGerais.CoSr = _redeCaixa.GetStringArea(9, 55, 58).Trim();
            cadastroGeral.DadosGerais.DeSr = _redeCaixa.GetStringArea(9, 66, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(10, 17, 17).Trim()))
        {
            cadastroGeral.DadosGerais.CoObjetivo = _redeCaixa.GetStringArea(10, 17, 34).Trim();
            cadastroGeral.DadosGerais.DeObjetivo = _redeCaixa.GetStringArea(10, 38, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(11, 17, 18).Trim()))
        {
            cadastroGeral.DadosGerais.DescProjeto = _redeCaixa.GetStringArea(11, 17, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(13, 17, 18).Trim()))
        {
            cadastroGeral.DadosGerais.DtAssinatura =
                new DateTime(
                    Convert.ToInt32(_redeCaixa.GetStringArea(13, 27, 30).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(13, 22, 23).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(13, 17, 18).Trim())
                    );
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(13, 43, 44).Trim()))
        {
            cadastroGeral.DadosGerais.DiaEleito = Convert.ToInt32(_redeCaixa.GetStringArea(13, 43, 44).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(13, 59, 59).Trim()))
        {
            cadastroGeral.DadosGerais.FatSAM = Convert.ToDouble(_redeCaixa.GetStringArea(13, 59, 62).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(13, 76, 76).Trim()))
        {
            cadastroGeral.DadosGerais.FatConv = Convert.ToDouble(_redeCaixa.GetStringArea(13, 76, 79).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(14, 17, 43).Trim()))
        {
            cadastroGeral.DadosGerais.VeHistorico = _redeCaixa.GetStringArea(14, 17, 43).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(14, 58, 60).Trim()))
        {
            cadastroGeral.DadosGerais.CoPadraoMonetario = _redeCaixa.GetStringArea(14, 58, 60).Trim();
            cadastroGeral.DadosGerais.DePadraoMonetario = _redeCaixa.GetStringArea(14, 64, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(15, 17, 18).Trim()))
        {
            cadastroGeral.DadosGerais.CoSituacaoCobranca = Convert.ToInt32(_redeCaixa.GetStringArea(15, 17, 18).Trim());
            cadastroGeral.DadosGerais.DeSituacaoCobranca = _redeCaixa.GetStringArea(15, 23, 43).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(15, 58, 59).Trim()))
        {
            cadastroGeral.DadosGerais.CoSituacaoContrato = Convert.ToInt32(_redeCaixa.GetStringArea(15, 58, 59).Trim());
            cadastroGeral.DadosGerais.DeSituacaoContrato = _redeCaixa.GetStringArea(15, 64, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(16, 17, 43).Trim()))
        {
            cadastroGeral.DadosGerais.InstrumentoFormalDeAlteracao = _redeCaixa.GetStringArea(16, 17, 43).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(16, 58, 59).Trim()))
        {
            cadastroGeral.DadosGerais.DtInicEEC =
                new DateTime(
                    Convert.ToInt32(_redeCaixa.GetStringArea(16, 68, 71).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(16, 63, 64).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(16, 58, 59).Trim())
                    );
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(17, 23, 80).Trim()))
        {
            cadastroGeral.DadosGerais.HistoricoAlteracaoCont = _redeCaixa.GetStringArea(17, 23, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(18, 17, 80).Trim()))
        {
            cadastroGeral.DadosGerais.MotivoAlteracao = _redeCaixa.GetStringArea(18, 17, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(19, 17, 18).Trim()))
        {
            cadastroGeral.DadosGerais.TipoGarantia = _redeCaixa.GetStringArea(18, 17, 41).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(19, 56, 60).Trim()))
        {
            cadastroGeral.DadosGerais.Garantidor = _redeCaixa.GetStringArea(19, 56, 60).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(19, 78, 79).Trim()))
        {
            cadastroGeral.DadosGerais.IndiceRiscoDeCredito = _redeCaixa.GetStringArea(19, 78, 79).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(20, 17, 19).Trim()))
        {
            cadastroGeral.DadosGerais.CorreioBacen = _redeCaixa.GetStringArea(20, 17, 41).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(20, 56, 60).Trim()))
        {
            cadastroGeral.DadosGerais.OfStn = _redeCaixa.GetStringArea(20, 56, 60).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(20, 78, 79).Trim()))
        {
            cadastroGeral.DadosGerais.ModBacen = _redeCaixa.GetStringArea(20, 78, 79).Trim();
        }

        TeclarAguardandoAlteracao(Tecla.Pf7, "MB014");

        //PROXIMA PAGINA => F7 DADOS COMP
        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(04, 78, 80).Trim()))
        {
            cadastroGeral.DadosComplementares.Status = _redeCaixa.GetStringArea(04, 78, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(05, 15, 20).Trim()))
        {
            cadastroGeral.DadosComplementares.PvVinculado = _redeCaixa.GetStringArea(05, 15, 20).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(05, 32, 41).Trim()))
        {
            cadastroGeral.DadosComplementares.CoCadip = _redeCaixa.GetStringArea(05, 32, 41).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(05, 57, 60).Trim()))
        {
            cadastroGeral.DadosComplementares.CoRedur = _redeCaixa.GetStringArea(05, 32, 41).Trim();
            cadastroGeral.DadosComplementares.DeRedur = _redeCaixa.GetStringArea(05, 66, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(06, 16, 40).Trim()))
        {
            cadastroGeral.DadosComplementares.IdExterna = _redeCaixa.GetStringArea(06, 16, 40).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(06, 56, 59).Replace('_', ' ').Trim()))
        {
            cadastroGeral.DadosComplementares.CtaCorrente = _redeCaixa.GetStringArea(06, 56, 78).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(07, 28, 32).Trim()))
        {
            cadastroGeral.DadosComplementares.DadosOrcamentariosCta = _redeCaixa.GetStringArea(07, 28, 32).Trim();
            cadastroGeral.DadosComplementares.DadosOrcamentariosCtaDv = _redeCaixa.GetStringArea(07, 36, 37).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(07, 46, 48).Trim()))
        {
            cadastroGeral.DadosComplementares.ResAut = _redeCaixa.GetStringArea(07, 46, 48).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(08, 16, 18).Trim()))
        {
            cadastroGeral.DadosComplementares.AutorizStn = _redeCaixa.GetStringArea(08, 16, 43).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(08, 56, 80).Trim()))
        {
            cadastroGeral.DadosComplementares.CoProduto = _redeCaixa.GetStringArea(08, 56, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(09, 23, 24).Trim()))
        {
            cadastroGeral.DadosComplementares.DadosVendedorCnpj = _redeCaixa.GetStringArea(09, 23, 40).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(09, 56, 57).Trim()))
        {
            cadastroGeral.DadosComplementares.CtaVendedor = _redeCaixa.GetStringArea(09, 56, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(10, 16, 21).Trim()))
        {
            cadastroGeral.DadosComplementares.CoOrgAssessor = _redeCaixa.GetStringArea(10, 16, 26).Trim();
            cadastroGeral.DadosComplementares.DeOrgAssessor = _redeCaixa.GetStringArea(10, 29, 43).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(10, 56, 60).Trim()))
        {
            cadastroGeral.DadosComplementares.CoBenFinal = _redeCaixa.GetStringArea(10, 56, 60).Trim();
            cadastroGeral.DadosComplementares.DeBenFinal = _redeCaixa.GetStringArea(10, 64, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(11, 17, 21).Replace('_', ' ').Trim()))
        {
            cadastroGeral.DadosComplementares.CoCessionario = _redeCaixa.GetStringArea(11, 17, 21).Replace('_', ' ').Trim();
            cadastroGeral.DadosComplementares.CoCessionarioDv = _redeCaixa.GetStringArea(11, 25, 25).Replace('_', ' ').Trim();
            cadastroGeral.DadosComplementares.DeCessionario = _redeCaixa.GetStringArea(11, 29, 80).Replace('_', ' ').Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(12, 21, 24).Replace('_', ' ').Trim()))
        {
            cadastroGeral.DadosComplementares.CtaCorrenteNSGD = _redeCaixa.GetStringArea(12, 21, 52).Trim();
        }

        //Dados de Consulta a Construtora
        var linha = 14;
        while (linha <= 16)
        {
            if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, 2, 6).Trim()))
            {
                var consultaConstrutora = new ConsultaConstrutora
                {
                    CoConstrutora = _redeCaixa.GetStringArea(linha, 2, 6).Trim(),
                    CoConstrutoraDv = _redeCaixa.GetStringArea(linha, 10, 10).Trim(),
                    DeConstrutora = _redeCaixa.GetStringArea(linha, 14, 41).Trim()
                };

                if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, 42, 44).Trim()))
                {
                    consultaConstrutora.DtConceitoConstrutora = new DateTime(
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, 52, 58).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, 48, 49).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, 42, 44).Trim())
                    );
                }
                cadastroGeral.DadosComplementares.ConstrConsult.Add(consultaConstrutora);
            }
            linha++;
        }

        //Dados de Conta Reserva
        linha = 18;
        while (linha <= 20)
        {
            if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, 2, 6).Trim()))
            {
                cadastroGeral.DadosComplementares.ContaReserva.Add(_redeCaixa.GetStringArea(linha, 2, 42).Trim());
            }
            linha++;
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(17, 63, 80).Trim()))
        {
            cadastroGeral.DadosComplementares.CartaConsulta = _redeCaixa.GetStringArea(17, 63, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(18, 63, 80).Trim()))
        {
            cadastroGeral.DadosComplementares.TermoHabilitacao = _redeCaixa.GetStringArea(18, 63, 80).Trim();
        }

        //DADOS DE OBRA F5
        TeclarAguardandoAlteracao(Tecla.Pf3, "MB010");
        TeclarAguardandoAlteracao(Tecla.Pf5, "MB363");

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(3, 53, 54).Replace('_', ' ').Trim()))
        {
            cadastroGeral.DadosObra.DtInicioObra = new DateTime(
            Convert.ToInt32(_redeCaixa.GetStringArea(3, 63, 66).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(3, 58, 59).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(3, 53, 54).Trim())
            );
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(4, 53, 54).Replace('_', ' ').Trim()))
        {
            cadastroGeral.DadosObra.DtTerminoObra = new DateTime(
            Convert.ToInt32(_redeCaixa.GetStringArea(4, 63, 66).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(4, 58, 59).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(4, 53, 54).Trim())
            );
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(5, 53, 54).Replace('_', ' ').Trim()))
        {
            cadastroGeral.DadosObra.DtInauguracao = new DateTime(
            Convert.ToInt32(_redeCaixa.GetStringArea(5, 63, 66).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(5, 58, 59).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(5, 53, 54).Trim())
            );
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(2, 66, 67).Replace('_', ' ').Trim()))
        {
            cadastroGeral.DadosObra.DtInauguracao = new DateTime(
            Convert.ToInt32(_redeCaixa.GetStringArea(2, 76, 79).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(2, 71, 72).Trim()),
            Convert.ToInt32(_redeCaixa.GetStringArea(2, 66, 67).Trim())
            );
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(5, 39, 39).Trim()))
        {
            cadastroGeral.DadosObra.LicitacaoConcluida = _redeCaixa.GetStringArea(5, 39, 39).Trim();
        }

        //Capturar Registros de obra
        linha = 8;
        var colunaInicial = 2;

        while (linha <= 20)
        {
            if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, colunaInicial, colunaInicial + 1).Trim()))
            {
                var registro = new RegistroDeObra
                {
                    AnoMes = new DateTime(
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, colunaInicial + 5, colunaInicial + 8).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, colunaInicial, colunaInicial + 1).Trim()),
                    1)
                };

                registro.Previsao = !string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, colunaInicial + 10, colunaInicial + 15).Trim())
                    ? Convert.ToDouble(_redeCaixa.GetStringArea(linha, colunaInicial + 10, colunaInicial + 15).Trim())
                    : 0;

                registro.Real = !string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, colunaInicial + 16, colunaInicial + 22).Trim())
                    ? Convert.ToDouble(_redeCaixa.GetStringArea(linha, colunaInicial + 16, colunaInicial + 22).Trim())
                    : 0;

                registro.Situacao = _redeCaixa.GetStringArea(linha, colunaInicial + 24, colunaInicial + 24).Trim();

                cadastroGeral.DadosObra.RegistrosDeObra.Add(registro);
            }
            else
            {
                break;
            }

            if (colunaInicial == 43)
            {
                colunaInicial = 2;
                linha++;

                if (linha == 21)
                {
                    linha = 8;
                    colunaInicial = 2;
                    TeclarAguardandoAlteracao(Tecla.Pf8, "MB363");
                    TeclarAguardandoAlteracao(Tecla.Pf7, "MB363");
                    TeclarAguardandoAlteracao(Tecla.Pf8, "MB363");
                }
            }
            else if (colunaInicial == 2)
            {
                colunaInicial = 43;
            }
        }

        //IR PARA A TELA F8 => SELECAO DE ETAPA DE EVOLUCAO CONTRATUAL EEC
        TeclarAguardandoAlteracao(Tecla.Pf3, "MB010");
        TeclarAguardandoAlteracao(Tecla.Pf9, "MS070");

        linha = 7;

        while (linha <= 20)
        {
            if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, 5, 7).Trim()))
            {
                var eec = new SelecaoEtapaEvolucaoContratual
                {
                    CoEEC = Convert.ToInt32(_redeCaixa.GetStringArea(linha, 5, 7).Trim()),
                    DtInicio = new DateTime(
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, 19, 22).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, 14, 15).Trim()),
                    Convert.ToInt32(_redeCaixa.GetStringArea(linha, 9, 10).Trim()))
                };

                if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, 26, 69).Trim()))
                {
                    eec.MotivoDaCriacao = _redeCaixa.GetStringArea(linha, 26, 69).Trim();
                }
                if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(linha, 71, 80).Trim()))
                {
                    eec.Situacao = _redeCaixa.GetStringArea(linha, 71, 80).Trim();
                }

                cadastroGeral.EtapaEvolucaoContratual.Add(eec);
            }
            else
            {
                break;
            }

            linha++;

            if (linha == 21)
            {
                linha = 7;
                TeclarAguardandoAlteracao(Tecla.Pf8, "MS070");
            }
        }

        //IR PARA A TELA F10 => DADOS CADASTRAIS - ACOMPANHAMENTO
        TeclarAguardandoAlteracao(Tecla.Pf3, "MB010");
        TeclarAguardandoAlteracao(Tecla.Pf10, "MB130");

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(6, 24, 25).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.Competencia =
            new DateTime(
                Convert.ToInt32(_redeCaixa.GetStringArea(6, 34, 37).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(6, 29, 30).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(6, 24, 25).Trim()));
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(7, 51, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.SaldoDevedor =
            Convert.ToDouble(_redeCaixa.GetStringArea(7, 51, 80).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(8, 51, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.SaldoCalculaAM =
            Convert.ToDouble(_redeCaixa.GetStringArea(8, 51, 80).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(9, 51, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.SaldoDevedorENC =
            Convert.ToDouble(_redeCaixa.GetStringArea(9, 51, 80).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(12, 24, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.ValTxAdmNumero =
            _redeCaixa.GetStringArea(12, 24, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(17, 70, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.TaxaDeJuros =
            Convert.ToDouble(_redeCaixa.GetStringArea(17, 70, 80).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(14, 24, 26).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.NumeroPrestacao =
            Convert.ToInt32(_redeCaixa.GetStringArea(14, 24, 26).Trim());

            cadastroGeral.DadosAcompanhamento.ValorPrestacao =
            Convert.ToDouble(_redeCaixa.GetStringArea(15, 51, 80).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(18, 70, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.SistemaDeAmortizacao =
           _redeCaixa.GetStringArea(18, 70, 80).Trim();
        }
        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(19, 70, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.PlanoDeReajustamento =
            _redeCaixa.GetStringArea(19, 70, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(20, 70, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.PeriodicidadeCalculo =
            _redeCaixa.GetStringArea(20, 70, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(21, 70, 72).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.Prazo =
            Convert.ToInt32(_redeCaixa.GetStringArea(21, 70, 72).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(16, 24, 80).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.RazaoRecorrencia =
            _redeCaixa.GetStringArea(16, 24, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(17, 24, 25).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.UltimoReajusteSaldoDevedor =
            new DateTime(
                Convert.ToInt32(_redeCaixa.GetStringArea(17, 34, 37).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(17, 29, 30).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(17, 24, 25).Trim()));
        }
        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(18, 24, 25).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.UltimoReajustePrestacao =
            new DateTime(
                Convert.ToInt32(_redeCaixa.GetStringArea(18, 34, 37).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(18, 29, 30).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(18, 24, 25).Trim()));
        }
        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(19, 24, 25).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.UltimaVariacaoTaxaJuros =
            new DateTime(
                Convert.ToInt32(_redeCaixa.GetStringArea(19, 34, 37).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(19, 29, 30).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(19, 24, 25).Trim()));
        }
        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(20, 24, 25).Trim()))
        {
            cadastroGeral.DadosAcompanhamento.UltimoCalculo =
            new DateTime(
                Convert.ToInt32(_redeCaixa.GetStringArea(20, 34, 37).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(20, 29, 30).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(20, 24, 25).Trim()));
        }

        //IR PARA A TELA ENTER => DADOS CADASTRAIS CARENCIA I
        TeclarAguardandoAlteracao(Tecla.Pf3, "MB010");
        TeclarAguardandoAlteracao(Tecla.Enter, "MB020");

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(6, 22, 23).Trim()))
        {
            cadastroGeral.DadosCarencia.DtTermino =
            new DateTime(
                Convert.ToInt32(_redeCaixa.GetStringArea(6, 32, 35).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(6, 27, 28).Trim()),
                Convert.ToInt32(_redeCaixa.GetStringArea(6, 22, 23).Trim()));
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(07, 61, 64).Trim()))
        {
            cadastroGeral.DadosCarencia.CoPeriodicidadeReajusteSaldo = _redeCaixa.GetStringArea(07, 61, 64).Trim();
            cadastroGeral.DadosCarencia.DePeriodicidadeReajusteSaldo = _redeCaixa.GetStringArea(07, 68, 80).Trim();
            cadastroGeral.DadosCarencia.CoEpocaReajusteSaldo = _redeCaixa.GetStringArea(08, 61, 64).Trim();
            cadastroGeral.DadosCarencia.DeEpocaReajusteSaldo = _redeCaixa.GetStringArea(08, 68, 80).Trim();
            cadastroGeral.DadosCarencia.CoRegAtuaCapCop = _redeCaixa.GetStringArea(09, 61, 64).Trim();
            cadastroGeral.DadosCarencia.DeRegAtuaCapCop = _redeCaixa.GetStringArea(09, 68, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(11, 21, 22).Trim()))
        {
            cadastroGeral.DadosCarencia.CoPeriodicidadeCalculo = _redeCaixa.GetStringArea(11, 21, 22).Trim();
            cadastroGeral.DadosCarencia.DePeriodicidadeCalculo = _redeCaixa.GetStringArea(11, 26, 40).Trim();
            cadastroGeral.DadosCarencia.CoEpocaCalculo = _redeCaixa.GetStringArea(12, 21, 22).Trim();
            cadastroGeral.DadosCarencia.DeEpocaCalculo = _redeCaixa.GetStringArea(12, 26, 40).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(11, 61, 70).Trim()))
        {
            cadastroGeral.DadosCarencia.TxDeJuros = Convert.ToDouble(_redeCaixa.GetStringArea(11, 61, 70).Trim());
            cadastroGeral.DadosCarencia.Impontualidade = Convert.ToDouble(_redeCaixa.GetStringArea(12, 61, 70).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(15, 21, 21).Trim()))
        {
            cadastroGeral.DadosCarencia.CoJurosRegime = _redeCaixa.GetStringArea(15, 21, 21).Trim();
            cadastroGeral.DadosCarencia.DeJurosRegime = _redeCaixa.GetStringArea(15, 25, 40).Trim();
            cadastroGeral.DadosCarencia.CoSeguroRegime = _redeCaixa.GetStringArea(16, 21, 22).Trim();
            cadastroGeral.DadosCarencia.DeSeguroRegime = _redeCaixa.GetStringArea(16, 26, 40).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(15, 62, 64).Trim()))
        {
            cadastroGeral.DadosCarencia.CoAtualizaEncargos = _redeCaixa.GetStringArea(15, 62, 64).Trim();
            cadastroGeral.DadosCarencia.DeAtualizaEncargos = _redeCaixa.GetStringArea(15, 68, 80).Trim();

            cadastroGeral.DadosCarencia.CoJurosRemuneratorio = _redeCaixa.GetStringArea(16, 62, 64).Trim();
            cadastroGeral.DadosCarencia.DeJurosRemuneratorio = _redeCaixa.GetStringArea(16, 68, 80).Trim();

            cadastroGeral.DadosCarencia.CoCritTxAdministracao = _redeCaixa.GetStringArea(17, 62, 64).Trim();
            cadastroGeral.DadosCarencia.DeCritTxAdministracao = _redeCaixa.GetStringArea(17, 68, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(19, 21, 22).Trim()))
        {
            cadastroGeral.DadosCarencia.CoPeriodicidadeTxAdministrativa = _redeCaixa.GetStringArea(19, 21, 22).Trim();
            cadastroGeral.DadosCarencia.DePeriodicidadeTxAdministrativa = _redeCaixa.GetStringArea(19, 26, 40).Trim();
            cadastroGeral.DadosCarencia.CoEpocaTxAdministrativa = _redeCaixa.GetStringArea(20, 21, 22).Trim();
            cadastroGeral.DadosCarencia.DeEpocaTxAdministrativa = _redeCaixa.GetStringArea(20, 26, 40).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(18, 62, 72).Trim()))
        {
            cadastroGeral.DadosCarencia.TxAdministracao = Convert.ToDouble(_redeCaixa.GetStringArea(18, 62, 72).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(19, 66, 80).Trim()))
        {
            cadastroGeral.DadosCarencia.CoTxAcordoBID = _redeCaixa.GetStringArea(19, 66, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(20, 66, 80).Trim()))
        {
            cadastroGeral.DadosCarencia.IndiceProrrogTxAdm = _redeCaixa.GetStringArea(20, 66, 80).Trim();
        }

        //IR PARA A TELA ENTERx2 => DADOS CADASTRAIS RETORNO I
        TeclarAguardandoAlteracao(Tecla.Enter, "MB030");

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(6, 21, 25).Trim()))
        {
            cadastroGeral.DadosRetorno.PrazoRetorno = Convert.ToInt32(_redeCaixa.GetStringArea(6, 21, 25).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(07, 61, 64).Trim()))
        {
            cadastroGeral.DadosRetorno.CoPeriodicidadeReajusteSaldo = _redeCaixa.GetStringArea(07, 61, 64).Trim();
            cadastroGeral.DadosRetorno.DePeriodicidadeReajusteSaldo = _redeCaixa.GetStringArea(07, 68, 80).Trim();
            cadastroGeral.DadosRetorno.CoEpocaReajusteSaldo = _redeCaixa.GetStringArea(08, 61, 64).Trim();
            cadastroGeral.DadosRetorno.DeEpocaReajusteSaldo = _redeCaixa.GetStringArea(08, 68, 80).Trim();
            cadastroGeral.DadosRetorno.CoRegAtuaCapCop = _redeCaixa.GetStringArea(09, 61, 64).Trim();
            cadastroGeral.DadosRetorno.DeRegAtuaCapCop = _redeCaixa.GetStringArea(09, 68, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(11, 21, 22).Trim()))
        {
            cadastroGeral.DadosRetorno.CoPeriodicidadeCalculo = _redeCaixa.GetStringArea(11, 21, 22).Trim();
            cadastroGeral.DadosRetorno.DePeriodicidadeCalculo = _redeCaixa.GetStringArea(11, 26, 40).Trim();
            cadastroGeral.DadosRetorno.CoEpocaCalculo = _redeCaixa.GetStringArea(12, 21, 22).Trim();
            cadastroGeral.DadosRetorno.DeEpocaCalculo = _redeCaixa.GetStringArea(12, 26, 40).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(11, 61, 70).Trim()))
        {
            cadastroGeral.DadosRetorno.TxDeJuros = Convert.ToDouble(_redeCaixa.GetStringArea(11, 61, 70).Trim());
            cadastroGeral.DadosRetorno.Impontualidade = Convert.ToDouble(_redeCaixa.GetStringArea(12, 61, 70).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(14, 20, 24).Trim()))
        {
            cadastroGeral.DadosRetorno.CoPeriodoReajustePrestacao = _redeCaixa.GetStringArea(14, 20, 24).Trim();
            cadastroGeral.DadosRetorno.DePeriodoReajustePrestacao = _redeCaixa.GetStringArea(14, 28, 40).Trim();
            cadastroGeral.DadosRetorno.CoEpocaReajustePrestacao = _redeCaixa.GetStringArea(15, 20, 24).Trim();
            cadastroGeral.DadosRetorno.DeEpocaReajustePrestacao = _redeCaixa.GetStringArea(15, 28, 40).Trim();

            cadastroGeral.DadosRetorno.CoSistemaAmortizacao = _redeCaixa.GetStringArea(16, 20, 24).Trim();
            cadastroGeral.DadosRetorno.DeSistemaAmortizacao = _redeCaixa.GetStringArea(16, 28, 40).Trim();

            cadastroGeral.DadosRetorno.CoPlanoReajustePrestacao = _redeCaixa.GetStringArea(17, 20, 24).Trim();
            cadastroGeral.DadosRetorno.DePlanoReajustePrestacao = _redeCaixa.GetStringArea(17, 28, 40).Trim();

            cadastroGeral.DadosRetorno.CoCategoriaReajustePrestacao = _redeCaixa.GetStringArea(18, 20, 24).Trim();
            cadastroGeral.DadosRetorno.DeCategoriaReajustePrestacao = _redeCaixa.GetStringArea(18, 28, 40).Trim();

            cadastroGeral.DadosRetorno.CesFeqPrestacao = Convert.ToDouble(_redeCaixa.GetStringArea(19, 20, 29).Trim());
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(14, 62, 64).Trim()))
        {
            cadastroGeral.DadosRetorno.CoAtualizaEncargos = _redeCaixa.GetStringArea(14, 62, 64).Trim();
            cadastroGeral.DadosRetorno.DeAtualizaEncargos = _redeCaixa.GetStringArea(14, 68, 80).Trim();

            cadastroGeral.DadosRetorno.CoJurosRemuneratorio = _redeCaixa.GetStringArea(15, 62, 64).Trim();
            cadastroGeral.DadosRetorno.DeJurosRemuneratorio = _redeCaixa.GetStringArea(15, 68, 80).Trim();
        }

        if (!string.IsNullOrEmpty(_redeCaixa.GetStringArea(18, 61, 62).Trim()))
        {
            cadastroGeral.DadosRetorno.CoPeriodicidadeTxAdministrativa = _redeCaixa.GetStringArea(18, 61, 62).Trim();
            cadastroGeral.DadosRetorno.CoPeriodicidadeTxAdministrativa = _redeCaixa.GetStringArea(18, 66, 80).Trim();

            cadastroGeral.DadosRetorno.CoEpocaTxAdministrativa = _redeCaixa.GetStringArea(19, 61, 62).Trim();
            cadastroGeral.DadosRetorno.DeEpocaTxAdministrativa = _redeCaixa.GetStringArea(19, 66, 80).Trim();
        }

        _redeCaixa.Reconectar();

        return cadastroGeral;
    }

    #endregion

    #region Utilitários

    private void TeclarAguardandoAlteracao(
        Tecla tecla, string? nomeTela = null, string? nomeTela2 = null, int segundos = 30, int minutos = 0, int linhaParaVerificar = 0)
    {
        var telaAntes = _redeCaixa.ObterTelaInteira();
        RegistrarTela();
        var descTelaAntes = _redeCaixa.GetStringArea(1, 1, 80);

        _redeCaixa.Teclar(tecla);

        var cronometro = new Stopwatch();
        cronometro.Start();
        var timeLimite = new TimeSpan(0, minutos, segundos);

        do
        {
            if (cronometro.Elapsed >= timeLimite)
            {
                RegistrarTela();
                _redeCaixa.Reconectar();
                throw new RedeCaixaException("RC29 - Tempo de tolerância excedido. Método: TeclarAguardandoAlteracao");
            }

            if (_redeCaixa.GetStringArea(24, 1, 80).ToUpper().Contains("ERROR"))
            {
                RegistrarTela();
                throw new RedeCaixaException($"RC30 - RedeCaixa Erro: {_redeCaixa.GetStringArea(24, 1, 80).Trim()}");
            }
            if (_redeCaixa.GetStringArea(1, 1, 80).Contains("FROM IDMS31:"))
            {
                _redeCaixa.Teclar(Tecla.Pf3);
                _redeCaixa.Teclar(tecla);
            }
        } while (telaAntes == _redeCaixa.ObterTelaInteira()
        || _redeCaixa.ObterTelaInteira().Length == 0
        || _redeCaixa.GetStringArea(1, 1, 80).ToUpper().Contains("AGUARDE"));

        var tela = _redeCaixa.ObterTelaInteira();
        RegistrarTela();
        var descTelaDepois = _redeCaixa.GetStringArea(1, 1, 80);
        if (
            tela.Contains(@"ERROR OCCURRED IN DIALOG") ||
            tela.Contains(@"HIT ENTER TO RETURN TO DC OR ENTER NEXT TASK CODE") ||
            tela.Contains(@"PREVIOUS TASK ABENDED WITH ABEND CODE")
            )
        {
            RegistrarTela();
            throw new RedeCaixaException("RC31 - O último comando gerou um Abend.");
        }

        if (linhaParaVerificar != 0 && descTelaAntes == descTelaDepois)
        {
            var linha = _redeCaixa.GetStringArea(linhaParaVerificar, 1, 80).Trim();
            if (!string.IsNullOrEmpty(linha))
            {
                RegistrarTela();
                throw new RedeCaixaException($"RC32 - Erro na tela {nomeTela} : {linha} ");
            }
        }

        if (nomeTela is not null && !ConferirTela(nomeTela, nomeTela2))
        {
            RegistrarTela();
            throw new RedeCaixaException("RC33 - Tela inesperada");
        }

        RegistrarTela();
    }

    private bool ConferirTela(string nomeDaTela, string? segundoNome = null, int linha = 1)
    {
        if (segundoNome is null)
        {
            if (_redeCaixa.GetStringArea(linha, 1, 80).ToUpper().Contains(nomeDaTela))
            {
                return true;
            }
        }
        else
        {
            if (_redeCaixa.GetStringArea(linha, 1, 80).ToUpper().Contains(nomeDaTela) &&
                _redeCaixa.GetStringArea(linha, 1, 80).ToUpper().Contains(segundoNome))
            {
                return true;
            }
        }
        return false;
    }

    private static bool EstaNaTelaInicial(IRedeCaixa redeCaixa)
    {
        return redeCaixa.GetStringArea(17, 1, 80).Contains("SELECIONE");
    }

    private void RegistrarTela()
    {
        if (_telas.Count > 10)
        {
            _telas.RemoveAt(0);
        }
        _telas.Add($"REGISTRO: {DateTime.Now}\r\n{_redeCaixa.ObterTelaInteira()}");
        _erroRedeCaixa = _redeCaixa.GetStringArea(24, 1, 80).Trim();
    }

    #endregion

    public ValueTask DisposeAsync()
    {
        try
        {
            RegistrarTela();
            _redeCaixa.Reconectar();
        }
        catch
        {
            // Encerramento de sessão best-effort — não pode derrubar a resposta
            // da consulta por conta de erro no fechamento da sessão do terminal.
        }
        return ValueTask.CompletedTask;
    }
}
