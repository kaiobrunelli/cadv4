using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    public partial class RenomearTrilhaAuditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TB001_TRILHA_AUDITORIA",
                table: "TB001_TRILHA_AUDITORIA");

            migrationBuilder.RenameTable(
                name: "TB001_TRILHA_AUDITORIA",
                newName: "CAD_TB000_TRILHA_AUDITORIA");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CAD_TB000_TRILHA_AUDITORIA",
                table: "CAD_TB000_TRILHA_AUDITORIA",
                column: "ID_SOLICITACAO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CAD_TB000_TRILHA_AUDITORIA",
                table: "CAD_TB000_TRILHA_AUDITORIA");

            migrationBuilder.RenameTable(
                name: "CAD_TB000_TRILHA_AUDITORIA",
                newName: "TB001_TRILHA_AUDITORIA");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TB001_TRILHA_AUDITORIA",
                table: "TB001_TRILHA_AUDITORIA",
                column: "ID_SOLICITACAO");
        }
    }
}
