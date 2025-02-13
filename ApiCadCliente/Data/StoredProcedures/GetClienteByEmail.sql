CREATE PROCEDURE [dbo].[GetClienteByEmail]
    @Email nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT c.Id, c.Nome, c.Email, c.Logotipo,
           l.Id as LogradouroId, l.Endereco
    FROM Clientes c
    LEFT JOIN Logradouros l ON c.Id = l.ClienteId
    WHERE c.Email = @Email;
END
