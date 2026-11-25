using System.Globalization;

namespace SistemaGestionCitas.src.Forms;

internal sealed class SelectorFechaCitas : Panel
{
    internal DateTimePicker Fecha { get; } = new()
    {
        Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", AccessibleName = "Fecha"
    };

    private readonly FechaEnReposo vista = new();

    internal SelectorFechaCitas()
    {
        Controls.Add(Fecha);
        Controls.Add(vista);
        vista.BringToFront();
        Fecha.GotFocus += (_, _) => ActualizarVista();
        Fecha.LostFocus += (_, _) => ActualizarVista();
        Fecha.ValueChanged += (_, _) => ActualizarVista();
        Fecha.KeyDown += (_, e) => OnKeyDown(e);
        ActualizarVista();
    }

    public override Size GetPreferredSize(Size proposedSize) => Fecha.PreferredSize;

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        int alto = Fecha.PreferredHeight;
        Fecha.SetBounds(0, (Height - alto) / 2, Width, alto);
        vista.Bounds = Fecha.Bounds;
    }

    internal void AplicarPaleta(PaletaCitorna paleta)
    {
        vista.Paleta = paleta;
        ActualizarVista();
    }

    private void ActualizarVista()
    {
        vista.Text = Fecha.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        vista.Visible = vista.Paleta.EsOscura && !Fecha.Focused;
        vista.Invalidate();
    }

    private sealed class FechaEnReposo : Control
    {
        internal PaletaCitorna Paleta { get; set; } = PaletaCitorna.Clara;

        internal FechaEnReposo()
        {
            TabStop = false;
            AccessibleRole = AccessibleRole.None;
            SetStyle(ControlStyles.Selectable, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Paleta.Superficie);
            int margen = LogicalToDeviceUnits(5);
            int lado = LogicalToDeviceUnits(16);
            int icono = Width - lado - margen;
            TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(margen, 0, Math.Max(1, icono - margen), Height),
                Paleta.Tinta, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            IconosAgenda.Dibujar(e.Graphics, SimboloAgenda.Calendario, Paleta.Acento, new(icono, (Height - lado) / 2, lado, lado));
        }

        protected override void WndProc(ref Message m)
        {
            const int WmNcHitTest = 0x0084;
            // Windows entrega los clics al selector situado debajo de esta vista.
            if (m.Msg == WmNcHitTest) m.Result = new IntPtr(-1);
            else base.WndProc(ref m);
        }
    }
}
