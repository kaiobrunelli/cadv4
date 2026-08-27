-- ============================================================================
-- SEED de teste — 4 empregados na CEFGA06 + 2 na CEFGA12, em
-- RH_TB003_EMPREGADOS_ATIVOS. Dado FICTÍCIO, só pra testar localmente
-- "Vincular responsável" e o lookup de gestor (ObterGestorAtivoOuEventualAsync).
--
-- CO_SITUACAO = 1 (ativo — EmpregadoCADService filtra CodigoSituacao != 9)
-- CO_FUNCAO   = 1 no primeiro de cada coordenação (é o único código que a
--               aplicação usa de verdade, pra achar o "gestor" da coordenação
--               em NotificacaoService.ObterGestorAtivoOuEventualAsync).
--
-- Rode conectado no PlataformaNotificacaoDev.
-- ============================================================================

USE [PlataformaNotificacaoDev];
GO

INSERT INTO [RH_TB003_EMPREGADOS_ATIVOS] (
    MATRICULA, MATRICULA_DV, NOME, DATA_ADMISSAO, DATA_NASCIMENTO,
    CGC, CO_FUNCAO, TERMO_LGPD, CO_EVENTUAL, COORDENACAO, DATA_ENTRADA, CO_SITUACAO
) VALUES
    -- CEFGA06 (4 empregados, o primeiro é o coordenador)
    (N'c200101', 1, N'Marina Alves Coutinho',   '2018-03-12', '1988-05-20', 11111111, 1,    1, NULL, N'CEFGA06', '2018-03-12', 1),
    (N'c200102', 2, N'Rafael Souza Prado',      '2019-07-01', '1990-11-02', 22222222, NULL, 1, NULL, N'CEFGA06', '2019-07-01', 1),
    (N'c200103', 3, N'Juliana Ramos Nogueira',  '2020-01-15', '1992-02-14', 33333333, NULL, 1, NULL, N'CEFGA06', '2020-01-15', 1),
    (N'c200104', 4, N'Thiago Martins Beserra',  '2021-09-05', '1985-08-30', 44444444, NULL, 1, NULL, N'CEFGA06', '2021-09-05', 1),

    -- CEFGA12 (2 empregados, o primeiro é o coordenador)
    (N'c200105', 5, N'Patrícia Lopes Cardoso',  '2017-04-20', '1987-12-01', 55555555, 1,    1, NULL, N'CEFGA12', '2017-04-20', 1),
    (N'c200106', 6, N'Bruno Andrade Ferreira',  '2022-02-10', '1994-06-18', 66666666, NULL, 1, NULL, N'CEFGA12', '2022-02-10', 1);
GO

SELECT MATRICULA, NOME, COORDENACAO, CO_FUNCAO, CO_SITUACAO
FROM [RH_TB003_EMPREGADOS_ATIVOS]
ORDER BY COORDENACAO, CO_FUNCAO DESC, MATRICULA;
GO
