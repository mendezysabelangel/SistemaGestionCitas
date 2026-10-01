using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmDashboard : Form
    {
        public FrmDashboard()
        {
            InitializeComponent();
            CargarSesion();

            FormClosed += FrmDashboard_FormClosed;
        }

        private void CargarSesion()
        {
            if (SesionActual.UsuarioActual == null)
                return;

            lblBienvenida.Text =
                $"Bienvenido, {SesionActual.UsuarioActual.NombreCompleto}";

            lblUsuario.Text =
                SesionActual.UsuarioActual.NombreUsuario;

            lblRol.Text =
                SesionActual.UsuarioActual.NombreRol;

            // Opciones administrativas
            btnCrearUsuario.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);

            btnUsuario.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);

            btnCrearRol.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearRoles);

            // Mostrar visualmente los permisos del rol
            ConfigurarBotonPermiso(
                btnConsultar,
                PermisosSistema.Consultar);

            ConfigurarBotonPermiso(
                btnAgregar,
                PermisosSistema.Agregar);

            ConfigurarBotonPermiso(
                btnModificar,
                PermisosSistema.Modificar);

            ConfigurarBotonPermiso(
                btnEliminar,
                PermisosSistema.Eliminar);
        }

        private void ConfigurarBotonPermiso(
            Button boton,
            string permiso)
        {
            bool permitido =
                SesionActual.TienePermiso(permiso);

            if (permitido)
            {
                boton.BackColor =
                    Color.FromArgb(220, 232, 255);

                boton.ForeColor =
                    Color.FromArgb(28, 69, 171);
            }
            else
            {
                boton.BackColor =
                    Color.FromArgb(235, 237, 240);

                boton.ForeColor =
                    Color.Gray;
            }
        }

        private void EjecutarAccion(
            string permiso,
            string accion)
        {
            if (!SesionActual.VerificarPermiso(
                permiso,
                accion))
            {
                return;
            }

            MessageBox.Show(
                $"Acceso permitido.\n\nSu rol puede {accion}.",
                "Permiso concedido",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnConsultar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Consultar,
                "consultar información");
        }

        private void btnAgregar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Agregar,
                "agregar registros");
        }

        private void btnModificar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Modificar,
                "modificar registros");
        }

        private void btnEliminar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Eliminar,
                "eliminar registros");
        }

        private void btnCrearUsuario_Click(
            object sender,
            EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.CrearUsuarios,
                "crear usuarios"))
            {
                return;
            }

            using CrearUsuario formulario =
                new CrearUsuario();

            formulario.ShowDialog(this);
        }

        private void btnCrearRol_Click(
            object sender,
            EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.CrearRoles,
                "crear nuevos roles"))
            {
                return;
            }

            using FrmCrearRol formulario =
                new FrmCrearRol();

            formulario.ShowDialog(this);
        }

        private void btnCerrarSesion_Click(
            object sender,
            EventArgs e)
        {
            SesionActual.CerrarSesion();
            Close();
        }

        private void FrmDashboard_FormClosed(
            object? sender,
            FormClosedEventArgs e)
        {
            SesionActual.CerrarSesion();
        }

        private void btnUsuarios_Click(
            object sender,
            EventArgs e)
        {
        }

        private void lblSistema_Click(
            object sender,
            EventArgs e)
        {
        }

        private void lblUsuario_Click(
            object sender,
            EventArgs e)
        {
        }

        private void pnlTopbar_Paint(
            object sender,
            PaintEventArgs e)
        {
        }
    }
}