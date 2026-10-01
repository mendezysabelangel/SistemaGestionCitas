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

            // Solo usuarios con permiso de crear usuarios internos
            // podrán ver esta opción.
            btnCrearUsuario.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);
            btnUsuario.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);
            btnCrearRol.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearRoles);
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {

        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            SesionActual.CerrarSesion();
            Close();
        }

        private void FrmDashboard_FormClosed(object? sender, FormClosedEventArgs e)
        {
            SesionActual.CerrarSesion();
        }

        private void lblSistema_Click(object sender, EventArgs e)
        {

        }

        private void lblUsuario_Click(object sender, EventArgs e)
        {

        }

        private void pnlTopbar_Paint(object sender, PaintEventArgs e)
        {

        }



        private void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.CrearUsuarios,
                "crear usuarios"))
            {
                return;
            }

            using CrearUsuario formulario = new CrearUsuario();
            formulario.ShowDialog(this);
        }
        private void btnCrearRol_Click(object sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.CrearRoles,
                "crear nuevos roles"))
            {
                return;
            }

            using FrmCrearRol formulario = new FrmCrearRol();
            formulario.ShowDialog(this);
        }
    }
}