using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;
using static SistemaGestionCitas.src.Models.NombresEstadoCita;

namespace SistemaGestionCitas.src.Services;

public sealed class CitaService
{
    private readonly CitaRepository repositorio = new();
    private readonly BitacoraCitasRepository bitacora = new();
    private readonly TimeProvider reloj;

    public CitaService() : this(TimeProvider.System) { }

    internal CitaService(TimeProvider reloj) => this.reloj = reloj;

    internal (List<Cliente> Clientes, List<Empleado> Empleados, List<Servicio> Servicios) ObtenerOpcionesRegistro()
    {
        if (!SesionActual.HaySesion || !SesionActual.TienePermiso(PermisosSistema.Agregar))
            throw new UnauthorizedAccessException("No tiene permiso para registrar citas.");

        return ObtenerOpcionesActivas();
    }

    internal (List<Cliente> Clientes, List<Empleado> Empleados, List<Servicio> Servicios) ObtenerOpcionesEdicion()
    {
        ExigirPermisoConsulta();
        if (!PuedeModificar()) throw new UnauthorizedAccessException("No tiene permiso para editar citas.");
        return ObtenerOpcionesActivas();
    }

    private static (List<Cliente> Clientes, List<Empleado> Empleados, List<Servicio> Servicios) ObtenerOpcionesActivas() =>
        (new ClienteRepository().Buscar(null, null, true),
            new EmpleadoRepository().Buscar(null, true), new ServicioRepository().Buscar(null, true));

    public async Task<IReadOnlyList<Cita>> ConsultarPorFechaAsync(DateOnly fecha, CancellationToken cancelacion = default)
    {
        ExigirPermisoConsulta();
        return await repositorio.ConsultarPorFechaAsync(fecha, cancelacion).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<EstadoCita>> ConsultarEstadosAsync(CancellationToken cancelacion = default)
    {
        ExigirPermisoConsulta();
        return await repositorio.ConsultarEstadosAsync(cancelacion).ConfigureAwait(false);
    }

    public Task<Cita?> ConsultarPorIdAsync(int idCita, CancellationToken cancelacion = default)
    {
        ExigirPermisoConsulta();
        return repositorio.ConsultarPorIdAsync(idCita, cancelacion);
    }

    public async Task<ValidacionProgramacionCita> ComprobarDisponibilidadAsync(int idEmpleado, DateOnly fecha,
        TimeOnly inicio, TimeOnly fin, int? idCitaEnEdicion = null, CancellationToken cancelacion = default)
    {
        ExigirPermisoConsulta();
        if (idEmpleado <= 0)
            return new(false, "Selecciona un empleado válido.");
        if (idCitaEnEdicion is <= 0)
            return new(false, "El ID de la cita en edición debe ser positivo.");
        if (fin <= inicio)
            return new(false, "La hora de fin debe ser posterior a la hora de inicio del mismo día.");

        var (empleadoActivo, hayCruce) = await repositorio.ConsultarDisponibilidadAsync(
            idEmpleado, fecha, inicio, fin, idCitaEnEdicion, cancelacion).ConfigureAwait(false);
        if (!empleadoActivo)
            return new(false, "El empleado no existe o está inactivo.");
        if (hayCruce)
            return new(false, "El empleado ya tiene una cita que ocupa parte de ese horario.");

        return new(true, "El horario está disponible.", fin);
    }

    public async Task<ValidacionProgramacionCita> ValidarProgramacionAsync(DatosCita datos, int? idCitaEnEdicion = null,
        CancellationToken cancelacion = default)
    {
        ExigirPermisoConsulta();
        if (idCitaEnEdicion is <= 0)
            return new(false, "El ID de la cita en edición debe ser positivo.");
        Cita? actual = null;
        if (idCitaEnEdicion is int id)
        {
            actual = await repositorio.ConsultarPorIdAsync(id, cancelacion).ConfigureAwait(false);
            if (actual == null || !Editable(actual.Estado))
                return new(false, "Solo se pueden editar citas pendientes o confirmadas.");
        }
        return await ValidarDatosAsync(datos, actual, null, cancelacion).ConfigureAwait(false);
    }

    public Task<ResultadoCita> RegistrarAsync(DatosCita datos, CancellationToken cancelacion = default)
    {
        Usuario? usuario = SesionActual.UsuarioActual;
        if (usuario == null || !SesionActual.TienePermiso(PermisosSistema.Agregar))
            return Task.FromResult(new ResultadoCita(false, "No tiene permiso para registrar citas."));
        int idUsuario = usuario.IdUsuario;
        string nombreUsuario = usuario.NombreUsuario;

        return repositorio.EnTransaccionAsync(async transaccion =>
        {
            var validacion = await ValidarDatosAsync(datos, null, transaccion, cancelacion).ConfigureAwait(false);
            if (validacion.HoraFin is not TimeOnly fin)
                return new(false, validacion.Mensaje);
            int? estado = await repositorio.BuscarEstadoActivoAsync(Pendiente, transaccion, cancelacion).ConfigureAwait(false);
            if (estado is not int idEstado)
                return new(false, "El estado Pendiente no está disponible.");

            int idCita = await repositorio.InsertarAsync(datos, fin, idEstado, idUsuario, transaccion, cancelacion)
                .ConfigureAwait(false);
            if (!await bitacora.RegistrarAsync(idCita, idUsuario, nombreUsuario, "Cita registrada", null, Pendiente,
                    transaccion, cancelacion).ConfigureAwait(false))
                return new(false, BitacoraCitasRepository.MensajeInstalacion);
            return new(true, "Cita registrada correctamente.", idCita);
        }, cancelacion);
    }

    public Task<ResultadoCita> ModificarAsync(int idCita, DatosCita datos, CancellationToken cancelacion = default)
    {
        Usuario? usuario = SesionActual.UsuarioActual;
        if (usuario == null || !SesionActual.TienePermiso(PermisosSistema.Modificar))
            return Task.FromResult(new ResultadoCita(false, "No tiene permiso para modificar citas."));
        if (idCita <= 0)
            return Task.FromResult(new ResultadoCita(false, "Selecciona una cita válida."));

        int idUsuario = usuario.IdUsuario;
        string nombreUsuario = usuario.NombreUsuario;
        return repositorio.EnTransaccionAsync(async transaccion =>
        {
            Cita? actual = await repositorio.ConsultarPorIdAsync(idCita, cancelacion, transaccion).ConfigureAwait(false);
            if (actual == null)
                return new(false, "La cita no existe.");
            if (!Editable(actual.Estado))
                return new(false, "Solo se pueden editar citas pendientes o confirmadas.");

            var validacion = await ValidarDatosAsync(datos, actual, transaccion, cancelacion).ConfigureAwait(false);
            if (validacion.HoraFin is not TimeOnly fin)
                return new(false, validacion.Mensaje);

            await repositorio.ActualizarAsync(idCita, datos, fin, transaccion, cancelacion).ConfigureAwait(false);
            if (!await bitacora.RegistrarAsync(idCita, idUsuario, nombreUsuario, "Cita editada", null, actual.Estado,
                    transaccion, cancelacion).ConfigureAwait(false))
                return new(false, BitacoraCitasRepository.MensajeInstalacion);
            return new(true, "Cita modificada correctamente.", idCita);
        }, cancelacion);
    }

    public Task<ResultadoCita> ConfirmarAsync(int idCita, CancellationToken cancelacion = default) =>
        CambiarEstadoAsync(idCita, Confirmada, cancelacion);

    public Task<ResultadoCita> CompletarAsync(int idCita, CancellationToken cancelacion = default) =>
        CambiarEstadoAsync(idCita, Completada, cancelacion);

    public Task<ResultadoCita> CancelarAsync(int idCita, CancellationToken cancelacion = default) =>
        CambiarEstadoAsync(idCita, Cancelada, cancelacion);

    public Task<ResultadoCita> MarcarInasistenciaAsync(int idCita, CancellationToken cancelacion = default) =>
        CambiarEstadoAsync(idCita, NoAsistio, cancelacion);

    private Task<ResultadoCita> CambiarEstadoAsync(int idCita, string destino, CancellationToken cancelacion)
    {
        Usuario? usuario = SesionActual.UsuarioActual;
        if (usuario == null || !SesionActual.TienePermiso(PermisosSistema.Modificar))
            return Task.FromResult(new ResultadoCita(false, "No tiene permiso para cambiar el estado de citas."));
        if (idCita <= 0)
            return Task.FromResult(new ResultadoCita(false, "Selecciona una cita válida."));

        int idUsuario = usuario.IdUsuario;
        string nombreUsuario = usuario.NombreUsuario;
        return repositorio.EnTransaccionAsync(async transaccion =>
        {
            Cita? actual = await repositorio.ConsultarPorIdAsync(idCita, cancelacion, transaccion).ConfigureAwait(false);
            if (actual == null)
                return new(false, "La cita no existe.");

            if (!PermiteTransicion(actual.Estado, destino))
                return new(false, $"No se puede cambiar una cita de «{actual.Estado}» a «{destino}».");
            int? estado = await repositorio.BuscarEstadoActivoAsync(destino, transaccion, cancelacion).ConfigureAwait(false);
            if (estado is not int idEstado)
                return new(false, $"El estado «{destino}» no está disponible.");

            await repositorio.ActualizarEstadoAsync(idCita, idEstado, transaccion, cancelacion).ConfigureAwait(false);
            string accion = destino switch
            {
                Confirmada => "Cita confirmada",
                Cancelada => "Cita cancelada",
                Completada => "Cita completada",
                NoAsistio => "Inasistencia registrada",
                _ => throw new InvalidOperationException("El destino no corresponde a una transición de Citas.")
            };
            if (!await bitacora.RegistrarAsync(idCita, idUsuario, nombreUsuario, accion, actual.Estado,
                    destino, transaccion, cancelacion).ConfigureAwait(false))
                return new(false, BitacoraCitasRepository.MensajeInstalacion);
            return new(true, $"Estado actualizado a «{destino}».", idCita);
        }, cancelacion);
    }

    private async Task<ValidacionProgramacionCita> ValidarDatosAsync(DatosCita datos, Cita? actual,
        SqlTransaction? transaccion, CancellationToken cancelacion)
    {
        ArgumentNullException.ThrowIfNull(datos);
        if (datos.IdCliente <= 0 || datos.IdEmpleado <= 0 || datos.IdServicio <= 0)
            return new(false, "Selecciona un cliente, un empleado y un servicio válidos.");
        if (datos.Observaciones?.Length > 500)
            return new(false, "Las observaciones admiten hasta 500 caracteres.");

        var (clienteActivo, empleadoActivo, duracion) = await repositorio.ConsultarReferenciasAsync(
            datos, cancelacion, transaccion).ConfigureAwait(false);
        if (actual?.IdCliente != datos.IdCliente && !clienteActivo)
            return new(false, "El cliente no existe o está inactivo.");
        if (actual?.IdEmpleado != datos.IdEmpleado && !empleadoActivo)
            return new(false, "El empleado no existe o está inactivo.");
        // Se conserva la duración reservada si el servicio sigue siendo el mismo.
        TimeSpan? intervalo = actual != null && actual.IdServicio == datos.IdServicio
            ? actual.HoraFin.ToTimeSpan() - actual.HoraInicio.ToTimeSpan()
            : duracion is > 0 ? TimeSpan.FromMinutes(duracion.Value) : null;
        if (intervalo is not TimeSpan duracionCita)
            return new(false, "El servicio debe estar activo y tener una duración válida.");
        TimeSpan fin = datos.HoraInicio.ToTimeSpan() + duracionCita;
        if (fin >= TimeSpan.FromDays(1))
            return new(false, "La duración del servicio hace que la cita termine fuera del mismo día.");

        TimeOnly horaFin = TimeOnly.FromTimeSpan(fin);
        var (_, hayCruce) = await repositorio.ConsultarDisponibilidadAsync(datos.IdEmpleado, datos.Fecha,
            datos.HoraInicio, horaFin, actual?.IdCita, cancelacion, transaccion).ConfigureAwait(false);

        // Al guardar, esta validación se ejecuta dentro de la transacción, después de adquirir el bloqueo.
        bool cambiaHorario = actual == null || actual.Fecha != datos.Fecha || actual.HoraInicio != datos.HoraInicio;
        if (cambiaHorario && datos.Fecha.ToDateTime(datos.HoraInicio) <= reloj.GetLocalNow().DateTime)
            return new(false, "No se pueden agendar citas en fechas u horarios anteriores al momento actual. El inicio debe ser posterior a la hora actual.");

        return hayCruce ? new(false, "El empleado ya tiene una cita que ocupa parte de ese horario.")
            : new(true, "El horario está disponible.", horaFin);
    }

    internal static bool Editable(string estado) => estado is Pendiente or Confirmada;

    internal static bool PermiteTransicion(string origen, string destino) => origen switch
    {
        Pendiente => destino is Confirmada or Cancelada,
        Confirmada => destino is Completada or Cancelada or NoAsistio,
        _ => false
    };

    private static bool PuedeModificar() => SesionActual.HaySesion && SesionActual.TienePermiso(PermisosSistema.Modificar);

    private static void ExigirPermisoConsulta()
    {
        if (!SesionActual.HaySesion || !SesionActual.TienePermiso(PermisosSistema.Consultar))
            throw new UnauthorizedAccessException("No tiene permiso para consultar las citas.");
    }
}
