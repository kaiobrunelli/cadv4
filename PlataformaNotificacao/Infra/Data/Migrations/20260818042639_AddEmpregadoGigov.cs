using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaNotificacao.Infra.Data.Migrations
{
    public partial class AddEmpregadoGigov : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PLA_NOT_TB003_EMPREGADOS_GIGOV",
                columns: table => new
                {
                    CO_EMPREGADO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MATRICULA = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    NOME = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ATIVO = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PLA_NOT_TB003_EMPREGADOS_GIGOV", x => x.CO_EMPREGADO);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PLA_NOT_TB003_EMPREGADOS_GIGOV");
        }
    }
}
