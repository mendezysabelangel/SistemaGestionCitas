using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Globalization;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public class FrmReporteInventario : Form
    {
        private static readonly Color FondoIcono = Color.FromArgb(238, 243, 248);
        private static readonly Color FondoSinExistencias = Color.FromArgb(252, 229, 232);
        private static readonly Color TintaSinExistencias = Color.FromArgb(136, 43, 57);
        private static readonly Color FilaSinExistencias = Color.FromArgb(254, 246, 247);
        private static readonly Color FondoStockBajo = Color.FromArgb(253, 239, 211);
        private static readonly Color TintaStockBajo = Color.FromArgb(111, 68, 17);
        private static readonly Color FilaStockBajo = Color.FromArgb(255, 251, 242);
        private static readonly Color FondoDisponible = Color.FromArgb(221, 242, 234);
        private static readonly Color TintaDisponible = Color.FromArgb(0, 82, 67);
        private static readonly Color TintaActivo = Color.FromArgb(0, 90, 73);

        private readonly ReporteInventarioService servicio = new();
        private readonly FiltroReporteInventario? filtroInicial;

        private readonly Font fuenteBase = new("Segoe UI", 10F);
        private readonly Font fuenteTitulo = new("Segoe UI", 20F, FontStyle.Bold);
        private readonly Font fuenteKpiValor = new("Segoe UI", 15.5F, FontStyle.Bold);
        private readonly Font fuenteKpiEtiqueta = new("Segoe UI", 9.5F, FontStyle.Bold);
        private readonly Font fuentePequena = new("Segoe UI", 9F);
        private readonly Font fuenteTabla = new("Segoe UI", 9.75F);
        private readonly Font fuenteEncabezadoTabla = new("Segoe UI", 9.5F, FontStyle.Bold);
        private readonly Font fuenteInsignia = new("Segoe UI", 9F, FontStyle.Bold);
        private readonly Font fuenteNegritaTabla = new("Segoe UI", 9.75F, FontStyle.Bold);

        private readonly SuperficieReporte superficie = new();
        private readonly TableLayoutPanel pagina = new();
        private readonly TableLayoutPanel panelResumen = new();
        private readonly TableLayoutPanel panelFiltros = new();
        private readonly TarjetaAgenda[] tarjetasResumen = new TarjetaAgenda[4];

        private readonly Label lblMetadatos = new();
        private readonly Label lblResumenCantidad = new();
        private readonly Label lblResumenUnidades = new();
        private readonly Label lblResumenSinStock = new();
        private readonly Label lblResumenStockBajo = new();
        private readonly Label lblResumenValor = new();

        private readonly TextBox txtBusqueda = ControlesRegistro.Texto(100);
        private readonly ComboBox cmbCategoria = ControlesRegistro.Opciones("Todas");
        private readonly ComboBox cmbEstado = ControlesRegistro.Opciones("Todos", "Activo", "Inactivo");
        private readonly ComboBox cmbNivelStock = ControlesRegistro.Opciones(
            FiltroReporteInventario.EtiquetaNivel(NivelExistencias.Todos),
            FiltroReporteInventario.EtiquetaNivel(NivelExistencias.SinExistencias),
            FiltroReporteInventario.EtiquetaNivel(NivelExistencias.StockBajo),
            FiltroReporteInventario.EtiquetaNivel(NivelExistencias.Disponible));

        private Control campoBusqueda = null!;
        private Control campoCategoria = null!;
        private Control campoEstado = null!;
        private Control campoNivelStock = null!;
        private Control grupoBotonesFiltro = null!;

        private readonly Button btnConsultar = ControlesRegistro.Boton("Consultar");
        private readonly Button btnLimpiar = ControlesRegistro.Boton("Limpiar");
        private readonly Button btnExportarExcel = ControlesRegistro.Boton("Exportar Excel", ControlesRegistro.Primario, Color.White);
        private readonly Button btnExportarCsv = ControlesRegistro.Boton("Exportar CSV");
        private readonly Button btnCerrar = ControlesRegistro.Boton("Cerrar");

        private readonly RejillaRegistros tabla = new();
        private readonly TableLayoutPanel panelEstadoTabla = new();
        private readonly Label lblMensajeTabla = new();
        private readonly Button btnAccionTabla = ControlesRegistro.Boton("Reintentar");
        private readonly Label lblEstadoPie = new();
        private readonly Label lblCriterioStock = new();

        private readonly System.Windows.Forms.Timer temporizadorBusqueda = new() { Interval = 350 };

        private ResultadoReporteInventario? reporteActual;
        private IReadOnlyList<ItemReporteInventario> items = Array.Empty<ItemReporteInventario>();
        private Action? accionTablaActual;
        private FiltroReporteInventario? solicitudPendiente;
        private bool categoriasCargadas;
        private bool actualizandoFiltros;
        private bool consultando;
        private bool exportando;
        private bool datosVigentes;
        private bool ultimoErrorConexion;
        private bool destruido;
        private bool? disposicionCompacta;

        public FrmReporteInventario()
        {
            ConstruirInterfaz();
        }

        public FrmReporteInventario(FiltroReporteInventario filtroInicial) : this()
        {
            this.filtroInicial = filtroInicial;
            AplicarFiltroInicialEnControles(filtroInicial);
        }

        private void ConstruirInterfaz()
        {
            SuspendLayout();
            Name = "FrmReporteInventario";
            Text = "Reporte de inventario";
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1180, 700);
            MinimumSize = new Size(780, 560);
            Font = fuenteBase;
            ForeColor = EstiloAgenda.Tinta;
            BackColor = EstiloAgenda.Fondo;

            superficie.Dock = DockStyle.Fill;
            superficie.AutoScroll = true;
            superficie.Resize += (_, _) => AjustarDisposicion();

            pagina.Dock = DockStyle.Top;
            pagina.BackColor = Color.Transparent;
            pagina.Margin = Padding.Empty;
            pagina.Padding = new Padding(20, 14, 20, 14);
            pagina.ColumnCount = 1;
            pagina.RowCount = 4;
            pagina.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pagina.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pagina.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pagina.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pagina.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            pagina.Controls.Add(ConstruirCabecera(), 0, 0);
            pagina.Controls.Add(ConstruirResumen(), 0, 1);
            pagina.Controls.Add(ConstruirFiltros(), 0, 2);
            pagina.Controls.Add(ConstruirTablaYEstado(), 0, 3);

            superficie.Controls.Add(pagina);
            Controls.Add(superficie);

            temporizadorBusqueda.Tick += (_, _) =>
            {
                temporizadorBusqueda.Stop();
                if (!destruido) SolicitarConsulta();
            };

            Shown += (_, _) =>
            {
                tabla.RowTemplate.Height = LogicalToDeviceUnits(38);
                AjustarDisposicion();
                SolicitarConsulta();
            };

            AjustarDisposicion();
            ActualizarEstadoBotones();
            ResumeLayout(true);
        }

        private Control ConstruirCabecera()
        {
            var cabecera = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 0, 0, 10)
            };
            cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            cabecera.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var titulos = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = Color.Transparent,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty
            };
            titulos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            titulos.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            titulos.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblTitulo = new Label
            {
                Text = "Reporte de inventario",
                Font = fuenteTitulo,
                ForeColor = EstiloAgenda.Tinta,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 1)
            };

            lblMetadatos.Text = "Existencias, alertas de stock y valoración";
            lblMetadatos.ForeColor = EstiloAgenda.Secundario;
            lblMetadatos.AutoSize = true;
            lblMetadatos.Dock = DockStyle.Fill;
            lblMetadatos.AutoEllipsis = true;
            lblMetadatos.Margin = Padding.Empty;

            titulos.Controls.Add(lblTitulo, 0, 0);
            titulos.Controls.Add(lblMetadatos, 0, 1);

            var acciones = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                BackColor = Color.Transparent,
                Margin = new Padding(12, 4, 0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            btnExportarExcel.Margin = new Padding(0, 0, 6, 0);
            btnExportarCsv.Margin = new Padding(0, 0, 6, 0);
            btnCerrar.Margin = Padding.Empty;

            btnExportarExcel.Click += async (_, _) => await ExportarAsync(esExcel: true);
            btnExportarCsv.Click += async (_, _) => await ExportarAsync(esExcel: false);
            btnCerrar.Click += (_, _) => Close();

            acciones.Controls.Add(btnExportarExcel);
            acciones.Controls.Add(btnExportarCsv);
            acciones.Controls.Add(btnCerrar);

            cabecera.Controls.Add(titulos, 0, 0);
            cabecera.Controls.Add(acciones, 1, 0);
            return cabecera;
        }

        private Control ConstruirResumen()
        {
            panelResumen.Dock = DockStyle.Fill;
            panelResumen.AutoSize = true;
            panelResumen.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelResumen.BackColor = Color.Transparent;
            panelResumen.Margin = new Padding(0, 0, 0, 10);

            tarjetasResumen[0] = CrearTarjetaKpi(
                "Productos",
                lblResumenCantidad,
                lblResumenUnidades,
                "Total en el reporte",
                "Cantidad de productos",
                EstiloAgenda.Tinta,
                SimboloAgenda.Productos);

            tarjetasResumen[1] = CrearTarjetaKpi(
                "Sin existencias",
                lblResumenSinStock,
                null,
                "Stock igual a 0",
                "Productos sin existencias",
                TintaSinExistencias);

            tarjetasResumen[2] = CrearTarjetaKpi(
                "Stock bajo",
                lblResumenStockBajo,
                null,
                $"Entre 1 y {ProductoService.LimiteStockBajo} uds.",
                "Productos con stock bajo",
                TintaStockBajo);

            tarjetasResumen[3] = CrearTarjetaKpi(
                "Valor del inventario",
                lblResumenValor,
                null,
                "Precio × existencias",
                "Valor total del inventario",
                ControlesRegistro.Primario);

            return panelResumen;
        }

        private TarjetaAgenda CrearTarjetaKpi(
            string titulo,
            Label lblValor,
            Label? lblDetalleEditable,
            string textoDetalle,
            string nombreAccesible,
            Color colorValor,
            SimboloAgenda? simbolo = null)
        {
            var tarjeta = new TarjetaAgenda
            {
                Dock = DockStyle.Fill,
                Radio = 13,
                Padding = new Padding(12, 8, 12, 8),
                MinimumSize = new Size(0, 82)
            };

            var tablaCard = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ColumnCount = simbolo.HasValue ? 2 : 1,
                RowCount = 3,
                Margin = Padding.Empty
            };

            if (simbolo.HasValue)
            {
                tablaCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                tablaCard.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34));
            }
            else
            {
                tablaCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            }

            tablaCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablaCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tablaCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = fuenteKpiEtiqueta,
                ForeColor = EstiloAgenda.Secundario,
                AutoSize = true,
                Dock = DockStyle.Fill,
                AutoEllipsis = true,
                Margin = Padding.Empty
            };

            lblValor.Text = "—";
            lblValor.Font = fuenteKpiValor;
            lblValor.ForeColor = colorValor;
            lblValor.AccessibleName = nombreAccesible;
            lblValor.Dock = DockStyle.Fill;
            lblValor.TextAlign = ContentAlignment.MiddleLeft;
            lblValor.AutoEllipsis = true;
            lblValor.Margin = new Padding(0, 1, 0, 1);

            Label lblDetalle = lblDetalleEditable ?? new Label();
            lblDetalle.Text = textoDetalle;
            lblDetalle.Font = fuentePequena;
            lblDetalle.ForeColor = EstiloAgenda.Secundario;
            lblDetalle.AutoSize = false;
            lblDetalle.Height = 18;
            lblDetalle.Dock = DockStyle.Fill;
            lblDetalle.AutoEllipsis = true;
            lblDetalle.Margin = Padding.Empty;

            tablaCard.Controls.Add(lblTitulo, 0, 0);
            tablaCard.Controls.Add(lblValor, 0, 1);
            tablaCard.Controls.Add(lblDetalle, 0, 2);

            if (simbolo is { } iconoSimbolo)
            {
                var icono = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Margin = new Padding(4, 0, 0, 0),
                    TabStop = false
                };
                icono.Paint += (_, e) =>
                {
                    int caja = icono.LogicalToDeviceUnits(30);
                    var rectCaja = new Rectangle(Math.Max(0, (icono.Width - caja) / 2), 0, caja, caja);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using GraphicsPath forma = EstiloAgenda.RectanguloRedondo(rectCaja, icono.LogicalToDeviceUnits(8));
                    using SolidBrush fondo = new(FondoIcono);
                    e.Graphics.FillPath(fondo, forma);
                    int lado = icono.LogicalToDeviceUnits(17);
                    IconosAgenda.Dibujar(e.Graphics, iconoSimbolo, EstiloAgenda.Tinta,
                        new Rectangle(rectCaja.X + (caja - lado) / 2, rectCaja.Y + (caja - lado) / 2, lado, lado));
                };
                tablaCard.Controls.Add(icono, 1, 0);
                tablaCard.SetRowSpan(icono, 2);
            }

            tarjeta.Controls.Add(tablaCard);
            return tarjeta;
        }

        private Control ConstruirFiltros()
        {
            var tarjetaFiltros = new TarjetaAgenda
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Radio = 13,
                Padding = new Padding(14, 10, 14, 10),
                Margin = new Padding(0, 0, 0, 10)
            };

            txtBusqueda.PlaceholderText = "Buscar por nombre o código";
            txtBusqueda.TextChanged += (_, _) =>
            {
                if (actualizandoFiltros || ultimoErrorConexion) return;
                temporizadorBusqueda.Stop();
                temporizadorBusqueda.Start();
            };
            txtBusqueda.KeyDown += (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                temporizadorBusqueda.Stop();
                SolicitarConsulta();
            };

            cmbCategoria.SelectedIndexChanged += (_, _) => AlCambiarFiltroSelector();
            cmbEstado.SelectedIndexChanged += (_, _) => AlCambiarFiltroSelector();
            cmbNivelStock.SelectedIndexChanged += (_, _) => AlCambiarFiltroSelector();

            btnConsultar.Margin = new Padding(0, 0, 6, 0);
            btnConsultar.Click += (_, _) =>
            {
                temporizadorBusqueda.Stop();
                SolicitarConsulta();
            };

            btnLimpiar.Margin = Padding.Empty;
            btnLimpiar.Click += (_, _) => RestablecerFiltros();

            campoBusqueda = CrearCampoFiltro("Nombre o código", txtBusqueda);
            campoCategoria = CrearCampoFiltro("Categoría", cmbCategoria);
            campoEstado = CrearCampoFiltro("Estado", cmbEstado);
            campoNivelStock = CrearCampoFiltro("Nivel de existencias", cmbNivelStock);

            var botonesFiltro = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                BackColor = Color.White,
                Margin = new Padding(0, 21, 0, 0),
                Padding = Padding.Empty,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            botonesFiltro.Controls.Add(btnConsultar);
            botonesFiltro.Controls.Add(btnLimpiar);
            grupoBotonesFiltro = botonesFiltro;

            panelFiltros.Dock = DockStyle.Fill;
            panelFiltros.AutoSize = true;
            panelFiltros.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelFiltros.BackColor = Color.White;
            panelFiltros.Margin = Padding.Empty;

            tarjetaFiltros.Controls.Add(panelFiltros);
            return tarjetaFiltros;
        }

        private static Control CrearCampoFiltro(string nombre, Control campo)
        {
            var celda = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Height = 61,
                BackColor = Color.White,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty
            };
            celda.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            celda.RowStyles.Add(new RowStyle(SizeType.Absolute, 21));
            celda.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            celda.Controls.Add(CampoCitorna.Rotulo(nombre), 0, 0);
            celda.Controls.Add(CampoCitorna.Enmarcar(campo, nombre, 40), 0, 1);
            return celda;
        }

        private Control ConstruirTablaYEstado()
        {
            var tarjeta = new TarjetaAgenda
            {
                Dock = DockStyle.Fill,
                Radio = 14,
                Padding = new Padding(14, 12, 14, 10),
                Margin = Padding.Empty
            };

            var contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty
            };
            contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            ConfigurarTabla();
            ConfigurarPanelEstadoTabla();

            var cuerpoTabla = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = Padding.Empty
            };
            cuerpoTabla.Controls.Add(tabla);
            cuerpoTabla.Controls.Add(panelEstadoTabla);
            contenido.Controls.Add(cuerpoTabla, 0, 0);

            var pie = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                BackColor = Color.White,
                ColumnCount = 2,
                RowCount = 1,
                Margin = new Padding(0, 6, 0, 0)
            };
            pie.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            pie.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pie.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            lblEstadoPie.AutoSize = true;
            lblEstadoPie.Dock = DockStyle.Fill;
            lblEstadoPie.ForeColor = EstiloAgenda.Secundario;
            lblEstadoPie.Font = fuentePequena;
            lblEstadoPie.Margin = new Padding(0, 2, 12, 0);
            lblEstadoPie.Text = "Consultando inventario…";
            lblEstadoPie.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;

            lblCriterioStock.AutoSize = true;
            lblCriterioStock.ForeColor = EstiloAgenda.Secundario;
            lblCriterioStock.Font = fuentePequena;
            lblCriterioStock.Margin = new Padding(0, 2, 0, 0);
            lblCriterioStock.Text = $"Sin existencias: 0  ·  Stock bajo: 1 a {ProductoService.LimiteStockBajo} uds.";

            pie.Controls.Add(lblEstadoPie, 0, 0);
            pie.Controls.Add(lblCriterioStock, 1, 0);
            contenido.Controls.Add(pie, 0, 1);

            tarjeta.Controls.Add(contenido);
            return tarjeta;
        }

        private void ConfigurarTabla()
        {
            tabla.Dock = DockStyle.Fill;
            tabla.Margin = Padding.Empty;
            tabla.AccessibleName = "Listado del reporte de inventario";
            tabla.Font = fuenteTabla;
            tabla.RowTemplate.Height = 38;
            tabla.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 6, 6, 6);
            tabla.ColumnHeadersDefaultCellStyle.Font = fuenteEncabezadoTabla;
            tabla.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            tabla.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            tabla.AgregarColumna("Código", 10, 78);
            tabla.AgregarColumna("Producto", 24, 160);
            tabla.AgregarColumna("Categoría", 16, 125);
            tabla.AgregarColumna("Precio", 13, 105);
            tabla.AgregarColumna("Existencias", 11, 96);
            tabla.AgregarColumna("Valor", 14, 118);
            tabla.AgregarColumna("Nivel", 13, 118);
            tabla.AgregarColumna("Estado", 8, 72);

            AlinearColumna(3, DataGridViewContentAlignment.MiddleRight);
            AlinearColumna(4, DataGridViewContentAlignment.MiddleRight);
            AlinearColumna(5, DataGridViewContentAlignment.MiddleRight);
            AlinearColumna(6, DataGridViewContentAlignment.MiddleCenter);
            AlinearColumna(7, DataGridViewContentAlignment.MiddleCenter);

            tabla.CellValueNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < items.Count)
                    e.Value = ValorCelda(items[e.RowIndex], e.ColumnIndex);
            };

            tabla.CellToolTipTextNeeded += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < items.Count && e.ColumnIndex >= 0)
                    e.ToolTipText = ValorCelda(items[e.RowIndex], e.ColumnIndex) ?? "";
            };

            tabla.CellFormatting += Tabla_CellFormatting;
            tabla.CellPainting += Tabla_CellPainting;
            tabla.Visible = false;
        }

        private void AlinearColumna(int indice, DataGridViewContentAlignment alineacion)
        {
            tabla.Columns[indice].DefaultCellStyle.Alignment = alineacion;
            tabla.Columns[indice].HeaderCell.Style.Alignment = alineacion;
        }

        private void ConfigurarPanelEstadoTabla()
        {
            panelEstadoTabla.Dock = DockStyle.Fill;
            panelEstadoTabla.BackColor = Color.White;
            panelEstadoTabla.ColumnCount = 1;
            panelEstadoTabla.RowCount = 4;
            panelEstadoTabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panelEstadoTabla.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            panelEstadoTabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelEstadoTabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelEstadoTabla.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            lblMensajeTabla.AutoSize = true;
            lblMensajeTabla.MaximumSize = new Size(560, 0);
            lblMensajeTabla.ForeColor = EstiloAgenda.Secundario;
            lblMensajeTabla.TextAlign = ContentAlignment.MiddleCenter;
            lblMensajeTabla.Anchor = AnchorStyles.None;
            lblMensajeTabla.Margin = new Padding(16, 0, 16, 10);
            lblMensajeTabla.Text = "Consultando inventario…";
            lblMensajeTabla.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;

            btnAccionTabla.Anchor = AnchorStyles.None;
            btnAccionTabla.Visible = false;
            btnAccionTabla.Click += (_, _) => accionTablaActual?.Invoke();

            panelEstadoTabla.Controls.Add(lblMensajeTabla, 0, 1);
            panelEstadoTabla.Controls.Add(btnAccionTabla, 0, 2);
        }

        private void AjustarDisposicion()
        {
            if (pagina.Parent == null) return;

            bool compacto = superficie.ClientSize.Width < LogicalToDeviceUnits(900);
            int alturaMinima = LogicalToDeviceUnits(compacto ? 820 : 480);
            pagina.Height = Math.Max(superficie.ClientSize.Height, alturaMinima);

            if (disposicionCompacta == compacto) return;
            disposicionCompacta = compacto;

            int espacio = LogicalToDeviceUnits(10);

            // Resumen KPI responsivo (4 columnas en escritorio, 2x2 en compacto)
            panelResumen.SuspendLayout();
            panelResumen.Controls.Clear();
            panelResumen.ColumnStyles.Clear();
            panelResumen.RowStyles.Clear();

            if (compacto)
            {
                panelResumen.ColumnCount = 2;
                panelResumen.RowCount = 2;
                panelResumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                panelResumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                panelResumen.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panelResumen.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                for (int i = 0; i < tarjetasResumen.Length; i++)
                {
                    int col = i % 2;
                    int fila = i / 2;
                    tarjetasResumen[i].Margin = new Padding(0, 0, col == 0 ? espacio : 0, fila == 0 ? espacio : 0);
                    panelResumen.Controls.Add(tarjetasResumen[i], col, fila);
                }
            }
            else
            {
                panelResumen.ColumnCount = 4;
                panelResumen.RowCount = 1;
                float[] pesos = { 23F, 21F, 21F, 35F };
                panelResumen.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                for (int i = 0; i < tarjetasResumen.Length; i++)
                {
                    panelResumen.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, pesos[i]));
                    tarjetasResumen[i].Margin = new Padding(0, 0, i < tarjetasResumen.Length - 1 ? espacio : 0, 0);
                    panelResumen.Controls.Add(tarjetasResumen[i], i, 0);
                }
            }
            panelResumen.ResumeLayout(true);

            // Barra de filtros responsiva (1 fila en escritorio, 2 filas en compacto)
            panelFiltros.SuspendLayout();
            panelFiltros.Controls.Clear();
            panelFiltros.ColumnStyles.Clear();
            panelFiltros.RowStyles.Clear();

            if (compacto)
            {
                panelFiltros.ColumnCount = 3;
                panelFiltros.RowCount = 2;
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
                panelFiltros.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panelFiltros.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                campoBusqueda.Margin = new Padding(0, 0, espacio, espacio);
                campoCategoria.Margin = new Padding(0, 0, espacio, espacio);
                campoEstado.Margin = new Padding(0, 0, 0, espacio);
                campoNivelStock.Margin = new Padding(0, 0, espacio, 0);
                grupoBotonesFiltro.Margin = new Padding(0, LogicalToDeviceUnits(21), 0, 0);

                panelFiltros.Controls.Add(campoBusqueda, 0, 0);
                panelFiltros.Controls.Add(campoCategoria, 1, 0);
                panelFiltros.Controls.Add(campoEstado, 2, 0);
                panelFiltros.Controls.Add(campoNivelStock, 0, 1);
                panelFiltros.Controls.Add(grupoBotonesFiltro, 1, 1);
                panelFiltros.SetColumnSpan(grupoBotonesFiltro, 2);
            }
            else
            {
                panelFiltros.ColumnCount = 5;
                panelFiltros.RowCount = 1;
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18));
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
                panelFiltros.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                panelFiltros.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                campoBusqueda.Margin = new Padding(0, 0, espacio, 0);
                campoCategoria.Margin = new Padding(0, 0, espacio, 0);
                campoEstado.Margin = new Padding(0, 0, espacio, 0);
                campoNivelStock.Margin = new Padding(0, 0, espacio, 0);
                grupoBotonesFiltro.Margin = new Padding(0, LogicalToDeviceUnits(21), 0, 0);

                panelFiltros.Controls.Add(campoBusqueda, 0, 0);
                panelFiltros.Controls.Add(campoCategoria, 1, 0);
                panelFiltros.Controls.Add(campoEstado, 2, 0);
                panelFiltros.Controls.Add(campoNivelStock, 3, 0);
                panelFiltros.Controls.Add(grupoBotonesFiltro, 4, 0);
                panelFiltros.SetColumnSpan(grupoBotonesFiltro, 1);
            }
            panelFiltros.ResumeLayout(true);
        }

        private void AplicarFiltroInicialEnControles(FiltroReporteInventario filtro)
        {
            actualizandoFiltros = true;
            try
            {
                txtBusqueda.Text = filtro.Texto ?? "";
                cmbEstado.SelectedIndex = filtro.Activo switch
                {
                    true => 1,
                    false => 2,
                    null => 0
                };
                cmbNivelStock.SelectedIndex = (int)filtro.NivelExistencias;
            }
            finally
            {
                actualizandoFiltros = false;
            }
        }

        private FiltroReporteInventario ConstruirFiltroActual()
        {
            int? idCategoria = null;
            string? nombreCategoria = null;

            if (cmbCategoria.SelectedItem is CategoriaProducto categoria && categoria.IdCategoria > 0)
            {
                idCategoria = categoria.IdCategoria;
                nombreCategoria = categoria.Nombre;
            }
            else if (!categoriasCargadas && filtroInicial?.IdCategoria is { } idInicial && idInicial > 0)
            {
                idCategoria = idInicial;
                nombreCategoria = filtroInicial.NombreCategoria;
            }

            NivelExistencias nivel = Enum.IsDefined(typeof(NivelExistencias), cmbNivelStock.SelectedIndex)
                ? (NivelExistencias)cmbNivelStock.SelectedIndex
                : NivelExistencias.Todos;

            return new FiltroReporteInventario
            {
                Texto = txtBusqueda.Text.Trim(),
                IdCategoria = idCategoria,
                NombreCategoria = nombreCategoria,
                Activo = ControlesRegistro.Estado(cmbEstado.SelectedIndex),
                NivelExistencias = nivel
            };
        }

        private void AlCambiarFiltroSelector()
        {
            if (actualizandoFiltros) return;
            temporizadorBusqueda.Stop();
            SolicitarConsulta();
        }

        private void RestablecerFiltros()
        {
            temporizadorBusqueda.Stop();
            actualizandoFiltros = true;
            try
            {
                txtBusqueda.Clear();
                if (cmbCategoria.Items.Count > 0) cmbCategoria.SelectedIndex = 0;
                cmbEstado.SelectedIndex = 0;
                cmbNivelStock.SelectedIndex = 0;
            }
            finally
            {
                actualizandoFiltros = false;
            }

            SolicitarConsulta();
        }

        private async void SolicitarConsulta()
        {
            await EjecutarConsultaAsync(ConstruirFiltroActual());
        }

        private async Task EjecutarConsultaAsync(FiltroReporteInventario filtro)
        {
            if (destruido) return;

            if (!ReporteInventarioService.TienePermisoConsulta())
            {
                MostrarSinPermiso();
                return;
            }

            solicitudPendiente = filtro;
            datosVigentes = false;

            if (items.Count == 0)
                MostrarEstadoEnTabla("Consultando inventario…");
            else
                lblEstadoPie.Text = "Actualizando inventario…";

            if (consultando) return;

            consultando = true;
            ActualizarEstadoBotones();
            try
            {
                while (!destruido && solicitudPendiente != null)
                {
                    FiltroReporteInventario filtroEnCurso = solicitudPendiente;
                    solicitudPendiente = null;
                    bool cargarCategorias = !categoriasCargadas;

                    var reloj = Stopwatch.StartNew();
                    try
                    {
                        var (resultado, categoriasObtenidas) = await Task.Run(() =>
                        {
                            List<CategoriaProducto>? listaCategorias = cargarCategorias
                                ? servicio.ObtenerCategorias()
                                : null;
                            ResultadoReporteInventario reporte = servicio.Consultar(filtroEnCurso);
                            return (reporte, listaCategorias);
                        });

                        if (destruido || solicitudPendiente != null) continue;
                        if (!ReporteInventarioService.TienePermisoConsulta())
                        {
                            MostrarSinPermiso();
                            return;
                        }

                        if (categoriasObtenidas != null)
                            VincularCategorias(categoriasObtenidas, filtroEnCurso.IdCategoria);

                        ultimoErrorConexion = false;
                        AplicarResultado(resultado);
                        Trace.TraceInformation("Citorna reporte inventario: consulta {0} ms, {1} productos.",
                            reloj.ElapsedMilliseconds, resultado.Items.Count);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        if (!destruido && solicitudPendiente == null)
                            MostrarSinPermiso();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("Citorna reporte inventario ({0}): {1}", ex.GetType().Name, ex.Message);
                        if (!destruido && solicitudPendiente == null)
                        {
                            ultimoErrorConexion = true;
                            MostrarErrorConsulta(ReporteInventarioService.MensajeErrorConsulta(ex));
                        }
                    }
                }
            }
            finally
            {
                consultando = false;
                if (!destruido) ActualizarEstadoBotones();
            }
        }

        private void VincularCategorias(List<CategoriaProducto> categoriasObtenidas, int? categoriaSeleccionada)
        {
            actualizandoFiltros = true;
            try
            {
                var opciones = new List<CategoriaProducto>
                {
                    new() { IdCategoria = 0, Nombre = "Todas" }
                };
                opciones.AddRange(categoriasObtenidas);

                cmbCategoria.DisplayMember = nameof(CategoriaProducto.Nombre);
                cmbCategoria.ValueMember = nameof(CategoriaProducto.IdCategoria);
                cmbCategoria.DataSource = opciones;
                categoriasCargadas = true;

                if (categoriaSeleccionada is { } id && opciones.Any(c => c.IdCategoria == id))
                    cmbCategoria.SelectedValue = id;
                else
                    cmbCategoria.SelectedIndex = 0;
            }
            finally
            {
                actualizandoFiltros = false;
            }
        }

        private void AplicarResultado(ResultadoReporteInventario resultado)
        {
            reporteActual = resultado;
            items = resultado.Items;
            datosVigentes = true;

            string usuario = SesionActual.UsuarioActual?.NombreCompleto ?? "-";
            lblMetadatos.Text =
                $"Actualizado: {resultado.FechaGeneracion:dd/MM/yyyy HH:mm}  ·  Usuario: {usuario}";

            ActualizarTarjetasResumen(resultado.Resumen);

            tabla.SuspendLayout();
            try
            {
                tabla.RowCount = 0;
                tabla.RowCount = items.Count;
                tabla.ClearSelection();
                tabla.CurrentCell = null;
            }
            finally
            {
                tabla.ResumeLayout(true);
            }

            if (items.Count > 0)
            {
                panelEstadoTabla.Visible = false;
                tabla.Visible = true;
                string textoConteo = items.Count == 1
                    ? "1 producto encontrado"
                    : $"{items.Count:N0} productos encontrados";
                lblEstadoPie.Text = $"{textoConteo}  ·  {resultado.Filtro.Descripcion()}";
            }
            else if (resultado.Filtro.TieneFiltros)
            {
                MostrarEstadoEnTabla(
                    "No se encontraron productos con los filtros aplicados.",
                    "Limpiar filtros",
                    RestablecerFiltros);
                lblEstadoPie.Text = $"0 productos encontrados  ·  {resultado.Filtro.Descripcion()}";
            }
            else
            {
                MostrarEstadoEnTabla("No hay productos registrados en el inventario.");
                lblEstadoPie.Text = "No hay productos registrados en el inventario.";
            }

            ActualizarEstadoBotones();
        }

        private void ActualizarTarjetasResumen(ResumenInventario? resumen)
        {
            if (resumen == null)
            {
                lblResumenCantidad.Text = "—";
                lblResumenUnidades.Text = "Sin datos disponibles";
                lblResumenSinStock.Text = "—";
                lblResumenSinStock.ForeColor = EstiloAgenda.Secundario;
                lblResumenStockBajo.Text = "—";
                lblResumenStockBajo.ForeColor = EstiloAgenda.Secundario;
                lblResumenValor.Text = "—";
                return;
            }

            lblResumenCantidad.Text = resumen.CantidadProductos.ToString("N0", CultureInfo.CurrentCulture);
            lblResumenUnidades.Text = resumen.UnidadesTotales == 1
                ? "1 unidad en total"
                : $"{resumen.UnidadesTotales:N0} unidades en total";

            lblResumenSinStock.Text = resumen.ProductosSinExistencias.ToString("N0", CultureInfo.CurrentCulture);
            lblResumenSinStock.ForeColor = resumen.ProductosSinExistencias > 0
                ? TintaSinExistencias
                : EstiloAgenda.Tinta;

            lblResumenStockBajo.Text = resumen.ProductosStockBajo.ToString("N0", CultureInfo.CurrentCulture);
            lblResumenStockBajo.ForeColor = resumen.ProductosStockBajo > 0
                ? TintaStockBajo
                : EstiloAgenda.Tinta;

            lblResumenValor.Text = FormatearMoneda(resumen.ValorInventario);
        }

        private void MostrarSinPermiso()
        {
            reporteActual = null;
            items = Array.Empty<ItemReporteInventario>();
            datosVigentes = false;
            tabla.RowCount = 0;
            ActualizarTarjetasResumen(null);

            const string mensaje = "No tiene permiso para consultar el reporte de inventario.";
            MostrarEstadoEnTabla(mensaje);
            lblEstadoPie.Text = mensaje;
            ActualizarEstadoBotones();
        }

        private void MostrarErrorConsulta(string mensaje)
        {
            reporteActual = null;
            items = Array.Empty<ItemReporteInventario>();
            datosVigentes = false;
            tabla.RowCount = 0;
            ActualizarTarjetasResumen(null);

            MostrarEstadoEnTabla(mensaje, "Reintentar", SolicitarConsulta);
            lblEstadoPie.Text = "Error de conexión al consultar el inventario.";
            ActualizarEstadoBotones();
        }

        private void MostrarEstadoEnTabla(string mensaje, string? textoAccion = null, Action? callback = null)
        {
            tabla.Visible = false;
            panelEstadoTabla.Visible = true;
            lblMensajeTabla.Text = mensaje;
            accionTablaActual = callback;
            if (textoAccion != null) btnAccionTabla.Text = textoAccion;
            btnAccionTabla.Visible = callback != null;
        }

        private void ActualizarEstadoBotones()
        {
            bool tienePermiso = ReporteInventarioService.TienePermisoConsulta();
            bool filtrosActivos = tienePermiso && !exportando;
            bool puedeExportar = tienePermiso && !consultando && !exportando && datosVigentes && items.Count > 0;

            txtBusqueda.Enabled = filtrosActivos;
            cmbCategoria.Enabled = filtrosActivos;
            cmbEstado.Enabled = filtrosActivos;
            cmbNivelStock.Enabled = filtrosActivos;
            btnConsultar.Enabled = tienePermiso && !consultando && !exportando;
            btnLimpiar.Enabled = filtrosActivos && !consultando;
            btnExportarExcel.Enabled = puedeExportar;
            btnExportarCsv.Enabled = puedeExportar;
        }

        private async Task ExportarAsync(bool esExcel)
        {
            if (exportando || consultando) return;

            if (!SesionActual.VerificarPermiso(PermisosSistema.Consultar, "exportar el reporte de inventario"))
            {
                MostrarSinPermiso();
                return;
            }

            // Si el usuario escribió en la búsqueda y pulsó exportar antes del debounce, sincronizamos primero.
            if (temporizadorBusqueda.Enabled)
            {
                temporizadorBusqueda.Stop();
                await EjecutarConsultaAsync(ConstruirFiltroActual());
                if (destruido) return;
            }

            if (reporteActual == null || !datosVigentes || reporteActual.Items.Count == 0)
            {
                MessageBox.Show(this,
                    "No hay productos en el reporte para exportar.",
                    "Reporte vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string filtroDialogo = esExcel
                ? "Libro de Excel (*.xlsx)|*.xlsx"
                : "Archivo CSV (*.csv)|*.csv";
            string extension = esExcel ? "xlsx" : "csv";

            string? ruta = PedirRutaExportacion(filtroDialogo, extension);
            if (ruta == null) return;

            ResultadoReporteInventario reporteAExportar = reporteActual;
            string usuario = SesionActual.UsuarioActual?.NombreCompleto ?? "-";

            exportando = true;
            ActualizarEstadoBotones();
            string pieAnterior = lblEstadoPie.Text;
            lblEstadoPie.Text = esExcel ? "Exportando reporte a Excel…" : "Exportando reporte a CSV…";

            try
            {
                await Task.Run(() =>
                {
                    if (esExcel)
                        servicio.ExportarExcel(reporteAExportar, ruta, usuario);
                    else
                        servicio.ExportarCsv(reporteAExportar, ruta);
                });

                if (destruido) return;
                lblEstadoPie.Text = pieAnterior;

                DialogResult respuesta = MessageBox.Show(this,
                    $"Archivo guardado correctamente:\n{ruta}\n\n¿Desea abrirlo ahora?",
                    "Exportación completada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (respuesta == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(ruta) { UseShellExecute = true });
                    }
                    catch (Exception exAbrir)
                    {
                        MessageBox.Show(this,
                            $"El archivo se guardó correctamente, pero no se pudo abrir automáticamente.\n\n{exAbrir.Message}",
                            "Abrir archivo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (!destruido)
                {
                    MostrarSinPermiso();
                    MessageBox.Show(this,
                        "Permisos insuficientes para exportar el reporte de inventario.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Citorna reporte inventario (exportación): {0}", ex.Message);
                if (!destruido)
                {
                    lblEstadoPie.Text = pieAnterior;
                    MessageBox.Show(this,
                        $"No se pudo exportar el reporte.\n\n{ex.Message}\n\n" +
                        "Si el archivo está abierto en otro programa, ciérrelo e intente de nuevo.",
                        "Error de exportación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            finally
            {
                exportando = false;
                if (!destruido) ActualizarEstadoBotones();
            }
        }

        private string? PedirRutaExportacion(string filtro, string extension)
        {
            using var dialogo = new SaveFileDialog
            {
                Title = "Guardar reporte de inventario",
                Filter = filtro,
                DefaultExt = extension,
                FileName = $"Reporte_Inventario_{DateTime.Now:yyyyMMdd_HHmm}"
            };

            return dialogo.ShowDialog(this) == DialogResult.OK ? dialogo.FileName : null;
        }

        private static string? ValorCelda(ItemReporteInventario item, int indiceColumna) => indiceColumna switch
        {
            0 => item.Codigo,
            1 => item.Nombre,
            2 => ControlesRegistro.Disponible(item.Categoria),
            3 => FormatearMoneda(item.Precio),
            4 => $"{item.Stock:N0} uds.",
            5 => FormatearMoneda(item.ValorInventario),
            6 => item.NivelExistenciasTexto,
            7 => item.Estado,
            _ => null
        };

        private static string FormatearMoneda(decimal monto) =>
            $"RD$ {monto.ToString("N2", CultureInfo.CurrentCulture)}";

        private static Color ColorFondoFila(ItemReporteInventario item) => item.NivelExistencias switch
        {
            NivelExistencias.SinExistencias => FilaSinExistencias,
            NivelExistencias.StockBajo => FilaStockBajo,
            _ => Color.White
        };

        private void Tabla_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= items.Count || e.CellStyle == null) return;

            ItemReporteInventario item = items[e.RowIndex];
            e.CellStyle.BackColor = ColorFondoFila(item);

            if (e.ColumnIndex == 4)
            {
                if (item.EsSinExistencias)
                {
                    e.CellStyle.ForeColor = TintaSinExistencias;
                    e.CellStyle.Font = fuenteNegritaTabla;
                }
                else if (item.EsStockBajo)
                {
                    e.CellStyle.ForeColor = TintaStockBajo;
                    e.CellStyle.Font = fuenteNegritaTabla;
                }
            }
            else if (e.ColumnIndex == 7)
            {
                e.CellStyle.ForeColor = item.Activo ? TintaActivo : EstiloAgenda.Secundario;
            }
        }

        private void Tabla_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= items.Count || e.ColumnIndex != 6 || e.Graphics is not { } g)
                return;

            ItemReporteInventario item = items[e.RowIndex];
            bool seleccionado = (e.State & DataGridViewElementStates.Selected) != 0;
            Rectangle bordes = e.CellBounds;

            GraphicsState estadoDibujo = g.Save();
            g.SetClip(Rectangle.Intersect(bordes, e.ClipBounds));

            Color fondoCelda = seleccionado ? EstiloAgenda.Seleccion : ColorFondoFila(item);
            using (var pincelFondo = new SolidBrush(fondoCelda))
                g.FillRectangle(pincelFondo, bordes);

            (Color fondoPill, Color tintaPill) = item.NivelExistencias switch
            {
                NivelExistencias.SinExistencias => (FondoSinExistencias, TintaSinExistencias),
                NivelExistencias.StockBajo => (FondoStockBajo, TintaStockBajo),
                _ => (FondoDisponible, TintaDisponible)
            };

            string texto = item.NivelExistenciasTexto;
            int margen = LogicalToDeviceUnits(6);
            int alto = LogicalToDeviceUnits(24);
            int anchoTexto = TextRenderer.MeasureText(texto, fuenteInsignia).Width;
            int ancho = Math.Max(1, Math.Min(bordes.Width - margen, anchoTexto + LogicalToDeviceUnits(12)));
            var pill = new Rectangle(
                bordes.X + (bordes.Width - ancho) / 2,
                bordes.Y + (bordes.Height - alto) / 2,
                ancho,
                alto);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath forma = EstiloAgenda.RectanguloRedondo(pill, alto / 2))
            using (var pincelPill = new SolidBrush(fondoPill))
                g.FillPath(pincelPill, forma);

            TextRenderer.DrawText(
                g,
                texto,
                fuenteInsignia,
                pill,
                tintaPill,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);

            g.SmoothingMode = SmoothingMode.None;
            using (var separador = new Pen(EstiloAgenda.Borde))
                g.DrawLine(separador, bordes.Left, bordes.Bottom - 1, bordes.Right, bordes.Bottom - 1);

            if (seleccionado && tabla.Focused && tabla.CurrentCellAddress == new Point(e.ColumnIndex, e.RowIndex))
                ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(bordes, -2, -2), EstiloAgenda.Tinta, EstiloAgenda.Seleccion);

            g.Restore(estadoDibujo);
            e.Handled = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                destruido = true;
                solicitudPendiente = null;
                temporizadorBusqueda.Stop();
                temporizadorBusqueda.Dispose();
            }

            base.Dispose(disposing);

            if (!disposing) return;
            fuenteBase.Dispose();
            fuenteTitulo.Dispose();
            fuenteKpiValor.Dispose();
            fuenteKpiEtiqueta.Dispose();
            fuentePequena.Dispose();
            fuenteTabla.Dispose();
            fuenteEncabezadoTabla.Dispose();
            fuenteInsignia.Dispose();
            fuenteNegritaTabla.Dispose();
        }

        private sealed class SuperficieReporte : Panel
        {
            public SuperficieReporte()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
                BackColor = EstiloAgenda.Fondo;
            }

            protected override void OnPaintBackground(PaintEventArgs e) => FondoInterfaces.Pintar(this, e);
        }
    }
}
