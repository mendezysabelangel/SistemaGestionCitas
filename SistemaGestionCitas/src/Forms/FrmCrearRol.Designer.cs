namespace SistemaGestionCitas.src.Forms
{
    partial class FrmCrearRol
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
            lblTitulo = new Label();
            lblDescripcion = new Label();

            lblNombreRol = new Label();
            txtNombreRol = new TextBox();

            lblDescripcionRol = new Label();
            txtDescripcionRol = new TextBox();

            lblPermisos = new Label();

            chkAgregar = new CheckBox();
            chkModificar = new CheckBox();
            chkEliminar = new CheckBox();
            chkConsultar = new CheckBox();
            chkCrearUsuarios = new CheckBox();
            chkCrearRoles = new CheckBox();

            btnCrearRol = new Button();
            btnCancelar = new Button();

            pnlLateral.SuspendLayout();
            pnlFormulario.SuspendLayout();
            SuspendLayout();

            // =========================
            // PANEL LATERAL
            // =========================

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

            // lblNombreSistema

            lblNombreSistema.AutoSize = true;
            lblNombreSistema.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblNombreSistema.ForeColor = Color.White;
            lblNombreSistema.Location = new Point(45, 65);
            lblNombreSistema.Name = "lblNombreSistema";
            lblNombreSistema.Size = new Size(219, 32);
            lblNombreSistema.TabIndex = 0;
            lblNombreSistema.Text = "Sistema de Gestión";

            // lblSubtituloSistema

            lblSubtituloSistema.AutoSize = true;
            lblSubtituloSistema.Font = new Font("Segoe UI", 10F);
            lblSubtituloSistema.ForeColor = Color.FromArgb(205, 217, 245);
            lblSubtituloSistema.Location = new Point(47, 105);
            lblSubtituloSistema.Name = "lblSubtituloSistema";
            lblSubtituloSistema.Size = new Size(181, 23);
            lblSubtituloSistema.TabIndex = 1;
            lblSubtituloSistema.Text = "Administración de citas";

            // lblMensajePrincipal

            lblMensajePrincipal.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblMensajePrincipal.ForeColor = Color.White;
            lblMensajePrincipal.Location = new Point(45, 210);
            lblMensajePrincipal.Name = "lblMensajePrincipal";
            lblMensajePrincipal.Size = new Size(280, 160);
            lblMensajePrincipal.TabIndex = 2;
            lblMensajePrincipal.Text = "Crea nuevos roles y define sus permisos";

            // lblDescripcionLateral

            lblDescripcionLateral.Font = new Font("Segoe UI", 10F);
            lblDescripcionLateral.ForeColor = Color.FromArgb(215, 225, 248);
            lblDescripcionLateral.Location = new Point(47, 505);
            lblDescripcionLateral.Name = "lblDescripcionLateral";
            lblDescripcionLateral.Size = new Size(270, 90);
            lblDescripcionLateral.TabIndex = 3;
            lblDescripcionLateral.Text =
                "Define qué acciones podrá realizar cada nuevo rol dentro del sistema.";

            // =========================
            // PANEL FORMULARIO
            // =========================

            pnlFormulario.BackColor = Color.White;
            pnlFormulario.Controls.Add(lblTitulo);
            pnlFormulario.Controls.Add(lblDescripcion);

            pnlFormulario.Controls.Add(lblNombreRol);
            pnlFormulario.Controls.Add(txtNombreRol);

            pnlFormulario.Controls.Add(lblDescripcionRol);
            pnlFormulario.Controls.Add(txtDescripcionRol);

            pnlFormulario.Controls.Add(lblPermisos);

            pnlFormulario.Controls.Add(chkAgregar);
            pnlFormulario.Controls.Add(chkModificar);
            pnlFormulario.Controls.Add(chkEliminar);
            pnlFormulario.Controls.Add(chkConsultar);
            pnlFormulario.Controls.Add(chkCrearUsuarios);
            pnlFormulario.Controls.Add(chkCrearRoles);

            pnlFormulario.Controls.Add(btnCrearRol);
            pnlFormulario.Controls.Add(btnCancelar);

            pnlFormulario.Location = new Point(420, 45);
            pnlFormulario.Name = "pnlFormulario";
            pnlFormulario.Size = new Size(520, 610);
            pnlFormulario.TabIndex = 1;

            // lblTitulo

            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(20, 20, 25);
            lblTitulo.Location = new Point(45, 40);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(185, 46);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Crear rol";

            // lblDescripcion

            lblDescripcion.AutoSize = true;
            lblDescripcion.Font = new Font("Segoe UI", 9.5F);
            lblDescripcion.ForeColor = Color.Gray;
            lblDescripcion.Location = new Point(48, 90);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(351, 21);
            lblDescripcion.TabIndex = 1;
            lblDescripcion.Text = "Registra un nuevo rol y asigna sus permisos.";

            // lblNombreRol

            lblNombreRol.AutoSize = true;
            lblNombreRol.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblNombreRol.Location = new Point(48, 140);
            lblNombreRol.Name = "lblNombreRol";
            lblNombreRol.Size = new Size(127, 21);
            lblNombreRol.TabIndex = 2;
            lblNombreRol.Text = "Nombre del rol";

            // txtNombreRol

            txtNombreRol.Font = new Font("Segoe UI", 10F);
            txtNombreRol.Location = new Point(48, 168);
            txtNombreRol.MaxLength = 50;
            txtNombreRol.Name = "txtNombreRol";
            txtNombreRol.PlaceholderText = "Ej: Recepcionista";
            txtNombreRol.Size = new Size(420, 30);
            txtNombreRol.TabIndex = 0;

            // lblDescripcionRol

            lblDescripcionRol.AutoSize = true;
            lblDescripcionRol.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblDescripcionRol.Location = new Point(48, 220);
            lblDescripcionRol.Name = "lblDescripcionRol";
            lblDescripcionRol.Size = new Size(99, 21);
            lblDescripcionRol.TabIndex = 4;
            lblDescripcionRol.Text = "Descripción";

            // txtDescripcionRol

            txtDescripcionRol.Font = new Font("Segoe UI", 10F);
            txtDescripcionRol.Location = new Point(48, 248);
            txtDescripcionRol.MaxLength = 200;
            txtDescripcionRol.Multiline = true;
            txtDescripcionRol.Name = "txtDescripcionRol";
            txtDescripcionRol.PlaceholderText = "Describe brevemente este rol";
            txtDescripcionRol.Size = new Size(420, 70);
            txtDescripcionRol.TabIndex = 1;

            // lblPermisos

            lblPermisos.AutoSize = true;
            lblPermisos.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPermisos.Location = new Point(48, 345);
            lblPermisos.Name = "lblPermisos";
            lblPermisos.Size = new Size(83, 23);
            lblPermisos.TabIndex = 6;
            lblPermisos.Text = "Permisos";

            // chkConsultar

            chkConsultar.AutoSize = true;
            chkConsultar.Font = new Font("Segoe UI", 9.5F);
            chkConsultar.Location = new Point(50, 385);
            chkConsultar.Name = "chkConsultar";
            chkConsultar.Size = new Size(103, 25);
            chkConsultar.TabIndex = 2;
            chkConsultar.Text = "Consultar";
            chkConsultar.UseVisualStyleBackColor = true;

            // chkAgregar

            chkAgregar.AutoSize = true;
            chkAgregar.Font = new Font("Segoe UI", 9.5F);
            chkAgregar.Location = new Point(190, 385);
            chkAgregar.Name = "chkAgregar";
            chkAgregar.Size = new Size(92, 25);
            chkAgregar.TabIndex = 3;
            chkAgregar.Text = "Agregar";
            chkAgregar.UseVisualStyleBackColor = true;

            // chkModificar

            chkModificar.AutoSize = true;
            chkModificar.Font = new Font("Segoe UI", 9.5F);
            chkModificar.Location = new Point(330, 385);
            chkModificar.Name = "chkModificar";
            chkModificar.Size = new Size(102, 25);
            chkModificar.TabIndex = 4;
            chkModificar.Text = "Modificar";
            chkModificar.UseVisualStyleBackColor = true;

            // chkEliminar

            chkEliminar.AutoSize = true;
            chkEliminar.Font = new Font("Segoe UI", 9.5F);
            chkEliminar.Location = new Point(50, 425);
            chkEliminar.Name = "chkEliminar";
            chkEliminar.Size = new Size(90, 25);
            chkEliminar.TabIndex = 5;
            chkEliminar.Text = "Eliminar";
            chkEliminar.UseVisualStyleBackColor = true;

            // chkCrearUsuarios

            chkCrearUsuarios.AutoSize = true;
            chkCrearUsuarios.Font = new Font("Segoe UI", 9.5F);
            chkCrearUsuarios.Location = new Point(190, 425);
            chkCrearUsuarios.Name = "chkCrearUsuarios";
            chkCrearUsuarios.Size = new Size(133, 25);
            chkCrearUsuarios.TabIndex = 6;
            chkCrearUsuarios.Text = "Crear usuarios";
            chkCrearUsuarios.UseVisualStyleBackColor = true;

            // chkCrearRoles

            chkCrearRoles.AutoSize = true;
            chkCrearRoles.Font = new Font("Segoe UI", 9.5F);
            chkCrearRoles.Location = new Point(330, 425);
            chkCrearRoles.Name = "chkCrearRoles";
            chkCrearRoles.Size = new Size(111, 25);
            chkCrearRoles.TabIndex = 7;
            chkCrearRoles.Text = "Crear roles";
            chkCrearRoles.UseVisualStyleBackColor = true;

            // btnCrearRol

            btnCrearRol.BackColor = Color.FromArgb(36, 80, 177);
            btnCrearRol.Cursor = Cursors.Hand;
            btnCrearRol.FlatAppearance.BorderSize = 0;
            btnCrearRol.FlatStyle = FlatStyle.Flat;
            btnCrearRol.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCrearRol.ForeColor = Color.White;
            btnCrearRol.Location = new Point(48, 510);
            btnCrearRol.Name = "btnCrearRol";
            btnCrearRol.Size = new Size(200, 48);
            btnCrearRol.TabIndex = 8;
            btnCrearRol.Text = "Crear rol";
            btnCrearRol.UseVisualStyleBackColor = false;
            btnCrearRol.Click += btnCrearRol_Click;

            // btnCancelar

            btnCancelar.BackColor = Color.White;
            btnCancelar.Cursor = Cursors.Hand;
            btnCancelar.FlatAppearance.BorderColor = Color.Gray;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Font = new Font("Segoe UI", 9.5F);
            btnCancelar.ForeColor = Color.FromArgb(50, 50, 50);
            btnCancelar.Location = new Point(268, 510);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(200, 48);
            btnCancelar.TabIndex = 9;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;

            // =========================
            // FrmCrearRol
            // =========================

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 248, 252);
            ClientSize = new Size(1000, 700);
            Controls.Add(pnlFormulario);
            Controls.Add(pnlLateral);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmCrearRol";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Crear Rol - Sistema de Gestión de Citas";

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
        private Label lblTitulo;
        private Label lblDescripcion;

        private Label lblNombreRol;
        private TextBox txtNombreRol;

        private Label lblDescripcionRol;
        private TextBox txtDescripcionRol;

        private Label lblPermisos;

        private CheckBox chkAgregar;
        private CheckBox chkModificar;
        private CheckBox chkEliminar;
        private CheckBox chkConsultar;
        private CheckBox chkCrearUsuarios;
        private CheckBox chkCrearRoles;

        private Button btnCrearRol;
        private Button btnCancelar;
    }
}