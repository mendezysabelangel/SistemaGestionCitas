USE [SistemaGestionCitasDB];
GO

IF DB_NAME() <> N'SistemaGestionCitasDB'
    THROW 50001, N'La bitácora debe instalarse en SistemaGestionCitasDB.', 1;

IF OBJECT_ID(N'dbo.Citas', N'U') IS NULL
    THROW 50002, N'Primero ejecuta 002_CreateTables.sql para crear la tabla Citas.', 1;

IF OBJECT_ID(N'dbo.BitacoraCitas', N'U') IS NULL
BEGIN
    -- Se guardan los IDs y el nombre de usuario como datos históricos del evento.
    CREATE TABLE dbo.BitacoraCitas (
        IdEvento BIGINT IDENTITY(1,1) PRIMARY KEY,
        FechaEvento DATETIME2(0) NOT NULL CONSTRAINT DF_BitacoraCitas_FechaEvento DEFAULT SYSDATETIME(),
        IdCita INT NOT NULL,
        IdUsuario INT NOT NULL,
        NombreUsuario NVARCHAR(50) NOT NULL,
        Accion NVARCHAR(30) NOT NULL,
        EstadoAnterior NVARCHAR(100) NULL,
        EstadoNuevo NVARCHAR(100) NOT NULL
    );
END;
GO
