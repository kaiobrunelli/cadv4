using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    public partial class RenomearSaldosEConcluidoBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SALDO_A_DESEMBOLSAR",
                table: "CAD_TB001_DESEMBOLSO",
                newName: "SALDO_DESEMBOLSAR");

            migrationBuilder.RenameColumn(
                name: "SALDO_A_INTEGRALIZAR",
                table: "CAD_TB001_DESEMBOLSO",
                newName: "SALDO_INTEGRALIZAR");

            // Não existe conversão válida de date pra bit no SQL Server (nem
            // implícita, nem via CAST) — um ALTER COLUMN direto falha sempre,
            // com dado ou sem dado na coluna. Sem sentido tentar preservar o
            // valor mesmo: os dados antigos em formato date nunca foram uma
            // resposta Sim/Não válida (o campo virou bool só depois, quando
            // corrigimos pra bater com o XP3). Troca a coluna via drop+add.
            migrationBuilder.DropColumn(
                name: "CONCLUIDO",
                table: "CAD_TB001_DESEMBOLSO");

            migrationBuilder.AddColumn<bool>(
                name: "CONCLUIDO",
                table: "CAD_TB001_DESEMBOLSO",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SALDO_DESEMBOLSAR",
                table: "CAD_TB001_DESEMBOLSO",
                newName: "SALDO_A_DESEMBOLSAR");

            migrationBuilder.RenameColumn(
                name: "SALDO_INTEGRALIZAR",
                table: "CAD_TB001_DESEMBOLSO",
                newName: "SALDO_A_INTEGRALIZAR");

            migrationBuilder.DropColumn(
                name: "CONCLUIDO",
                table: "CAD_TB001_DESEMBOLSO");

            migrationBuilder.AddColumn<DateTime>(
                name: "CONCLUIDO",
                table: "CAD_TB001_DESEMBOLSO",
                type: "date",
                nullable: true);
        }
    }
}
