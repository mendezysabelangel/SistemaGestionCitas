using System.Diagnostics;
using System.Drawing.Printing;
using System.Text;
using ClosedXML.Excel;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmReporteClientes : Form
    {
        private readonly ClienteService clienteService = new ClienteService();
        private readonly FiltroCliente filtroInicial;

        // datosBase    = lo que devolvió la base de datos con los filtros previos
        // datosMostrados = datosBase después de aplicar los buscadores del reporte
        private List<Cliente> datosBase = new List<Cliente>();
        private List<Cliente> datosMostrados = new List<Cliente>();

        private readonly Color azul = Color.FromArgb(27, 68, 170);
        private readonly string usuario = SesionActual.UsuarioActual?.NombreCompleto ?? "-";

        // Columnas del reporte (se usan en pantalla, impresión, Excel y CSV)
        private readonly string[] encabezados =
            { "ID", "Nombre completo", "Teléfono", "Correo", "Dirección", "Documento", "Estado", "Registro" };

        // Ancho relativo de cada columna al imprimir
        private readonly float[] pesos = { 5, 22, 14, 24, 25, 13, 9, 13 };

        // Para la impresión por páginas
        private int filaImpresa;
        private int numeroPagina;

        // ----- Controles -----
        private readonly TextBox txtBuscarId = new TextBox();
        private readonly TextBox txtBuscarNombre = new TextBox();
        private readonly ComboBox cmbEstado = new ComboBox();
        private readonly ComboBox cmbContacto = new ComboBox();
        private readonly DataGridView dgvReporte = new DataGridView();
        private readonly Label lblTotal = new Label();

        public FrmReporteClientes(FiltroCliente filtro)
        {
            InitializeComponent();
            filtroInicial = filtro;
            ConstruirInterfaz();
            Load += FrmReporteClientes_Load;
        }

        // =====================================================
        //  INTERFAZ
        // =====================================================
        private void ConstruirInterfaz()
        {
            SuspendLayout();
            Text = "Reporte de clientes";
            var fuente = new Font("Segoe UI", 10.5F);
            var tituloFuente = new Font("Segoe UI", 18F, FontStyle.Bold);
            Font = fuente;
            Disposed += (_, _) => { tituloFuente.Dispose(); fuente.Dispose(); };
            BackColor = EstiloAgenda.Barra;
            ClientSize = new Size(1050, 640);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);

            var superficie = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14),
                ColumnCount = 1, RowCount = 5 };
            superficie.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            superficie.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            superficie.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            superficie.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            superficie.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            superficie.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var encabezado = DialogoCitorna.Tabla();
            Label titulo = CampoCitorna.Rotulo("Reporte de clientes");
            titulo.Font = tituloFuente;
            encabezado.Controls.Add(titulo);
            encabezado.Controls.Add(CampoCitorna.Rotulo(filtroInicial.Descripcion(), true));
            encabezado.Controls.Add(CampoCitorna.Rotulo("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                + "  ·  Usuario: " + usuario, true));
            superficie.Controls.Add(encabezado, 0, 0);

            txtBuscarId.MaxLength = 9;
            txtBuscarId.KeyPress += TxtBuscarId_KeyPress;
            txtBuscarId.TextChanged += (_, _) => AplicarFiltros();
            txtBuscarNombre.TextChanged += (_, _) => AplicarFiltros();
            cmbEstado.DropDownStyle = cmbContacto.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cmbContacto.Items.AddRange(new object[] { "Todos", "Con correo", "Sin correo" });
            cmbEstado.SelectedIndex = cmbContacto.SelectedIndex = 0;
            cmbEstado.SelectedIndexChanged += (_, _) => AplicarFiltros();
            cmbContacto.SelectedIndexChanged += (_, _) => AplicarFiltros();
            var filtros = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 8, 0, 7) };
            filtros.Controls.Add(CampoFiltro("ID", txtBuscarId, 88));
            filtros.Controls.Add(CampoFiltro("Nombre", txtBuscarNombre, 190));
            filtros.Controls.Add(CampoFiltro("Estado", cmbEstado, 115));
            filtros.Controls.Add(CampoFiltro("Contacto", cmbContacto, 130));
            var limpiar = Accion("Limpiar", BtnLimpiar_Click);
            limpiar.Margin = new Padding(3, 20, 3, 0);
            filtros.Controls.Add(limpiar);
            superficie.Controls.Add(filtros, 0, 1);

            ConfigurarTabla();
            superficie.Controls.Add(dgvReporte, 0, 2);
            lblTotal.Margin = new Padding(3, 4, 0, 5);
            lblTotal.AutoSize = true;
            lblTotal.ForeColor = EstiloAgenda.Secundario;
            lblTotal.Text = "Total de registros: 0";
            superficie.Controls.Add(lblTotal, 0, 3);

            var acciones = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Margin = Padding.Empty };
            acciones.Controls.Add(Accion("Vista previa", BtnVistaPrevia_Click));
            acciones.Controls.Add(Accion("Imprimir", BtnImprimir_Click, true));
            acciones.Controls.Add(Accion("Exportar PDF", BtnExportarPdf_Click));
            acciones.Controls.Add(Accion("Exportar Excel", BtnExportarExcel_Click));
            acciones.Controls.Add(Accion("Exportar CSV", BtnExportarCsv_Click));
            acciones.Controls.Add(Accion("Cerrar", (_, _) => Close()));
            superficie.Controls.Add(acciones, 0, 4);
            Controls.Add(superficie);
            ResumeLayout(true);
        }

        private static Control CampoFiltro(string nombre, Control campo, int ancho)
        {
            var celda = new TableLayoutPanel { Width = ancho, Height = 59, ColumnCount = 1, RowCount = 2 };
            celda.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            celda.RowStyles.Add(new RowStyle(SizeType.Absolute, 21));
            celda.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            celda.Controls.Add(CampoCitorna.Rotulo(nombre), 0, 0);
            celda.Controls.Add(CampoCitorna.Enmarcar(campo, nombre), 0, 1);
            return celda;
        }

        private static Button Accion(string texto, EventHandler alHacerClic, bool primaria = false)
        {
            Button boton = primaria ? ControlesRegistro.Boton(texto, ControlesRegistro.Primario, Color.White)
                : ControlesRegistro.Boton(texto);
            boton.Click += alHacerClic;
            return boton;
        }

        private void ConfigurarTabla()
        {
            dgvReporte.Dock = DockStyle.Fill;
            dgvReporte.ReadOnly = true;
            dgvReporte.AllowUserToAddRows = false;
            dgvReporte.AllowUserToDeleteRows = false;
            dgvReporte.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReporte.AutoGenerateColumns = false;
            dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReporte.BackgroundColor = Color.White;
            dgvReporte.RowHeadersVisible = false;
            dgvReporte.EnableHeadersVisualStyles = false;
            dgvReporte.BorderStyle = BorderStyle.None;
            dgvReporte.GridColor = EstiloAgenda.Borde;
            dgvReporte.ColumnHeadersDefaultCellStyle.BackColor = EstiloAgenda.Fondo;
            dgvReporte.ColumnHeadersDefaultCellStyle.ForeColor = EstiloAgenda.Tinta;
            dgvReporte.DefaultCellStyle.SelectionBackColor = EstiloAgenda.Seleccion;
            dgvReporte.DefaultCellStyle.SelectionForeColor = EstiloAgenda.Tinta;
            dgvReporte.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 244, 252);

            AgregarColumna("IdCliente", encabezados[0], 5);
            AgregarColumna("NombreCompleto", encabezados[1], 22);
            AgregarColumna("Telefono", encabezados[2], 14);
            AgregarColumna("Correo", encabezados[3], 24);
            AgregarColumna("Direccion", encabezados[4], 25);
            AgregarColumna("Documento", encabezados[5], 13);
            AgregarColumna("Estado", encabezados[6], 9);
            AgregarColumna("FechaRegistro", encabezados[7], 13, "dd/MM/yyyy");
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
            dgvReporte.Columns.Add(col);
        }

        // =====================================================
        //  CARGA Y FILTROS DINÁMICOS
        // =====================================================
        private void FrmReporteClientes_Load(object? sender, EventArgs e)
        {
            try
            {
                // Filtros previos: se aplican en SQL Server (consulta dinámica)
                datosBase = clienteService.Buscar(
                    filtroInicial.IdCliente,
                    filtroInicial.Nombre,
                    filtroInicial.Activo,
                    filtroInicial.Desde,
                    filtroInicial.Hasta);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar el reporte.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                datosBase = new List<Cliente>();
            }

            AplicarFiltros();
        }

        // Filtros del reporte: se aplican en memoria con LINQ, sin volver a la base de datos
        private void AplicarFiltros()
        {
            IEnumerable<Cliente> consulta = datosBase;

            // Buscador por ID (coincidencia exacta)
            string textoId = txtBuscarId.Text.Trim();
            if (textoId != "")
            {
                if (int.TryParse(textoId, out int id))
                    consulta = consulta.Where(c => c.IdCliente == id);
                else
                    consulta = Enumerable.Empty<Cliente>();
            }

            // Buscador por nombre (contiene, sin distinguir mayúsculas)
            string nombre = txtBuscarNombre.Text.Trim();
            if (nombre != "")
            {
                consulta = consulta.Where(c =>
                    c.NombreCompleto.Contains(nombre, StringComparison.CurrentCultureIgnoreCase));
            }

            // Filtro dinámico: estado
            if (cmbEstado.SelectedIndex == 1)
                consulta = consulta.Where(c => c.Activo);
            else if (cmbEstado.SelectedIndex == 2)
                consulta = consulta.Where(c => !c.Activo);

            // Filtro dinámico: tiene o no correo
            if (cmbContacto.SelectedIndex == 1)
                consulta = consulta.Where(c => !string.IsNullOrWhiteSpace(c.Correo));
            else if (cmbContacto.SelectedIndex == 2)
                consulta = consulta.Where(c => string.IsNullOrWhiteSpace(c.Correo));

            datosMostrados = consulta.ToList();
            dgvReporte.DataSource = datosMostrados;
            lblTotal.Text = $"Total de registros: {datosMostrados.Count} de {datosBase.Count}";
        }

        private void BtnLimpiar_Click(object? sender, EventArgs e)
        {
            txtBuscarId.Clear();
            txtBuscarNombre.Clear();
            cmbEstado.SelectedIndex = 0;
            cmbContacto.SelectedIndex = 0;
            AplicarFiltros();
        }

        private void TxtBuscarId_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // Una fila del reporte convertida en textos (la usan impresión, CSV y Excel)
        private string[] ValoresFila(Cliente c)
        {
            return new[]
            {
                c.IdCliente.ToString(),
                c.NombreCompleto,
                c.Telefono,
                c.Correo ?? "",
                c.Direccion,
                c.Documento ?? "",
                c.Estado,
                c.FechaRegistro.ToString("dd/MM/yyyy")
            };
        }

        private bool HayDatos()
        {
            if (datosMostrados.Count > 0) return true;

            MessageBox.Show("No hay registros en el reporte para imprimir o exportar.",
                "Reporte vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        // =====================================================
        //  IMPRESIÓN
        // =====================================================
        private PrintDocument CrearDocumento()
        {
            PrintDocument doc = new PrintDocument();
            doc.DocumentName = "Reporte de clientes";
            doc.DefaultPageSettings.Landscape = true;   // hoja horizontal
            doc.BeginPrint += (s, e) => { filaImpresa = 0; numeroPagina = 0; };
            doc.PrintPage += Documento_PrintPage;
            return doc;
        }

        // Se ejecuta una vez por cada página que se imprime
        private void Documento_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics!;
            numeroPagina++;

            using Font fuenteTitulo = new Font("Segoe UI", 16F, FontStyle.Bold);
            using Font fuenteNormal = new Font("Segoe UI", 9F);
            using Font fuenteNegrita = new Font("Segoe UI", 9F, FontStyle.Bold);
            using Brush pincelAzul = new SolidBrush(azul);
            using StringFormat formato = new StringFormat
            {
                Trimming = StringTrimming.None,
                FormatFlags = StringFormatFlags.LineLimit
            };

            Rectangle m = e.MarginBounds;   // área imprimible dentro de los márgenes
            float y = m.Top;

            // Encabezado
            g.DrawString("Reporte de Clientes", fuenteTitulo, Brushes.Black, m.Left, y);
            y += fuenteTitulo.GetHeight(g) + 4;
            g.DrawString(filtroInicial.Descripcion(), fuenteNormal, Brushes.DimGray, m.Left, y);
            y += fuenteNormal.GetHeight(g);
            g.DrawString("Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "  ·  Usuario: " + usuario,
                fuenteNormal, Brushes.DimGray, m.Left, y);
            y += fuenteNormal.GetHeight(g) + 12;

            // Fila de títulos de columnas
            float altoEncabezado = CalcularAltoFila(g, encabezados, fuenteNegrita, m, formato);
            DibujarFila(g, encabezados, fuenteNegrita, Brushes.White, pincelAzul, m, y, altoEncabezado, formato);
            y += altoEncabezado;

            // Filas de datos: se dibujan hasta que se acabe la página o los registros
            float limite = m.Bottom - fuenteNormal.GetHeight(g) - 10;
            while (filaImpresa < datosMostrados.Count)
            {
                string[] valores = ValoresFila(datosMostrados[filaImpresa]);
                float altoFila = CalcularAltoFila(g, valores, fuenteNormal, m, formato);
                if (y + altoFila > limite) break;

                DibujarFila(g, valores, fuenteNormal,
                    Brushes.Black, null, m, y, altoFila, formato);
                y += altoFila;
                filaImpresa++;
            }

            // Pie de página
            bool hayMasPaginas = filaImpresa < datosMostrados.Count;
            string pie = "Página " + numeroPagina;
            if (!hayMasPaginas)
                pie += "  ·  Total de registros: " + datosMostrados.Count;

            g.DrawString(pie, fuenteNormal, Brushes.DimGray, m.Left, m.Bottom - fuenteNormal.GetHeight(g));

            // Si queda algo por imprimir, Windows vuelve a llamar a este método para la página siguiente
            e.HasMorePages = hayMasPaginas;
        }

        // La dirección puede ocupar varias líneas; se imprime completa sin cortar la fila.
        private float CalcularAltoFila(Graphics g, string[] valores, Font fuente, Rectangle m, StringFormat formato)
        {
            float totalPesos = pesos.Sum();
            float alto = fuente.GetHeight(g);

            for (int i = 0; i < valores.Length; i++)
            {
                float ancho = m.Width * pesos[i] / totalPesos - 6;
                SizeF medida = g.MeasureString(valores[i], fuente, new SizeF(ancho, float.MaxValue), formato);
                alto = Math.Max(alto, medida.Height);
            }

            return (float)Math.Ceiling(alto) + 6;
        }

        private void DibujarFila(Graphics g, string[] valores, Font fuente, Brush colorTexto, Brush? fondo,
                                 Rectangle m, float y, float alto, StringFormat formato)
        {
            float totalPesos = pesos.Sum();
            float x = m.Left;

            if (fondo != null)
                g.FillRectangle(fondo, m.Left, y, m.Width, alto);

            for (int i = 0; i < valores.Length; i++)
            {
                float ancho = m.Width * pesos[i] / totalPesos;
                g.DrawString(valores[i], fuente, colorTexto,
                    new RectangleF(x + 3, y + 3, ancho - 6, alto - 3), formato);
                x += ancho;
            }

            g.DrawLine(Pens.LightGray, m.Left, y + alto, m.Right, y + alto);
        }

        private void BtnVistaPrevia_Click(object? sender, EventArgs e)
        {
            if (!HayDatos()) return;

            using PrintDocument doc = CrearDocumento();
            using PrintPreviewDialog vista = new PrintPreviewDialog
            {
                Document = doc,
                Width = 1100,
                Height = 750,
                StartPosition = FormStartPosition.CenterScreen
            };
            vista.ShowDialog(this);
        }

        private void BtnImprimir_Click(object? sender, EventArgs e)
        {
            if (!HayDatos()) return;

            try
            {
                using PrintDocument doc = CrearDocumento();
                using PrintDialog dialogo = new PrintDialog { Document = doc };

                if (dialogo.ShowDialog(this) == DialogResult.OK)
                    doc.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo imprimir.\n\n" + ex.Message,
                    "Error de impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        //  EXPORTACIÓN
        // =====================================================

        // Pregunta dónde guardar el archivo. Devuelve null si el usuario cancela.
        private string? PedirRuta(string filtro, string extension)
        {
            using SaveFileDialog dialogo = new SaveFileDialog
            {
                Title = "Guardar reporte",
                Filter = filtro,
                DefaultExt = extension,
                FileName = "Reporte_Clientes_" + DateTime.Now.ToString("yyyyMMdd_HHmm")
            };

            return dialogo.ShowDialog(this) == DialogResult.OK ? dialogo.FileName : null;
        }

        private void ExportacionTerminada(string ruta)
        {
            DialogResult respuesta = MessageBox.Show(
                "Archivo guardado correctamente:\n" + ruta + "\n\n¿Desea abrirlo ahora?",
                "Exportación completada", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (respuesta == DialogResult.Yes)
                Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
        }

        private void MostrarErrorExportacion(Exception ex)
        {
            MessageBox.Show(
                "No se pudo exportar el reporte.\n\n" + ex.Message +
                "\n\nSi el archivo está abierto en otro programa, ciérrelo e intente de nuevo.",
                "Error de exportación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ----- CSV -----
        private void BtnExportarCsv_Click(object? sender, EventArgs e)
        {
            if (!HayDatos()) return;

            string? ruta = PedirRuta("Archivo CSV (*.csv)|*.csv", "csv");
            if (ruta == null) return;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Join(",", encabezados.Select(EscaparCsv)));

                foreach (Cliente c in datosMostrados)
                    sb.AppendLine(string.Join(",", ValoresFila(c).Select(EscaparCsv)));

                // UTF8 con BOM para que Excel muestre bien las tildes y la ñ
                File.WriteAllText(ruta, sb.ToString(), new UTF8Encoding(true));
                ExportacionTerminada(ruta);
            }
            catch (Exception ex)
            {
                MostrarErrorExportacion(ex);
            }
        }

        // Si un valor tiene coma, comillas o salto de línea, se encierra entre comillas
        private static string EscaparCsv(string valor)
        {
            if (valor.Contains(',') || valor.Contains('"') || valor.Contains('\n'))
                return "\"" + valor.Replace("\"", "\"\"") + "\"";

            return valor;
        }

        // ----- Excel (.xlsx) con ClosedXML -----
        private void BtnExportarExcel_Click(object? sender, EventArgs e)
        {
            if (!HayDatos()) return;

            string? ruta = PedirRuta("Libro de Excel (*.xlsx)|*.xlsx", "xlsx");
            if (ruta == null) return;

            try
            {
                using (XLWorkbook libro = new XLWorkbook())
                {
                    IXLWorksheet hoja = libro.Worksheets.Add("Clientes");

                    // Título y datos del reporte
                    hoja.Cell(1, 1).Value = "Reporte de Clientes";
                    hoja.Cell(1, 1).Style.Font.Bold = true;
                    hoja.Cell(1, 1).Style.Font.FontSize = 16;
                    hoja.Cell(2, 1).Value = filtroInicial.Descripcion();
                    hoja.Cell(3, 1).Value = "Generado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "  ·  Usuario: " + usuario;

                    // Fila de encabezados
                    int filaEncabezado = 5;
                    for (int i = 0; i < encabezados.Length; i++)
                        hoja.Cell(filaEncabezado, i + 1).Value = encabezados[i];

                    IXLRange rangoEncabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, encabezados.Length);
                    rangoEncabezado.Style.Font.Bold = true;
                    rangoEncabezado.Style.Font.FontColor = XLColor.White;
                    rangoEncabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#1B44AA");

                    // Filas de datos
                    int fila = filaEncabezado + 1;
                    foreach (Cliente c in datosMostrados)
                    {
                        string[] valores = ValoresFila(c);

                        hoja.Cell(fila, 1).Value = c.IdCliente;   // el ID se guarda como número
                        for (int i = 1; i < valores.Length; i++)
                            hoja.Cell(fila, i + 1).Value = valores[i];

                        fila++;
                    }

                    // Ajustar el ancho de las columnas a su contenido
                    for (int i = 1; i <= encabezados.Length; i++)
                    {
                        hoja.Column(i).AdjustToContents(filaEncabezado, fila - 1);
                    }
                    libro.SaveAs(ruta);
                }

                ExportacionTerminada(ruta);
            }
            catch (Exception ex)
            {
                MostrarErrorExportacion(ex);
            }
        }

        // ----- PDF (usando la impresora virtual de Windows) -----
        private void BtnExportarPdf_Click(object? sender, EventArgs e)
        {
            if (!HayDatos()) return;

            string? ruta = PedirRuta("Archivo PDF (*.pdf)|*.pdf", "pdf");
            if (ruta == null) return;

            try
            {
                using PrintDocument doc = CrearDocumento();
                doc.PrinterSettings.PrinterName = "Microsoft Print to PDF";

                if (!doc.PrinterSettings.IsValid)
                {
                    MessageBox.Show(
                        "No se encontró la impresora \"Microsoft Print to PDF\" en este equipo.\n\n" +
                        "Puede usar \"Exportar Excel\" o \"Exportar CSV\".",
                        "PDF no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                doc.PrinterSettings.PrintToFile = true;       // imprimir a un archivo...
                doc.PrinterSettings.PrintFileName = ruta;     // ...en esta ruta
                doc.Print();

                ExportacionTerminada(ruta);
            }
            catch (Exception ex)
            {
                MostrarErrorExportacion(ex);
            }
        }
    }
}
