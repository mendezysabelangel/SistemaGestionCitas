using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmClientes : Form
    {
        private readonly TextBox buscarId = ControlesRegistro.Texto(9);
        private readonly ComboBox buscarEstado = ControlesRegistro.Opciones("Todos", "Activo", "Inactivo");
        private readonly VistaRegistros<Cliente> vista = new("Clientes", "Consulta y gestión de clientes",
            "Buscar por nombre o apellido", "cliente", c => c.IdCliente, "Generar reporte");

        public FrmClientes()
        {
            InitializeComponent();
            SuspendLayout();
            vista.Tabla.RowTemplate.Height = 43;
            vista.Tabla.AgregarColumna("Cliente", 48, 110);
            vista.Tabla.AgregarColumna("Teléfono", 31, 94);
            vista.Tabla.AgregarColumna("Estado", 21, 75);
            vista.Tabla.CellFormatting += (_, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 2 && vista.Registro(e.RowIndex) is { } c)
                    e.CellStyle!.ForeColor = c.Activo ? vista.Paleta.TextoActivo : vista.Paleta.Secundario;
            };
            vista.ValorCelda = (c, indice) => indice switch
            {
                0 => c.NombreCompleto,
                1 => c.Telefono,
                2 => c.Estado,
                _ => null
            };
            vista.MostrarSeleccion = MostrarFicha;
            vista.AgregarFiltros(("ID de cliente", buscarId), ("Estado", buscarEstado));
            vista.Consultar += () => Consultar();
            vista.Restablecer += () =>
            {
                buscarId.Clear();
                vista.Busqueda.Clear();
                buscarEstado.SelectedIndex = 0;
                Consultar();
            };
            vista.Nuevo += () => AbrirEditor(null);
            vista.Editar += AbrirEditor;
            vista.Eliminar += async cliente => await vista.EliminarConfirmadoAsync(cliente.NombreCompleto,
                () => new ClienteService().Eliminar(cliente.IdCliente), () => Consultar(sinSeleccion: true));
            vista.AccionExtra!.Click += BtnReporte_Click;
            Controls.Add(vista);
            vista.ActualizarPermisos();
            Shown += (_, _) =>
            {
                vista.Tabla.RowTemplate.Height = LogicalToDeviceUnits(43);
                Consultar();
            };
            AutoScaleDimensions = new SizeF(96F, 96F);
            ResumeLayout(true);
        }

        private void Consultar(int? seleccionado = null, bool sinSeleccion = false)
        {
            if (!vista.LeerId(buscarId, out int? id)) return;
            string texto = vista.Busqueda.Text.Trim();
            bool? activo = ControlesRegistro.Estado(buscarEstado.SelectedIndex);
            vista.Cargar(() => (new ClienteService().Buscar(id, texto, activo), null),
                id.HasValue || texto.Length > 0 || activo.HasValue, seleccionado, sinSeleccion);
        }

        private void MostrarFicha(Cliente? cliente)
        {
            if (cliente == null)
            {
                vista.PresentarFicha("Selecciona un cliente para ver sus datos");
                return;
            }

            vista.PresentarFicha(cliente.NombreCompleto,
                ("Teléfono", ControlesRegistro.Disponible(cliente.Telefono)),
                ("Correo", ControlesRegistro.Disponible(cliente.Correo)),
                ("Dirección", ControlesRegistro.Disponible(cliente.Direccion)),
                ("Documento", ControlesRegistro.Disponible(cliente.Documento)),
                ("Fecha de nacimiento", cliente.FechaNacimiento?.ToString("dd/MM/yyyy") ?? "No registrado"),
                ("Fecha de registro", cliente.FechaRegistro.ToString("dd/MM/yyyy")),
                ("Estado", cliente.Estado));
        }

        private void AbrirEditor(Cliente? cliente)
        {
            string permiso = cliente == null ? PermisosSistema.Agregar : PermisosSistema.Modificar;
            if (!SesionActual.VerificarPermiso(permiso, cliente == null ? "registrar clientes" : "modificar clientes"))
                return;

            using var editor = new EditorRegistro(cliente == null ? "Nuevo cliente" : "Editar cliente", permiso, cliente != null);
            var nombres = ControlesRegistro.Texto(80, cliente?.Nombres ?? "");
            var apellidos = ControlesRegistro.Texto(80, cliente?.Apellidos ?? "");
            var telefono = ControlesRegistro.Texto(20, cliente?.Telefono ?? "");
            var correo = ControlesRegistro.Texto(100, cliente?.Correo ?? "");
            var direccion = ControlesRegistro.Texto(250, cliente?.Direccion ?? "");
            var documento = ControlesRegistro.Texto(30, cliente?.Documento ?? "");
            var fecha = new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = true,
                Value = cliente?.FechaNacimiento ?? DateTime.Today, Checked = cliente?.FechaNacimiento.HasValue == true };
            var activo = new CheckBox { Text = "Cliente activo", Checked = cliente?.Activo ?? true, AutoSize = true };
            editor.AgregarCampo("Nombres", nombres);
            editor.AgregarCampo("Apellidos", apellidos);
            editor.AgregarCampo("Teléfono", telefono);
            editor.AgregarCampo("Correo (opcional)", correo);
            editor.AgregarCampo("Dirección", direccion);
            editor.AgregarCampo("Documento (opcional)", documento);
            editor.AgregarCampo("Fecha de nacimiento (opcional)", fecha);
            editor.AgregarCampo("Estado", activo);

            editor.PrepararGuardado = () =>
            {
                var datos = new Cliente
                {
                    IdCliente = cliente?.IdCliente ?? 0, Nombres = nombres.Text, Apellidos = apellidos.Text,
                    Telefono = telefono.Text, Correo = correo.Text, Direccion = direccion.Text, Documento = documento.Text,
                    FechaNacimiento = fecha.Checked ? fecha.Value.Date : null, Activo = activo.Checked
                };
                return cliente == null ? () => new ClienteService().Registrar(datos) : () => new ClienteService().Modificar(datos);
            };
            vista.Paleta.AplicarDialogo(editor);
            if (editor.ShowDialog(this) == DialogResult.OK) Consultar(cliente?.IdCliente, cliente == null);
        }

        private void BtnReporte_Click(object? sender, EventArgs e)
        {
            if (!SesionActual.VerificarPermiso(PermisosSistema.Consultar, "consultar información")) return;
            using FrmFiltroReporteClientes ventanaFiltros = new();
            if (ventanaFiltros.ShowDialog(this) != DialogResult.OK || ventanaFiltros.Filtro == null) return;

            FrmReporteClientes reporte = new(ventanaFiltros.Filtro);
            if (MdiParent is FrmDashboard dashboard)
                dashboard.AbrirHijo(reporte, "Reporte de clientes", true);
            else
                reporte.Show(this);
        }
    }
}
