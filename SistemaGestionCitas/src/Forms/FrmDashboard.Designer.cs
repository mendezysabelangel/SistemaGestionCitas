namespace SistemaGestionCitas.src.Forms
{
    partial class FrmDashboard
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
            pnlSidebar = new Panel();
            btnCerrarSesion = new Button();
            btnUsuarios = new Button();
            btnClientes = new Button();
            btnCitas = new Button();
            btnInicio = new Button();
            lblSistema = new Label();
            lblLogo = new Label();
            pnlTopbar = new Panel();
            lblRol = new Label();
            lblUsuario = new Label();
            lblSeccion = new Label();
            pnlContenido = new Panel();
            pnlConfirmadas = new Panel();
            lblConfirmadasNumero = new Label();
            lblConfirmadasTitulo = new Label();
            pnlPendientes = new Panel();
            lblPendientesNumero = new Label();
            lblPendientesTitulo = new Label();
            pnlCitasHoy = new Panel();
            lblCitasHoyNumero = new Label();
            lblCitasHoyTitulo = new Label();
            lblDescripcion = new Label();
            lblBienvenida = new Label();
            pnlSidebar.SuspendLayout();
            pnlTopbar.SuspendLayout();
            pnlContenido.SuspendLayout();
            pnlConfirmadas.SuspendLayout();
            pnlPendientes.SuspendLayout();
            pnlCitasHoy.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(248, 250, 253);
            pnlSidebar.Controls.Add(btnCerrarSesion);
            pnlSidebar.Controls.Add(btnUsuarios);
            pnlSidebar.Controls.Add(btnClientes);
            pnlSidebar.Controls.Add(btnCitas);
            pnlSidebar.Controls.Add(btnInicio);
            pnlSidebar.Controls.Add(lblSistema);
            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Margin = new Padding(4);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(312, 900);
            pnlSidebar.TabIndex = 0;
            // 
            // btnCerrarSesion
            // 
            btnCerrarSesion.FlatAppearance.BorderSize = 0;
            btnCerrarSesion.FlatStyle = FlatStyle.Flat;
            btnCerrarSesion.Font = new Font("Segoe UI", 10F);
            btnCerrarSesion.ForeColor = Color.FromArgb(90, 100, 115);
            btnCerrarSesion.Location = new Point(19, 812);
            btnCerrarSesion.Margin = new Padding(4);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Size = new Size(275, 56);
            btnCerrarSesion.TabIndex = 6;
            btnCerrarSesion.Text = "Cerrar sesión";
            btnCerrarSesion.TextAlign = ContentAlignment.MiddleLeft;
            btnCerrarSesion.Click += btnCerrarSesion_Click;
            // 
            // btnUsuarios
            // 
            btnUsuarios.FlatAppearance.BorderSize = 0;
            btnUsuarios.FlatStyle = FlatStyle.Flat;
            btnUsuarios.Font = new Font("Segoe UI", 10F);
            btnUsuarios.ForeColor = Color.FromArgb(70, 80, 95);
            btnUsuarios.Location = new Point(19, 356);
            btnUsuarios.Margin = new Padding(4);
            btnUsuarios.Name = "btnUsuarios";
            btnUsuarios.Size = new Size(275, 56);
            btnUsuarios.TabIndex = 5;
            btnUsuarios.Text = "Usuarios";
            btnUsuarios.TextAlign = ContentAlignment.MiddleLeft;
            btnUsuarios.Click += btnUsuarios_Click;
            // 
            // btnClientes
            // 
            btnClientes.FlatAppearance.BorderSize = 0;
            btnClientes.FlatStyle = FlatStyle.Flat;
            btnClientes.Font = new Font("Segoe UI", 10F);
            btnClientes.ForeColor = Color.FromArgb(70, 80, 95);
            btnClientes.Location = new Point(19, 288);
            btnClientes.Margin = new Padding(4);
            btnClientes.Name = "btnClientes";
            btnClientes.Size = new Size(275, 56);
            btnClientes.TabIndex = 4;
            btnClientes.Text = "Clientes";
            btnClientes.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnCitas
            // 
            btnCitas.FlatAppearance.BorderSize = 0;
            btnCitas.FlatStyle = FlatStyle.Flat;
            btnCitas.Font = new Font("Segoe UI", 10F);
            btnCitas.ForeColor = Color.FromArgb(70, 80, 95);
            btnCitas.Location = new Point(19, 219);
            btnCitas.Margin = new Padding(4);
            btnCitas.Name = "btnCitas";
            btnCitas.Size = new Size(275, 56);
            btnCitas.TabIndex = 3;
            btnCitas.Text = "Citas";
            btnCitas.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnInicio
            // 
            btnInicio.BackColor = Color.FromArgb(220, 232, 255);
            btnInicio.FlatAppearance.BorderSize = 0;
            btnInicio.FlatStyle = FlatStyle.Flat;
            btnInicio.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnInicio.ForeColor = Color.FromArgb(28, 69, 171);
            btnInicio.Location = new Point(19, 150);
            btnInicio.Margin = new Padding(4);
            btnInicio.Name = "btnInicio";
            btnInicio.Size = new Size(275, 56);
            btnInicio.TabIndex = 2;
            btnInicio.Text = "Inicio";
            btnInicio.TextAlign = ContentAlignment.MiddleLeft;
            btnInicio.UseVisualStyleBackColor = false;
            // 
            // lblSistema
            // 
            lblSistema.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSistema.ForeColor = Color.FromArgb(20, 27, 38);
            lblSistema.Location = new Point(96, 31);
            lblSistema.Margin = new Padding(4, 0, 4, 0);
            lblSistema.Name = "lblSistema";
            lblSistema.Size = new Size(205, 110);
            lblSistema.TabIndex = 1;
            lblSistema.Text = "Sistema de Gestión de Citas";
            lblSistema.Click += lblSistema_Click;
            // 
            // lblLogo
            // 
            lblLogo.BackColor = Color.FromArgb(28, 69, 171);
            lblLogo.Font = new Font("Segoe UI Emoji", 20F);
            lblLogo.ForeColor = Color.White;
            lblLogo.Location = new Point(19, 31);
            lblLogo.Margin = new Padding(4, 0, 4, 0);
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(69, 69);
            lblLogo.TabIndex = 0;
            lblLogo.Text = "📅";
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlTopbar
            // 
            pnlTopbar.BackColor = Color.White;
            pnlTopbar.Controls.Add(lblRol);
            pnlTopbar.Controls.Add(lblUsuario);
            pnlTopbar.Controls.Add(lblSeccion);
            pnlTopbar.Dock = DockStyle.Top;
            pnlTopbar.Location = new Point(312, 0);
            pnlTopbar.Margin = new Padding(4);
            pnlTopbar.Name = "pnlTopbar";
            pnlTopbar.Size = new Size(1188, 112);
            pnlTopbar.TabIndex = 1;
            // 
            // lblRol
            // 
            lblRol.Font = new Font("Segoe UI", 9F);
            lblRol.ForeColor = Color.FromArgb(96, 106, 120);
            lblRol.Location = new Point(900, 60);
            lblRol.Margin = new Padding(4, 0, 4, 0);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(238, 28);
            lblRol.TabIndex = 2;
            lblRol.Text = "Rol";
            // 
            // lblUsuario
            // 
            lblUsuario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsuario.ForeColor = Color.FromArgb(20, 27, 38);
            lblUsuario.Location = new Point(900, 25);
            lblUsuario.Margin = new Padding(4, 0, 4, 0);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(238, 31);
            lblUsuario.TabIndex = 1;
            lblUsuario.Text = "usuario";
            lblUsuario.Click += lblUsuario_Click;
            // 
            // lblSeccion
            // 
            lblSeccion.AutoSize = true;
            lblSeccion.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblSeccion.ForeColor = Color.FromArgb(20, 27, 38);
            lblSeccion.Location = new Point(50, 60);
            lblSeccion.Margin = new Padding(4, 0, 4, 0);
            lblSeccion.Name = "lblSeccion";
            lblSeccion.Size = new Size(109, 46);
            lblSeccion.TabIndex = 0;
            lblSeccion.Text = "Inicio";
            // 
            // pnlContenido
            // 
            pnlContenido.BackColor = Color.FromArgb(248, 250, 253);
            pnlContenido.Controls.Add(pnlConfirmadas);
            pnlContenido.Controls.Add(pnlPendientes);
            pnlContenido.Controls.Add(pnlCitasHoy);
            pnlContenido.Controls.Add(lblDescripcion);
            pnlContenido.Controls.Add(lblBienvenida);
            pnlContenido.Dock = DockStyle.Fill;
            pnlContenido.Location = new Point(312, 112);
            pnlContenido.Margin = new Padding(4);
            pnlContenido.Name = "pnlContenido";
            pnlContenido.Size = new Size(1188, 788);
            pnlContenido.TabIndex = 2;
            // 
            // pnlConfirmadas
            // 
            pnlConfirmadas.BackColor = Color.White;
            pnlConfirmadas.BorderStyle = BorderStyle.FixedSingle;
            pnlConfirmadas.Controls.Add(lblConfirmadasNumero);
            pnlConfirmadas.Controls.Add(lblConfirmadasTitulo);
            pnlConfirmadas.Location = new Point(750, 200);
            pnlConfirmadas.Margin = new Padding(4);
            pnlConfirmadas.Name = "pnlConfirmadas";
            pnlConfirmadas.Size = new Size(312, 174);
            pnlConfirmadas.TabIndex = 4;
            // 
            // lblConfirmadasNumero
            // 
            lblConfirmadasNumero.AutoSize = true;
            lblConfirmadasNumero.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            lblConfirmadasNumero.ForeColor = Color.FromArgb(20, 27, 38);
            lblConfirmadasNumero.Location = new Point(25, 69);
            lblConfirmadasNumero.Margin = new Padding(4, 0, 4, 0);
            lblConfirmadasNumero.Name = "lblConfirmadasNumero";
            lblConfirmadasNumero.Size = new Size(58, 67);
            lblConfirmadasNumero.TabIndex = 0;
            lblConfirmadasNumero.Text = "0";
            // 
            // lblConfirmadasTitulo
            // 
            lblConfirmadasTitulo.AutoSize = true;
            lblConfirmadasTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblConfirmadasTitulo.ForeColor = Color.FromArgb(96, 106, 120);
            lblConfirmadasTitulo.Location = new Point(25, 25);
            lblConfirmadasTitulo.Margin = new Padding(4, 0, 4, 0);
            lblConfirmadasTitulo.Name = "lblConfirmadasTitulo";
            lblConfirmadasTitulo.Size = new Size(131, 28);
            lblConfirmadasTitulo.TabIndex = 1;
            lblConfirmadasTitulo.Text = "Confirmadas";
            // 
            // pnlPendientes
            // 
            pnlPendientes.BackColor = Color.White;
            pnlPendientes.BorderStyle = BorderStyle.FixedSingle;
            pnlPendientes.Controls.Add(lblPendientesNumero);
            pnlPendientes.Controls.Add(lblPendientesTitulo);
            pnlPendientes.Location = new Point(400, 200);
            pnlPendientes.Margin = new Padding(4);
            pnlPendientes.Name = "pnlPendientes";
            pnlPendientes.Size = new Size(312, 174);
            pnlPendientes.TabIndex = 3;
            // 
            // lblPendientesNumero
            // 
            lblPendientesNumero.AutoSize = true;
            lblPendientesNumero.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            lblPendientesNumero.ForeColor = Color.FromArgb(20, 27, 38);
            lblPendientesNumero.Location = new Point(25, 69);
            lblPendientesNumero.Margin = new Padding(4, 0, 4, 0);
            lblPendientesNumero.Name = "lblPendientesNumero";
            lblPendientesNumero.Size = new Size(58, 67);
            lblPendientesNumero.TabIndex = 0;
            lblPendientesNumero.Text = "0";
            // 
            // lblPendientesTitulo
            // 
            lblPendientesTitulo.AutoSize = true;
            lblPendientesTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPendientesTitulo.ForeColor = Color.FromArgb(96, 106, 120);
            lblPendientesTitulo.Location = new Point(25, 25);
            lblPendientesTitulo.Margin = new Padding(4, 0, 4, 0);
            lblPendientesTitulo.Name = "lblPendientesTitulo";
            lblPendientesTitulo.Size = new Size(115, 28);
            lblPendientesTitulo.TabIndex = 1;
            lblPendientesTitulo.Text = "Pendientes";
            // 
            // pnlCitasHoy
            // 
            pnlCitasHoy.BackColor = Color.White;
            pnlCitasHoy.BorderStyle = BorderStyle.FixedSingle;
            pnlCitasHoy.Controls.Add(lblCitasHoyNumero);
            pnlCitasHoy.Controls.Add(lblCitasHoyTitulo);
            pnlCitasHoy.Location = new Point(50, 200);
            pnlCitasHoy.Margin = new Padding(4);
            pnlCitasHoy.Name = "pnlCitasHoy";
            pnlCitasHoy.Size = new Size(312, 174);
            pnlCitasHoy.TabIndex = 2;
            // 
            // lblCitasHoyNumero
            // 
            lblCitasHoyNumero.AutoSize = true;
            lblCitasHoyNumero.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            lblCitasHoyNumero.ForeColor = Color.FromArgb(20, 27, 38);
            lblCitasHoyNumero.Location = new Point(25, 69);
            lblCitasHoyNumero.Margin = new Padding(4, 0, 4, 0);
            lblCitasHoyNumero.Name = "lblCitasHoyNumero";
            lblCitasHoyNumero.Size = new Size(58, 67);
            lblCitasHoyNumero.TabIndex = 0;
            lblCitasHoyNumero.Text = "0";
            // 
            // lblCitasHoyTitulo
            // 
            lblCitasHoyTitulo.AutoSize = true;
            lblCitasHoyTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCitasHoyTitulo.ForeColor = Color.FromArgb(96, 106, 120);
            lblCitasHoyTitulo.Location = new Point(25, 25);
            lblCitasHoyTitulo.Margin = new Padding(4, 0, 4, 0);
            lblCitasHoyTitulo.Name = "lblCitasHoyTitulo";
            lblCitasHoyTitulo.Size = new Size(128, 28);
            lblCitasHoyTitulo.TabIndex = 1;
            lblCitasHoyTitulo.Text = "Citas de hoy";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Font = new Font("Segoe UI", 11F);
            lblDescripcion.ForeColor = Color.FromArgb(96, 106, 120);
            lblDescripcion.Location = new Point(52, 119);
            lblDescripcion.Margin = new Padding(4, 0, 4, 0);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(431, 30);
            lblDescripcion.TabIndex = 1;
            lblDescripcion.Text = "Aquí tienes un resumen de las operaciones.";
            // 
            // lblBienvenida
            // 
            lblBienvenida.AutoSize = true;
            lblBienvenida.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblBienvenida.ForeColor = Color.FromArgb(20, 27, 38);
            lblBienvenida.Location = new Point(50, 50);
            lblBienvenida.Margin = new Padding(4, 0, 4, 0);
            lblBienvenida.Name = "lblBienvenida";
            lblBienvenida.Size = new Size(259, 60);
            lblBienvenida.TabIndex = 0;
            lblBienvenida.Text = "Bienvenido";
            // 
            // FrmDashboard
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 253);
            ClientSize = new Size(1500, 900);
            Controls.Add(pnlContenido);
            Controls.Add(pnlTopbar);
            Controls.Add(pnlSidebar);
            Margin = new Padding(4);
            MinimumSize = new Size(1370, 798);
            Name = "FrmDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sistema de Gestión de Citas";
            pnlSidebar.ResumeLayout(false);
            pnlTopbar.ResumeLayout(false);
            pnlTopbar.PerformLayout();
            pnlContenido.ResumeLayout(false);
            pnlContenido.PerformLayout();
            pnlConfirmadas.ResumeLayout(false);
            pnlConfirmadas.PerformLayout();
            pnlPendientes.ResumeLayout(false);
            pnlPendientes.PerformLayout();
            pnlCitasHoy.ResumeLayout(false);
            pnlCitasHoy.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSidebar;
        private Label lblLogo;
        private Label lblSistema;
        private Button btnInicio;
        private Button btnCitas;
        private Button btnClientes;
        private Button btnUsuarios;
        private Button btnCerrarSesion;

        private Panel pnlTopbar;
        private Label lblSeccion;
        private Label lblUsuario;
        private Label lblRol;

        private Panel pnlContenido;
        private Label lblBienvenida;
        private Label lblDescripcion;

        private Panel pnlCitasHoy;
        private Label lblCitasHoyTitulo;
        private Label lblCitasHoyNumero;

        private Panel pnlPendientes;
        private Label lblPendientesTitulo;
        private Label lblPendientesNumero;

        private Panel pnlConfirmadas;
        private Label lblConfirmadasTitulo;
        private Label lblConfirmadasNumero;
    }
}