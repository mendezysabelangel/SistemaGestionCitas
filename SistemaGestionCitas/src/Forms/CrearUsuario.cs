using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class CrearUsuario : Form
    {
        private readonly UsuarioService usuarioService = new UsuarioService();
        private readonly RolRepository rolRepository = new RolRepository();

        public CrearUsuario()
        {
            InitializeComponent();

            txtNombreCompleto.MaxLength = 100;
            txtNombreUsuario.MaxLength = 50;
            txtCorreo.MaxLength = 100;
            txtPassword.MaxLength = 100;
            txtConfirmarPassword.MaxLength = 100;

            CargarRoles();
        }

        private void CargarRoles()
        {
            try
            {
                List<string> roles = rolRepository.ObtenerRolesActivos();

                cmbRol.DataSource = roles;
                cmbRol.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron cargar los roles.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            string nombreCompleto = txtNombreCompleto.Text.Trim();
            string nombreUsuario = txtNombreUsuario.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string nombreRol = cmbRol.SelectedItem?.ToString() ?? "";
            string password = txtPassword.Text;
            string confirmarPassword = txtConfirmarPassword.Text;

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                MessageBox.Show(
                    "Ingrese el nombre completo.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNombreCompleto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                MessageBox.Show(
                    "Ingrese un nombre de usuario.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNombreUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show(
                    "Ingrese un correo electrónico.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCorreo.Focus();
                return;
            }

            if (!correo.Contains("@") || !correo.Contains("."))
            {
                MessageBox.Show(
                    "Ingrese un correo electrónico válido.",
                    "Correo inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtCorreo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                MessageBox.Show(
                    "Seleccione el rol que tendrá el usuario.",
                    "Rol requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbRol.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Ingrese una contraseña.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPassword.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show(
                    "La contraseña debe tener al menos 6 caracteres.",
                    "Contraseña inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(confirmarPassword))
            {
                MessageBox.Show(
                    "Confirme la contraseña.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtConfirmarPassword.Focus();
                return;
            }

            if (password != confirmarPassword)
            {
                MessageBox.Show(
                    "Las contraseñas no coinciden.",
                    "Error de contraseña",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtConfirmarPassword.Focus();
                return;
            }

            btnCrearUsuario.Enabled = false;

            ResultadoOperacion resultado;

            try
            {
                resultado = usuarioService.RegistrarUsuario(
                    nombreCompleto,
                    nombreUsuario,
                    correo,
                    password,
                    nombreRol);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar con la base de datos.\n\n" + ex.Message,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                btnCrearUsuario.Enabled = true;
                return;
            }

            btnCrearUsuario.Enabled = true;

            if (!resultado.Exitoso)
            {
                MessageBox.Show(
                    resultado.Mensaje,
                    "No se pudo crear el usuario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            MessageBox.Show(
                $"Usuario creado correctamente.\n\nRol asignado: {nombreRol}",
                "Usuario creado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Close();
        }

        private void txtNombreCompleto_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtNombreUsuario_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtCorreo_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtConfirmarPassword_TextChanged(object sender, EventArgs e)
        {
        }
    }
}