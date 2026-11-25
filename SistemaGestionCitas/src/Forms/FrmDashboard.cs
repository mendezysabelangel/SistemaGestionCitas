using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmDashboard : Form
    {
        private readonly Dictionary<Form, string> seccionesHijas = new();
        private readonly List<BotonMenu> botones = new();
        private readonly Stopwatch relojMenu = new();
        private readonly System.Windows.Forms.Timer animacionMenu;
        private readonly ToolTip ayudas;
        private readonly Font fuente;
        private readonly Font fuenteActiva;
        private readonly AgendaInicio agenda;
        private readonly PictureBox marca = new();
        private readonly BotonMenu btnPlegar = new("Contraer menú", SimboloAgenda.Anterior);
        private readonly BotonMenu btnInicio = new("Inicio", SimboloAgenda.Inicio);
        private readonly BotonMenu btnCitas = new("Citas", SimboloAgenda.Calendario);
        private readonly BotonMenu btnClientes = new("Clientes", SimboloAgenda.Clientes);
        private readonly BotonMenu btnServicios = new("Servicios", SimboloAgenda.Servicios);
        private readonly BotonMenu btnProductos = new("Productos", SimboloAgenda.Productos);
        private readonly BotonMenu btnEmpleados = new("Empleados", SimboloAgenda.Empleados);
        private readonly BotonMenu btnUsuario = new("Usuarios", SimboloAgenda.Usuarios);
        private readonly BotonMenu btnCrearUsuario = new("Crear usuario", SimboloAgenda.UsuarioNuevo);
        private readonly BotonMenu btnCrearRol = new("Crear rol", SimboloAgenda.Rol);
        private readonly BotonMenu btnBitacora = new("Bitácora", SimboloAgenda.Reloj);
        private readonly BotonMenu btnCerrarSesion = new("Cerrar sesión", SimboloAgenda.Salir);
        private readonly Bitmap logoLargo;
        private readonly Bitmap logoIcono;
        private BotonMenu? botonActivo;
        private BotonMenu? botonModal;
        private string seccionModal = "";
        private bool contraido;
        private bool modoAplicadoContraido;
        private bool iniciado;
        private int anchoInicial;

        public FrmDashboard()
        {
            InitializeComponent();
            SuspendLayout();
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;
            fuente = new Font("Segoe UI", 11F);
            fuenteActiva = new Font(fuente, FontStyle.Bold);
            Font = fuente;
            logoLargo = CargarMarca("Citorna.Login.LogoLargo.png");
            logoIcono = CargarMarca("Citorna.Dashboard.LogoIcono.png");

            components ??= new System.ComponentModel.Container();
            ayudas = new ToolTip(components);
            animacionMenu = new System.Windows.Forms.Timer(components) { Interval = 15 };
            animacionMenu.Tick += (_, _) => AnimarMenu();
            agenda = new AgendaInicio { Dock = DockStyle.Fill };
            pnlContenido.Controls.Add(agenda);
            ConstruirMenu();
            CargarSesion();
            SeleccionarMenu(btnInicio, false);
            MdiChildActivate += (_, _) => ActualizarNavegacion();
            if (AreaMdi is { } area) area.SizeChanged += (_, _) => AjustarHijos();
            Shown += (_, _) =>
            {
                iniciado = true;
                agenda.ActualizarPermiso();
            };
            FormClosed += (_, _) =>
            {
                animacionMenu.Stop();
                relojMenu.Stop();
                SesionActual.CerrarSesion();
            };
            Disposed += (_, _) =>
            {
                logoLargo.Dispose();
                logoIcono.Dispose();
                fuenteActiva.Dispose();
                fuente.Dispose();
            };
            AutoScaleDimensions = new SizeF(96F, 96F);
            ResumeLayout(true);
        }

        private MdiClient? AreaMdi => Controls.OfType<MdiClient>().FirstOrDefault();

        private void ConstruirMenu()
        {
            var estructura = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4,
                BackColor = EstiloAgenda.Barra, TabStop = false };
            estructura.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            estructura.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
            estructura.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            estructura.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            estructura.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));

            var marcaMenu = new TableLayoutPanel { Dock = DockStyle.Fill, Margin = new Padding(10, 11, 10, 8),
                ColumnCount = 1, RowCount = 2 };
            marcaMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            marcaMenu.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            marcaMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            btnPlegar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            btnPlegar.Width = 38;
            btnPlegar.SoloIcono = true;
            btnPlegar.Click += (_, _) => CambiarAnchoMenu();
            marcaMenu.Controls.Add(btnPlegar, 0, 0);
            marca.Dock = DockStyle.Fill;
            marca.Margin = new Padding(12, 2, 12, 5);
            marca.Image = logoLargo;
            marca.SizeMode = PictureBoxSizeMode.Zoom;
            marca.AccessibleName = "Citorna";
            marcaMenu.Controls.Add(marca, 0, 1);
            ayudas.SetToolTip(btnPlegar, btnPlegar.AccessibleName);
            estructura.Controls.Add(marcaMenu, 0, 0);

            var navegacion = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = new Padding(9, 0, 9, 0) };
            botones.AddRange([btnInicio, btnCitas, btnClientes, btnServicios, btnProductos, btnEmpleados,
                btnUsuario, btnCrearUsuario, btnCrearRol, btnBitacora, btnCerrarSesion]);
            for (int i = botones.Count - 2; i >= 0; i--)
            {
                BotonMenu boton = botones[i];
                boton.Height = 48;
                boton.Dock = DockStyle.Top;
                boton.TabIndex = i;
                navegacion.Controls.Add(boton);
            }
            btnInicio.Click += (_, _) => VolverAInicio();
            btnCitas.Click += (_, _) => AbrirCitas();
            btnClientes.Click += (_, _) => AbrirConsulta<FrmClientes>("Clientes");
            btnServicios.Click += (_, _) => AbrirConsulta<FrmServicios>("Servicios");
            btnProductos.Click += (_, _) => AbrirConsulta<FrmProductos>("Productos");
            btnEmpleados.Click += (_, _) => AbrirConsulta<FrmEmpleados>("Empleados");
            btnBitacora.Click += (_, _) =>
            {
                if (BitacoraCitasService.PuedeConsultar())
                    AbrirModulo<FrmBitacoraCitas>("Bitácora de Citas");
            };
            btnUsuario.Click += (_, _) =>
            {
                if (SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, "consultar usuarios"))
                    AbrirModulo<FrmUsuarios>("Usuarios");
            };
            btnCrearUsuario.Click += (_, _) =>
            {
                if (!SesionActual.VerificarPermiso(PermisosSistema.CrearUsuarios, "crear usuarios")) return;
                using CrearUsuario formulario = new();
                AbrirDialogo(formulario, btnCrearUsuario);
            };
            btnCrearRol.Click += (_, _) =>
            {
                if (!SesionActual.VerificarPermiso(PermisosSistema.CrearRoles, "crear nuevos roles")) return;
                using FrmCrearRol formulario = new();
                AbrirDialogo(formulario, btnCrearRol);
            };
            estructura.Controls.Add(navegacion, 0, 1);

            btnCerrarSesion.Dock = DockStyle.Fill;
            btnCerrarSesion.Margin = new Padding(9, 1, 9, 4);
            btnCerrarSesion.Click += (_, _) =>
            {
                SesionActual.CerrarSesion();
                Close();
            };
            estructura.Controls.Add(btnCerrarSesion, 0, 2);
            estructura.Controls.Add(new OndasMenu { Dock = DockStyle.Fill, Margin = Padding.Empty }, 0, 3);
            pnlSidebar.Controls.Add(estructura);
        }

        private static Bitmap CargarMarca(string nombre)
        {
            using Stream archivo = typeof(FrmDashboard).Assembly.GetManifestResourceStream(nombre)
                ?? throw new InvalidOperationException($"No se encontró el recurso {nombre}.");
            using Image imagen = Image.FromStream(archivo);
            return new Bitmap(imagen);
        }

        private void CambiarAnchoMenu()
        {
            contraido = !contraido;
            btnPlegar.Simbolo = contraido ? SimboloAgenda.Siguiente : SimboloAgenda.Anterior;
            btnPlegar.AccessibleName = contraido ? "Expandir menú" : "Contraer menú";
            ayudas.SetToolTip(btnPlegar, btnPlegar.AccessibleName);
            btnPlegar.Invalidate();
            if (contraido) AplicarModoMenu();
            IniciarAnimacion();
            if (!animacionMenu.Enabled) AplicarModoMenu();
        }

        private void AplicarModoMenu()
        {
            if (modoAplicadoContraido == contraido) return;
            modoAplicadoContraido = contraido;
            marca.Image = contraido ? logoIcono : logoLargo;
            int margen = LogicalToDeviceUnits(contraido ? 1 : 12);
            marca.Margin = new Padding(margen, LogicalToDeviceUnits(2), margen, LogicalToDeviceUnits(5));
            foreach (BotonMenu boton in botones)
            {
                boton.SoloIcono = contraido;
                boton.Text = contraido ? "" : boton.Etiqueta;
                ayudas.SetToolTip(boton, contraido ? boton.Etiqueta : null);
                boton.Invalidate();
            }
        }

        private void SeleccionarMenu(BotonMenu seleccionado, bool animar = true)
        {
            if (botonActivo == seleccionado) return;
            if (botonActivo != null)
            {
                botonActivo.Font = fuente;
                botonActivo.AccessibleDescription = null;
            }
            botonActivo = seleccionado;
            seleccionado.Font = fuenteActiva;
            seleccionado.AccessibleDescription = "Sección actual";
            IniciarAnimacion(animar);
        }

        private void IniciarAnimacion(bool animar = true)
        {
            animacionMenu.Stop();
            anchoInicial = pnlSidebar.Width;
            foreach (BotonMenu boton in botones) boton.IntensidadInicial = boton.Intensidad;
            if (animar && Visible && SystemInformation.IsMenuAnimationEnabled && SystemInformation.UIEffectsEnabled && !SystemInformation.HighContrast)
            {
                relojMenu.Restart();
                animacionMenu.Start();
            }
            else
            {
                AplicarPasoMenu(1);
            }
        }

        private void AnimarMenu()
        {
            double tiempo = Math.Min(1, relojMenu.Elapsed.TotalMilliseconds / 180);
            AplicarPasoMenu(tiempo * tiempo * (3 - 2 * tiempo));
            if (tiempo < 1) return;
            animacionMenu.Stop();
            relojMenu.Stop();
            AplicarModoMenu();
        }

        private void AplicarPasoMenu(double progreso)
        {
            int destino = LogicalToDeviceUnits(contraido ? 72 : 236);
            int ancho = (int)Math.Round(anchoInicial + (destino - anchoInicial) * progreso);
            if (pnlSidebar.Width != ancho) pnlSidebar.Width = ancho;
            foreach (BotonMenu boton in botones)
            {
                double valor = boton.IntensidadInicial + ((boton == botonActivo ? 1 : 0) - boton.IntensidadInicial) * progreso;
                if (Math.Abs(boton.Intensidad - valor) < .001) continue;
                boton.Intensidad = valor;
                boton.Invalidate();
            }
        }

        public void ActualizarDatosSesion() => CargarSesion();

        private void CargarSesion()
        {
            bool consultar = SesionActual.TienePermiso(PermisosSistema.Consultar);
            bool usuarios = SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);
            btnCitas.Visible = btnClientes.Visible = btnServicios.Visible = btnProductos.Visible = btnEmpleados.Visible = consultar;
            btnUsuario.Visible = btnCrearUsuario.Visible = usuarios;
            btnCrearRol.Visible = SesionActual.TienePermiso(PermisosSistema.CrearRoles);
            btnBitacora.Visible = BitacoraCitasService.PuedeConsultar();
            if (iniciado) agenda.ActualizarPermiso();
        }

        private void AbrirConsulta<T>(string seccion) where T : Form, new()
        {
            if (SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar información"))
                AbrirModulo<T>(seccion);
        }

        private void AbrirCitas()
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar citas")) return;
            if (!ActivarHijoAbierto(typeof(FrmCitas)))
                MostrarHijo(new FrmCitas(agenda.FechaSeleccionada, agenda.ActualizarCitas), "Citas");
        }

        internal void AbrirModulo<T>(string seccion) where T : Form, new()
        {
            // Consultar las ventanas abiertas antes de construir controles para otra instancia.
            if (!ActivarHijoAbierto(typeof(T))) MostrarHijo(new T(), seccion);
        }

        private void AbrirDialogo(Form formulario, BotonMenu boton)
        {
            botonModal = boton;
            seccionModal = boton.Etiqueta;
            ActualizarNavegacion();
            try
            {
                formulario.ShowDialog(this);
            }
            finally
            {
                botonModal = null;
                seccionModal = "";
                ActualizarNavegacion();
            }
        }

        private void VolverAInicio()
        {
            foreach (Form hijo in MdiChildren) hijo.Close();
            ActualizarNavegacion();
        }

        private void ActualizarNavegacion()
        {
            if (IsDisposed || Disposing) return;
            Form? hijo = ActiveMdiChild;
            if (hijo == null || hijo.IsDisposed || !hijo.Visible || !seccionesHijas.ContainsKey(hijo))
                hijo = seccionesHijas.Keys.LastOrDefault(formulario => !formulario.IsDisposed && formulario.Visible);
            pnlContenido.Visible = hijo == null;
            string seccion = hijo == null ? "Inicio" : seccionesHijas[hijo];
            Text = $"Citorna — {(botonModal != null ? seccionModal : seccion)}";
            SeleccionarMenu(botonModal ?? (seccion switch
            {
                "Clientes" or "Reporte de clientes" => btnClientes,
                "Citas" => btnCitas,
                "Servicios" => btnServicios,
                "Productos" or "Reporte de inventario" => btnProductos,
                "Empleados" => btnEmpleados,
                "Usuarios" => btnUsuario,
                "Bitácora de Citas" => btnBitacora,
                _ => btnInicio
            }));
        }

        private bool ActivarHijoAbierto(Type tipo)
        {
            Form? abierto = MdiChildren.FirstOrDefault(hijo => hijo.GetType() == tipo && !hijo.IsDisposed);
            if (abierto == null) return false;
            if (abierto.WindowState == FormWindowState.Minimized) abierto.WindowState = FormWindowState.Normal;
            if (ActiveMdiChild != abierto) abierto.Activate();
            return true;
        }

        public void AbrirHijo(Form hijo, string seccion, bool permitirDuplicados = false)
        {
            if (!permitirDuplicados && ActivarHijoAbierto(hijo.GetType()))
            {
                hijo.Dispose();
                return;
            }

            MostrarHijo(hijo, seccion);
        }

        private void MostrarHijo(Form hijo, string seccion)
        {
            if (pnlContenido.Visible) pnlContenido.Hide();
            seccionesHijas[hijo] = seccion;
            hijo.FormClosed += (_, _) =>
            {
                seccionesHijas.Remove(hijo);
                ActualizarNavegacion();
            };

            hijo.MdiParent = this;
            hijo.StartPosition = FormStartPosition.Manual;
            if (AreaMdi is { } area) hijo.Bounds = new Rectangle(Point.Empty, area.ClientSize);
            hijo.Show();
            ActualizarNavegacion();
        }

        private void AjustarHijos()
        {
            if (AreaMdi is not { } area || AreaMdi.ClientSize.Width < 1 || AreaMdi.ClientSize.Height < 1) return;
            foreach (Form hijo in MdiChildren)
            {
                if (hijo.WindowState == FormWindowState.Normal && !hijo.IsDisposed)
                    hijo.Bounds = new Rectangle(Point.Empty, area.ClientSize);
            }
        }

        private sealed class BotonMenu : Button
        {
            public string Etiqueta { get; }
            public SimboloAgenda Simbolo { get; set; }
            public bool SoloIcono { get; set; }
            public double Intensidad { get; set; }
            public double IntensidadInicial { get; set; }

            public BotonMenu(string texto, SimboloAgenda simbolo)
            {
                Etiqueta = texto;
                Text = texto;
                AccessibleName = texto;
                Simbolo = simbolo;
                Margin = Padding.Empty;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                BackColor = EstiloAgenda.Barra;
                ForeColor = EstiloAgenda.Tinta;
                UseVisualStyleBackColor = false;
                DoubleBuffered = true;
                ResizeRedraw = true;
                Cursor = Cursors.Hand;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.Clear(EstiloAgenda.Barra);
                if (Width < 6 || Height < 6) return;
                int margen = LogicalToDeviceUnits(3);
                var rect = new Rectangle(margen, margen, Width - margen * 2, Height - margen * 2);
                if (rect.Width <= 0 || rect.Height <= 0) return;
                bool encima = ClientRectangle.Contains(PointToClient(MousePosition));
                if (Intensidad > 0 || encima || Focused)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using GraphicsPath forma = EstiloAgenda.RectanguloRedondo(rect, LogicalToDeviceUnits(9));
                    using SolidBrush relleno = new(Color.FromArgb(
                        (int)(EstiloAgenda.Seleccion.R * Intensidad + 239 * (1 - Intensidad)),
                        (int)(EstiloAgenda.Seleccion.G * Intensidad + 246 * (1 - Intensidad)),
                        (int)(EstiloAgenda.Seleccion.B * Intensidad + 254 * (1 - Intensidad))));
                    e.Graphics.FillPath(relleno, forma);
                    if (Focused)
                    {
                        using Pen borde = new(EstiloAgenda.Azul, LogicalToDeviceUnits(2));
                        e.Graphics.DrawPath(borde, forma);
                    }
                }

                int lado = LogicalToDeviceUnits(23);
                int x = SoloIcono ? (Width - lado) / 2 : LogicalToDeviceUnits(15);
                IconosAgenda.Dibujar(e.Graphics, Simbolo, EstiloAgenda.Tinta, new(x, (Height - lado) / 2, lado, lado));
                int izquierda = LogicalToDeviceUnits(54);
                if (!SoloIcono && Width > izquierda + margen)
                    TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(izquierda, 0, Width - izquierda - margen, Height),
                        EstiloAgenda.Tinta, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                Invalidate();
            }
        }
    }
}
