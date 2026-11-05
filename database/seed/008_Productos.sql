/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 008_Productos.sql
   DESCRIPCIÓN: Productos de prueba (solo si la tabla está vacía)
   ========================================================= */

USE SistemaGestionCitasDB;
GO

IF NOT EXISTS (SELECT 1 FROM Productos)
BEGIN
    INSERT INTO Productos (Codigo, Nombre, Precio, Stock, IdCategoria, Activo)
    SELECT V.Codigo, V.Nombre, V.Precio, V.Stock, C.IdCategoria, V.Activo
    FROM (VALUES
        ('CAB-001', 'Shampoo reparador 500 ml',        650.00,  25, 'Cuidado del cabello', 1),
        ('CAB-002', 'Acondicionador hidratante 500 ml', 700.00,  18, 'Cuidado del cabello', 1),
        ('CAB-003', 'Mascarilla capilar de keratina',   950.00,   4, 'Cuidado del cabello', 1),
        ('CAB-004', 'Aceite de argán 100 ml',          1200.00,   0, 'Cuidado del cabello', 1),
        ('PIE-001', 'Crema facial hidratante 50 g',     890.00,  30, 'Cuidado de la piel',  1),
        ('PIE-002', 'Protector solar FPS 50',           780.00,  12, 'Cuidado de la piel',  1),
        ('PIE-003', 'Limpiador facial en gel',          540.00,   5, 'Cuidado de la piel',  1),
        ('PIE-004', 'Mascarilla de arcilla',            450.00,   0, 'Cuidado de la piel',  0),
        ('UNA-001', 'Esmalte semipermanente rojo',      350.00,  40, 'Uñas',                1),
        ('UNA-002', 'Removedor de esmalte 250 ml',      180.00,  22, 'Uñas',                1),
        ('ACC-001', 'Cepillo de cabello profesional',   420.00,  15, 'Accesorios',          1),
        ('OTR-001', 'Tarjeta de regalo',               1000.00, 100, 'Otros',               1)
    ) AS V(Codigo, Nombre, Precio, Stock, Categoria, Activo)
    INNER JOIN CategoriasProducto C ON C.Nombre = V.Categoria;
END;
GO

SELECT P.IdProducto, P.Codigo, P.Nombre, P.Precio, P.Stock, C.Nombre AS Categoria, P.Activo
FROM Productos P
INNER JOIN CategoriasProducto C ON P.IdCategoria = C.IdCategoria
ORDER BY P.IdProducto;
GO