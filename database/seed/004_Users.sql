/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 004_Users.sql
   DESCRIPCIÓN: Inserción de usuarios iniciales del sistema
   ========================================================= */

USE SistemaGestionCitasDB;
GO

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE NombreUsuario = 'admin')
BEGIN
    INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, IdRol)
    SELECT
        'admin',
        '100000.OMSmNC9EtSMCp4Y5AFaiHg==.UfngOthZVPit7GG78jjylQx4/lWJYu+l557u2nm7Bzw=',
        'Administrador del Sistema',
        IdRol
    FROM Roles
    WHERE Nombre = 'Administrador';
END;


IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE NombreUsuario = 'supervisor')
BEGIN
    INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, IdRol)
    SELECT
        'supervisor',
        '100000./Gx+AaKjnvwKZesuiE1Ppw==.LR55+gJVnHSZOon+IymxjwwYTMU3Yb+vGrzl2yPxfxE=',
        'Supervisor del Sistema',
        IdRol
    FROM Roles
    WHERE Nombre = 'Supervisor';
END;


IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE NombreUsuario = 'ejecutor')
BEGIN
    INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, IdRol)
    SELECT
        'ejecutor',
        '100000.lW1oYh5xON2gB2qREjjKRg==.kciRv6fTFJtwIhX9Lv6u3V7zJRsuDgALOI8uDqHMgoQ=',
        'Ejecutor del Sistema',
        IdRol
    FROM Roles
    WHERE Nombre = 'Ejecutor';
END;