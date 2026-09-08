namespace ControleAnaliseDesembolso.Domain.Entitys;

// Matrículas com permissão total pra vincular/remover QUALQUER responsável
// pela análise em QUALQUER desembolso (hoje: gestor, supervisor e o sênior
// João — apesar de o cargo dele ser comum, entra na lista pela mesma razão
// que gestor/supervisor). Fora dessa lista, cada analista só mexe na
// própria atribuição — ver VincularResponsavel em ControleAnaliseDesembolsoService.
//
// Editada direto no banco (INSERT/DELETE), sem precisar de deploy — não tem
// tela nem endpoint pra gerenciar isso hoje, de propósito.
public class AutorizadoResponsavelAnalise
{
    public string Matricula { get; set; } = string.Empty;

    public string? Nome { get; set; }

    public DateTime DtInclusao { get; set; }
}
