namespace SistemaGestionCitas.src.Models;

public sealed record Cita(int IdCita, int IdCliente, int IdEmpleado, int IdServicio, int IdEstadoCita,
    DateOnly Fecha, TimeOnly HoraInicio, TimeOnly HoraFin, string? Observaciones, DateTime FechaCreacion,
    int IdUsuarioCreacion, string Cliente, string Empleado, string Servicio, string Estado);

public sealed record EstadoCita(int IdEstadoCita, string Nombre, bool Activo);

public sealed record ValidacionProgramacionCita(bool EsValida, string Mensaje, TimeOnly? HoraFin = null);

public sealed record DatosCita(int IdCliente, int IdEmpleado, int IdServicio, DateOnly Fecha,
    TimeOnly HoraInicio, string? Observaciones = null);

public sealed record ResultadoCita(bool Exitoso, string Mensaje, int? IdCita = null);

internal static class NombresEstadoCita
{
    internal const string Pendiente = "Pendiente";
    internal const string Confirmada = "Confirmada";
    internal const string Completada = "Completada";
    internal const string Cancelada = "Cancelada";
    internal const string NoAsistio = "No asistió";
}
