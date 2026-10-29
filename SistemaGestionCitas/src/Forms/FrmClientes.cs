using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmClientes : Form
    {
        private readonly ClienteService clienteService = new ClienteService();

        // Id del cliente seleccionado en la tabla (null = ninguno)
        private int? idSeleccionado = null;

        private readonly Color azul = Color.FromArgb(27, 68, 170);
        private readonly Color rojo = Color.FromArgb(185, 28, 28);
        private readonly Color fondoClaro = Color.FromArgb(247, 248, 252);

        // ----- Controles de los filtros -----
        private readonly TextBox txtBuscarId = new TextBox();
        private readonly TextBox txtBuscarNombre = new TextBox();
        private readonly ComboBox cmbBuscarEstado = new ComboBox();
        private readonly Label lblTotal = new Label();

        // ----- Tabla -----
        private readonly DataGridView dgvClientes = new DataGridView();

        // ----- Campos de datos del cliente -----
        private readonly TextBox txtNombres = new TextBox();
        private readonly TextBox txtApellidos = new TextBox();
        private readonly TextBox txtTelefono = new TextBox();
        private readonly TextBox txtCorreo = new TextBox();
        private readonly TextBox txtDocumento = new TextBox();
        private readonly DateTimePicker dtpFechaNacimiento = new DateTimePicker();
        private readonly CheckBox chkActivo = new CheckBox();

        public FrmClientes()
        {
            InitializeComponent();
            ConstruirInterfaz();
            Load += FrmClientes_Load;
        }

        // =====================================================
        //  CONSTRUCCIÓN DE LA INTERFAZ
        // =====================================================
        private void ConstruirInterfaz()
        {
            Text = "Gestión de Clientes";
            Font = new Font("Segoe UI", 10F);
            BackColor = fondoClaro;
            ClientSize = new Size(1000, 640);

            // ---------- Grupo de filtros (arriba) ----------
            GroupBox grpFiltros = new GroupBox
            {
                Text = "Filtros de búsqueda",
                Dock = DockStyle.Top,
                Height = 95
            };

            grpFiltros.Controls.Add(CrearLabel("ID:", 15, 31, 30));
            grpFiltros.Controls.Add(ConfigurarTexto(txtBuscarId, 50, 28, 90, 9));

            grpFiltros.Controls.Add(CrearLabel("Nombre:", 160, 31, 65));
            grpFiltros.Controls.Add(ConfigurarTexto(txtBuscarNombre, 228, 28, 220, 80));

            grpFiltros.Controls.Add(CrearLabel("Estado:", 470, 31, 60));
            cmbBuscarEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBuscarEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cmbBuscarEstado.SelectedIndex = 0;
            cmbBuscarEstado.Location = new Point(535, 28);
            cmbBuscarEstado.Width = 130;
            cmbBuscarEstado.SelectedIndexChanged += (s, e) => Consultar();   // filtra al cambiar
            grpFiltros.Controls.Add(cmbBuscarEstado);

            grpFiltros.Controls.Add(ConfigurarBoton(new Button(), "Consultar", 690, 25, 110, azul, Color.White, BtnConsultar_Click));
            grpFiltros.Controls.Add(ConfigurarBoton(new Button(), "Limpiar", 810, 25, 110, Color.White, Color.Black, BtnLimpiarFiltros_Click));

            lblTotal.Location = new Point(15, 64);
            lblTotal.AutoSize = true;
            lblTotal.Text = "Registros encontrados: 0";
            grpFiltros.Controls.Add(lblTotal);

            // Enter dentro de los filtros = consultar. En ID solo se permiten números.
            txtBuscarId.KeyDown += Filtro_KeyDown;
            txtBuscarNombre.KeyDown += Filtro_KeyDown;
            txtBuscarId.KeyPress += TxtBuscarId_KeyPress;

            // ---------- Grupo de datos (abajo) ----------
            GroupBox grpDatos = new GroupBox
            {
                Text = "Datos del cliente",
                Dock = DockStyle.Bottom,
                Height = 200
            };

            grpDatos.Controls.Add(CrearLabel("Nombres:", 15, 31, 80));
            grpDatos.Controls.Add(ConfigurarTexto(txtNombres, 100, 28, 200, 80));
            grpDatos.Controls.Add(CrearLabel("Apellidos:", 330, 31, 80));
            grpDatos.Controls.Add(ConfigurarTexto(txtApellidos, 410, 28, 200, 80));
            grpDatos.Controls.Add(CrearLabel("Teléfono:", 640, 31, 75));
            grpDatos.Controls.Add(ConfigurarTexto(txtTelefono, 720, 28, 150, 20));

            grpDatos.Controls.Add(CrearLabel("Correo:", 15, 78, 80));
            grpDatos.Controls.Add(ConfigurarTexto(txtCorreo, 100, 75, 200, 100));
            grpDatos.Controls.Add(CrearLabel("Documento:", 330, 78, 80));
            grpDatos.Controls.Add(ConfigurarTexto(txtDocumento, 410, 75, 200, 30));
            grpDatos.Controls.Add(CrearLabel("Nacimiento:", 640, 78, 85));

            dtpFechaNacimiento.Format = DateTimePickerFormat.Short;
            dtpFechaNacimiento.ShowCheckBox = true;      // casilla: si está desmarcada, no hay fecha
            dtpFechaNacimiento.MaxDate = DateTime.Today;
            dtpFechaNacimiento.Checked = false;
            dtpFechaNacimiento.Location = new Point(730, 75);
            dtpFechaNacimiento.Width = 140;
            grpDatos.Controls.Add(dtpFechaNacimiento);

            chkActivo.Text = "Cliente activo";
            chkActivo.Checked = true;
            chkActivo.AutoSize = true;
            chkActivo.Location = new Point(100, 118);
            grpDatos.Controls.Add(chkActivo);

            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Nuevo", 100, 150, 110, Color.White, Color.Black, BtnNuevo_Click));
            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Registrar", 220, 150, 110, azul, Color.White, BtnRegistrar_Click));
            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Modificar", 340, 150, 110, azul, Color.White, BtnModificar_Click));
            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Eliminar", 460, 150, 110, rojo, Color.White, BtnEliminar_Click));
            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Generar reporte", 590, 150, 150, Color.White, azul, BtnReporte_Click));

            // ---------- Tabla (centro) ----------
            ConfigurarTabla();

            // La tabla (Fill) se agrega primero y se pone al frente
            Controls.Add(dgvClientes);
            Controls.Add(grpDatos);
            Controls.Add(grpFiltros);
            dgvClientes.BringToFront();
        }

        private void ConfigurarTabla()
        {
            dgvClientes.Dock = DockStyle.Fill;
            dgvClientes.ReadOnly = true;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToDeleteRows = false;
            dgvClientes.MultiSelect = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.AutoGenerateColumns = false;     // las columnas las definimos nosotros
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClientes.BackgroundColor = Color.White;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.ColumnHeadersDefaultCellStyle.BackColor = azul;
            dgvClientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // DataPropertyName = nombre de la propiedad de la clase Cliente
            AgregarColumna("IdCliente", "ID", 40);
            AgregarColumna("Nombres", "Nombres", 90);
            AgregarColumna("Apellidos", "Apellidos", 90);
            AgregarColumna("Telefono", "Teléfono", 80);
            AgregarColumna("Correo", "Correo", 120);
            AgregarColumna("Documento", "Documento", 80);
            AgregarColumna("FechaNacimiento", "Nacimiento", 70, "dd/MM/yyyy");
            AgregarColumna("Estado", "Estado", 60);
            AgregarColumna("FechaRegistro", "Registro", 70, "dd/MM/yyyy");

            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;
        }

        private void AgregarColumna(string propiedad, string titulo, int peso, string? formato = null)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo,
                FillWeight = peso,
                Name = "col" + propiedad
            };

            if (formato != null) col.DefaultCellStyle.Format = formato;
            dgvClientes.Columns.Add(col);
        }

        // ----- Métodos auxiliares para crear controles sin repetir código -----
        private Label CrearLabel(string texto, int x, int y, int ancho)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(ancho, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private TextBox ConfigurarTexto(TextBox caja, int x, int y, int ancho, int maxLength)
        {
            caja.Location = new Point(x, y);
            caja.Width = ancho;
            caja.MaxLength = maxLength;
            return caja;
        }

        private Button ConfigurarBoton(Button boton, string texto, int x, int y, int ancho,
                                       Color fondo, Color letra, EventHandler alHacerClic)
        {
            boton.Text = texto;
            boton.Location = new Point(x, y);
            boton.Size = new Size(ancho, 34);
            boton.BackColor = fondo;
            boton.ForeColor = letra;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderColor = fondo == Color.White ? Color.Silver : fondo;
            boton.Cursor = Cursors.Hand;
            boton.Click += alHacerClic;
            return boton;
        }

        // =====================================================
        //  LÓGICA DE LA PANTALLA
        // =====================================================
        private void FrmClientes_Load(object? sender, EventArgs e)
        {
            Consultar();
        }

        // CONSULTAR: lee los 3 filtros, pide los datos y los muestra en la tabla
        private void Consultar()
        {
            // Filtro 1: por ID
            int? id = null;
            if (!string.IsNullOrWhiteSpace(txtBuscarId.Text))
            {
                if (!int.TryParse(txtBuscarId.Text.Trim(), out int idLeido) || idLeido <= 0)
                {
                    MessageBox.Show("El ID debe ser un número entero positivo.",
                        "Filtro inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBuscarId.Focus();
                    return;
                }
                id = idLeido;
            }

            // Filtro 3: por estado (0 = Todos, 1 = Activo, 2 = Inactivo)
            bool? activo = null;
            if (cmbBuscarEstado.SelectedIndex == 1) activo = true;
            else if (cmbBuscarEstado.SelectedIndex == 2) activo = false;

            try
            {
                // Filtro 2: por nombre (se envía el texto tal cual)
                List<Cliente> lista = clienteService.Buscar(id, txtBuscarNombre.Text, activo);

                dgvClientes.DataSource = lista;
                dgvClientes.ClearSelection();
                LimpiarFormulario();
                lblTotal.Text = "Registros encontrados: " + lista.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo consultar la base de datos.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Al seleccionar una fila de la tabla, sus datos pasan a los campos de abajo
        private void DgvClientes_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0) return;
            if (dgvClientes.SelectedRows[0].DataBoundItem is not Cliente c) return;

            idSeleccionado = c.IdCliente;
            txtNombres.Text = c.Nombres;
            txtApellidos.Text = c.Apellidos;
            txtTelefono.Text = c.Telefono;
            txtCorreo.Text = c.Correo ?? "";
            txtDocumento.Text = c.Documento ?? "";
            chkActivo.Checked = c.Activo;

            if (c.FechaNacimiento.HasValue)
            {
                dtpFechaNacimiento.Value = c.FechaNacimiento.Value;
                dtpFechaNacimiento.Checked = true;
            }
            else
            {
                dtpFechaNacimiento.Checked = false;
            }
        }

        private void LimpiarFormulario()
        {
            idSeleccionado = null;
            txtNombres.Clear();
            txtApellidos.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtDocumento.Clear();
            chkActivo.Checked = true;
            dtpFechaNacimiento.Checked = false;
        }

        // Arma un objeto Cliente con lo que hay escrito en los campos
        private Cliente LeerFormulario()
        {
            return new Cliente
            {
                IdCliente = idSeleccionado ?? 0,
                Nombres = txtNombres.Text,
                Apellidos = txtApellidos.Text,
                Telefono = txtTelefono.Text,
                Correo = txtCorreo.Text,
                Documento = txtDocumento.Text,
                FechaNacimiento = dtpFechaNacimiento.Checked ? dtpFechaNacimiento.Value.Date : null,
                Activo = chkActivo.Checked
            };
        }

        // Ejecuta Registrar/Modificar/Eliminar y muestra el resultado (evita repetir código)
        private void EjecutarOperacion(Func<ResultadoOperacion> operacion, string titulo)
        {
            ResultadoOperacion resultado;

            try
            {
                resultado = operacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error con la base de datos.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!resultado.Exitoso)
            {
                MessageBox.Show(resultado.Mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(resultado.Mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Consultar();   // refresca la tabla
        }

        // =====================================================
        //  EVENTOS DE LOS BOTONES
        // =====================================================
        private void BtnConsultar_Click(object? sender, EventArgs e)
        {
            Consultar();
        }

        private void BtnLimpiarFiltros_Click(object? sender, EventArgs e)
        {
            txtBuscarId.Clear();
            txtBuscarNombre.Clear();
            cmbBuscarEstado.SelectedIndex = 0;   // esto dispara Consultar() solo si cambió
            Consultar();
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            dgvClientes.ClearSelection();
            LimpiarFormulario();
            txtNombres.Focus();
        }

        // REGISTRAR (permiso: Agregar)
        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Agregar, "registrar clientes"))
                return;

            if (idSeleccionado != null)
            {
                MessageBox.Show(
                    "Hay un cliente seleccionado en la tabla.\n\n" +
                    "Pulse \"Nuevo\" para registrar uno distinto o \"Modificar\" para actualizar el seleccionado.",
                    "Cliente seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EjecutarOperacion(() => clienteService.Registrar(LeerFormulario()), "Registrar cliente");
        }

        // MODIFICAR (permiso: Modificar)
        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Modificar, "modificar registros"))
                return;

            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione un cliente de la tabla para modificarlo.",
                    "Modificar cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EjecutarOperacion(() => clienteService.Modificar(LeerFormulario()), "Modificar cliente");
        }

        // ELIMINAR (permiso: Eliminar)
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Eliminar, "eliminar registros"))
                return;

            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione un cliente de la tabla para eliminarlo.",
                    "Eliminar cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                $"¿Está seguro de eliminar a {txtNombres.Text} {txtApellidos.Text}?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            int id = idSeleccionado.Value;
            EjecutarOperacion(() => clienteService.Eliminar(id), "Eliminar cliente");
        }

        // GENERAR REPORTE (permiso: Consultar)
        private void BtnReporte_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar información"))
                return;

            // 1. Se abre la ventana de filtros (modal: hay que cerrarla para seguir)
            using FrmFiltroReporteClientes ventanaFiltros = new FrmFiltroReporteClientes();

            if (ventanaFiltros.ShowDialog(this) != DialogResult.OK)
                return;   // el usuario canceló

            // 2. Se abre el reporte como ventana hija MDI con los filtros elegidos
            FrmReporteClientes reporte = new FrmReporteClientes(ventanaFiltros.Filtro);

            if (MdiParent is FrmDashboard dashboard)
                dashboard.AbrirHijo(reporte, "Reporte de clientes", true);
            else
                reporte.Show();
        }

        // Enter en un filtro = consultar
        private void Filtro_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;   // evita el "ding" de Windows
                Consultar();
            }
        }

        // En el cuadro de ID solo se aceptan dígitos
        private void TxtBuscarId_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}