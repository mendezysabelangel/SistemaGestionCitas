namespace SistemaGestionCitas.src.Forms;

internal sealed class VistaSelectorCitorna : Control, IVistaConPaleta
{
    private readonly ComboBox selector;
    private PaletaCitorna paleta = PaletaCitorna.Clara;

    internal VistaSelectorCitorna(ComboBox selector)
    {
        this.selector = selector;
        Visible = false;
        TabStop = false;
        AccessibleRole = AccessibleRole.None;
        SetStyle(ControlStyles.Selectable, false);
        DoubleBuffered = true;
        selector.TextChanged += (_, _) => Invalidate();
        selector.SelectedIndexChanged += (_, _) => Invalidate();
        selector.EnabledChanged += (_, _) => Invalidate();
    }

    public void AplicarPaleta(PaletaCitorna nueva)
    {
        paleta = nueva;
        Visible = nueva.EsOscura && !SystemInformation.HighContrast;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(paleta.Superficie);
        int margen = LogicalToDeviceUnits(4);
        int flecha = LogicalToDeviceUnits(5);
        int x = Width - LogicalToDeviceUnits(12);
        int y = Height / 2;
        Color tinta = selector.Enabled ? paleta.Tinta : paleta.Secundario;
        var zona = new Rectangle(margen, 0, Math.Max(1, x - flecha - 2 * margen), Height);
        TextRenderer.DrawText(e.Graphics, selector.Text, selector.Font, zona, tinta,
            TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        using var linea = new Pen(selector.Enabled ? paleta.Acento : paleta.Secundario, LogicalToDeviceUnits(1));
        e.Graphics.DrawLines(linea, new Point[] { new(x - flecha, y - flecha / 2), new(x, y + flecha / 2), new(x + flecha, y - flecha / 2) });
    }

    protected override void WndProc(ref Message m)
    {
        const int WmNcHitTest = 0x0084;
        // El ComboBox sigue recibiendo los clics, el foco y las teclas.
        if (m.Msg == WmNcHitTest) m.Result = new IntPtr(-1);
        else base.WndProc(ref m);
    }
}
