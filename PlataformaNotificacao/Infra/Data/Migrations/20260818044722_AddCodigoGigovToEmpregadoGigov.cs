using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaNotificacao.Infra.Data.Migrations
{
    public partial class AddCodigoGigovToEmpregadoGigov : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CO_GIGOV",
                table: "PLA_NOT_TB003_EMPREGADOS_GIGOV",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CO_GIGOV",
                table: "PLA_NOT_TB003_EMPREGADOS_GIGOV");
        }
    }
}
