namespace SistemaGestionCitas.src.Forms
{
    partial class CrearUsuario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNombreSistema = new Label();
            lblSubtituloSistema = new Label();
            lblMensajePrincipal = new Label();
            lblDescripcionLateral = new Label();
            lblTitulo = new Label();
            lblDescripcion = new Label();
            lblNombreCompleto = new Label();
            txtNombreCompleto = new TextBox();
            txtNombreUsuario = new TextBox();
            lblNombreUsuario = new Label();
            txtCorreo = new TextBox();
            lblCorreo = new Label();
            txtPassword = new TextBox();
            lblPassword = new Label();
            txtConfirmarPassword = new TextBox();
            lblConfirmarPassword = new Label();
            btnCrearUsuario = new Button();
            btnCancelar = new Button();
            SuspendLayout();
            // 
            // lblNombreSistema
            // 
            lblNombreSistema.AutoSize = true;
            lblNombreSistema.Location = new Point(25, 29);
            lblNombreSistema.Name = "lblNombreSistema";
            lblNombreSistema.Size = new Size(136, 20);
            lblNombreSistema.TabIndex = 0;
            lblNombreSistema.Text = "Sistema de Gestión";
            lblNombreSistema.Click += label1_Click;
            // 
            // lblSubtituloSistema
            // 
            lblSubtituloSistema.AutoSize = true;
            lblSubtituloSistema.Location = new Point(25, 58);
            lblSubtituloSistema.Name = "lblSubtituloSistema";
            lblSubtituloSistema.Size = new Size(164, 20);
            lblSubtituloSistema.TabIndex = 1;
            lblSubtituloSistema.Text = "Administración de citas";
            // 
            // lblMensajePrincipal
            // 
            lblMensajePrincipal.AutoSize = true;
            lblMensajePrincipal.Location = new Point(25, 102);
            lblMensajePrincipal.Name = "lblMensajePrincipal";
            lblMensajePrincipal.Size = new Size(295, 20);
            lblMensajePrincipal.TabIndex = 2;
            lblMensajePrincipal.Text = "Crea y administra usuarios de forma segura";
            lblMensajePrincipal.Click += label1_Click_1;
            // 
            // lblDescripcionLateral
            // 
            lblDescripcionLateral.Location = new Point(25, 378);
            lblDescripcionLateral.Name = "lblDescripcionLateral";
            lblDescripcionLateral.Size = new Size(295, 77);
            lblDescripcionLateral.TabIndex = 3;
            lblDescripcionLateral.Text = "Gestiona el acceso al sistema y mantén un control seguro de tu equipo de trabajo.";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(504, 40);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(96, 20);
            lblTitulo.TabIndex = 4;
            lblTitulo.Text = "Crear usuario";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(504, 70);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(289, 20);
            lblDescripcion.TabIndex = 5;
            lblDescripcion.Text = "Registra una nueva cuenta para el sistema.";
            // 
            // lblNombreCompleto
            // 
            lblNombreCompleto.AutoSize = true;
            lblNombreCompleto.Location = new Point(504, 102);
            lblNombreCompleto.Name = "lblNombreCompleto";
            lblNombreCompleto.Size = new Size(132, 20);
            lblNombreCompleto.TabIndex = 6;
            lblNombreCompleto.Text = "Nombre completo";
            lblNombreCompleto.Click += lblNombreCompleto_Click;
            // 
            // txtNombreCompleto
            // 
            txtNombreCompleto.ForeColor = SystemColors.ScrollBar;
            txtNombreCompleto.Location = new Point(508, 133);
            txtNombreCompleto.Name = "txtNombreCompleto";
            txtNombreCompleto.Size = new Size(285, 27);
            txtNombreCompleto.TabIndex = 7;
            txtNombreCompleto.Text = "Ingrese el nombre completo";
            // 
            // txtNombreUsuario
            // 
            txtNombreUsuario.ForeColor = SystemColors.ScrollBar;
            txtNombreUsuario.Location = new Point(508, 203);
            txtNombreUsuario.Name = "txtNombreUsuario";
            txtNombreUsuario.Size = new Size(285, 27);
            txtNombreUsuario.TabIndex = 9;
            txtNombreUsuario.Text = "Ingrese el nombre de usuario";
            // 
            // lblNombreUsuario
            // 
            lblNombreUsuario.AutoSize = true;
            lblNombreUsuario.Location = new Point(504, 172);
            lblNombreUsuario.Name = "lblNombreUsuario";
            lblNombreUsuario.Size = new Size(137, 20);
            lblNombreUsuario.TabIndex = 8;
            lblNombreUsuario.Text = "Nombre de usuario";
            lblNombreUsuario.Click += label1_Click_2;
            // 
            // txtCorreo
            // 
            txtCorreo.ForeColor = SystemColors.ScrollBar;
            txtCorreo.Location = new Point(508, 279);
            txtCorreo.Name = "txtCorreo";
            txtCorreo.Size = new Size(285, 27);
            txtCorreo.TabIndex = 11;
            txtCorreo.Text = "Ingrese su correo electronico";
            // 
            // lblCorreo
            // 
            lblCorreo.AutoSize = true;
            lblCorreo.Location = new Point(504, 248);
            lblCorreo.Name = "lblCorreo";
            lblCorreo.Size = new Size(132, 20);
            lblCorreo.TabIndex = 10;
            lblCorreo.Text = "Correo electronico";
            lblCorreo.Click += lblCorreo_Click;
            // 
            // txtPassword
            // 
            txtPassword.ForeColor = SystemColors.ScrollBar;
            txtPassword.Location = new Point(508, 353);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(285, 27);
            txtPassword.TabIndex = 13;
            txtPassword.Text = "Ingrese su contraseña";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(504, 322);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(83, 20);
            lblPassword.TabIndex = 12;
            lblPassword.Text = "Contraseña";
            // 
            // txtConfirmarPassword
            // 
            txtConfirmarPassword.ForeColor = SystemColors.ScrollBar;
            txtConfirmarPassword.Location = new Point(504, 428);
            txtConfirmarPassword.Name = "txtConfirmarPassword";
            txtConfirmarPassword.Size = new Size(285, 27);
            txtConfirmarPassword.TabIndex = 15;
            txtConfirmarPassword.Text = "Ingrese su contraseña";
            // 
            // lblConfirmarPassword
            // 
            lblConfirmarPassword.AutoSize = true;
            lblConfirmarPassword.Location = new Point(500, 397);
            lblConfirmarPassword.Name = "lblConfirmarPassword";
            lblConfirmarPassword.Size = new Size(151, 20);
            lblConfirmarPassword.TabIndex = 14;
            lblConfirmarPassword.Text = "Confirmar contraseña";
            // 
            // btnCrearUsuario
            // 
            btnCrearUsuario.Location = new Point(495, 474);
            btnCrearUsuario.Name = "btnCrearUsuario";
            btnCrearUsuario.Size = new Size(141, 29);
            btnCrearUsuario.TabIndex = 16;
            btnCrearUsuario.Text = "Crear usuario";
            btnCrearUsuario.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(650, 474);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(139, 29);
            btnCancelar.TabIndex = 17;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            // 
            // CrearUsuario
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(946, 618);
            Controls.Add(btnCancelar);
            Controls.Add(btnCrearUsuario);
            Controls.Add(txtConfirmarPassword);
            Controls.Add(lblConfirmarPassword);
            Controls.Add(txtPassword);
            Controls.Add(lblPassword);
            Controls.Add(txtCorreo);
            Controls.Add(lblCorreo);
            Controls.Add(txtNombreUsuario);
            Controls.Add(lblNombreUsuario);
            Controls.Add(txtNombreCompleto);
            Controls.Add(lblNombreCompleto);
            Controls.Add(lblDescripcion);
            Controls.Add(lblTitulo);
            Controls.Add(lblDescripcionLateral);
            Controls.Add(lblMensajePrincipal);
            Controls.Add(lblSubtituloSistema);
            Controls.Add(lblNombreSistema);
            Name = "CrearUsuario";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Crear usuario";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNombreSistema;
        private Label lblSubtituloSistema;
        private Label lblMensajePrincipal;
        private Label lblDescripcionLateral;
        private Label lblTitulo;
        private Label lblDescripcion;
        private Label lblNombreCompleto;
        private TextBox txtNombreCompleto;
        private TextBox txtNombreUsuario;
        private Label lblNombreUsuario;
        private TextBox txtCorreo;
        private Label lblCorreo;
        private TextBox txtPassword;
        private Label lblPassword;
        private TextBox txtConfirmarPassword;
        private Label lblConfirmarPassword;
        private Button btnCrearUsuario;
        private Button btnCancelar;
    }
}