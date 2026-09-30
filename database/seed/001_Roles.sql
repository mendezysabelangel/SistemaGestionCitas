/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 001_Roles.sql
   DESCRIPCIÓN: Inserción de roles iniciales del sistema
   ========================================================= */

USE SistemaGestionCitasDB;
GO

INSERT INTO Roles (Nombre, Descripcion)
VALUES
('Administrador', 'Acceso total al sistema'),
('Supervisor', 'Puede consultar y modificar registros'),
('Ejecutor', 'Puede consultar y agregar registros');

GO