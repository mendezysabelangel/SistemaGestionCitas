/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 004_CreateProductos.sql
   DESCRIPCIÓN: Tablas del mantenimiento de productos
   (se puede ejecutar varias veces sin borrar datos)
   ========================================================= */

USE SistemaGestionCitasDB;
GO

IF OBJECT_ID(N'dbo.CategoriasProducto', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CategoriasProducto (
        IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
        Nombre VARCHAR(80) NOT NULL UNIQUE,
        Activo BIT NOT NULL DEFAULT 1
    );
END;
GO

IF OBJECT_ID(N'dbo.Productos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos (
        IdProducto INT IDENTITY(1,1) PRIMARY KEY,
        Codigo VARCHAR(30) NOT NULL UNIQUE,
        Nombre VARCHAR(100) NOT NULL,
        Precio DECIMAL(10,2) NOT NULL,
        Stock INT NOT NULL DEFAULT 0,
        IdCategoria INT NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

        FOREIGN KEY (IdCategoria) REFERENCES dbo.CategoriasProducto(IdCategoria),
        CHECK (Precio >= 0),
        CHECK (Stock >= 0)
    );
END;
GO