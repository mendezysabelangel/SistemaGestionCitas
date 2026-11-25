namespace SistemaGestionCitas.src.Forms
{
    partial class CrearUsuario
    {
        private readonly TextBox txtNombreCompleto = new();
        private readonly TextBox txtNombreUsuario = new();
        private readonly TextBox txtCorreo = new();
        private readonly ComboBox cmbRol = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtPassword = new() { UseSystemPasswordChar = true };
        private readonly TextBox txtConfirmarPassword = new() { UseSystemPasswordChar = true };
        private readonly CheckBox chkActivo = new() { Text = "Cuenta activa", Checked = true, AutoSize = true };
        private Button btnCrearUsuario = null!;

        private void InitializeComponent()
        {
            Name = "CrearUsuario";
            btnCrearUsuario = ControlesRegistro.Boton("Crear usuario", ControlesRegistro.Primario, Color.White);
            var cancelar = ControlesRegistro.Boton("Cancelar");
            btnCrearUsuario.Click += btnCrearUsuario_Click;
            var contenido = DialogoCitorna.Tabla();
            contenido.Controls.Add(DialogoCitorna.Campos(2,
                ("Nombre completo", txtNombreCompleto, 40), ("Nombre de usuario", txtNombreUsuario, 40),
                ("Correo electrónico", txtCorreo, 40), ("Rol", cmbRol, 40),
                ("Contraseña", txtPassword, 40), ("Confirmar contraseña", txtConfirmarPassword, 40)));
            chkActivo.Margin = new Padding(2, 6, 0, 2);
            contenido.Controls.Add(chkActivo);
            var ayuda = CampoCitorna.Rotulo("Una cuenta activa puede iniciar sesión.", true);
            ayuda.Margin = new Padding(2, 0, 0, 3);
            contenido.Controls.Add(ayuda);
            DialogoCitorna.Configurar(this, "Crear usuario", "Registra una nueva cuenta para Citorna.",
                new Size(665, 465), contenido, btnCrearUsuario, cancelar);
        }
    }
}
