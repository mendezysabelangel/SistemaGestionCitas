/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 002_CreateTables.sql
   DESCRIPCIÓN: Creación de las tablas del sistema
   ========================================================= */

USE SistemaGestionCitasDB;
GO

CREATE TABLE Roles (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL UNIQUE,
    Descripcion VARCHAR(200) NULL,
    Activo BIT NOT NULL DEFAULT 1 
    );

CREATE TABLE Permisos (
    IdPermiso INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL UNIQUE,
    Descripcion VARCHAR(200) NULL,
    Activo BIT NOT NULL DEFAULT 1
    );

CREATE TABLE RolPermiso (
    IdRolPermiso INT IDENTITY(1,1) PRIMARY KEY,
    IdRol INT NOT NULL,
    IdPermiso INT NOT NULL,
    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol),
    FOREIGN KEY (IdPermiso) REFERENCES Permisos(IdPermiso),
    UNIQUE (IdRol, IdPermiso)
    );

CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    NombreCompleto VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) NULL,
    IdRol INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    IntentosFallidos INT NOT NULL DEFAULT 0,
    BloqueadoHasta DATETIME2 NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
    );

CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombres VARCHAR(80) NOT NULL,
    Apellidos VARCHAR(80) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Correo VARCHAR(100) NULL,
    Documento VARCHAR(30) NULL,
    FechaNacimiento DATE NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );

CREATE TABLE Empleados (
    IdEmpleado INT IDENTITY(1,1) PRIMARY KEY,
    Nombres VARCHAR(80) NOT NULL,
    Apellidos VARCHAR(80) NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Correo VARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME()
    );

CREATE TABLE Servicios (
    IdServicio INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(250) NULL,
    DuracionMinutos INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CHECK (DuracionMinutos > 0),
    CHECK (Precio >= 0)
    );

CREATE TABLE EstadosCita (
    IdEstadoCita INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Descripcion VARCHAR(150) NULL,
    Activo BIT NOT NULL DEFAULT 1
    );

CREATE TABLE Citas (
    IdCita INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    IdEmpleado INT NOT NULL,
    IdServicio INT NOT NULL,
    IdEstadoCita INT NOT NULL,
    Fecha DATE NOT NULL,
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
    Observaciones VARCHAR(500) NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    IdUsuarioCreacion INT NOT NULL,

    FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente),
    FOREIGN KEY (IdEmpleado) REFERENCES Empleados(IdEmpleado),
    FOREIGN KEY (IdServicio) REFERENCES Servicios(IdServicio),
    FOREIGN KEY (IdEstadoCita) REFERENCES EstadosCita(IdEstadoCita),
    FOREIGN KEY (IdUsuarioCreacion) REFERENCES Usuarios(IdUsuario),

    CHECK (HoraFin > HoraInicio)
    );

GO