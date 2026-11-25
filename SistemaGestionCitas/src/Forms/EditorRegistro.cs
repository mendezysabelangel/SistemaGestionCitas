using System.Diagnostics;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

internal sealed class EditorRegistro : Form
{
    private readonly TableLayoutPanel campos = new();
    private readonly Label mensaje = new();
    private readonly Button guardar;
    private readonly Button cancelar;
    private readonly Font fuente = new("Segoe UI", 11F);
    private readonly string permiso;
    private readonly string? permisoAcceso;
    private bool guardando;
    internal Func<Func<ResultadoOperacion>?>? PrepararGuardado { get; set; }

    internal EditorRegistro(string titulo, string permiso, bool edicion, string? permisoAcceso = null)
    {
        this.permiso = permiso;
        this.permisoAcceso = permisoAcceso;
        SuspendLayout();
        Text = titulo;
        Font = fuente;
        ForeColor = EstiloAgenda.Tinta;
        BackColor = Color.White;
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        MinimizeBox = false;
        MaximizeBox = false;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        MinimumSize = new Size(330, 315);
        ClientSize = new Size(490, 515);

        var distribucion = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20, 15, 20, 14),
            ColumnCount = 1, RowCount = 3 };
        distribucion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        distribucion.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        distribucion.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        distribucion.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        campos.Dock = DockStyle.Top;
        campos.ColumnCount = 1;
        campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        campos.AutoSize = true;
        campos.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        campos.Margin = Padding.Empty;
        scroll.Controls.Add(campos);
        distribucion.Controls.Add(scroll, 0, 0);

        mensaje.AutoSize = true;
        mensaje.Dock = DockStyle.Fill;
        mensaje.ForeColor = ControlesRegistro.Peligro;
        mensaje.Margin = new Padding(0, 5, 0, 5);
        mensaje.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Assertive;
        mensaje.Visible = false;
        distribucion.Controls.Add(mensaje, 0, 1);

        var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false, Margin = Padding.Empty };
        guardar = ControlesRegistro.Boton(edicion ? "Guardar cambios" : "Guardar", ControlesRegistro.Primario, Color.White);
        cancelar = ControlesRegistro.Boton("Cancelar");
        cancelar.DialogResult = DialogResult.Cancel;
        guardar.Click += (_, _) => GuardarAsync();
        acciones.Controls.Add(guardar);
        acciones.Controls.Add(cancelar);
        distribucion.Controls.Add(acciones, 0, 2);
        Controls.Add(distribucion);
        AcceptButton = guardar;
        CancelButton = cancelar;
        FormClosing += (_, e) => { if (guardando) e.Cancel = true; };
        ResumeLayout(true);
    }

    internal void AgregarCampo(string nombre, Control control, int alto = 38)
    {
        var etiqueta = new Label { Text = nombre, AutoSize = true, Dock = DockStyle.Fill, Margin = Padding.Empty };
        Control entrada = control is CheckBox ? control : CampoCitorna.Enmarcar(control, nombre, alto);
        if (control is CheckBox) control.AccessibleName = nombre;
        entrada.Dock = DockStyle.Top;
        entrada.Margin = new Padding(0, 2, 5, 7);
        int fila = campos.RowCount;
        campos.RowCount += 2;
        campos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        campos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        campos.Controls.Add(etiqueta, 0, fila);
        campos.Controls.Add(entrada, 0, fila + 1);
    }

    internal Func<ResultadoOperacion>? Advertir(string texto, Control campo)
    {
        MostrarMensaje(texto);
        campo.Focus();
        return null;
    }

    private void MostrarMensaje(string texto)
    {
        mensaje.Text = texto;
        mensaje.Visible = texto.Length != 0;
    }

    private async void GuardarAsync()
    {
        if (guardando || (permisoAcceso != null && !SesionActual.VerificarPermiso(permisoAcceso, "guardar este registro"))
            || !SesionActual.VerificarPermiso(permiso, "guardar este registro")) return;
        MostrarMensaje("");
        Func<ResultadoOperacion>? guardarRegistro = PrepararGuardado?.Invoke();
        if (guardarRegistro == null) return;

        guardando = true;
        campos.Enabled = guardar.Enabled = cancelar.Enabled = false;
        guardar.Text = "Guardando…";
        string? resultadoError = null;
        try
        {
            ResultadoOperacion resultado = await Task.Run(guardarRegistro);
            if (!resultado.Exitoso) resultadoError = resultado.Mensaje;
        }
        catch (Exception ex)
        {
            Trace.TraceWarning("Citorna, guardar registro: {0}", ex.GetType().Name);
            resultadoError = "No se pudo guardar el registro. Compruebe la conexión y vuelva a intentarlo.";
        }
        finally
        {
            guardando = false;
            guardar.Text = permiso == PermisosSistema.Modificar ? "Guardar cambios" : "Guardar";
            campos.Enabled = guardar.Enabled = cancelar.Enabled = true;
        }
        if (resultadoError != null)
        {
            MostrarMensaje(resultadoError);
            return;
        }
        DialogResult = DialogResult.OK;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) fuente.Dispose();
    }
}
