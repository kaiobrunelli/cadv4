-- ============================================================================
-- REFERÊNCIA para inserir desembolsos na mão no CadDesembolsoDev.
--
-- Ordem de inserção (respeita as FKs):
--   1) CAD_TB001_DESEMBOLSO           — a ficha em si (dados do contrato)
--   2) CAD_TB002_CONTROLE_DESEMBOLSO  — o controle de fluxo (status/prazo),
--      1 linha por desembolso, aponta pro CO_DESEMBOLSO da TB001.
--      SEM ESSA LINHA O DESEMBOLSO NÃO APARECE EM LUGAR NENHUM NA TELA.
--   3) CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO — o checklist (opcional, mas
--      sem isso o card mostra "0 de 0" validações). 1 linha por item do
--      catálogo (CAD_TB003_VALIDACAO), copiando os que estiverem ativos.
--   4) CAD_TB005_MENSAGEM — comentários/justificativas (totalmente opcional).
--
-- Use SCOPE_IDENTITY() pra pegar o ID que acabou de ser gerado (IDENTITY)
-- e usar na tabela seguinte, como nos exemplos abaixo.
-- ============================================================================

USE [CadDesembolsoDev];
GO

-- ────────────────────────────────────────────────────────────────────────
-- 1) CAD_TB001_DESEMBOLSO
-- Y = obrigatório (NOT NULL) | N = opcional (aceita NULL)
-- ────────────────────────────────────────────────────────────────────────
DECLARE @coDesembolso INT;

INSERT INTO [CAD_TB001_DESEMBOLSO] (
    MATRICULA_SOLICITANTE,      -- Y  nvarchar(7)   — matrícula de quem pediu (ex: 'c123456')
    CO_GIGOV,                   -- Y  nchar(4)      — código GIGOV (4 dígitos, ex: '1001')
    MATRICULA_GESTOR,           -- Y  nvarchar(7)
    DT_SOLICITADO,               -- Y  date
    NU_DESEMBOLSO,               -- Y  int
    CO_CONTRATO_AF,              -- Y  nvarchar(20)
    CO_CONTRATO_AF_DV,           -- Y  nvarchar(10)
    PRIMEIRO_DESEMBOLSO,         -- Y  bit           — 0 ou 1
    AGENTE_FINANCEIRO,           -- Y  nvarchar(255)
    CNPJ_AF,                     -- Y  nchar(14)
    MUTUARIO_FINAL,              -- Y  nvarchar(255)
    CNPJ_MUTUARIO_FINAL,         -- Y  nchar(14)
    AGENTE_TECNICO_OPERADOR,     -- N  nvarchar(255) — pode ser NULL
    CNPJ_AGENTE_TECNICO_OPERADOR,-- N  nchar(14)
    AGENTE_PROMOTOR,             -- Y  nvarchar(255)
    CNPJ_AGENTE_PROMOTOR,        -- Y  nchar(14)
    CO_PROGRAMA,                 -- Y  int  — ver tabela "Programa" no rodapé (0-3)
    ULTIMO_DESEMBOLSO,           -- Y  bit
    FUNCIONALIDADE,              -- N  bit           — Sim/Não/NULL (não respondido)
    CONCLUIDO,                   -- N  bit
    DT_ENGENHARIA,                -- Y  date
    CO_SITUACAO_OBRA,             -- N  int  — ver tabela "TipoSituacaoObra" (0-1)
    DT_SOCIO_AMBIENTAL,           -- N  date
    PERCENTUAL_OBRA,              -- Y  decimal(18,4)
    CO_TIPO_DESEMBOLSO,           -- Y  int  — ver tabela "TipoDesembolso" (0-1)
    RETORNO_PARCIAL,              -- N  bit
    PLACA_LOCAL,                  -- N  bit
    LICENSA_INSTALACAO,           -- N  bit
    LICENSA_OPERACAO,             -- N  bit
    CND_VALIDO,                   -- N  bit
    CRP_VALIDO,                   -- N  bit
    SOLICITADO_VI,                -- Y  decimal(18,2)
    GLOSSADO_VI,                  -- Y  decimal(18,2)
    ACEITO_VI,                    -- Y  decimal(18,2)
    PARTICIPACAO_FGTS,            -- Y  decimal(18,2)
    CONTRAPARTIDA,                -- Y  decimal(18,2)
    VALOR_EMPRESTIMO,             -- Y  decimal(18,2)
    DESEMBOLSADO,                 -- Y  decimal(18,2)
    SALDO_DESEMBOLSAR,            -- Y  decimal(18,2)
    EXCEPCIONALIZADO,             -- N  bit
    CONTRAPARTIDA_ATUAL,          -- Y  decimal(18,2)
    INTEGRALIZADO,                -- Y  decimal(18,2)
    SALDO_INTEGRALIZAR,           -- Y  decimal(18,2)
    CONTRAPARTIDA_ALTERADA,       -- N  bit
    -- AMORTIZACAO removida (coluna não existe mais)
    SANEPAR,                      -- N  bit
    MENSAGEM                      -- N  nvarchar(3000) — "OBS AF"
    -- MOTIVO_REJEICAO fica de fora aqui: só é preenchido quando rejeitar (ver Rejeitar no service)
) VALUES (
    N'c123456', N'1001', N'c200001', GETDATE(), 1,
    N'0700001', N'01', 1, N'Caixa Econômica Federal', N'00360305000104',
    N'Prefeitura Municipal Exemplo', N'11111111000101', NULL, NULL,
    N'Agente Promotor Exemplo', N'22222222000101',
    0,                             -- CO_PROGRAMA: 0=Pro_Transporte
    0, NULL, NULL, GETDATE(),
    0,                             -- CO_SITUACAO_OBRA: 0=NORMAL
    NULL, 0.0000,
    0,                             -- CO_TIPO_DESEMBOLSO: 0=NORMAL
    NULL, NULL, NULL, NULL, NULL, NULL,
    50000.00, 0.00, 50000.00, 200000.00,
    10000.00, 210000.00, 0.00, 200000.00,
    NULL, 200000.00, 0.00, 200000.00,
    NULL, NULL, NULL
);

SET @coDesembolso = SCOPE_IDENTITY();
SELECT @coDesembolso AS CoDesembolsoGerado;

-- ────────────────────────────────────────────────────────────────────────
-- 2) CAD_TB002_CONTROLE_DESEMBOLSO — obrigatória pra o desembolso aparecer
-- ────────────────────────────────────────────────────────────────────────
DECLARE @coControleDesembolso INT;

INSERT INTO [CAD_TB002_CONTROLE_DESEMBOLSO] (
    CO_DESEMBOLSO,               -- Y  int — o @coDesembolso gerado acima
    RESPONSAVEL_ANALISE,         -- N  nvarchar(7) — matrícula do analista vinculado (ou NULL)
    RESPONSAVEL_BAIXA,           -- N  nvarchar(7) — matrícula de quem foi designado a baixar (setado na aprovação)
    RESPONSAVEL_DESEMBOLSO,      -- N  nvarchar(7) — matrícula de quem confirmou a baixa (setado só quando finaliza)
    GESTOR,                      -- N  nvarchar(7)
    DT_PRAZO,                    -- Y  date
    CO_STATUS_DESEMBOLSO,        -- Y  int — ver tabela "TipoStatusDesembolso" (1-5)
    DT_CONCLUSAO                 -- N  date — preenchido quando aprova/rejeita
) VALUES (
    @coDesembolso, NULL, NULL, NULL, NULL,
    DATEADD(DAY, 2, CAST(GETDATE() AS date)),
    1,                            -- CO_STATUS_DESEMBOLSO: 1=PENDENTE
    NULL
);

SET @coControleDesembolso = SCOPE_IDENTITY();
SELECT @coControleDesembolso AS CoControleDesembolsoGerado;

-- ────────────────────────────────────────────────────────────────────────
-- 3) CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO — checklist (opcional, mas
-- recomendado). Copia todo item ativo do catálogo pro novo desembolso,
-- todos começando como "a analisar".
-- ────────────────────────────────────────────────────────────────────────
INSERT INTO [CAD_TB004_VALIDACAO_CONTROLE_DESEMBOLSO] (CO_VALIDACAO, CO_CONTROLE_DESEMBOLSO, DE_VALIDACAO, SITUACAO)
SELECT CO_VALIDACAO, @coControleDesembolso, DE_VALIDACAO, 0   -- SITUACAO: 0=ANALISAR
FROM [CAD_TB003_VALIDACAO]
WHERE DESATIVADO = 0;

-- ────────────────────────────────────────────────────────────────────────
-- 4) CAD_TB005_MENSAGEM — comentário/justificativa (totalmente opcional)
-- ────────────────────────────────────────────────────────────────────────
-- INSERT INTO [CAD_TB005_MENSAGEM] (
--     CO_VALIDACAO,             -- Y  int — de qual item do checklist (ou de "Pedido Negado" se for rejeição)
--     CO_CONTROLE_DESEMBOLSO,   -- Y  int
--     DE_MENSAGEM,              -- N  nvarchar(3000)
--     CO_TIPO_MENSAGEM,         -- Y  int — ver tabela "TipoMensagem" (0-4)
--     CO_USUARIO,               -- N  nvarchar(7)
--     DE_USUARIO,               -- N  nvarchar(255)
--     UNIDADE_USUARIO,          -- Y  int — 7175 = CEFGA; qualquer outro valor = GIGOV
--     DT_CRIACAO,               -- Y  datetime
--     ATIVO                     -- Y  bit — default 1 (soft-delete usa 0)
-- ) VALUES (
--     1, @coControleDesembolso, N'Comentário de teste', 1,
--     N'c123456', N'Fulano de Tal', 7175, GETDATE(), 1
-- );

GO

-- ============================================================================
-- TABELA DE CÓDIGOS (enums) — todos os int abaixo são o valor cru salvo na
-- coluna, na ordem/numeração ATUAL do código (já alinhada com a Branch).
-- ============================================================================

-- TipoStatusDesembolso (CAD_TB002.CO_STATUS_DESEMBOLSO)
--   1 = PENDENTE     (aguardando 1ª análise / macro ainda não rodou)
--   2 = ANALISAR      (macro rodou, sobrou pendência manual)
--   3 = DESEMBOLSAR   (aprovado, aguardando baixa da DRP)
--   4 = NEGAR         (rejeitado)
--   5 = FINALIZAR     (baixado / concluído)

-- TipoSituacaoValidacao (CAD_TB004.SITUACAO)
--   0 = ANALISAR
--   1 = APROVADO
--   2 = NEGADO

-- TipoMensagem (CAD_TB005.CO_TIPO_MENSAGEM)
--   0 = JUSTIFICATIVA
--   1 = INFORMATIVO
--   2 = PARECER
--   3 = OBSERVACAO
--   4 = REJEICAO

-- TipoDesembolso (CAD_TB001.CO_TIPO_DESEMBOLSO)
--   0 = NORMAL
--   1 = ADIANTAMENTO

-- TipoSituacaoObra (CAD_TB001.CO_SITUACAO_OBRA, aceita NULL)
--   0 = NORMAL
--   1 = ATRASADO

-- Programa (CAD_TB001.CO_PROGRAMA)
--   0 = Pro_Transporte
--   1 = Pro_Moradia
--   2 = Saneamento
--   3 = Saude
