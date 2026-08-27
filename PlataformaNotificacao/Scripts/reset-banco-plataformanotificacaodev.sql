-- ============================================================================
-- SCRIPT ÚNICO — RESET COMPLETO do banco PlataformaNotificacaoDev (LocalDB).
--
-- Esse é o SEGUNDO banco que a API usa (junto do CadDesembolsoDev) — toda
-- ação de escrita do CAD (aprovar, criar FPD, comentar etc.) chama o serviço
-- de notificação depois de salvar, e o dropdown de "Vincular analista"
-- (ObterEmpregadosPorCoordenacao) também consulta esse banco direto
-- (EmpregadoCADService injeta PlataformaNotificacaoContext). Sem esse banco
-- existir, a API cai em erro 500 genérico.
--
-- Escrito à mão, direto de:
--   PlataformaNotificacaoContext.cs, NotificacaoConfig.cs,
--   ControleVisualizacaoConfig.cs, EmpregadoAtivoConfig.cs,
--   EmpregadoGigovConfig.cs, e conferido contra
--   PlataformaNotificacaoContextModelSnapshot.cs.
--
-- ATENÇÃO: isso apaga todos os dados existentes.
-- Rode conectado sem banco específico (sqlcmd sem -d).
-- ============================================================================

IF DB_ID(N'PlataformaNotificacaoDev') IS NULL
BEGIN
    CREATE DATABASE [PlataformaNotificacaoDev];
END;
GO

USE [PlataformaNotificacaoDev];
GO

-- ============================================================================
-- 1) DROP (ordem que respeita as FKs)
-- ============================================================================
IF OBJECT_ID(N'[PLA_NOT_TB002_CONTROLE_VISUALIZACAO]') IS NOT NULL DROP TABLE [PLA_NOT_TB002_CONTROLE_VISUALIZACAO];
IF OBJECT_ID(N'[PLA_NOT_TB001_NOTIFICACAO]')            IS NOT NULL DROP TABLE [PLA_NOT_TB001_NOTIFICACAO];
IF OBJECT_ID(N'[PLA_NOT_TB003_EMPREGADOS_GIGOV]')       IS NOT NULL DROP TABLE [PLA_NOT_TB003_EMPREGADOS_GIGOV];
IF OBJECT_ID(N'[RH_TB003_EMPREGADOS_ATIVOS]')           IS NOT NULL DROP TABLE [RH_TB003_EMPREGADOS_ATIVOS];
IF OBJECT_ID(N'[__EFMigrationsHistory]')                IS NOT NULL DROP TABLE [__EFMigrationsHistory];
GO

-- ============================================================================
-- 2) CREATE — schema final
-- ============================================================================

CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

-- RH_TB003_EMPREGADOS_ATIVOS: entidade SEM chave no EF (.HasNoKey() —
-- dados espelhados do RH, só leitura). Sem PRIMARY KEY de propósito.
CREATE TABLE [RH_TB003_EMPREGADOS_ATIVOS] (
    [CO_EMPREGADO]      int NOT NULL IDENTITY,
    [MATRICULA]         nvarchar(7)   NULL,
    [MATRICULA_DV]      int           NULL,
    [NOME]              nvarchar(150) NULL,
    [DATA_ADMISSAO]     date          NULL,
    [DATA_NASCIMENTO]   date          NULL,
    [CGC]               int           NULL,
    [CO_FUNCAO]         int           NULL,
    [TERMO_LGPD]        int           NULL,
    [CO_EVENTUAL]       int           NULL,
    [COORDENACAO]       nvarchar(7)   NULL,
    [DATA_ENTRADA]      date          NULL,
    [CO_SITUACAO]       int           NULL
);
GO

CREATE TABLE [PLA_NOT_TB003_EMPREGADOS_GIGOV] (
    [CO_EMPREGADO]  int NOT NULL IDENTITY,
    [MATRICULA]     nvarchar(7)   NOT NULL,
    [NOME]          nvarchar(150) NOT NULL,
    [ATIVO]         bit           NOT NULL,
    [CO_GIGOV]      nvarchar(10)  NOT NULL,
    CONSTRAINT [PK_PLA_NOT_TB003_EMPREGADOS_GIGOV] PRIMARY KEY ([CO_EMPREGADO])
);
GO

CREATE TABLE [PLA_NOT_TB001_NOTIFICACAO] (
    [CO_NOTIFICACAO]        int NOT NULL IDENTITY,
    [CO_APLICATIVO]         int            NULL,
    [CO_USUARIO_EMISSOR]    nvarchar(7)    NULL,
    [TITULO]                nvarchar(200)  NOT NULL,
    [MENSAGEM]              nvarchar(1000) NOT NULL,
    [TIPO]                  int            NOT NULL,
    [DT_CRIACAO]            datetime2      NOT NULL,
    [DT_VALIDADE]           datetime2      NULL,
    CONSTRAINT [PK_PLA_NOT_TB001_NOTIFICACAO] PRIMARY KEY ([CO_NOTIFICACAO])
);
GO

CREATE TABLE [PLA_NOT_TB002_CONTROLE_VISUALIZACAO] (
    [CO_VISUALIZACAO]   int NOT NULL IDENTITY,
    [CO_NOTIFICACAO]    int         NOT NULL,
    [CO_USUARIO]        nvarchar(7) NOT NULL,
    [DT_VISUALIZACAO]   datetime2   NULL,
    [LINK]              nvarchar(500) NULL,
    CONSTRAINT [PK_PLA_NOT_TB002_CONTROLE_VISUALIZACAO] PRIMARY KEY ([CO_VISUALIZACAO]),
    CONSTRAINT [FK_PLA_NOT_TB002_CONTROLE_VISUALIZACAO_PLA_NOT_TB001_NOTIFICACAO] FOREIGN KEY ([CO_NOTIFICACAO]) REFERENCES [PLA_NOT_TB001_NOTIFICACAO] ([CO_NOTIFICACAO]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_PLA_NOT_TB002_CONTROLE_VISUALIZACAO_CO_NOTIFICACAO] ON [PLA_NOT_TB002_CONTROLE_VISUALIZACAO] ([CO_NOTIFICACAO]);
GO

-- Marca as 3 migrations existentes como "já aplicadas".
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES
    (N'20260818021621_AddEmpregadoAtivo', N'8.0.28'),
    (N'20260818042639_AddEmpregadoGigov', N'8.0.28'),
    (N'20260818044722_AddCodigoGigovToEmpregadoGigov', N'8.0.28');
GO

-- ============================================================================
-- 3) Sem seed de funcionário aqui de propósito — RH_TB003_EMPREGADOS_ATIVOS
-- e PLA_NOT_TB003_EMPREGADOS_GIGOV são dados reais de RH/GIGOV, não tenho
-- uma fonte legítima pra inventar isso. Sem pelo menos 1 linha em cada,
-- "Vincular responsável" e a notificação por GIGOV ficam sem ninguém pra
-- escolher/notificar (não quebra, só fica vazio). Se quiser, eu insiro
-- funcionários de teste — é só pedir.
-- ============================================================================
