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

            """ + "\n" + CitaRepository.ConsultaDelDia;
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

        await lector.NextResultAsync(cancelacion).ConfigureAwait(false);
        List<Cita> citas = await CitaRepository.LeerCitasAsync(lector, cancelacion).ConfigureAwait(false);
        return new(total, pendientes, confirmadas, dias,
            citas.ConvertAll(cita => new CitaDelDia(cita.HoraInicio.ToTimeSpan(), cita.Cliente, cita.Servicio, cita.Estado)));
    }
}
