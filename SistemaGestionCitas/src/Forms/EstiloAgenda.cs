using System.Drawing.Drawing2D;

namespace SistemaGestionCitas.src.Forms;

internal static class EstiloAgenda
{
    internal static readonly Color Fondo = Color.FromArgb(232, 241, 252);
    internal static readonly Color Barra = Color.FromArgb(247, 250, 254);
    internal static readonly Color Tinta = Color.FromArgb(8, 45, 80);
    internal static readonly Color Secundario = Color.FromArgb(72, 94, 119);
    internal static readonly Color Borde = Color.FromArgb(208, 222, 237);
    internal static readonly Color Azul = Color.FromArgb(20, 84, 179);
    internal static readonly Color Seleccion = Color.FromArgb(218, 234, 252);
    internal static readonly Color Turquesa = Color.FromArgb(0, 128, 119);

    internal static GraphicsPath RectanguloRedondo(RectangleF rect, int radio)
    {
        float d = Math.Min(radio * 2, Math.Min(rect.Width, rect.Height));
        GraphicsPath forma = new();
        forma.AddArc(rect.Left, rect.Top, d, d, 180, 90);
        forma.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
        forma.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        forma.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
        forma.CloseFigure();
        return forma;
    }
}

internal sealed class TarjetaAgenda : Panel
{
    internal int Radio { get; init; } = 15;
    internal bool ResaltarFoco { get; init; }
    public TarjetaAgenda()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.Transparent;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        int radio = LogicalToDeviceUnits(Radio);
        // Un hijo que sólo cubre el interior blanco no necesita volver a pintar la imagen de fondo.
        if (Width > 2 * radio + 4 && Height > 2 * radio + 4
            && (new Rectangle(radio + 2, 3, Width - 2 * radio - 4, Height - 6).Contains(e.ClipRectangle)
                || new Rectangle(3, radio + 2, Width - 6, Height - 2 * radio - 4).Contains(e.ClipRectangle)))
        {
            e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
            return;
        }

        base.OnPaintBackground(e);
        if (Width < 3 || Height < 3) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using GraphicsPath forma = EstiloAgenda.RectanguloRedondo(new RectangleF(1, 1, Width - 3, Height - 3), radio);
        bool enfocada = ResaltarFoco && ContainsFocus;
        using Pen borde = new(enfocada ? EstiloAgenda.Azul : EstiloAgenda.Borde,
            enfocada ? LogicalToDeviceUnits(2) : 1);
        e.Graphics.FillPath(Brushes.White, forma);
        e.Graphics.DrawPath(borde, forma);
    }
}

internal sealed class OndasMenu : Control
{
    public OndasMenu()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        TabStop = false;
        BackColor = EstiloAgenda.Barra;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using GraphicsPath clara = new();
        using GraphicsPath oscura = new();
        clara.AddBezier(0, Height * .75F, Width * .40F, Height * 1.12F,
            Width * .64F, -Height * .15F, Width, Height * .13F);
        clara.AddLine(Width, Height, 0, Height);
        clara.CloseFigure();
        oscura.AddBezier(0, Height * .33F, Width * .38F, 0, Width * .46F, Height * 1.25F, Width, Height * .48F);
        oscura.AddLine(Width, Height, 0, Height);
        oscura.CloseFigure();
        using SolidBrush azulClaro = new(Color.FromArgb(219, 235, 253));
        using SolidBrush azulSuave = new(Color.FromArgb(124, 173, 225, 245));
        e.Graphics.FillPath(azulClaro, clara);
        e.Graphics.FillPath(azulSuave, oscura);
    }
}

internal enum SimboloAgenda { Inicio, Clientes, Productos, Usuarios, UsuarioNuevo, Rol, Salir, Anterior, Siguiente, Calendario, Reloj, Confirmada }

internal static class IconosAgenda
{
    internal static void Dibujar(Graphics dibujo, SimboloAgenda simbolo, Color color, Rectangle zona)
    {
        var estado = dibujo.Save();
        dibujo.SmoothingMode = SmoothingMode.AntiAlias;
        dibujo.TranslateTransform(zona.X, zona.Y);
        dibujo.ScaleTransform(zona.Width / 24F, zona.Height / 24F);
        using Pen linea = new(color, 1.65F) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        switch (simbolo)
        {
            case SimboloAgenda.Inicio:
                dibujo.DrawLines(linea, (Point[])[new(3, 11), new(12, 3), new(21, 11), new(21, 21),
                    new(15, 21), new(15, 14), new(9, 14), new(9, 21), new(3, 21), new(3, 11)]);
                break;
            case SimboloAgenda.Clientes:
            case SimboloAgenda.Usuarios:
            case SimboloAgenda.UsuarioNuevo:
                dibujo.DrawEllipse(linea, 5, 3, 8, 8);
                dibujo.DrawArc(linea, 2, 13, 14, 13, 180, 180);
                if (simbolo == SimboloAgenda.UsuarioNuevo)
                {
                    dibujo.DrawLine(linea, 18, 5, 18, 13);
                    dibujo.DrawLine(linea, 14, 9, 22, 9);
                }
                else
                {
                    dibujo.DrawArc(linea, 13, 5, 7, 7, 275, 125);
                    dibujo.DrawArc(linea, 12, 13, 10, 12, 275, 85);
                }
                break;
            case SimboloAgenda.Productos:
                dibujo.DrawPolygon(linea, (Point[])[new(12, 2), new(21, 7), new(21, 17), new(12, 22), new(3, 17), new(3, 7)]);
                dibujo.DrawLines(linea, (Point[])[new(3, 7), new(12, 12), new(21, 7)]);
                dibujo.DrawLine(linea, 12, 12, 12, 22);
                break;
            case SimboloAgenda.Rol:
                dibujo.DrawLines(linea, (Point[])[new(12, 2), new(20, 5), new(20, 13), new(17, 18),
                    new(12, 22), new(7, 18), new(4, 13), new(4, 5), new(12, 2)]);
                dibujo.DrawLines(linea, (Point[])[new(8, 12), new(11, 15), new(16, 9)]);
                break;
            case SimboloAgenda.Salir:
                dibujo.DrawLines(linea, (Point[])[new(11, 3), new(3, 3), new(3, 21), new(11, 21)]);
                dibujo.DrawLine(linea, 8, 12, 21, 12);
                dibujo.DrawLines(linea, (Point[])[new(16, 7), new(21, 12), new(16, 17)]);
                break;
            case SimboloAgenda.Anterior:
                dibujo.DrawLines(linea, (Point[])[new(15, 5), new(8, 12), new(15, 19)]);
                break;
            case SimboloAgenda.Siguiente:
                dibujo.DrawLines(linea, (Point[])[new(9, 5), new(16, 12), new(9, 19)]);
                break;
            case SimboloAgenda.Calendario:
                dibujo.DrawRectangle(linea, 3, 5, 18, 16);
                dibujo.DrawLine(linea, 3, 10, 21, 10);
                dibujo.DrawLine(linea, 8, 3, 8, 7);
                dibujo.DrawLine(linea, 16, 3, 16, 7);
                break;
            case SimboloAgenda.Reloj:
                dibujo.DrawEllipse(linea, 2, 2, 20, 20);
                dibujo.DrawLines(linea, (Point[])[new(12, 6), new(12, 12), new(16, 14)]);
                break;
            case SimboloAgenda.Confirmada:
                dibujo.DrawEllipse(linea, 2, 2, 20, 20);
                dibujo.DrawLines(linea, (Point[])[new(7, 12), new(11, 16), new(17, 8)]);
                break;
        }
        dibujo.Restore(estado);
    }
}
