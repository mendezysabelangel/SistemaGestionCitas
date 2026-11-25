/* =========================================================
   Citorna - Personal y catálogo de barbería
   Ejecutar después de 002_CreateTables.sql.
   Cada empleado se identifica por su teléfono y cada servicio
   por su nombre. Solo se agregan los registros faltantes.
   ========================================================= */

USE [SistemaGestionCitasDB];
GO

IF DB_NAME() <> N'SistemaGestionCitasDB'
    THROW 50001, N'Selecciona la base SistemaGestionCitasDB antes de ejecutar este script.', 1;

IF OBJECT_ID(N'dbo.Empleados', N'U') IS NULL OR OBJECT_ID(N'dbo.Servicios', N'U') IS NULL
    THROW 50002, N'Primero ejecuta 002_CreateTables.sql para disponer de Empleados y Servicios.', 1;

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @EmpleadosInsertados int, @ServiciosInsertados int;

DECLARE @Empleados TABLE (
    Nombres nvarchar(80), Apellidos nvarchar(80), Telefono varchar(20) PRIMARY KEY
);
INSERT INTO @Empleados (Nombres, Apellidos, Telefono)
VALUES
    (N'Lucía',  N'Rivas',  '809-555-0131'),
    (N'Mateo',  N'Santos', '809-555-0132'),
    (N'Elena',  N'Peña',   '809-555-0133'),
    (N'Daniel', N'Vargas', '809-555-0134');

DECLARE @Servicios TABLE (
    Nombre nvarchar(100) PRIMARY KEY, Descripcion nvarchar(250),
    DuracionMinutos int, Precio decimal(10, 2)
);
INSERT INTO @Servicios (Nombre, Descripcion, DuracionMinutos, Precio)
VALUES
    (N'Arreglo de barba', N'Perfilado y recorte de barba.',                         15,  350.00),
    (N'Corte clásico',   N'Corte con tijera o máquina y peinado.',                 30,  650.00),
    (N'Afeitado clásico',N'Afeitado con navaja, toalla caliente y bálsamo.',        25,  500.00),
    (N'Corte y barba',   N'Corte de cabello, recorte y perfilado de barba.',       50, 1000.00),
    (N'Corte degradado', N'Degradado con máquina y acabado de contornos.',         40,  800.00),
    (N'Corte infantil',  N'Corte de cabello para niños con tijera o máquina.',     25,  500.00);

BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO dbo.Empleados (Nombres, Apellidos, Telefono, Activo)
    SELECT Datos.Nombres, Datos.Apellidos, Datos.Telefono, 1
    FROM @Empleados AS Datos
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Empleados AS Existente WITH (UPDLOCK, HOLDLOCK)
        WHERE Existente.Telefono = Datos.Telefono
    );

    SET @EmpleadosInsertados = @@ROWCOUNT;

    INSERT INTO dbo.Servicios (Nombre, Descripcion, DuracionMinutos, Precio, Activo)
    SELECT Datos.Nombre, Datos.Descripcion, Datos.DuracionMinutos, Datos.Precio, 1
    FROM @Servicios AS Datos
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Servicios AS Existente WITH (UPDLOCK, HOLDLOCK)
        WHERE Existente.Nombre = Datos.Nombre
    );

    SET @ServiciosInsertados = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT @EmpleadosInsertados AS EmpleadosInsertados, @ServiciosInsertados AS ServiciosInsertados;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
