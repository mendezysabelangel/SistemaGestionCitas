using System.Diagnostics;
using System.Globalization;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

internal sealed class FrmNuevaCita : Form
{
    private readonly CitaService servicioCitas = new();
    private readonly ComboBox clientes = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox empleados = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox servicios = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker fecha = new() { Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker hora = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "hh:mm tt", ShowUpDown = true };
    private readonly Label duracion = CampoCitorna.Rotulo("");
    private readonly Label mensaje = CampoCitorna.Rotulo("Cargando opciones…", true);
    private readonly Button guardar = ControlesRegistro.Boton("Guardar", ControlesRegistro.Primario, Color.White);
    private readonly Button cancelar = ControlesRegistro.Boton("Cancelar");
    private readonly Control campos;
    private bool guardando;

    internal DateTime? FechaRegistrada { get; private set; }
    private TimeOnly HoraInicio => new(hora.Value.Hour, hora.Value.Minute);

    internal FrmNuevaCita(DateTime seleccion)
    {
        DateTime ahora = DateTime.Now;
        DateTime inicial = ahora.AddMinutes(15);
        DateTime dia = seleccion.Date > ahora.Date ? seleccion.Date : inicial.Date;
        fecha.Value = dia > fecha.MaxDate ? fecha.MaxDate : dia;
        hora.Value = fecha.Value.Date > inicial.Date ? fecha.Value.Date.AddHours(9) : inicial;

        campos = DialogoCitorna.Tabla();
        campos.Controls.Add(DialogoCitorna.Campos(1, ("Cliente", clientes, 38),
            ("Empleado", empleados, 38), ("Servicio", servicios, 38)));
        campos.Controls.Add(DialogoCitorna.Campos(2, ("Fecha", fecha, 38), ("Hora de inicio", hora, 38)));
        campos.Controls.Add(duracion);
        campos.Enabled = false;
        guardar.Enabled = false;
        mensaje.Margin = new Padding(0, 12, 0, 0);
        mensaje.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Assertive;
        var contenido = DialogoCitorna.Tabla();
        contenido.Controls.Add(campos);
        contenido.Controls.Add(mensaje);
        DialogoCitorna.Configurar(this, "Nueva cita", "Selecciona los datos y el horario de la reserva.",
            new Size(510, 490), contenido, guardar, cancelar);

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
            var opciones = await Task.Run(servicioCitas.ObtenerOpcionesRegistro);
            if (IsDisposed || !Visible) return;

            clientes.DisplayMember = nameof(Cliente.NombreCompleto);
            empleados.DisplayMember = nameof(Empleado.NombreCompleto);
            servicios.DisplayMember = nameof(Servicio.Nombre);
            clientes.DataSource = opciones.Clientes;
            empleados.DataSource = opciones.Empleados;
            servicios.DataSource = opciones.Servicios;
            clientes.SelectedIndex = empleados.SelectedIndex = servicios.SelectedIndex = -1;

            var faltantes = new List<string>();
            if (opciones.Clientes.Count == 0) faltantes.Add("clientes");
            if (opciones.Empleados.Count == 0) faltantes.Add("empleados");
            if (opciones.Servicios.Count == 0) faltantes.Add("servicios");
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
            Trace.TraceWarning("Citorna, opciones de Nueva cita: {0}", ex.GetType().Name);
            if (!IsDisposed && Visible) MostrarMensaje("No se pudieron cargar las opciones. Comprueba la conexión y vuelve a abrir el formulario.");
        }
    }

    private void ActualizarResumen()
    {
        guardar.Enabled = campos.Enabled && !guardando && clientes.SelectedItem is Cliente
            && empleados.SelectedItem is Empleado && servicios.SelectedItem is Servicio;
        if (servicios.SelectedItem is not Servicio elegir)
        {
            duracion.Text = "Selecciona un servicio para ver la duración y la hora de fin.";
            return;
        }

        TimeSpan fin = HoraInicio.ToTimeSpan() + TimeSpan.FromMinutes(elegir.DuracionMinutos);
        string final = fin >= TimeSpan.FromDays(1) ? "fuera del mismo día"
            : TimeOnly.FromTimeSpan(fin).ToString("h:mm tt", CultureInfo.InvariantCulture);
        duracion.Text = $"Duración {elegir.DuracionMinutos} min · Fin {final}";
    }

    private async Task GuardarAsync()
    {
        if (guardando || !campos.Enabled || clientes.SelectedItem is not Cliente cliente
            || empleados.SelectedItem is not Empleado empleado || servicios.SelectedItem is not Servicio servicio)
            return;
        if (!SesionActual.VerificarPermiso(PermisosSistema.Agregar, "registrar citas")) return;

        var datos = new DatosCita(cliente.IdCliente, empleado.IdEmpleado, servicio.IdServicio,
            DateOnly.FromDateTime(fecha.Value), HoraInicio);
        guardando = true;
        campos.Enabled = guardar.Enabled = cancelar.Enabled = false;
        guardar.Text = "Guardando…";
        MostrarMensaje("");
        try
        {
            ResultadoCita resultado = await servicioCitas.RegistrarAsync(datos);
            if (resultado.Exitoso) FechaRegistrada = datos.Fecha.ToDateTime(TimeOnly.MinValue);
            else MostrarMensaje(resultado.Mensaje);
        }
        catch (Exception ex)
        {
            Trace.TraceWarning("Citorna, guardar nueva cita: {0}", ex.GetType().Name);
            MostrarMensaje("No se pudo confirmar el guardado. Consulta la agenda antes de volver a intentarlo.");
        }
        finally
        {
            guardando = false;
            guardar.Text = "Guardar";
            campos.Enabled = cancelar.Enabled = true;
            ActualizarResumen();
        }

        if (FechaRegistrada.HasValue) DialogResult = DialogResult.OK;
    }

    private void MostrarMensaje(string texto)
    {
        mensaje.Text = texto;
        mensaje.ForeColor = ControlesRegistro.Peligro;
        mensaje.Visible = texto.Length > 0;
    }
}
