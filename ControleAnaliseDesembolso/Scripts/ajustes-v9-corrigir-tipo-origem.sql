-- ============================================================================
-- Script v9 — corrige o tipo da coluna ORIGEM em CAD_TB003_VALIDACAO e
-- CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO: o script v8 criou como TINYINT,
-- mas o EF (ValidacaoConfig/ValidacaoControleDesembolsoConfig, HasConversion<int>())
-- espera INT. O SqlClient não converte TINYINT->INT implicitamente na leitura
-- e estoura:
--   InvalidCastException: Unable to cast object of type 'System.Byte' to
--   type 'System.Int32'.
-- (isso é o que estava por trás do "Data is Null" relatado em ObterTodosDesembolsos
-- — a mensagem que o Swagger mostrou era o texto genérico do handler de exceção,
-- não a exceção real).
--
-- Rode isso SE você já executou o ajustes-v8-recriar-schema-completo.sql
-- (o v8 já foi corrigido no arquivo, mas isso não conserta um banco que já
-- rodou a versão antiga). Não precisa rodar o v8 de novo — isso aqui é só
-- um ALTER, preserva os dados.
--
-- Idempotente — seguro rodar mais de uma vez.
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

IF EXISTS (
    SELECT 1 FROM sys.columns c
    JOIN sys.types t ON t.user_type_id = c.user_type_id
    WHERE c.object_id = OBJECT_ID('CAD_TB003_VALIDACAO') AND c.name = 'ORIGEM' AND t.name = 'tinyint'
)
BEGIN
    ALTER TABLE [CAD_TB003_VALIDACAO] DROP CONSTRAINT [DF_CAD_TB003_ORIGEM];
    ALTER TABLE [CAD_TB003_VALIDACAO] ALTER COLUMN [ORIGEM] INT NOT NULL;
    ALTER TABLE [CAD_TB003_VALIDACAO] ADD CONSTRAINT [DF_CAD_TB003_ORIGEM] DEFAULT ((0)) FOR [ORIGEM];
END
GO

IF EXISTS (
    SELECT 1 FROM sys.columns c
    JOIN sys.types t ON t.user_type_id = c.user_type_id
    WHERE c.object_id = OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO') AND c.name = 'ORIGEM' AND t.name = 'tinyint'
)
BEGIN
    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] DROP CONSTRAINT [DF_CAD_TB004_ORIGEM];
    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ALTER COLUMN [ORIGEM] INT NOT NULL;
    ALTER TABLE [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] ADD CONSTRAINT [DF_CAD_TB004_ORIGEM] DEFAULT ((0)) FOR [ORIGEM];
END
GO

SELECT c.name AS Coluna, t.name AS Tipo
FROM sys.columns c
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE (c.object_id = OBJECT_ID('CAD_TB003_VALIDACAO') OR c.object_id = OBJECT_ID('CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO'))
  AND c.name = 'ORIGEM';
