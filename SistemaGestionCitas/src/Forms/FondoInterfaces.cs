using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SistemaGestionCitas.src.Forms;

internal static class FondoInterfaces
{
    private static readonly Lazy<Bitmap> Imagen = new(CargarImagen);
    private static Bitmap? escalada;

    private static Bitmap CargarImagen()
    {
        using Stream recurso = typeof(FondoInterfaces).Assembly.GetManifestResourceStream("Citorna.Interfaces.Fondo.png")
            ?? throw new InvalidOperationException("No se encontró el fondo de las interfaces de Citorna.");
        using Image imagen = Image.FromStream(recurso);
        var bitmap = new Bitmap(imagen);
        Application.ApplicationExit += Liberar;

        void Liberar(object? sender, EventArgs e)
        {
            Application.ApplicationExit -= Liberar;
            escalada?.Dispose();
            escalada = null;
            bitmap.Dispose();
        }
        return bitmap;
    }

    internal static void Pintar(Control superficie, PaintEventArgs e)
    {
        Rectangle destino = superficie.ClientRectangle;
        Rectangle visible = Rectangle.Intersect(e.ClipRectangle, destino);
        if (visible.Width <= 0 || visible.Height <= 0) return;
        if (destino.Width < superficie.LogicalToDeviceUnits(590) || destino.Height < 1 || SystemInformation.HighContrast)
        {
            using var plano = new SolidBrush(EstiloAgenda.Fondo);
            e.Graphics.FillRectangle(plano, visible);
            return;
        }

        Bitmap imagen = Imagen.Value;
        // Redondear la altura evita volver a escalar en cada paso del menú plegable.
        int alturaNecesaria = Math.Max(destino.Height, (int)Math.Ceiling(destino.Width * (double)imagen.Height / imagen.Width));
        int altura = (alturaNecesaria + 31) / 32 * 32;
        if (escalada == null || escalada.Height < alturaNecesaria || escalada.Height > alturaNecesaria + 64)
        {
            int ancho = (int)Math.Ceiling(altura * (double)imagen.Width / imagen.Height);
            var siguiente = new Bitmap(ancho, altura, PixelFormat.Format32bppPArgb);
            using (var dibujo = Graphics.FromImage(siguiente))
            {
                dibujo.Clear(EstiloAgenda.Fondo);
                dibujo.InterpolationMode = InterpolationMode.HighQualityBilinear;
                dibujo.DrawImage(imagen, new Rectangle(0, 0, ancho, altura));
            }
            escalada?.Dispose();
            escalada = siguiente;
        }

        int recorteIzquierdo = escalada.Width - destino.Width;
        e.Graphics.DrawImage(escalada, visible, visible.X + recorteIzquierdo, visible.Y,
            visible.Width, visible.Height, GraphicsUnit.Pixel);
    }
}

internal sealed class BotonCitorna : Button
{
    internal Color Relleno { get; set; } = Color.White;
    internal SimboloAgenda? Icono { get; init; }
    private bool presionado;

    internal BotonCitorna()
    {
        FlatStyle = FlatStyle.Flat;
        BackColor = Color.Transparent;
        DoubleBuffered = true;
        ResizeRedraw = true;
        UseVisualStyleBackColor = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
        if (Width < 3 || Height < 3) return;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var area = new RectangleF(1, 1, Width - 2, Height - 2);
        using var forma = EstiloAgenda.RectanguloRedondo(area, LogicalToDeviceUnits(9));
        bool encima = Enabled && ClientRectangle.Contains(PointToClient(MousePosition));
        Color relleno = Relleno;
        if (!Enabled) relleno = EstiloAgenda.Barra;
        else if (encima || presionado)
        {
            relleno = Relleno == Color.White ? EstiloAgenda.Barra
                : ControlPaint.Dark(Relleno, presionado ? .16F : .07F);
        }
        using var fondo = new SolidBrush(relleno);
        g.FillPath(fondo, forma);
        if (FlatAppearance.BorderSize > 0)
        {
            using var borde = new Pen(FlatAppearance.BorderColor);
            g.DrawPath(borde, forma);
        }

        Color tinta = Enabled ? ForeColor : SystemColors.GrayText;
        if (Icono is { } icono)
        {
            int lado = LogicalToDeviceUnits(22);
            IconosAgenda.Dibujar(g, icono, tinta, new Rectangle((Width - lado) / 2, (Height - lado) / 2, lado, lado));
        }
        else
        {
            TextFormatFlags opciones = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis;
            if (!ShowKeyboardCues) opciones |= TextFormatFlags.HidePrefix;
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, tinta, opciones);
        }

        // Un único aro interior sigue la forma del botón y sólo se muestra con indicaciones de teclado.
        int margen = LogicalToDeviceUnits(4);
        if (Focused && ShowFocusCues && Width > 2 * margen + 2 && Height > 2 * margen + 2)
        {
            var interior = new RectangleF(margen, margen, Width - 2 * margen - 1, Height - 2 * margen - 1);
            using var formaFoco = EstiloAgenda.RectanguloRedondo(interior, LogicalToDeviceUnits(6));
            using var foco = new Pen(ForeColor, LogicalToDeviceUnits(2));
            g.DrawPath(foco, formaFoco);
        }
    }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); Invalidate(); }
    protected override void OnMouseDown(MouseEventArgs e) { presionado = e.Button == MouseButtons.Left; base.OnMouseDown(e); Invalidate(); }
    protected override void OnMouseUp(MouseEventArgs e) { presionado = false; base.OnMouseUp(e); Invalidate(); }
    protected override void OnMouseCaptureChanged(EventArgs e) { presionado = false; base.OnMouseCaptureChanged(e); Invalidate(); }
}
