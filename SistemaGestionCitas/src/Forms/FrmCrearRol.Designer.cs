namespace SistemaGestionCitas.src.Forms
{
    partial class FrmCrearRol
    {
        private readonly TextBox txtNombreRol = new() { MaxLength = 50 };
        private readonly TextBox txtDescripcionRol = new() { MaxLength = 200, Multiline = true };
        private readonly CheckBox chkConsultar = Casilla("Consultar");
        private readonly CheckBox chkAgregar = Casilla("Agregar");
        private readonly CheckBox chkModificar = Casilla("Modificar");
        private readonly CheckBox chkEliminar = Casilla("Eliminar");
        private readonly CheckBox chkCrearUsuarios = Casilla("Crear usuarios");
        private readonly CheckBox chkCrearRoles = Casilla("Crear roles");
        private Button btnCrearRol = null!;

        private static CheckBox Casilla(string texto) => new()
        {
            Text = texto, AutoSize = true, MinimumSize = new Size(0, 29), Margin = new Padding(0, 1, 0, 0)
        };

        private void InitializeComponent()
        {
            Name = "FrmCrearRol";
            btnCrearRol = ControlesRegistro.Boton("Crear rol", ControlesRegistro.Primario, Color.White);
            var cancelar = ControlesRegistro.Boton("Cancelar");
            btnCrearRol.Click += btnCrearRol_Click;
            var contenido = DialogoCitorna.Tabla();
            contenido.Controls.Add(DialogoCitorna.Campos(1, ("Nombre del rol", txtNombreRol, 40),
                ("Descripción (opcional)", txtDescripcionRol, 64)));
            var etiqueta = CampoCitorna.Rotulo("Permisos del sistema");
            etiqueta.Margin = new Padding(0, 8, 0, 2);
            contenido.Controls.Add(etiqueta);
            var ayuda = CampoCitorna.Rotulo("Selecciona al menos un permiso.", true);
            ayuda.Margin = new Padding(0, 0, 0, 8);
            contenido.Controls.Add(ayuda);
            contenido.Controls.Add(DialogoCitorna.DisponerColumnas(2,
                Grupo("Operaciones generales", chkConsultar, chkAgregar, chkModificar, chkEliminar),
                Grupo("Administración de accesos", chkCrearUsuarios, chkCrearRoles)));
            DialogoCitorna.Configurar(this, "Crear rol", "Define un rol y asigna sus permisos.",
                new Size(665, 542), contenido, btnCrearRol, cancelar);
            if (contenido.Parent is ScrollableControl superficie)
            {
                // Esperar a que termine la distribución antes de medir los grupos apilados.
                void AjustarDesplazamiento()
                {
                    if (superficie.IsHandleCreated)
                        superficie.BeginInvoke(() => superficie.AutoScrollMinSize = new Size(0, contenido.Height));
                }
                contenido.SizeChanged += (_, _) => AjustarDesplazamiento();
                Shown += (_, _) => AjustarDesplazamiento();
            }
        }

        private static Control Grupo(string nombre, params CheckBox[] casillas)
        {
            var grupo = new GrupoPermisos { Text = nombre, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12, 8, 8, 8), Margin = new Padding(0, 0, 9, 6), ForeColor = EstiloAgenda.Tinta };
            var lista = DialogoCitorna.Tabla();
            foreach (var casilla in casillas) lista.Controls.Add(casilla);
            grupo.Controls.Add(lista);
            return grupo;
        }
    }
}
