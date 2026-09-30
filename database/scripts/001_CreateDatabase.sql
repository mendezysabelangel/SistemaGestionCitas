/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 001_CreateDatabase.sql
   DESCRIPCIÓN: Creación inicial de la base de datos
   ========================================================= */

IF DB_ID('SistemaGestionCitasDB') IS NULL
BEGIN
    CREATE DATABASE SistemaGestionCitasDB;
END;
GO

USE SistemaGestionCitasDB;
GO