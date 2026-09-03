-- ============================================================================
-- Ajustes de schema pro pacote de 16 mudanças pedidas nesta sessão.
-- Rode conectado no database CadDesembolsoDev (ou o banco real equivalente).
-- Todas as colunas novas são NULLABLE (exceto onde indicado) — não quebra
-- linhas já existentes.
-- ============================================================================

USE [CadDesembolsoDev];
GO

-- Item 2: check "Recorrente" na exclusividade (primeiro/último/adiantamento/recorrente)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'RECORRENTE')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [RECORRENTE] BIT NOT NULL CONSTRAINT DF_CAD_TB001_RECORRENTE DEFAULT (0);
GO

-- Item 3: Pró-Transporte — "Tem carroceria" / "Veículo possui adesivos"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'TEM_CARROCERIA')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [TEM_CARROCERIA] BIT NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'VEICULO_POSSUI_ADESIVOS')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [VEICULO_POSSUI_ADESIVOS] BIT NULL;
GO

-- Item 5: CRP com opção NSA (CRP_VALIDO fica NULL quando CRP_NSA = 1)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'CRP_NSA')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [CRP_NSA] BIT NOT NULL CONSTRAINT DF_CAD_TB001_CRP_NSA DEFAULT (0);
GO

-- Item 6: Data de início da obra
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'DATA_INICIO_OBRA')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [DATA_INICIO_OBRA] DATE NULL;
GO

-- Item 16 (segundo "15" da lista): 1º desembolso — destinação da coleta de resíduos sólidos
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB001_DESEMBOLSO') AND name = 'DESTINACAO_COLETA_RESIDUOS_SOLIDOS')
    ALTER TABLE [CAD_TB001_DESEMBOLSO] ADD [DESTINACAO_COLETA_RESIDUOS_SOLIDOS] BIT NULL;
GO

-- Item 12: motivo do cancelamento (status novo CO_STATUS_DESEMBOLSO = 6 = CANCELAR,
-- não precisa de alteração de schema pro status em si, só a coluna de motivo)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CAD_TB002_CONTROLE_DESEMBOLSO') AND name = 'MOTIVO_CANCELAMENTO')
    ALTER TABLE [CAD_TB002_CONTROLE_DESEMBOLSO] ADD [MOTIVO_CANCELAMENTO] NVARCHAR(3000) NULL;
GO

-- Item 4: inserir "Licença de Operação" e "Licença de Instalação" no catálogo de
-- validações (CAD_TB003_VALIDACAO), só se ainda não existirem — esses dois itens
-- já estão previstos no seed original (seed-catalogo-validacoes.sql, IDs 1 e 2);
-- este bloco é uma rede de segurança caso o catálogo do seu banco não os tenha.
IF NOT EXISTS (SELECT 1 FROM [CAD_TB003_VALIDACAO] WHERE DE_VALIDACAO = N'Licença de operação')
    INSERT INTO [CAD_TB003_VALIDACAO] (DE_VALIDACAO, DT_CRIACAO, DESATIVADO) VALUES (N'Licença de operação', GETDATE(), 0);
GO
IF NOT EXISTS (SELECT 1 FROM [CAD_TB003_VALIDACAO] WHERE DE_VALIDACAO = N'Licença de instalação')
    INSERT INTO [CAD_TB003_VALIDACAO] (DE_VALIDACAO, DT_CRIACAO, DESATIVADO) VALUES (N'Licença de instalação', GETDATE(), 0);
GO

SELECT * FROM [CAD_TB003_VALIDACAO] ORDER BY CO_VALIDACAO;
