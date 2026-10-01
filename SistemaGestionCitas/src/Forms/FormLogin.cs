using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FormLogin : Form
    {
        // Colores
        private readonly Color azul = Color.FromArgb(27, 68, 170);
        private readonly Color fondoClaro = Color.FromArgb(247, 248, 252);
        private readonly Color textoOscuro = Color.FromArgb(17, 24, 39);
        private readonly Color textoGris = Color.FromArgb(90, 100, 115);
        private readonly Color azulClaro = Color.FromArgb(190, 205, 240);

       
        private readonly AuthService authService = new AuthService();

        // Elementos de la pantalla
        private readonly Panel pnlIzquierdo = new Panel();
        private readonly Panel pnlDerecho = new Panel();
        private readonly Panel pnlCentro = new Panel();
        private readonly TextBox txtUsuario = new TextBox();
        private readonly TextBox txtPassword = new TextBox();
        private readonly Button btnVerPassword = new Button();
        private readonly Button btnIniciar = new Button();
        private readonly Button btnSalir = new Button();
        private readonly Button btnCambiarPantalla = new Button();


        public FormLogin()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            Text = "Sistema de Gestión de Citas - Iniciar sesión";
            ClientSize = new Size(1100, 650);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);
            BackColor = fondoClaro;

            ConstruirPanelIzquierdo();
            ConstruirPanelDerecho();
            pnlDerecho.BringToFront();

            AcceptButton = btnIniciar;

            CentrarContenido();
        }

        private void ConstruirPanelIzquierdo()
        {
            pnlIzquierdo.Dock = DockStyle.Left;
            pnlIzquierdo.Width = 480;
            pnlIzquierdo.BackColor = azul;

            pnlIzquierdo.Controls.Add(CrearLabel("Sistema de Gestión", 40, 40, 400, 20, 10F, FontStyle.Bold, azulClaro));
            pnlIzquierdo.Controls.Add(CrearLabel("Administración de citas", 40, 55, 360, 60, 10F, FontStyle.Regular, azulClaro));
            pnlIzquierdo.Controls.Add(CrearLabel("Organiza las operaciones diarias desde un solo lugar.", 40, 255, 400, 120, 22F, FontStyle.Bold, Color.White));
            pnlIzquierdo.Controls.Add(CrearLabel("Administra citas, clientes, empleados y servicios con una estructura clara y segura.", 40, 375, 360, 60, 10F, FontStyle.Regular, azulClaro));

            Controls.Add(pnlIzquierdo);
        }

        private void ConstruirPanelDerecho()
        {
            pnlDerecho.Dock = DockStyle.Fill;
            pnlDerecho.BackColor = fondoClaro;
            pnlDerecho.Resize += (sender, e) => CentrarContenido();

            Panel pnlTarjeta = new Panel();
            pnlTarjeta.Size = new Size(430, 430);
            pnlTarjeta.Location = new Point(0, 0);
            pnlTarjeta.BackColor = Color.White;
            pnlTarjeta.BorderStyle = BorderStyle.FixedSingle;

            Label lblIcono = CrearLabel("🔒", 20, 20, 44, 44, 16F, FontStyle.Regular, azul);
            lblIcono.Font = new Font("Segoe UI Emoji", 16F);
            lblIcono.BackColor = Color.FromArgb(230, 236, 248);
            lblIcono.TextAlign = ContentAlignment.MiddleCenter;

            pnlTarjeta.Controls.Add(lblIcono);
            pnlTarjeta.Controls.Add(CrearLabel("Iniciar sesión", 20, 80, 390, 36, 18F, FontStyle.Bold, textoOscuro));
            pnlTarjeta.Controls.Add(CrearLabel("Accede para administrar las operaciones del sistema.", 20, 118, 390, 22, 9F, FontStyle.Regular, textoGris));

            // Usuario
            pnlTarjeta.Controls.Add(CrearLabel("Usuario", 20, 160, 390, 22, 9.5F, FontStyle.Bold, textoOscuro));
            txtUsuario.Location = new Point(20, 184);
            txtUsuario.Width = 390;
            txtUsuario.Font = new Font("Segoe UI", 10F);
            txtUsuario.PlaceholderText = "Ingrese su usuario";
            txtUsuario.MaxLength = 50;
            pnlTarjeta.Controls.Add(txtUsuario);

            // Contraseña
            pnlTarjeta.Controls.Add(CrearLabel("Contraseña", 20, 232, 390, 22, 9.5F, FontStyle.Bold, textoOscuro));
            txtPassword.Location = new Point(20, 256);
            txtPassword.Width = 350;
            txtPassword.Font = new Font("Segoe UI", 10F);
            txtPassword.PlaceholderText = "Ingrese su contraseña";
            txtPassword.MaxLength = 100;
            txtPassword.UseSystemPasswordChar = true;
            pnlTarjeta.Controls.Add(txtPassword);

            // Botón para mostrar y ocultar la contraseña
            btnVerPassword.Text = "👁";
            btnVerPassword.Font = new Font("Segoe UI Emoji", 10F);
            btnVerPassword.Location = new Point(376, 255);
            btnVerPassword.Size = new Size(34, txtPassword.Height + 2);
            btnVerPassword.FlatStyle = FlatStyle.Flat;
            btnVerPassword.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnVerPassword.BackColor = Color.White;
            btnVerPassword.Cursor = Cursors.Hand;
            btnVerPassword.TabStop = false;   
            btnVerPassword.Click += BtnVerPassword_Click;
            pnlTarjeta.Controls.Add(btnVerPassword);

            // Botón Iniciar sesión
            btnIniciar.Text = "Iniciar sesión";
            btnIniciar.Location = new Point(20, 310);
            btnIniciar.Size = new Size(190, 40);
            btnIniciar.BackColor = azul;
            btnIniciar.ForeColor = Color.White;
            btnIniciar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnIniciar.FlatStyle = FlatStyle.Flat;
            btnIniciar.FlatAppearance.BorderSize = 0;
            btnIniciar.Cursor = Cursors.Hand;
            btnIniciar.Click += BtnIniciar_Click;
            pnlTarjeta.Controls.Add(btnIniciar);

            // Botón Salir
            btnSalir.Text = "Salir";
            btnSalir.Location = new Point(220, 310);
            btnSalir.Size = new Size(190, 40);
            btnSalir.BackColor = Color.White;
            btnSalir.ForeColor = textoOscuro;
            btnSalir.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSalir.FlatStyle = FlatStyle.Flat;
            btnSalir.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnSalir.Cursor = Cursors.Hand;
            btnSalir.Click += BtnSalir_Click;
            pnlTarjeta.Controls.Add(btnSalir);

            //Botón cambiar pantalla
            btnCambiarPantalla.Text = "¿No tienes una cuenta? Haz click aquí";
            btnCambiarPantalla.Location = new Point(62, 375);
            btnCambiarPantalla.Size = new Size(300, 40);
            btnCambiarPantalla.BackColor = Color.White;
            btnCambiarPantalla.ForeColor = azul;
            btnCambiarPantalla.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnCambiarPantalla.FlatStyle = FlatStyle.Flat;
            btnCambiarPantalla.FlatAppearance.BorderSize = 0;
            btnCambiarPantalla.Cursor = Cursors.Hand;
            btnCambiarPantalla.Click += BtnCambiarPantalla_Click;
            pnlTarjeta.Controls.Add(btnCambiarPantalla);


            pnlCentro.Size = new Size(430, 459);
            pnlCentro.BackColor = Color.Transparent;
            pnlCentro.Controls.Add(pnlTarjeta);

            pnlDerecho.Controls.Add(pnlCentro);
            Controls.Add(pnlDerecho);
        }

        private Label CrearLabel(string texto, int x, int y, int ancho, int alto,
                                 float tamano, FontStyle estilo, Color color)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                AutoSize = false,
                Font = new Font("Segoe UI", tamano, estilo),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private void CentrarContenido()
        {
            pnlCentro.Left = (pnlDerecho.ClientSize.Width - pnlCentro.Width) / 2;
            pnlCentro.Top = Math.Max(10, (pnlDerecho.ClientSize.Height - pnlCentro.Height) / 2);
        }

        //  EVENTOS DE LOS BOTONES

        // Mostrar/ocultar la contraseña
        private void BtnVerPassword_Click(object? sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            txtPassword.Focus();
        }

        // Botón Iniciar sesión
        private void BtnIniciar_Click(object? sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

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
                MessageBox.Show(resultado.Mensaje, "No se pudo iniciar sesión",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtPassword.Clear();
                if (string.IsNullOrWhiteSpace(usuario)) txtUsuario.Focus();
                else txtPassword.Focus();
                return;
            }

            //MessageBox.Show(
            //    resultado.Mensaje + "\nRol: " + resultado.Usuario!.NombreRol,
            //    "Acceso concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //txtPassword.Clear();

            FrmDashboard dashboard = new FrmDashboard();
            dashboard.Show();
            this.Hide();
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
    }
}