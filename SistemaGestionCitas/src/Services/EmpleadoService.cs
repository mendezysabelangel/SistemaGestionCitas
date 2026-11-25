using System.Net.Mail;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services;

public class EmpleadoService
{
    private readonly EmpleadoRepository repositorio = new();

    public List<Empleado> Buscar(string? texto, bool? activo)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Consultar))
            throw new UnauthorizedAccessException("Permisos insuficientes para consultar empleados.");

        return repositorio.Buscar(string.IsNullOrWhiteSpace(texto) ? null : texto.Trim(), activo);
    }

    public ResultadoOperacion Registrar(Empleado empleado)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Agregar))
            return Error("Permisos insuficientes para registrar empleados.");

        string? error = Validar(empleado);
        if (error != null) return Error(error);

        repositorio.Insertar(empleado);
        return new ResultadoOperacion { Exitoso = true, Mensaje = "Empleado registrado correctamente." };
    }

    public ResultadoOperacion Modificar(Empleado empleado)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Modificar))
            return Error("Permisos insuficientes para modificar empleados.");

        if (empleado.IdEmpleado <= 0)
            return Error("Seleccione un empleado para modificar.");

        string? error = Validar(empleado);
        if (error != null) return Error(error);

        if (!repositorio.Actualizar(empleado))
            return Error("El empleado ya no existe en la base de datos.");

        return new ResultadoOperacion { Exitoso = true, Mensaje = "Empleado modificado correctamente." };
    }

    public ResultadoOperacion Eliminar(int idEmpleado)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Eliminar))
            return Error("Permisos insuficientes para eliminar empleados.");

        try
        {
            if (!repositorio.Eliminar(idEmpleado))
                return Error("El empleado ya no existe en la base de datos.");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return Error("El empleado tiene citas asociadas y no se puede eliminar. Puede desactivarlo desde Editar.");
        }

        return new ResultadoOperacion { Exitoso = true, Mensaje = "Empleado eliminado correctamente." };
    }

    private static string? Validar(Empleado empleado)
    {
        empleado.Nombres = empleado.Nombres.Trim();
        empleado.Apellidos = empleado.Apellidos.Trim();
        empleado.Telefono = empleado.Telefono.Trim();
        empleado.Correo = string.IsNullOrWhiteSpace(empleado.Correo) ? null : empleado.Correo.Trim();

        if (empleado.Nombres.Length == 0 || empleado.Nombres.Length > Empleado.MaxNombres)
            return $"Los nombres son obligatorios y admiten hasta {Empleado.MaxNombres} caracteres.";

        if (empleado.Apellidos.Length == 0 || empleado.Apellidos.Length > Empleado.MaxApellidos)
            return $"Los apellidos son obligatorios y admiten hasta {Empleado.MaxApellidos} caracteres.";

        if (empleado.Telefono.Length == 0 || empleado.Telefono.Length > Empleado.MaxTelefono)
            return $"El teléfono es obligatorio y admite hasta {Empleado.MaxTelefono} caracteres.";

        if (empleado.Correo is { } correo && (correo.Length > Empleado.MaxCorreo
            || !MailAddress.TryCreate(correo, out MailAddress? direccion) || direccion.Address != correo))
            return $"Ingrese un correo electrónico válido de hasta {Empleado.MaxCorreo} caracteres.";

        return null;
    }

    private static ResultadoOperacion Error(string mensaje) => new() { Mensaje = mensaje };
}
