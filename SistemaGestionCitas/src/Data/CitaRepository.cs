using System.Data;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data;

internal sealed class CitaRepository
{
    private const int TiempoLimiteSegundos = 8;
    private const string SeleccionarCitas = """
        SELECT cita.IdCita, cita.IdCliente, cita.IdEmpleado, cita.IdServicio, cita.IdEstadoCita,
               cita.Fecha, cita.HoraInicio, cita.HoraFin, cita.Observaciones, cita.FechaCreacion,
               cita.IdUsuarioCreacion, CONCAT(cliente.Nombres, ' ', cliente.Apellidos),
               CONCAT(empleado.Nombres, ' ', empleado.Apellidos), servicio.Nombre, estado.Nombre
        FROM dbo.Citas cita
        JOIN dbo.Clientes cliente ON cliente.IdCliente = cita.IdCliente
        JOIN dbo.Empleados empleado ON empleado.IdEmpleado = cita.IdEmpleado
        JOIN dbo.Servicios servicio ON servicio.IdServicio = cita.IdServicio
        JOIN dbo.EstadosCita estado ON estado.IdEstadoCita = cita.IdEstadoCita
        """;

    internal const string ConsultaDelDia = SeleccionarCitas +
        "\nWHERE cita.Fecha = @seleccion ORDER BY cita.HoraInicio, cita.IdCita;";

    private const string ConsultaCruces = """
        SELECT CAST(CASE WHEN EXISTS (
            SELECT 1 FROM dbo.Citas cita
            JOIN dbo.EstadosCita estado ON estado.IdEstadoCita = cita.IdEstadoCita
            WHERE cita.Fecha = @fecha AND cita.IdEmpleado = @empleado
              AND cita.HoraInicio < @fin AND cita.HoraFin > @inicio
              AND (@excluir IS NULL OR cita.IdCita <> @excluir)
              AND estado.Nombre <> @cancelada
        ) THEN 1 ELSE 0 END AS bit);
        """;

    private const string EmpleadoActivo = """
        EXISTS (SELECT 1 FROM dbo.Empleados WHERE IdEmpleado = @empleado AND Activo = 1)
        """;

    internal async Task<List<Cita>> ConsultarPorFechaAsync(DateOnly fecha, CancellationToken cancelacion)
    {
        using SqlConnection conexion = new Conexion().CrearConexion();
        using var comando = new SqlCommand(ConsultaDelDia, conexion) { CommandTimeout = TiempoLimiteSegundos };
        comando.Parameters.Add("@seleccion", SqlDbType.Date).Value = fecha;
        await conexion.OpenAsync(cancelacion).ConfigureAwait(false);
        using SqlDataReader lector = await comando.ExecuteReaderAsync(cancelacion).ConfigureAwait(false);
        return await LeerCitasAsync(lector, cancelacion).ConfigureAwait(false);
    }

    internal Task<Cita?> ConsultarPorIdAsync(int idCita, CancellationToken cancelacion, SqlTransaction? transaccion = null) =>
        ConConexionAsync(transaccion, cancelacion, async conexion =>
        {
            const string sql = SeleccionarCitas + "\nWHERE cita.IdCita = @id;";
            using var comando = new SqlCommand(sql, conexion, transaccion) { CommandTimeout = TiempoLimiteSegundos };
            comando.Parameters.Add("@id", SqlDbType.Int).Value = idCita;
            using SqlDataReader lector = await comando.ExecuteReaderAsync(cancelacion).ConfigureAwait(false);
            return (await LeerCitasAsync(lector, cancelacion).ConfigureAwait(false)).SingleOrDefault();
        });

    internal static async Task<List<Cita>> LeerCitasAsync(SqlDataReader lector, CancellationToken cancelacion)
    {
        var citas = new List<Cita>();
        while (await lector.ReadAsync(cancelacion).ConfigureAwait(false))
        {
            citas.Add(new Cita(lector.GetInt32(0), lector.GetInt32(1), lector.GetInt32(2), lector.GetInt32(3),
                lector.GetInt32(4), DateOnly.FromDateTime(lector.GetDateTime(5)), TimeOnly.FromTimeSpan(lector.GetTimeSpan(6)),
                TimeOnly.FromTimeSpan(lector.GetTimeSpan(7)), lector.IsDBNull(8) ? null : lector.GetString(8),
                lector.GetDateTime(9), lector.GetInt32(10), lector.GetString(11), lector.GetString(12),
                lector.GetString(13), lector.GetString(14)));
        }
        return citas;
    }

    internal async Task<List<EstadoCita>> ConsultarEstadosAsync(CancellationToken cancelacion)
    {
        const string sql = "SELECT IdEstadoCita, Nombre, Activo FROM dbo.EstadosCita ORDER BY Nombre;";
        using SqlConnection conexion = new Conexion().CrearConexion();
        using var comando = new SqlCommand(sql, conexion) { CommandTimeout = TiempoLimiteSegundos };
        await conexion.OpenAsync(cancelacion).ConfigureAwait(false);
        using SqlDataReader lector = await comando.ExecuteReaderAsync(cancelacion).ConfigureAwait(false);
        var estados = new List<EstadoCita>();
        while (await lector.ReadAsync(cancelacion).ConfigureAwait(false))
            estados.Add(new EstadoCita(lector.GetInt32(0), lector.GetString(1), lector.GetBoolean(2)));
        return estados;
    }

    internal Task<(bool ClienteActivo, bool EmpleadoActivo, int? DuracionMinutos)> ConsultarReferenciasAsync(
        DatosCita datos, CancellationToken cancelacion, SqlTransaction? transaccion) =>
        ConConexionAsync(transaccion, cancelacion, async conexion =>
        {
            const string sql = $"""
                SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.Clientes WHERE IdCliente = @cliente AND Activo = 1)
                            THEN 1 ELSE 0 END AS bit),
                       CAST(CASE WHEN {EmpleadoActivo} THEN 1 ELSE 0 END AS bit),
                       (SELECT DuracionMinutos FROM dbo.Servicios WHERE IdServicio = @servicio AND Activo = 1);
                """;
            using var comando = new SqlCommand(sql, conexion, transaccion) { CommandTimeout = TiempoLimiteSegundos };
            comando.Parameters.Add("@cliente", SqlDbType.Int).Value = datos.IdCliente;
            comando.Parameters.Add("@empleado", SqlDbType.Int).Value = datos.IdEmpleado;
            comando.Parameters.Add("@servicio", SqlDbType.Int).Value = datos.IdServicio;
            using SqlDataReader lector = await comando.ExecuteReaderAsync(cancelacion).ConfigureAwait(false);
            await lector.ReadAsync(cancelacion).ConfigureAwait(false);
            return (lector.GetBoolean(0), lector.GetBoolean(1), lector.IsDBNull(2) ? (int?)null : lector.GetInt32(2));
        });

    internal Task<(bool EmpleadoActivo, bool HayCruce)> ConsultarDisponibilidadAsync(
        int idEmpleado, DateOnly fecha, TimeOnly inicio, TimeOnly fin, int? excluir, CancellationToken cancelacion,
        SqlTransaction? transaccion = null) =>
        ConConexionAsync(transaccion, cancelacion, async conexion =>
        {
            const string sql = "SELECT CAST(CASE WHEN " + EmpleadoActivo + " THEN 1 ELSE 0 END AS bit);\n" + ConsultaCruces;
            using var comando = new SqlCommand(sql, conexion, transaccion) { CommandTimeout = TiempoLimiteSegundos };
            comando.Parameters.Add("@empleado", SqlDbType.Int).Value = idEmpleado;
            comando.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha;
            comando.Parameters.Add("@inicio", SqlDbType.Time).Value = inicio;
            comando.Parameters.Add("@fin", SqlDbType.Time).Value = fin;
            comando.Parameters.Add("@excluir", SqlDbType.Int).Value = (object?)excluir ?? DBNull.Value;
            comando.Parameters.Add("@cancelada", SqlDbType.VarChar, 100).Value = NombresEstadoCita.Cancelada;
            using SqlDataReader lector = await comando.ExecuteReaderAsync(cancelacion).ConfigureAwait(false);
            await lector.ReadAsync(cancelacion).ConfigureAwait(false);
            bool activo = lector.GetBoolean(0);
            await lector.NextResultAsync(cancelacion).ConfigureAwait(false);
            await lector.ReadAsync(cancelacion).ConfigureAwait(false);
            return (activo, lector.GetBoolean(0));
        });

    internal async Task<ResultadoCita> EnTransaccionAsync(Func<SqlTransaction, Task<ResultadoCita>> ejecutar,
        CancellationToken cancelacion)
    {
        using SqlConnection conexion = new Conexion().CrearConexion();
        await conexion.OpenAsync(cancelacion).ConfigureAwait(false);
        await using SqlTransaction transaccion = (SqlTransaction)await conexion.BeginTransactionAsync(
            IsolationLevel.Serializable, cancelacion).ConfigureAwait(false);
        // Todas las escrituras de citas toman el mismo bloqueo antes de leer. SQL Server lo libera al cerrar la transacción.
        const string sql = """
            DECLARE @resultado int;
            EXEC @resultado = sys.sp_getapplock
                @Resource = N'Citorna.Citas.Escritura', @LockMode = 'Exclusive',
                @LockOwner = 'Transaction', @LockTimeout = 6000;
            SELECT @resultado;
            """;
        using var comando = new SqlCommand(sql, conexion, transaccion) { CommandTimeout = TiempoLimiteSegundos };
        if ((int)(await comando.ExecuteScalarAsync(cancelacion).ConfigureAwait(false))! < 0)
            return new(false, "La agenda está ocupada. Vuelve a intentar la operación.");

        ResultadoCita resultado = await ejecutar(transaccion).ConfigureAwait(false);
        if (resultado.Exitoso)
        {
            cancelacion.ThrowIfCancellationRequested();
            await transaccion.CommitAsync(CancellationToken.None).ConfigureAwait(false);
        }
        return resultado;
    }

    internal async Task<int?> BuscarEstadoActivoAsync(string nombre, SqlTransaction transaccion, CancellationToken cancelacion)
    {
        const string sql = "SELECT IdEstadoCita FROM dbo.EstadosCita WHERE Nombre = @estado AND Activo = 1;";
        using var comando = new SqlCommand(sql, transaccion.Connection, transaccion) { CommandTimeout = TiempoLimiteSegundos };
        comando.Parameters.Add("@estado", SqlDbType.VarChar, 100).Value = nombre;
        return (int?)await comando.ExecuteScalarAsync(cancelacion).ConfigureAwait(false);
    }

    internal async Task<int> InsertarAsync(DatosCita datos, TimeOnly fin, int idEstado, int idUsuario,
        SqlTransaction transaccion, CancellationToken cancelacion)
    {
        const string sql = """
            INSERT INTO dbo.Citas (IdCliente, IdEmpleado, IdServicio, IdEstadoCita, Fecha, HoraInicio, HoraFin,
                                   Observaciones, IdUsuarioCreacion)
            OUTPUT INSERTED.IdCita
            VALUES (@cliente, @empleado, @servicio, @estado, @fecha, @inicio, @fin, @observaciones, @usuario);
            """;
        using var comando = new SqlCommand(sql, transaccion.Connection, transaccion) { CommandTimeout = TiempoLimiteSegundos };
        AgregarDatos(comando, datos, fin);
        comando.Parameters.Add("@estado", SqlDbType.Int).Value = idEstado;
        comando.Parameters.Add("@usuario", SqlDbType.Int).Value = idUsuario;
        return (int)(await comando.ExecuteScalarAsync(cancelacion).ConfigureAwait(false))!;
    }

    internal async Task ActualizarAsync(int idCita, DatosCita datos, TimeOnly fin, SqlTransaction transaccion,
        CancellationToken cancelacion)
    {
        const string sql = """
            UPDATE dbo.Citas
            SET IdCliente = @cliente, IdEmpleado = @empleado, IdServicio = @servicio, Fecha = @fecha,
                HoraInicio = @inicio, HoraFin = @fin, Observaciones = @observaciones
            WHERE IdCita = @id;
            """;
        using var comando = new SqlCommand(sql, transaccion.Connection, transaccion) { CommandTimeout = TiempoLimiteSegundos };
        AgregarDatos(comando, datos, fin);
        comando.Parameters.Add("@id", SqlDbType.Int).Value = idCita;
        await comando.ExecuteNonQueryAsync(cancelacion).ConfigureAwait(false);
    }

    internal async Task ActualizarEstadoAsync(int idCita, int idEstado, SqlTransaction transaccion, CancellationToken cancelacion)
    {
        const string sql = "UPDATE dbo.Citas SET IdEstadoCita = @estado WHERE IdCita = @id;";
        using var comando = new SqlCommand(sql, transaccion.Connection, transaccion) { CommandTimeout = TiempoLimiteSegundos };
        comando.Parameters.Add("@id", SqlDbType.Int).Value = idCita;
        comando.Parameters.Add("@estado", SqlDbType.Int).Value = idEstado;
        await comando.ExecuteNonQueryAsync(cancelacion).ConfigureAwait(false);
    }

    private static void AgregarDatos(SqlCommand comando, DatosCita datos, TimeOnly fin)
    {
        comando.Parameters.Add("@cliente", SqlDbType.Int).Value = datos.IdCliente;
        comando.Parameters.Add("@empleado", SqlDbType.Int).Value = datos.IdEmpleado;
        comando.Parameters.Add("@servicio", SqlDbType.Int).Value = datos.IdServicio;
        comando.Parameters.Add("@fecha", SqlDbType.Date).Value = datos.Fecha;
        comando.Parameters.Add("@inicio", SqlDbType.Time).Value = datos.HoraInicio;
        comando.Parameters.Add("@fin", SqlDbType.Time).Value = fin;
        comando.Parameters.Add("@observaciones", SqlDbType.VarChar, 500).Value = (object?)datos.Observaciones ?? DBNull.Value;
    }

    private static async Task<T> ConConexionAsync<T>(SqlTransaction? transaccion, CancellationToken cancelacion,
        Func<SqlConnection, Task<T>> ejecutar)
    {
        if (transaccion?.Connection is { } actual)
            return await ejecutar(actual).ConfigureAwait(false);
        using SqlConnection nueva = new Conexion().CrearConexion();
        await nueva.OpenAsync(cancelacion).ConfigureAwait(false);
        return await ejecutar(nueva).ConfigureAwait(false);
    }
}
