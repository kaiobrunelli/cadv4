-- ============================================================================
-- Script v4 — adiciona os campos de DRP e CRF na CAD_TB001_DESEMBOLSO.
-- Todos nulos até serem preenchidos (DRP só depois de emitida; CRF só
-- depois de conferido pra cada parte do contrato).
--
-- NUMERO_DRP, DV_DRP, SENHA_DRP, DT_DRP — dados da DRP emitida pro desembolso.
-- CRF_AF, CRF_TOMADOR, CRF_AP, CRF_AT — data do CRF (Certificado de
-- Regularidade do FGTS) de cada parte: Agente Financeiro, Tomador,
-- Agente Promotor, Agente Técnico.
--
-- Idempotente — seguro rodar mais de uma vez.
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'NUMERO_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [NUMERO_DRP] NVARCHAR(20) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'DV_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [DV_DRP] NVARCHAR(5) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'SENHA_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [SENHA_DRP] NVARCHAR(20) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'DT_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [DT_DRP] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_AF')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CRF_AF] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_TOMADOR')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CRF_TOMADOR] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_AP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CRF_AP] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_AT')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CRF_AT] DATE NULL;
GO

SELECT NUMERO_DRP, DV_DRP, SENHA_DRP, DT_DRP, CRF_AF, CRF_TOMADOR, CRF_AP, CRF_AT
FROM [CAD_TB001_DESEMBOLSO] WHERE 1 = 0;
