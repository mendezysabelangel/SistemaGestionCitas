using System.Diagnostics;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms;

internal static class ControlesRegistro
{
    internal static readonly Color Primario = Color.FromArgb(0, 112, 119);
    internal static readonly Color Peligro = Color.FromArgb(158, 35, 49);

    internal static Button Boton(string texto, Color? fondo = null, Color? tinta = null)
    {
        var boton = new BotonCitorna { Text = texto, AutoSize = true, MinimumSize = new Size(0, 40),
            FlatStyle = FlatStyle.Flat, Padding = new Padding(10, 0, 10, 0),
            Relleno = fondo ?? Color.White, ForeColor = tinta ?? EstiloAgenda.Tinta,
            UseVisualStyleBackColor = false, Cursor = Cursors.Hand };
        boton.FlatAppearance.BorderColor = tinta == Peligro ? Peligro : EstiloAgenda.Borde;
        if (fondo.HasValue) boton.FlatAppearance.BorderSize = 0;
        return boton;
    }

    internal static TextBox Texto(int maximo, string valor = "") => new() { MaxLength = maximo, Text = valor };

    internal static ComboBox Opciones(params object[] opciones)
    {
        var control = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        control.Items.AddRange(opciones);
        if (opciones.Length > 0) control.SelectedIndex = 0;
        return control;
    }

    internal static bool? Estado(int indice) => indice switch { 1 => true, 2 => false, _ => null };

    internal static string Disponible(string? valor) => string.IsNullOrWhiteSpace(valor) ? "No registrado" : valor;
}

internal sealed class RejillaRegistros : DataGridView
{
    public RejillaRegistros()
    {
        DoubleBuffered = true;
        ReadOnly = true;
        VirtualMode = true;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        MultiSelect = false;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        RowHeadersVisible = false;
        BorderStyle = BorderStyle.None;
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        GridColor = EstiloAgenda.Borde;
        BackgroundColor = Color.White;
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        EnableHeadersVisualStyles = false;
        ColumnHeadersDefaultCellStyle.BackColor = EstiloAgenda.Fondo;
        ColumnHeadersDefaultCellStyle.ForeColor = EstiloAgenda.Tinta;
        ColumnHeadersDefaultCellStyle.SelectionBackColor = EstiloAgenda.Fondo;
        ColumnHeadersDefaultCellStyle.SelectionForeColor = EstiloAgenda.Tinta;
        DefaultCellStyle.BackColor = Color.White;
        DefaultCellStyle.ForeColor = EstiloAgenda.Tinta;
        DefaultCellStyle.SelectionBackColor = EstiloAgenda.Seleccion;
        DefaultCellStyle.SelectionForeColor = EstiloAgenda.Tinta;
    }

    internal void AgregarColumna(string nombre, int peso, int minimo) => Columns.Add(new DataGridViewTextBoxColumn
    {
        Name = nombre, HeaderText = nombre, FillWeight = peso, MinimumWidth = minimo,
        SortMode = DataGridViewColumnSortMode.NotSortable
    });
}

internal sealed class VistaRegistros<T> : UserControl, IVistaConPaleta where T : class
{
    private readonly TableLayoutPanel pagina = new();
    private readonly TableLayoutPanel cuerpo = new();
    private readonly TarjetaAgenda listado = new();
    private readonly TarjetaAgenda detalle = new();
    private readonly FlowLayoutPanel filtros = new();
    private readonly TableLayoutPanel datosFicha = new();
    private readonly Panel scrollFicha = new();
    private readonly FlowLayoutPanel accionesFicha = new();
    private readonly Label nombreFicha = new();
    private readonly Label estado = new();
    private readonly Button accionEstado = ControlesRegistro.Boton("Reintentar");
    private readonly Button editar = ControlesRegistro.Boton("Editar");
    private readonly Button eliminar = ControlesRegistro.Boton("Eliminar", tinta: ControlesRegistro.Peligro);
    private readonly Button nuevo;
    private readonly Font fuenteTitulo = new("Segoe UI", 23, FontStyle.Bold);
    private readonly Font fuenteFicha = new("Segoe UI", 15, FontStyle.Bold);
    private readonly Font fuenteEtiqueta = new("Segoe UI", 10.5F, FontStyle.Bold);
    private readonly Font fuenteBase = new("Segoe UI", 11F);
    private readonly Func<T, int> idDe;
    private readonly string plural;
    private readonly string singular;
    private readonly string permisoConsulta;
    private readonly string permisoNuevo;
    private readonly bool femenino;
    private Button? accionFichaAdicional;
    private Button? desbloquear;
    private Func<T, bool>? puedeDesbloquear;
    private Func<T, bool>? puedeEliminar;
    private Func<T, bool>? puedeEditar;
    private List<T> registros = new();
    private Action? accionEstadoActual;
    private bool aplicando;
    private bool ocupado;
    private bool destruido;
    private bool consultando;
    private bool datosVigentes;
    private bool? compacto;
    private PaletaCitorna paleta = PaletaCitorna.Clara;
    internal PaletaCitorna Paleta => paleta;
    private Solicitud? pendiente;

    private sealed record Solicitud(Func<Task<(List<T> Datos, Action? AlAplicar)>> Buscar, bool Filtrada, int? Id, bool ConservarSeleccion);
    internal RejillaRegistros Tabla { get; } = new() { Dock = DockStyle.Fill };
    internal TextBox Busqueda { get; } = new();
    internal Button? AccionExtra { get; }
    internal event Action? Consultar;
    internal event Action? Restablecer;
    internal Action? ReintentarConsulta { get; set; }
    internal event Action? Nuevo;
    internal event Action<T>? Editar;
    internal event Action<T>? Eliminar;
    internal Func<T, int, string?>? ValorCelda { get; set; }
    internal Action<T?>? MostrarSeleccion { get; set; }

    internal VistaRegistros(string titulo, string subtitulo, string buscar, string singular, Func<T, int> idDe,
        string? accionExtra = null, string permisoConsulta = PermisosSistema.Consultar,
        string permisoNuevo = PermisosSistema.Agregar, string? etiquetaConsulta = null,
        Control? campoConsulta = null, bool femenino = false, Control? accionCabecera = null)
    {
        this.idDe = idDe;
        this.singular = singular;
        this.permisoConsulta = permisoConsulta;
        this.permisoNuevo = permisoNuevo;
        this.femenino = femenino;
        plural = titulo.ToLowerInvariant();
        SuspendLayout();
        Size = new Size(1040, 650);
        AutoScroll = true;
        Dock = DockStyle.Fill;
        DoubleBuffered = true;
        ResizeRedraw = true;
        Font = fuenteBase;
        BackColor = EstiloAgenda.Fondo;
        nuevo = ControlesRegistro.Boton($"{(femenino ? "Nueva" : "Nuevo")} {singular}", ControlesRegistro.Primario, Color.White);
        if (accionCabecera != null) Disposed += (_, _) => nuevo.Dispose();
        AccionExtra = accionExtra == null ? null : ControlesRegistro.Boton(accionExtra);
        nuevo.Click += (_, _) => Nuevo?.Invoke();
        editar.Click += (_, _) => { if (Seleccion is { } fila) Editar?.Invoke(fila); };
        eliminar.Click += (_, _) => { if (Seleccion is { } fila) Eliminar?.Invoke(fila); };
        accionEstado.Click += (_, _) => accionEstadoActual?.Invoke();

        pagina.Dock = DockStyle.Top;
        pagina.BackColor = Color.Transparent;
        pagina.Margin = Padding.Empty;
        pagina.Padding = new Padding(23, 20, 23, 18);
        pagina.ColumnCount = 1;
        pagina.RowCount = 4;
        pagina.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        pagina.RowStyles.Add(new RowStyle(SizeType.Absolute, 79));
        pagina.RowStyles.Add(new RowStyle(SizeType.Absolute, 49));
        pagina.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pagina.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        pagina.Controls.Add(ConstruirCabecera(titulo, subtitulo, accionCabecera ?? nuevo), 0, 0);
        pagina.Controls.Add(campoConsulta == null ? ConstruirBusqueda(buscar, etiquetaConsulta ?? "Consultar")
            : ConstruirConsultaPorCampo(buscar, campoConsulta), 0, 1);

        filtros.Dock = DockStyle.Fill;
        filtros.AutoSize = true;
        filtros.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        filtros.Margin = Padding.Empty;
        filtros.Padding = new Padding(2, 2, 0, 4);
        filtros.Visible = false;
        pagina.Controls.Add(filtros, 0, 2);

        cuerpo.Dock = DockStyle.Fill;
        cuerpo.Margin = new Padding(0, 12, 0, 0);
        pagina.Controls.Add(cuerpo, 0, 3);
        ConstruirListado();
        ConstruirFicha();
        Controls.Add(pagina);
        PresentarFicha($"Selecciona {(femenino ? "una" : "un")} {singular} para ver sus datos");
        MostrarEstado($"Cargando {plural}…");
        Ajustar();
        ResumeLayout(true);
    }

    private Control ConstruirCabecera(string titulo, string subtitulo, Control accion)
    {
        var rejilla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
        rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        rejilla.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        rejilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        rejilla.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var principal = Rotulo(titulo, fuenteTitulo);
        principal.Dock = DockStyle.Fill;
        var secundario = Rotulo(subtitulo);
        secundario.ForeColor = EstiloAgenda.Secundario;
        secundario.Dock = DockStyle.Fill;
        rejilla.Controls.Add(principal, 0, 0);
        rejilla.Controls.Add(secundario, 0, 1);
        rejilla.Controls.Add(accion, 1, 0);
        rejilla.SetRowSpan(accion, 2);
        if (accion == nuevo) nuevo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        return rejilla;
    }

    private Control ConstruirBusqueda(string buscar, string etiquetaConsulta)
    {
        var barra = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Margin = Padding.Empty };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        barra.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var caja = new TarjetaAgenda { Dock = DockStyle.Fill, Radio = 11, ResaltarFoco = true,
            Margin = new Padding(0, 3, 6, 3), Padding = new Padding(37, 0, 10, 0) };
        Busqueda.PlaceholderText = buscar;
        Busqueda.AccessibleName = buscar;
        Busqueda.MaxLength = 200;
        Busqueda.BorderStyle = BorderStyle.None;
        caja.Controls.Add(Busqueda);
        caja.Paint += (_, e) =>
        {
            int lado = caja.LogicalToDeviceUnits(13);
            int x = caja.LogicalToDeviceUnits(12);
            int y = (caja.Height - lado) / 2 - 1;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using Pen trazo = new(paleta.Secundario, caja.LogicalToDeviceUnits(2));
            e.Graphics.DrawEllipse(trazo, x, y, lado, lado);
            e.Graphics.DrawLine(trazo, x + lado - 2, y + lado - 2, x + lado + 3, y + lado + 3);
        };
        caja.Click += (_, _) => Busqueda.Focus();
        caja.Enter += (_, _) => caja.Invalidate();
        caja.Leave += (_, _) => caja.Invalidate();
        caja.SizeChanged += (_, _) =>
        {
            Busqueda.SetBounds(caja.Padding.Left, Math.Max(0, (caja.ClientSize.Height - Busqueda.PreferredHeight) / 2),
                Math.Max(1, caja.ClientSize.Width - caja.Padding.Horizontal), Busqueda.PreferredHeight);
        };
        Busqueda.KeyDown += BuscarConEnter;
        barra.Controls.Add(caja, 0, 0);

        Button filtro = ControlesRegistro.Boton("Filtros");
        filtro.Anchor = AnchorStyles.Left;
        filtro.Click += (_, _) =>
        {
            filtros.Visible = !filtros.Visible;
            filtro.AccessibleDescription = filtros.Visible ? "Filtros desplegados" : "Filtros contraídos";
        };
        barra.Controls.Add(filtro, 1, 0);
        Button consultar = ControlesRegistro.Boton(etiquetaConsulta);
        consultar.Anchor = AnchorStyles.Left;
        consultar.Click += (_, _) => Consultar?.Invoke();
        barra.Controls.Add(consultar, 2, 0);
        return barra;
    }

    private Control ConstruirConsultaPorCampo(string nombre, Control campo)
    {
        var barra = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = Padding.Empty, ColumnCount = 4, RowCount = 1 };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 168));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Label etiqueta = Rotulo(nombre);
        etiqueta.Anchor = AnchorStyles.Left;
        etiqueta.Margin = new Padding(0, 0, 8, 0);

        var entrada = new TarjetaAgenda { Anchor = AnchorStyles.Left | AnchorStyles.Right, Height = 40, Radio = 9,
            ResaltarFoco = true, Padding = new Padding(9, 0, 9, 0), Margin = new Padding(0, 0, 8, 0), TabStop = false };
        var interior = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 1,
            BackColor = Color.Transparent, Margin = Padding.Empty, TabStop = false };
        interior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        interior.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        campo.AccessibleName = nombre;
        campo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        campo.Margin = Padding.Empty;
        interior.Controls.Add(campo, 0, 0);
        entrada.Controls.Add(interior);
        entrada.Enter += (_, _) => entrada.Invalidate();
        entrada.Leave += (_, _) => entrada.Invalidate();
        campo.KeyDown += BuscarConEnter;

        Button consultar = ControlesRegistro.Boton("Consultar");
        consultar.Anchor = AnchorStyles.Left;
        consultar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        consultar.Margin = Padding.Empty;
        consultar.Click += (_, _) => Consultar?.Invoke();
        barra.Controls.Add(etiqueta, 0, 0);
        barra.Controls.Add(entrada, 1, 0);
        barra.Controls.Add(consultar, 2, 0);
        return barra;
    }

    internal void AgregarFiltros(params (string Nombre, Control Campo)[] campos)
    {
        foreach (var (nombre, campo) in campos)
        {
            var columna = new TableLayoutPanel { Width = 144, Height = 57, ColumnCount = 1, RowCount = 2,
                Margin = new Padding(0, 0, 9, 0) };
            columna.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            columna.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            columna.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var etiqueta = Rotulo(nombre);
            columna.Controls.Add(etiqueta, 0, 0);
            if (campo is TextBox) campo.KeyDown += BuscarConEnter;
            var borde = CampoCitorna.Enmarcar(campo, nombre);
            columna.Controls.Add(borde, 0, 1);
            filtros.Controls.Add(columna);
        }
        Button restablecer = ControlesRegistro.Boton("Restablecer");
        restablecer.Margin = new Padding(0, 12, 0, 0);
        restablecer.Click += (_, _) => Restablecer?.Invoke();
        filtros.Controls.Add(restablecer);
    }

    private void BuscarConEnter(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        Consultar?.Invoke();
    }

    private void ConstruirListado()
    {
        listado.Dock = DockStyle.Fill;
        listado.Padding = new Padding(13);
        var contenido = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = Padding.Empty };
        contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Tabla.AccessibleName = $"Listado de {plural}";
        Tabla.Margin = Padding.Empty;
        Tabla.CellValueNeeded += (_, e) =>
        {
            if (e.RowIndex >= 0 && e.RowIndex < registros.Count)
                e.Value = ValorCelda?.Invoke(registros[e.RowIndex], e.ColumnIndex);
        };
        Tabla.CellToolTipTextNeeded += (_, e) =>
        {
            if (e.RowIndex >= 0 && e.RowIndex < registros.Count && e.ColumnIndex >= 0)
                e.ToolTipText = ValorCelda?.Invoke(registros[e.RowIndex], e.ColumnIndex) ?? "";
        };
        Tabla.SelectionChanged += (_, _) => { if (!aplicando) ActualizarSeleccion(); };
        contenido.Controls.Add(Tabla, 0, 0);

        var pie = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2, RowCount = 2,
            Margin = new Padding(0, 5, 0, 0) };
        pie.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        pie.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        pie.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        pie.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        estado.AutoSize = true;
        estado.Dock = DockStyle.Fill;
        estado.Margin = new Padding(0, 6, 6, 5);
        estado.ForeColor = EstiloAgenda.Secundario;
        estado.LiveSetting = System.Windows.Forms.Automation.AutomationLiveSetting.Polite;
        pie.Controls.Add(estado, 0, 0);
        pie.Controls.Add(accionEstado, 1, 0);
        if (AccionExtra != null)
        {
            AccionExtra.Anchor = AnchorStyles.Left;
            AccionExtra.Margin = new Padding(0, 3, 0, 0);
            pie.Controls.Add(AccionExtra, 0, 1);
            pie.SetColumnSpan(AccionExtra, 2);
        }
        contenido.Controls.Add(pie, 0, 1);
        listado.Controls.Add(contenido);
    }

    private void ConstruirFicha()
    {
        detalle.Dock = DockStyle.Fill;
        detalle.Padding = new Padding(16);
        var contenido = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = Padding.Empty };
        contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        contenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenido.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        nombreFicha.Font = fuenteFicha;
        nombreFicha.AutoSize = true;
        nombreFicha.Margin = new Padding(0, 0, 0, 9);
        nombreFicha.Dock = DockStyle.Fill;
        nombreFicha.ForeColor = EstiloAgenda.Tinta;
        contenido.Controls.Add(nombreFicha, 0, 0);

        scrollFicha.Dock = DockStyle.Fill;
        scrollFicha.AutoScroll = true;
        scrollFicha.Margin = Padding.Empty;
        datosFicha.Dock = DockStyle.Top;
        datosFicha.AutoSize = true;
        datosFicha.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        datosFicha.ColumnCount = 1;
        datosFicha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        scrollFicha.Controls.Add(datosFicha);
        contenido.Controls.Add(scrollFicha, 0, 1);

        accionesFicha.Dock = DockStyle.Fill;
        accionesFicha.WrapContents = false;
        accionesFicha.Margin = new Padding(0, 8, 0, 0);
        accionesFicha.Controls.Add(editar);
        accionesFicha.Controls.Add(eliminar);
        contenido.Controls.Add(accionesFicha, 0, 2);
        detalle.Controls.Add(contenido);
    }

    private Label Rotulo(string texto, Font? fuente = null)
    {
        var etiqueta = new Label { Text = texto, ForeColor = paleta.Tinta, AutoSize = true, Margin = Padding.Empty };
        if (fuente != null) etiqueta.Font = fuente;
        return etiqueta;
    }

    internal void PresentarFicha(string titulo, params (string Nombre, string Valor)[] valores)
    {
        nombreFicha.Text = titulo;
        datosFicha.SuspendLayout();
        foreach (Control control in datosFicha.Controls.Cast<Control>().ToArray()) control.Dispose();
        datosFicha.RowStyles.Clear();
        datosFicha.RowCount = 0;
        foreach (var (nombre, valor) in valores)
        {
            Label etiqueta = Rotulo(nombre, fuenteEtiqueta);
            etiqueta.Margin = new Padding(0, 7, 0, 1);
            Label dato = Rotulo(valor);
            dato.ForeColor = paleta.Secundario;
            dato.Margin = new Padding(0, 0, 0, 6);
            AgregarDato(etiqueta);
            AgregarDato(dato);
        }
        datosFicha.ResumeLayout(true);
        scrollFicha.AutoScrollPosition = Point.Empty;
    }

    private void AgregarDato(Label etiqueta)
    {
        int fila = datosFicha.RowCount++;
        datosFicha.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        etiqueta.Dock = DockStyle.Fill;
        datosFicha.Controls.Add(etiqueta, 0, fila);
    }

    private T? Seleccion => Tabla.SelectedRows.Count != 0 ? Registro(Tabla.SelectedRows[0].Index) : null;

    internal T? Registro(int indice) => indice >= 0 && indice < registros.Count ? registros[indice] : null;

    internal void ConfigurarAccionFicha(string texto, Func<T, bool> permitirEdicion, Action<Button, T> ejecutar)
    {
        puedeEditar = permitirEdicion;
        var boton = ControlesRegistro.Boton(texto);
        boton.Click += (_, _) =>
        {
            if (Seleccion is { } elegido) ejecutar(boton, elegido);
        };
        accionFichaAdicional = boton;
        accionesFicha.Controls.Add(boton);
        accionesFicha.WrapContents = true;
        ((TableLayoutPanel)accionesFicha.Parent!).RowStyles[2].Height = 96;
    }

    internal void ConfigurarAccionesCuenta(Func<T, bool> permitirBorrado, Action<T> cambiarPassword,
        Func<T, bool> tieneBloqueo, Action<T> desbloquearCuenta)
    {
        puedeEliminar = permitirBorrado;
        accionFichaAdicional = ControlesRegistro.Boton("Cambiar contraseña");
        accionFichaAdicional.Click += (_, _) =>
        {
            if (Seleccion is { } elegido) cambiarPassword(elegido);
        };
        accionesFicha.WrapContents = true;
        accionesFicha.Controls.Add(accionFichaAdicional);
        accionesFicha.Controls.SetChildIndex(accionFichaAdicional, 1);
        puedeDesbloquear = tieneBloqueo;
        desbloquear = ControlesRegistro.Boton("Desbloquear");
        desbloquear.Click += (_, _) =>
        {
            if (Seleccion is { } elegido) desbloquearCuenta(elegido);
        };
        accionesFicha.Controls.Add(desbloquear);
        accionesFicha.Controls.SetChildIndex(desbloquear, 2);
        ((TableLayoutPanel)accionesFicha.Parent!).RowStyles[2].Height = 112;
        ActualizarAcciones();
    }

    internal void ActualizarPermisos()
    {
        bool consultar = SesionActual.TienePermiso(permisoConsulta);
        nuevo.Visible = Nuevo != null && consultar && SesionActual.TienePermiso(permisoNuevo);
        if (AccionExtra != null) AccionExtra.Visible = consultar;
        ActualizarAcciones();
    }

    private void ActualizarSeleccion()
    {
        MostrarSeleccion?.Invoke(Seleccion);
        ActualizarAcciones();
    }

    private void ActualizarAcciones()
    {
        nuevo.Enabled = !ocupado;
        T? elegido = Seleccion;
        bool seleccion = elegido != null && SesionActual.TienePermiso(permisoConsulta);
        bool modificar = seleccion && SesionActual.TienePermiso(PermisosSistema.Modificar)
            && (puedeEditar?.Invoke(elegido!) ?? true);
        bool disponible = !ocupado && !consultando && datosVigentes;
        editar.Visible = modificar;
        eliminar.Visible = Eliminar != null && seleccion && SesionActual.TienePermiso(PermisosSistema.Eliminar);
        editar.Enabled = disponible;
        eliminar.Enabled = disponible && elegido != null && (puedeEliminar?.Invoke(elegido) ?? true);
        if (accionFichaAdicional != null)
        {
            accionFichaAdicional.Visible = modificar;
            accionFichaAdicional.Enabled = disponible;
        }
        if (desbloquear != null)
        {
            desbloquear.Visible = modificar && elegido != null && puedeDesbloquear?.Invoke(elegido) == true;
            desbloquear.Enabled = disponible;
        }
    }

    internal void AplicarPaleta(PaletaCitorna nueva)
    {
        if (paleta == nueva) return;
        paleta = nueva;
        BackColor = nueva.Fondo;
        nueva.AplicarControles(this);
        Invalidate(true);
    }

    void IVistaConPaleta.AplicarPaleta(PaletaCitorna nueva) => AplicarPaleta(nueva);

    protected override void OnPaintBackground(PaintEventArgs e) => FondoInterfaces.Pintar(this, e, paleta);

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (pagina?.Parent != null) Ajustar();
    }

    private void Ajustar()
    {
        bool estrecho = ClientSize.Width < LogicalToDeviceUnits(900);
        pagina.Height = Math.Max(ClientSize.Height, LogicalToDeviceUnits(estrecho ? 1050 : 520));
        if (compacto == estrecho) return;
        compacto = estrecho;
        cuerpo.SuspendLayout();
        cuerpo.Controls.Clear();
        cuerpo.ColumnStyles.Clear();
        cuerpo.RowStyles.Clear();
        cuerpo.ColumnCount = estrecho ? 1 : 2;
        cuerpo.RowCount = estrecho ? 2 : 1;
        cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, estrecho ? 100 : 60));
        cuerpo.RowStyles.Add(new RowStyle(SizeType.Percent, estrecho ? 51 : 100));
        if (estrecho) cuerpo.RowStyles.Add(new RowStyle(SizeType.Percent, 49));
        else cuerpo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

        int espacio = LogicalToDeviceUnits(12);
        listado.Margin = estrecho ? new Padding(0, 0, 0, espacio) : new Padding(0, 0, espacio, 0);
        detalle.Margin = Padding.Empty;
        cuerpo.Controls.Add(listado, 0, 0);
        cuerpo.Controls.Add(detalle, estrecho ? 0 : 1, estrecho ? 1 : 0);
        cuerpo.ResumeLayout(true);
    }

    internal bool LeerId(TextBox caja, out int? id)
    {
        id = null;
        if (string.IsNullOrWhiteSpace(caja.Text)) return true;
        if (int.TryParse(caja.Text.Trim(), out int numero) && numero > 0)
        {
            id = numero;
            return true;
        }
        MostrarEstado("El ID debe ser un número entero positivo.");
        caja.Focus();
        return false;
    }

    internal void Cargar(Func<(List<T> Datos, Action? AlAplicar)> buscar, bool filtrada, int? id = null, bool sinSeleccion = false) =>
        CargarAsync(() => Task.Run(buscar), filtrada, id, sinSeleccion);

    internal async void CargarAsync(Func<Task<(List<T> Datos, Action? AlAplicar)>> buscar, bool filtrada,
        int? id = null, bool sinSeleccion = false)
    {
        if (destruido || !SesionActual.VerificarPermiso(permisoConsulta, $"consultar {plural}")) return;
        pendiente = new Solicitud(buscar, filtrada, sinSeleccion ? null : id, !sinSeleccion && id == null);
        datosVigentes = false;
        MostrarEstado(registros.Count > 0 ? "Actualizando resultados…" : $"Cargando {plural}…");
        if (consultando) return;
        consultando = true;
        ActualizarAcciones();
        try
        {
            while (!destruido && pendiente != null)
            {
                Solicitud solicitud = pendiente;
                pendiente = null;
                var reloj = Stopwatch.StartNew();
                try
                {
                    var resultado = await solicitud.Buscar();
                    long consultaMs = reloj.ElapsedMilliseconds;
                    if (destruido || pendiente != null || !SesionActual.TienePermiso(permisoConsulta)) continue;
                    resultado.AlAplicar?.Invoke();
                    AplicarResultados(resultado.Datos, solicitud);
                    Trace.TraceInformation("Citorna {0}: consulta {1} ms, aplicación {2} ms, {3} filas.",
                        plural, consultaMs, reloj.ElapsedMilliseconds - consultaMs, resultado.Datos.Count);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("Citorna {0}: {1}", plural, ex.GetType().Name);
                    if (!destruido && pendiente == null)
                        MostrarEstado(registros.Count == 0 ? $"No se pudieron cargar {(femenino ? "las" : "los")} {plural}."
                            : "No se pudo actualizar. Se conservan los resultados anteriores.", "Reintentar", () => (ReintentarConsulta ?? Consultar)?.Invoke());
                }
            }
        }
        finally
        {
            consultando = false;
            if (!destruido) ActualizarAcciones();
        }
    }

    private void AplicarResultados(List<T> datos, Solicitud solicitud)
    {
        int? idVisible = solicitud.ConservarSeleccion && Seleccion is { } anterior ? idDe(anterior) : solicitud.Id;
        aplicando = true;
        Tabla.SuspendLayout();
        try
        {
            Tabla.CurrentCell = null;
            Tabla.ClearSelection();
            Tabla.RowCount = 0;
            registros = datos;
            Tabla.RowCount = datos.Count;
            Tabla.ClearSelection();
            Tabla.CurrentCell = null;
            int indice = idVisible is { } id ? datos.FindIndex(item => idDe(item) == id) : -1;
            if (indice >= 0)
            {
                Tabla.CurrentCell = Tabla[0, indice];
                Tabla.Rows[indice].Selected = true;
            }
        }
        finally
        {
            Tabla.ResumeLayout(true);
            aplicando = false;
        }
        datosVigentes = true;
        ActualizarSeleccion();
        if (datos.Count > 0)
            MostrarEstado(datos.Count == 1 ? "1 registro encontrado" : $"{datos.Count} registros encontrados");
        else if (solicitud.Filtrada)
            MostrarEstado($"No se encontraron {plural} con estos filtros.", "Restablecer", () => Restablecer?.Invoke());
        else
            MostrarEstado($"Aún no hay {plural} {(femenino ? "registradas" : "registrados")}.");
    }

    private void MostrarEstado(string mensaje, string? accion = null, Action? callback = null)
    {
        estado.Text = mensaje;
        accionEstadoActual = callback;
        if (accion != null) accionEstado.Text = accion;
        accionEstado.Visible = callback != null;
    }

    internal async Task EliminarConfirmadoAsync(string nombre, Func<ResultadoOperacion> ejecutar, Action actualizar)
    {
        if (ocupado || !SesionActual.VerificarPermiso(permisoConsulta, $"eliminar {plural}")
            || !SesionActual.VerificarPermiso(PermisosSistema.Eliminar, $"eliminar {plural}")) return;
        if (MessageBox.Show(FindForm(), $"¿Eliminar {nombre}? Esta acción no se puede deshacer.",
            "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;
        await EjecutarOperacionAsync("Eliminar", ejecutar, resultado =>
        {
            if (resultado.Exitoso)
                actualizar();
            else
                MessageBox.Show(FindForm(), resultado.Mensaje, $"Eliminar {singular}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        });
    }

    internal Task EjecutarOperacionAsync(string titulo, Func<ResultadoOperacion> ejecutar, Action<ResultadoOperacion> presentar) =>
        EjecutarOperacionAsync(titulo, () => Task.Run(ejecutar), presentar);

    internal async Task EjecutarOperacionAsync(string titulo, Func<Task<ResultadoOperacion>> ejecutar,
        Action<ResultadoOperacion> presentar)
    {
        if (ocupado) return;
        ocupado = true;
        ActualizarAcciones();
        try
        {
            ResultadoOperacion resultado = await ejecutar();
            if (!destruido) presentar(resultado);
        }
        catch (Exception ex)
        {
            Trace.TraceWarning("Citorna, {0} {1}: {2}", titulo, singular, ex.GetType().Name);
            if (!destruido) MessageBox.Show(FindForm(), "No se pudo completar la operación. Vuelva a intentarlo.",
                $"{titulo} {singular}", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            ocupado = false;
            if (!destruido) ActualizarAcciones();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { destruido = true; pendiente = null; }
        base.Dispose(disposing);
        if (!disposing) return;
        fuenteTitulo.Dispose();
        fuenteFicha.Dispose();
        fuenteEtiqueta.Dispose();
        fuenteBase.Dispose();
    }
}
