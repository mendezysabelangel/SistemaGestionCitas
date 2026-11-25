using System.Drawing.Drawing2D;

namespace SistemaGestionCitas.src.Forms;

internal interface IVistaConPaleta
{
    void AplicarPaleta(PaletaCitorna paleta);
}

internal sealed class PaletaCitorna
{
    internal static readonly PaletaCitorna Clara = new(false);
    internal static readonly PaletaCitorna Oscura = new(true);

    internal bool EsOscura { get; }
    internal Color Fondo => EsOscura ? Color.FromArgb(23, 24, 32) : EstiloAgenda.Fondo;
    internal Color Barra => EsOscura ? Color.FromArgb(27, 28, 38) : EstiloAgenda.Barra;
    internal Color Superficie => EsOscura ? Color.FromArgb(35, 38, 48) : Color.White;
    internal Color Encabezado => EsOscura ? Color.FromArgb(45, 45, 63) : EstiloAgenda.Fondo;
    internal Color Tinta => EsOscura ? Color.FromArgb(242, 241, 250) : EstiloAgenda.Tinta;
    internal Color Secundario => EsOscura ? Color.FromArgb(184, 189, 210) : EstiloAgenda.Secundario;
    internal Color Borde => EsOscura ? Color.FromArgb(68, 72, 93) : EstiloAgenda.Borde;
    internal Color Acento => EsOscura ? Color.FromArgb(184, 175, 255) : EstiloAgenda.Azul;
    internal Color Seleccion => EsOscura ? Color.FromArgb(58, 52, 89) : EstiloAgenda.Seleccion;
    internal Color Principal => EsOscura ? Color.FromArgb(94, 75, 181) : ControlesRegistro.Primario;
    internal Color DiaSeleccionado => EsOscura ? Principal : EstiloAgenda.Azul;
    internal Color DiaFueraDeMes => EsOscura ? Fondo : Color.FromArgb(248, 251, 255);
    internal Color SobreMenu => EsOscura ? Encabezado : Color.FromArgb(239, 246, 254);
    internal Color OndaClara => EsOscura ? Color.FromArgb(95, 79, 161) : Color.FromArgb(219, 235, 253);
    internal Color OndaSuave => EsOscura ? Color.FromArgb(155, 164, 139, 238) : Color.FromArgb(124, 173, 225, 245);
    internal Color Error => EsOscura ? Color.FromArgb(255, 169, 181) : ControlesRegistro.Peligro;
    internal Color TextoActivo => EsOscura ? Color.FromArgb(169, 236, 211) : Color.FromArgb(0, 90, 73);

    private PaletaCitorna(bool oscuro) => EsOscura = oscuro;

    internal static CheckBox CrearInterruptor() => new InterruptorTema
    {
        Text = "Modo oscuro", AutoSize = true, Anchor = AnchorStyles.Right,
        ForeColor = EstiloAgenda.Tinta, Margin = new Padding(12, 0, 0, 0),
        Cursor = Cursors.Hand
    };

    internal void AplicarControles(Control raiz)
    {
        if (raiz is ScrollableControl { AutoScroll: true })
            TemaNativoCitorna.Aplicar(raiz, EsOscura);

        foreach (Control control in raiz.Controls)
        {
            switch (control)
            {
                case IVistaConPaleta vista:
                    vista.AplicarPaleta(this);
                    continue;
                case TarjetaAgenda tarjeta:
                    tarjeta.Paleta = this;
                    break;
                case BotonCitorna boton:
                    boton.Paleta = this;
                    break;
                case DataGridView tabla:
                    AplicarTabla(tabla);
                    continue;
                case Label etiqueta:
                    Color color = etiqueta.ForeColor;
                    if (color == Clara.Error || color == Oscura.Error) etiqueta.ForeColor = Error;
                    else if (color == Clara.Secundario || color == Oscura.Secundario) etiqueta.ForeColor = Secundario;
                    else etiqueta.ForeColor = Tinta;
                    break;
                case InterruptorTema interruptor:
                    interruptor.Paleta = this;
                    break;
                case ComboBox combo:
                    AplicarCombo(combo);
                    break;
                case TextBoxBase texto:
                    control.BackColor = Superficie;
                    control.ForeColor = Tinta;
                    if (texto.Multiline) TemaNativoCitorna.Aplicar(texto, EsOscura);
                    break;
                case CheckBox or GroupBox:
                    control.ForeColor = Tinta;
                    break;
            }
            AplicarControles(control);
        }
    }

    internal void AplicarDialogo(Form formulario)
    {
        if (!EsOscura) return;
        formulario.BackColor = Fondo;
        formulario.ForeColor = Tinta;
        AplicarControles(formulario);
    }

    private void AplicarCombo(ComboBox combo)
    {
        combo.BackColor = Superficie;
        combo.ForeColor = Tinta;
        if (combo.DropDownStyle != ComboBoxStyle.DropDownList) return;

        combo.DrawItem -= DibujarOpcionOscura;
        combo.DrawMode = EsOscura ? DrawMode.OwnerDrawFixed : DrawMode.Normal;
        if (EsOscura)
        {
            combo.ItemHeight = TextRenderer.MeasureText("Ag", combo.Font).Height + 2;
            combo.DrawItem += DibujarOpcionOscura;
        }
        TemaNativoCitorna.Aplicar(combo, EsOscura);
    }

    private static void DibujarOpcionOscura(object? sender, DrawItemEventArgs e)
    {
        if (sender is not ComboBox combo) return;
        bool seleccionada = (e.State & DrawItemState.Selected) != 0 && (e.State & DrawItemState.ComboBoxEdit) == 0;
        using var fondo = new SolidBrush(seleccionada ? Oscura.Seleccion : Oscura.Superficie);
        e.Graphics.FillRectangle(fondo, e.Bounds);
        if (e.Index < 0 || e.Index >= combo.Items.Count) return;
        Rectangle texto = Rectangle.Inflate(e.Bounds, -3, 0);
        TextRenderer.DrawText(e.Graphics, combo.GetItemText(combo.Items[e.Index]), combo.Font, texto,
            combo.Enabled ? Oscura.Tinta : Oscura.Secundario,
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        if ((e.State & DrawItemState.Focus) != 0)
            ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, Oscura.Acento, Oscura.Superficie);
    }

    private void AplicarTabla(DataGridView tabla)
    {
        foreach (var barra in tabla.Controls.OfType<ScrollBar>())
            TemaNativoCitorna.Aplicar(barra, EsOscura);

        tabla.BackgroundColor = Superficie;
        tabla.GridColor = Borde;
        tabla.ColumnHeadersBorderStyle = EsOscura ? DataGridViewHeaderBorderStyle.None : DataGridViewHeaderBorderStyle.Raised;
        tabla.DefaultCellStyle.BackColor = Superficie;
        tabla.DefaultCellStyle.ForeColor = Tinta;
        tabla.DefaultCellStyle.SelectionBackColor = Seleccion;
        tabla.DefaultCellStyle.SelectionForeColor = Tinta;
        tabla.ColumnHeadersDefaultCellStyle.BackColor = Encabezado;
        tabla.ColumnHeadersDefaultCellStyle.ForeColor = Tinta;
        tabla.ColumnHeadersDefaultCellStyle.SelectionBackColor = Encabezado;
        tabla.ColumnHeadersDefaultCellStyle.SelectionForeColor = Tinta;
        tabla.Invalidate();
    }
}

internal sealed class InterruptorTema : CheckBox
{
    private PaletaCitorna paleta = PaletaCitorna.Clara;
    internal PaletaCitorna Paleta
    {
        set { paleta = value; Invalidate(); }
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        Size texto = TextRenderer.MeasureText(Text, Font, Size.Empty, TextFormatFlags.NoPadding);
        return new Size(texto.Width + LogicalToDeviceUnits(46), Math.Max(texto.Height, LogicalToDeviceUnits(22)));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
        int ancho = LogicalToDeviceUnits(32);
        int alto = LogicalToDeviceUnits(16);
        int margen = LogicalToDeviceUnits(2);
        int y = (Height - alto) / 2;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using GraphicsPath pista = EstiloAgenda.RectanguloRedondo(new Rectangle(margen, y, ancho, alto), alto / 2);
        using var fondo = new SolidBrush(Checked ? paleta.Principal : paleta.Borde);
        e.Graphics.FillPath(fondo, pista);
        using var punto = new SolidBrush(Checked ? Color.White : paleta.EsOscura ? paleta.Tinta : paleta.Secundario);
        int diametro = alto - margen * 2;
        int x = Checked ? ancho - diametro : margen * 2;
        e.Graphics.FillEllipse(punto, x, y + margen, diametro, diametro);

        int inicio = ancho + LogicalToDeviceUnits(10);
        TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(inicio, 0, Math.Max(1, Width - inicio), Height),
            paleta.Tinta, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        if (Focused && ShowFocusCues) ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle, paleta.Acento, paleta.Fondo);
    }

    protected override void OnCheckedChanged(EventArgs e)
    {
        base.OnCheckedChanged(e);
        Invalidate();
    }
}
