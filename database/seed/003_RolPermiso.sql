/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 003_RolPermiso.sql
   DESCRIPCIÓN: Asignación de permisos a los roles
   ========================================================= */

USE SistemaGestionCitasDB;
GO

-- Administrador: todos los permisos
INSERT INTO RolPermiso (IdRol, IdPermiso)
SELECT R.IdRol, P.IdPermiso
FROM Roles R
CROSS JOIN Permisos P
WHERE R.Nombre = 'Administrador';


-- Supervisor: consultar y modificar
INSERT INTO RolPermiso (IdRol, IdPermiso)
SELECT R.IdRol, P.IdPermiso
FROM Roles R
CROSS JOIN Permisos P
WHERE R.Nombre = 'Supervisor'
AND P.Nombre IN ('Consultar', 'Modificar');


-- Ejecutor: consultar y agregar
INSERT INTO RolPermiso (IdRol, IdPermiso)
SELECT R.IdRol, P.IdPermiso
FROM Roles R
CROSS JOIN Permisos P
WHERE R.Nombre = 'Ejecutor'
AND P.Nombre IN ('Consultar', 'Agregar');
GO

SELECT
    R.Nombre AS Rol,
    P.Nombre AS Permiso
FROM RolPermiso RP
INNER JOIN Roles R
    ON RP.IdRol = R.IdRol
INNER JOIN Permisos P
    ON RP.IdPermiso = P.IdPermiso
ORDER BY R.Nombre, P.Nombre;