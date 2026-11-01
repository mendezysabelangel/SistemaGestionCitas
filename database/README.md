# Base de datos - Sistema de Gestión de Citas

Esta carpeta contiene los archivos necesarios para crear y configurar la base de datos del proyecto.

## Estructura

### scripts
Contiene los scripts SQL utilizados para crear la estructura de la base de datos, incluyendo tablas, relaciones, restricciones, vistas y procedimientos almacenados.

### seed
Contiene los datos iniciales y datos de prueba necesarios para ejecutar el sistema.

## Motor de base de datos

SQL Server

## Dirección de clientes

Si la base de datos ya existe, ejecutar `scripts/003_AddDireccionClientes.sql`
en SQL Server antes de abrir la versión actualizada de la aplicación. El script
puede ejecutarse nuevamente y conserva los clientes existentes.

Para una instalación nueva, `scripts/002_CreateTables.sql` ya incluye la columna
`Direccion` de tipo `NVARCHAR(250)`.

La dirección es obligatoria al registrar o modificar un cliente, con un máximo
de 250 caracteres. Los clientes anteriores sin dirección pueden consultarse;
al modificarlos se debe completar ese dato. También se muestra en el listado,
el reporte y sus exportaciones.
