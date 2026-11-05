using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;
using SistemaGestionCitas.src.Data;

namespace SistemaGestionCitas.src.Forms
{
    public class FrmUsuarios : Form
    {
        private readonly UsuarioService usuarioService = new UsuarioService();
        private readonly RolRepository rolRepository = new RolRepository();
        private readonly DataGridView dgvUsuarios = new DataGridView();
        private readonly Button btnNuevo = new Button();
        private readonly Button btnModificar = new Button();
        private readonly Button btnPassword = new Button();
        private readonly Button btnEliminar = new Button();
        private readonly Button btnLimpiar = new Button();
        private readonly TextBox txtId = new TextBox();
        private readonly TextBox txtNombreCompleto = new TextBox();
        private readonly TextBox txtNombreUsuario = new TextBox();
        private readonly TextBox txtCorreo = new TextBox();
        private readonly ComboBox cmbRol = new ComboBox();
        private readonly CheckBox chkActivo = new CheckBox();
        private readonly Label lblTotal = new Label();
        private readonly Label lblSeleccion = new Label();
        private readonly Color azul = Color.FromArgb(27, 68, 170);
        private List<string> rolesActivos = new List<string>();
        private int? idSeleccionado;
        private string nombreUsuarioSeleccionado = "";
        private bool cargandoUsuarios;

        public FrmUsuarios()
        {
            ConstruirInterfaz();
            Load += FrmUsuarios_Load;
        }

        private void ConstruirInterfaz()
        {
            Text = "Gestión de Usuarios";
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.FromArgb(247, 248, 252);
            ClientSize = new Size(1000, 700);
            AutoScaleMode = AutoScaleMode.Font;

            Panel cabecera = new Panel { Dock = DockStyle.Top, Height = 75, BackColor = Color.White };
            cabecera.Controls.Add(new Label
            {
                Text = "Usuarios", Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = azul, AutoSize = true, Location = new Point(15, 20)
            });

            FlowLayoutPanel acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Right, Width = 370,
                Padding = new Padding(10, 17, 10, 10), WrapContents = false
            };
            acciones.Controls.Add(ConfigurarBoton(btnNuevo, "Nuevo", 100, azul, Color.White, BtnNuevo_Click));
            acciones.Controls.Add(ConfigurarBoton(new Button(), "Actualizar", 115,
                Color.White, azul, (s, e) => Consultar()));
            acciones.Controls.Add(ConfigurarBoton(new Button(), "Cerrar", 100,
                Color.White, Color.Black, (s, e) => Close()));
            cabecera.Controls.Add(acciones);

            Panel pie = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.White };
            lblTotal.Text = "Usuarios registrados: 0";
            lblTotal.Dock = DockStyle.Top;
            lblTotal.Height = 33;
            lblTotal.Padding = new Padding(15, 8, 0, 0);
            lblSeleccion.Text = "Seleccione un usuario de la tabla.";
            lblSeleccion.Dock = DockStyle.Fill;
            lblSeleccion.Padding = new Padding(15, 0, 15, 0);
            lblSeleccion.TextAlign = ContentAlignment.MiddleLeft;
            lblSeleccion.AutoEllipsis = true;
            pie.Controls.Add(lblSeleccion);
            pie.Controls.Add(lblTotal);

            GroupBox datos = CrearEditor();
            ConfigurarTabla();
            Controls.Add(dgvUsuarios);
            Controls.Add(datos);
            Controls.Add(pie);
            Controls.Add(cabecera);
            dgvUsuarios.BringToFront();
            ConfigurarPermisos();
        }

        private GroupBox CrearEditor()
        {
            GroupBox datos = new GroupBox
            {
                Text = "Datos del usuario", Dock = DockStyle.Bottom, Height = 220
            };
            TableLayoutPanel campos = new TableLayoutPanel
            {
                Dock = DockStyle.Top, Height = 140, ColumnCount = 4, RowCount = 3,
                Padding = new Padding(10, 8, 10, 0)
            };
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            campos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int i = 0; i < 3; i++)
                campos.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            txtNombreCompleto.MaxLength = 100;
            txtNombreUsuario.MaxLength = 50;
            txtCorreo.MaxLength = 100;
            txtId.ReadOnly = true;
            cmbRol.DropDownStyle = ComboBoxStyle.DropDownList;
            chkActivo.Text = "Usuario activo";
            chkActivo.Checked = true;

            AgregarCampo(campos, "Nombre completo:", txtNombreCompleto, 0, 0);
            AgregarCampo(campos, "Usuario:", txtNombreUsuario, 2, 0);
            AgregarCampo(campos, "Correo:", txtCorreo, 0, 1);
            AgregarCampo(campos, "Rol:", cmbRol, 2, 1);
            AgregarCampo(campos, "ID:", txtId, 0, 2);
            AgregarCampo(campos, "Estado:", chkActivo, 2, 2);

            FlowLayoutPanel botones = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, Height = 60,
                Padding = new Padding(12, 9, 12, 8), WrapContents = false
            };
            botones.Controls.Add(ConfigurarBoton(btnModificar, "Modificar", 120, azul, Color.White, BtnModificar_Click));
            botones.Controls.Add(ConfigurarBoton(btnPassword, "Cambiar contraseña", 185, Color.White, azul, BtnPassword_Click));
            botones.Controls.Add(ConfigurarBoton(btnEliminar, "Eliminar", 110, Color.FromArgb(185, 28, 28), Color.White, BtnEliminar_Click));
            botones.Controls.Add(ConfigurarBoton(btnLimpiar, "Limpiar", 105, Color.White, Color.Black, (s, e) =>
            {
                dgvUsuarios.ClearSelection();
                LimpiarFormulario();
            }));
            datos.Controls.Add(botones);
            datos.Controls.Add(campos);
            return datos;
        }

        private void AgregarCampo(TableLayoutPanel panel, string texto, Control campo, int columna, int fila)
        {
            panel.Controls.Add(new Label
            {
                Text = texto, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
            }, columna, fila);
            campo.Dock = DockStyle.Fill;
            campo.Margin = new Padding(3, 5, 12, 5);
            panel.Controls.Add(campo, columna + 1, fila);
        }

        private Button ConfigurarBoton(Button boton, string texto, int ancho,
                                       Color fondo, Color letra, EventHandler alHacerClic)
        {
            boton.Text = texto;
            boton.Size = new Size(ancho, 36);
            boton.BackColor = fondo;
            boton.ForeColor = letra;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderColor = fondo == Color.White ? Color.Silver : fondo;
            boton.Cursor = Cursors.Hand;
            boton.Click += alHacerClic;
            return boton;
        }

        private void ConfigurarTabla()
        {
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = azul;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 252);

            AgregarColumna(nameof(Usuario.IdUsuario), "ID", 7, 55);
            AgregarColumna(nameof(Usuario.NombreUsuario), "Usuario", 16, 120);
            AgregarColumna(nameof(Usuario.NombreCompleto), "Nombre completo", 24, 170);
            AgregarColumna(nameof(Usuario.Correo), "Correo", 24, 190);
            AgregarColumna(nameof(Usuario.NombreRol), "Rol", 15, 110);
            AgregarColumna(nameof(Usuario.Estado), "Estado", 10, 90);
            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;
        }

        private void AgregarColumna(string propiedad, string titulo, int peso, int anchoMinimo)
        {
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "col" + propiedad, DataPropertyName = propiedad, HeaderText = titulo,
                FillWeight = peso, MinimumWidth = anchoMinimo,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        private void FrmUsuarios_Load(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, "consultar usuarios"))
            {
                Close();
                return;
            }
            Consultar();
        }

        private void Consultar(int? seleccionarId = null)
        {
            cargandoUsuarios = true;
            try
            {
                List<Usuario> usuarios = usuarioService.ListarUsuarios();
                rolesActivos = rolRepository.ObtenerRolesActivos();
                dgvUsuarios.DataSource = usuarios;
                dgvUsuarios.ClearSelection();
                LimpiarFormulario();
                lblTotal.Text = "Usuarios registrados: " + usuarios.Count;
            }
            catch (Exception ex)
            {
                dgvUsuarios.DataSource = null;
                LimpiarFormulario();
                lblTotal.Text = "No se pudo cargar el listado de usuarios.";
                MessageBox.Show("No se pudieron consultar los usuarios.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                cargandoUsuarios = false;
            }

            if (seleccionarId.HasValue)
            {
                foreach (DataGridViewRow fila in dgvUsuarios.Rows)
                {
                    if (fila.DataBoundItem is Usuario usuario && usuario.IdUsuario == seleccionarId.Value)
                    {
                        dgvUsuarios.CurrentCell = fila.Cells[0];
                        fila.Selected = true;
                        break;
                    }
                }
            }
            ConfigurarPermisos();
        }

        private void DgvUsuarios_SelectionChanged(object? sender, EventArgs e)
        {
            if (cargandoUsuarios) return;
            if (dgvUsuarios.SelectedRows.Count == 0 ||
                dgvUsuarios.SelectedRows[0].DataBoundItem is not Usuario usuario)
            {
                LimpiarFormulario();
                return;
            }

            idSeleccionado = usuario.IdUsuario;
            nombreUsuarioSeleccionado = usuario.NombreUsuario;
            txtId.Text = usuario.IdUsuario.ToString();
            txtNombreCompleto.Text = usuario.NombreCompleto;
            txtNombreUsuario.Text = usuario.NombreUsuario;
            txtCorreo.Text = usuario.Correo ?? "";
            List<string> roles = new List<string>(rolesActivos);
            if (!roles.Contains(usuario.NombreRol)) roles.Add(usuario.NombreRol);
            cmbRol.DataSource = roles;
            cmbRol.SelectedItem = usuario.NombreRol;
            chkActivo.Checked = usuario.Activo;
            lblSeleccion.Text = $"Usuario seleccionado: {usuario.NombreUsuario} (ID: {usuario.IdUsuario})";
            ConfigurarPermisos();
        }

        private void LimpiarFormulario()
        {
            idSeleccionado = null;
            nombreUsuarioSeleccionado = "";
            txtId.Clear();
            txtNombreCompleto.Clear();
            txtNombreUsuario.Clear();
            txtCorreo.Clear();
            cmbRol.DataSource = new List<string>(rolesActivos);
            cmbRol.SelectedIndex = -1;
            chkActivo.Checked = true;
            lblSeleccion.Text = "Seleccione un usuario de la tabla.";
            ConfigurarPermisos();
        }

        private void ConfigurarPermisos()
        {
            bool administrar = SesionActual.HaySesion && SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);
            bool modificar = administrar && SesionActual.TienePermiso(PermisosSistema.Modificar);
            bool eliminar = administrar && SesionActual.TienePermiso(PermisosSistema.Eliminar);
            bool seleccionado = idSeleccionado.HasValue;
            bool cuentaActual = seleccionado && idSeleccionado == SesionActual.UsuarioActual?.IdUsuario;
            btnNuevo.Visible = administrar;
            btnModificar.Visible = btnPassword.Visible = modificar;
            btnEliminar.Visible = eliminar;
            btnModificar.Enabled = btnPassword.Enabled = seleccionado && modificar;
            btnEliminar.Enabled = seleccionado && eliminar && !cuentaActual;
            txtNombreCompleto.Enabled = txtNombreUsuario.Enabled = txtCorreo.Enabled = seleccionado && modificar;
            cmbRol.Enabled = chkActivo.Enabled = seleccionado && modificar && !cuentaActual;
        }

        private bool VerificarSeleccion()
        {
            if (idSeleccionado.HasValue) return true;
            MessageBox.Show("Seleccione un usuario de la tabla.", "Usuario requerido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private bool VerificarAccion(string permiso, string accion)
        {
            return SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, accion) &&
                SesionActual.VerificarPermiso(permiso, accion);
        }

        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            if (!VerificarAccion(PermisosSistema.Modificar, "modificar usuarios") || !VerificarSeleccion()) return;
            int id = idSeleccionado!.Value;
            EjecutarOperacion(() => usuarioService.ModificarUsuario(id, txtNombreCompleto.Text,
                txtNombreUsuario.Text, txtCorreo.Text, cmbRol.SelectedItem?.ToString() ?? "", chkActivo.Checked),
                "Modificar usuario", id);
        }

        private void BtnPassword_Click(object? sender, EventArgs e)
        {
            if (!VerificarAccion(PermisosSistema.Modificar, "cambiar contraseñas") || !VerificarSeleccion()) return;
            int id = idSeleccionado!.Value;
            using FrmCambiarPasswordUsuario formulario = new FrmCambiarPasswordUsuario(id, nombreUsuarioSeleccionado);
            if (formulario.ShowDialog(this) == DialogResult.OK)
                Consultar(id);
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (!VerificarAccion(PermisosSistema.Eliminar, "eliminar usuarios") || !VerificarSeleccion()) return;
            int id = idSeleccionado!.Value;
            if (MessageBox.Show($"¿Desea eliminar al usuario {nombreUsuarioSeleccionado}?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            EjecutarOperacion(() => usuarioService.EliminarUsuario(id), "Eliminar usuario");
        }

        private void EjecutarOperacion(Func<ResultadoOperacion> operacion, string titulo, int? seleccionarId = null)
        {
            try
            {
                ResultadoOperacion resultado = operacion();
                MessageBox.Show(resultado.Mensaje, titulo, MessageBoxButtons.OK,
                    resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                if (!resultado.Exitoso) return;
                if (MdiParent is FrmDashboard dashboard) dashboard.ActualizarDatosSesion();
                Consultar(seleccionarId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo completar la operación.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, "crear usuarios")) return;
            using CrearUsuario formulario = new CrearUsuario();
            if (formulario.ShowDialog(this) == DialogResult.OK)
                Consultar();
        }
    }
}
