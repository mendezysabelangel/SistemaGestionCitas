using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;
using System.Data;

namespace SistemaGestionCitas.src.Data;

internal sealed record CitaDelDia(TimeSpan Hora, string Cliente, string Servicio, string Estado);

internal sealed record ResumenAgenda(long TotalHoy, long PendientesHoy, long ConfirmadasHoy,
    IReadOnlyDictionary<DateTime, long> DiasConCitas, IReadOnlyList<CitaDelDia> CitasDelDia);

internal sealed class AgendaRepository
{
    public async Task<ResumenAgenda> ConsultarAsync(DateTime seleccion, DateTime hoy, CancellationToken cancelacion)
    {
        if (!SesionActual.HaySesion || !SesionActual.TienePermiso(PermisosSistema.Consultar))
            throw new UnauthorizedAccessException("No tiene permiso para consultar las citas.");

        DateTime inicioMes = new(seleccion.Year, seleccion.Month, 1);
        using SqlConnection conexion = new Conexion().CrearConexion();
        using SqlCommand comando = conexion.CreateCommand();
        comando.CommandTimeout = 8;
        comando.CommandText = """
            SELECT COUNT_BIG(*),
                   COUNT_BIG(CASE WHEN estado.Nombre = 'Pendiente' THEN 1 END),
                   COUNT_BIG(CASE WHEN estado.Nombre = 'Confirmada' THEN 1 END)
            FROM dbo.Citas cita
            JOIN dbo.EstadosCita estado ON estado.IdEstadoCita = cita.IdEstadoCita
            WHERE cita.Fecha = @hoy;

            SELECT Fecha, COUNT_BIG(*)
            FROM dbo.Citas
            WHERE Fecha >= @inicioMes AND Fecha <= @finMes
            GROUP BY Fecha;

            SELECT cita.HoraInicio, CONCAT(cliente.Nombres, ' ', cliente.Apellidos),
                   servicio.Nombre, estado.Nombre
            FROM dbo.Citas cita
            JOIN dbo.Clientes cliente ON cliente.IdCliente = cita.IdCliente
            JOIN dbo.Servicios servicio ON servicio.IdServicio = cita.IdServicio
            JOIN dbo.EstadosCita estado ON estado.IdEstadoCita = cita.IdEstadoCita
            WHERE cita.Fecha = @seleccion
            ORDER BY cita.HoraInicio, cita.IdCita;
            """;
        comando.Parameters.Add("@hoy", SqlDbType.Date).Value = hoy.Date;
        comando.Parameters.Add("@inicioMes", SqlDbType.Date).Value = inicioMes;
        comando.Parameters.Add("@finMes", SqlDbType.Date).Value =
            inicioMes.AddDays(DateTime.DaysInMonth(seleccion.Year, seleccion.Month) - 1);
        comando.Parameters.Add("@seleccion", SqlDbType.Date).Value = seleccion.Date;

        await conexion.OpenAsync(cancelacion).ConfigureAwait(false);
        using SqlDataReader lector = await comando.ExecuteReaderAsync(cancelacion).ConfigureAwait(false);
        await lector.ReadAsync(cancelacion).ConfigureAwait(false);
        long total = lector.GetInt64(0);
        long pendientes = lector.GetInt64(1);
        long confirmadas = lector.GetInt64(2);

        Dictionary<DateTime, long> dias = new();
        await lector.NextResultAsync(cancelacion).ConfigureAwait(false);
        while (await lector.ReadAsync(cancelacion).ConfigureAwait(false))
            dias.Add(lector.GetDateTime(0), lector.GetInt64(1));

        List<CitaDelDia> citas = new();
        await lector.NextResultAsync(cancelacion).ConfigureAwait(false);
        while (await lector.ReadAsync(cancelacion).ConfigureAwait(false))
            citas.Add(new(lector.GetTimeSpan(0), lector.GetString(1), lector.GetString(2), lector.GetString(3)));

        return new(total, pendientes, confirmadas, dias, citas);
    }
}
