/* Agrega Dirección a una base de datos existente sin borrar clientes.
   Se permite NULL para los registros anteriores que aún no tienen dirección.
   La aplicación exige completar la dirección al registrar o modificar. */

USE SistemaGestionCitasDB;
GO

IF COL_LENGTH(N'dbo.Clientes', N'Direccion') IS NULL
BEGIN
    ALTER TABLE dbo.Clientes ADD Direccion NVARCHAR(250) NULL;
END;
GO
