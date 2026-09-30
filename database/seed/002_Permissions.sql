/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 002_Permissions.sql
   DESCRIPCIÓN: Inserción de permisos iniciales del sistema
   ========================================================= */

USE SistemaGestionCitasDB;
GO

INSERT INTO Permisos (Nombre, Descripcion)
VALUES
('Agregar', 'Permite agregar nuevos registros'),
('Modificar', 'Permite modificar registros existentes'),
('Eliminar', 'Permite eliminar registros'),
('Consultar', 'Permite consultar información'),
('CrearUsuarios', 'Permite crear nuevos usuarios'),
('CrearRoles', 'Permite crear nuevos roles');
GO