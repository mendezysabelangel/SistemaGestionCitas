namespace SistemaGestionCitas.src.Forms
{
    partial class CrearUsuario
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlLateral = new Panel();
            lblDescripcionLateral = new Label();
            lblMensajePrincipal = new Label();
            lblSubtituloSistema = new Label();
            lblNombreSistema = new Label();

            pnlFormulario = new Panel();
            lblIconoUsuario = new Label();
            lblTitulo = new Label();
            lblDescripcion = new Label();

            lblNombreCompleto = new Label();
            txtNombreCompleto = new TextBox();

            lblNombreUsuario = new Label();
            txtNombreUsuario = new TextBox();

            lblCorreo = new Label();
            txtCorreo = new TextBox();

            lblRol = new Label();
            cmbRol = new ComboBox();

            lblPassword = new Label();
            txtPassword = new TextBox();

            lblConfirmarPassword = new Label();
            txtConfirmarPassword = new TextBox();

            btnCrearUsuario = new Button();
            btnCancelar = new Button();

            pnlLateral.SuspendLayout();
            pnlFormulario.SuspendLayout();
            SuspendLayout();

            // 
            // pnlLateral
            // 
            pnlLateral.BackColor = Color.FromArgb(36, 80, 177);
            pnlLateral.Controls.Add(lblDescripcionLateral);
            pnlLateral.Controls.Add(lblMensajePrincipal);
            pnlLateral.Controls.Add(lblSubtituloSistema);
            pnlLateral.Controls.Add(lblNombreSistema);
            pnlLateral.Dock = DockStyle.Left;
            pnlLateral.Location = new Point(0, 0);
            pnlLateral.Name = "pnlLateral";
            pnlLateral.Size = new Size(360, 700);
            pnlLateral.TabIndex = 0;

            // 
            // lblNombreSistema
            // 
            lblNombreSistema.AutoSize = true;
            lblNombreSistema.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblNombreSistema.ForeColor = Color.White;
            lblNombreSistema.Location = new Point(45, 65);
            lblNombreSistema.Name = "lblNombreSistema";
            lblNombreSistema.Size = new Size(231, 32);
            lblNombreSistema.TabIndex = 0;
            lblNombreSistema.Text = "Sistema de Gestión";

            // 
            // lblSubtituloSistema
            // 
            lblSubtituloSistema.AutoSize = true;
            lblSubtituloSistema.Font = new Font("Segoe UI", 10F);
            lblSubtituloSistema.ForeColor = Color.FromArgb(205, 217, 245);
            lblSubtituloSistema.Location = new Point(47, 105);
            lblSubtituloSistema.Name = "lblSubtituloSistema";
            lblSubtituloSistema.Size = new Size(187, 23);
            lblSubtituloSistema.TabIndex = 1;
            lblSubtituloSistema.Text = "Administración de citas";

            // 
            // lblMensajePrincipal
            // 
            lblMensajePrincipal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblMensajePrincipal.ForeColor = Color.White;
            lblMensajePrincipal.Location = new Point(45, 210);
            lblMensajePrincipal.Name = "lblMensajePrincipal";
            lblMensajePrincipal.Size = new Size(280, 160);
            lblMensajePrincipal.TabIndex = 2;
            lblMensajePrincipal.Text = "Crea y administra usuarios de forma segura";

            // 
            // lblDescripcionLateral
            // 
            lblDescripcionLateral.Font = new Font("Segoe UI", 10F);
            lblDescripcionLateral.ForeColor = Color.FromArgb(215, 225, 248);
            lblDescripcionLateral.Location = new Point(47, 505);
            lblDescripcionLateral.Name = "lblDescripcionLateral";
            lblDescripcionLateral.Size = new Size(270, 90);
            lblDescripcionLateral.TabIndex = 3;
            lblDescripcionLateral.Text =
                "Gestiona el acceso al sistema y asigna el rol correspondiente a cada usuario.";

            // 
            // pnlFormulario
            // 
            pnlFormulario.BackColor = Color.White;
            pnlFormulario.Controls.Add(lblIconoUsuario);
            pnlFormulario.Controls.Add(lblTitulo);
            pnlFormulario.Controls.Add(lblDescripcion);

            pnlFormulario.Controls.Add(lblNombreCompleto);
            pnlFormulario.Controls.Add(txtNombreCompleto);

            pnlFormulario.Controls.Add(lblNombreUsuario);
            pnlFormulario.Controls.Add(txtNombreUsuario);

            pnlFormulario.Controls.Add(lblCorreo);
            pnlFormulario.Controls.Add(txtCorreo);

            pnlFormulario.Controls.Add(lblRol);
            pnlFormulario.Controls.Add(cmbRol);

            pnlFormulario.Controls.Add(lblPassword);
            pnlFormulario.Controls.Add(txtPassword);

            pnlFormulario.Controls.Add(lblConfirmarPassword);
            pnlFormulario.Controls.Add(txtConfirmarPassword);

            pnlFormulario.Controls.Add(btnCrearUsuario);
            pnlFormulario.Controls.Add(btnCancelar);

            pnlFormulario.Location = new Point(430, 30);
            pnlFormulario.Name = "pnlFormulario";
            pnlFormulario.Size = new Size(510, 640);
            pnlFormulario.TabIndex = 1;

            // 
            // lblIconoUsuario
            // 
            lblIconoUsuario.BackColor = Color.FromArgb(235, 240, 253);
            lblIconoUsuario.Font = new Font("Segoe UI Emoji", 24F);
            lblIconoUsuario.ForeColor = Color.FromArgb(36, 80, 177);
            lblIconoUsuario.Location = new Point(55, 25);
            lblIconoUsuario.Name = "lblIconoUsuario";
            lblIconoUsuario.Size = new Size(65, 65);
            lblIconoUsuario.TabIndex = 0;
            lblIconoUsuario.Text = "♙";
            lblIconoUsuario.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(20, 20, 25);
            lblTitulo.Location = new Point(140, 28);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(234, 46);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Crear usuario";

            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Font = new Font("Segoe UI", 9.5F);
            lblDescripcion.ForeColor = Color.Gray;
            lblDescripcion.Location = new Point(142, 74);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(304, 21);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Registra una nueva cuenta para el sistema.";

            // 
            // lblNombreCompleto
            // 
            lblNombreCompleto.AutoSize = true;
            lblNombreCompleto.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblNombreCompleto.Location = new Point(55, 120);
            lblNombreCompleto.Name = "lblNombreCompleto";
            lblNombreCompleto.Size = new Size(150, 21);
            lblNombreCompleto.TabIndex = 3;
            lblNombreCompleto.Text = "Nombre completo";

            // 
            // txtNombreCompleto
            // 
            txtNombreCompleto.Font = new Font("Segoe UI", 10F);
            txtNombreCompleto.Location = new Point(55, 147);
            txtNombreCompleto.Name = "txtNombreCompleto";
            txtNombreCompleto.PlaceholderText = "Ingrese el nombre completo";
            txtNombreCompleto.Size = new Size(400, 30);
            txtNombreCompleto.TabIndex = 0;
            txtNombreCompleto.TextChanged += txtNombreCompleto_TextChanged;

            // 
            // lblNombreUsuario
            // 
            lblNombreUsuario.AutoSize = true;
            lblNombreUsuario.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblNombreUsuario.Location = new Point(55, 190);
            lblNombreUsuario.Name = "lblNombreUsuario";
            lblNombreUsuario.Size = new Size(157, 21);
            lblNombreUsuario.TabIndex = 5;
            lblNombreUsuario.Text = "Nombre de usuario";

            // 
            // txtNombreUsuario
            // 
            txtNombreUsuario.Font = new Font("Segoe UI", 10F);
            txtNombreUsuario.Location = new Point(55, 217);
            txtNombreUsuario.Name = "txtNombreUsuario";
            txtNombreUsuario.PlaceholderText = "Ingrese el nombre de usuario";
            txtNombreUsuario.Size = new Size(400, 30);
            txtNombreUsuario.TabIndex = 1;
            txtNombreUsuario.TextChanged += txtNombreUsuario_TextChanged;

            // 
            // lblCorreo
            // 
            lblCorreo.AutoSize = true;
            lblCorreo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCorreo.Location = new Point(55, 260);
            lblCorreo.Name = "lblCorreo";
            lblCorreo.Size = new Size(61, 21);
            lblCorreo.TabIndex = 7;
            lblCorreo.Text = "Correo";

            // 
            // txtCorreo
            // 
            txtCorreo.Font = new Font("Segoe UI", 10F);
            txtCorreo.Location = new Point(55, 287);
            txtCorreo.Name = "txtCorreo";
            txtCorreo.PlaceholderText = "Ingrese el correo electrónico";
            txtCorreo.Size = new Size(400, 30);
            txtCorreo.TabIndex = 2;
            txtCorreo.TextChanged += txtCorreo_TextChanged;

            // 
            // lblRol
            // 
            lblRol.AutoSize = true;
            lblRol.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblRol.Location = new Point(55, 330);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(34, 21);
            lblRol.TabIndex = 9;
            lblRol.Text = "Rol";

            // 
            // cmbRol
            // 
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRol.Font = new Font("Segoe UI", 10F);
            cmbRol.FormattingEnabled = true;
            cmbRol.Location = new Point(55, 357);
            cmbRol.Name = "cmbRol";
            cmbRol.Size = new Size(400, 31);
            cmbRol.TabIndex = 3;

            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblPassword.Location = new Point(55, 400);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(96, 21);
            lblPassword.TabIndex = 11;
            lblPassword.Text = "Contraseña";

            // 
            // txtPassword
            // 
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.Location = new Point(55, 427);
            txtPassword.Name = "txtPassword";
            txtPassword.PlaceholderText = "Ingrese la contraseña";
            txtPassword.Size = new Size(400, 30);
            txtPassword.TabIndex = 4;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.TextChanged += txtPassword_TextChanged;

            // 
            // lblConfirmarPassword
            // 
            lblConfirmarPassword.AutoSize = true;
            lblConfirmarPassword.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblConfirmarPassword.Location = new Point(55, 470);
            lblConfirmarPassword.Name = "lblConfirmarPassword";
            lblConfirmarPassword.Size = new Size(175, 21);
            lblConfirmarPassword.TabIndex = 13;
            lblConfirmarPassword.Text = "Confirmar contraseña";

            // 
            // txtConfirmarPassword
            // 
            txtConfirmarPassword.Font = new Font("Segoe UI", 10F);
            txtConfirmarPassword.Location = new Point(55, 497);
            txtConfirmarPassword.Name = "txtConfirmarPassword";
            txtConfirmarPassword.PlaceholderText = "Confirme la contraseña";
            txtConfirmarPassword.Size = new Size(400, 30);
            txtConfirmarPassword.TabIndex = 5;
            txtConfirmarPassword.UseSystemPasswordChar = true;
            txtConfirmarPassword.TextChanged += txtConfirmarPassword_TextChanged;

            // 
            // btnCrearUsuario
            // 
            btnCrearUsuario.BackColor = Color.FromArgb(36, 80, 177);
            btnCrearUsuario.Cursor = Cursors.Hand;
            btnCrearUsuario.FlatAppearance.BorderSize = 0;
            btnCrearUsuario.FlatStyle = FlatStyle.Flat;
            btnCrearUsuario.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCrearUsuario.ForeColor = Color.White;
            btnCrearUsuario.Location = new Point(55, 560);
            btnCrearUsuario.Name = "btnCrearUsuario";
            btnCrearUsuario.Size = new Size(190, 48);
            btnCrearUsuario.TabIndex = 6;
            btnCrearUsuario.Text = "Crear usuario";
            btnCrearUsuario.UseVisualStyleBackColor = false;
            btnCrearUsuario.Click += btnCrearUsuario_Click;

            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.White;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderColor = Color.Gray;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9.5F);
            btnCancelar.ForeColor = Color.FromArgb(50, 50, 50);
            btnCancelar.Location = new Point(265, 560);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(190, 48);
            btnCancelar.TabIndex = 7;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;

            // 
            // CrearUsuario
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 248, 252);
            ClientSize = new Size(1000, 700);
            Controls.Add(pnlFormulario);
            Controls.Add(pnlLateral);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "CrearUsuario";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Crear Usuario - Sistema de Gestión de Citas";

            pnlLateral.ResumeLayout(false);
            pnlLateral.PerformLayout();

            pnlFormulario.ResumeLayout(false);
            pnlFormulario.PerformLayout();

            ResumeLayout(false);
        }

        #endregion

        private Panel pnlLateral;
        private Label lblNombreSistema;
        private Label lblSubtituloSistema;
        private Label lblMensajePrincipal;
        private Label lblDescripcionLateral;

        private Panel pnlFormulario;
        private Label lblIconoUsuario;
        private Label lblTitulo;
        private Label lblDescripcion;

        private Label lblNombreCompleto;
        private TextBox txtNombreCompleto;

        private Label lblNombreUsuario;
        private TextBox txtNombreUsuario;

        private Label lblCorreo;
        private TextBox txtCorreo;

        private Label lblRol;
        private ComboBox cmbRol;

        private Label lblPassword;
        private TextBox txtPassword;

        private Label lblConfirmarPassword;
        private TextBox txtConfirmarPassword;

        private Button btnCrearUsuario;
        private Button btnCancelar;
    }
}