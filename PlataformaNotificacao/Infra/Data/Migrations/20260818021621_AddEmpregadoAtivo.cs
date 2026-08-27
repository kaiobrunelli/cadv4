using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaNotificacao.Infra.Data.Migrations
{
    public partial class AddEmpregadoAtivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RH_TB003_EMPREGADOS_ATIVOS",
                columns: table => new
                {
                    CO_EMPREGADO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MATRICULA = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    MATRICULA_DV = table.Column<int>(type: "int", nullable: true),
                    NOME = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DATA_ADMISSAO = table.Column<DateTime>(type: "date", nullable: true),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "date", nullable: true),
                    CGC = table.Column<int>(type: "int", nullable: true),
                    CO_FUNCAO = table.Column<int>(type: "int", nullable: true),
                    TERMO_LGPD = table.Column<int>(type: "int", nullable: true),
                    CO_EVENTUAL = table.Column<int>(type: "int", nullable: true),
                    COORDENACAO = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    DATA_ENTRADA = table.Column<DateTime>(type: "date", nullable: true),
                    CO_SITUACAO = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RH_TB003_EMPREGADOS_ATIVOS");
        }
    }
}
