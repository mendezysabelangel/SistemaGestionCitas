/* =========================================================
   PROYECTO: Sistema de Gestión de Citas
   SCRIPT: 005_EstadosCita.sql
   DESCRIPCIÓN: Inserción del estado de las citas en el sistema
   ========================================================= */

USE SistemaGestionCitasDB;
GO

IF NOT EXISTS ( SELECT 1 FROM EstadosCita WHERE Nombre = 'Pendiente')
BEGIN
	INSERT INTO EstadosCita (Nombre, Descripcion)
	VALUES (
        'Pendiente',
        'La cita ha sido registrada y está pendiente de confirmación'
    );
END;

IF NOT EXISTS ( SELECT 1 FROM EstadosCita WHERE Nombre = 'Confirmada')
BEGIN
	INSERT INTO EstadosCita (Nombre, Descripcion)
	VALUES (
        'Confirmada',
        'La cita ha sido confirmada y está programada para ser atendida'
    );
END;

IF NOT EXISTS ( SELECT 1 FROM EstadosCita WHERE Nombre = 'Completada')
BEGIN
	INSERT INTO EstadosCita (Nombre, Descripcion)
	VALUES (
        'Completada',
        'La cita fue atendida y finalizada correctamente'
    );
END;

IF NOT EXISTS ( SELECT 1 FROM EstadosCita WHERE Nombre = 'Cancelada')
BEGIN
	INSERT INTO EstadosCita (Nombre, Descripcion)
	VALUES (
        'Cancelada',
        'La cita fue cancelada y no será atendida'
    );
END;

IF NOT EXISTS ( SELECT 1 FROM EstadosCita WHERE Nombre = 'No asistió')
BEGIN
	INSERT INTO EstadosCita (Nombre, Descripcion)
	VALUES (
        'No asistió',
        'El cliente no se presentó a la cita programada'
    );
END;