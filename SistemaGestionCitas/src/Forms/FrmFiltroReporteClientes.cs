using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Forms
{
    // Ventana modal: el usuario elige los filtros y, si acepta, quien la abrió lee la propiedad Filtro
    public partial class FrmFiltroReporteClientes : Form
    {
        public FiltroCliente Filtro { get; private set; } = new FiltroCliente();

        private readonly Color azul = Color.FromArgb(27, 68, 170);

        private readonly TextBox txtId = new TextBox();
        private readonly TextBox txtNombre = new TextBox();
        private readonly ComboBox cmbEstado = new ComboBox();
        private readonly DateTimePicker dtpDesde = new DateTimePicker();
        private readonly DateTimePicker dtpHasta = new DateTimePicker();
        private readonly Button btnGenerar = new Button();
        private readonly Button btnLimpiar = new Button();
        private readonly Button btnCancelar = new Button();

        public FrmFiltroReporteClientes()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            Text = "Filtros del reporte de clientes";
            Font = new Font("Segoe UI", 10F);
            BackColor = Color.White;
            ClientSize = new Size(480, 385);
            FormBorderStyle = FormBorderStyle.FixedDialog;   // tamaño fijo, como un cuadro de diálogo
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            Label titulo = new Label
            {
                Text = "Filtros del reporte",
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };
            Label ayuda = new Label
            {
                Text = "Deje vacío lo que no desee filtrar.",
                ForeColor = Color.Gray,
                Location = new Point(22, 52),
                AutoSize = true
            };
            Controls.Add(titulo);
            Controls.Add(ayuda);

            // ID
            Controls.Add(CrearLabel("ID del cliente:", 20, 93));
            txtId.Location = new Point(190, 90);
            txtId.Width = 120;
            txtId.MaxLength = 9;
            txtId.KeyPress += TxtId_KeyPress;
            Controls.Add(txtId);

            // Nombre
            Controls.Add(CrearLabel("Nombre contiene:", 20, 138));
            txtNombre.Location = new Point(190, 135);
            txtNombre.Width = 270;
            txtNombre.MaxLength = 80;
            Controls.Add(txtNombre);

            // Estado
            Controls.Add(CrearLabel("Estado:", 20, 183));
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cmbEstado.SelectedIndex = 0;
            cmbEstado.Location = new Point(190, 180);
            cmbEstado.Width = 150;
            Controls.Add(cmbEstado);

            // Rango de fechas de registro
            Controls.Add(CrearLabel("Registrados desde:", 20, 228));
            ConfigurarFecha(dtpDesde, 190, 225);

            Controls.Add(CrearLabel("Registrados hasta:", 20, 273));
            ConfigurarFecha(dtpHasta, 190, 270);

            // Botones
            ConfigurarBoton(btnGenerar, "Generar reporte", 20, 330, 160, azul, Color.White);
            btnGenerar.Click += BtnGenerar_Click;

            ConfigurarBoton(btnLimpiar, "Limpiar", 190, 330, 120, Color.White, Color.Black);
            btnLimpiar.Click += BtnLimpiar_Click;

            ConfigurarBoton(btnCancelar, "Cancelar", 320, 330, 120, Color.White, Color.Black);
            btnCancelar.DialogResult = DialogResult.Cancel;   // cierra la ventana devolviendo "Cancel"

            Controls.Add(btnGenerar);
            Controls.Add(btnLimpiar);
            Controls.Add(btnCancelar);

            AcceptButton = btnGenerar;    // Enter = Generar reporte
            CancelButton = btnCancelar;   // Esc = Cancelar
        }

        private Label CrearLabel(string texto, int x, int y)
        {
            return new Label { Text = texto, Location = new Point(x, y), AutoSize = true };
        }

        private void ConfigurarFecha(DateTimePicker dtp, int x, int y)
        {
            dtp.Format = DateTimePickerFormat.Short;
            dtp.ShowCheckBox = true;        // si la casilla está desmarcada, ese filtro no se aplica
            dtp.Checked = false;
            dtp.MaxDate = DateTime.Today;
            dtp.Location = new Point(x, y);
            dtp.Width = 150;
            Controls.Add(dtp);
        }

        private void ConfigurarBoton(Button boton, string texto, int x, int y, int ancho, Color fondo, Color letra)
        {
            boton.Text = texto;
            boton.Location = new Point(x, y);
            boton.Size = new Size(ancho, 36);
            boton.BackColor = fondo;
            boton.ForeColor = letra;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderColor = fondo == Color.White ? Color.Silver : fondo;
            boton.Cursor = Cursors.Hand;
        }

        // En el cuadro de ID solo se aceptan dígitos
        private void TxtId_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void BtnLimpiar_Click(object? sender, EventArgs e)
        {
            txtId.Clear();
            txtNombre.Clear();
            cmbEstado.SelectedIndex = 0;
            dtpDesde.Checked = false;
            dtpHasta.Checked = false;
            txtId.Focus();
        }

        private void BtnGenerar_Click(object? sender, EventArgs e)
        {
            // Validar el ID
            int? id = null;
            if (!string.IsNullOrWhiteSpace(txtId.Text))
            {
                if (!int.TryParse(txtId.Text.Trim(), out int idLeido) || idLeido <= 0)
                {
                    MessageBox.Show("El ID debe ser un número entero positivo.",
                        "Filtro inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtId.Focus();
                    return;
                }
                id = idLeido;
            }

            // Leer las fechas (solo si su casilla está marcada)
            DateTime? desde = null;
            DateTime? hasta = null;
            if (dtpDesde.Checked) desde = dtpDesde.Value.Date;
            if (dtpHasta.Checked) hasta = dtpHasta.Value.Date;

            if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
            {
                MessageBox.Show("La fecha \"desde\" no puede ser mayor que la fecha \"hasta\".",
                    "Rango de fechas inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Estado: 0 = Todos, 1 = Activo, 2 = Inactivo
            bool? activo = null;
            if (cmbEstado.SelectedIndex == 1) activo = true;
            else if (cmbEstado.SelectedIndex == 2) activo = false;

            Filtro = new FiltroCliente
            {
                IdCliente = id,
                Nombre = txtNombre.Text.Trim(),
                Activo = activo,
                Desde = desde,
                Hasta = hasta
            };

            DialogResult = DialogResult.OK;   // cierra la ventana devolviendo "OK"
        }
    }
}