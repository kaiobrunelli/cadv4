-- ============================================================================
-- Script v6 (revisado) — remove a tabela de anexos criada pelo script
-- anterior (CAD_TB007_ANEXO_DESEMBOLSO). A feature de upload/armazenamento
-- de anexo foi revertida — por enquanto o botão "Anexar licença" no front é
-- só simulação, sem persistência nenhuma.
--
-- Se você nunca chegou a rodar o script antigo que criava essa tabela, este
-- aqui não encontra nada e não faz nada.
--
-- Idempotente — seguro rodar mais de uma vez.
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

IF OBJECT_ID('CAD_TB007_ANEXO_DESEMBOLSO') IS NOT NULL
BEGIN
    DROP TABLE [CAD_TB007_ANEXO_DESEMBOLSO];
END
GO
