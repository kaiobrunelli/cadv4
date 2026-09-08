namespace ControleAnaliseDesembolso.Domain.Enums
{
    // De onde vem o resultado de uma linha de ValidacaoControleDesembolso:
    // Manual/AutomaticaCampo nascem na criação da FPD e são mutadas em lugar
    // (uma linha por item, sempre SIM/NÃO/NSA); ConferenciaSiapf nasce quando a
    // macro de conferência roda e cada execução INSERE uma linha nova
    // (histórico), nunca sobrescreve — ver ExecutarConferenciaCamposInterno.
    //
    // ConferenciaLocal (=2) existiu, mas foi removida: validações de campo
    // simples (CNPJ, matrícula do Gestor, Programa, Agente Financeiro/Tomador)
    // viraram checagem síncrona na criação/reenvio da FPD (ver
    // ValidarDadosObrigatoriosFpd em ControleAnaliseDesembolsoService), em vez
    // de item de checklist avaliado depois. O valor 2 fica reservado (não
    // reaproveitar) porque pode existir em CAD_TB003/CAD_TB004 histórico.
    public enum TipoOrigemValidacao
    {
        Manual = 0,
        AutomaticaCampo = 1,
        ConferenciaSiapf = 3,
    }
}
