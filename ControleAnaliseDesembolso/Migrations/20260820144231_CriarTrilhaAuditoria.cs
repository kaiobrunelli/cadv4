using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    public partial class CriarTrilhaAuditoria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB001_TRILHA_AUDITORIA",
                columns: table => new
                {
                    ID_SOLICITACAO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USUARIO = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ENDERECO_LOGICO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DT_SOLICITACAO = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EVENTO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DESC_EVENTO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RESPOSTA = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB001_TRILHA_AUDITORIA", x => x.ID_SOLICITACAO);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB001_TRILHA_AUDITORIA");
        }
    }
}
