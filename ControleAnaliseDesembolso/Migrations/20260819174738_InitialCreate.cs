using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAnaliseDesembolso.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CAD_TB001_DESEMBOLSO",
                columns: table => new
                {
                    CO_DESEMBOLSO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MATRICULA_SOLICITANTE = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    CO_GIGOV = table.Column<string>(type: "nchar(4)", fixedLength: true, maxLength: 4, nullable: false),
                    MATRICULA_GESTOR = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    DT_SOLICITADO = table.Column<DateTime>(type: "date", nullable: false),
                    NU_DESEMBOLSO = table.Column<int>(type: "int", nullable: false),
                    CO_CONTRATO_AF = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CO_CONTRATO_AF_DV = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PRIMEIRO_DESEMBOLSO = table.Column<bool>(type: "bit", nullable: false),
                    AGENTE_FINANCEIRO = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CNPJ_AF = table.Column<string>(type: "nchar(14)", fixedLength: true, maxLength: 14, nullable: false),
                    MUTUARIO_FINAL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CNPJ_MUTUARIO_FINAL = table.Column<string>(type: "nchar(14)", fixedLength: true, maxLength: 14, nullable: false),
                    AGENTE_TECNICO_OPERADOR = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CNPJ_AGENTE_TECNICO_OPERADOR = table.Column<string>(type: "nchar(14)", fixedLength: true, maxLength: 14, nullable: true),
                    AGENTE_PROMOTOR = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CNPJ_AGENTE_PROMOTOR = table.Column<string>(type: "nchar(14)", fixedLength: true, maxLength: 14, nullable: false),
                    CO_PROGRAMA = table.Column<int>(type: "int", nullable: false),
                    ULTIMO_DESEMBOLSO = table.Column<bool>(type: "bit", nullable: false),
                    FUNCIONALIDADE = table.Column<bool>(type: "bit", nullable: true),
                    CONCLUIDO = table.Column<DateTime>(type: "date", nullable: true),
                    DT_ENGENHARIA = table.Column<DateTime>(type: "date", nullable: false),
                    CO_SITUACAO_OBRA = table.Column<int>(type: "int", nullable: true),
                    DT_SOCIO_AMBIENTAL = table.Column<DateTime>(type: "date", nullable: true),
                    PERCENTUAL_OBRA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CO_TIPO_DESEMBOLSO = table.Column<int>(type: "int", nullable: false),
                    RETORNO_PARCIAL = table.Column<bool>(type: "bit", nullable: true),
                    PLACA_LOCAL = table.Column<bool>(type: "bit", nullable: true),
                    LICENSA_INSTALACAO = table.Column<bool>(type: "bit", nullable: true),
                    LICENSA_OPERACAO = table.Column<bool>(type: "bit", nullable: true),
                    CND_VALIDO = table.Column<bool>(type: "bit", nullable: true),
                    CRP_VALIDO = table.Column<bool>(type: "bit", nullable: true),
                    SOLICITADO_VI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GLOSSADO_VI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ACEITO_VI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PARTICIPACAO_FGTS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CONTRAPARTIDA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VALOR_EMPRESTIMO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DESEMBOLSADO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SALDO_A_DESEMBOLSAR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EXCEPCIONALIZADO = table.Column<bool>(type: "bit", nullable: true),
                    CONTRAPARTIDA_ATUAL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    INTEGRALIZADO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SALDO_A_INTEGRALIZAR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CONTRAPARTIDA_ALTERADA = table.Column<bool>(type: "bit", nullable: true),
                    AMORTIZACAO = table.Column<bool>(type: "bit", nullable: true),
                    SANEPAR = table.Column<bool>(type: "bit", nullable: true),
                    MENSAGEM = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAD_TB001_DESEMBOLSO", x => x.CO_DESEMBOLSO);
                });

            migrationBuilder.CreateTable(
                name: "CAD_TB003_VALIDACAO",
                columns: table => new
                {
                    CO_VALIDACAO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DE_VALIDACAO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DT_CRIACAO = table.Column<DateTime>(type: "datetime", nullable: false),
                    CAMPO_VINCULADO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    USUARIO_EXCLUSAO = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    DT_EXCLUSAO = table.Column<DateTime>(type: "datetime", nullable: true),
                    DESATIVADO = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAD_TB003_VALIDACAO", x => x.CO_VALIDACAO);
                });

            migrationBuilder.CreateTable(
                name: "CAD_TB005_MENSAGEM",
                columns: table => new
                {
                    CO_MENSAGEM = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CO_VALIDACAO = table.Column<int>(type: "int", nullable: false),
                    CO_CONTROLE_DESEMBOLSO = table.Column<int>(type: "int", nullable: false),
                    DE_MENSAGEM = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    CO_TIPO_MENSAGEM = table.Column<int>(type: "int", nullable: false),
                    CO_USUARIO = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    DE_USUARIO = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UNIDADE_USUARIO = table.Column<int>(type: "int", nullable: false),
                    DT_CRIACAO = table.Column<DateTime>(type: "datetime", nullable: false),
                    ATIVO = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAD_TB005_MENSAGEM", x => x.CO_MENSAGEM);
                });

            migrationBuilder.CreateTable(
                name: "CAD_TB002_CONTROLE_DESEMBOLSO",
                columns: table => new
                {
                    CO_CONTROLE_DESEMBOLSO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CO_DESEMBOLSO = table.Column<int>(type: "int", nullable: false),
                    RESPONSAVEL_ANALISE = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    RESPONSAVEL_DESEMBOLSO = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    MATRICULA_BAIXA = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    GESTOR = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    DT_PRAZO = table.Column<DateTime>(type: "date", nullable: false),
                    CO_STATUS_DESEMBOLSO = table.Column<int>(type: "int", nullable: false),
                    DT_CONCLUSAO = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAD_TB002_CONTROLE_DESEMBOLSO", x => x.CO_CONTROLE_DESEMBOLSO);
                    table.ForeignKey(
                        name: "FK_CAD_TB001_DESEMBOLSO",
                        column: x => x.CO_DESEMBOLSO,
                        principalTable: "CAD_TB001_DESEMBOLSO",
                        principalColumn: "CO_DESEMBOLSO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                columns: table => new
                {
                    CO_VALIDACAO = table.Column<int>(type: "int", nullable: false),
                    CO_CONTROLE_DESEMBOLSO = table.Column<int>(type: "int", nullable: false),
                    DE_VALIDACAO = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CAMPO_VINCULADO = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SITUACAO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO", x => new { x.CO_VALIDACAO, x.CO_CONTROLE_DESEMBOLSO });
                    table.ForeignKey(
                        name: "FK_CAD_TB002_CONTROLE_DESEMBOLSO",
                        column: x => x.CO_CONTROLE_DESEMBOLSO,
                        principalTable: "CAD_TB002_CONTROLE_DESEMBOLSO",
                        principalColumn: "CO_CONTROLE_DESEMBOLSO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAD_TB003_VALIDACAO",
                        column: x => x.CO_VALIDACAO,
                        principalTable: "CAD_TB003_VALIDACAO",
                        principalColumn: "CO_VALIDACAO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CAD_TB002_CONTROLE_DESEMBOLSO_CO_DESEMBOLSO",
                table: "CAD_TB002_CONTROLE_DESEMBOLSO",
                column: "CO_DESEMBOLSO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO_CO_CONTROLE_DESEMBOLSO",
                table: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO",
                column: "CO_CONTROLE_DESEMBOLSO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropTable(
                name: "CAD_TB005_MENSAGEM");

            migrationBuilder.DropTable(
                name: "CAD_TB002_CONTROLE_DESEMBOLSO");

            migrationBuilder.DropTable(
                name: "CAD_TB003_VALIDACAO");

            migrationBuilder.DropTable(
                name: "CAD_TB001_DESEMBOLSO");
        }
    }
}
