-- ============================================================================
-- Script v3 — passa a conferência de campos (CAD_TB005/CAD_TB006) a confrontar
-- o FPD com o cadastro REAL do contrato consultado no SIAPF, em vez de só
-- checar consistência interna do próprio FPD.
--
-- Antes: ValidadorDesembolsoService/ISiapfService rodavam contra um mock
-- fixo (RedeCaixaUtilitario\Application\SiapfService.cs simulado). Agora o
-- CAD tem uma implementação real de ISiapfService (mesmo projeto, região
-- Cadastro portada do SIAPF de verdade) — este script só adiciona ao
-- catálogo as novas chaves que o código passa a reconhecer em
-- _regrasConferenciaSiapf (ControleAnaliseDesembolsoService.cs).
--
-- CHAVE precisa bater exatamente com as chaves de _regrasConferenciaSiapf.
-- Pra adicionar mais uma comparação real com o SIAPF no futuro: (1) insere
-- a linha aqui com uma CHAVE nova, (2) adiciona a entrada correspondente em
-- _regrasConferenciaSiapf — até lá o campo aparece como "Pendente".
--
-- Idempotente — seguro rodar mais de uma vez.
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

IF NOT EXISTS (SELECT 1 FROM [CAD_TB005_CAMPO_CONFERENCIA] WHERE CHAVE = N'SiapfContratoAf')
    INSERT INTO [CAD_TB005_CAMPO_CONFERENCIA] (CHAVE, DE_CAMPO, DT_CRIACAO, DESATIVADO) VALUES (N'SiapfContratoAf', N'Contrato AF (confronto SIAPF)', GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM [CAD_TB005_CAMPO_CONFERENCIA] WHERE CHAVE = N'SiapfMutuarioFinal')
    INSERT INTO [CAD_TB005_CAMPO_CONFERENCIA] (CHAVE, DE_CAMPO, DT_CRIACAO, DESATIVADO) VALUES (N'SiapfMutuarioFinal', N'Tomador/Mutuário (confronto SIAPF)', GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM [CAD_TB005_CAMPO_CONFERENCIA] WHERE CHAVE = N'SiapfAgentePromotor')
    INSERT INTO [CAD_TB005_CAMPO_CONFERENCIA] (CHAVE, DE_CAMPO, DT_CRIACAO, DESATIVADO) VALUES (N'SiapfAgentePromotor', N'Agente Promotor (confronto SIAPF)', GETDATE(), 0);
IF NOT EXISTS (SELECT 1 FROM [CAD_TB005_CAMPO_CONFERENCIA] WHERE CHAVE = N'SiapfPrograma')
    INSERT INTO [CAD_TB005_CAMPO_CONFERENCIA] (CHAVE, DE_CAMPO, DT_CRIACAO, DESATIVADO) VALUES (N'SiapfPrograma', N'Programa (confronto SIAPF)', GETDATE(), 0);
GO

SELECT * FROM [CAD_TB005_CAMPO_CONFERENCIA] ORDER BY CO_CAMPO;
