
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Documento = '00100000001')
BEGIN
    INSERT INTO Clientes (Nombres, Apellidos, Telefono, Correo, Documento, FechaNacimiento)
    VALUES (
    'Juan Pérez', 
    '8095551234', 
    'juan.prueba@email.com', 
    '00100000001', 
    '1998-05-15'
    );
END;

