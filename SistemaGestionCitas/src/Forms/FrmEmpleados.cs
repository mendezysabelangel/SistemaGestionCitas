using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

public class FrmEmpleados : Form
{
    private readonly EmpleadoService servicio = new();
    private readonly ComboBox buscarEstado = ControlesRegistro.Opciones("Todos", "Activo", "Inactivo");
    private readonly VistaRegistros<Empleado> vista = new("Empleados", "Consulta y gestión de empleados",
        "Buscar por nombre o apellido", "empleado", empleado => empleado.IdEmpleado);

    public FrmEmpleados()
    {
        SuspendLayout();
        Text = "Empleados";
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        FormBorderStyle = FormBorderStyle.None;
        ClientSize = new Size(1040, 650);

        vista.Tabla.AgregarColumna("Empleado", 49, 110);
        vista.Tabla.AgregarColumna("Teléfono", 31, 103);
        vista.Tabla.AgregarColumna("Estado", 20, 78);
        vista.ValorCelda = (empleado, indice) => indice switch
        {
            0 => empleado.NombreCompleto,
            1 => empleado.Telefono,
            2 => empleado.Estado,
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
        vista.Eliminar += async empleado => await vista.EliminarConfirmadoAsync(empleado.NombreCompleto,
            () => servicio.Eliminar(empleado.IdEmpleado), () => Consultar(sinSeleccion: true));
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

    private void MostrarFicha(Empleado? empleado)
    {
        if (empleado == null)
        {
            vista.PresentarFicha("Selecciona un empleado para ver sus datos");
            return;
        }

        vista.PresentarFicha(empleado.NombreCompleto,
            ("Teléfono", empleado.Telefono),
            ("Correo", ControlesRegistro.Disponible(empleado.Correo)),
            ("Fecha de registro", empleado.FechaRegistro.ToString("dd/MM/yyyy")),
            ("Estado", empleado.Estado));
    }

    private void AbrirEditor(Empleado? empleado)
    {
        bool edicion = empleado != null;
        string permiso = edicion ? PermisosSistema.Modificar : PermisosSistema.Agregar;
        if (!SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar empleados")
            || !SesionActual.VerificarPermiso(permiso, edicion ? "modificar empleados" : "registrar empleados"))
            return;

        using var editor = new EditorRegistro(edicion ? "Editar empleado" : "Nuevo empleado", permiso, edicion,
            PermisosSistema.Consultar);
        var nombres = ControlesRegistro.Texto(Empleado.MaxNombres, empleado?.Nombres ?? "");
        var apellidos = ControlesRegistro.Texto(Empleado.MaxApellidos, empleado?.Apellidos ?? "");
        var telefono = ControlesRegistro.Texto(Empleado.MaxTelefono, empleado?.Telefono ?? "");
        var correo = ControlesRegistro.Texto(Empleado.MaxCorreo, empleado?.Correo ?? "");
        var activo = empleado == null ? null
            : new CheckBox { Text = "Empleado activo", Checked = empleado.Activo, AutoSize = true };
        editor.AgregarCampo("Nombres", nombres);
        editor.AgregarCampo("Apellidos", apellidos);
        editor.AgregarCampo("Teléfono", telefono);
        editor.AgregarCampo("Correo (opcional)", correo);
        if (activo != null) editor.AgregarCampo("Estado", activo);

        editor.PrepararGuardado = () =>
        {
            var datos = new Empleado
            {
                IdEmpleado = empleado?.IdEmpleado ?? 0,
                Nombres = nombres.Text,
                Apellidos = apellidos.Text,
                Telefono = telefono.Text,
                Correo = correo.Text,
                Activo = activo?.Checked ?? true
            };
            return edicion ? () => servicio.Modificar(datos) : () => servicio.Registrar(datos);
        };
        vista.Paleta.AplicarDialogo(editor);
        if (editor.ShowDialog(this) == DialogResult.OK) Consultar(empleado?.IdEmpleado, sinSeleccion: !edicion);
    }
}
