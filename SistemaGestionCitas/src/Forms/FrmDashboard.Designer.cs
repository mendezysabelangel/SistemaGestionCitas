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
            btnUsuario = new Button();
            btnCerrarSesion = new Button();
            btnCrearUsuario = new Button();
            btnCrearRol = new Button();
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
            pnlPermisos = new Panel();
            lblPermisos = new Label();
            btnConsultar = new Button();
            btnAgregar = new Button();
            btnModificar = new Button();
            btnEliminar = new Button();
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
            pnlSidebar.Controls.Add(btnUsuario);
            pnlSidebar.Controls.Add(btnCerrarSesion);
            pnlSidebar.Controls.Add(btnCrearUsuario);
            pnlSidebar.Controls.Add(btnCrearRol);
            pnlSidebar.Controls.Add(btnClientes);
            pnlSidebar.Controls.Add(btnCitas);
            pnlSidebar.Controls.Add(btnInicio);
            pnlSidebar.Controls.Add(lblSistema);
            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Margin = new Padding(3, 2, 3, 2);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(218, 449);
            pnlSidebar.TabIndex = 0;
            // 
            // btnUsuario
            // 
            btnUsuario.FlatAppearance.BorderSize = 0;
            btnUsuario.FlatStyle = FlatStyle.Flat;
            btnUsuario.Font = new Font("Segoe UI", 10F);
            btnUsuario.ForeColor = Color.FromArgb(70, 80, 95);
            btnUsuario.Location = new Point(13, 218);
            btnUsuario.Margin = new Padding(3, 2, 3, 2);
            btnUsuario.Name = "btnUsuario";
            btnUsuario.Size = new Size(192, 34);
            btnUsuario.TabIndex = 7;
            btnUsuario.Text = "Usuarios";
            btnUsuario.TextAlign = ContentAlignment.MiddleLeft;
            btnUsuario.Click += btnUsuarios_Click;
            // 
            // btnCerrarSesion
            // 
            btnCerrarSesion.FlatAppearance.BorderSize = 0;
            btnCerrarSesion.FlatStyle = FlatStyle.Flat;
            btnCerrarSesion.Font = new Font("Segoe UI", 10F);
            btnCerrarSesion.ForeColor = Color.FromArgb(90, 100, 115);
            btnCerrarSesion.Location = new Point(13, 487);
            btnCerrarSesion.Margin = new Padding(3, 2, 3, 2);
            btnCerrarSesion.Name = "btnCerrarSesion";
            btnCerrarSesion.Size = new Size(192, 34);
            btnCerrarSesion.TabIndex = 6;
            btnCerrarSesion.Text = "Cerrar sesión";
            btnCerrarSesion.TextAlign = ContentAlignment.MiddleLeft;
            btnCerrarSesion.Click += btnCerrarSesion_Click;
            // 
            // btnCrearUsuario
            // 
            btnCrearUsuario.FlatAppearance.BorderSize = 0;
            btnCrearUsuario.FlatStyle = FlatStyle.Flat;
            btnCrearUsuario.Font = new Font("Segoe UI", 10F);
            btnCrearUsuario.ForeColor = Color.FromArgb(70, 80, 95);
            btnCrearUsuario.Location = new Point(13, 262);
            btnCrearUsuario.Margin = new Padding(3, 2, 3, 2);
            btnCrearUsuario.Name = "btnCrearUsuario";
            btnCrearUsuario.Size = new Size(192, 34);
            btnCrearUsuario.TabIndex = 5;
            btnCrearUsuario.Text = "Crear nuevo usuario";
            btnCrearUsuario.TextAlign = ContentAlignment.MiddleLeft;
            btnCrearUsuario.Click += btnCrearUsuario_Click;

            // 
            // btnCrearRol
            // 
            btnCrearRol.FlatAppearance.BorderSize = 0;
            btnCrearRol.FlatStyle = FlatStyle.Flat;
            btnCrearRol.Font = new Font("Segoe UI", 10F);
            btnCrearRol.ForeColor = Color.FromArgb(70, 80, 95);
            btnCrearRol.Location = new Point(13, 306);
            btnCrearRol.Margin = new Padding(3, 2, 3, 2);
            btnCrearRol.Name = "btnCrearRol";
            btnCrearRol.Size = new Size(192, 34);
            btnCrearRol.TabIndex = 8;
            btnCrearRol.Text = "Crear nuevo rol";
            btnCrearRol.TextAlign = ContentAlignment.MiddleLeft;
            btnCrearRol.Click += btnCrearRol_Click;

            // 
            // btnClientes
            // 
            btnClientes.FlatAppearance.BorderSize = 0;
            btnClientes.FlatStyle = FlatStyle.Flat;
            btnClientes.Font = new Font("Segoe UI", 10F);
            btnClientes.ForeColor = Color.FromArgb(70, 80, 95);
            btnClientes.Location = new Point(13, 173);
            btnClientes.Margin = new Padding(3, 2, 3, 2);
            btnClientes.Name = "btnClientes";
            btnClientes.Size = new Size(192, 34);
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
            btnCitas.Location = new Point(13, 131);
            btnCitas.Margin = new Padding(3, 2, 3, 2);
            btnCitas.Name = "btnCitas";
            btnCitas.Size = new Size(192, 34);
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
            btnInicio.Location = new Point(13, 90);
            btnInicio.Margin = new Padding(3, 2, 3, 2);
            btnInicio.Name = "btnInicio";
            btnInicio.Size = new Size(192, 34);
            btnInicio.TabIndex = 2;
            btnInicio.Text = "Inicio";
            btnInicio.TextAlign = ContentAlignment.MiddleLeft;
            btnInicio.UseVisualStyleBackColor = false;
            // 
            // lblSistema
            // 
            lblSistema.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSistema.ForeColor = Color.FromArgb(20, 27, 38);
            lblSistema.Location = new Point(67, 19);
            lblSistema.Name = "lblSistema";
            lblSistema.Size = new Size(144, 66);
            lblSistema.TabIndex = 1;
            lblSistema.Text = "Sistema de Gestión de Citas";
            lblSistema.Click += lblSistema_Click;
            // 
            // lblLogo
            // 
            lblLogo.BackColor = Color.FromArgb(28, 69, 171);
            lblLogo.Font = new Font("Segoe UI Emoji", 20F);
            lblLogo.ForeColor = Color.White;
            lblLogo.Location = new Point(13, 19);
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(48, 41);
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
            pnlTopbar.Location = new Point(218, 0);
            pnlTopbar.Margin = new Padding(3, 2, 3, 2);
            pnlTopbar.Name = "pnlTopbar";
            pnlTopbar.Size = new Size(741, 67);
            pnlTopbar.TabIndex = 1;
            pnlTopbar.Paint += pnlTopbar_Paint;
            // 
            // lblRol
            // 
            lblRol.Font = new Font("Segoe UI", 9F);
            lblRol.ForeColor = Color.FromArgb(96, 106, 120);
            lblRol.Location = new Point(630, 36);
            lblRol.Name = "lblRol";
            lblRol.Size = new Size(167, 17);
            lblRol.TabIndex = 2;
            lblRol.Text = "Rol";
            // 
            // lblUsuario
            // 
            lblUsuario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsuario.ForeColor = Color.FromArgb(20, 27, 38);
            lblUsuario.Location = new Point(630, 15);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(167, 19);
            lblUsuario.TabIndex = 1;
            lblUsuario.Text = "usuario";
            lblUsuario.Click += lblUsuario_Click;
            // 
            // lblSeccion
            // 
            lblSeccion.AutoSize = true;
            lblSeccion.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
            lblSeccion.ForeColor = Color.FromArgb(20, 27, 38);
            lblSeccion.Location = new Point(35, 36);
            lblSeccion.Name = "lblSeccion";
            lblSeccion.Size = new Size(74, 31);
            lblSeccion.TabIndex = 0;
            lblSeccion.Text = "Inicio";
            // 
            // pnlContenido
            // 
            pnlContenido.BackColor = Color.FromArgb(248, 250, 253);
            pnlContenido.Controls.Add(pnlConfirmadas);
            pnlContenido.Controls.Add(pnlPendientes);
            pnlContenido.Controls.Add(pnlCitasHoy);
            pnlContenido.Controls.Add(pnlPermisos);
            pnlContenido.Controls.Add(lblDescripcion);
            pnlContenido.Controls.Add(lblBienvenida);
            pnlContenido.Dock = DockStyle.Fill;
            pnlContenido.Location = new Point(218, 67);
            pnlContenido.Margin = new Padding(3, 2, 3, 2);
            pnlContenido.Name = "pnlContenido";
            pnlContenido.Size = new Size(741, 382);
            pnlContenido.TabIndex = 2;
            // 
            // pnlConfirmadas
            // 
            pnlConfirmadas.BackColor = Color.White;
            pnlConfirmadas.BorderStyle = BorderStyle.FixedSingle;
            pnlConfirmadas.Controls.Add(lblConfirmadasNumero);
            pnlConfirmadas.Controls.Add(lblConfirmadasTitulo);
            pnlConfirmadas.Location = new Point(525, 120);
            pnlConfirmadas.Margin = new Padding(3, 2, 3, 2);
            pnlConfirmadas.Name = "pnlConfirmadas";
            pnlConfirmadas.Size = new Size(219, 105);
            pnlConfirmadas.TabIndex = 4;
            // 
            // lblConfirmadasNumero
            // 
            lblConfirmadasNumero.AutoSize = true;
            lblConfirmadasNumero.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            lblConfirmadasNumero.ForeColor = Color.FromArgb(20, 27, 38);
            lblConfirmadasNumero.Location = new Point(18, 41);
            lblConfirmadasNumero.Name = "lblConfirmadasNumero";
            lblConfirmadasNumero.Size = new Size(40, 46);
            lblConfirmadasNumero.TabIndex = 0;
            lblConfirmadasNumero.Text = "0";
            // 
            // lblConfirmadasTitulo
            // 
            lblConfirmadasTitulo.AutoSize = true;
            lblConfirmadasTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblConfirmadasTitulo.ForeColor = Color.FromArgb(96, 106, 120);
            lblConfirmadasTitulo.Location = new Point(18, 15);
            lblConfirmadasTitulo.Name = "lblConfirmadasTitulo";
            lblConfirmadasTitulo.Size = new Size(94, 19);
            lblConfirmadasTitulo.TabIndex = 1;
            lblConfirmadasTitulo.Text = "Confirmadas";
            // 
            // pnlPendientes
            // 
            pnlPendientes.BackColor = Color.White;
            pnlPendientes.BorderStyle = BorderStyle.FixedSingle;
            pnlPendientes.Controls.Add(lblPendientesNumero);
            pnlPendientes.Controls.Add(lblPendientesTitulo);
            pnlPendientes.Location = new Point(280, 120);
            pnlPendientes.Margin = new Padding(3, 2, 3, 2);
            pnlPendientes.Name = "pnlPendientes";
            pnlPendientes.Size = new Size(219, 105);
            pnlPendientes.TabIndex = 3;
            // 
            // lblPendientesNumero
            // 
            lblPendientesNumero.AutoSize = true;
            lblPendientesNumero.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            lblPendientesNumero.ForeColor = Color.FromArgb(20, 27, 38);
            lblPendientesNumero.Location = new Point(18, 41);
            lblPendientesNumero.Name = "lblPendientesNumero";
            lblPendientesNumero.Size = new Size(40, 46);
            lblPendientesNumero.TabIndex = 0;
            lblPendientesNumero.Text = "0";
            // 
            // lblPendientesTitulo
            // 
            lblPendientesTitulo.AutoSize = true;
            lblPendientesTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPendientesTitulo.ForeColor = Color.FromArgb(96, 106, 120);
            lblPendientesTitulo.Location = new Point(18, 15);
            lblPendientesTitulo.Name = "lblPendientesTitulo";
            lblPendientesTitulo.Size = new Size(82, 19);
            lblPendientesTitulo.TabIndex = 1;
            lblPendientesTitulo.Text = "Pendientes";
            // 
            // pnlCitasHoy
            // 
            pnlCitasHoy.BackColor = Color.White;
            pnlCitasHoy.BorderStyle = BorderStyle.FixedSingle;
            pnlCitasHoy.Controls.Add(lblCitasHoyNumero);
            pnlCitasHoy.Controls.Add(lblCitasHoyTitulo);
            pnlCitasHoy.Location = new Point(35, 120);
            pnlCitasHoy.Margin = new Padding(3, 2, 3, 2);
            pnlCitasHoy.Name = "pnlCitasHoy";
            pnlCitasHoy.Size = new Size(219, 105);
            pnlCitasHoy.TabIndex = 2;
            // 
            // lblCitasHoyNumero
            // 
            lblCitasHoyNumero.AutoSize = true;
            lblCitasHoyNumero.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            lblCitasHoyNumero.ForeColor = Color.FromArgb(20, 27, 38);
            lblCitasHoyNumero.Location = new Point(18, 41);
            lblCitasHoyNumero.Name = "lblCitasHoyNumero";
            lblCitasHoyNumero.Size = new Size(40, 46);
            lblCitasHoyNumero.TabIndex = 0;
            lblCitasHoyNumero.Text = "0";
            // 
            // lblCitasHoyTitulo
            // 
            lblCitasHoyTitulo.AutoSize = true;
            lblCitasHoyTitulo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCitasHoyTitulo.ForeColor = Color.FromArgb(96, 106, 120);
            lblCitasHoyTitulo.Location = new Point(18, 15);
            lblCitasHoyTitulo.Name = "lblCitasHoyTitulo";
            lblCitasHoyTitulo.Size = new Size(91, 19);
            lblCitasHoyTitulo.TabIndex = 1;
            lblCitasHoyTitulo.Text = "Citas de hoy";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Font = new Font("Segoe UI", 11F);
            lblDescripcion.ForeColor = Color.FromArgb(96, 106, 120);
            lblDescripcion.Location = new Point(36, 71);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(294, 20);
            lblDescripcion.TabIndex = 1;
            lblDescripcion.Text = "Aquí tienes un resumen de las operaciones.";
            // 
            // lblBienvenida
            // 
            lblBienvenida.AutoSize = true;
            lblBienvenida.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblBienvenida.ForeColor = Color.FromArgb(20, 27, 38);
            lblBienvenida.Location = new Point(35, 30);
            lblBienvenida.Name = "lblBienvenida";
            lblBienvenida.Size = new Size(176, 41);
            lblBienvenida.TabIndex = 0;
            lblBienvenida.Text = "Bienvenido";

            // 
            // pnlPermisos
            // 
            pnlPermisos.BackColor = Color.White;
            pnlPermisos.BorderStyle = BorderStyle.FixedSingle;
            pnlPermisos.Controls.Add(btnEliminar);
            pnlPermisos.Controls.Add(btnModificar);
            pnlPermisos.Controls.Add(btnAgregar);
            pnlPermisos.Controls.Add(btnConsultar);
            pnlPermisos.Controls.Add(lblPermisos);
            pnlPermisos.Location = new Point(35, 250);
            pnlPermisos.Name = "pnlPermisos";
            pnlPermisos.Size = new Size(709, 105);
            pnlPermisos.TabIndex = 5;

            // 
            // lblPermisos
            // 
            lblPermisos.AutoSize = true;
            lblPermisos.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPermisos.ForeColor = Color.FromArgb(70, 80, 95);
            lblPermisos.Location = new Point(18, 15);
            lblPermisos.Name = "lblPermisos";
            lblPermisos.Size = new Size(180, 19);
            lblPermisos.TabIndex = 0;
            lblPermisos.Text = "Acciones según tu rol";

            // 
            // btnConsultar
            // 
            btnConsultar.BackColor = Color.FromArgb(220, 232, 255);
            btnConsultar.FlatAppearance.BorderSize = 0;
            btnConsultar.FlatStyle = FlatStyle.Flat;
            btnConsultar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConsultar.ForeColor = Color.FromArgb(28, 69, 171);
            btnConsultar.Location = new Point(18, 50);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.Size = new Size(150, 35);
            btnConsultar.TabIndex = 1;
            btnConsultar.Text = "Consultar";
            btnConsultar.UseVisualStyleBackColor = false;
            btnConsultar.Click += btnConsultar_Click;

            // 
            // btnAgregar
            // 
            btnAgregar.BackColor = Color.FromArgb(220, 232, 255);
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.FlatStyle = FlatStyle.Flat;
            btnAgregar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAgregar.ForeColor = Color.FromArgb(28, 69, 171);
            btnAgregar.Location = new Point(190, 50);
            btnAgregar.Name = "btnAgregar";
            btnAgregar.Size = new Size(150, 35);
            btnAgregar.TabIndex = 2;
            btnAgregar.Text = "Agregar";
            btnAgregar.UseVisualStyleBackColor = false;
            btnAgregar.Click += btnAgregar_Click;

            // 
            // btnModificar
            // 
            btnModificar.BackColor = Color.FromArgb(220, 232, 255);
            btnModificar.FlatAppearance.BorderSize = 0;
            btnModificar.FlatStyle = FlatStyle.Flat;
            btnModificar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnModificar.ForeColor = Color.FromArgb(28, 69, 171);
            btnModificar.Location = new Point(362, 50);
            btnModificar.Name = "btnModificar";
            btnModificar.Size = new Size(150, 35);
            btnModificar.TabIndex = 3;
            btnModificar.Text = "Modificar";
            btnModificar.UseVisualStyleBackColor = false;
            btnModificar.Click += btnModificar_Click;

            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.FromArgb(220, 232, 255);
            btnEliminar.FlatAppearance.BorderSize = 0;
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEliminar.ForeColor = Color.FromArgb(28, 69, 171);
            btnEliminar.Location = new Point(534, 50);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(150, 35);
            btnEliminar.TabIndex = 4;
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += btnEliminar_Click;

            // 
            // FrmDashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 253);
            ClientSize = new Size(959, 449);
            Controls.Add(pnlContenido);
            Controls.Add(pnlTopbar);
            Controls.Add(pnlSidebar);
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(960, 451);
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
        private Button btnCrearUsuario;
        private Button btnCerrarSesion;
        private Button btnCrearRol;

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
        private Button btnUsuario;

        private Panel pnlPermisos;
        private Label lblPermisos;

        private Button btnConsultar;
        private Button btnAgregar;
        private Button btnModificar;
        private Button btnEliminar;
    }
}