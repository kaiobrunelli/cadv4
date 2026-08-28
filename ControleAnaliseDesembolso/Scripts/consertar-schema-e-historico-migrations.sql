-- ============================================================================
-- CONSERTO MANUAL: schema físico x histórico de migrations do EF Core
--
-- Contexto: o LocalDB ficou fora do ar por dias (erro "SQL Server process
-- failed to start" — causa real: registro do LocalDB corrompido, "DataDirectory
-- registry value is missing" pra várias instâncias órfãs, visto no Log de
-- Eventos do Windows). Quando voltou, __EFMigrationsHistory estava vazia (0
-- linhas) mesmo com as tabelas já existindo — então `dotnet ef database
-- update` tentou reaplicar InitialCreate do zero e bateu em "already exists".
--
-- Este script NÃO decide sozinho o que já rodou: cada passo checa o schema
-- atual (nome/tipo de coluna, existência de tabela) antes de alterar algo, e
-- só altera o que realmente falta. Rode do início ao fim, na ordem — é seguro
-- rodar de novo se algo falhar no meio, os passos já aplicados são pulados.
--
-- Rode conectado no database CadDesembolsoDev, com um usuário que tenha
-- permissão de ALTER/CREATE TABLE.
-- ============================================================================

USE [CadDesembolsoDev];
GO

-- ---------------------------------------------------------------------------
-- PASSO 1 — CriarTrilhaAuditoria (20260820144231)
-- Só cria se a trilha de auditoria não existir de jeito nenhum (nem com o
-- nome antigo nem com o novo).
-- ---------------------------------------------------------------------------
IF OBJECT_ID('TB001_TRILHA_AUDITORIA') IS NULL AND OBJECT_ID('CAD_TB000_TRILHA_AUDITORIA') IS NULL
BEGIN
    CREATE TABLE [TB001_TRILHA_AUDITORIA] (
        [ID_SOLICITACAO] int NOT NULL IDENTITY,
        [USUARIO] nvarchar(7) NOT NULL,
        [ENDERECO_LOGICO] nvarchar(50) NOT NULL,
        [DT_SOLICITACAO] datetime2 NOT NULL,
        [EVENTO] nvarchar(100) NOT NULL,
        [DESC_EVENTO] nvarchar(500) NOT NULL,
        [RESPOSTA] nvarchar(20) NULL,
        CONSTRAINT [PK_TB001_TRILHA_AUDITORIA] PRIMARY KEY ([ID_SOLICITACAO])
    );
    PRINT 'Passo 1: TB001_TRILHA_AUDITORIA criada.';
END
ELSE PRINT 'Passo 1: já existia (nome antigo ou novo) — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 2 — RenomearTrilhaAuditoria (20260820151135)
-- Só renomeia se ainda estiver com o nome antigo.
-- ---------------------------------------------------------------------------
IF OBJECT_ID('TB001_TRILHA_AUDITORIA') IS NOT NULL AND OBJECT_ID('CAD_TB000_TRILHA_AUDITORIA') IS NULL
BEGIN
    ALTER TABLE [TB001_TRILHA_AUDITORIA] DROP CONSTRAINT [PK_TB001_TRILHA_AUDITORIA];
    EXEC sp_rename 'TB001_TRILHA_AUDITORIA', 'CAD_TB000_TRILHA_AUDITORIA';
    ALTER TABLE [CAD_TB000_TRILHA_AUDITORIA] ADD CONSTRAINT [PK_CAD_TB000_TRILHA_AUDITORIA] PRIMARY KEY ([ID_SOLICITACAO]);
    PRINT 'Passo 2: TB001_TRILHA_AUDITORIA renomeada pra CAD_TB000_TRILHA_AUDITORIA.';
END
ELSE PRINT 'Passo 2: já estava com o nome novo (ou tabela não existe) — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 3 — AumentarPrecisaoPercentualObra (20260821034844)
-- decimal(18,2) -> decimal(18,4). Só altera se ainda estiver com scale 2.
-- ---------------------------------------------------------------------------
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'PERCENTUAL_OBRA' AND scale = 2
)
BEGIN
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ALTER COLUMN [PERCENTUAL_OBRA] decimal(18,4) NOT NULL;
    PRINT 'Passo 3: PERCENTUAL_OBRA ajustada pra decimal(18,4).';
END
ELSE PRINT 'Passo 3: PERCENTUAL_OBRA já estava decimal(18,4) (ou coluna não existe) — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 4 — RenomearSaldosEConcluidoBool (20260821184834)
-- Renomeia as 2 colunas de saldo e troca CONCLUIDO de date pra bit (sem
-- tentar converter valor — o campo só virou Sim/Não depois, dado antigo em
-- formato date nunca foi uma resposta válida).
-- ---------------------------------------------------------------------------
IF COL_LENGTH('CAD_TB001_DESEMBOLSO', 'SALDO_A_DESEMBOLSAR') IS NOT NULL
BEGIN
    EXEC sp_rename 'CAD_TB001_DESEMBOLSO.SALDO_A_DESEMBOLSAR', 'SALDO_DESEMBOLSAR', 'COLUMN';
    PRINT 'Passo 4a: SALDO_A_DESEMBOLSAR renomeada.';
END
ELSE PRINT 'Passo 4a: já renomeada (ou não existe) — pulado.';
GO

IF COL_LENGTH('CAD_TB001_DESEMBOLSO', 'SALDO_A_INTEGRALIZAR') IS NOT NULL
BEGIN
    EXEC sp_rename 'CAD_TB001_DESEMBOLSO.SALDO_A_INTEGRALIZAR', 'SALDO_INTEGRALIZAR', 'COLUMN';
    PRINT 'Passo 4b: SALDO_A_INTEGRALIZAR renomeada.';
END
ELSE PRINT 'Passo 4b: já renomeada (ou não existe) — pulado.';
GO

IF EXISTS (
    SELECT 1 FROM sys.columns c
    JOIN sys.types t ON t.system_type_id = c.system_type_id AND t.name = 'date'
    WHERE c.object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND c.name = 'CONCLUIDO'
)
BEGIN
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [CONCLUIDO];
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CONCLUIDO] bit NULL;
    PRINT 'Passo 4c: CONCLUIDO recriada como bit (dado antigo em date foi descartado, por design).';
END
ELSE PRINT 'Passo 4c: CONCLUIDO já é bit (ou coluna não existe) — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 5 — RemoverAmortizacao (20260826153908)
-- ---------------------------------------------------------------------------
IF COL_LENGTH('CAD_TB001_DESEMBOLSO', 'AMORTIZACAO') IS NOT NULL
BEGIN
    ALTER TABLE [CAD_TB001_DESEMBOLSO] DROP COLUMN [AMORTIZACAO];
    PRINT 'Passo 5: AMORTIZACAO removida.';
END
ELSE PRINT 'Passo 5: já removida — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 6 — RemapearStatusDesembolso (20260826191252) — NÃO AUTOMÁTICO.
-- Isso é um UPDATE de DADO (renumeração do enum), não de schema — não dá pra
-- detectar com segurança se já rodou só olhando o schema. Rodar 2x por engano
-- EMBARALHA os status dos desembolsos existentes. Antes de decidir, rode:
--
--     SELECT CO_STATUS_DESEMBOLSO, COUNT(*) AS Qtd
--     FROM CAD_TB002_CONTROLE_DESEMBOLSO
--     GROUP BY CO_STATUS_DESEMBOLSO
--     ORDER BY CO_STATUS_DESEMBOLSO;
--
-- Numeração ANTIGA (pré-remap): Pendente=0, Analisar=1, Desembolsar=2, Finalizar=3, Negar=4
-- Numeração NOVA  (pós-remap):  PENDENTE=1, ANALISAR=2, DESEMBOLSAR=3, NEGAR=4, FINALIZAR=5
--
-- - Se aparecer algum registro com valor 0 -> AINDA NÃO rodou. Rode o UPDATE
--   abaixo (descomentado).
-- - Se não aparecer nenhum 0 e os valores fizerem sentido pela numeração NOVA
--   (ex.: 5 = Finalizar) -> já rodou, não faça nada aqui.
-- - Se a tabela estiver vazia (0 linhas) -> tanto faz, pode rodar ou não, sem
--   efeito nenhum.
--
-- UPDATE CAD_TB002_CONTROLE_DESEMBOLSO
-- SET CO_STATUS_DESEMBOLSO = CASE CO_STATUS_DESEMBOLSO
--     WHEN 0 THEN 1
--     WHEN 1 THEN 2
--     WHEN 2 THEN 3
--     WHEN 3 THEN 5
--     WHEN 4 THEN 4
--     ELSE CO_STATUS_DESEMBOLSO
-- END;

-- ---------------------------------------------------------------------------
-- PASSO 7 — TrocarResponsavelBaixaEDesembolso (20260826191441)
-- Detecta pela existência da coluna antiga MATRICULA_BAIXA.
-- ---------------------------------------------------------------------------
IF COL_LENGTH('CAD_TB002_CONTROLE_DESEMBOLSO', 'MATRICULA_BAIXA') IS NOT NULL
BEGIN
    EXEC sp_rename 'CAD_TB002_CONTROLE_DESEMBOLSO.RESPONSAVEL_DESEMBOLSO', 'RESPONSAVEL_DESEMBOLSO_TMP', 'COLUMN';
    EXEC sp_rename 'CAD_TB002_CONTROLE_DESEMBOLSO.MATRICULA_BAIXA', 'RESPONSAVEL_DESEMBOLSO', 'COLUMN';
    EXEC sp_rename 'CAD_TB002_CONTROLE_DESEMBOLSO.RESPONSAVEL_DESEMBOLSO_TMP', 'RESPONSAVEL_BAIXA', 'COLUMN';
    PRINT 'Passo 7: troca cruzada RESPONSAVEL_DESEMBOLSO / MATRICULA_BAIXA / RESPONSAVEL_BAIXA aplicada.';
END
ELSE PRINT 'Passo 7: já aplicada (MATRICULA_BAIXA não existe) — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 8 — AdicionarContratoAo (20260827084400)
-- ---------------------------------------------------------------------------
IF COL_LENGTH('CAD_TB001_DESEMBOLSO', 'CO_CONTRATO_AO') IS NULL
BEGIN
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CO_CONTRATO_AO] nvarchar(20) NOT NULL DEFAULT '';
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CO_CONTRATO_AO_DV] nvarchar(10) NOT NULL DEFAULT '';
    PRINT 'Passo 8: CO_CONTRATO_AO / CO_CONTRATO_AO_DV adicionadas.';
END
ELSE PRINT 'Passo 8: já existiam — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 9 — AddValidacaoValoresSiapf (20260828043144) — a migration desta
-- sessão. Adiciona a coluna GERADA_AUTOMATICAMENTE em CAD_TB003_VALIDACAO.
-- ---------------------------------------------------------------------------
IF COL_LENGTH('CAD_TB003_VALIDACAO', 'GERADA_AUTOMATICAMENTE') IS NULL
BEGIN
    ALTER TABLE [CAD_TB003_VALIDACAO] ADD [GERADA_AUTOMATICAMENTE] bit NOT NULL DEFAULT 0;
    PRINT 'Passo 9: GERADA_AUTOMATICAMENTE adicionada em CAD_TB003_VALIDACAO.';
END
ELSE PRINT 'Passo 9: coluna já existia — pulado.';
GO

-- ---------------------------------------------------------------------------
-- PASSO 10 — item "VALORES" no catálogo (CO_VALIDACAO = 9000)
-- Você já tinha inserido essa linha manualmente — isso aqui só garante que
-- ela tenha GERADA_AUTOMATICAMENTE = 1 (senão ela vaza no checklist da FPD
-- na criação, o que não é o que você quer) e cria se por acaso não existir.
-- CO_VALIDACAO é IDENTITY nesta tabela, por isso o IDENTITY_INSERT.
-- ---------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM CAD_TB003_VALIDACAO WHERE CO_VALIDACAO = 9000)
BEGIN
    UPDATE CAD_TB003_VALIDACAO
    SET DE_VALIDACAO = N'Conferência de valores (SIAPF)',
        DESATIVADO = 0,
        GERADA_AUTOMATICAMENTE = 1
    WHERE CO_VALIDACAO = 9000;
    PRINT 'Passo 10: linha 9000 (VALORES) já existia — GERADA_AUTOMATICAMENTE ajustada pra 1.';
END
ELSE
BEGIN
    SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] ON;
    INSERT INTO CAD_TB003_VALIDACAO (CO_VALIDACAO, DE_VALIDACAO, DT_CRIACAO, DESATIVADO, GERADA_AUTOMATICAMENTE)
    VALUES (9000, N'Conferência de valores (SIAPF)', GETDATE(), 0, 1);
    SET IDENTITY_INSERT [CAD_TB003_VALIDACAO] OFF;
    PRINT 'Passo 10: linha 9000 (VALORES) inserida.';
END
GO

-- ---------------------------------------------------------------------------
-- PASSO 11 — confirmar o schema final antes de mexer no histórico do EF
-- ---------------------------------------------------------------------------
SELECT CO_VALIDACAO, DE_VALIDACAO, DESATIVADO, GERADA_AUTOMATICAMENTE
FROM CAD_TB003_VALIDACAO ORDER BY CO_VALIDACAO;

SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') ORDER BY name;
GO
