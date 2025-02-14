-- Desabilitar verificação de chaves estrangeiras
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT ALL"

-- Limpar todas as tabelas
EXEC sp_MSforeachtable "DELETE FROM ?"

-- Resetar identity de todas as tabelas
EXEC sp_MSforeachtable "DBCC CHECKIDENT ('?', RESEED, 0)"

-- Habilitar verificação de chaves estrangeiras novamente
EXEC sp_MSforeachtable "ALTER TABLE ? CHECK CONSTRAINT ALL"
