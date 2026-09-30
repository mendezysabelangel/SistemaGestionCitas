using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services
{

    public static class SesionActual
    {
        public static Usuario? UsuarioActual { get; private set; }
        public static List<string> Permisos { get; private set; } = new List<string>();

        public static bool HaySesion => UsuarioActual != null;

        public static void Iniciar(Usuario usuario, List<string> permisos)
        {
            UsuarioActual = usuario;
            Permisos = permisos;
        }

        public static void CerrarSesion()
        {
            UsuarioActual = null;
            Permisos = new List<string>();
        }

        public static bool TienePermiso(string permiso)
        {
            return Permisos.Contains(permiso);
        }

        public static bool VerificarPermiso(string permiso, string accion)
        {
            if (TienePermiso(permiso))
                return true;

            string rol = UsuarioActual?.NombreRol ?? "Sin sesión";
            MessageBox.Show(
                $"Permisos insuficientes.\n\nEl rol \"{rol}\" no puede {accion}.",
                "Acceso denegado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }
    }
}