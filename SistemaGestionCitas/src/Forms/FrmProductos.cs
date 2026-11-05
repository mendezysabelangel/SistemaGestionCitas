using System.Globalization;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    // Formulario SIN archivo Designer (igual que FrmUsuarios):
    // todos los controles se crean por código dentro de ConstruirInterfaz().
    public class FrmProductos : Form
    {
        private readonly ProductoService productoService = new ProductoService();

        // Id del producto seleccionado en la tabla (null = ninguno)
        private int? idSeleccionado = null;

        // true mientras se llenan o reinician los filtros: evita consultar a la BD varias veces
        private bool cargando = false;

        private readonly Color azul = Color.FromArgb(27, 68, 170);
        private readonly Color rojo = Color.FromArgb(185, 28, 28);
        private readonly Color fondoClaro = Color.FromArgb(247, 248, 252);
        private readonly Color rojoClaro = Color.FromArgb(254, 226, 226);      // fila de producto agotado
        private readonly Color amarilloClaro = Color.FromArgb(254, 243, 199);  // fila con stock bajo

        // ----- Controles de los filtros -----
        private readonly TextBox txtBuscarId = new TextBox();
        private readonly TextBox txtBuscarTexto = new TextBox();
        private readonly ComboBox cmbBuscarCategoria = new ComboBox();
        private readonly ComboBox cmbBuscarEstado = new ComboBox();
        private readonly ComboBox cmbBuscarStock = new ComboBox();
        private readonly Label lblTotal = new Label();

        // ----- Tabla -----
        private readonly DataGridView dgvProductos = new DataGridView();

        // ----- Campos de datos del producto -----
        private readonly TextBox txtCodigo = new TextBox();
        private readonly TextBox txtNombre = new TextBox();
        private readonly TextBox txtPrecio = new TextBox();
        private readonly TextBox txtStock = new TextBox();
        private readonly ComboBox cmbCategoria = new ComboBox();
        private readonly CheckBox chkActivo = new CheckBox();

        public FrmProductos()
        {
            ConstruirInterfaz();
            Load += FrmProductos_Load;
        }

        // =====================================================
        //  CONSTRUCCIÓN DE LA INTERFAZ
        // =====================================================
        private void ConstruirInterfaz()
        {
            Text = "Gestión de Productos";
            Font = new Font("Segoe UI", 10F);
            BackColor = fondoClaro;
            ClientSize = new Size(1000, 640);
            AutoScaleMode = AutoScaleMode.Font;

            // ---------- Grupo de filtros (arriba) ----------
            GroupBox grpFiltros = new GroupBox
            {
                Text = "Filtros de búsqueda",
                Dock = DockStyle.Top,
                Height = 135
            };

            // Fila 1: ID, nombre o código, categoría
            grpFiltros.Controls.Add(CrearLabel("ID:", 15, 31, 30));
            grpFiltros.Controls.Add(ConfigurarTexto(txtBuscarId, 50, 28, 90, 9));

            grpFiltros.Controls.Add(CrearLabel("Nombre o código:", 160, 31, 120));
            grpFiltros.Controls.Add(ConfigurarTexto(txtBuscarTexto, 285, 28, 210, 100));

            grpFiltros.Controls.Add(CrearLabel("Categoría:", 515, 31, 80));
            ConfigurarCombo(cmbBuscarCategoria, 600, 28, 200);
            grpFiltros.Controls.Add(cmbBuscarCategoria);

            // Fila 2: estado, nivel de stock, botones
            grpFiltros.Controls.Add(CrearLabel("Estado:", 15, 73, 55));
            ConfigurarCombo(cmbBuscarEstado, 75, 70, 120);
            cmbBuscarEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cmbBuscarEstado.SelectedIndex = 0;
            grpFiltros.Controls.Add(cmbBuscarEstado);

            grpFiltros.Controls.Add(CrearLabel("Stock:", 215, 73, 50));
            ConfigurarCombo(cmbBuscarStock, 270, 70, 210);
            cmbBuscarStock.Items.AddRange(new object[]
            {
                "Todos",
                "Agotado (0)",
                $"Stock bajo (1 a {ProductoService.LimiteStockBajo})",
                $"Disponible (más de {ProductoService.LimiteStockBajo})"
            });
            cmbBuscarStock.SelectedIndex = 0;
            grpFiltros.Controls.Add(cmbBuscarStock);

            grpFiltros.Controls.Add(ConfigurarBoton(new Button(), "Consultar", 600, 66, 110, azul, Color.White, BtnConsultar_Click));
            grpFiltros.Controls.Add(ConfigurarBoton(new Button(), "Limpiar filtros", 720, 66, 130, Color.White, Color.Black, BtnLimpiarFiltros_Click));

            lblTotal.Location = new Point(15, 106);
            lblTotal.AutoSize = true;
            lblTotal.Text = "Registros encontrados: 0";
            grpFiltros.Controls.Add(lblTotal);

            grpFiltros.Controls.Add(new Label
            {
                Text = "Fila roja = agotado   ·   Fila amarilla = stock bajo",
                Location = new Point(260, 106),
                AutoSize = true,
                ForeColor = Color.DimGray
            });

            // Los combos consultan solos al cambiar (a menos que "cargando" sea true)
            cmbBuscarCategoria.SelectedIndexChanged += FiltroCambiado;
            cmbBuscarEstado.SelectedIndexChanged += FiltroCambiado;
            cmbBuscarStock.SelectedIndexChanged += FiltroCambiado;

            // Enter dentro de los cuadros de búsqueda = consultar. En ID solo dígitos.
            txtBuscarId.KeyDown += Filtro_KeyDown;
            txtBuscarTexto.KeyDown += Filtro_KeyDown;
            txtBuscarId.KeyPress += SoloDigitos_KeyPress;

            // ---------- Grupo de datos (abajo) ----------
            GroupBox grpDatos = new GroupBox
            {
                Text = "Datos del producto",
                Dock = DockStyle.Bottom,
                Height = 190
            };

            grpDatos.Controls.Add(CrearLabel("Código:", 15, 31, 65));
            grpDatos.Controls.Add(ConfigurarTexto(txtCodigo, 85, 28, 150, 30));
            grpDatos.Controls.Add(CrearLabel("Nombre:", 255, 31, 65));
            grpDatos.Controls.Add(ConfigurarTexto(txtNombre, 325, 28, 270, 100));
            grpDatos.Controls.Add(CrearLabel("Categoría:", 615, 31, 80));
            ConfigurarCombo(cmbCategoria, 700, 28, 220);
            grpDatos.Controls.Add(cmbCategoria);

            grpDatos.Controls.Add(CrearLabel("Precio:", 15, 78, 65));
            grpDatos.Controls.Add(ConfigurarTexto(txtPrecio, 85, 75, 150, 12));
            txtPrecio.KeyPress += TxtPrecio_KeyPress;

            grpDatos.Controls.Add(CrearLabel("Stock:", 255, 78, 65));
            grpDatos.Controls.Add(ConfigurarTexto(txtStock, 325, 75, 120, 7));
            txtStock.KeyPress += SoloDigitos_KeyPress;

            chkActivo.Text = "Producto activo";
            chkActivo.Checked = true;
            chkActivo.AutoSize = true;
            chkActivo.Location = new Point(470, 77);
            grpDatos.Controls.Add(chkActivo);

            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Registrar", 85, 130, 120, azul, Color.White, BtnRegistrar_Click));
            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Modificar", 215, 130, 120, azul, Color.White, BtnModificar_Click));
            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Eliminar", 345, 130, 110, rojo, Color.White, BtnEliminar_Click));
            grpDatos.Controls.Add(ConfigurarBoton(new Button(), "Limpiar campos", 465, 130, 150, Color.White, Color.Black, BtnLimpiarCampos_Click));

            // ---------- Tabla (centro) ----------
            ConfigurarTabla();

            // La tabla (Fill) se agrega primero y se pone al frente
            Controls.Add(dgvProductos);
            Controls.Add(grpDatos);
            Controls.Add(grpFiltros);
            dgvProductos.BringToFront();
        }

        private void ConfigurarTabla()
        {
            dgvProductos.Dock = DockStyle.Fill;
            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.MultiSelect = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.AutoGenerateColumns = false;     // las columnas las definimos nosotros
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.ColumnHeadersDefaultCellStyle.BackColor = azul;
            dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 252);

            // DataPropertyName = nombre de la propiedad de la clase Producto
            AgregarColumna(nameof(Producto.IdProducto), "ID", 40);
            AgregarColumna(nameof(Producto.Codigo), "Código", 70);
            AgregarColumna(nameof(Producto.Nombre), "Nombre", 180);
            AgregarColumna(nameof(Producto.Categoria), "Categoría", 110);
            AgregarColumna(nameof(Producto.Precio), "Precio", 70, "N2", true);
            AgregarColumna(nameof(Producto.Stock), "Stock", 50, null, true);
            AgregarColumna(nameof(Producto.Estado), "Estado", 60);

            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
            dgvProductos.DataBindingComplete += (s, e) => ResaltarStock();
        }

        private void AgregarColumna(string propiedad, string titulo, int peso,
                                    string? formato = null, bool alinearDerecha = false)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn
            {
                DataPropertyName = propiedad,
                HeaderText = titulo,
                FillWeight = peso,
                Name = "col" + propiedad
            };

            if (formato != null) col.DefaultCellStyle.Format = formato;
            if (alinearDerecha) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvProductos.Columns.Add(col);
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

        private void ConfigurarCombo(ComboBox combo, int x, int y, int ancho)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;   // solo se puede elegir, no escribir
            combo.Location = new Point(x, y);
            combo.Width = ancho;
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
        //  CARGA DE DATOS
        // =====================================================
        private void FrmProductos_Load(object? sender, EventArgs e)
        {
            CargarCategorias();
            Consultar();
        }

        // Llena los dos ComboBox de categoría con los datos de la base de datos
        private void CargarCategorias()
        {
            try
            {
                List<CategoriaProducto> categorias = productoService.ObtenerCategorias();

                // Cada combo lleva una primera opción que no es una categoría real (Id = 0)
                List<CategoriaProducto> paraFiltro = new List<CategoriaProducto>
                {
                    new CategoriaProducto { IdCategoria = 0, Nombre = "Todas" }
                };
                paraFiltro.AddRange(categorias);

                List<CategoriaProducto> paraEditor = new List<CategoriaProducto>
                {
                    new CategoriaProducto { IdCategoria = 0, Nombre = "-- Seleccione --" }
                };
                paraEditor.AddRange(categorias);

                cargando = true;
                VincularCombo(cmbBuscarCategoria, paraFiltro);
                VincularCombo(cmbCategoria, paraEditor);
                cargando = false;
            }
            catch (Exception ex)
            {
                cargando = false;
                MessageBox.Show("No se pudieron cargar las categorías.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DisplayMember = lo que se ve; ValueMember = el valor real (el Id) que se lee después
        private void VincularCombo(ComboBox combo, List<CategoriaProducto> datos)
        {
            combo.DisplayMember = nameof(CategoriaProducto.Nombre);
            combo.ValueMember = nameof(CategoriaProducto.IdCategoria);
            combo.DataSource = datos;
        }

        // =====================================================
        //  CONSULTAR (FILTROS DINÁMICOS)
        // =====================================================
        private void Consultar()
        {
            // Filtro por ID
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

            // Filtro por categoría (0 = "Todas")
            int? idCategoria = null;
            if (cmbBuscarCategoria.SelectedValue is int categoriaElegida && categoriaElegida > 0)
                idCategoria = categoriaElegida;

            // Filtro por estado (0 = Todos, 1 = Activo, 2 = Inactivo)
            bool? activo = null;
            if (cmbBuscarEstado.SelectedIndex == 1) activo = true;
            else if (cmbBuscarEstado.SelectedIndex == 2) activo = false;

            // Filtro por nivel de stock (se traduce a un rango mínimo-máximo)
            int? stockMinimo = null;
            int? stockMaximo = null;
            switch (cmbBuscarStock.SelectedIndex)
            {
                case 1:   // agotados
                    stockMinimo = 0;
                    stockMaximo = 0;
                    break;
                case 2:   // stock bajo
                    stockMinimo = 1;
                    stockMaximo = ProductoService.LimiteStockBajo;
                    break;
                case 3:   // disponibles
                    stockMinimo = ProductoService.LimiteStockBajo + 1;
                    break;
            }

            FiltroProducto filtro = new FiltroProducto
            {
                IdProducto = id,
                Texto = txtBuscarTexto.Text,
                IdCategoria = idCategoria,
                Activo = activo,
                StockMinimo = stockMinimo,
                StockMaximo = stockMaximo
            };

            try
            {
                List<Producto> lista = productoService.Buscar(filtro);

                dgvProductos.DataSource = lista;
                dgvProductos.ClearSelection();
                LimpiarFormulario();
                lblTotal.Text = "Registros encontrados: " + lista.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo consultar la base de datos.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Colorea las filas según el stock: rojo = agotado, amarillo = stock bajo
        private void ResaltarStock()
        {
            foreach (DataGridViewRow fila in dgvProductos.Rows)
            {
                if (fila.DataBoundItem is not Producto p) continue;

                if (p.Stock == 0)
                    fila.DefaultCellStyle.BackColor = rojoClaro;
                else if (p.Stock <= ProductoService.LimiteStockBajo)
                    fila.DefaultCellStyle.BackColor = amarilloClaro;
                else
                    fila.DefaultCellStyle.BackColor = Color.Empty;   // vuelve al color normal
            }
        }

        // Al seleccionar una fila de la tabla, sus datos pasan a los campos de abajo
        private void DgvProductos_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0) return;
            if (dgvProductos.SelectedRows[0].DataBoundItem is not Producto p) return;

            idSeleccionado = p.IdProducto;
            txtCodigo.Text = p.Codigo;
            txtNombre.Text = p.Nombre;
            txtPrecio.Text = p.Precio.ToString("0.00", CultureInfo.CurrentCulture);
            txtStock.Text = p.Stock.ToString();
            cmbCategoria.SelectedValue = p.IdCategoria;
            chkActivo.Checked = p.Activo;
        }

        // =====================================================
        //  LIMPIEZA Y LECTURA DEL FORMULARIO
        // =====================================================
        private void LimpiarFormulario()
        {
            idSeleccionado = null;
            txtCodigo.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            chkActivo.Checked = true;

            if (cmbCategoria.Items.Count > 0)
                cmbCategoria.SelectedIndex = 0;   // "-- Seleccione --"
        }

        private void ReiniciarFiltros()
        {
            cargando = true;
            txtBuscarId.Clear();
            txtBuscarTexto.Clear();
            if (cmbBuscarCategoria.Items.Count > 0) cmbBuscarCategoria.SelectedIndex = 0;
            cmbBuscarEstado.SelectedIndex = 0;
            cmbBuscarStock.SelectedIndex = 0;
            cargando = false;
        }

        // Valida campos vacíos y tipos de datos. Si todo está bien, arma el objeto Producto.
        // Las demás reglas (formato del código, rangos, duplicados) las valida el servicio.
        private bool TryLeerFormulario(out Producto producto)
        {
            producto = new Producto();

            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                return Advertir("Ingrese el código del producto.", txtCodigo);

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return Advertir("Ingrese el nombre del producto.", txtNombre);

            if (cmbCategoria.SelectedValue is not int idCategoria || idCategoria <= 0)
                return Advertir("Seleccione una categoría.", cmbCategoria);

            if (string.IsNullOrWhiteSpace(txtPrecio.Text))
                return Advertir("Ingrese el precio.", txtPrecio);

            // Tipo de dato: el precio debe ser un número decimal
            if (!decimal.TryParse(txtPrecio.Text.Trim(), NumberStyles.Number,
                    CultureInfo.CurrentCulture, out decimal precio))
                return Advertir("El precio debe ser un número válido (ejemplo: 650.00).", txtPrecio);

            if (string.IsNullOrWhiteSpace(txtStock.Text))
                return Advertir("Ingrese el stock.", txtStock);

            // Tipo de dato: el stock debe ser un número entero
            if (!int.TryParse(txtStock.Text.Trim(), NumberStyles.None,
                    CultureInfo.InvariantCulture, out int stock))
                return Advertir("El stock debe ser un número entero (0 o más).", txtStock);

            producto = new Producto
            {
                IdProducto = idSeleccionado ?? 0,
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                Precio = precio,
                Stock = stock,
                IdCategoria = idCategoria,
                Activo = chkActivo.Checked
            };
            return true;
        }

        // Muestra el aviso, pone el cursor en el campo con problema y devuelve false
        private bool Advertir(string mensaje, Control campo)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            campo.Focus();
            return false;
        }

        // Ejecuta Registrar/Modificar/Eliminar y muestra el resultado (evita repetir código)
        private void EjecutarOperacion(Func<ResultadoOperacion> operacion, string titulo,
                                       bool reiniciarFiltros = false)
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

            // Al registrar se limpian los filtros para que el producto nuevo aparezca en la tabla
            if (reiniciarFiltros) ReiniciarFiltros();
            Consultar();
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
            ReiniciarFiltros();
            Consultar();
        }

        private void BtnLimpiarCampos_Click(object? sender, EventArgs e)
        {
            dgvProductos.ClearSelection();
            LimpiarFormulario();
            txtCodigo.Focus();
        }

        // REGISTRAR (permiso: Agregar)
        private void BtnRegistrar_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Agregar, "registrar productos"))
                return;

            if (idSeleccionado != null)
            {
                MessageBox.Show(
                    "Hay un producto seleccionado en la tabla.\n\n" +
                    "Pulse \"Limpiar campos\" para registrar uno distinto o \"Modificar\" para actualizar el seleccionado.",
                    "Producto seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryLeerFormulario(out Producto producto)) return;

            EjecutarOperacion(() => productoService.Registrar(producto), "Registrar producto", true);
        }

        // MODIFICAR (permiso: Modificar)
        private void BtnModificar_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Modificar, "modificar registros"))
                return;

            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto de la tabla para modificarlo.",
                    "Modificar producto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!TryLeerFormulario(out Producto producto)) return;

            EjecutarOperacion(() => productoService.Modificar(producto), "Modificar producto");
        }

        // ELIMINAR (permiso: Eliminar)
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Eliminar, "eliminar registros"))
                return;

            if (idSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto de la tabla para eliminarlo.",
                    "Eliminar producto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                $"¿Está seguro de eliminar el producto {txtCodigo.Text} - {txtNombre.Text}?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            int id = idSeleccionado.Value;
            EjecutarOperacion(() => productoService.Eliminar(id), "Eliminar producto");
        }

        // Los combos de filtro llaman a este método al cambiar su selección
        private void FiltroCambiado(object? sender, EventArgs e)
        {
            if (!cargando) Consultar();
        }

        // Enter en un cuadro de búsqueda = consultar
        private void Filtro_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;   // evita el "ding" de Windows
                Consultar();
            }
        }

        // Solo se aceptan dígitos (ID de búsqueda y stock)
        private void SoloDigitos_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // En el precio: dígitos y un único separador decimal (el de la configuración de Windows)
        private void TxtPrecio_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;

            string separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (e.KeyChar.ToString() == separador && !txtPrecio.Text.Contains(separador)) return;

            e.Handled = true;   // cualquier otra tecla se descarta
        }
    }
}