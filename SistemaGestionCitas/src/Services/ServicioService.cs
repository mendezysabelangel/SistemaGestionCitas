using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services;

public class ServicioService
{
    private readonly ServicioRepository repositorio = new();

    public List<Servicio> Buscar(string? texto, bool? activo)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Consultar))
            throw new UnauthorizedAccessException("Permisos insuficientes para consultar servicios.");

        return repositorio.Buscar(string.IsNullOrWhiteSpace(texto) ? null : texto.Trim(), activo);
    }

    public ResultadoOperacion Registrar(Servicio servicio)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Agregar))
            return Error("Permisos insuficientes para registrar servicios.");

        string? error = Validar(servicio);
        if (error != null) return Error(error);

        repositorio.Insertar(servicio);
        return new ResultadoOperacion { Exitoso = true, Mensaje = "Servicio registrado correctamente." };
    }

    public ResultadoOperacion Modificar(Servicio servicio)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Modificar))
            return Error("Permisos insuficientes para modificar servicios.");

        if (servicio.IdServicio <= 0)
            return Error("Seleccione un servicio para modificar.");

        string? error = Validar(servicio);
        if (error != null) return Error(error);

        if (!repositorio.Actualizar(servicio))
            return Error("El servicio ya no existe en la base de datos.");

        return new ResultadoOperacion { Exitoso = true, Mensaje = "Servicio modificado correctamente." };
    }

    public ResultadoOperacion Eliminar(int idServicio)
    {
        if (!SesionActual.TienePermiso(PermisosSistema.Eliminar))
            return Error("Permisos insuficientes para eliminar servicios.");

        try
        {
            if (!repositorio.Eliminar(idServicio))
                return Error("El servicio ya no existe en la base de datos.");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return Error("El servicio tiene citas asociadas y no se puede eliminar. Puede desactivarlo desde Editar.");
        }

        return new ResultadoOperacion { Exitoso = true, Mensaje = "Servicio eliminado correctamente." };
    }

    private static string? Validar(Servicio servicio)
    {
        servicio.Nombre = servicio.Nombre.Trim();
        servicio.Descripcion = string.IsNullOrWhiteSpace(servicio.Descripcion) ? null : servicio.Descripcion.Trim();

        if (servicio.Nombre.Length == 0 || servicio.Nombre.Length > Servicio.MaxNombre)
            return $"El nombre es obligatorio y admite hasta {Servicio.MaxNombre} caracteres.";

        if (servicio.Descripcion?.Length > Servicio.MaxDescripcion)
            return $"La descripción admite hasta {Servicio.MaxDescripcion} caracteres.";

        if (servicio.DuracionMinutos <= 0)
            return "La duración debe ser un número entero de minutos mayor que cero.";

        if (servicio.Precio < 0 || servicio.Precio > Servicio.PrecioMaximo)
            return $"El precio debe estar entre 0 y {Servicio.PrecioMaximo:N2}.";

        if (servicio.Precio.Scale > Servicio.DecimalesPrecio)
            return $"El precio admite hasta {Servicio.DecimalesPrecio} decimales.";

        return null;
    }

    private static ResultadoOperacion Error(string mensaje) => new() { Mensaje = mensaje };
}
