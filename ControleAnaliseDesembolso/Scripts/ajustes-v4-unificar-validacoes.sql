-- ============================================================================
-- Script v4 — unifica o Checklist de validação (CAD_TB003/CAD_TB004) e a
-- Conferência de campos/SIAPF (CAD_TB005/CAD_TB006) numa única tabela, já
-- que os dois são conceitualmente "validação da FPD":
--
--   1) CAD_TB003_VALIDACAO ganha a coluna ORIGEM e passa a ser o catálogo
--      único (checklist manual/automático + campos de conferência local/SIAPF).
--      A coluna CAMPO_VINCULADO (sempre NULL até hoje) passa a guardar, pros
--      itens de conferência, a mesma CHAVE que CAD_TB005_CAMPO_CONFERENCIA
--      guardava — é o que ExecutarConferenciaCamposInterno usa pra achar a
--      regra em _regrasConferencia/_regrasConferenciaSiapf.
--   2) CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO troca a PK composta
--      (CO_VALIDACAO, CO_CONTROLE_DESEMBOLSO) por uma PK substituta
--      (CO_VALIDACAO_CONTROLE, identity) e ganha ORIGEM/MENSAGEM/DT_VALIDACAO
--      — agora guarda tanto o checklist (1 linha por item, mutada em lugar)
--      quanto o histórico da conferência (1 linha nova a cada execução, nunca
--      mais apagada — antes CAD_TB006 era limpa e recriada toda vez).
--   3) CAD_TB005/CAD_TB006 são migradas pra dentro de CAD_TB003/CAD_TB004 e
--      renomeadas pra _OLD_* (não apagadas ainda, pra dar segurança de rollback).
--
-- ORIGEM: 0=Manual, 1=AutomaticaCampo, 2=ConferenciaLocal, 3=ConferenciaSiapf
--   (mesmos números do enum OrigemValidacao em C#).
-- SITUACAO (CAD_TB004, unificada): 0=ANALISAR, 1=APROVADO, 2=NEGADO (mantidos
--   pra não reescrever linhas existentes do checklist), 3=OK, 4=ERRO,
--   5=PENDENTE (novos, remapeados a partir do TipoSituacaoConferencia antigo:
--   OK=1→3, PENDENTE=2→5, ERRO=3→4).
--
-- Idempotente — seguro rodar mais de uma vez. Rode conectado no database
-- CadDesembolsoDev. Faça backup antes de rodar em produção.
-- ============================================================================

USE [CadDesembolsoDev];
GO

-- ────────────────────────────────────────────────────────────────────────
-- 1) CAD_TB003_VALIDACAO — coluna ORIGEM
-- ────────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB003_VALIDACAO') AND name = 'ORIGEM')
    ALTER TABLE [CAD_TB003_VALIDACAO] ADD [ORIGEM] TINYINT NOT NULL CONSTRAINT DF_CAD_TB003_ORIGEM DEFAULT (0);
GO

-- Itens hoje auto-aprovados por ValidadorDesembolsoService a partir de um
-- campo booleano da FPD (ver _regras em ValidadorDesembolsoService.cs) —
-- os demais (ex. Tomador adimplente) ficam ORIGEM=0 (Manual), já é o default.
UPDATE [CAD_TB003_VALIDACAO]
   SET [ORIGEM] = 1 -- AutomaticaCampo
 WHERE [CO_VALIDACAO] IN (1, 2, 6, 7, 8, 9, 1005);
GO

-- ────────────────────────────────────────────────────────────────────────
-- 2) Migra o catálogo CAD_TB005_CAMPO_CONFERENCIA pra dentro de CAD_TB003
--    (só roda se CAD_TB005 ainda existir e ainda não tiver sido migrada).
-- ────────────────────────────────────────────────────────────────────────
IF OBJECT_ID('CAD_TB005_CAMPO_CONFERENCIA') IS NOT NULL
BEGIN
    INSERT INTO [CAD_TB003_VALIDACAO] (DE_VALIDACAO, DT_CRIACAO, CAMPO_VINCULADO, DESATIVADO, ORIGEM)
    SELECT
        cc.DE_CAMPO,
        cc.DT_CRIACAO,
        cc.CHAVE,
        cc.DESATIVADO,
        CASE WHEN cc.CHAVE IN (N'SiapfContratoAf', N'SiapfMutuarioFinal', N'SiapfAgentePromotor', N'SiapfPrograma')
             THEN 3  -- ConferenciaSiapf
             ELSE 2  -- ConferenciaLocal
        END
    FROM [CAD_TB005_CAMPO_CONFERENCIA] cc
    WHERE NOT EXISTS (
        SELECT 1 FROM [CAD_TB003_VALIDACAO] v WHERE v.CAMPO_VINCULADO = cc.CHAVE
    );
END
GO

-- ────────────────────────────────────────────────────────────────────────
-- 3) CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO — troca a PK composta por uma
--    PK substituta e adiciona ORIGEM/MENSAGEM/DT_VALIDACAO.
-- ────────────────────────────────────────────────────────────────────────
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO') AND name = 'CO_VALIDACAO_CONTROLE')
BEGIN
    DECLARE @pk NVARCHAR(200);
    SELECT @pk = kc.name
    FROM sys.key_constraints kc
    WHERE kc.parent_object_id = OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO')
      AND kc.type = 'PK';

    IF @pk IS NOT NULL
        EXEC('ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] DROP CONSTRAINT [' + @pk + ']');

    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ADD [CO_VALIDACAO_CONTROLE] INT IDENTITY(1,1) NOT NULL;
    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ADD CONSTRAINT PK_CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO PRIMARY KEY ([CO_VALIDACAO_CONTROLE]);

    CREATE INDEX IX_CAD_TB004_VALIDACAO_CONTROLE ON [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ([CO_VALIDACAO], [CO_CONTROLE_DESEMBOLSO]);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO') AND name = 'ORIGEM')
    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ADD [ORIGEM] TINYINT NOT NULL CONSTRAINT DF_CAD_TB004_ORIGEM DEFAULT (0);
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO') AND name = 'MENSAGEM')
    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ADD [MENSAGEM] NVARCHAR(1000) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO') AND name = 'DT_VALIDACAO')
    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ADD [DT_VALIDACAO] DATETIME NULL;
GO

-- Popula ORIGEM/DT_VALIDACAO das linhas de checklist já existentes (o
-- checklist nunca teve histórico, então DT_VALIDACAO vira a data desta
-- migração — não tem timestamp original pra recuperar).
UPDATE vcd
   SET vcd.[ORIGEM] = v.[ORIGEM],
       vcd.[DT_VALIDACAO] = ISNULL(vcd.[DT_VALIDACAO], GETDATE())
  FROM [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] vcd
  JOIN [CAD_TB003_VALIDACAO] v ON v.CO_VALIDACAO = vcd.CO_VALIDACAO
 WHERE vcd.[DT_VALIDACAO] IS NULL;
GO
ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ALTER COLUMN [DT_VALIDACAO] DATETIME NOT NULL;
GO

-- ────────────────────────────────────────────────────────────────────────
-- 4) Migra o histórico CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO pra dentro
--    de CAD_TB004, remapeando CO_CAMPO -> novo CO_VALIDACAO (via CHAVE) e
--    SITUACAO (OK=1->3, PENDENTE=2->5, ERRO=3->4).
-- ────────────────────────────────────────────────────────────────────────
IF OBJECT_ID('CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] WHERE [ORIGEM] IN (2, 3))
BEGIN
    INSERT INTO [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO]
        (CO_VALIDACAO, CO_CONTROLE_DESEMBOLSO, DE_VALIDACAO, CAMPO_VINCULADO, SITUACAO, ORIGEM, MENSAGEM, DT_VALIDACAO)
    SELECT
        v.CO_VALIDACAO,
        cc.CO_CONTROLE_DESEMBOLSO,
        cc.DE_CAMPO,
        v.CAMPO_VINCULADO,
        CASE cc.CO_SITUACAO WHEN 1 THEN 3 WHEN 2 THEN 5 WHEN 3 THEN 4 END, -- OK/PENDENTE/ERRO -> códigos novos
        v.ORIGEM,
        cc.MENSAGEM,
        cc.DT_CONFERENCIA
    FROM [CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO] cc
    JOIN [CAD_TB005_CAMPO_CONFERENCIA] camp ON camp.CO_CAMPO = cc.CO_CAMPO
    JOIN [CAD_TB003_VALIDACAO] v ON v.CAMPO_VINCULADO = camp.CHAVE;
END
GO

-- ────────────────────────────────────────────────────────────────────────
-- 5) Renomeia (não apaga) as tabelas antigas — dropar de vez só depois de
--    validar que tudo funciona com a tabela unificada.
-- ────────────────────────────────────────────────────────────────────────
IF OBJECT_ID('CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO') IS NOT NULL AND OBJECT_ID('CAD_TB006_OLD_CONFERENCIA_CONTROLE_DESEMBOLSO') IS NULL
    EXEC sp_rename 'CAD_TB006_CONFERENCIA_CONTROLE_DESEMBOLSO', 'CAD_TB006_OLD_CONFERENCIA_CONTROLE_DESEMBOLSO';
GO
IF OBJECT_ID('CAD_TB005_CAMPO_CONFERENCIA') IS NOT NULL AND OBJECT_ID('CAD_TB005_OLD_CAMPO_CONFERENCIA') IS NULL
    EXEC sp_rename 'CAD_TB005_CAMPO_CONFERENCIA', 'CAD_TB005_OLD_CAMPO_CONFERENCIA';
GO

SELECT * FROM [CAD_TB003_VALIDACAO] ORDER BY CO_VALIDACAO;
SELECT TOP 50 * FROM [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ORDER BY CO_VALIDACAO_CONTROLE DESC;
