/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 007_CategoriasProducto.sql
   DESCRIPCIÓN: Categorías iniciales de productos
   ========================================================= */

USE SistemaGestionCitasDB;
GO

INSERT INTO CategoriasProducto (Nombre)
SELECT V.Nombre
FROM (VALUES
    ('Cuidado del cabello'),
    ('Cuidado de la piel'),
    ('Uñas'),
    ('Accesorios'),
    ('Otros')
) AS V(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM CategoriasProducto C WHERE C.Nombre = V.Nombre);
GO

SELECT IdCategoria, Nombre, Activo FROM CategoriasProducto;
GO