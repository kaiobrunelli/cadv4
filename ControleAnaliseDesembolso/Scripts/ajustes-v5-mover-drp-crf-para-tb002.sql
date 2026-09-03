-- ============================================================================
-- Script v5 — move os campos de DRP e CRF do script v4 (que tinham ido pra
-- CAD_TB001_DESEMBOLSO) pra CAD_TB002_CONTROLE_DESEMBOLSO, onde deveriam
-- estar desde o início.
--
-- Passos: (1) cria as colunas na TB002, (2) copia qualquer valor já
-- preenchido na TB001 pra TB002 (join por CO_DESEMBOLSO — defensivo, caso
-- alguém já tenha preenchido algo entre o v4 e este script), (3) remove as
-- colunas da TB001.
--
-- Se o script v4 (ajustes-v4-drp-e-crf.sql) nunca chegou a rodar nesse
-- banco, os passos 2 e 3 simplesmente não encontram as colunas antigas e
-- são pulados — só roda o passo 1 mesmo.
--
-- Idempotente — seguro rodar mais de uma vez.
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

-- ────────────────────────────────────────────────────────────────────────
-- 1) Colunas novas na CAD_TB002_CONTROLE_DESEMBOLSO
-- ────────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'NUMERO_DRP')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [NUMERO_DRP] NVARCHAR(20) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'DV_DRP')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [DV_DRP] NVARCHAR(5) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'SENHA_DRP')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [SENHA_DRP] NVARCHAR(20) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'DT_DRP')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [DT_DRP] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'CRF_AF')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [CRF_AF] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'CRF_TOMADOR')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [CRF_TOMADOR] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'CRF_AP')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [CRF_AP] DATE NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'CRF_AT')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [CRF_AT] DATE NULL;
GO

-- ────────────────────────────────────────────────────────────────────────
-- 2) Copia valores da TB001 (se as colunas antigas ainda existirem lá)
-- ────────────────────────────────────────────────────────────────────────
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'NUMERO_DRP')
BEGIN
    UPDATE c
    SET c.NUMERO_DRP = d.NUMERO_DRP,
        c.DV_DRP     = d.DV_DRP,
        c.SENHA_DRP  = d.SENHA_DRP,
        c.DT_DRP     = d.DT_DRP,
        c.CRF_AF      = d.CRF_AF,
        c.CRF_TOMADOR = d.CRF_TOMADOR,
        c.CRF_AP      = d.CRF_AP,
        c.CRF_AT      = d.CRF_AT
    FROM [CAD_TB002_CONTROLE_DESEMBOLSO] c
    JOIN [CAD_TB001_DESEMBOLSO] d ON d.CO_DESEMBOLSO = c.CO_DESEMBOLSO
    WHERE d.NUMERO_DRP IS NOT NULL OR d.DV_DRP IS NOT NULL OR d.SENHA_DRP IS NOT NULL OR d.DT_DRP IS NOT NULL
       OR d.CRF_AF IS NOT NULL OR d.CRF_TOMADOR IS NOT NULL OR d.CRF_AP IS NOT NULL OR d.CRF_AT IS NOT NULL;
END
GO

-- ────────────────────────────────────────────────────────────────────────
-- 3) Remove as colunas antigas da TB001
-- ────────────────────────────────────────────────────────────────────────
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'NUMERO_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [NUMERO_DRP];
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'DV_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [DV_DRP];
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'SENHA_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [SENHA_DRP];
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'DT_DRP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [DT_DRP];
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_AF')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [CRF_AF];
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_TOMADOR')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [CRF_TOMADOR];
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_AP')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [CRF_AP];
GO
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRF_AT')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [CRF_AT];
GO

SELECT NUMERO_DRP, DV_DRP, SENHA_DRP, DT_DRP, CRF_AF, CRF_TOMADOR, CRF_AP, CRF_AT
FROM [CAD_TB002_CONTROLE_DESEMBOLSO] WHERE 1 = 0;
