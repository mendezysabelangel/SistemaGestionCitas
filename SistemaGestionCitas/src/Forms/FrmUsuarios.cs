using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public class FrmUsuarios : Form
    {
        private readonly UsuarioService usuarioService = new UsuarioService();
        private readonly DataGridView dgvUsuarios = new DataGridView();
        private readonly Button btnNuevo = new Button();
        private readonly Label lblTotal = new Label();
        private readonly Label lblSeleccion = new Label();
        private readonly Color azul = Color.FromArgb(27, 68, 170);

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
            ClientSize = new Size(1000, 600);
            AutoScaleMode = AutoScaleMode.Font;

            Panel cabecera = new Panel { Dock = DockStyle.Top, Height = 75, BackColor = Color.White };
            cabecera.Controls.Add(new Label
            {
                Text = "Usuarios",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = azul,
                AutoSize = true,
                Location = new Point(15, 20)
            });

            FlowLayoutPanel acciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = 370,
                Padding = new Padding(10, 17, 10, 10),
                WrapContents = false
            };
            ConfigurarBoton(btnNuevo, "Nuevo", 100, azul, Color.White, BtnNuevo_Click);
            btnNuevo.Visible = SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);
            acciones.Controls.Add(btnNuevo);
            acciones.Controls.Add(ConfigurarBoton(new Button(), "Actualizar", 115,
                Color.White, azul, (s, e) => Consultar()));
            acciones.Controls.Add(ConfigurarBoton(new Button(), "Cerrar", 100,
                Color.White, Color.Black, (s, e) => Close()));
            cabecera.Controls.Add(acciones);

            Panel pie = new Panel { Dock = DockStyle.Bottom, Height = 75, BackColor = Color.White };
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

            ConfigurarTabla();
            Controls.Add(dgvUsuarios);
            Controls.Add(pie);
            Controls.Add(cabecera);
            dgvUsuarios.BringToFront();
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
                Name = "col" + propiedad,
                DataPropertyName = propiedad,
                HeaderText = titulo,
                FillWeight = peso,
                MinimumWidth = anchoMinimo,
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

        private void Consultar()
        {
            btnNuevo.Visible = SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);

            try
            {
                List<Usuario> usuarios = usuarioService.ListarUsuarios();
                dgvUsuarios.DataSource = usuarios;
                dgvUsuarios.ClearSelection();
                lblSeleccion.Text = "Seleccione un usuario de la tabla.";
                lblTotal.Text = "Usuarios registrados: " + usuarios.Count;
            }
            catch (Exception ex)
            {
                dgvUsuarios.DataSource = null;
                lblSeleccion.Text = "Seleccione un usuario de la tabla.";
                lblTotal.Text = "No se pudo cargar el listado de usuarios.";
                MessageBox.Show("No se pudieron consultar los usuarios.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvUsuarios_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0 ||
                dgvUsuarios.SelectedRows[0].DataBoundItem is not Usuario usuario)
            {
                lblSeleccion.Text = "Seleccione un usuario de la tabla.";
                return;
            }

            lblSeleccion.Text = $"Usuario seleccionado: {usuario.NombreUsuario} (ID: {usuario.IdUsuario})";
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, "crear usuarios"))
                return;

            using CrearUsuario formulario = new CrearUsuario();
            if (formulario.ShowDialog(this) == DialogResult.OK)
                Consultar();
        }
    }
}
