using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class CrearUsuario : Form
    {
        private readonly UsuarioService usuarioService = new UsuarioService();

        public CrearUsuario()
        {
            InitializeComponent();

            txtNombreCompleto.MaxLength = 100;
            txtNombreUsuario.MaxLength = 50;
            txtCorreo.MaxLength = 100;
            txtPassword.MaxLength = 100;
            txtConfirmarPassword.MaxLength = 100;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VolverAlLogin()
        {
            FormLogin? login = Application.OpenForms.OfType<FormLogin>().FirstOrDefault();

            if (login == null)
                login = new FormLogin();

            login.Show();
            this.Close();
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
            btnCrearUsuario.Enabled = false;

            ResultadoOperacion resultado;
            try
            {
                resultado = usuarioService.RegistrarUsuarioEjecutor(
                    nombreCompleto, nombreUsuario, correo, password);
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
                resultado.Mensaje,
                "Usuario creado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

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
