-- ============================================================================
-- Script v8 — DROP + CREATE completo do schema do CAD, alinhado ao script
-- "verdadeiro" (extraído de [DB7175_RH]) que a área nos passou, MAIS as
-- correções que já fizemos aqui no CAD (DRP, CRF, unificação de validações,
-- novos campos da FPD).
--
-- !!! DESTRUTIVO !!! Isso apaga TODAS as tabelas do CAD e recria vazias.
-- Só rode em CadDesembolsoDev (dev/homologação). NUNCA rode isso contra
-- DB7175_RH ou qualquer banco com dado de produção — não tem passo de
-- migração de dado aqui, é reset total.
--
-- ────────────────────────────────────────────────────────────────────────
-- O QUE MUDOU EM RELAÇÃO AO SCRIPT "VERDADEIRO" QUE VOCÊ PASSOU (leia antes
-- de rodar — são decisões que tomei e podem precisar de ajuste):
--
-- 1) Nomes de tabela/coluna/constraint PK e FK foram mantidos EXATAMENTE
--    como no script verdadeiro, incluindo inconsistências como
--    PK_CAD_TB007_SITUACAO_OBRA (tabela CAD_TB007_TIPO_SITUACAO_OBRA) e
--    PK_CAD_TB006_PROGRAMA (tabela CAD_TB010_PROGRAMA) — são os nomes reais.
-- 2) Só recriei os FKs que o script verdadeiro realmente tinha (5 FKs, todos
--    de tabela "TIPO"). NÃO adicionei FK de CAD_TB002→CAD_TB001,
--    CAD_TB004→CAD_TB002/003 nem CAD_TB005→CAD_TB002/003 — o script real não
--    tem essas, então mantive assim ("regra de PK/FK como está").
-- 3) CAD_TB001_DESEMBOLSO e CAD_TB002_CONTROLE_DESEMBOLSO ganharam as colunas
--    que só existem no nosso local (RECORRENTE, CRP_NSA, MENSAGEM_CEFGA,
--    TEM_CARROCERIA, VEICULO_POSSUI_ADESIVOS, DATA_INICIO_OBRA,
--    DESTINACAO_COLETA_RESIDUOS_SOLIDOS na TB001; MOTIVO_CANCELAMENTO,
--    DT_ULTIMA_CONFERENCIA, NUMERO_DRP/DV_DRP/SENHA_DRP/DT_DRP/CRF_* na
--    TB002) — o script verdadeiro não tinha essas ainda.
-- 4) CAD_TB003_VALIDACAO/CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO usam a forma
--    JÁ UNIFICADA (com ORIGEM, e TB004 com PK substituta CO_VALIDACAO_CONTROLE
--    em vez da PK composta do script verdadeiro) — combinam checklist E
--    conferência de campo/SIAPF na mesma tabela. Isso é intencional: o
--    script verdadeiro reflete o schema ANTES dessa unificação; aqui já
--    entra corrigido. Ver ControleAnaliseDesembolsoService.cs.
-- 5) Criei CAD_TB012_TIPO_ORIGEM_VALIDACAO (não existia) espelhando o enum
--    TipoOrigemValidacao (Manual=0, AutomaticaCampo=1, ConferenciaSiapf=3 —
--    o valor 2/ConferenciaLocal foi removido do enum, ver
--    TipoOrigemValidacao.cs, então fica pulado de propósito). Segui o mesmo
--    padrão das outras tabelas TIPO, mas SEM FK a partir de ORIGEM — o
--    script verdadeiro também não tem FK ligando SITUACAO à
--    CAD_TB011_TIPO_SITUACAO_VALIDACAO, então mantive o mesmo padrão.
-- 6) CAD_TB006_VALIDACAO_REGISTRO e CAD_TB010_PROGRAMA aparecem no script
--    verdadeiro sem nenhum FK apontando pra elas nem nada do nosso código
--    usa essas tabelas hoje (usamos CAD_TB005_MENSAGEM e
--    CAD_TB006_TIPO_PROGRAMA). Mantive as duas do jeito que estão — são os
--    nomes reais — mas se forem lixo de uma versão antiga e puderem ser
--    dropadas de vez, me avise que ajusto o script.
-- 7) Todas as colunas texto do script verdadeiro são VARCHAR (não NVARCHAR);
--    ajustei o EF (IsUnicode(false)) pra bater com isso. As colunas que só
--    existem aqui no nosso local (NUMERO_DRP/DV_DRP/SENHA_DRP) continuam
--    NVARCHAR, como já estavam.
-- 8) PERCENTUAL_OBRA ficou DECIMAL(18,4) (não DECIMAL(5,2) do script
--    verdadeiro) — já tínhamos aumentado essa precisão de propósito
--    (migration AumentarPrecisaoPercentualObra) antes desse alinhamento.
-- 9) DT_CONCLUSAO virou DATETIME (não DATE) — o código grava DateTime.Now
--    (com hora) nesse campo; como DATE, a hora seria descartada.
-- 10) CAD_TB000_TRILHA_AUDITORIA: o script verdadeiro não mostra nenhuma PK
--     nessa tabela (só a IDENTITY, sem CONSTRAINT). Mantive uma PK em
--     ID_SOLICITACAO (nome PK_CAD_TB000_TRILHA_AUDITORIA, que é o que nosso
--     EF já espera) porque toda outra tabela do schema tem PK e o EF Core
--     exige uma chave no modelo. Se a ausência de PK lá é proposital, avise
--     que eu tiro.
-- 11) CO_VALIDACAO=6 ("Amortização") no catálogo de validações ficou
--     DESATIVADO=1: o campo Amortizacao foi removido da FPD faz tempo
--     (migration RemoverAmortizacao) e a regra automática correspondente já
--     está comentada em ValidadorDesembolsoService — sem isso, esse item do
--     checklist nunca seria aprovado e travaria a FPD pra sempre em ANALISAR.
-- 12) UNIDADE_USUARIO em CAD_TB005_MENSAGEM ficou NOT NULL (script verdadeiro
--     mostra NULL) porque a propriedade C# (Mensagem.UnidadeUsuario) é int
--     não-anulável e sempre preenchida — não há como (nem motivo pra)
--     gravar NULL aí.
-- 13) CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE (NOVA — ver script v10): lista
--     de matrículas com permissão total pra vincular/remover qualquer
--     responsável pela análise. Editada direto no banco, sem tela/endpoint.
--
-- Idempotente na parte de DROP (usa IF OBJECT_ID). A parte de CREATE assume
-- que o DROP já rodou antes (schema limpo) — rodar CREATE duas vezes sem
-- dropar no meio dá erro de "already exists", o que é esperado.
-- ============================================================================

USE [CadDesembolsoDev];
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

-- Lixo da unificação anterior (ver ajustes-v4-unificar-validacoes.sql, passo 5)
-- — a tabela "CONFERENCIA_CONTROLE_DESEMBOLSO" (filha, tem FK pra
-- "CAMPO_CONFERENCIA") precisa cair ANTES da "CAMPO_CONFERENCIA" (mãe),
-- renomeada ou não.
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

-- ── CAD_TB006_TIPO_PROGRAMA ────────────────────────────────────────────
CREATE TABLE [dbo].[CAD_TB006_TIPO_PROGRAMA](
	[CO_PROGRAMA] [int] NOT NULL,
	[DE_PROGRAMA] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB006_TIPO_PROGRAMA] PRIMARY KEY CLUSTERED ([CO_PROGRAMA] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB007_TIPO_SITUACAO_OBRA ───────────────────────────────────────
CREATE TABLE [dbo].[CAD_TB007_TIPO_SITUACAO_OBRA](
	[CO_SITUACAO_OBRA] [int] NOT NULL,
	[NO_SITUACAO_OBRA] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB007_SITUACAO_OBRA] PRIMARY KEY CLUSTERED ([CO_SITUACAO_OBRA] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB008_TIPO_DESEMBOLSO ──────────────────────────────────────────
CREATE TABLE [dbo].[CAD_TB008_TIPO_DESEMBOLSO](
	[CO_TIPO_DESEMBOLSO] [int] NOT NULL,
	[NO_TIPO_DESEMBOLSO] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB008_TIPO_DESEMBOLSO] PRIMARY KEY CLUSTERED ([CO_TIPO_DESEMBOLSO] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB009_TIPO_STATUS_DESEMBOLSO ───────────────────────────────────
CREATE TABLE [dbo].[CAD_TB009_TIPO_STATUS_DESEMBOLSO](
	[CO_STATUS_DESEMBOLSO] [int] NOT NULL,
	[DE_STATUS_DESEMBOLSO] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB009_STATUS_DESEMBOLSO] PRIMARY KEY CLUSTERED ([CO_STATUS_DESEMBOLSO] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB010_PROGRAMA (legado — sem FK apontando pra ela, nada do nosso
--    código usa; mantida pelo nome real) ────────────────────────────────
CREATE TABLE [dbo].[CAD_TB010_PROGRAMA](
	[CO_PROGRAMA] [int] NOT NULL,
	[NO_PROGRAMA] [varchar](50) NOT NULL,
 CONSTRAINT [PK_CAD_TB006_PROGRAMA] PRIMARY KEY CLUSTERED ([CO_PROGRAMA] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB010_TIPO_MENSAGEM ────────────────────────────────────────────
CREATE TABLE [dbo].[CAD_TB010_TIPO_MENSAGEM](
	[CO_TIPO_MENSAGEM] [int] NOT NULL,
	[DE_TIPO_MENSAGEM] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CAD_TB010_TIPO_REGISTRO] PRIMARY KEY CLUSTERED ([CO_TIPO_MENSAGEM] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB011_TIPO_SITUACAO_VALIDACAO ──────────────────────────────────
CREATE TABLE [dbo].[CAD_TB011_TIPO_SITUACAO_VALIDACAO](
	[CO_TIPO_SITUACAO_VALIDACAO] [int] NOT NULL,
	[DE_TIPO_SITUACAO] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CAD_TB011_TIPO_SITUACAO_VALIDACAO] PRIMARY KEY CLUSTERED ([CO_TIPO_SITUACAO_VALIDACAO] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB012_TIPO_ORIGEM_VALIDACAO (NOVA — não existia; espelha o enum
--    TipoOrigemValidacao) ───────────────────────────────────────────────
CREATE TABLE [dbo].[CAD_TB012_TIPO_ORIGEM_VALIDACAO](
	[CO_TIPO_ORIGEM_VALIDACAO] [int] NOT NULL,
	[DE_TIPO_ORIGEM_VALIDACAO] [varchar](100) NOT NULL,
 CONSTRAINT [PK_CAD_TB012_TIPO_ORIGEM_VALIDACAO] PRIMARY KEY CLUSTERED ([CO_TIPO_ORIGEM_VALIDACAO] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE (NOVA — ver script v10) ─────
-- Matrículas com permissão total pra vincular/remover qualquer responsável
-- pela análise. Editada direto aqui no banco (INSERT/DELETE), sem deploy.
CREATE TABLE [dbo].[CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE](
	[MATRICULA] [varchar](7) NOT NULL,
	[NOME] [varchar](100) NULL,
	[DT_INCLUSAO] [datetime] NOT NULL CONSTRAINT [DF_CAD_TB013_DT_INCLUSAO] DEFAULT (GETDATE()),
 CONSTRAINT [PK_CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE] PRIMARY KEY CLUSTERED ([MATRICULA] ASC)
) ON [PRIMARY];
GO

-- ── CAD_TB000_TRILHA_AUDITORIA ─────────────────────────────────────────
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

-- ── CAD_TB001_DESEMBOLSO ───────────────────────────────────────────────
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

-- ── CAD_TB002_CONTROLE_DESEMBOLSO ──────────────────────────────────────
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

-- ── CAD_TB003_VALIDACAO (já com ORIGEM — catálogo unificado) ───────────
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

-- ── CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO (PK substituta — guarda tanto
--    o checklist quanto o histórico de conferência/SIAPF) ───────────────
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

-- ── CAD_TB005_MENSAGEM ─────────────────────────────────────────────────
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

-- ── CAD_TB006_VALIDACAO_REGISTRO (legado — sem FK, nada do nosso código usa;
--    mantida pelo nome real) ───────────────────────────────────────────
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
-- PARTE 5 — SEED das tabelas TIPO (obrigatório: CO_PROGRAMA/CO_SITUACAO_OBRA/
-- CO_TIPO_DESEMBOLSO/CO_STATUS_DESEMBOLSO/CO_TIPO_MENSAGEM têm FK real agora —
-- sem seed, a primeira FPD/comentário/status gravado já quebra por violação
-- de FK). Códigos batem 1:1 com os enums em Domain/Enums.
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

-- Checklist manual/automático — CO_VALIDACAO precisa ser EXATAMENTE esses
-- números (casam com os hardcoded em ValidadorDesembolsoService/front-end).
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

-- Conferência SIAPF — sem número fixo (casam pela CHAVE em CAMPO_VINCULADO,
-- não pelo CO_VALIDACAO), então entram por IDENTITY normal (próximo = 1006+).
INSERT INTO [CAD_TB003_VALIDACAO] (DE_VALIDACAO, DT_CRIACAO, CAMPO_VINCULADO, DESATIVADO, ORIGEM) VALUES
    (N'Contrato AF (confronto SIAPF)',          GETDATE(), N'SiapfContratoAf',     0, 3),
    (N'Tomador/Mutuário (confronto SIAPF)',     GETDATE(), N'SiapfMutuarioFinal',  0, 3),
    (N'Agente Promotor (confronto SIAPF)',      GETDATE(), N'SiapfAgentePromotor', 0, 3),
    (N'Programa (confronto SIAPF)',             GETDATE(), N'SiapfPrograma',       0, 3);
GO

-- ════════════════════════════════════════════════════════════════════════
-- PARTE 7 — Conferência final
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
