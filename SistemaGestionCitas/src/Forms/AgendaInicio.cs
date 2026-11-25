using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;
using System.Diagnostics;
using System.Globalization;

namespace SistemaGestionCitas.src.Forms;

internal sealed class AgendaInicio : UserControl
{
    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-DO");
    private static readonly string[] Semana = ["Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom"];
    private readonly AgendaRepository repositorio = new();
    private readonly Button[] dias = new Button[42];
    private readonly Label[] valores = new Label[3];
    private readonly Control[] iconosIndicadores = new Control[3];
    private readonly Label lblMes = new();
    private readonly Label lblFecha = new();
    private readonly Label lblEstado = new();
    private readonly Button anterior;
    private readonly Button siguiente;
    private readonly Button reintentar = new();
    private readonly Button nuevaCita = ControlesRegistro.Boton("Nueva cita", ControlesRegistro.Primario, Color.White);
    private readonly DataGridView lista = new();
    private readonly TableLayoutPanel pagina = new();
    private readonly TableLayoutPanel zonaAgenda = new();
    private readonly TarjetaAgenda calendario = new();
    private readonly TarjetaAgenda citas = new();
    private readonly Font titulo = new("Segoe UI", 25F, FontStyle.Bold);
    private readonly Font encabezado = new("Segoe UI", 17F, FontStyle.Bold);
    private readonly Font enfasis = new("Segoe UI", 11F, FontStyle.Bold);
    private DateTime seleccion = DateTime.Today;
    private IReadOnlyDictionary<DateTime, long> diasConCitas = new Dictionary<DateTime, long>();
    private CancellationTokenSource? consulta;
    private bool consultando;
    private bool pendiente;
    private bool? apilada;
    private bool? permisoAnterior;
    private bool cerrando;

    internal DateTime FechaSeleccionada => seleccion;
    internal void ActualizarCitas() => SolicitarDatos();

    public AgendaInicio()
    {
        SuspendLayout();
        DoubleBuffered = true;
        AutoScroll = true;
        BackColor = EstiloAgenda.Fondo;
        anterior = CrearFlecha("Mes anterior", SimboloAgenda.Anterior, -1);
        siguiente = CrearFlecha("Mes siguiente", SimboloAgenda.Siguiente, 1);

        ResizeRedraw = true;
        pagina.BackColor = Color.Transparent;
        pagina.Dock = DockStyle.Top;
        pagina.Padding = new Padding(24, 22, 24, 20);
        pagina.ColumnCount = 1;
        pagina.RowCount = 4;
        pagina.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        pagina.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
        pagina.RowStyles.Add(new RowStyle(SizeType.Absolute, 29));
        pagina.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
        pagina.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var cabecera = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
        cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        cabecera.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        cabecera.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        cabecera.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Cabecera(cabecera, "Inicio", titulo, 0);
        Cabecera(cabecera, "Tu agenda de trabajo", null, 1, EstiloAgenda.Secundario);
        nuevaCita.Anchor = AnchorStyles.Right;
        nuevaCita.Margin = new Padding(10, 0, 0, 0);
        nuevaCita.Visible = false;
        nuevaCita.Click += (_, _) => AbrirNuevaCita();
        cabecera.Controls.Add(nuevaCita, 1, 0);
        cabecera.SetRowSpan(nuevaCita, 2);
        pagina.Controls.Add(cabecera, 0, 0);
        Cabecera(pagina, "Hoy", enfasis, 1);
        pagina.Controls.Add(CrearIndicadores(), 0, 2);

        CrearCalendario();
        CrearListado();
        zonaAgenda.Dock = DockStyle.Fill;
        zonaAgenda.Margin = new Padding(0, 10, 0, 0);
        pagina.Controls.Add(zonaAgenda, 0, 3);
        Controls.Add(pagina);
        ResumeLayout();
        AjustarDisposicion();
        DibujarCalendario();
    }

    private static void Cabecera(TableLayoutPanel tabla, string texto, Font? fuente, int fila, Color? color = null)
    {
        var rotulo = new Label { Text = texto, Dock = DockStyle.Fill, Margin = Padding.Empty,
            TextAlign = ContentAlignment.MiddleLeft, ForeColor = color ?? EstiloAgenda.Tinta };
        if (fuente != null) rotulo.Font = fuente;
        tabla.Controls.Add(rotulo, 0, fila);
    }

    private Control CrearIndicadores()
    {
        var indicadores = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = Padding.Empty, ColumnCount = 3, RowCount = 1 };
        indicadores.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        string[] nombres = ["Total de citas", "Pendientes", "Confirmadas"];
        SimboloAgenda[] simbolos = [SimboloAgenda.Calendario, SimboloAgenda.Reloj, SimboloAgenda.Confirmada];
        for (int i = 0; i < nombres.Length; i++)
        {
            indicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3));
            var tarjeta = new TarjetaAgenda { Dock = DockStyle.Fill, Padding = new Padding(10, 5, 10, 5),
                Margin = new Padding(0, 0, i < 2 ? 9 : 0, 0) };
            var columnas = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
            columnas.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            columnas.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var etiqueta = new Label { Text = nombres[i], Dock = DockStyle.Fill, Margin = Padding.Empty,
                TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true, ForeColor = EstiloAgenda.Tinta };
            valores[i] = new Label { Text = "—", Font = encabezado, AutoSize = true, Dock = DockStyle.Fill,
                Margin = new Padding(5, 0, 0, 0), TextAlign = ContentAlignment.MiddleRight, ForeColor = EstiloAgenda.Tinta };
            var simbolo = simbolos[i];
            var icono = new Panel { Dock = DockStyle.Left, Width = 32, TabStop = false };
            icono.Paint += (_, e) =>
            {
                int lado = icono.LogicalToDeviceUnits(22);
                IconosAgenda.Dibujar(e.Graphics, simbolo, EstiloAgenda.Tinta, new(0, (icono.Height - lado) / 2, lado, lado));
            };
            iconosIndicadores[i] = icono;
            columnas.Controls.Add(etiqueta, 0, 0);
            columnas.Controls.Add(valores[i], 1, 0);
            tarjeta.Controls.Add(columnas);
            tarjeta.Controls.Add(icono);
            indicadores.Controls.Add(tarjeta, i, 0);
        }
        return indicadores;
    }

    private void CrearCalendario()
    {
        calendario.Dock = DockStyle.Fill;
        calendario.Padding = new Padding(16);
        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var selector = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1,
            Margin = new Padding(0, 0, 0, 10) };
        selector.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44));
        lblMes.Font = encabezado;
        lblMes.ForeColor = EstiloAgenda.Tinta;
        lblMes.Dock = DockStyle.Fill;
        lblMes.Margin = Padding.Empty;
        lblMes.TextAlign = ContentAlignment.MiddleCenter;
        selector.Controls.Add(anterior, 0, 0);
        selector.Controls.Add(lblMes, 1, 0);
        selector.Controls.Add(siguiente, 2, 0);
        tabla.Controls.Add(selector, 0, 0);

        var cuadricula = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 7, RowCount = 7,
            Margin = Padding.Empty, Padding = new Padding(1, 1, 0, 0), BackColor = EstiloAgenda.Borde };
        cuadricula.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        for (int d = 0; d < 7; d++)
        {
            cuadricula.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 7));
            var diaSemana = new Label { Text = Semana[d], Margin = new Padding(0, 0, 1, 1), Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter, ForeColor = EstiloAgenda.Tinta, BackColor = EstiloAgenda.Fondo };
            cuadricula.Controls.Add(diaSemana, d, 0);
        }
        for (int fila = 0; fila < 6; fila++)
        {
            cuadricula.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 6));
            for (int columna = 0; columna < 7; columna++)
            {
                Button dia = BotonPlano();
                dia.TabStop = false;
                dia.Margin = new Padding(0, 0, 1, 1);
                dia.Click += (_, _) =>
                {
                    if (dia.Tag is DateTime fecha) Seleccionar(fecha);
                };
                dia.PreviewKeyDown += (_, e) =>
                {
                    if (e.KeyCode is Keys.Left or Keys.Right or Keys.Up or Keys.Down) e.IsInputKey = true;
                };
                dia.KeyDown += Dia_KeyDown;
                dia.Paint += (_, e) =>
                {
                    if (dia.Tag is not DateTime fecha || !diasConCitas.ContainsKey(fecha)) return;
                    int lado = dia.LogicalToDeviceUnits(5);
                    using SolidBrush marca = new(fecha == seleccion ? Color.FromArgb(174, 238, 218) : EstiloAgenda.Turquesa);
                    e.Graphics.FillEllipse(marca, (dia.Width - lado) / 2, dia.Height - lado - dia.LogicalToDeviceUnits(4), lado, lado);
                };
                dias[fila * 7 + columna] = dia;
                cuadricula.Controls.Add(dia, columna, fila + 1);
            }
        }
        tabla.Controls.Add(cuadricula, 0, 1);
        calendario.Controls.Add(tabla);
    }

    private void CrearListado()
    {
        citas.Dock = DockStyle.Fill;
        citas.Padding = new Padding(16);
        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Margin = Padding.Empty };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 37));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        tabla.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        lblFecha.Dock = DockStyle.Fill;
        lblFecha.Margin = Padding.Empty;
        lblFecha.Font = encabezado;
        lblFecha.ForeColor = EstiloAgenda.Tinta;
        lblFecha.TextAlign = ContentAlignment.MiddleLeft;
        tabla.Controls.Add(lblFecha, 0, 0);
        Cabecera(tabla, "Citas del día", null, 1, EstiloAgenda.Secundario);

        var cuerpo = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
        lista.Dock = DockStyle.Fill;
        lista.AccessibleName = "Citas de la fecha seleccionada";
        lista.ReadOnly = true;
        lista.AllowUserToAddRows = false;
        lista.AllowUserToDeleteRows = false;
        lista.AllowUserToResizeRows = false;
        lista.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        lista.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
        lista.BackgroundColor = Color.White;
        lista.BorderStyle = BorderStyle.None;
        lista.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        lista.GridColor = EstiloAgenda.Borde;
        lista.RowHeadersVisible = false;
        lista.MultiSelect = false;
        lista.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        lista.EnableHeadersVisualStyles = false;
        lista.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        lista.ColumnHeadersDefaultCellStyle.BackColor = EstiloAgenda.Fondo;
        lista.ColumnHeadersDefaultCellStyle.ForeColor = EstiloAgenda.Tinta;
        lista.DefaultCellStyle.ForeColor = EstiloAgenda.Tinta;
        lista.DefaultCellStyle.SelectionBackColor = EstiloAgenda.Seleccion;
        lista.DefaultCellStyle.SelectionForeColor = EstiloAgenda.Tinta;
        lista.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        lista.DefaultCellStyle.Padding = new Padding(3, 5, 3, 5);
        AgregarColumna("Hora", 22, 76);
        AgregarColumna("Cliente", 30, 80);
        AgregarColumna("Servicio", 24, 76);
        AgregarColumna("Estado", 24, 88);
        lista.Visible = false;
        cuerpo.Controls.Add(lista);

        lblEstado.Dock = DockStyle.Top;
        lblEstado.Margin = Padding.Empty;
        lblEstado.MinimumSize = new Size(0, 58);
        lblEstado.ForeColor = EstiloAgenda.Secundario;
        lblEstado.Text = "Cargando citas…";
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;
        lblEstado.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
        cuerpo.Controls.Add(lblEstado);
        tabla.Controls.Add(cuerpo, 0, 2);

        reintentar.Text = "Reintentar";
        reintentar.AutoSize = true;
        reintentar.MinimumSize = new Size(98, 36);
        reintentar.FlatStyle = FlatStyle.Flat;
        reintentar.FlatAppearance.BorderColor = EstiloAgenda.Borde;
        reintentar.ForeColor = EstiloAgenda.Azul;
        reintentar.Click += (_, _) => SolicitarDatos();
        reintentar.Visible = false;
        tabla.Controls.Add(reintentar, 0, 3);
        citas.Controls.Add(tabla);
    }

    private void AgregarColumna(string texto, int peso, int minimo)
    {
        lista.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = texto, Name = texto,
            FillWeight = peso, MinimumWidth = minimo, SortMode = DataGridViewColumnSortMode.NotSortable });
    }

    private Button CrearFlecha(string nombre, SimboloAgenda simbolo, int movimiento)
    {
        var boton = new BotonCitorna { Dock = DockStyle.Fill, Margin = new Padding(0, 2, 0, 2),
            ForeColor = EstiloAgenda.Tinta, Cursor = Cursors.Hand, Icono = simbolo, AccessibleName = nombre };
        boton.FlatAppearance.BorderSize = 1;
        boton.FlatAppearance.BorderColor = EstiloAgenda.Borde;
        boton.Click += (_, _) =>
        {
            DateTime mes = new(seleccion.Year, seleccion.Month, 1);
            if (movimiento < 0 && mes == DateTime.MinValue || movimiento > 0 && mes.Year == 9999 && mes.Month == 12)
                return;
            Seleccionar(mes.AddMonths(movimiento));
        };
        return boton;
    }

    private static Button BotonPlano()
    {
        Button boton = new() { Dock = DockStyle.Fill, Margin = Padding.Empty, FlatStyle = FlatStyle.Flat,
            ForeColor = EstiloAgenda.Tinta, BackColor = Color.White, Cursor = Cursors.Hand, UseVisualStyleBackColor = false };
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = EstiloAgenda.Seleccion;
        return boton;
    }

    protected override void OnPaintBackground(PaintEventArgs e) => FondoInterfaces.Pintar(this, e);

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (pagina?.Parent != null) AjustarDisposicion();
    }

    private void AjustarDisposicion()
    {
        bool apilar = ClientSize.Width < LogicalToDeviceUnits(900);
        pagina.Height = Math.Max(ClientSize.Height, LogicalToDeviceUnits(apilar ? 970 : 600));
        if (apilada == apilar) return;
        apilada = apilar;
        zonaAgenda.SuspendLayout();
        zonaAgenda.Controls.Clear();
        zonaAgenda.ColumnStyles.Clear();
        zonaAgenda.RowStyles.Clear();
        zonaAgenda.ColumnCount = apilar ? 1 : 2;
        zonaAgenda.RowCount = apilar ? 2 : 1;
        zonaAgenda.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, apilar ? 100 : 58));
        zonaAgenda.RowStyles.Add(new RowStyle(SizeType.Percent, apilar ? 58 : 100));
        if (apilar) zonaAgenda.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
        else zonaAgenda.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        int espacio = LogicalToDeviceUnits(14);
        calendario.Margin = apilar ? new Padding(0, 0, 0, espacio) : new Padding(0, 0, espacio, 0);
        citas.Margin = Padding.Empty;
        zonaAgenda.Controls.Add(calendario, 0, 0);
        zonaAgenda.Controls.Add(citas, apilar ? 0 : 1, apilar ? 1 : 0);
        zonaAgenda.ResumeLayout();
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        bool mostrarIconos = ClientSize.Width >= LogicalToDeviceUnits(650);
        if (iconosIndicadores == null) return;
        foreach (var icono in iconosIndicadores)
        {
            if (icono != null) icono.Visible = mostrarIconos;
        }
    }

    private void DibujarCalendario()
    {
        DateTime mes = new(seleccion.Year, seleccion.Month, 1);
        DateTime hoy = DateTime.Today;
        lblMes.Text = Espanol.TextInfo.ToTitleCase(mes.ToString("MMMM yyyy", Espanol));
        lblFecha.Text = seleccion.ToString("d 'de' MMMM 'de' yyyy", Espanol);
        anterior.Enabled = mes > DateTime.MinValue;
        siguiente.Enabled = mes.Year != 9999 || mes.Month != 12;
        int comienzo = ((int)mes.DayOfWeek + 6) % 7;
        int longitud = DateTime.DaysInMonth(mes.Year, mes.Month);

        foreach (var (boton, indice) in dias.Select((dia, i) => (dia, i)))
        {
            int numero = indice - comienzo + 1;
            if (numero < 1 || numero > longitud)
            {
                boton.Tag = null;
                boton.Text = "";
                boton.AccessibleName = "";
                boton.TabStop = false;
                boton.Enabled = false;
                boton.BackColor = Color.FromArgb(248, 251, 255);
                boton.FlatAppearance.BorderSize = 0;
                continue;
            }

            DateTime fecha = mes.AddDays(numero - 1);
            bool elegida = fecha == seleccion;
            bool esHoy = fecha == hoy;
            boton.Enabled = true;
            boton.Text = numero.ToString(Espanol);
            boton.Tag = fecha;
            boton.TabStop = elegida;
            boton.AccessibleName = fecha.ToString("dddd, d 'de' MMMM 'de' yyyy", Espanol)
                + (esHoy ? ", hoy" : "") + (elegida ? ", seleccionado" : "")
                + (diasConCitas.TryGetValue(fecha, out long cantidad) ? $", {cantidad} citas" : "");
            boton.BackColor = elegida ? EstiloAgenda.Azul : Color.White;
            boton.ForeColor = elegida ? Color.White : EstiloAgenda.Tinta;
            boton.FlatAppearance.BorderSize = esHoy && !elegida ? 2 : 0;
            boton.FlatAppearance.BorderColor = EstiloAgenda.Azul;
            boton.FlatAppearance.MouseOverBackColor = elegida ? EstiloAgenda.Azul : EstiloAgenda.Seleccion;
            boton.FlatAppearance.MouseDownBackColor = boton.FlatAppearance.MouseOverBackColor;
            boton.Invalidate();
        }
    }

    private void Seleccionar(DateTime fecha)
    {
        if (fecha == seleccion) return;
        if (fecha.Year != seleccion.Year || fecha.Month != seleccion.Month)
            diasConCitas = new Dictionary<DateTime, long>();
        seleccion = fecha;
        DibujarCalendario();
        SolicitarDatos();
    }

    private void Dia_KeyDown(object? sender, KeyEventArgs e)
    {
        int salto = e.KeyCode switch
        {
            Keys.Left => -1, Keys.Right => 1, Keys.Up => -7, Keys.Down => 7, _ => 0
        };
        if (salto == 0) return;
        e.SuppressKeyPress = true;
        if (salto < 0 && seleccion < DateTime.MinValue.AddDays(-salto)
            || salto > 0 && seleccion > DateTime.MaxValue.Date.AddDays(-salto))
            return;
        Seleccionar(seleccion.AddDays(salto));
        dias.First(d => d.Tag is DateTime fecha && fecha == seleccion).Focus();
    }

    public void ActualizarPermiso()
    {
        nuevaCita.Visible = SesionActual.HaySesion && SesionActual.TienePermiso(PermisosSistema.Agregar);
        bool consultar = PuedeConsultar();
        if (permisoAnterior == consultar) return;
        permisoAnterior = consultar;
        SolicitarDatos();
    }

    private void AbrirNuevaCita()
    {
        if (!SesionActual.HaySesion || !SesionActual.VerificarPermiso(PermisosSistema.Agregar, "registrar citas"))
            return;

        using var formulario = new FrmNuevaCita(seleccion);
        if (formulario.ShowDialog(FindForm()) != DialogResult.OK || formulario.FechaGuardada is not DateTime fecha)
            return;

        if (seleccion == fecha) SolicitarDatos();
        else Seleccionar(fecha);
        MessageBox.Show(FindForm(), "Cita registrada correctamente.", "Nueva cita",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static bool PuedeConsultar() => SesionActual.HaySesion && SesionActual.TienePermiso(PermisosSistema.Consultar);

    private async void SolicitarDatos()
    {
        if (cerrando) return;
        bool permitido = PuedeConsultar();
        if (!permitido) LimpiarDatos();
        MostrarEstado(permitido ? "Cargando citas…" : "No tiene permiso para consultar las citas.");
        consulta?.Cancel();
        if (consultando)
        {
            pendiente = true;
            return;
        }

        consultando = true;
        try
        {
            do
            {
                pendiente = false;
                if (!PuedeConsultar())
                {
                    LimpiarDatos();
                    return;
                }

                DateTime fecha = seleccion;
                using CancellationTokenSource operacion = new();
                consulta = operacion;
                try
                {
                    await Task.Delay(110, operacion.Token);
                    ResumenAgenda resultado = await repositorio.ConsultarAsync(fecha, DateTime.Today, operacion.Token);
                    if (!cerrando && !operacion.IsCancellationRequested && PuedeConsultar())
                        MostrarDatos(resultado);
                }
                catch (OperationCanceledException) when (operacion.IsCancellationRequested) { }
                catch (UnauthorizedAccessException)
                {
                    if (!cerrando && !operacion.IsCancellationRequested)
                    {
                        LimpiarDatos();
                        MostrarEstado("No tiene permiso para consultar las citas.");
                    }
                }
                catch (Exception ex) when (ex is SqlException or InvalidOperationException)
                {
                    Trace.TraceWarning("No fue posible leer la agenda de Citorna: {0}", ex.Message);
                    if (!cerrando && !operacion.IsCancellationRequested)
                    {
                        LimpiarDatos();
                        MostrarEstado("No se pudieron cargar las citas. Compruebe la conexión y vuelva a intentarlo.", true);
                    }
                }
                finally
                {
                    consulta = null;
                }
            } while (pendiente && !cerrando);
        }
        finally
        {
            consultando = false;
        }
    }

    private void MostrarDatos(ResumenAgenda resultado)
    {
        diasConCitas = resultado.DiasConCitas;
        DibujarCalendario();
        valores[0].Text = resultado.TotalHoy.ToString("N0", Espanol);
        valores[1].Text = resultado.PendientesHoy.ToString("N0", Espanol);
        valores[2].Text = resultado.ConfirmadasHoy.ToString("N0", Espanol);
        lista.Rows.Clear();
        foreach (CitaDelDia cita in resultado.CitasDelDia)
        {
            string hora = DateTime.MinValue.Add(cita.Hora).ToString("h:mm tt", CultureInfo.InvariantCulture);
            int fila = lista.Rows.Add(hora, cita.Cliente, cita.Servicio, cita.Estado);
            if (cita.Estado == "Confirmada")
                ColorearEstado(lista.Rows[fila].Cells[3], Color.FromArgb(221, 241, 232), Color.FromArgb(0, 78, 60));
            else if (cita.Estado == "Pendiente")
                ColorearEstado(lista.Rows[fila].Cells[3], Color.FromArgb(251, 239, 213), Color.FromArgb(104, 66, 14));
        }

        if (lista.Rows.Count == 0)
            MostrarEstado("No hay citas registradas para esta fecha.");
        else
        {
            lblEstado.Visible = false;
            lista.Visible = true;
            lista.ClearSelection();
        }
    }

    private static void ColorearEstado(DataGridViewCell celda, Color fondo, Color texto)
    {
        celda.Style.BackColor = fondo;
        celda.Style.ForeColor = texto;
    }

    private void LimpiarDatos()
    {
        foreach (Label valor in valores) valor.Text = "—";
        diasConCitas = new Dictionary<DateTime, long>();
        lista.Rows.Clear();
        DibujarCalendario();
    }

    private void MostrarEstado(string mensaje, bool permitirReintentar = false)
    {
        lista.Visible = false;
        lblEstado.Text = mensaje;
        lblEstado.Visible = true;
        reintentar.Visible = permitirReintentar;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            cerrando = true;
            consulta?.Cancel();
        }
        base.Dispose(disposing);
        if (disposing)
        {
            titulo.Dispose();
            encabezado.Dispose();
            enfasis.Dispose();
        }
    }
}
