using System.Globalization;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

public class FrmServicios : Form
{
    private readonly ServicioService servicio = new();
    private readonly ComboBox buscarEstado = ControlesRegistro.Opciones("Todos", "Activo", "Inactivo");
    private readonly VistaRegistros<Servicio> vista = new("Servicios", "Catálogo y gestión de servicios",
        "Buscar por nombre", "servicio", registro => registro.IdServicio);

    public FrmServicios()
    {
        SuspendLayout();
        Text = "Servicios";
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        FormBorderStyle = FormBorderStyle.None;
        ClientSize = new Size(1040, 650);

        vista.Tabla.AgregarColumna("Servicio", 43, 88);
        vista.Tabla.AgregarColumna("Duración", 16, 75);
        vista.Tabla.AgregarColumna("Precio", 25, 110);
        vista.Tabla.AgregarColumna("Estado", 16, 78);
        vista.ValorCelda = (registro, indice) => indice switch
        {
            0 => registro.Nombre,
            1 => MostrarDuracion(registro.DuracionMinutos),
            2 => MostrarPrecio(registro.Precio),
            3 => registro.Estado,
            _ => null
        };
        vista.MostrarSeleccion = MostrarFicha;
        vista.AgregarFiltros(("Estado", buscarEstado));
        vista.Consultar += () => Consultar();
        vista.Restablecer += () =>
        {
            buscarEstado.SelectedIndex = 0;
            vista.Busqueda.Clear();
            Consultar();
        };
        vista.Nuevo += () => AbrirEditor(null);
        vista.Editar += AbrirEditor;
        vista.Eliminar += async registro => await vista.EliminarConfirmadoAsync(registro.Nombre,
            () => servicio.Eliminar(registro.IdServicio), () => Consultar(sinSeleccion: true));
        Controls.Add(vista);
        vista.ActualizarPermisos();
        Shown += (_, _) =>
        {
            vista.Tabla.RowTemplate.Height = LogicalToDeviceUnits(43);
            Consultar();
        };
        ResumeLayout(true);
    }

    private void Consultar(int? seleccionado = null, bool sinSeleccion = false)
    {
        string texto = vista.Busqueda.Text.Trim();
        bool? activo = ControlesRegistro.Estado(buscarEstado.SelectedIndex);
        vista.Cargar(() => (servicio.Buscar(texto, activo), null),
            texto.Length > 0 || activo.HasValue, seleccionado, sinSeleccion);
    }

    private static string MostrarDuracion(int minutos) => $"{minutos:N0} min";
    private static string MostrarPrecio(decimal precio) => $"RD$ {precio:N2}";

    private void MostrarFicha(Servicio? registro)
    {
        if (registro == null)
        {
            vista.PresentarFicha("Selecciona un servicio para ver sus detalles");
            return;
        }

        vista.PresentarFicha(registro.Nombre,
            ("Descripción", ControlesRegistro.Disponible(registro.Descripcion)),
            ("Duración", MostrarDuracion(registro.DuracionMinutos)),
            ("Precio", MostrarPrecio(registro.Precio)),
            ("Estado", registro.Estado));
    }

    private void AbrirEditor(Servicio? registro)
    {
        bool edicion = registro != null;
        string permiso = edicion ? PermisosSistema.Modificar : PermisosSistema.Agregar;
        if (!SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar servicios")
            || !SesionActual.VerificarPermiso(permiso, edicion ? "modificar servicios" : "registrar servicios"))
            return;

        using var editor = new EditorRegistro(edicion ? "Editar servicio" : "Nuevo servicio", permiso, edicion,
            PermisosSistema.Consultar);
        var nombre = ControlesRegistro.Texto(Servicio.MaxNombre, registro?.Nombre ?? "");
        var descripcion = ControlesRegistro.Texto(Servicio.MaxDescripcion, registro?.Descripcion ?? "");
        descripcion.Multiline = true;
        descripcion.AcceptsReturn = true;
        var duracion = ControlesRegistro.Texto(20, registro?.DuracionMinutos.ToString() ?? "");
        var precio = ControlesRegistro.Texto(32, registro?.Precio.ToString("0.00") ?? "");
        var activo = registro == null ? null
            : new CheckBox { Text = "Servicio activo", Checked = registro.Activo, AutoSize = true };
        string ejemploPrecio = 250.50m.ToString("0.00");
        editor.AgregarCampo("Nombre", nombre);
        editor.AgregarCampo("Descripción (opcional)", descripcion, 75);
        editor.AgregarCampo("Duración (minutos)", duracion);
        editor.AgregarCampo($"Precio (RD$, ej. {ejemploPrecio})", precio);
        if (activo != null) editor.AgregarCampo("Estado", activo);

        editor.PrepararGuardado = () =>
        {
            if (!int.TryParse(duracion.Text, out int minutos) || minutos <= 0)
                return editor.Advertir("Ingrese un número entero de minutos mayor que cero.", duracion);

            const NumberStyles formatoPrecio = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
            var cultura = CultureInfo.CurrentCulture;
            if (!decimal.TryParse(precio.Text.Trim(), formatoPrecio, cultura, out decimal monto))
                return editor.Advertir($"Ingrese un precio válido, por ejemplo {ejemploPrecio}.", precio);

            var datos = new Servicio
            {
                IdServicio = registro?.IdServicio ?? 0,
                Nombre = nombre.Text,
                Descripcion = descripcion.Text,
                DuracionMinutos = minutos,
                Precio = monto,
                Activo = activo?.Checked ?? true
            };
            return edicion ? () => servicio.Modificar(datos) : () => servicio.Registrar(datos);
        };
        vista.Paleta.AplicarDialogo(editor);
        if (editor.ShowDialog(this) == DialogResult.OK) Consultar(registro?.IdServicio, sinSeleccion: !edicion);
    }
}
