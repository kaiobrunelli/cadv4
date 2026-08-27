-- ============================================================================
-- Catálogo de validações (CAD_TB003_VALIDACAO) — espelha 1:1 os itens
-- hardcoded na Etapa 2 (Verificações) do Preencher/Editar FPD
-- (PainelPreencherFpdEtapas.razor) e as regras automáticas em
-- ValidadorDesembolsoService._regras.
--
-- IMPORTANTE: CO_VALIDACAO é IDENTITY, mas os números abaixo (1, 2, 6, 7, 8,
-- 9, 1005) precisam ser EXATAMENTE esses — são os mesmos números hardcoded
-- em ValidadorDesembolsoService, que casa CoValidacao com o campo bool da
-- ficha pra aprovar automaticamente. Se os IDs saírem diferentes (por
-- IDENTITY normal), a validação automática nunca encontra o item certo.
-- Por isso o script usa SET IDENTITY_INSERT.
--
-- DE_VALIDACAO precisa bater EXATAMENTE com o "titulo" passado pro
-- ItemToggle no front (é assim que ResolverCoValidacao acha o item pelo
-- texto) — mesma acentuação, sem espaço a mais.
--
-- "Agente Promotor adimplente" (5) e "CP alterada" (1005) estão com
-- DESATIVADO = 1 porque os toggles deles estão comentados no front hoje
-- (ver PainelPreencherFpdEtapas.razor linhas 172 e 177) — se ficassem
-- ativos, toda FPD nova ganharia um item de checklist que a GIGOV nunca
-- consegue preencher (sem toggle na tela), e ficaria pendente pra sempre.
-- Quando descomentar o toggle correspondente, muda o DESATIVADO pra 0 aqui.
--
-- O número "5" pra "Agente Promotor adimplente" é uma inferência minha (só
-- achei confirmação explícita de 1, 2, 3 e 1005 nos comentários do código;
-- "novo" sugere que não tem número documentado) — confira se bate com o
-- que já existe no seu banco real antes de aplicar em produção.
--
-- Rode conectado no database CadDesembolsoDev.
-- ============================================================================

USE [CadDesembolsoDev];
GO

SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] ON;
GO

INSERT INTO [CAD_TB003_VALIDACAO] (CO_VALIDACAO, DE_VALIDACAO, DT_CRIACAO, DESATIVADO) VALUES
    (1,    N'Licença de operação',           GETDATE(), 0),
    (2,    N'Licença de instalação',         GETDATE(), 0),
    (3,    N'Tomador adimplente',            GETDATE(), 0),
    (5,    N'Agente Promotor adimplente',    GETDATE(), 1), -- toggle comentado no front
    (6,    N'Amortização',                   GETDATE(), 0),
    (7,    N'Retorno parcial',                GETDATE(), 0),
    (8,    N'Placa local',                    GETDATE(), 0),
    (9,    N'Excepcionalização',              GETDATE(), 0),
    (1005, N'CP alterada',                    GETDATE(), 1); -- toggle comentado no front
GO

SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] OFF;
GO

SELECT * FROM [CAD_TB003_VALIDACAO] ORDER BY CO_VALIDACAO;
