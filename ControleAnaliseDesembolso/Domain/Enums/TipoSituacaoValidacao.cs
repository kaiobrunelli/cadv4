namespace ControleAnaliseDesembolso.Domain.Enums
{
    // Valores fixos: preservam o que já está gravado em CAD_TB004 (ANALISAR/APROVADO/NEGADO)
    // e reservam códigos novos pro que antes era TipoSituacaoConferencia (OK/ERRO/PENDENTE),
    // agora que as duas situações moram na mesma coluna SITUACAO.
    public enum TipoSituacaoValidacao
    {
        ANALISAR = 0,
        APROVADO = 1,
        NEGADO = 2,
        OK = 3,
        ERRO = 4,
        PENDENTE = 5,
    }
}
