using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SistemaGestionCitas.src.Forms
{
    public partial class CrearUsuario : Form
    {
        public CrearUsuario()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void lblNombreCompleto_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void lblCorreo_Click(object sender, EventArgs e)
        {

        }

        private void CrearUsuario_Load(object sender, EventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Hide();

            FormLogin ventanaInicio = new FormLogin();
            ventanaInicio.ShowDialog();

            this.Show();
        }

        private void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            // Obtener los datos de los TextBox
            string nombreCompleto = txtNombreCompleto.Text.Trim();
            string nombreUsuario = txtNombreUsuario.Text.Trim();
            string correo = txtCorreo.Text.Trim();
            string password = txtPassword.Text;
            string confirmarPassword = txtConfirmarPassword.Text;

            // Validar nombre completo
            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                MessageBox.Show(
                    "Ingrese su nombre completo.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNombreCompleto.Focus();
                return;
            }

            // Validar nombre de usuario
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                MessageBox.Show(
                    "Ingrese un nombre de usuario.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNombreUsuario.Focus();
                return;
            }

            // Validar correo
            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show(
                    "Ingrese un correo electrónico.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtCorreo.Focus();
                return;
            }

            // Validar formato básico del correo
            if (!correo.Contains("@") || !correo.Contains("."))
            {
                MessageBox.Show(
                    "Ingrese un correo electrónico válido.",
                    "Correo inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtCorreo.Focus();
                return;
            }

            // Validar contraseña
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Ingrese una contraseña.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPassword.Focus();
                return;
            }

            // Validar longitud de contraseña
            if (password.Length < 6)
            {
                MessageBox.Show(
                    "La contraseña debe tener al menos 6 caracteres.",
                    "Contraseña inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPassword.Focus();
                return;
            }

            // Validar confirmación
            if (string.IsNullOrWhiteSpace(confirmarPassword))
            {
                MessageBox.Show(
                    "Confirme la contraseña.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtConfirmarPassword.Focus();
                return;
            }

            // Validar que las contraseñas coincidan
            if (password != confirmarPassword)
            {
                MessageBox.Show(
                    "Las contraseñas no coinciden.",
                    "Error de contraseña",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtConfirmarPassword.Focus();
                return;
            }

            // Todos los datos pasaron las validaciones
            MessageBox.Show(
                "Los datos son válidos.",
                "Correcto",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // AQUÍ VA EL INSERT A LA BASE DE DATOS

            // Después de que el INSERT sea exitoso,
            // entonces se abre el Dashboard.

            this.Hide();

            FrmDashboard ventanaDashboard = new FrmDashboard();
            ventanaDashboard.ShowDialog();

            this.Close();
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
