using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services
{
    public class AuthService
    {
        private const int MaxIntentos = 3;   
        private const int MinutosBloqueo = 5;

        private readonly UsuarioRepository repositorio = new UsuarioRepository();
        private readonly PermisoRepository permisoRepositorio = new PermisoRepository();   

        public ResultadoLogin IniciarSesion(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password))
            {
                return Fallo("Debe ingresar el usuario y la contraseña.");
            }

            nombreUsuario = nombreUsuario.Trim();

            Usuario? usuario = repositorio.ObtenerPorNombreUsuario(nombreUsuario);

            if (usuario == null)
            {
                return Fallo("Usuario o contraseña incorrectos.");
            }

            if (!usuario.Activo)
            {
                return Fallo("Este usuario está desactivado. Contacte al administrador.");
            }

            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta.Value > DateTime.Now)
            {
                TimeSpan restante = usuario.BloqueadoHasta.Value - DateTime.Now;
                int minutos = (int)Math.Ceiling(restante.TotalMinutes);
                return Fallo($"Usuario bloqueado. Intente de nuevo en {minutos} minuto(s).");
            }

            bool passwordCorrecta = SeguridadHelper.VerificarPassword(password, usuario.PasswordHash);

            if (!passwordCorrecta)
            {
                int intentos = usuario.IntentosFallidos + 1;

                if (intentos >= MaxIntentos)
                {
                    DateTime bloqueadoHasta = DateTime.Now.AddMinutes(MinutosBloqueo);
                    repositorio.ActualizarIntentos(usuario.IdUsuario, 0, bloqueadoHasta);
                    return Fallo($"Demasiados intentos fallidos. Usuario bloqueado por {MinutosBloqueo} minutos.");
                }

                repositorio.ActualizarIntentos(usuario.IdUsuario, intentos, null);
                int restantes = MaxIntentos - intentos;
                return Fallo($"Usuario o contraseña incorrectos. Le quedan {restantes} intento(s).");
            }

            repositorio.ReiniciarIntentos(usuario.IdUsuario);

            List<string> permisos = permisoRepositorio.ObtenerPermisosPorRol(usuario.IdRol);

            usuario.PasswordHash = "";

            SesionActual.Iniciar(usuario, permisos);

            return new ResultadoLogin
            {
                Exitoso = true,
                Mensaje = "Bienvenido, " + usuario.NombreCompleto,
                Usuario = usuario
            };
        }

        private ResultadoLogin Fallo(string mensaje)
        {
            return new ResultadoLogin { Exitoso = false, Mensaje = mensaje };
        }
    }
}