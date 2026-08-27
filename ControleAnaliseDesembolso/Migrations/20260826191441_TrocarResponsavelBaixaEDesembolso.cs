using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    /// <inheritdoc />
    public partial class TrocarResponsavelBaixaEDesembolso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Troca cruzada de significado (não um simples rename):
            // RESPONSAVEL_DESEMBOLSO (antigo: quem foi designado na aprovação) -> RESPONSAVEL_BAIXA
            // MATRICULA_BAIXA        (antigo: quem confirmou a baixa)          -> RESPONSAVEL_DESEMBOLSO
            // Passa por um nome temporário pra não perder dado na colisão dos dois nomes.
            migrationBuilder.RenameColumn(
                name: "RESPONSAVEL_DESEMBOLSO",
                table: "CAD_TB002_CONTROLE_DESEMBOLSO",
                newName: "RESPONSAVEL_DESEMBOLSO_TMP");

            migrationBuilder.RenameColumn(
                name: "MATRICULA_BAIXA",
                table: "CAD_TB002_CONTROLE_DESEMBOLSO",
                newName: "RESPONSAVEL_DESEMBOLSO");

            migrationBuilder.RenameColumn(
                name: "RESPONSAVEL_DESEMBOLSO_TMP",
                table: "CAD_TB002_CONTROLE_DESEMBOLSO",
                newName: "RESPONSAVEL_BAIXA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RESPONSAVEL_BAIXA",
                table: "CAD_TB002_CONTROLE_DESEMBOLSO",
                newName: "RESPONSAVEL_DESEMBOLSO_TMP");

            migrationBuilder.RenameColumn(
                name: "RESPONSAVEL_DESEMBOLSO",
                table: "CAD_TB002_CONTROLE_DESEMBOLSO",
                newName: "MATRICULA_BAIXA");

            migrationBuilder.RenameColumn(
                name: "RESPONSAVEL_DESEMBOLSO_TMP",
                table: "CAD_TB002_CONTROLE_DESEMBOLSO",
                newName: "RESPONSAVEL_DESEMBOLSO");
        }
    }
}
