namespace SistemaGestionCitas.src.Forms
{
    partial class FrmLogin
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
            pnlBrand = new Panel();
            lblHeroDescription = new Label();
            lblHeroTitle = new Label();
            lblBrandSubtitle = new Label();
            lblBrandTitle = new Label();
            pnlLoginArea = new Panel();
            pnlLoginCard = new Panel();
            btnSalir = new Button();
            btnIniciarSesion = new Button();
            btnTogglePassword = new Button();
            txtContrasena = new TextBox();
            lblContrasena = new Label();
            txtUsuario = new TextBox();
            lblUsuario = new Label();
            lblDescripcion = new Label();
            lblTitulo = new Label();
            lblLockIcon = new Label();
            pnlBrand.SuspendLayout();
            pnlLoginArea.SuspendLayout();
            pnlLoginCard.SuspendLayout();
            SuspendLayout();
            // 
            // pnlBrand
            // 
            pnlBrand.BackColor = Color.FromArgb(28, 69, 171);
            pnlBrand.Controls.Add(lblHeroDescription);
            pnlBrand.Controls.Add(lblHeroTitle);
            pnlBrand.Controls.Add(lblBrandSubtitle);
            pnlBrand.Controls.Add(lblBrandTitle);
            pnlBrand.Dock = DockStyle.Left;
            pnlBrand.Location = new Point(0, 0);
            pnlBrand.Margin = new Padding(4, 5, 4, 5);
            pnlBrand.Name = "pnlBrand";
            pnlBrand.Size = new Size(571, 1083);
            pnlBrand.TabIndex = 0;
            // 
            // lblHeroDescription
            // 
            lblHeroDescription.Font = new Font("Segoe UI", 10.5F);
            lblHeroDescription.ForeColor = Color.FromArgb(208, 219, 246);
            lblHeroDescription.Location = new Point(64, 608);
            lblHeroDescription.Margin = new Padding(4, 0, 4, 0);
            lblHeroDescription.Name = "lblHeroDescription";
            lblHeroDescription.Size = new Size(429, 133);
            lblHeroDescription.TabIndex = 3;
            lblHeroDescription.Text = "Administra citas, clientes, empleados y servicios con una estructura clara y segura.";
            lblHeroDescription.Click += lblHeroDescription_Click;
            // 
            // lblHeroTitle
            // 
            lblHeroTitle.Font = new Font("Segoe UI", 23F, FontStyle.Bold);
            lblHeroTitle.ForeColor = Color.White;
            lblHeroTitle.Location = new Point(43, 280);
            lblHeroTitle.Margin = new Padding(4, 0, 4, 0);
            lblHeroTitle.Name = "lblHeroTitle";
            lblHeroTitle.Size = new Size(500, 356);
            lblHeroTitle.TabIndex = 2;
            lblHeroTitle.Text = "Organiza las operaciones diarias desde un solo lugar";
            // 
            // lblBrandSubtitle
            // 
            lblBrandSubtitle.AutoSize = true;
            lblBrandSubtitle.Font = new Font("Segoe UI", 10F);
            lblBrandSubtitle.ForeColor = Color.FromArgb(205, 218, 250);
            lblBrandSubtitle.Location = new Point(63, 142);
            lblBrandSubtitle.Margin = new Padding(4, 0, 4, 0);
            lblBrandSubtitle.Name = "lblBrandSubtitle";
            lblBrandSubtitle.Size = new Size(215, 28);
            lblBrandSubtitle.TabIndex = 1;
            lblBrandSubtitle.Text = "Administración de citas";
            // 
            // lblBrandTitle
            // 
            lblBrandTitle.AutoSize = true;
            lblBrandTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblBrandTitle.ForeColor = Color.White;
            lblBrandTitle.Location = new Point(60, 87);
            lblBrandTitle.Margin = new Padding(4, 0, 4, 0);
            lblBrandTitle.Name = "lblBrandTitle";
            lblBrandTitle.Size = new Size(286, 41);
            lblBrandTitle.TabIndex = 0;
            lblBrandTitle.Text = "Sistema de Gestión";
            // 
            // pnlLoginArea
            // 
            pnlLoginArea.BackColor = Color.FromArgb(248, 250, 253);
            pnlLoginArea.Controls.Add(pnlLoginCard);
            pnlLoginArea.Dock = DockStyle.Fill;
            pnlLoginArea.Location = new Point(571, 0);
            pnlLoginArea.Margin = new Padding(4, 5, 4, 5);
            pnlLoginArea.Name = "pnlLoginArea";
            pnlLoginArea.Size = new Size(858, 1083);
            pnlLoginArea.TabIndex = 1;
            pnlLoginArea.Paint += pnlLoginArea_Paint;
            // 
            // pnlLoginCard
            // 
            pnlLoginCard.BackColor = Color.White;
            pnlLoginCard.BorderStyle = BorderStyle.FixedSingle;
            pnlLoginCard.Controls.Add(btnSalir);
            pnlLoginCard.Controls.Add(btnIniciarSesion);
            pnlLoginCard.Controls.Add(btnTogglePassword);
            pnlLoginCard.Controls.Add(txtContrasena);
            pnlLoginCard.Controls.Add(lblContrasena);
            pnlLoginCard.Controls.Add(txtUsuario);
            pnlLoginCard.Controls.Add(lblUsuario);
            pnlLoginCard.Controls.Add(lblDescripcion);
            pnlLoginCard.Controls.Add(lblTitulo);
            pnlLoginCard.Controls.Add(lblLockIcon);
            pnlLoginCard.Location = new Point(121, 158);
            pnlLoginCard.Margin = new Padding(4, 5, 4, 5);
            pnlLoginCard.Name = "pnlLoginCard";
            pnlLoginCard.Size = new Size(613, 749);
            pnlLoginCard.TabIndex = 0;
            pnlLoginCard.Paint += pnlLoginCard_Paint;
            // 
            // btnSalir
            // 
            btnSalir.BackColor = Color.White;
            btnSalir.FlatStyle = FlatStyle.Flat;
            btnSalir.Font = new Font("Segoe UI", 9.5F);
            btnSalir.ForeColor = Color.FromArgb(35, 42, 52);
            btnSalir.Location = new Point(321, 590);
            btnSalir.Margin = new Padding(4, 5, 4, 5);
            btnSalir.Name = "btnSalir";
            btnSalir.Size = new Size(236, 70);
            btnSalir.TabIndex = 4;
            btnSalir.Text = "Salir";
            btnSalir.UseVisualStyleBackColor = false;
            btnSalir.Click += btnSalir_Click;
            // 
            // btnIniciarSesion
            // 
            btnIniciarSesion.BackColor = Color.FromArgb(28, 69, 171);
            btnIniciarSesion.FlatAppearance.BorderSize = 0;
            btnIniciarSesion.FlatStyle = FlatStyle.Flat;
            btnIniciarSesion.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnIniciarSesion.ForeColor = Color.White;
            btnIniciarSesion.Location = new Point(57, 590);
            btnIniciarSesion.Margin = new Padding(4, 5, 4, 5);
            btnIniciarSesion.Name = "btnIniciarSesion";
            btnIniciarSesion.Size = new Size(236, 70);
            btnIniciarSesion.TabIndex = 3;
            btnIniciarSesion.Text = "Iniciar sesión";
            btnIniciarSesion.UseVisualStyleBackColor = false;
            btnIniciarSesion.Click += btnIniciarSesion_Click;
            // 
            // btnTogglePassword
            // 
            btnTogglePassword.BackColor = Color.White;
            btnTogglePassword.FlatStyle = FlatStyle.Flat;
            btnTogglePassword.Font = new Font("Segoe UI", 8.5F);
            btnTogglePassword.ForeColor = Color.FromArgb(65, 75, 90);
            btnTogglePassword.Location = new Point(457, 473);
            btnTogglePassword.Margin = new Padding(4, 5, 4, 5);
            btnTogglePassword.Name = "btnTogglePassword";
            btnTogglePassword.Size = new Size(100, 48);
            btnTogglePassword.TabIndex = 2;
            btnTogglePassword.Text = "Ver";
            btnTogglePassword.UseVisualStyleBackColor = false;
            btnTogglePassword.Click += btnTogglePassword_Click;
            // 
            // txtContrasena
            // 
            txtContrasena.BorderStyle = BorderStyle.FixedSingle;
            txtContrasena.Font = new Font("Segoe UI", 11F);
            txtContrasena.Location = new Point(57, 475);
            txtContrasena.Margin = new Padding(4, 5, 4, 5);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.PlaceholderText = "Ingrese su contraseña";
            txtContrasena.Size = new Size(392, 37);
            txtContrasena.TabIndex = 1;
            txtContrasena.UseSystemPasswordChar = true;
            // 
            // lblContrasena
            // 
            lblContrasena.AutoSize = true;
            lblContrasena.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblContrasena.ForeColor = Color.FromArgb(35, 42, 52);
            lblContrasena.Location = new Point(57, 435);
            lblContrasena.Margin = new Padding(4, 0, 4, 0);
            lblContrasena.Name = "lblContrasena";
            lblContrasena.Size = new Size(113, 25);
            lblContrasena.TabIndex = 5;
            lblContrasena.Text = "Contraseña";
            // 
            // txtUsuario
            // 
            txtUsuario.BorderStyle = BorderStyle.FixedSingle;
            txtUsuario.Font = new Font("Segoe UI", 11F);
            txtUsuario.Location = new Point(57, 352);
            txtUsuario.Margin = new Padding(4, 5, 4, 5);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.PlaceholderText = "Ingrese su usuario";
            txtUsuario.Size = new Size(499, 37);
            txtUsuario.TabIndex = 0;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblUsuario.ForeColor = Color.FromArgb(35, 42, 52);
            lblUsuario.Location = new Point(57, 312);
            lblUsuario.Margin = new Padding(4, 0, 4, 0);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(81, 25);
            lblUsuario.TabIndex = 3;
            lblUsuario.Text = "Usuario";
            // 
            // lblDescripcion
            // 
            lblDescripcion.Font = new Font("Segoe UI", 9.5F);
            lblDescripcion.ForeColor = Color.FromArgb(96, 106, 120);
            lblDescripcion.Location = new Point(57, 220);
            lblDescripcion.Margin = new Padding(4, 0, 4, 0);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(500, 70);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Accede para administrar las operaciones del sistema.";
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(20, 27, 38);
            lblTitulo.Location = new Point(208, 130);
            lblTitulo.Margin = new Padding(4, 0, 4, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(267, 54);
            lblTitulo.TabIndex = 1;
            lblTitulo.Text = "Iniciar sesión";
            // 
            // lblLockIcon
            // 
            lblLockIcon.BackColor = Color.FromArgb(232, 238, 253);
            lblLockIcon.Font = new Font("Segoe UI Emoji", 17F);
            lblLockIcon.ForeColor = Color.FromArgb(28, 69, 171);
            lblLockIcon.Location = new Point(108, 121);
            lblLockIcon.Margin = new Padding(4, 0, 4, 0);
            lblLockIcon.Name = "lblLockIcon";
            lblLockIcon.Size = new Size(66, 77);
            lblLockIcon.TabIndex = 0;
            lblLockIcon.Text = "🔒";
            lblLockIcon.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FrmLogin
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 253);
            ClientSize = new Size(1429, 1083);
            Controls.Add(pnlLoginArea);
            Controls.Add(pnlBrand);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Name = "FrmLogin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sistema de Gestión de Citas";
            pnlBrand.ResumeLayout(false);
            pnlBrand.PerformLayout();
            pnlLoginArea.ResumeLayout(false);
            pnlLoginCard.ResumeLayout(false);
            pnlLoginCard.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlBrand;

        private Label lblBrandTitle;
        private Label lblBrandSubtitle;
        private Label lblHeroTitle;
        private Label lblHeroDescription;

        private Panel pnlLoginArea;
        private Panel pnlLoginCard;

        private Label lblLockIcon;
        private Label lblTitulo;
        private Label lblDescripcion;

        private Label lblUsuario;
        private TextBox txtUsuario;

        private Label lblContrasena;
        private TextBox txtContrasena;

        private Button btnTogglePassword;
        private Button btnIniciarSesion;
        private Button btnSalir;
    }
}