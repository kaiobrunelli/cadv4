-- Seed de teste para PLA_NOT_TB003_EMPREGADOS_GIGOV (banco local PlataformaNotificacaoDev).
-- Rodar manualmente após "dotnet ef database update". Diferente de RH_TB003
-- (espelho do RH externo), essa tabela é controle próprio — não tem fonte
-- externa equivalente pra GIGOV, então é cadastro manual mesmo.
--
-- CO_GIGOV é o MESMO número que vai em FichaPedidoDesembolso.CoGigov — uma
-- notificação "pra GIGOV" só alcança quem está cadastrado no número certo
-- (ver EmpregadoService.ObterMatriculasGigovPorNumero). Aqui: 7000 e 7050 com
-- 2 pessoas cada (mostra notificação em grupo), 7171 com 1 só.

USE PlataformaNotificacaoDev;
GO

DELETE FROM PLA_NOT_TB003_EMPREGADOS_GIGOV;
GO

INSERT INTO PLA_NOT_TB003_EMPREGADOS_GIGOV (MATRICULA, NOME, CO_GIGOV, ATIVO)
VALUES
    ('c300001', 'Simone Ribeiro',    '7000', 1),
    ('c300002', 'Alexandre Freitas', '7000', 1),
    ('c300003', 'Tatiane Gomes',     '7171', 1),
    ('c300004', 'Bruno Cavalcanti',  '7050', 1),
    ('c300005', 'Luciana Marques',   '7050', 1);
GO
