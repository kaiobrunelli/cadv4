using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    /// <inheritdoc />
    public partial class RemapearStatusDesembolso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alinha a numeração do enum TipoStatusDesembolso com a BranchRefatorada:
            // PENDENTE=1, ANALISAR=2, DESEMBOLSAR=3, NEGAR=4, FINALIZAR=5
            // (antes: Pendente=0, Analisar=1, Desembolsar=2, Finalizar=3, Negar=4)
            migrationBuilder.Sql(@"
                UPDATE CAD_TB002_CONTROLE_DESEMBOLSO
                SET CO_STATUS_DESEMBOLSO = CASE CO_STATUS_DESEMBOLSO
                    WHEN 0 THEN 1
                    WHEN 1 THEN 2
                    WHEN 2 THEN 3
                    WHEN 3 THEN 5
                    WHEN 4 THEN 4
                    ELSE CO_STATUS_DESEMBOLSO
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE CAD_TB002_CONTROLE_DESEMBOLSO
                SET CO_STATUS_DESEMBOLSO = CASE CO_STATUS_DESEMBOLSO
                    WHEN 1 THEN 0
                    WHEN 2 THEN 1
                    WHEN 3 THEN 2
                    WHEN 5 THEN 3
                    WHEN 4 THEN 4
                    ELSE CO_STATUS_DESEMBOLSO
                END");
        }
    }
}
