using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SistemaGestionCitas.src.Forms;

internal sealed class TemaNativoCitorna
{
    private static readonly ConditionalWeakTable<Control, TemaNativoCitorna> controles = new();
    private readonly Control control;
    private readonly string estilo;
    private readonly FlatStyle estiloClaro;

    private TemaNativoCitorna(Control control)
    {
        this.control = control;
        estilo = control is ComboBox ? "DarkMode_CFD" : "DarkMode_Explorer";
        if (control is ComboBox combo) estiloClaro = combo.FlatStyle;
        control.HandleCreated += AplicarOscuro;
    }

    internal static void Aplicar(Control control, bool oscuro)
    {
        if (oscuro && !SystemInformation.HighContrast)
        {
            var tema = controles.GetValue(control, static c => new TemaNativoCitorna(c));
            // WinForms dibuja su propio borde y flecha en FlatStyle.Flat.
            if (control is ComboBox combo) combo.FlatStyle = FlatStyle.Standard;
            tema.AplicarOscuro(control, EventArgs.Empty);
        }
        else if (controles.TryGetValue(control, out var tema))
        {
            controles.Remove(control);
            control.HandleCreated -= tema.AplicarOscuro;
            if (control is ComboBox combo) combo.FlatStyle = tema.estiloClaro;
            if (control.IsHandleCreated) SetWindowTheme(control.Handle, null, null);
        }
    }

    private void AplicarOscuro(object? sender, EventArgs e)
    {
        if (control.IsHandleCreated) SetWindowTheme(control.Handle, estilo, null);
    }

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern int SetWindowTheme(nint ventana, string? estilo, string? lista);
}
