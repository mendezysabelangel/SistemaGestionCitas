using SistemaGestionCitas.src.Services;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Forms
{
    public class FrmCambiarPasswordUsuario : Form
    {
        private readonly UsuarioService usuarioService = new UsuarioService();
        private readonly int idUsuario;
        private readonly TextBox txtPassword = new TextBox();
        private readonly TextBox txtConfirmarPassword = new TextBox();

        public FrmCambiarPasswordUsuario(int idUsuario, string nombreUsuario)
        {
            this.idUsuario = idUsuario;
            txtPassword.MaxLength = txtConfirmarPassword.MaxLength = 100;
            txtPassword.UseSystemPasswordChar = txtConfirmarPassword.UseSystemPasswordChar = true;
            var guardar = ControlesRegistro.Boton("Guardar", ControlesRegistro.Primario, Color.White);
            var cancelar = ControlesRegistro.Boton("Cancelar");
            guardar.Click += BtnGuardar_Click;
            var contenido = DialogoCitorna.Campos(1,
                ("Nueva contraseña", txtPassword, 40), ("Confirmar contraseña", txtConfirmarPassword, 40));
            DialogoCitorna.Configurar(this, "Cambiar contraseña",
                $"Cuenta: {nombreUsuario} · Entre 6 y 100 caracteres.", new Size(495, 340), contenido, guardar, cancelar);
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            try
            {
                ResultadoOperacion resultado = usuarioService.CambiarPassword(
                    idUsuario, txtPassword.Text, txtConfirmarPassword.Text);
                MessageBox.Show(resultado.Mensaje, "Cambiar contraseña", MessageBoxButtons.OK,
                    resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (!resultado.Exitoso) return;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar la contraseña.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
