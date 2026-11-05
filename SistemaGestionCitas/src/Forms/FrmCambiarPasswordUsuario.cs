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
            Text = "Cambiar contraseña";
            Font = new Font("Segoe UI", 10F);
            ClientSize = new Size(480, 290);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            Controls.Add(new Label
            {
                Text = "Usuario: " + nombreUsuario, Location = new Point(20, 15),
                Size = new Size(440, 42), AutoEllipsis = true
            });
            Controls.Add(new Label
            {
                Text = "Nueva contraseña:", Location = new Point(20, 65), AutoSize = true
            });
            txtPassword.Location = new Point(20, 95);
            txtPassword.Width = 440;
            txtPassword.MaxLength = 100;
            txtPassword.UseSystemPasswordChar = true;
            Controls.Add(txtPassword);
            Controls.Add(new Label
            {
                Text = "Confirmar contraseña:", Location = new Point(20, 140), AutoSize = true
            });
            txtConfirmarPassword.Location = new Point(20, 170);
            txtConfirmarPassword.Width = 440;
            txtConfirmarPassword.MaxLength = 100;
            txtConfirmarPassword.UseSystemPasswordChar = true;
            Controls.Add(txtConfirmarPassword);
            Controls.Add(new Label
            {
                Text = "Entre 6 y 100 caracteres.", Location = new Point(20, 205),
                AutoSize = true, ForeColor = Color.DimGray
            });

            Button guardar = new Button
            {
                Text = "Guardar", Location = new Point(220, 235), Size = new Size(110, 36),
                BackColor = Color.FromArgb(27, 68, 170), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            guardar.Click += BtnGuardar_Click;
            Button cancelar = new Button
            {
                Text = "Cancelar", Location = new Point(345, 235), Size = new Size(115, 36),
                DialogResult = DialogResult.Cancel
            };
            Controls.Add(guardar);
            Controls.Add(cancelar);
            AcceptButton = guardar;
            CancelButton = cancelar;
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
