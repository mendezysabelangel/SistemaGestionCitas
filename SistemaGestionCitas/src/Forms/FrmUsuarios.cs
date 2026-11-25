using System.Globalization;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

public class FrmUsuarios : Form
{
    private readonly ComboBox rolFiltro = ControlesRegistro.Opciones("Todos los roles");
    private readonly ComboBox estadoFiltro = ControlesRegistro.Opciones("Todos", "Activo", "Inactivo");
    private readonly ComboBox bloqueoFiltro = ControlesRegistro.Opciones("Todos", "Bloqueados", "Sin bloqueo");
    private readonly System.Windows.Forms.Timer vencimientos = new();
    private readonly VistaRegistros<Usuario> vista = new("Usuarios", "Gestión de cuentas y accesos",
        "Buscar por nombre, usuario o correo", "usuario", u => u.IdUsuario, "Actualizar listado",
        permisoConsulta: PermisosSistema.CrearUsuarios, permisoNuevo: PermisosSistema.CrearUsuarios,
        etiquetaConsulta: "Buscar");
    private List<Usuario>? usuarios;
    private List<string> rolesActivos = new();
    private bool cargandoFiltros;

    public FrmUsuarios()
    {
        SuspendLayout();
        Text = "Usuarios";
        Name = "FrmUsuarios";
        FormBorderStyle = FormBorderStyle.None;
        ClientSize = new Size(1040, 650);
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoScaleDimensions = new SizeF(96F, 96F);
        vista.Tabla.RowTemplate.Height = 43;
        vista.Tabla.AgregarColumna("Usuario", 23, 75);
        vista.Tabla.AgregarColumna("Nombre completo", 31, 118);
        vista.Tabla.AgregarColumna("Correo", 29, 96);
        vista.Tabla.AgregarColumna("Estado", 16, 70);
        vista.Tabla.AgregarColumna("Bloqueo", 19, 90);
        vista.Tabla.SizeChanged += (_, _) =>
        {
            // El correo permanece visible en la ficha cuando falta ancho en el listado.
            vista.Tabla.Columns["Correo"].Visible = vista.Tabla.Width >= LogicalToDeviceUnits(485);
        };
        vista.Tabla.CellFormatting += (_, e) =>
        {
            if (vista.Registro(e.RowIndex) is not { } usuario) return;
            if (e.ColumnIndex == 3)
                e.CellStyle!.ForeColor = usuario.Activo ? Color.FromArgb(0, 90, 73) : ControlesRegistro.Peligro;
            else if (e.ColumnIndex == 4 && usuario.EstaBloqueado(DateTime.Now))
                e.CellStyle!.ForeColor = ControlesRegistro.Peligro;
        };
        vista.ValorCelda = (usuario, columna) => columna switch
        {
            0 => usuario.NombreUsuario,
            1 => usuario.NombreCompleto,
            2 => ControlesRegistro.Disponible(usuario.Correo),
            3 => usuario.Estado,
            4 => usuario.EstaBloqueado(DateTime.Now) ? "Bloqueado" : "Sin bloqueo",
            _ => null
        };
        vista.MostrarSeleccion = MostrarFicha;
        vista.AgregarFiltros(("Rol", rolFiltro), ("Estado", estadoFiltro), ("Bloqueo temporal", bloqueoFiltro));
        vista.Consultar += () => Consultar();
        vista.AccionExtra!.Click += (_, _) => Consultar(renovar: true);
        vista.ReintentarConsulta = () => Consultar(renovar: true);
        estadoFiltro.SelectedIndexChanged += (_, _) => { if (!cargandoFiltros) Consultar(); };
        rolFiltro.SelectedIndexChanged += (_, _) => { if (!cargandoFiltros) Consultar(); };
        bloqueoFiltro.SelectedIndexChanged += (_, _) => { if (!cargandoFiltros) Consultar(); };
        vista.Restablecer += () =>
        {
            cargandoFiltros = true;
            vista.Busqueda.Clear();
            rolFiltro.SelectedIndex = 0;
            estadoFiltro.SelectedIndex = 0;
            bloqueoFiltro.SelectedIndex = 0;
            cargandoFiltros = false;
            Consultar(sinSeleccion: true);
        };
        vista.Nuevo += CrearCuenta;
        vista.Editar += EditarCuenta;
        vista.Eliminar += EliminarCuenta;
        vista.ConfigurarAccionesCuenta(usuario => !CuentaActual(usuario), CambiarPassword,
            usuario => usuario.EstaBloqueado(DateTime.Now), DesbloquearCuenta);
        vencimientos.Tick += (_, _) =>
        {
            ProgramarVencimiento();
            Consultar();
        };
        Controls.Add(vista);
        vista.ActualizarPermisos();
        Load += (_, _) =>
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, "consultar usuarios"))
                Close();
        };
        Shown += (_, _) =>
        {
            vista.Tabla.RowTemplate.Height = LogicalToDeviceUnits(43);
            Consultar(renovar: true);
        };
        ResumeLayout(true);
    }

    private void Consultar(bool renovar = false, int? seleccionarId = null, bool sinSeleccion = false)
    {
        string texto = vista.Busqueda.Text.Trim();
        string? rol = rolFiltro.SelectedIndex > 0 ? rolFiltro.SelectedItem?.ToString() : null;
        bool? activo = ControlesRegistro.Estado(estadoFiltro.SelectedIndex);
        bool? bloqueado = bloqueoFiltro.SelectedIndex switch { 1 => true, 2 => false, _ => null };
        List<Usuario>? cache = renovar ? null : usuarios;

        vista.Cargar(() =>
        {
            var lista = cache ?? new UsuarioService().ListarUsuarios();
            var roles = cache == null ? new RolRepository().ObtenerRolesActivos() : null;
            DateTime ahora = DateTime.Now;
            List<Usuario> filtrados = lista.Where(u => (!activo.HasValue || u.Activo == activo)
                && (!bloqueado.HasValue || u.EstaBloqueado(ahora) == bloqueado)
                && (rol == null || string.Equals(u.NombreRol, rol, StringComparison.CurrentCultureIgnoreCase))
                && (texto.Length == 0 || Coincide(u.NombreUsuario) || Coincide(u.NombreCompleto) || Coincide(u.Correo))).ToList();
            return (filtrados, roles == null ? null : () => GuardarListado(lista, roles, rol));
        }, texto.Length > 0 || rol != null || activo.HasValue || bloqueado.HasValue, seleccionarId, sinSeleccion);

        bool Coincide(string? dato) => dato?.Contains(texto, StringComparison.CurrentCultureIgnoreCase) == true;
    }

    private void GuardarListado(List<Usuario> lista, List<string> roles, string? rol)
    {
        usuarios = lista;
        rolesActivos = roles;
        ProgramarVencimiento();
        cargandoFiltros = true;
        try
        {
            var opciones = roles.Concat(lista.Select(u => u.NombreRol))
                .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                .Distinct(StringComparer.CurrentCultureIgnoreCase).OrderBy(nombre => nombre).ToList();
            // Si el último usuario de un rol desaparece, el filtro conserva su nombre.
            if (rol != null && !opciones.Contains(rol, StringComparer.CurrentCultureIgnoreCase)) opciones.Add(rol);
            rolFiltro.Items.Clear();
            rolFiltro.Items.Add("Todos los roles");
            rolFiltro.Items.AddRange(opciones.Cast<object>().ToArray());
            rolFiltro.SelectedIndex = rol == null ? 0 : 1 + opciones.FindIndex(nombre =>
                string.Equals(nombre, rol, StringComparison.CurrentCultureIgnoreCase));
        }
        finally { cargandoFiltros = false; }
    }

    private void ProgramarVencimiento()
    {
        vencimientos.Stop();
        DateTime ahora = DateTime.Now;
        DateTime? proximo = usuarios?.Where(u => u.EstaBloqueado(ahora)).Min(u => u.BloqueadoHasta);
        if (proximo is not { } hasta) return;

        // Refrescar el estado al vencer el próximo bloqueo, sin volver a consultar SQL.
        vencimientos.Interval = (int)Math.Clamp(Math.Ceiling((hasta - ahora).TotalMilliseconds), 50, int.MaxValue);
        vencimientos.Start();
    }

    private void MostrarFicha(Usuario? usuario)
    {
        if (usuario == null)
        {
            vista.PresentarFicha("Selecciona un usuario para consultar sus datos");
            return;
        }
        string bloqueo = usuario.BloqueadoHasta is { } hasta && usuario.EstaBloqueado(DateTime.Now)
            ? "Bloqueado hasta " + hasta.ToString("dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture)
            : "Sin bloqueo";
        vista.PresentarFicha(usuario.NombreCompleto,
            ("Usuario", usuario.NombreUsuario),
            ("Correo", ControlesRegistro.Disponible(usuario.Correo)),
            ("Rol asignado", ControlesRegistro.Disponible(usuario.NombreRol)),
            ("Estado", usuario.Estado),
            ("Bloqueo temporal", bloqueo));
    }

    private static bool CuentaActual(Usuario usuario) => SesionActual.UsuarioActual?.IdUsuario == usuario.IdUsuario;

    private static bool VerificarAccion(string permiso, string accion) =>
        SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, accion) && SesionActual.VerificarPermiso(permiso, accion);

    private void CrearCuenta()
    {
        if (!SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, "crear usuarios")) return;
        using var formulario = new CrearUsuario();
        if (formulario.ShowDialog(this) == DialogResult.OK) Consultar(renovar: true, sinSeleccion: true);
    }

    private void EditarCuenta(Usuario usuario)
    {
        if (!VerificarAccion(PermisosSistema.Modificar, "modificar usuarios")) return;
        bool propia = CuentaActual(usuario);
        using var editor = new EditorRegistro("Editar usuario", PermisosSistema.Modificar, true, PermisosSistema.CrearUsuarios);
        var nombre = ControlesRegistro.Texto(100, usuario.NombreCompleto);
        var cuenta = ControlesRegistro.Texto(50, usuario.NombreUsuario);
        var correo = ControlesRegistro.Texto(100, usuario.Correo ?? "");
        var roles = ControlesRegistro.Opciones(rolesActivos.Append(usuario.NombreRol).Distinct().Cast<object>().ToArray());
        roles.SelectedItem = usuario.NombreRol;
        roles.Enabled = !propia;
        var activo = new CheckBox { Text = "Cuenta activa", Checked = usuario.Activo, Enabled = !propia, AutoSize = true };
        editor.AgregarCampo("Nombre completo", nombre);
        editor.AgregarCampo("Nombre de usuario", cuenta);
        editor.AgregarCampo("Correo electrónico", correo);
        editor.AgregarCampo("Rol", roles);
        editor.AgregarCampo("Estado", activo);
        editor.PrepararGuardado = () =>
        {
            string nombrePuesto = nombre.Text, cuentaPuesta = cuenta.Text, correoPuesto = correo.Text;
            string rol = propia ? usuario.NombreRol : roles.SelectedItem?.ToString() ?? "";
            bool estado = propia ? usuario.Activo : activo.Checked;
            return () => new UsuarioService().ModificarUsuario(usuario.IdUsuario, nombrePuesto, cuentaPuesta, correoPuesto, rol, estado);
        };
        if (editor.ShowDialog(this) != DialogResult.OK) return;
        if (MdiParent is FrmDashboard dashboard) dashboard.ActualizarDatosSesion();
        Consultar(renovar: true, seleccionarId: usuario.IdUsuario);
    }

    private void CambiarPassword(Usuario usuario)
    {
        if (!VerificarAccion(PermisosSistema.Modificar, "cambiar contraseñas")) return;
        using var formulario = new FrmCambiarPasswordUsuario(usuario.IdUsuario, usuario.NombreUsuario);
        if (formulario.ShowDialog(this) == DialogResult.OK) Consultar(renovar: true, seleccionarId: usuario.IdUsuario);
    }

    private async void DesbloquearCuenta(Usuario usuario)
    {
        if (!VerificarAccion(PermisosSistema.Modificar, "desbloquear usuarios")) return;
        if (!usuario.EstaBloqueado(DateTime.Now))
        {
            Consultar(renovar: true, seleccionarId: usuario.IdUsuario);
            return;
        }

        if (MessageBox.Show(this, $"¿Desbloquear a {usuario.NombreUsuario}? El estado de la cuenta permanecerá igual.",
            "Confirmar desbloqueo", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

        await vista.EjecutarOperacionAsync("Desbloquear", () => new UsuarioService().DesbloquearUsuario(usuario.IdUsuario),
            resultado =>
            {
                Consultar(renovar: true, seleccionarId: usuario.IdUsuario);
                MessageBox.Show(this, resultado.Mensaje, "Desbloquear usuario", MessageBoxButtons.OK,
                    resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            });
    }

    private async void EliminarCuenta(Usuario usuario)
    {
        if (CuentaActual(usuario)) return;
        await vista.EliminarConfirmadoAsync(usuario.NombreUsuario, () => new UsuarioService().EliminarUsuario(usuario.IdUsuario),
            () => Consultar(renovar: true, sinSeleccion: true));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing) vencimientos.Dispose();
        base.Dispose(disposing);
    }
}
