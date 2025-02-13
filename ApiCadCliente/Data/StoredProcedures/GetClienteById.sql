CREATE PROCEDURE [dbo].[GetClienteById]
    @Id int
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT c.Id, c.Nome, c.Email, c.Logotipo,
           l.Id as LogradouroId, l.Endereco
    FROM Clientes c
    LEFT JOIN Logradouros l ON c.Id = l.ClienteId
    WHERE c.Id = @Id;
END
