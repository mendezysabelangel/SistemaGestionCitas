USE SistemaGestionCitasDB;
GO

IF NOT EXISTS (SELECT 1 FROM Clientes)
BEGIN
    INSERT INTO Clientes (Nombres, Apellidos, Telefono, Correo, Documento, FechaNacimiento, Activo, FechaRegistro)
    VALUES
    ('Juan Carlos', 'Pérez Martínez',   '809-555-0101', 'juan.perez@example.com',   '001-0000001-1', '1990-03-15', 1, DATEADD(DAY, -180, SYSDATETIME())),
    ('María',       'Rodríguez Santos', '829-555-0102', 'maria.rodriguez@example.com','001-0000002-2', '1985-07-22', 1, DATEADD(DAY, -150, SYSDATETIME())),
    ('Pedro',       'Gómez Almonte',    '849-555-0103', NULL,                        '001-0000003-3', '1978-11-05', 1, DATEADD(DAY, -120, SYSDATETIME())),
    ('Ana María',   'Santana Reyes',    '809-555-0104', 'ana.santana@example.com',   NULL,            '1995-01-30', 1, DATEADD(DAY, -100, SYSDATETIME())),
    ('Luis',        'Fernández Cruz',   '829-555-0105', 'luis.fernandez@example.com','001-0000005-5', NULL,         0, DATEADD(DAY,  -90, SYSDATETIME())),
    ('Carmen',      'Díaz Peña',        '809-555-0106', 'carmen.diaz@example.com',   '001-0000006-6', '1982-09-12', 1, DATEADD(DAY,  -75, SYSDATETIME())),
    ('Rafael',      'Méndez Ortiz',     '849-555-0107', NULL,                        NULL,            '1999-04-18', 1, DATEADD(DAY,  -60, SYSDATETIME())),
    ('Marina',      'Castillo Núñez',   '809-555-0108', 'marina.castillo@example.com','001-0000008-8','1988-12-01', 0, DATEADD(DAY,  -45, SYSDATETIME())),
    ('José Miguel', 'Ramírez Polanco',  '829-555-0109', 'jose.ramirez@example.com',  '001-0000009-9', '1975-06-27', 1, DATEADD(DAY,  -30, SYSDATETIME())),
    ('Sofía',       'Vargas Medina',    '809-555-0110', 'sofia.vargas@example.com',  '001-0000010-0', '2001-02-14', 1, DATEADD(DAY,  -15, SYSDATETIME())),
    ('Marcos',      'Herrera Báez',     '849-555-0111', 'marcos.herrera@example.com','001-0000011-1', '1992-08-09', 1, DATEADD(DAY,   -7, SYSDATETIME())),
    ('Laura',       'Jiménez Mora',     '809-555-0112', NULL,                        '001-0000012-2', '1997-10-23', 1, DATEADD(DAY,   -2, SYSDATETIME()));
END;
GO

SELECT IdCliente, Nombres, Apellidos, Telefono, Activo, FechaRegistro FROM Clientes;
GO