-- ============================================================================
-- SCRIPT ÚNICO — RESET COMPLETO do banco CadDesembolsoDev (LocalDB).
--
-- Dropa tudo, recria já no schema final e popula as tabelas de catálogo/tipo.
-- Escrito à mão, coluna por coluna, direto dos arquivos de config atuais do
-- projeto (DesembolsoConfig.cs, ControleDesembolsoConfig.cs, ValidacaoConfig.cs,
-- ValidacaoControleDesembolsoConfig.cs, MensagemConfig.cs, TrilhaAuditoriaConfig.cs)
-- e conferido contra o ControleAnaliseDesembolsoContextModelSnapshot.cs —
-- NÃO é replay de migrations, então não passa por nenhum nome de coluna
-- intermediário/antigo.
--
-- Conferido em 2026-08-26, direto do disco:
--   - SALDO_INTEGRALIZAR (sem "A") — bate com
--     DesembolsoConfig.cs: builder.Property(x => x.SaldoIntegralizar).HasColumnName("SALDO_INTEGRALIZAR")
--   - SALDO_DESEMBOLSAR (sem "A")
--   - Sem coluna AMORTIZACAO (removida)
--   - RESPONSAVEL_BAIXA / RESPONSAVEL_DESEMBOLSO (nomes já trocados)
--
-- ATENÇÃO: isso apaga todos os dados existentes.
-- Rode conectado sem banco específico (sqlcmd sem -d), o próprio script
-- cria o banco se não existir.
-- ============================================================================

IF DB_ID(N'CadDesembolsoDev') IS NULL
BEGIN
    CREATE DATABASE [CadDesembolsoDev];
END;
GO

USE [CadDesembolsoDev];
GO

-- ============================================================================
-- 1) DROP (ordem que respeita as FKs: filhas antes das pais)
-- ============================================================================
IF OBJECT_ID(N'[CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO]') IS NOT NULL DROP TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO];
IF OBJECT_ID(N'[CAD_TB005_MENSAGEM]')                       IS NOT NULL DROP TABLE [CAD_TB005_MENSAGEM];
IF OBJECT_ID(N'[CAD_TB002_CONTROLE_DESEMBOLSO]')             IS NOT NULL DROP TABLE [CAD_TB002_CONTROLE_DESEMBOLSO];
IF OBJECT_ID(N'[CAD_TB001_DESEMBOLSO]')                      IS NOT NULL DROP TABLE [CAD_TB001_DESEMBOLSO];
IF OBJECT_ID(N'[CAD_TB003_VALIDACAO]')                       IS NOT NULL DROP TABLE [CAD_TB003_VALIDACAO];
IF OBJECT_ID(N'[CAD_TB000_TRILHA_AUDITORIA]')                IS NOT NULL DROP TABLE [CAD_TB000_TRILHA_AUDITORIA];
IF OBJECT_ID(N'[__EFMigrationsHistory]')                     IS NOT NULL DROP TABLE [__EFMigrationsHistory];
IF OBJECT_ID(N'[TIPO_STATUS_DESEMBOLSO]')                    IS NOT NULL DROP TABLE [TIPO_STATUS_DESEMBOLSO];
IF OBJECT_ID(N'[TIPO_SITUACAO_VALIDACAO]')                   IS NOT NULL DROP TABLE [TIPO_SITUACAO_VALIDACAO];
IF OBJECT_ID(N'[TIPO_MENSAGEM]')                             IS NOT NULL DROP TABLE [TIPO_MENSAGEM];
IF OBJECT_ID(N'[TIPO_DESEMBOLSO]')                           IS NOT NULL DROP TABLE [TIPO_DESEMBOLSO];
IF OBJECT_ID(N'[TIPO_SITUACAO_OBRA]')                        IS NOT NULL DROP TABLE [TIPO_SITUACAO_OBRA];
IF OBJECT_ID(N'[TIPO_PROGRAMA]')                             IS NOT NULL DROP TABLE [TIPO_PROGRAMA];
GO

-- ============================================================================
-- 2) CREATE — schema final (direto dos Config.cs atuais)
-- ============================================================================

CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

CREATE TABLE [CAD_TB001_DESEMBOLSO] (
    [CO_DESEMBOLSO]                  int NOT NULL IDENTITY,
    [MATRICULA_SOLICITANTE]          nvarchar(7)   NOT NULL,
    [CO_GIGOV]                       nchar(4)      NOT NULL,
    [MATRICULA_GESTOR]               nvarchar(7)   NOT NULL,
    [DT_SOLICITADO]                  date          NOT NULL,
    [NU_DESEMBOLSO]                  int           NOT NULL,
    [CO_CONTRATO_AF]                 nvarchar(20)  NOT NULL,
    [CO_CONTRATO_AF_DV]              nvarchar(10)  NOT NULL,
    [PRIMEIRO_DESEMBOLSO]            bit           NOT NULL,
    [AGENTE_FINANCEIRO]              nvarchar(255) NOT NULL,
    [CNPJ_AF]                        nchar(14)     NOT NULL,
    [MUTUARIO_FINAL]                 nvarchar(255) NOT NULL,
    [CNPJ_MUTUARIO_FINAL]            nchar(14)     NOT NULL,
    [AGENTE_TECNICO_OPERADOR]        nvarchar(255) NULL,
    [CNPJ_AGENTE_TECNICO_OPERADOR]   nchar(14)     NULL,
    [AGENTE_PROMOTOR]                nvarchar(255) NOT NULL,
    [CNPJ_AGENTE_PROMOTOR]           nchar(14)     NOT NULL,
    [CO_PROGRAMA]                    int           NOT NULL,
    [ULTIMO_DESEMBOLSO]              bit           NOT NULL,
    [FUNCIONALIDADE]                 bit           NULL,
    [CONCLUIDO]                      bit           NULL,
    [DT_ENGENHARIA]                  date          NOT NULL,
    [CO_SITUACAO_OBRA]               int           NULL,
    [DT_SOCIO_AMBIENTAL]             date          NULL,
    [PERCENTUAL_OBRA]                decimal(18,4) NOT NULL,
    [CO_TIPO_DESEMBOLSO]             int           NOT NULL,
    [RETORNO_PARCIAL]                bit           NULL,
    [PLACA_LOCAL]                    bit           NULL,
    [LICENSA_INSTALACAO]             bit           NULL,
    [LICENSA_OPERACAO]               bit           NULL,
    [CND_VALIDO]                     bit           NULL,
    [CRP_VALIDO]                     bit           NULL,
    [SOLICITADO_VI]                  decimal(18,2) NOT NULL,
    [GLOSSADO_VI]                    decimal(18,2) NOT NULL,
    [ACEITO_VI]                      decimal(18,2) NOT NULL,
    [PARTICIPACAO_FGTS]              decimal(18,2) NOT NULL,
    [CONTRAPARTIDA]                  decimal(18,2) NOT NULL,
    [VALOR_EMPRESTIMO]               decimal(18,2) NOT NULL,
    [DESEMBOLSADO]                   decimal(18,2) NOT NULL,
    [SALDO_DESEMBOLSAR]              decimal(18,2) NOT NULL,
    [EXCEPCIONALIZADO]               bit           NULL,
    [CONTRAPARTIDA_ATUAL]            decimal(18,2) NOT NULL,
    [INTEGRALIZADO]                  decimal(18,2) NOT NULL,
    [SALDO_INTEGRALIZAR]             decimal(18,2) NOT NULL,
    [CONTRAPARTIDA_ALTERADA]         bit           NULL,
    [SANEPAR]                        bit           NULL,
    [MENSAGEM]                       nvarchar(3000) NULL,
    [MOTIVO_REJEICAO]                nvarchar(3000) NULL,
    CONSTRAINT [PK_CAD_TB001_DESEMBOLSO] PRIMARY KEY ([CO_DESEMBOLSO])
);
GO

CREATE TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] (
    [CO_CONTROLE_DESEMBOLSO]  int NOT NULL IDENTITY,
    [CO_DESEMBOLSO]           int NOT NULL,
    [RESPONSAVEL_ANALISE]     nvarchar(7) NULL,
    [RESPONSAVEL_BAIXA]       nvarchar(7) NULL,
    [RESPONSAVEL_DESEMBOLSO]  nvarchar(7) NULL,
    [GESTOR]                  nvarchar(7) NULL,
    [DT_PRAZO]                date NOT NULL,
    [CO_STATUS_DESEMBOLSO]    int NOT NULL,
    [DT_CONCLUSAO]            date NULL,
    CONSTRAINT [PK_CAD_TB002_CONTROLE_DESEMBOLSO] PRIMARY KEY ([CO_CONTROLE_DESEMBOLSO]),
    CONSTRAINT [FK_CAD_TB001_DESEMBOLSO] FOREIGN KEY ([CO_DESEMBOLSO]) REFERENCES [CAD_TB001_DESEMBOLSO] ([CO_DESEMBOLSO]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_CAD_TB002_CONTROLE_DESEMBOLSO_CO_DESEMBOLSO] ON [CAD_TB002_CONTROLE_DESEMBOLSO] ([CO_DESEMBOLSO]);
GO

CREATE TABLE [CAD_TB003_VALIDACAO] (
    [CO_VALIDACAO]       int NOT NULL IDENTITY,
    [DE_VALIDACAO]       nvarchar(500) NULL,
    [DT_CRIACAO]         datetime NOT NULL,
    [CAMPO_VINCULADO]    nvarchar(100) NULL,
    [USUARIO_EXCLUSAO]   nvarchar(7) NULL,
    [DT_EXCLUSAO]        datetime NULL,
    [DESATIVADO]         bit NOT NULL,
    CONSTRAINT [PK_CAD_TB003_VALIDACAO] PRIMARY KEY ([CO_VALIDACAO])
);
GO

CREATE TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] (
    [CO_VALIDACAO]            int NOT NULL,
    [CO_CONTROLE_DESEMBOLSO]  int NOT NULL,
    [DE_VALIDACAO]            nvarchar(500) NULL,
    [CAMPO_VINCULADO]         nvarchar(100) NULL,
    [SITUACAO]                int NOT NULL,
    CONSTRAINT [PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] PRIMARY KEY ([CO_VALIDACAO], [CO_CONTROLE_DESEMBOLSO]),
    CONSTRAINT [FK_CAD_TB002_CONTROLE_DESEMBOLSO] FOREIGN KEY ([CO_CONTROLE_DESEMBOLSO]) REFERENCES [CAD_TB002_CONTROLE_DESEMBOLSO] ([CO_CONTROLE_DESEMBOLSO]) ON DELETE CASCADE,
    CONSTRAINT [FK_CAD_TB003_VALIDACAO] FOREIGN KEY ([CO_VALIDACAO]) REFERENCES [CAD_TB003_VALIDACAO] ([CO_VALIDACAO]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO_CO_CONTROLE_DESEMBOLSO] ON [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ([CO_CONTROLE_DESEMBOLSO]);
GO

CREATE TABLE [CAD_TB005_MENSAGEM] (
    [CO_MENSAGEM]              int NOT NULL IDENTITY,
    [CO_VALIDACAO]             int NOT NULL,
    [CO_CONTROLE_DESEMBOLSO]   int NOT NULL,
    [DE_MENSAGEM]              nvarchar(3000) NULL,
    [CO_TIPO_MENSAGEM]         int NOT NULL,
    [CO_USUARIO]               nvarchar(7) NULL,
    [DE_USUARIO]               nvarchar(255) NULL,
    [UNIDADE_USUARIO]          int NOT NULL,
    [DT_CRIACAO]               datetime NOT NULL,
    [ATIVO]                    bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_CAD_TB005_MENSAGEM] PRIMARY KEY ([CO_MENSAGEM])
);
GO

CREATE TABLE [CAD_TB000_TRILHA_AUDITORIA] (
    [ID_SOLICITACAO]     int NOT NULL IDENTITY,
    [USUARIO]            nvarchar(7) NOT NULL,
    [ENDERECO_LOGICO]    nvarchar(50) NOT NULL,
    [DT_SOLICITACAO]     datetime2 NOT NULL,
    [EVENTO]             nvarchar(100) NOT NULL,
    [DESC_EVENTO]        nvarchar(500) NOT NULL,
    [RESPOSTA]           nvarchar(20) NULL,
    CONSTRAINT [PK_CAD_TB000_TRILHA_AUDITORIA] PRIMARY KEY ([ID_SOLICITACAO])
);
GO

-- Marca as 8 migrations existentes como "já aplicadas" — dotnet-ef database
-- update continua funcionando normal daqui pra frente, só pra migrations
-- futuras que ainda não existem.
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES
    (N'20260819174738_InitialCreate', N'8.0.28'),
    (N'20260820144231_CriarTrilhaAuditoria', N'8.0.28'),
    (N'20260820151135_RenomearTrilhaAuditoria', N'8.0.28'),
    (N'20260821034844_AumentarPrecisaoPercentualObra', N'8.0.28'),
    (N'20260821184834_RenomearSaldosEConcluidoBool', N'8.0.28'),
    (N'20260826153908_RemoverAmortizacao', N'8.0.28'),
    (N'20260826191252_RemapearStatusDesembolso', N'8.0.28'),
    (N'20260826191441_TrocarResponsavelBaixaEDesembolso', N'8.0.28');
GO

-- ============================================================================
-- 3) TABELAS "TIPO" (lookup/referência) — NÃO fazem parte do modelo do EF
-- (o C# guarda esses códigos como int cru, validado só no enum da aplicação;
-- não existe DbSet nem FK pra elas). Criadas aqui só como referência/consulta
-- no banco — de propósito SEM FK das tabelas principais pra essas, assim elas
-- não interferem em nada que o `dotnet ef database update` for rodar depois.
-- ============================================================================

CREATE TABLE [TIPO_STATUS_DESEMBOLSO] (
    [CODIGO] int NOT NULL,
    [DESCRICAO] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TIPO_STATUS_DESEMBOLSO] PRIMARY KEY ([CODIGO])
);
GO
INSERT INTO [TIPO_STATUS_DESEMBOLSO] (CODIGO, DESCRICAO) VALUES
    (1, N'PENDENTE'), (2, N'ANALISAR'), (3, N'DESEMBOLSAR'), (4, N'NEGAR'), (5, N'FINALIZAR');
GO

CREATE TABLE [TIPO_SITUACAO_VALIDACAO] (
    [CODIGO] int NOT NULL,
    [DESCRICAO] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TIPO_SITUACAO_VALIDACAO] PRIMARY KEY ([CODIGO])
);
GO
INSERT INTO [TIPO_SITUACAO_VALIDACAO] (CODIGO, DESCRICAO) VALUES
    (0, N'ANALISAR'), (1, N'APROVADO'), (2, N'NEGADO');
GO

CREATE TABLE [TIPO_MENSAGEM] (
    [CODIGO] int NOT NULL,
    [DESCRICAO] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TIPO_MENSAGEM] PRIMARY KEY ([CODIGO])
);
GO
INSERT INTO [TIPO_MENSAGEM] (CODIGO, DESCRICAO) VALUES
    (0, N'JUSTIFICATIVA'), (1, N'INFORMATIVO'), (2, N'PARECER'), (3, N'OBSERVACAO'), (4, N'REJEICAO');
GO

CREATE TABLE [TIPO_DESEMBOLSO] (
    [CODIGO] int NOT NULL,
    [DESCRICAO] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TIPO_DESEMBOLSO] PRIMARY KEY ([CODIGO])
);
GO
INSERT INTO [TIPO_DESEMBOLSO] (CODIGO, DESCRICAO) VALUES
    (0, N'NORMAL'), (1, N'ADIANTAMENTO');
GO

CREATE TABLE [TIPO_SITUACAO_OBRA] (
    [CODIGO] int NOT NULL,
    [DESCRICAO] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TIPO_SITUACAO_OBRA] PRIMARY KEY ([CODIGO])
);
GO
INSERT INTO [TIPO_SITUACAO_OBRA] (CODIGO, DESCRICAO) VALUES
    (0, N'NORMAL'), (1, N'ATRASADO');
GO

CREATE TABLE [TIPO_PROGRAMA] (
    [CODIGO] int NOT NULL,
    [DESCRICAO] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_TIPO_PROGRAMA] PRIMARY KEY ([CODIGO])
);
GO
INSERT INTO [TIPO_PROGRAMA] (CODIGO, DESCRICAO) VALUES
    (0, N'Pro_Transporte'), (1, N'Pro_Moradia'), (2, N'Saneamento'), (3, N'Saude');
GO

-- ============================================================================
-- 4) CATÁLOGO DE VALIDAÇÕES (CAD_TB003_VALIDACAO) — essa sim faz parte do
-- modelo real, é o catálogo de itens de checklist. Espelha 1:1 os itens
-- hardcoded na Etapa 2 (Verificações) do Preencher/Editar FPD
-- (PainelPreencherFpdEtapas.razor) e as regras automáticas em
-- ValidadorDesembolsoService._regras.
--
-- IDs precisam ser EXATAMENTE esses (1, 2, 3, 5, 7, 8, 9, 1005) — são os
-- mesmos números hardcoded em ValidadorDesembolsoService, que casa
-- CoValidacao com o campo bool da ficha pra aprovar automaticamente. Por
-- isso usa SET IDENTITY_INSERT.
-- ============================================================================

SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] ON;
GO

INSERT INTO [CAD_TB003_VALIDACAO] (CO_VALIDACAO, DE_VALIDACAO, DT_CRIACAO, DESATIVADO) VALUES
    (1,    N'Licença de operação',           GETDATE(), 0),
    (2,    N'Licença de instalação',         GETDATE(), 0),
    (3,    N'Tomador adimplente',            GETDATE(), 0),
    (5,    N'Agente Promotor adimplente',    GETDATE(), 1), -- toggle comentado no front
    (6,    N'Amortização',                   GETDATE(), 0), -- regra automática removida; item manual
    (7,    N'Retorno parcial',                GETDATE(), 0),
    (8,    N'Placa local',                    GETDATE(), 0),
    (9,    N'Excepcionalização',              GETDATE(), 0),
    (1005, N'CP alterada',                    GETDATE(), 0);
GO

SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] OFF;
GO

SELECT * FROM [CAD_TB003_VALIDACAO] ORDER BY CO_VALIDACAO;
GO
