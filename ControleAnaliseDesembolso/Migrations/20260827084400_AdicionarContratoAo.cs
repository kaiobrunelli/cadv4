using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarContratoAo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CO_CONTRATO_AO",
                table: "CAD_TB001_DESEMBOLSO",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CO_CONTRATO_AO_DV",
                table: "CAD_TB001_DESEMBOLSO",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CO_CONTRATO_AO",
                table: "CAD_TB001_DESEMBOLSO");

            migrationBuilder.DropColumn(
                name: "CO_CONTRATO_AO_DV",
                table: "CAD_TB001_DESEMBOLSO");
        }
    }
}
