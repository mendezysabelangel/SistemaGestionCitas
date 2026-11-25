using System.Diagnostics;
using System.Globalization;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

internal sealed class FrmNuevaCita : Form
{
    private readonly CitaService servicioCitas = new();
    private readonly Cita? actual;
    private readonly ComboBox clientes = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox empleados = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox servicios = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker fecha = new() { Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker hora = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "hh:mm tt", ShowUpDown = true };
    private readonly TextBox? observaciones;
    private readonly Label duracion = CampoCitorna.Rotulo("");
    private readonly Label mensaje = CampoCitorna.Rotulo("Cargando opciones…", true);
    private readonly Button guardar = ControlesRegistro.Boton("Guardar", ControlesRegistro.Primario, Color.White);
    private readonly Button cancelar = ControlesRegistro.Boton("Cancelar");
    private readonly Control campos;
    private bool guardando;

    internal DateTime? FechaGuardada { get; private set; }
    private TimeOnly HoraInicio => actual != null && actual.HoraInicio.Hour == hora.Value.Hour
        && actual.HoraInicio.Minute == hora.Value.Minute ? actual.HoraInicio : new(hora.Value.Hour, hora.Value.Minute);
    private string AccionGuardar => actual == null ? "Guardar" : "Guardar cambios";
    private sealed record Opcion(int Id, string Nombre, int DuracionMinutos = 0);

    internal FrmNuevaCita(DateTime seleccion, Cita? actual = null)
    {
        this.actual = actual;
        DateTime ahora = DateTime.Now;
        DateTime inicial = ahora.AddMinutes(15);
        DateTime dia = seleccion.Date > ahora.Date ? seleccion.Date : inicial.Date;
        if (actual != null) dia = actual.Fecha.ToDateTime(TimeOnly.MinValue);
        fecha.Value = dia > fecha.MaxDate ? fecha.MaxDate : dia;
        hora.Value = fecha.Value.Date > inicial.Date ? fecha.Value.Date.AddHours(9) : inicial;
        if (actual != null) hora.Value = fecha.Value.Date.Add(actual.HoraInicio.ToTimeSpan());

        campos = DialogoCitorna.Tabla();
        campos.Controls.Add(DialogoCitorna.Campos(1, ("Cliente", clientes, 38),
            ("Empleado", empleados, 38), ("Servicio", servicios, 38)));
        campos.Controls.Add(DialogoCitorna.Campos(2, ("Fecha", fecha, 38), ("Hora de inicio", hora, 38)));
        campos.Controls.Add(duracion);
        if (actual != null)
        {
            observaciones = new TextBox { Multiline = true, AcceptsReturn = true, MaxLength = 500,
                Text = actual.Observaciones ?? "" };
            campos.Controls.Add(DialogoCitorna.Campos(1, ("Observaciones", observaciones, 80)));
        }
        campos.Enabled = false;
        guardar.Enabled = false;
        guardar.Text = AccionGuardar;
        mensaje.Margin = new Padding(0, 12, 0, 0);
        mensaje.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Assertive;
        var contenido = DialogoCitorna.Tabla();
        contenido.Controls.Add(campos);
        contenido.Controls.Add(mensaje);
        DialogoCitorna.Configurar(this, actual == null ? "Nueva cita" : "Editar cita",
            actual == null ? "Selecciona los datos y el horario de la reserva." : "Actualiza los datos de la reserva.",
            new Size(510, actual == null ? 490 : 605), contenido, guardar, cancelar);

        clientes.SelectedIndexChanged += (_, _) => ActualizarResumen();
        empleados.SelectedIndexChanged += (_, _) => ActualizarResumen();
        servicios.SelectedIndexChanged += (_, _) => ActualizarResumen();
        hora.ValueChanged += (_, _) => ActualizarResumen();
        guardar.Click += async (_, _) => await GuardarAsync();
        Shown += async (_, _) => await CargarOpcionesAsync();
        FormClosing += (_, e) => { if (guardando) e.Cancel = true; };
        ActualizarResumen();
    }

    private async Task CargarOpcionesAsync()
    {
        try
        {
            var opciones = actual == null
                ? await Task.Run(servicioCitas.ObtenerOpcionesRegistro)
                : await Task.Run(servicioCitas.ObtenerOpcionesEdicion);
            if (IsDisposed || !Visible) return;

            Vincular(clientes, opciones.Clientes.Select(c => new Opcion(c.IdCliente, c.NombreCompleto)),
                actual == null ? null : new(actual.IdCliente, actual.Cliente));
            Vincular(empleados, opciones.Empleados.Select(e => new Opcion(e.IdEmpleado, e.NombreCompleto)),
                actual == null ? null : new(actual.IdEmpleado, actual.Empleado));
            Vincular(servicios, opciones.Servicios.Select(s => new Opcion(s.IdServicio, s.Nombre, s.DuracionMinutos)),
                actual == null ? null : new(actual.IdServicio, actual.Servicio));

            var faltantes = new List<string>();
            if (clientes.Items.Count == 0) faltantes.Add("clientes");
            if (empleados.Items.Count == 0) faltantes.Add("empleados");
            if (servicios.Items.Count == 0) faltantes.Add("servicios");
            campos.Enabled = faltantes.Count == 0;
            MostrarMensaje(campos.Enabled ? "" : $"Faltan registros activos de {string.Join(", ", faltantes)}. Se necesita uno de cada tipo para guardar.");
            ActualizarResumen();
            if (campos.Enabled) clientes.Focus();
        }
        catch (UnauthorizedAccessException ex)
        {
            if (!IsDisposed && Visible) MostrarMensaje(ex.Message);
        }
        catch (Exception ex)
        {
            Trace.TraceWarning("Citorna, opciones de cita: {0}", ex.GetType().Name);
            if (!IsDisposed && Visible) MostrarMensaje("No se pudieron cargar las opciones. Comprueba la conexión y vuelve a abrir el formulario.");
        }
    }

    private static void Vincular(ComboBox combo, IEnumerable<Opcion> opciones, Opcion? seleccion)
    {
        var lista = opciones.ToList();
        int indice = -1;
        if (seleccion != null)
        {
            indice = lista.FindIndex(opcion => opcion.Id == seleccion.Id);
            if (indice < 0)
            {
                indice = lista.Count;
                lista.Add(seleccion with { Nombre = $"{seleccion.Nombre} (inactivo, actual)" });
            }
        }
        combo.DisplayMember = nameof(Opcion.Nombre);
        combo.DataSource = lista;
        combo.SelectedIndex = indice;
    }

    private void ActualizarResumen()
    {
        guardar.Enabled = campos.Enabled && !guardando && clientes.SelectedItem is Opcion
            && empleados.SelectedItem is Opcion && servicios.SelectedItem is Opcion;
        if (servicios.SelectedItem is not Opcion elegir)
        {
            duracion.Text = "Selecciona un servicio para ver la duración y la hora de fin.";
            return;
        }

        TimeSpan intervalo = actual?.IdServicio == elegir.Id
            ? actual.HoraFin.ToTimeSpan() - actual.HoraInicio.ToTimeSpan()
            : TimeSpan.FromMinutes(elegir.DuracionMinutos);
        TimeSpan fin = HoraInicio.ToTimeSpan() + intervalo;
        string final = fin >= TimeSpan.FromDays(1) ? "fuera del mismo día"
            : TimeOnly.FromTimeSpan(fin).ToString("h:mm tt", CultureInfo.InvariantCulture);
        duracion.Text = $"Duración {intervalo.TotalMinutes:0.##} min · Fin {final}";
    }

    private async Task GuardarAsync()
    {
        if (guardando || !campos.Enabled || clientes.SelectedItem is not Opcion cliente
            || empleados.SelectedItem is not Opcion empleado || servicios.SelectedItem is not Opcion servicio)
            return;
        if (actual == null)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Agregar, "registrar citas")) return;
        }
        else if (!SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar citas")
            || !SesionActual.VerificarPermiso(PermisosSistema.Modificar, "editar citas")) return;

        string? notas = observaciones?.Text;
        var datos = new DatosCita(cliente.Id, empleado.Id, servicio.Id,
            DateOnly.FromDateTime(fecha.Value), HoraInicio, string.IsNullOrWhiteSpace(notas) ? null : notas);
        guardando = true;
        campos.Enabled = guardar.Enabled = cancelar.Enabled = false;
        guardar.Text = "Guardando…";
        MostrarMensaje("");
        try
        {
            ResultadoCita resultado = actual == null ? await servicioCitas.RegistrarAsync(datos)
                : await servicioCitas.ModificarAsync(actual.IdCita, datos);
            if (resultado.Exitoso) FechaGuardada = datos.Fecha.ToDateTime(TimeOnly.MinValue);
            else MostrarMensaje(resultado.Mensaje);
        }
        catch (Exception ex)
        {
            Trace.TraceWarning("Citorna, guardar cita: {0}", ex.GetType().Name);
            MostrarMensaje("No se pudo confirmar el guardado. Consulta la agenda antes de volver a intentarlo.");
        }
        finally
        {
            guardando = false;
            guardar.Text = AccionGuardar;
            campos.Enabled = cancelar.Enabled = true;
            ActualizarResumen();
        }

        if (FechaGuardada.HasValue) DialogResult = DialogResult.OK;
    }

    private void MostrarMensaje(string texto)
    {
        mensaje.Text = texto;
        mensaje.ForeColor = ControlesRegistro.Peligro;
        mensaje.Visible = texto.Length > 0;
    }
}
