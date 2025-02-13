IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash)
    VALUES ('admin', '$2a$11$k7R1o4YR0hkqpMx9qgkJx.J7u/9WNJ.Eb5jxlOjSxM9.ekF0QhNr2') -- senha: admin123
END
