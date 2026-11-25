using System.Globalization;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;
using static SistemaGestionCitas.src.Models.NombresEstadoCita;

namespace SistemaGestionCitas.src.Forms;

internal sealed class FrmCitas : Form
{
    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-DO");
    private static readonly ToolStripRenderer MenuOscuro = new ToolStripProfessionalRenderer(new ColoresMenuOscuro());
    private readonly CitaService servicio = new();
    private readonly CheckBox modoOscuro = PaletaCitorna.CrearInterruptor();
    private readonly SelectorFechaCitas selectorFecha = new();
    private DateTimePicker fecha => selectorFecha.Fecha;
    private readonly ContextMenuStrip estados = new();
    private readonly VistaRegistros<Cita> vista;
    private readonly Action actualizarAgenda;
    internal event Action<bool>? TemaCambiado;

    internal FrmCitas(DateTime fechaInicial, Action actualizarAgenda)
    {
        this.actualizarAgenda = actualizarAgenda;
        SuspendLayout();
        Text = "Citas";
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        FormBorderStyle = FormBorderStyle.None;
        ClientSize = new Size(1040, 650);
        fecha.Value = DentroDelSelector(fechaInicial);

        vista = new("Citas", "Consulta y gestión de reservas", "Fecha", "cita",
            cita => cita.IdCita, campoConsulta: selectorFecha, femenino: true, accionCabecera: modoOscuro);
        modoOscuro.CheckedChanged += (_, _) => TemaCambiado?.Invoke(modoOscuro.Checked);
        vista.Tabla.AgregarColumna("Hora", 15, 72);
        vista.Tabla.AgregarColumna("Cliente", 23, 86);
        vista.Tabla.AgregarColumna("Servicio", 22, 82);
        vista.Tabla.AgregarColumna("Empleado", 21, 88);
        vista.Tabla.AgregarColumna("Estado", 19, 92);
        vista.ValorCelda = (cita, columna) => columna switch
        {
            0 => MostrarHora(cita.HoraInicio),
            1 => cita.Cliente,
            2 => cita.Servicio,
            3 => cita.Empleado,
            4 => cita.Estado,
            _ => null
        };
        vista.MostrarSeleccion = MostrarFicha;
        vista.Consultar += () => Consultar();
        vista.Editar += EditarCita;
        vista.ConfigurarAccionFicha("Cambiar estado", cita => CitaService.Editable(cita.Estado), MostrarAcciones);
        AgregarOpcion("Confirmar", Confirmada, servicio.ConfirmarAsync);
        AgregarOpcion("Completar", Completada, servicio.CompletarAsync);
        AgregarOpcion("Cancelar cita", Cancelada, servicio.CancelarAsync);
        AgregarOpcion("Marcar inasistencia", NoAsistio, servicio.MarcarInasistenciaAsync);
        fecha.ValueChanged += (_, _) => Consultar(sinSeleccion: true);
        Controls.Add(vista);
        vista.ActualizarPermisos();
        Shown += (_, _) =>
        {
            vista.Tabla.RowTemplate.Height = LogicalToDeviceUnits(43);
            Consultar();
        };
        Disposed += (_, _) => estados.Dispose();
        ResumeLayout(true);
    }

    internal void AplicarPaleta(PaletaCitorna paleta)
    {
        modoOscuro.Checked = paleta.EsOscura;
        vista.AplicarPaleta(paleta);
        selectorFecha.AplicarPaleta(paleta);
        if (paleta.EsOscura) estados.Renderer = MenuOscuro;
        else estados.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        foreach (ToolStripItem item in estados.Items)
            item.ForeColor = paleta.EsOscura ? paleta.Tinta : Color.Empty;
    }

    private DateTime DentroDelSelector(DateTime dia) =>
        dia.Date < fecha.MinDate ? fecha.MinDate : dia.Date > fecha.MaxDate ? fecha.MaxDate : dia.Date;

    private void Consultar(int? idCita = null, bool sinSeleccion = false)
    {
        DateOnly dia = DateOnly.FromDateTime(fecha.Value);
        vista.CargarAsync(async () => ((await servicio.ConsultarPorFechaAsync(dia)).ToList(), null),
            filtrada: false, id: idCita, sinSeleccion: sinSeleccion);
    }

    private static string MostrarHora(TimeOnly hora) => hora.ToString("h:mm tt", CultureInfo.InvariantCulture);

    private void MostrarFicha(Cita? cita)
    {
        if (cita == null)
        {
            vista.PresentarFicha("Selecciona una cita para ver sus datos");
            return;
        }

        var duracion = cita.HoraFin.ToTimeSpan() - cita.HoraInicio.ToTimeSpan();
        vista.PresentarFicha(cita.Cliente,
            ("Fecha", cita.Fecha.ToString("d 'de' MMMM 'de' yyyy", Espanol)),
            ("Horario", $"{MostrarHora(cita.HoraInicio)} – {MostrarHora(cita.HoraFin)}"),
            ("Duración", $"{duracion.TotalMinutes:0.##} min"),
            ("Servicio", cita.Servicio),
            ("Empleado", cita.Empleado),
            ("Estado", cita.Estado),
            ("Observaciones", ControlesRegistro.Disponible(cita.Observaciones)));
    }

    private void EditarCita(Cita cita)
    {
        if (!PuedeAdministrar() || !CitaService.Editable(cita.Estado)) return;
        DateTime fechaOriginal = cita.Fecha.ToDateTime(TimeOnly.MinValue);
        if (fechaOriginal != DentroDelSelector(fechaOriginal))
        {
            MessageBox.Show(this, "El selector de fechas no puede mostrar la fecha de esta cita.",
                "Editar cita", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        using var editor = new FrmNuevaCita(fechaOriginal, cita);
        if (editor.ShowDialog(this) != DialogResult.OK || editor.FechaGuardada is not DateTime dia) return;

        fecha.Value = DentroDelSelector(dia);
        Consultar(cita.IdCita);
        actualizarAgenda();
    }

    private void AgregarOpcion(string titulo, string destino, Func<int, CancellationToken, Task<ResultadoCita>> cambiar)
    {
        var opcion = new ToolStripMenuItem(titulo) { Tag = destino };
        opcion.Click += async (_, _) =>
        {
            if (estados.Tag is not Cita cita || !PuedeAdministrar() || !CitaService.PermiteTransicion(cita.Estado, destino))
                return;
            if (destino != Confirmada && MessageBox.Show(this,
                    $"¿Cambiar a «{destino}» la cita de {cita.Cliente}? El estado es definitivo.",
                    titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            await vista.EjecutarOperacionAsync(titulo, async () =>
            {
                ResultadoCita resultado = await cambiar(cita.IdCita, CancellationToken.None);
                return new ResultadoOperacion { Exitoso = resultado.Exitoso, Mensaje = resultado.Mensaje };
            }, resultado =>
            {
                Consultar(cita.IdCita);
                if (resultado.Exitoso) actualizarAgenda();
                else MessageBox.Show(this, resultado.Mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            });
        };
        estados.Items.Add(opcion);
    }

    private void MostrarAcciones(Button boton, Cita cita)
    {
        if (!PuedeAdministrar()) return;
        estados.Tag = cita;
        foreach (ToolStripItem item in estados.Items)
            item.Visible = item.Tag is string destino && CitaService.PermiteTransicion(cita.Estado, destino);
        estados.Show(boton, new Point(0, boton.Height));
    }

    private static bool PuedeAdministrar() => SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar citas")
        && SesionActual.VerificarPermiso(PermisosSistema.Modificar, "administrar citas");

    private sealed class ColoresMenuOscuro : ProfessionalColorTable
    {
        private readonly PaletaCitorna paleta = PaletaCitorna.Oscura;

        public override Color ToolStripDropDownBackground => paleta.Superficie;
        public override Color ImageMarginGradientBegin => paleta.Superficie;
        public override Color ImageMarginGradientMiddle => paleta.Superficie;
        public override Color ImageMarginGradientEnd => paleta.Superficie;
        public override Color MenuItemSelected => paleta.Seleccion;
        public override Color MenuItemBorder => paleta.Acento;
        public override Color MenuBorder => paleta.Borde;
    }
}
