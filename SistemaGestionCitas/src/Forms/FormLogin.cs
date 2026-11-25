using System.Drawing.Drawing2D;
using System.Windows.Forms.Automation;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FormLogin : Form
    {
        private static readonly Color Azul = Color.FromArgb(13, 79, 179);
        private static readonly Color AzulMarino = Color.FromArgb(4, 39, 71);
        private static readonly Color TextoEtiqueta = Color.FromArgb(67, 81, 97);
        private static readonly Color BordeCampo = Color.FromArgb(105, 124, 146);

        private readonly AuthService authService = new AuthService();
        private readonly Panel pnlCentro = new Panel();
        private readonly PictureBox picLogo = new PictureBox();
        private readonly TextBox txtUsuario = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly Button btnVerPassword = new Button();
        private readonly BotonPrincipal btnIniciar = new BotonPrincipal();
        private readonly LinkLabel btnSalir = new LinkLabel();
        private readonly Label lblMensaje = new Label();
        private readonly Button btnCambiarPantalla = new Button();
        private ToolTip? ayuda;
        private Bitmap? fondo;

        public FormLogin()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            SuspendLayout();
            Text = "Citorna — Iniciar sesión";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1100, 650);
            MinimumSize = new Size(900, 600);
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.FromArgb(219, 230, 247);

            Font fuente = new Font("Segoe UI", 11F);
            Font = fuente;
            ayuda = new ToolTip(components ??= new System.ComponentModel.Container());
            fondo = CargarImagen("FondoLogin.png");
            Bitmap logo = CargarImagen("LogoLargo.png");
            Disposed += (_, _) =>
            {
                fondo.Dispose();
                logo.Dispose();
                fuente.Dispose();
            };

            pnlCentro.Size = new Size(362, 420);
            pnlCentro.BackColor = Color.Transparent;
            pnlCentro.TabStop = false;

            picLogo.Image = logo;
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.Bounds = new Rectangle(9, 0, 344, 134);
            picLogo.AccessibleName = "Citorna";
            picLogo.TabStop = false;
            pnlCentro.Controls.Add(picLogo);

            txtUsuario.MaxLength = 50;
            txtPassword.MaxLength = 100;
            txtPassword.UseSystemPasswordChar = true;
            pnlCentro.Controls.Add(CrearCampo("Usuario", txtUsuario, 150, 0, false));
            pnlCentro.Controls.Add(CrearCampo("Contraseña", txtPassword, 222, 1, true));

            lblMensaje.Bounds = new Rectangle(6, 282, 350, 47);
            lblMensaje.BackColor = Color.Transparent;
            lblMensaje.ForeColor = Color.FromArgb(121, 30, 39);
            lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
            lblMensaje.LiveSetting = AutomationLiveSetting.Assertive;
            pnlCentro.Controls.Add(lblMensaje);

            btnIniciar.Text = "Iniciar sesión";
            btnIniciar.Bounds = new Rectangle(0, 334, 362, 52);
            btnIniciar.TabIndex = 2;
            btnIniciar.Cursor = Cursors.Hand;
            btnIniciar.Click += BtnIniciar_Click;
            pnlCentro.Controls.Add(btnIniciar);

            btnSalir.Text = "Salir";
            btnSalir.Bounds = new Rectangle(132, 390, 98, 30);
            btnSalir.TextAlign = ContentAlignment.MiddleCenter;
            btnSalir.LinkColor = AzulMarino;
            btnSalir.ActiveLinkColor = Azul;
            btnSalir.TabIndex = 3;
            btnSalir.TabStop = true;
            btnSalir.LinkClicked += BtnSalir_Click;
            pnlCentro.Controls.Add(btnSalir);

            Controls.Add(pnlCentro);
            AcceptButton = btnIniciar;
            Resize += (_, _) => CentrarContenido();
            // Las medidas anteriores están expresadas a 96 DPI.
            AutoScaleDimensions = new SizeF(96F, 96F);
            ResumeLayout(true);
            CentrarContenido();
        }

        private Panel CrearCampo(string nombre, TextBox texto, int posicionY, int orden, bool password)
        {
            Panel campo = new Panel
            {
                Bounds = new Rectangle(0, posicionY, 362, 56),
                BackColor = Color.Transparent,
                TabIndex = orden,
                TabStop = false
            };

            Label etiqueta = new Label
            {
                Text = nombre,
                AutoSize = true,
                Location = new Point(57, 5),
                BackColor = Color.Transparent,
                ForeColor = TextoEtiqueta,
                TabIndex = 0
            };
            texto.AccessibleName = nombre;
            texto.TabIndex = 1;
            texto.BorderStyle = BorderStyle.None;
            texto.BackColor = Color.White;
            texto.ForeColor = AzulMarino;
            texto.Location = new Point(58, 26);
            texto.Width = password ? 237 : 277;
            campo.Controls.Add(etiqueta);
            campo.Controls.Add(texto);

            if (password)
            {
                btnVerPassword.Bounds = new Rectangle(298, 6, 44, 44);
                btnVerPassword.TabIndex = 2;
                btnVerPassword.BackColor = Color.White;
                btnVerPassword.FlatStyle = FlatStyle.Flat;
                btnVerPassword.FlatAppearance.BorderSize = 0;
                btnVerPassword.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 239, 252);
                btnVerPassword.FlatAppearance.MouseDownBackColor = Color.FromArgb(216, 229, 248);
                btnVerPassword.Cursor = Cursors.Hand;
                btnVerPassword.Click += BtnVerPassword_Click;
                btnVerPassword.Paint += PintarOjo;
                campo.Controls.Add(btnVerPassword);
                ActualizarBotonPassword();
            }

            campo.Paint += (_, e) => PintarCampo(e.Graphics, campo, password);
            campo.Enter += (_, _) => campo.Invalidate();
            campo.Leave += (_, _) => campo.Invalidate();
            campo.Click += (_, _) => texto.Focus();
            etiqueta.Click += (_, _) => texto.Focus();
            return campo;
        }

        private static Bitmap CargarImagen(string nombre)
        {
            using Stream recurso = typeof(FormLogin).Assembly.GetManifestResourceStream($"Citorna.Login.{nombre}")
                ?? throw new InvalidOperationException($"No se encontró el recurso de Citorna {nombre}.");
            using Image imagen = Image.FromStream(recurso);
            return new Bitmap(imagen);
        }

        private void CentrarContenido()
        {
            int margen = LogicalToDeviceUnits(12);
            int arriba = (int)(ClientSize.Height * 0.065);
            pnlCentro.Location = new Point(
                (ClientSize.Width - pnlCentro.Width) / 2,
                Math.Max(margen, Math.Min(arriba, ClientSize.Height - pnlCentro.Height - margen)));
            pnlCentro.Invalidate(true);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (fondo == null || ClientSize.Width == 0 || ClientSize.Height == 0)
            {
                base.OnPaintBackground(e);
                return;
            }

            // Cubre toda la ventana sin deformar la foto. El recorte horizontal cae a la izquierda.
            float escala = Math.Max((float)ClientSize.Width / fondo.Width, (float)ClientSize.Height / fondo.Height);
            float ancho = fondo.Width * escala;
            float alto = fondo.Height * escala;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImage(fondo, new RectangleF(ClientSize.Width - ancho,
                (ClientSize.Height - alto) / 2, ancho, alto));
        }

        private static GraphicsPath CrearPildora(RectangleF rectangulo)
        {
            float diametro = rectangulo.Height;
            GraphicsPath ruta = new GraphicsPath();
            ruta.AddArc(rectangulo.Left, rectangulo.Top, diametro, diametro, 90, 180);
            ruta.AddArc(rectangulo.Right - diametro, rectangulo.Top, diametro, diametro, 270, 180);
            ruta.CloseFigure();
            return ruta;
        }

        private void PintarCampo(Graphics lienzo, Panel campo, bool password)
        {
            lienzo.SmoothingMode = SmoothingMode.AntiAlias;
            using GraphicsPath forma = CrearPildora(new RectangleF(1.5F, 1.5F, campo.Width - 3, campo.Height - 3));
            using Pen borde = new Pen(campo.ContainsFocus ? Azul : BordeCampo,
                campo.ContainsFocus ? LogicalToDeviceUnits(2) : 1F);
            lienzo.FillPath(Brushes.White, forma);
            lienzo.DrawPath(borde, forma);

            // Iconos lineales decorativos. Las etiquetas identifican los campos.
            var estado = lienzo.Save();
            lienzo.TranslateTransform(LogicalToDeviceUnits(29), campo.Height / 2F);
            float escala = campo.DeviceDpi / 96F;
            lienzo.ScaleTransform(escala, escala);
            using Pen icono = new Pen(TextoEtiqueta, 1.5F);
            if (password)
            {
                lienzo.DrawArc(icono, -5, -10, 10, 12, 180, 180);
                lienzo.DrawRectangle(icono, -7, -4, 14, 13);
            }
            else
            {
                lienzo.DrawEllipse(icono, -4, -10, 8, 8);
                lienzo.DrawArc(icono, -8, 0, 16, 16, 180, 180);
            }
            lienzo.Restore(estado);
        }

        private void ActualizarBotonPassword()
        {
            string accion = txtPassword.UseSystemPasswordChar
                ? "Contraseña oculta; mostrar contraseña"
                : "Contraseña visible; ocultar contraseña";
            btnVerPassword.AccessibleName = accion;
            ayuda?.SetToolTip(btnVerPassword, accion);
            btnVerPassword.Invalidate();
        }

        private void PintarOjo(object? sender, PaintEventArgs e)
        {
            Graphics lienzo = e.Graphics;
            var estado = lienzo.Save();
            lienzo.SmoothingMode = SmoothingMode.AntiAlias;
            lienzo.TranslateTransform(btnVerPassword.Width / 2F, btnVerPassword.Height / 2F);
            float escala = btnVerPassword.DeviceDpi / 96F;
            lienzo.ScaleTransform(escala, escala);
            using Pen trazo = new Pen(AzulMarino, 1.6F);
            using GraphicsPath ojo = new GraphicsPath();
            ojo.AddBezier(-10, 0, -4, -8, 4, -8, 10, 0);
            ojo.AddBezier(10, 0, 4, 8, -4, 8, -10, 0);
            lienzo.DrawPath(trazo, ojo);
            lienzo.DrawEllipse(trazo, -2.5F, -2.5F, 5, 5);
            if (txtPassword.UseSystemPasswordChar)
                lienzo.DrawLine(trazo, -9, 8, 9, -8);
            lienzo.Restore(estado);
        }

        private sealed class BotonPrincipal : Button
        {
            public BotonPrincipal()
            {
                FlatStyle = FlatStyle.Flat;
                BackColor = Color.Transparent;
                ForeColor = Color.White;
                DoubleBuffered = true;
                ResizeRedraw = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                // Button pinta en modo opaco. Dibujamos primero el fondo del padre.
                base.OnPaintBackground(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using GraphicsPath forma = CrearPildora(new RectangleF(1, 1, Width - 2, Height - 2));
                bool resaltado = ClientRectangle.Contains(PointToClient(MousePosition));
                using SolidBrush relleno = new SolidBrush(resaltado ? Color.FromArgb(9, 63, 148) : Azul);
                e.Graphics.FillPath(relleno, forma);
                if (Focused)
                {
                    int margen = LogicalToDeviceUnits(5);
                    using GraphicsPath foco = CrearPildora(new RectangleF(margen, margen,
                        Width - 2 * margen, Height - 2 * margen));
                    using Pen contraste = new Pen(Color.White, LogicalToDeviceUnits(2));
                    e.Graphics.DrawPath(contraste, foco);
                }
                TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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

        //  EVENTOS DE LOS BOTONES

        // Mostrar/ocultar la contraseña
        private void BtnVerPassword_Click(object? sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            ActualizarBotonPassword();
            txtPassword.Focus();
        }

        // Botón Iniciar sesión
        private void BtnIniciar_Click(object? sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            lblMensaje.Text = "";
            ResultadoLogin resultado;

            try
            {
                resultado = authService.IniciarSesion(usuario, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar con la base de datos.\n\n" + ex.Message,
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!resultado.Exitoso)
            {
                lblMensaje.Text = resultado.Mensaje;
                txtPassword.Clear();
                if (string.IsNullOrWhiteSpace(usuario)) txtUsuario.Focus();
                else txtPassword.Focus();
                return;
            }

            this.Hide();

            using (FrmDashboard dashboard = new FrmDashboard())
            {
                dashboard.ShowDialog();
            }

            txtUsuario.Clear();
            txtPassword.Clear();
            txtPassword.UseSystemPasswordChar = true;
            ActualizarBotonPassword();

            this.Show();
            txtUsuario.Focus();
        }

        // Botón Salir
        private void BtnSalir_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        // Botón cambiar de pantalla
        private void BtnCambiarPantalla_Click(object? sender, EventArgs e)
        {
            CrearUsuario crearUsuario = new CrearUsuario();
            crearUsuario.Show();
            this.Hide();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            ActiveControl = txtUsuario;
        }
    }
}
