using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Forms
{
    // Ventana modal: el usuario elige los filtros y, si acepta, quien la abrió lee la propiedad Filtro
    public partial class FrmFiltroReporteClientes : Form
    {
        public FiltroCliente Filtro { get; private set; } = new FiltroCliente();

        private readonly TextBox txtId = new TextBox();
        private readonly TextBox txtNombre = new TextBox();
        private readonly ComboBox cmbEstado = new ComboBox();
        private readonly DateTimePicker dtpDesde = new DateTimePicker();
        private readonly DateTimePicker dtpHasta = new DateTimePicker();
        private readonly Button btnGenerar = ControlesRegistro.Boton("Generar reporte", ControlesRegistro.Primario, Color.White);
        private readonly Button btnLimpiar = ControlesRegistro.Boton("Limpiar");
        private readonly Button btnCancelar = ControlesRegistro.Boton("Cancelar");

        public FrmFiltroReporteClientes()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            txtId.MaxLength = 9;
            txtId.KeyPress += TxtId_KeyPress;
            txtNombre.MaxLength = 80;
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cmbEstado.SelectedIndex = 0;
            ConfigurarFecha(dtpDesde);
            ConfigurarFecha(dtpHasta);
            btnGenerar.Click += BtnGenerar_Click;
            btnLimpiar.Click += BtnLimpiar_Click;

            var contenido = DialogoCitorna.Campos(2,
                ("ID del cliente", txtId, 40), ("Nombre contiene", txtNombre, 40),
                ("Estado", cmbEstado, 40), ("Registrados desde", dtpDesde, 40), ("Registrados hasta", dtpHasta, 40));
            DialogoCitorna.Configurar(this, "Filtros del reporte", "Deja en blanco los filtros que no quieras aplicar.",
                new Size(625, 420), contenido, btnGenerar, btnCancelar, btnLimpiar);
        }

        private static void ConfigurarFecha(DateTimePicker fecha)
        {
            fecha.Format = DateTimePickerFormat.Short;
            fecha.ShowCheckBox = true;
            fecha.MaxDate = DateTime.Today;
            fecha.Checked = false;
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