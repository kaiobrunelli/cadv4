-- ============================================================================
-- Script v10 — cria CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE: lista de
-- matrículas com permissão TOTAL pra vincular/remover QUALQUER responsável
-- pela análise em QUALQUER desembolso (hoje: gestor, supervisor e o sênior
-- João — o cargo dele é comum como o dos outros técnicos, mas entra na
-- lista pela mesma razão prática que gestor/supervisor).
--
-- Fora dessa lista, cada analista só mexe na PRÓPRIA atribuição: pode se
-- marcar como responsável, ou se remover quando é ele o atual — ver
-- ValidarAutorizacaoVincularResponsavel em ControleAnaliseDesembolsoService.cs.
--
-- Tabela pensada pra ser editada direto aqui no banco (INSERT pra adicionar,
-- DELETE pra remover alguém) — sem precisar de deploy. Não tem tela nem
-- endpoint gerenciando isso, de propósito.
--
-- Depois de rodar, insira as matrículas reais (troque os exemplos abaixo).
--
-- Idempotente — seguro rodar mais de uma vez.
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

IF OBJECT_ID('CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE') IS NULL
BEGIN
    CREATE TABLE [dbo].[CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE](
        [MATRICULA] [varchar](7) NOT NULL,
        [NOME] [varchar](100) NULL,
        [DT_INCLUSAO] [datetime] NOT NULL CONSTRAINT [DF_CAD_TB013_DT_INCLUSAO] DEFAULT (GETDATE()),
     CONSTRAINT [PK_CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE] PRIMARY KEY CLUSTERED
    (
        [MATRICULA] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];
END
GO

-- Exemplo — troque pelas matrículas reais do gestor, supervisor e sênior João:
-- INSERT INTO [CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE] (MATRICULA, NOME) VALUES
--     (N'c000001', N'Gestor'),
--     (N'c000002', N'Supervisor'),
--     (N'c000003', N'João (sênior)');
-- GO

SELECT * FROM [CAD_TB013_AUTORIZADO_RESPONSAVEL_ANALISE];
