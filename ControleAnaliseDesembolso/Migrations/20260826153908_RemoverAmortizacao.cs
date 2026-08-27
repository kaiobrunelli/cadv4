using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    /// <inheritdoc />
    public partial class RemoverAmortizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AMORTIZACAO",
                table: "CAD_TB001_DESEMBOLSO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AMORTIZACAO",
                table: "CAD_TB001_DESEMBOLSO",
                type: "bit",
                nullable: true);
        }
    }
}
