-- ============================================================================
-- Script completo — DROP + CREATE do schema do CAD no banco [DB7175_RH]
-- (ambiente de desenvolvimento/homologação isolado, apesar do nome bater
-- com a convenção do banco real), MAIS 2 registros de exemplo em cada
-- categoria das tabelas principais, pra já ter dado de teste depois de
-- rodar.
--
-- É a mesma base do ajustes-v8-recriar-schema-completo.sql +
-- ajustes-v10-autorizados-responsavel-analise.sql (mesmas 16 tabelas,
-- mesmos FKs, mesmas correções em relação ao script "verdadeiro" original —
-- ver os comentários de decisão no v8), só que:
--   1) USE [DB7175_RH] em vez de [CadDesembolsoDev];
--   2) além do seed das tabelas TIPO (que já é completo — todas as
--      categorias, não é isso que "2 de cada" se refere), insere 2
--      desembolsos de exemplo pra CADA categoria de status (Pendente,
--      Analisar, Desembolsar, Rejeitado, Finalizado, Cancelado = 12
--      desembolsos), 2 comentários de exemplo, 2 matrículas de exemplo na
--      lista de autorizados, e 2 eventos de exemplo na trilha de auditoria.
--
-- !!! DESTRUTIVO !!! Apaga TODAS as tabelas do CAD nesse banco e recria do
-- zero. Confirmado que [DB7175_RH] aqui é um ambiente isolado de
-- dev/homologação — se isso mudar, NÃO rode este script sem revisar de novo.
--
-- Idempotente na parte de DROP (usa IF OBJECT_ID). A parte de CREATE/INSERT
-- assume que o DROP já rodou antes (schema limpo) — rodar de novo sem
-- dropar no meio dá erro de "already exists"/PK duplicada, o que é esperado.
-- ============================================================================

USE [DB7175_RH];
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 1 — DROP (filhas antes das mães, por causa das FKs)
-- ════════════════════════════════════════════════════════════════════════

IF OBJECT_ID('CAD_TB005_MENSAGEM')                             IS NOT NULL DROP TABLE [CAD_TB005_MENSAGEM];
GO
IF OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO')        IS NOT NULL DROP TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO];
GO
IF OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO')                  IS NOT NULL DROP TABLE [CAD_TB002_CONTROLE_DESEMBOLSO];
GO
IF OBJECT_ID('CAD_TB001_DESEMBOLSO')                           IS NOT NULL DROP TABLE [CAD_TB001_DESEMBOLSO];
GO
IF OBJECT_ID('CAD_TB006_VALIDACAO_REGISTRO')                   IS NOT NULL DROP TABLE [CAD_TB006_VALIDACAO_REGISTRO];
GO
IF OBJECT_ID('CAD_TB003_VALIDACAO')                            IS NOT NULL DROP TABLE [CAD_TB003_VALIDACAO];
GO
IF OBJECT_ID('CAD_TB000_TRILHA_AUDITORIA')                     IS NOT NULL DROP TABLE [CAD_TB000_TRILHA_AUDITORIA];
GO
IF OBJECT_ID('CAD_TB006_TIPO_PROGRAMA')                        IS NOT NULL DROP TABLE [CAD_TB006_TIPO_PROGRAMA];
GO
IF OBJECT_ID('CAD_TB007_TIPO_SITUACAO_OBRA')                   IS NOT NULL DROP TABLE [CAD_TB007_TIPO_SITUACAO_OBRA];
GO
IF OBJECT_ID('CAD_TB008_TIPO_DESEMBOLSO')                      IS NOT NULL DROP TABLE [CAD_TB008_TIPO_DESEMBOLSO];
GO
IF OBJECT_ID('CAD_TB009_TIPO_STATUS_DESEMBOLSO')               IS NOT NULL DROP TABLE [CAD_TB009_TIPO_STATUS_DESEMBOLSO];
GO
IF OBJECT_ID('CAD_TB010_PROGRAMA')                             IS NOT NULL DROP TABLE [CAD_TB010_PROGRAMA];
GO
IF OBJECT_ID('CAD_TB010_TIPO_MENSAGEM')                        IS NOT NULL DROP TABLE [CAD_TB010_TIPO_MENSAGEM];
GO
IF OBJECT_ID('CAD_TB011_TIPO_SITUACAO_VALIDACAO')              IS NOT NULL DROP TABLE [CAD_TB011_TIPO_SITUACAO_VALIDACAO];
GO
IF OBJECT_ID('CAD_TB012_TIPO_ORIGEM_VALIDACAO')                IS NOT NULL DROP TABLE [CAD_TB012_TIPO_ORIGEM_VALIDACAO];
GO
IF OBJECT_ID('CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE')       IS NOT NULL DROP TABLE [CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE];
GO

-- Lixo de uma unificação anterior (ver ajustes-v4-unificar-validacoes.sql) —
-- a filha (tem FK pra "CAMPO_CONFERENCIA") precisa cair antes da mãe.
IF OBJECT_ID('CAD_TB006_OLD_CONFERENCIA_CONTROLE_DESEMBOLSO')  IS NOT NULL DROP TABLE [CAD_TB006_OLD_CONFERENCIA_CONTROLE_DESEMBOLSO];
GO
IF OBJECT_ID('CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO')      IS NOT NULL DROP TABLE [CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO];
GO
IF OBJECT_ID('CAD_TB005_OLD_CAMPO_CONFERENCIA')                IS NOT NULL DROP TABLE [CAD_TB005_OLD_CAMPO_CONFERENCIA];
GO
IF OBJECT_ID('CAD_TB005_CAMPO_CONFERENCIA')                    IS NOT NULL DROP TABLE [CAD_TB005_CAMPO_CONFERENCIA];
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 2 — CREATE (tabelas TIPO/lookup primeiro, depois as principais)
-- ════════════════════════════════════════════════════════════════════════

CREATE TABLE [dbo].[CAD_TB006_TIPO_PROGRAMA](
	[CO_PROGRAMA] [int] NOT NULL,
	[DE_PROGRAMA] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB006_TIPO_PROGRAMA] PRIMARY KEY CLUSTERED ([CO_PROGRAMA] ASC)
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB007_TIPO_SITUACAO_OBRA](
	[CO_SITUACAO_OBRA] [int] NOT NULL,
	[NO_SITUACAO_OBRA] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB007_SITUACAO_OBRA] PRIMARY KEY CLUSTERED ([CO_SITUACAO_OBRA] ASC)
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB008_TIPO_DESEMBOLSO](
	[CO_TIPO_DESEMBOLSO] [int] NOT NULL,
	[NO_TIPO_DESEMBOLSO] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB008_TIPO_DESEMBOLSO] PRIMARY KEY CLUSTERED ([CO_TIPO_DESEMBOLSO] ASC)
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB009_TIPO_STATUS_DESEMBOLSO](
	[CO_STATUS_DESEMBOLSO] [int] NOT NULL,
	[DE_STATUS_DESEMBOLSO] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB009_STATUS_DESEMBOLSO] PRIMARY KEY CLUSTERED ([CO_STATUS_DESEMBOLSO] ASC)
) ON [PRIMARY];
GO

-- Legado — sem FK apontando pra ela, nada do código usa; mantida pelo nome real.
CREATE TABLE [dbo].[CAD_TB010_PROGRAMA](
	[CO_PROGRAMA] [int] NOT NULL,
	[NO_PROGRAMA] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB006_PROGRAMA] PRIMARY KEY CLUSTERED ([CO_PROGRAMA] ASC)
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB010_TIPO_MENSAGEM](
	[CO_TIPO_MENSAGEM] [int] NOT NULL,
	[DE_TIPO_MENSAGEM] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CAD_TB010_TIPO_REGISTRO] PRIMARY KEY CLUSTERED ([CO_TIPO_MENSAGEM] ASC)
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB011_TIPO_SITUACAO_VALIDACAO](
	[CO_TIPO_SITUACAO_VALIDACAO] [int] NOT NULL,
	[DE_TIPO_SITUACAO] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CAD_TB011_TIPO_SITUACAO_VALIDACAO] PRIMARY KEY CLUSTERED ([CO_TIPO_SITUACAO_VALIDACAO] ASC)
) ON [PRIMARY];
GO

-- Espelha o enum TipoOrigemValidacao (Manual=0, AutomaticaCampo=1, ConferenciaSiapf=3).
CREATE TABLE [dbo].[CAD_TB012_TIPO_ORIGEM_VALIDACAO](
	[CO_TIPO_ORIGEM_VALIDACAO] [int] NOT NULL,
	[DE_TIPO_ORIGEM_VALIDACAO] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CAD_TB012_TIPO_ORIGEM_VALIDACAO] PRIMARY KEY CLUSTERED ([CO_TIPO_ORIGEM_VALIDACAO] ASC)
) ON [PRIMARY];
GO

-- Matrículas com permissão total pra vincular/remover qualquer responsável
-- pela análise. Editada direto aqui no banco (INSERT/DELETE), sem deploy.
CREATE TABLE [dbo].[CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE](
	[MATRICULA] [varchar](7) NOT NULL,
	[NOME] [varchar](100) NULL,
	[DT_INCLUSAO] [datetime] NOT NULL CONSTRAINT [DF_CAD_TB013_DT_INCLUSAO] DEFAULT (GETDATE()),
 CONSTRAINT [PK_CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE] PRIMARY KEY CLUSTERED ([MATRICULA] ASC)
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB000_TRILHA_AUDITORIA](
	[ID_SOLICITACAO] [int] IDENTITY(1,1) NOT NULL,
	[USUARIO] [varchar](7) NOT NULL,
	[ENDERECO_LOGICO] [varchar](50) NOT NULL,
	[DT_SOLICITACAO] [datetime2](7) NOT NULL,
	[EVENTO] [varchar](100) NOT NULL,
	[DESC_EVENTO] [varchar](max) NOT NULL,
	[RESPOSTA] [varchar](400) NULL,
 CONSTRAINT [PK_CAD_TB000_TRILHA_AUDITORIA] PRIMARY KEY CLUSTERED ([ID_SOLICITACAO] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB001_DESEMBOLSO](
	[CO_DESEMBOLSO] [int] IDENTITY(1,1) NOT NULL,
	[MATRICULA_SOLICITANTE] [varchar](7) NOT NULL,
	[CO_GIGOV] [varchar](4) NOT NULL,
	[MATRICULA_GESTOR] [varchar](7) NOT NULL,
	[DT_SOLICITADO] [date] NOT NULL,
	[NU_DESEMBOLSO] [int] NOT NULL,
	[CO_CONTRATO_AF] [varchar](15) NOT NULL,
	[CO_CONTRATO_AF_DV] [varchar](5) NOT NULL,
	[CO_CONTRATO_AO] [varchar](15) NULL,
	[CO_CONTRATO_AO_DV] [varchar](5) NULL,
	[PRIMEIRO_DESEMBOLSO] [bit] NOT NULL,
	[RECORRENTE] [bit] NOT NULL,
	[AGENTE_FINANCEIRO] [varchar](500) NOT NULL,
	[CNPJ_AF] [char](14) NOT NULL,
	[MUTUARIO_FINAL] [varchar](500) NOT NULL,
	[CNPJ_MUTUARIO_FINAL] [char](14) NOT NULL,
	[AGENTE_TECNICO_OPERADOR] [varchar](500) NULL,
	[CNPJ_AGENTE_TECNICO_OPERADOR] [char](14) NULL,
	[AGENTE_PROMOTOR] [varchar](500) NOT NULL,
	[CNPJ_AGENTE_PROMOTOR] [char](14) NOT NULL,
	[CO_PROGRAMA] [int] NOT NULL,
	[ULTIMO_DESEMBOLSO] [bit] NOT NULL,
	[FUNCIONALIDADE] [bit] NULL,
	[CONCLUIDO] [bit] NULL,
	[DT_ENGENHARIA] [date] NOT NULL,
	[CO_SITUACAO_OBRA] [int] NULL,
	[DT_SOCIO_AMBIENTAL] [date] NULL,
	[PERCENTUAL_OBRA] [decimal](18, 4) NOT NULL,
	[CO_TIPO_DESEMBOLSO] [int] NOT NULL,
	[RETORNO_PARCIAL] [bit] NULL,
	[PLACA_LOCAL] [bit] NOT NULL,
	[LICENSA_INSTALACAO] [bit] NOT NULL,
	[LICENSA_OPERACAO] [bit] NOT NULL,
	[CND_VALIDO] [bit] NULL,
	[CRP_VALIDO] [bit] NULL,
	[CRP_NSA] [bit] NOT NULL,
	[SOLICITADO_VI] [decimal](18, 2) NOT NULL,
	[GLOSSADO_VI] [decimal](18, 2) NOT NULL,
	[ACEITO_VI] [decimal](18, 2) NOT NULL,
	[PARTICIPACAO_FGTS] [decimal](18, 2) NOT NULL,
	[VALOR_EMPRESTIMO] [decimal](18, 2) NOT NULL,
	[DESEMBOLSADO] [decimal](18, 2) NOT NULL,
	[SALDO_DESEMBOLSAR] [decimal](18, 2) NOT NULL,
	[EXCEPCIONALIZADO] [bit] NOT NULL,
	[CONTRAPARTIDA] [decimal](18, 2) NOT NULL,
	[CONTRAPARTIDA_ATUAL] [decimal](18, 2) NOT NULL,
	[INTEGRALIZADO] [decimal](18, 2) NOT NULL,
	[SALDO_INTEGRALIZAR] [decimal](18, 2) NOT NULL,
	[CONTRAPARTIDA_ALTERADA] [bit] NOT NULL,
	[SANEPAR] [bit] NOT NULL,
	[MENSAGEM] [varchar](3000) NULL,
	[MENSAGEM_CEFGA] [varchar](3000) NULL,
	[MOTIVO_REJEICAO] [varchar](3000) NULL,
	[TEM_CARROCERIA] [bit] NULL,
	[VEICULO_POSSUI_ADESIVOS] [bit] NULL,
	[DATA_INICIO_OBRA] [date] NULL,
	[DESTINACAO_COLETA_RESIDUOS_SOLIDOS] [bit] NULL,
 CONSTRAINT [PK_CAD_TB001_DESEMBOLSO] PRIMARY KEY CLUSTERED
(
	[CO_DESEMBOLSO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB002_CONTROLE_DESEMBOLSO](
	[CO_CONTROLE_DESEMBOLSO] [int] IDENTITY(1,1) NOT NULL,
	[CO_DESEMBOLSO] [int] NOT NULL,
	[RESPONSAVEL_ANALISE] [varchar](7) NULL,
	[RESPONSAVEL_BAIXA] [varchar](7) NULL,
	[RESPONSAVEL_DESEMBOLSO] [varchar](7) NULL,
	[GESTOR] [varchar](7) NULL,
	[DT_PRAZO] [date] NOT NULL,
	[CO_STATUS_DESEMBOLSO] [int] NOT NULL,
	[DT_CONCLUSAO] [datetime] NULL,
	[MOTIVO_CANCELAMENTO] [varchar](3000) NULL,
	[DT_ULTIMA_CONFERENCIA] [datetime] NULL,
	[NUMERO_DRP] [nvarchar](20) NULL,
	[DV_DRP] [nvarchar](5) NULL,
	[SENHA_DRP] [nvarchar](20) NULL,
	[DT_DRP] [date] NULL,
	[CRF_AF] [date] NULL,
	[CRF_TOMADOR] [date] NULL,
	[CRF_AP] [date] NULL,
	[CRF_AT] [date] NULL,
 CONSTRAINT [PK_CAD_TB002_CONTROLE_DESEMBOLSO] PRIMARY KEY CLUSTERED
(
	[CO_CONTROLE_DESEMBOLSO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB003_VALIDACAO](
	[CO_VALIDACAO] [int] IDENTITY(1,1) NOT NULL,
	[DE_VALIDACAO] [varchar](500) NULL,
	[DT_CRIACAO] [datetime] NOT NULL,
	[CAMPO_VINCULADO] [varchar](100) NULL,
	[USUARIO_EXCLUSAO] [varchar](7) NULL,
	[DT_EXCLUSAO] [datetime] NULL,
	[DESATIVADO] [bit] NOT NULL,
	[ORIGEM] [int] NOT NULL CONSTRAINT [DF_CAD_TB003_ORIGEM] DEFAULT ((0)),
 CONSTRAINT [PK_CAD_TB003_VALIDACAO] PRIMARY KEY CLUSTERED
(
	[CO_VALIDACAO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO](
	[CO_VALIDACAO_CONTROLE] [int] IDENTITY(1,1) NOT NULL,
	[CO_VALIDACAO] [int] NOT NULL,
	[CO_CONTROLE_DESEMBOLSO] [int] NOT NULL,
	[DE_VALIDACAO] [varchar](500) NULL,
	[CAMPO_VINCULADO] [varchar](100) NULL,
	[SITUACAO] [int] NOT NULL,
	[ORIGEM] [int] NOT NULL CONSTRAINT [DF_CAD_TB004_ORIGEM] DEFAULT ((0)),
	[MENSAGEM] [varchar](1000) NULL,
	[DT_VALIDACAO] [datetime] NOT NULL,
 CONSTRAINT [PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] PRIMARY KEY CLUSTERED
(
	[CO_VALIDACAO_CONTROLE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO

CREATE NONCLUSTERED INDEX [IX_CAD_TB004_VALIDACAO_CONTROLE] ON [dbo].[CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO]
(
	[CO_VALIDACAO] ASC,
	[CO_CONTROLE_DESEMBOLSO] ASC
) ON [PRIMARY];
GO

CREATE TABLE [dbo].[CAD_TB005_MENSAGEM](
	[CO_MENSAGEM] [int] IDENTITY(1,1) NOT NULL,
	[CO_VALIDACAO] [int] NOT NULL,
	[CO_CONTROLE_DESEMBOLSO] [int] NOT NULL,
	[DE_MENSAGEM] [varchar](3000) NULL,
	[CO_TIPO_MENSAGEM] [int] NOT NULL,
	[CO_USUARIO] [varchar](7) NULL,
	[DE_USUARIO] [varchar](100) NULL,
	[UNIDADE_USUARIO] [int] NOT NULL,
	[DT_CRIACAO] [datetime] NOT NULL,
	[ATIVO] [bit] NOT NULL,
 CONSTRAINT [PK_CAD_TB005_MENSAGEM] PRIMARY KEY CLUSTERED
(
	[CO_MENSAGEM] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO

-- Legado — sem FK, nada do código usa; mantida pelo nome real.
CREATE TABLE [dbo].[CAD_TB006_VALIDACAO_REGISTRO](
	[CO_REGISTRO_VALIDACAO] [int] IDENTITY(1,1) NOT NULL,
	[CO_VALIDACAO] [int] NOT NULL,
	[CO_DESEMBOLSO] [int] NOT NULL,
	[DE_REGISTRO] [varchar](1000) NULL,
	[TIPO_REGISTRO] [int] NOT NULL,
	[CO_USUARIO] [varchar](7) NULL,
	[DE_USUARIO] [varchar](255) NULL,
	[UNIDADE_USUARIO] [int] NOT NULL,
	[DT_CRIACAO] [datetime] NOT NULL,
	[ATIVO] [bit] NOT NULL,
 CONSTRAINT [PK_CAD_TB006_VALIDACAO_REGISTRO] PRIMARY KEY CLUSTERED
(
	[CO_REGISTRO_VALIDACAO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 3 — DEFAULT constraints
-- ════════════════════════════════════════════════════════════════════════

ALTER TABLE [dbo].[CAD_TB005_MENSAGEM] ADD CONSTRAINT [DF_CAD_TB005_MENSAGEM_ATIVO] DEFAULT ((1)) FOR [ATIVO];
GO

ALTER TABLE [dbo].[CAD_TB006_VALIDACAO_REGISTRO] ADD CONSTRAINT [DF_CAD_TB006_VALIDACAO_REGISTRO_ATIVO] DEFAULT ((1)) FOR [ATIVO];
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 4 — FOREIGN KEYS (só as 5 que existem no schema real)
-- ════════════════════════════════════════════════════════════════════════

ALTER TABLE [dbo].[CAD_TB001_DESEMBOLSO]  WITH CHECK ADD  CONSTRAINT [FK_CAD_TB006_TIPO_PROGRAMA] FOREIGN KEY([CO_PROGRAMA])
REFERENCES [dbo].[CAD_TB006_TIPO_PROGRAMA] ([CO_PROGRAMA]);
GO
ALTER TABLE [dbo].[CAD_TB001_DESEMBOLSO] CHECK CONSTRAINT [FK_CAD_TB006_TIPO_PROGRAMA];
GO

ALTER TABLE [dbo].[CAD_TB001_DESEMBOLSO]  WITH CHECK ADD  CONSTRAINT [FK_CAD_TB007_TIPO_SITUACAO_OBRA] FOREIGN KEY([CO_SITUACAO_OBRA])
REFERENCES [dbo].[CAD_TB007_TIPO_SITUACAO_OBRA] ([CO_SITUACAO_OBRA]);
GO
ALTER TABLE [dbo].[CAD_TB001_DESEMBOLSO] CHECK CONSTRAINT [FK_CAD_TB007_TIPO_SITUACAO_OBRA];
GO

ALTER TABLE [dbo].[CAD_TB001_DESEMBOLSO]  WITH CHECK ADD  CONSTRAINT [FK_CAD_TB008_TIPO_DESEMBOLSO] FOREIGN KEY([CO_TIPO_DESEMBOLSO])
REFERENCES [dbo].[CAD_TB008_TIPO_DESEMBOLSO] ([CO_TIPO_DESEMBOLSO]);
GO
ALTER TABLE [dbo].[CAD_TB001_DESEMBOLSO] CHECK CONSTRAINT [FK_CAD_TB008_TIPO_DESEMBOLSO];
GO

ALTER TABLE [dbo].[CAD_TB002_CONTROLE_DESEMBOLSO]  WITH CHECK ADD  CONSTRAINT [FK_CAD_TB009_TIPO_STATUS_DESEMBOLSO] FOREIGN KEY([CO_STATUS_DESEMBOLSO])
REFERENCES [dbo].[CAD_TB009_TIPO_STATUS_DESEMBOLSO] ([CO_STATUS_DESEMBOLSO]);
GO
ALTER TABLE [dbo].[CAD_TB002_CONTROLE_DESEMBOLSO] CHECK CONSTRAINT [FK_CAD_TB009_TIPO_STATUS_DESEMBOLSO];
GO

ALTER TABLE [dbo].[CAD_TB005_MENSAGEM]  WITH CHECK ADD  CONSTRAINT [FK_CAD_TB010_TIPO_MENSAGEM] FOREIGN KEY([CO_TIPO_MENSAGEM])
REFERENCES [dbo].[CAD_TB010_TIPO_MENSAGEM] ([CO_TIPO_MENSAGEM]);
GO
ALTER TABLE [dbo].[CAD_TB005_MENSAGEM] CHECK CONSTRAINT [FK_CAD_TB010_TIPO_MENSAGEM];
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 5 — SEED das tabelas TIPO (todas as categorias — não é "2 de cada
-- categoria", pois cada linha AQUI já é uma categoria distinta; precisa
-- estar completo por causa das FKs reais).
-- ════════════════════════════════════════════════════════════════════════

INSERT INTO [CAD_TB006_TIPO_PROGRAMA] (CO_PROGRAMA, DE_PROGRAMA) VALUES
    (0, N'Pró-Transporte'),
    (1, N'Pró-Moradia'),
    (2, N'Saneamento Para Todos'),
    (3, N'FGTS-Saúde');
GO

INSERT INTO [CAD_TB010_PROGRAMA] (CO_PROGRAMA, NO_PROGRAMA) VALUES
    (0, N'Pró-Transporte'),
    (1, N'Pró-Moradia'),
    (2, N'Saneamento Para Todos'),
    (3, N'FGTS-Saúde');
GO

INSERT INTO [CAD_TB007_TIPO_SITUACAO_OBRA] (CO_SITUACAO_OBRA, NO_SITUACAO_OBRA) VALUES
    (0, N'Normal'),
    (1, N'Atrasado');
GO

INSERT INTO [CAD_TB008_TIPO_DESEMBOLSO] (CO_TIPO_DESEMBOLSO, NO_TIPO_DESEMBOLSO) VALUES
    (0, N'Normal'),
    (1, N'Adiantamento');
GO

INSERT INTO [CAD_TB009_TIPO_STATUS_DESEMBOLSO] (CO_STATUS_DESEMBOLSO, DE_STATUS_DESEMBOLSO) VALUES
    (1, N'Pendente'),
    (2, N'Analisar'),
    (3, N'Desembolsar'),
    (4, N'Rejeitado'),
    (5, N'Finalizado'),
    (6, N'Cancelado');
GO

INSERT INTO [CAD_TB010_TIPO_MENSAGEM] (CO_TIPO_MENSAGEM, DE_TIPO_MENSAGEM) VALUES
    (0, N'Justificativa'),
    (1, N'Informativo'),
    (2, N'Parecer'),
    (3, N'Observação'),
    (4, N'Rejeição');
GO

INSERT INTO [CAD_TB011_TIPO_SITUACAO_VALIDACAO] (CO_TIPO_SITUACAO_VALIDACAO, DE_TIPO_SITUACAO) VALUES
    (0, N'Analisar'),
    (1, N'Aprovado'),
    (2, N'Negado'),
    (3, N'Ok'),
    (4, N'Erro'),
    (5, N'Pendente');
GO

INSERT INTO [CAD_TB012_TIPO_ORIGEM_VALIDACAO] (CO_TIPO_ORIGEM_VALIDACAO, DE_TIPO_ORIGEM_VALIDACAO) VALUES
    (0, N'Manual'),
    (1, N'Automática por Campo'),
    (3, N'Conferência SIAPF');
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 6 — SEED do catálogo CAD_TB003_VALIDACAO (checklist + SIAPF)
-- ════════════════════════════════════════════════════════════════════════

SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] ON;
GO
INSERT INTO [CAD_TB003_VALIDACAO] (CO_VALIDACAO, DE_VALIDACAO, DT_CRIACAO, DESATIVADO, ORIGEM) VALUES
    (1,    N'Licença de operação',        GETDATE(), 0, 1), -- AutomaticaCampo
    (2,    N'Licença de instalação',      GETDATE(), 0, 1), -- AutomaticaCampo
    (3,    N'Tomador adimplente',         GETDATE(), 0, 0), -- Manual
    (5,    N'Agente Promotor adimplente', GETDATE(), 1, 0), -- Manual, desativado (sem toggle no front)
    (6,    N'Amortização',                GETDATE(), 1, 1), -- desativado: campo Amortizacao foi removido da FPD
    (7,    N'Retorno parcial',            GETDATE(), 0, 1), -- AutomaticaCampo
    (8,    N'Placa local',                GETDATE(), 0, 1), -- AutomaticaCampo
    (9,    N'Excepcionalização',          GETDATE(), 0, 1), -- AutomaticaCampo
    (1005, N'CP alterada',                GETDATE(), 1, 1); -- desativado (sem toggle no front)
GO
SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] OFF;
GO

INSERT INTO [CAD_TB003_VALIDACAO] (DE_VALIDACAO, DT_CRIACAO, CAMPO_VINCULADO, DESATIVADO, ORIGEM) VALUES
    (N'Contrato AF (confronto SIAPF)',          GETDATE(), N'SiapfContratoAf',     0, 3),
    (N'Tomador/Mutuário (confronto SIAPF)',     GETDATE(), N'SiapfMutuarioFinal',  0, 3),
    (N'Agente Promotor (confronto SIAPF)',      GETDATE(), N'SiapfAgentePromotor', 0, 3),
    (N'Programa (confronto SIAPF)',             GETDATE(), N'SiapfPrograma',       0, 3);
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 7 — 2 exemplos na lista de autorizados a gerenciar responsável
-- ════════════════════════════════════════════════════════════════════════
-- Só 2 de exemplo — no dia a dia essa lista tem o gestor, o supervisor e o
-- sênior João (ou quem fizer as vezes deles); troque/adicione as matrículas
-- reais depois.

INSERT INTO [CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE] (MATRICULA, NOME) VALUES
    (N'c900001', N'Exemplo Gestor'),
    (N'c900002', N'Exemplo Supervisor');
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 8 — 2 desembolsos de exemplo por categoria de status (12 no total):
-- Pendente, Analisar, Desembolsar (DRP aguardando), Rejeitado, Finalizado
-- (DRP baixada) e Cancelado — cada um já com o checklist (CAD_TB004)
-- correspondente.
-- ════════════════════════════════════════════════════════════════════════

DECLARE @Cenarios TABLE (
    Idx INT IDENTITY(1,1) PRIMARY KEY,
    Status INT,
    Sufixo VARCHAR(2),
    Mutuario VARCHAR(60),
    ResponsavelAnalise VARCHAR(7) NULL,
    ResponsavelBaixa VARCHAR(7) NULL,
    ResponsavelDesembolso VARCHAR(7) NULL,
    MotivoRejeicao VARCHAR(200) NULL,
    MotivoCancelamento VARCHAR(200) NULL,
    ChecklistSituacao INT
);

INSERT INTO @Cenarios (Status, Sufixo, Mutuario, ResponsavelAnalise, ResponsavelBaixa, ResponsavelDesembolso, MotivoRejeicao, MotivoCancelamento, ChecklistSituacao) VALUES
    (1, N'01', N'Prefeitura Teste Pendente 1',    NULL,       NULL,       NULL, NULL,                                      NULL,                                   0),
    (1, N'02', N'Prefeitura Teste Pendente 2',    NULL,       NULL,       NULL, NULL,                                      NULL,                                   0),
    (2, N'03', N'Prefeitura Teste Analisar 1',    N'c151896', NULL,       NULL, NULL,                                      NULL,                                   0),
    (2, N'04', N'Prefeitura Teste Analisar 2',    N'c151896', NULL,       NULL, NULL,                                      NULL,                                   0),
    (3, N'05', N'Prefeitura Teste Desembolsar 1', N'c151896', N'c151896', NULL, NULL,                                      NULL,                                   1),
    (3, N'06', N'Prefeitura Teste Desembolsar 2', N'c151896', N'c151896', NULL, NULL,                                      NULL,                                   1),
    (4, N'07', N'Prefeitura Teste Rejeitado 1',   N'c151896', N'c151896', NULL, N'Documentação divergente do contrato AF.', NULL,                                  1),
    (4, N'08', N'Prefeitura Teste Rejeitado 2',   N'c151896', N'c151896', NULL, N'CND/CRP vencidos.',                      NULL,                                   1),
    (5, N'09', N'Prefeitura Teste Finalizado 1',  N'c151896', N'c151896', N'c151896', NULL,                                NULL,                                   1),
    (5, N'10', N'Prefeitura Teste Finalizado 2',  N'c151896', N'c151896', N'c151896', NULL,                                NULL,                                   1),
    (6, N'11', N'Prefeitura Teste Cancelado 1',   NULL,       NULL,       NULL, NULL,                                      N'Solicitado pelo GIGOV.',              0),
    (6, N'12', N'Prefeitura Teste Cancelado 2',   NULL,       NULL,       NULL, NULL,                                      N'Contrato encerrado antecipadamente.', 0);

DECLARE @agora DATE = CAST(GETDATE() AS DATE);
DECLARE @idx INT, @status INT, @sufixo VARCHAR(2), @mutuario VARCHAR(60),
        @respAnalise VARCHAR(7), @respBaixa VARCHAR(7), @respDesembolso VARCHAR(7),
        @motivoRejeicao VARCHAR(200), @motivoCancelamento VARCHAR(200), @checklistSituacao INT;
DECLARE @coDesembolso INT, @coControle INT;

DECLARE cenario_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT Idx, Status, Sufixo, Mutuario, ResponsavelAnalise, ResponsavelBaixa, ResponsavelDesembolso,
           MotivoRejeicao, MotivoCancelamento, ChecklistSituacao
    FROM @Cenarios ORDER BY Idx;

OPEN cenario_cursor;
FETCH NEXT FROM cenario_cursor INTO @idx, @status, @sufixo, @mutuario, @respAnalise, @respBaixa, @respDesembolso,
    @motivoRejeicao, @motivoCancelamento, @checklistSituacao;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO [CAD_TB001_DESEMBOLSO] (
        MATRICULA_SOLICITANTE, CO_GIGOV, MATRICULA_GESTOR, DT_SOLICITADO, NU_DESEMBOLSO,
        CO_CONTRATO_AF, CO_CONTRATO_AF_DV, PRIMEIRO_DESEMBOLSO, RECORRENTE,
        AGENTE_FINANCEIRO, CNPJ_AF, MUTUARIO_FINAL, CNPJ_MUTUARIO_FINAL,
        AGENTE_PROMOTOR, CNPJ_AGENTE_PROMOTOR, CO_PROGRAMA, ULTIMO_DESEMBOLSO,
        DT_ENGENHARIA, CO_SITUACAO_OBRA, PERCENTUAL_OBRA, CO_TIPO_DESEMBOLSO,
        PLACA_LOCAL, LICENSA_INSTALACAO, LICENSA_OPERACAO, CRP_NSA,
        SOLICITADO_VI, GLOSSADO_VI, ACEITO_VI, PARTICIPACAO_FGTS,
        VALOR_EMPRESTIMO, DESEMBOLSADO, SALDO_DESEMBOLSAR, EXCEPCIONALIZADO,
        CONTRAPARTIDA, CONTRAPARTIDA_ATUAL, INTEGRALIZADO, SALDO_INTEGRALIZAR,
        CONTRAPARTIDA_ALTERADA, SANEPAR, MOTIVO_REJEICAO
    ) VALUES (
        N'c123' + @sufixo + N'0', N'20' + @sufixo, N'c2000' + @sufixo, @agora, 1,
        N'080' + @sufixo + N'00', @sufixo, 1, 0,
        N'Caixa Economica Federal', N'00360305000104', @mutuario, N'111111110000' + @sufixo,
        N'Agente Promotor Teste', N'222222220000' + @sufixo, 1, 0,
        @agora, 0, 50.0000, 0,
        1, 1, 1, 0,
        50000.00, 0.00, 50000.00, 200000.00,
        210000.00, 0.00, 200000.00, 0,
        10000.00, 0.00, 0.00, 10000.00,
        0, 1, @motivoRejeicao
    );
    SET @coDesembolso = SCOPE_IDENTITY();

    INSERT INTO [CAD_TB002_CONTROLE_DESEMBOLSO] (
        CO_DESEMBOLSO, DT_PRAZO, CO_STATUS_DESEMBOLSO, RESPONSAVEL_ANALISE, RESPONSAVEL_BAIXA,
        RESPONSAVEL_DESEMBOLSO, DT_CONCLUSAO, MOTIVO_CANCELAMENTO
    ) VALUES (
        @coDesembolso, DATEADD(DAY, 2, @agora), @status, @respAnalise, @respBaixa,
        @respDesembolso, CASE WHEN @status IN (3,4,5,6) THEN GETDATE() ELSE NULL END, @motivoCancelamento
    );
    SET @coControle = SCOPE_IDENTITY();

    -- Só o checklist (Manual/AutomaticaCampo — ORIGEM 0/1); a conferência
    -- SIAPF (ORIGEM 3) nasce quando a validação roda de verdade, não no seed.
    INSERT INTO [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] (CO_VALIDACAO, CO_CONTROLE_DESEMBOLSO, DE_VALIDACAO, SITUACAO, ORIGEM, DT_VALIDACAO)
    SELECT CO_VALIDACAO, @coControle, DE_VALIDACAO, @checklistSituacao, ORIGEM, GETDATE()
    FROM [CAD_TB003_VALIDACAO]
    WHERE DESATIVADO = 0 AND ORIGEM IN (0, 1);

    FETCH NEXT FROM cenario_cursor INTO @idx, @status, @sufixo, @mutuario, @respAnalise, @respBaixa, @respDesembolso,
        @motivoRejeicao, @motivoCancelamento, @checklistSituacao;
END

CLOSE cenario_cursor;
DEALLOCATE cenario_cursor;
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 9 — 2 comentários de exemplo, presos aos 2 primeiros itens de
-- checklist criados acima.
-- ════════════════════════════════════════════════════════════════════════

INSERT INTO [CAD_TB005_MENSAGEM] (CO_VALIDACAO, CO_CONTROLE_DESEMBOLSO, DE_MENSAGEM, CO_TIPO_MENSAGEM, CO_USUARIO, DE_USUARIO, UNIDADE_USUARIO, DT_CRIACAO, ATIVO)
SELECT TOP 2 CO_VALIDACAO, CO_CONTROLE_DESEMBOLSO, N'Comentário de teste — ' + DE_VALIDACAO, 1, N'c151896', N'Analista CEFGA Teste', 7175, GETDATE(), 1
FROM [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO]
ORDER BY CO_VALIDACAO_CONTROLE;
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 10 — 2 eventos de exemplo na trilha de auditoria
-- ════════════════════════════════════════════════════════════════════════

INSERT INTO [CAD_TB000_TRILHA_AUDITORIA] (USUARIO, ENDERECO_LOGICO, DT_SOLICITACAO, EVENTO, DESC_EVENTO, RESPOSTA) VALUES
    (N'c123010', N'127.0.0.1', SYSDATETIME(), N'Criar FPD', N'Solicitou novo desembolso pro contrato 0800900-09 (registro de exemplo/seed).', N'ACATADO'),
    (N'c151896', N'127.0.0.1', SYSDATETIME(), N'Validar', N'Validou o desembolso de exemplo (registro de exemplo/seed).', N'ACATADO');
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 11 — Conferência final
-- ════════════════════════════════════════════════════════════════════════

SELECT * FROM [CAD_TB000_TRILHA_AUDITORIA];
SELECT * FROM [CAD_TB001_DESEMBOLSO];
SELECT * FROM [CAD_TB002_CONTROLE_DESEMBOLSO];
SELECT * FROM [CAD_TB003_VALIDACAO] ORDER BY CO_VALIDACAO;
SELECT * FROM [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO];
SELECT * FROM [CAD_TB005_MENSAGEM];
SELECT * FROM [CAD_TB006_VALIDACAO_REGISTRO];
SELECT * FROM [CAD_TB006_TIPO_PROGRAMA];
SELECT * FROM [CAD_TB007_TIPO_SITUACAO_OBRA];
SELECT * FROM [CAD_TB008_TIPO_DESEMBOLSO];
SELECT * FROM [CAD_TB009_TIPO_STATUS_DESEMBOLSO];
SELECT * FROM [CAD_TB010_PROGRAMA];
SELECT * FROM [CAD_TB010_TIPO_MENSAGEM];
SELECT * FROM [CAD_TB011_TIPO_SITUACAO_VALIDACAO];
SELECT * FROM [CAD_TB012_TIPO_ORIGEM_VALIDACAO];
SELECT * FROM [CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE];
