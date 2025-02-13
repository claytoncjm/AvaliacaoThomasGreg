CREATE PROCEDURE [dbo].[GetAllClientes]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT c.Id, c.Nome, c.Email, c.Logotipo,
           l.Id as LogradouroId, l.Endereco
    FROM Clientes c
    LEFT JOIN Logradouros l ON c.Id = l.ClienteId;
END
