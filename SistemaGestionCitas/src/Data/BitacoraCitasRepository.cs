using System.Data;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data;

internal sealed class BitacoraCitasRepository
{
    internal const string MensajeInstalacion = "La bitácora de Citas requiere el script 005_CreateBitacoraCitas.sql.";

    internal async Task<bool> RegistrarAsync(int idCita, int idUsuario, string nombreUsuario, string accion,
        string? estadoAnterior, string estadoNuevo, SqlTransaction transaccion, CancellationToken cancelacion)
    {
        const string sql = """
            INSERT INTO dbo.BitacoraCitas (IdCita, IdUsuario, NombreUsuario, Accion, EstadoAnterior, EstadoNuevo)
            VALUES (@cita, @usuario, @nombreUsuario, @accion, @anterior, @nuevo);
            """;
        using var comando = new SqlCommand(sql, transaccion.Connection, transaccion);
        comando.Parameters.Add("@cita", SqlDbType.Int).Value = idCita;
        comando.Parameters.Add("@usuario", SqlDbType.Int).Value = idUsuario;
        comando.Parameters.Add("@nombreUsuario", SqlDbType.NVarChar, 50).Value = nombreUsuario;
        comando.Parameters.Add("@accion", SqlDbType.NVarChar, 30).Value = accion;
        comando.Parameters.Add("@anterior", SqlDbType.NVarChar, 100).Value = (object?)estadoAnterior ?? DBNull.Value;
        comando.Parameters.Add("@nuevo", SqlDbType.NVarChar, 100).Value = estadoNuevo;
        try
        {
            await comando.ExecuteNonQueryAsync(cancelacion).ConfigureAwait(false);
            return true;
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            return false;
        }
    }

    internal async Task<List<EventoBitacoraCita>> ConsultarRecientesAsync(int limite, CancellationToken cancelacion)
    {
        const string sql = """
            SELECT TOP (@limite) FechaEvento, NombreUsuario, Accion, IdCita, EstadoAnterior, EstadoNuevo
            FROM dbo.BitacoraCitas
            ORDER BY IdEvento DESC;
            """;
        using SqlConnection conexion = new Conexion().CrearConexion();
        using var comando = new SqlCommand(sql, conexion) { CommandTimeout = 8 };
        comando.Parameters.Add("@limite", SqlDbType.Int).Value = limite;
        await conexion.OpenAsync(cancelacion).ConfigureAwait(false);
        using SqlDataReader lector = await comando.ExecuteReaderAsync(cancelacion).ConfigureAwait(false);
        var eventos = new List<EventoBitacoraCita>();
        while (await lector.ReadAsync(cancelacion).ConfigureAwait(false))
            eventos.Add(new(lector.GetDateTime(0), lector.GetString(1), lector.GetString(2), lector.GetInt32(3),
                lector.IsDBNull(4) ? null : lector.GetString(4), lector.GetString(5)));
        return eventos;
    }
}
