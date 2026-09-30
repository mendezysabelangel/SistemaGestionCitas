namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            txtContrasena.UseSystemPasswordChar =
                !txtContrasena.UseSystemPasswordChar;

            if (txtContrasena.UseSystemPasswordChar)
            {
                btnTogglePassword.Text = "Ver";
            }
            else
            {
                btnTogglePassword.Text = "Ocultar";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show(
                    "Debe ingresar el usuario.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show(
                    "Debe ingresar la contraseña.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtContrasena.Focus();
                return;
            }

            MessageBox.Show(
                "Los campos fueron completados correctamente.",
                "Validación",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void lblHeroDescription_Click(object sender, EventArgs e)
        {

        }

        private void pnlLoginArea_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlLoginCard_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}