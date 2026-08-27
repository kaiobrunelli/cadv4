-- Seed de teste para RH_TB003_EMPREGADOS_ATIVOS (banco local PlataformaNotificacaoDev).
-- Rodar manualmente após "dotnet ef database update" — não faz parte da migration
-- porque a migration deve refletir só a estrutura real da tabela do RH, não dado de teste.
--
-- Convenção dos códigos (inventada pra simulação local — a tabela real do RH deve
-- ter seu próprio dicionário, ajustar aqui se/quando soubermos os valores reais):
--   CO_FUNCAO:   1 = Coordenador (gestor da coordenação)   |  2 = Analista
--   CO_SITUACAO: 1 = Ativo  |  2 = Férias  |  9 = Desligado/Inativo
--   CO_EVENTUAL: quando CO_SITUACAO = Férias, aponta pro CO_EMPREGADO de quem
--                assume no lugar (ver PlataformaNotificacao.EmpregadoService.
--                ObterGestorAtivoOuEventualAsync).
--
-- 14 coordenações CEFGA01..CEFGA14, 2 empregados cada — exceto CEFGA06, que tem 5
-- (reaproveitando as mesmas matrículas já usadas nos mocks antigos do front/EmpregadoService,
-- pra manter consistência com o que já circulava pela aplicação). Diego Santos (coordenador
-- da CEFGA06) está de férias de propósito, com Elena Ferreira como eventual — cenário pronto
-- pra exercitar o fallback de gestor sem precisar mexer em dado nenhum.

USE PlataformaNotificacaoDev;
GO

DELETE FROM RH_TB003_EMPREGADOS_ATIVOS;
GO

INSERT INTO RH_TB003_EMPREGADOS_ATIVOS
    (MATRICULA, MATRICULA_DV, NOME, DATA_ADMISSAO, DATA_NASCIMENTO, CGC, CO_FUNCAO, TERMO_LGPD, CO_EVENTUAL, COORDENACAO, DATA_ENTRADA, CO_SITUACAO)
VALUES
    -- CEFGA01
    ('c200001', 0, 'Marcos Vieira',    '2014-02-10', '1982-03-11', NULL, 1, 1, NULL, 'CEFGA01', '2014-02-10', 1),
    ('c200002', 0, 'Juliana Prado',    '2019-05-06', '1991-07-22', NULL, 2, 1, NULL, 'CEFGA01', '2019-05-06', 1),
    -- CEFGA02
    ('c200003', 0, 'Rodrigo Alves',    '2013-11-03', '1980-01-15', NULL, 1, 1, NULL, 'CEFGA02', '2013-11-03', 1),
    ('c200004', 0, 'Patrícia Nunes',   '2020-01-20', '1993-09-30', NULL, 2, 1, NULL, 'CEFGA02', '2020-01-20', 1),
    -- CEFGA03
    ('c200005', 0, 'Fernando Rocha',   '2012-08-14', '1979-04-02', NULL, 1, 1, NULL, 'CEFGA03', '2012-08-14', 1),
    ('c200006', 0, 'Camila Duarte',    '2021-03-09', '1994-11-18', NULL, 2, 1, NULL, 'CEFGA03', '2021-03-09', 1),
    -- CEFGA04
    ('c200007', 0, 'Gustavo Lima',     '2016-06-01', '1985-02-27', NULL, 1, 1, NULL, 'CEFGA04', '2016-06-01', 1),
    ('c200008', 0, 'Renata Silva',     '2018-09-17', '1990-06-05', NULL, 2, 1, NULL, 'CEFGA04', '2018-09-17', 1),
    -- CEFGA05
    ('c200009', 0, 'Thiago Moreira',   '2015-04-22', '1983-12-09', NULL, 1, 1, NULL, 'CEFGA05', '2015-04-22', 1),
    ('c200010', 0, 'Larissa Cardoso',  '2020-10-12', '1992-05-14', NULL, 2, 1, NULL, 'CEFGA05', '2020-10-12', 1),
    -- CEFGA06 (5 — matrículas já conhecidas do mock antigo)
    ('c123456', 0, 'Ana Lima',         '2018-03-10', '1990-05-12', NULL, 2, 1, NULL, 'CEFGA06', '2018-03-10', 1),
    ('c102944', 0, 'Bruno Costa',      '2015-07-01', '1985-11-03', NULL, 2, 1, NULL, 'CEFGA06', '2015-07-01', 1),
    ('c110233', 0, 'Diego Santos',     '2016-09-22', '1988-08-08', NULL, 1, 1, NULL, 'CEFGA06', '2016-09-22', 2), -- coordenador, de férias
    ('c145097', 0, 'Elena Ferreira',   '2012-04-05', '1980-01-30', NULL, 2, 1, NULL, 'CEFGA06', '2012-04-05', 1), -- eventual do Diego
    ('c151896', 0, 'Kaio Brunelli',    '2022-02-01', '1996-06-15', NULL, 2, 1, NULL, 'CEFGA06', '2022-02-01', 1),
    -- CEFGA07
    ('c200011', 0, 'Vinícius Teixeira','2014-01-13', '1981-10-19', NULL, 1, 1, NULL, 'CEFGA07', '2014-01-13', 1),
    ('c200012', 0, 'Beatriz Ramos',    '2019-07-08', '1992-03-25', NULL, 2, 1, NULL, 'CEFGA07', '2019-07-08', 1),
    -- CEFGA08
    ('c200013', 0, 'André Barbosa',    '2013-05-27', '1979-08-06', NULL, 1, 1, NULL, 'CEFGA08', '2013-05-27', 1),
    ('c200014', 0, 'Débora Farias',    '2020-02-16', '1993-01-11', NULL, 2, 1, NULL, 'CEFGA08', '2020-02-16', 1),
    -- CEFGA09
    ('c200015', 0, 'Leonardo Pires',   '2017-03-02', '1986-09-23', NULL, 1, 1, NULL, 'CEFGA09', '2017-03-02', 1),
    ('c200016', 0, 'Mariana Castro',   '2021-06-21', '1995-04-17', NULL, 2, 1, NULL, 'CEFGA09', '2021-06-21', 1),
    -- CEFGA10
    ('c200017', 0, 'Eduardo Nascimento','2014-10-30', '1982-12-01', NULL, 1, 1, NULL, 'CEFGA10', '2014-10-30', 1),
    ('c200018', 0, 'Sabrina Correia',  '2019-12-04', '1991-02-08', NULL, 2, 1, NULL, 'CEFGA10', '2019-12-04', 1),
    -- CEFGA11
    ('c200019', 0, 'Felipe Araujo',    '2016-11-11', '1984-07-14', NULL, 1, 1, NULL, 'CEFGA11', '2016-11-11', 1),
    ('c200020', 0, 'Vanessa Monteiro', '2018-04-25', '1989-10-27', NULL, 2, 1, NULL, 'CEFGA11', '2018-04-25', 1),
    -- CEFGA12
    ('c200021', 0, 'Rafael Batista',   '2015-09-19', '1983-05-09', NULL, 1, 1, NULL, 'CEFGA12', '2015-09-19', 1),
    ('c134872', 0, 'Carla Mendes',     '2019-01-15', '1992-02-20', NULL, 2, 1, NULL, 'CEFGA12', '2019-01-15', 1),
    -- CEFGA13
    ('c200022', 0, 'Henrique Souza',   '2012-12-07', '1978-11-28', NULL, 1, 1, NULL, 'CEFGA13', '2012-12-07', 1),
    ('c200023', 0, 'Priscila Andrade', '2020-08-03', '1994-06-16', NULL, 2, 1, NULL, 'CEFGA13', '2020-08-03', 1),
    -- CEFGA14
    ('c200024', 0, 'Otávio Martins',   '2017-07-24', '1987-01-04', NULL, 1, 1, NULL, 'CEFGA14', '2017-07-24', 1),
    ('c200025', 0, 'Fabiana Rezende',  '2021-01-18', '1996-08-21', NULL, 2, 1, NULL, 'CEFGA14', '2021-01-18', 1);
GO

-- Aponta CO_EVENTUAL do Diego (CEFGA06, de férias) pra Elena (mesma coordenação) —
-- feito à parte porque CO_EMPREGADO só existe depois do INSERT (é IDENTITY).
UPDATE eventual
SET eventual.CO_EVENTUAL = subst.CO_EMPREGADO
FROM RH_TB003_EMPREGADOS_ATIVOS eventual
JOIN RH_TB003_EMPREGADOS_ATIVOS subst ON subst.MATRICULA = 'c145097'
WHERE eventual.MATRICULA = 'c110233';
GO
