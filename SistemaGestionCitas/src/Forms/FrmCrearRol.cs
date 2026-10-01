using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public partial class FrmCrearRol : Form
    {
        private readonly RolRepository rolRepository = new RolRepository();

        public FrmCrearRol()
        {
            InitializeComponent();
        }

        private void btnCrearRol_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreRol.Text.Trim();
            string descripcion = txtDescripcionRol.Text.Trim();

            // Validar permisos del usuario actual
            if (!SesionActual.VerificarPermiso(
                    PermisosSistema.CrearRoles,
                    "crear nuevos roles"))
            {
                return;
            }

            // Validar nombre
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre del rol.",
                    "Campo requerido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNombreRol.Focus();
                return;
            }

            // Obtener permisos seleccionados
            List<string> permisos = ObtenerPermisosSeleccionados();

            if (permisos.Count == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar al menos un permiso para el rol.",
                    "Permisos requeridos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                // Verificar si ya existe
                if (rolRepository.ExisteRol(nombre))
                {
                    MessageBox.Show(
                        "Ya existe un rol con ese nombre.",
                        "Rol existente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtNombreRol.Focus();
                    return;
                }

                // Crear rol y asignar permisos
                rolRepository.CrearRol(nombre, descripcion, permisos);

                MessageBox.Show(
                    $"El rol \"{nombre}\" fue creado correctamente.",
                    "Rol creado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo crear el rol.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private List<string> ObtenerPermisosSeleccionados()
        {
            List<string> permisos = new List<string>();

            if (chkConsultar.Checked)
                permisos.Add(PermisosSistema.Consultar);

            if (chkAgregar.Checked)
                permisos.Add(PermisosSistema.Agregar);

            if (chkModificar.Checked)
                permisos.Add(PermisosSistema.Modificar);

            if (chkEliminar.Checked)
                permisos.Add(PermisosSistema.Eliminar);

            if (chkCrearUsuarios.Checked)
                permisos.Add(PermisosSistema.CrearUsuarios);

            if (chkCrearRoles.Checked)
                permisos.Add(PermisosSistema.CrearRoles);

            return permisos;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}