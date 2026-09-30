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
        'HASH_PENDIENTE',
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
        'HASH_PENDIENTE',
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
        'HASH_PENDIENTE',
        'Ejecutor del Sistema',
        IdRol
    FROM Roles
    WHERE Nombre = 'Ejecutor';
END;