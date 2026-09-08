using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    /// <inheritdoc />
    public partial class UnificarValidacoesEConferencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.AddColumn<int>(
                name: "CO_VALIDACAO_CONTROLE",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "DT_VALIDACAO",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MENSAGEM",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ORIGEM",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ORIGEM",
                table: "CAD_TB003_VALIDACAO",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Nota: colunas de CAD_TB001/CAD_TB002 que o "dotnet ef migrations add"
            // detectou como pendentes (CRF_*, DT_DRP, RECORRENTE, TEM_CARROCERIA etc.)
            // já existem no banco real — foram aplicadas por script .sql manual
            // (ver ControleAnaliseDesembolso/Scripts/ajustes-v2-observacoes-conferencia.sql
            // e ajustes-16-mudancas-cad.sql), sem uma migration correspondente. Não
            // fazem parte desta mudança (unificação de validações) e foram removidas
            // daqui pra essa migration não tentar recriar colunas que já existem.

            migrationBuilder.AddPrimaryKey(
                name: "PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                column: "CO_VALIDACAO_CONTROLE");

            migrationBuilder.CreateIndex(
                name: "IX_CAD_TB004_VALIDACAO_CONTROLE",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                columns: new[] { "CO_VALIDACAO", "CO_CONTROLE_DESEMBOLSO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropIndex(
                name: "IX_CAD_TB004_VALIDACAO_CONTROLE",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropColumn(
                name: "CO_VALIDACAO_CONTROLE",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropColumn(
                name: "DT_VALIDACAO",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropColumn(
                name: "MENSAGEM",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropColumn(
                name: "ORIGEM",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropColumn(
                name: "ORIGEM",
                table: "CAD_TB003_VALIDACAO");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                columns: new[] { "CO_VALIDACAO", "CO_CONTROLE_DESEMBOLSO" });
        }
    }
}
