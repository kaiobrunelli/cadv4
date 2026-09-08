-- ============================================================================
-- Script v7 — as validações de campo simples da FPD que viviam como item de
-- checklist ORIGEM=2 (ConferenciaLocal: CNPJ, matrícula do Gestor, Programa,
-- Agente Financeiro, Tomador, Agente Promotor, GIGOV, Contrato AF, data de
-- engenharia) viraram checagem síncrona na criação/reenvio da FPD — ver
-- ValidarDadosObrigatoriosFpd em ControleAnaliseDesembolsoService.cs. A API
-- agora rejeita a FPD na hora (mensagem pra snackbar) em vez de deixar entrar
-- e virar pendência de checklist.
--
-- A tabela CAD_TB003_VALIDACAO (catálogo de tipos de validação) passa a
-- cadastrar só o que continua em ORIGEM=0/1 (Manual/AutomaticaCampo — checklist
-- SIM/NÃO/NSA) e ORIGEM=3 (ConferenciaSiapf). Os itens ORIGEM=2 são apenas
-- DESATIVADOS (não apagados): apagá-los quebraria a FK de
-- CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO nas linhas de histórico das FPDs
-- já validadas antes dessa mudança. O enum TipoOrigemValidacao (C#) também
-- não usa mais o valor 2 — ver comentário em TipoOrigemValidacao.cs.
--
-- Idempotente — seguro rodar mais de uma vez.
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

UPDATE [CAD_TB003_VALIDACAO]
   SET [DESATIVADO] = 1
 WHERE [ORIGEM] = 2
   AND [DESATIVADO] = 0;
GO

SELECT * FROM [CAD_TB003_VALIDACAO] ORDER BY CO_VALIDACAO;
