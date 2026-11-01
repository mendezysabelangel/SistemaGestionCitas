using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;
using System.Diagnostics;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmDashboard : Form
    {
        private readonly Dictionary<Form, string> seccionesHijas = new Dictionary<Form, string>();
        private readonly Dictionary<Button, (Color Fondo, Color Texto)> coloresInicialesMenu =
            new Dictionary<Button, (Color Fondo, Color Texto)>();
        private readonly Stopwatch relojAnimacionMenu = new Stopwatch();
        private readonly System.Windows.Forms.Timer animacionMenu;
        private readonly Font fuenteMenuNormal;
        private readonly Font fuenteMenuActiva;
        private Button[] botonesNavegacion = Array.Empty<Button>();
        private Button? botonMenuActivo;
        private Button? botonModalActivo;
        private string seccionModalActiva = "";

        public FrmDashboard()
        {
            InitializeComponent();
            components ??= new System.ComponentModel.Container();
            animacionMenu = new System.Windows.Forms.Timer(components) { Interval = 15 };
            animacionMenu.Tick += AnimacionMenu_Tick;
            fuenteMenuNormal = new Font(btnInicio.Font, FontStyle.Regular);
            fuenteMenuActiva = new Font(btnInicio.Font, FontStyle.Bold);
            ConfigurarNavegacion();
            CargarSesion();

            IsMdiContainer = true;                     // esta ventana ahora es contenedor MDI
            WindowState = FormWindowState.Maximized;   // necesita espacio para las ventanas hijas

            // Conectamos por código los botones del menú lateral
            btnClientes.Click += btnClientes_Click;
            btnInicio.Click += btnInicio_Click;

            MdiChildActivate += (s, e) => ActualizarNavegacion();
            FormClosed += FrmDashboard_FormClosed;
            Disposed += (s, e) =>
            {
                fuenteMenuNormal.Dispose();
                fuenteMenuActiva.Dispose();
            };
        }

        private void ConfigurarNavegacion()
        {
            botonesNavegacion = new[] { btnInicio, btnCitas, btnClientes, btnUsuario, btnCrearUsuario, btnCrearRol };
            foreach (Button boton in botonesNavegacion)
            {
                boton.UseVisualStyleBackColor = false;
                boton.BackColor = pnlSidebar.BackColor;
                boton.ForeColor = Color.FromArgb(70, 80, 95);
                boton.Font = fuenteMenuNormal;
            }

            SeleccionarBotonMenu(btnInicio, false);
        }

        private void SeleccionarBotonMenu(Button seleccionado, bool animar = true)
        {
            if (botonMenuActivo == seleccionado) return;

            animacionMenu.Stop();
            botonMenuActivo = seleccionado;
            coloresInicialesMenu.Clear();
            foreach (Button boton in botonesNavegacion)
            {
                coloresInicialesMenu[boton] = (boton.BackColor, boton.ForeColor);
                boton.Font = boton == seleccionado ? fuenteMenuActiva : fuenteMenuNormal;
            }

            if (animar && Visible)
            {
                relojAnimacionMenu.Restart();
                animacionMenu.Start();
            }
            else
            {
                AplicarColoresMenu(1);
            }
        }

        private void AnimacionMenu_Tick(object? sender, EventArgs e)
        {
            double progreso = Math.Min(1, relojAnimacionMenu.Elapsed.TotalMilliseconds / 180);
            AplicarColoresMenu(progreso * progreso * (3 - 2 * progreso));
            if (progreso >= 1)
            {
                animacionMenu.Stop();
                relojAnimacionMenu.Stop();
            }
        }

        private void AplicarColoresMenu(double progreso)
        {
            foreach (Button boton in botonesNavegacion)
            {
                bool activo = boton == botonMenuActivo;
                Color fondoFinal = activo ? Color.FromArgb(220, 232, 255) : pnlSidebar.BackColor;
                Color textoFinal = activo ? Color.FromArgb(28, 69, 171) : Color.FromArgb(70, 80, 95);
                var inicial = coloresInicialesMenu[boton];
                boton.BackColor = InterpolarColor(inicial.Fondo, fondoFinal, progreso);
                boton.ForeColor = InterpolarColor(inicial.Texto, textoFinal, progreso);
                boton.FlatAppearance.MouseOverBackColor = activo ? boton.BackColor : Color.FromArgb(235, 240, 248);
                boton.FlatAppearance.MouseDownBackColor = boton.FlatAppearance.MouseOverBackColor;
            }
        }

        private static Color InterpolarColor(Color inicial, Color final, double progreso)
        {
            return Color.FromArgb(
                (int)Math.Round(inicial.R + (final.R - inicial.R) * progreso),
                (int)Math.Round(inicial.G + (final.G - inicial.G) * progreso),
                (int)Math.Round(inicial.B + (final.B - inicial.B) * progreso));
        }

        private void ActualizarNavegacion()
        {
            if (IsDisposed || Disposing) return;

            Form? hijoActivo = ActiveMdiChild;
            if (hijoActivo == null || !seccionesHijas.ContainsKey(hijoActivo) || hijoActivo.IsDisposed)
                hijoActivo = seccionesHijas.Keys.LastOrDefault(hijo => !hijo.IsDisposed && hijo.Visible);

            pnlContenido.Visible = hijoActivo == null;
            if (botonModalActivo != null)
            {
                lblSeccion.Text = seccionModalActiva;
                SeleccionarBotonMenu(botonModalActivo);
                return;
            }

            string seccion = hijoActivo == null ? "Inicio" : seccionesHijas[hijoActivo];
            lblSeccion.Text = seccion;
            Button boton = seccion switch
            {
                "Clientes" or "Reporte de clientes" => btnClientes,
                "Usuarios" => btnUsuario,
                "Citas" => btnCitas,
                _ => btnInicio
            };
            SeleccionarBotonMenu(boton);
        }

        private void AbrirDialogo(Form formulario, Button boton, string seccion)
        {
            botonModalActivo = boton;
            seccionModalActiva = seccion;
            ActualizarNavegacion();
            try
            {
                formulario.ShowDialog(this);
            }
            finally
            {
                botonModalActivo = null;
                seccionModalActiva = "";
                ActualizarNavegacion();
            }
        }

        private void CargarSesion()
        {
            if (SesionActual.UsuarioActual == null)
                return;

            lblBienvenida.Text =
                $"Bienvenido, {SesionActual.UsuarioActual.NombreCompleto}";

            lblUsuario.Text =
                SesionActual.UsuarioActual.NombreUsuario;

            lblRol.Text =
                SesionActual.UsuarioActual.NombreRol;

            // Opciones administrativas
            btnCrearUsuario.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);

            btnUsuario.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);

            btnCrearRol.Visible =
                SesionActual.TienePermiso(PermisosSistema.CrearRoles);

            // Mostrar visualmente los permisos del rol
            ConfigurarBotonPermiso(
                btnConsultar,
                PermisosSistema.Consultar);

            ConfigurarBotonPermiso(
                btnAgregar,
                PermisosSistema.Agregar);

            ConfigurarBotonPermiso(
                btnModificar,
                PermisosSistema.Modificar);

            ConfigurarBotonPermiso(
                btnEliminar,
                PermisosSistema.Eliminar);
        }

        private void ConfigurarBotonPermiso(
            Button boton,
            string permiso)
        {
            bool permitido =
                SesionActual.TienePermiso(permiso);

            if (permitido)
            {
                boton.BackColor =
                    Color.FromArgb(220, 232, 255);

                boton.ForeColor =
                    Color.FromArgb(28, 69, 171);
            }
            else
            {
                boton.BackColor =
                    Color.FromArgb(235, 237, 240);

                boton.ForeColor =
                    Color.Gray;
            }
        }

        private void EjecutarAccion(
            string permiso,
            string accion)
        {
            if (!SesionActual.VerificarPermiso(
                permiso,
                accion))
            {
                return;
            }

            MessageBox.Show(
                $"Acceso permitido.\n\nSu rol puede {accion}.",
                "Permiso concedido",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnConsultar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Consultar,
                "consultar información");
        }

        private void btnAgregar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Agregar,
                "agregar registros");
        }

        private void btnModificar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Modificar,
                "modificar registros");
        }

        private void btnEliminar_Click(
            object sender,
            EventArgs e)
        {
            EjecutarAccion(
                PermisosSistema.Eliminar,
                "eliminar registros");
        }

        private void btnCrearUsuario_Click(
            object sender,
            EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.CrearUsuarios,
                "crear usuarios"))
            {
                return;
            }

            using CrearUsuario formulario =
                new CrearUsuario();

            AbrirDialogo(formulario, btnCrearUsuario, "Crear nuevo usuario");
        }

        private void btnCrearRol_Click(
            object sender,
            EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.CrearRoles,
                "crear nuevos roles"))
            {
                return;
            }

            using FrmCrearRol formulario =
                new FrmCrearRol();

            AbrirDialogo(formulario, btnCrearRol, "Crear nuevo rol");
        }

        // Menú lateral: Clientes
        private void btnClientes_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.Consultar, "consultar información"))
            {
                return;
            }

            AbrirHijo(new FrmClientes(), "Clientes");
        }

        // Menú lateral: Inicio (cierra las ventanas hijas y vuelve al resumen)
        private void btnInicio_Click(object? sender, EventArgs e)
        {
            foreach (Form hijo in MdiChildren)
                hijo.Close();

            ActualizarNavegacion();
        }

        // Abre un formulario como ventana hija MDI dentro de este dashboard
        // Antes: private void AbrirHijo(Form hijo, string seccion)
        public void AbrirHijo(Form hijo, string seccion, bool permitirDuplicados = false)
        {
            // Si ya hay una ventana de ese tipo abierta, solo se trae al frente
            if (!permitirDuplicados)
            {
                foreach (Form abierto in MdiChildren)
                {
                    if (abierto.GetType() == hijo.GetType())
                    {
                        abierto.Activate();
                        hijo.Dispose();
                        ActualizarNavegacion();
                        return;
                    }
                }
            }

            // El panel de resumen (Dock = Fill) taparía a las ventanas hijas: se oculta
            pnlContenido.Visible = false;
            PerformLayout();

            seccionesHijas[hijo] = seccion;
            hijo.FormClosed += (s, e) =>
            {
                seccionesHijas.Remove(hijo);
                ActualizarNavegacion();
            };

            // Se hace hija del dashboard y se ajusta al área disponible
            hijo.MdiParent = this;

            MdiClient? area = Controls.OfType<MdiClient>().FirstOrDefault();
            if (area != null)
            {
                hijo.StartPosition = FormStartPosition.Manual;
                hijo.Location = Point.Empty;
                hijo.Size = area.ClientSize;
            }

            hijo.Show();
            ActualizarNavegacion();
        }

        private void btnCerrarSesion_Click(
            object sender,
            EventArgs e)
        {
            SesionActual.CerrarSesion();
            Close();
        }

        private void FrmDashboard_FormClosed(
            object? sender,
            FormClosedEventArgs e)
        {
            animacionMenu.Stop();
            relojAnimacionMenu.Stop();
            SesionActual.CerrarSesion();
        }

        private void btnUsuarios_Click(
            object sender,
            EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(
                PermisosSistema.CrearUsuarios, "consultar usuarios"))
            {
                return;
            }

            AbrirHijo(new FrmUsuarios(), "Usuarios");
        }

        private void lblSistema_Click(
            object sender,
            EventArgs e)
        {
        }

        private void lblUsuario_Click(
            object sender,
            EventArgs e)
        {
        }

        private void pnlTopbar_Paint(
            object sender,
            PaintEventArgs e)
        {
        }
    }
}
