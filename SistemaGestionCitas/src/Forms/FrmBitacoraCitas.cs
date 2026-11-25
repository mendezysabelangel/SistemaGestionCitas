using System.Diagnostics;
using System.Globalization;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

internal sealed class FrmBitacoraCitas : Form
{
    private readonly BitacoraCitasService servicio = new();
    private readonly CancellationTokenSource cierre = new();
    private readonly RejillaRegistros tabla = new() { Dock = DockStyle.Fill, AccessibleName = "Actividad de Citas" };
    private readonly Label estado = CampoCitorna.Rotulo("", true);
    private readonly Button actualizar = ControlesRegistro.Boton("Actualizar");
    private IReadOnlyList<EventoBitacoraCita> eventos = [];
    private PaletaCitorna paleta = PaletaCitorna.Clara;

    public FrmBitacoraCitas()
    {
        Text = "Bitácora de Citas";
        FormBorderStyle = FormBorderStyle.None;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96, 96);
        ClientSize = new Size(1040, 650);
        DoubleBuffered = true;
        var fuente = new Font("Segoe UI", 11);
        var fuenteTitulo = new Font("Segoe UI", 23, FontStyle.Bold);
        Font = fuente;

        var pagina = DialogoCitorna.Tabla();
        pagina.AutoSize = false;
        pagina.Dock = DockStyle.Fill;
        pagina.BackColor = Color.Transparent;
        pagina.Padding = new Padding(23, 20, 23, 18);
        pagina.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pagina.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Label titulo = CampoCitorna.Rotulo("Bitácora de Citas");
        titulo.Font = fuenteTitulo;
        titulo.Margin = new Padding(0, 0, 0, 5);

        var encabezado = DialogoCitorna.Tabla();
        encabezado.Controls.Add(titulo);
        encabezado.Controls.Add(CampoCitorna.Rotulo($"Hasta {BitacoraCitasService.LimiteEventos} eventos recientes", true));
        actualizar.Dock = DockStyle.None;
        actualizar.Anchor = AnchorStyles.Left;
        actualizar.Margin = new Padding(0, 10, 0, 12);
        encabezado.Controls.Add(actualizar);
        pagina.Controls.Add(encabezado, 0, 0);

        var tarjeta = new TarjetaAgenda { Dock = DockStyle.Fill, Padding = new Padding(12), Margin = Padding.Empty };
        var contenido = DialogoCitorna.Tabla();
        contenido.AutoSize = false;
        contenido.Dock = DockStyle.Fill;
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tabla.AgregarColumna("Fecha y hora", 23, 130);
        tabla.AgregarColumna("Usuario", 19, 100);
        tabla.AgregarColumna("Acción", 23, 125);
        tabla.AgregarColumna("Cita", 9, 57);
        tabla.AgregarColumna("Estado", 26, 160);
        tabla.CellValueNeeded += MostrarCelda;
        contenido.Controls.Add(tabla, 0, 0);
        estado.Margin = new Padding(0, 8, 0, 0);
        estado.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
        contenido.Controls.Add(estado, 0, 1);
        tarjeta.Controls.Add(contenido);
        pagina.Controls.Add(tarjeta, 0, 1);
        Controls.Add(pagina);

        Activated += async (_, _) => await ConsultarAsync();
        actualizar.Click += async (_, _) => await ConsultarAsync();
        Shown += (_, _) => tabla.RowTemplate.Height = LogicalToDeviceUnits(38);
        Disposed += (_, _) =>
        {
            cierre.Cancel();
            cierre.Dispose();
            fuenteTitulo.Dispose();
            fuente.Dispose();
        };
    }

    internal void AplicarPaleta(PaletaCitorna nueva)
    {
        if (paleta == nueva) return;
        paleta = nueva;
        BackColor = nueva.Fondo;
        nueva.AplicarControles(this);
        Invalidate(true);
    }

    protected override void OnPaintBackground(PaintEventArgs e) => FondoInterfaces.Pintar(this, e, paleta);

    private void MostrarCelda(object? sender, DataGridViewCellValueEventArgs e)
    {
        if ((uint)e.RowIndex >= (uint)eventos.Count) return;
        EventoBitacoraCita evento = eventos[e.RowIndex];
        e.Value = e.ColumnIndex switch
        {
            0 => evento.FechaEvento.ToString("dd/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
            1 => evento.NombreUsuario,
            2 => evento.Accion,
            3 => evento.IdCita,
            4 => evento.EstadoAnterior == null ? evento.EstadoNuevo : $"{evento.EstadoAnterior} → {evento.EstadoNuevo}",
            _ => null
        };
    }

    private async Task ConsultarAsync()
    {
        if (!actualizar.Enabled) return;
        actualizar.Enabled = false;
        estado.Text = "Cargando actividad…";
        CancellationToken cancelacion = cierre.Token;
        try
        {
            var resultado = await servicio.ConsultarAsync(cancelacion);
            if (IsDisposed || cancelacion.IsCancellationRequested) return;
            if (!BitacoraCitasService.PuedeConsultar())
                throw new UnauthorizedAccessException("No tiene permiso para consultar la bitácora de Citas.");
            eventos = resultado;
            tabla.RowCount = eventos.Count;
            tabla.Invalidate();
            estado.Text = eventos.Count == 0 ? "Aún no hay eventos registrados." : $"Eventos mostrados: {eventos.Count}";
        }
        catch (OperationCanceledException) when (cancelacion.IsCancellationRequested) { }
        catch (UnauthorizedAccessException ex)
        {
            if (IsDisposed) return;
            eventos = [];
            tabla.RowCount = 0;
            estado.Text = ex.Message;
        }
        catch (SqlException ex) when (ex.Number == 208)
        {
            if (!IsDisposed) estado.Text = BitacoraCitasRepository.MensajeInstalacion;
        }
        catch (Exception ex)
        {
            Trace.TraceWarning("Citorna, consulta de bitácora: {0}", ex.GetType().Name);
            if (!IsDisposed) estado.Text = "No se pudo cargar la actividad. Vuelve a intentarlo.";
        }
        finally
        {
            if (!IsDisposed) actualizar.Enabled = true;
        }
    }
}
