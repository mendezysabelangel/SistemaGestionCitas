using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository repositorio = new UsuarioRepository();

        public List<Usuario> ListarUsuarios()
        {
            if (!PuedeAdministrar())
                throw new UnauthorizedAccessException("Permisos insuficientes para consultar usuarios.");

            return repositorio.Listar();
        }

        public ResultadoOperacion RegistrarUsuario(string nombreCompleto, string nombreUsuario,
            string correo, string password, string nombreRol, bool activo = true)
        {
            if (!PuedeAdministrar())
                return Error("Permisos insuficientes para crear usuarios.");

            nombreCompleto = nombreCompleto.Trim();
            nombreUsuario = nombreUsuario.Trim();
            correo = correo.Trim();
            nombreRol = nombreRol.Trim();

            ResultadoOperacion validacion = ValidarDatos(nombreCompleto, nombreUsuario, correo, nombreRol, true);
            if (!validacion.Exitoso) return validacion;
            validacion = ValidarPassword(password);
            if (!validacion.Exitoso) return validacion;

            if (repositorio.ExisteNombreUsuario(nombreUsuario))
                return Error("Ese nombre de usuario ya está en uso. Elija otro.");

            try
            {
                if (!repositorio.CrearUsuarioConRol(nombreUsuario, SeguridadHelper.GenerarHash(password),
                    nombreCompleto, correo, nombreRol, activo))
                    return Error("El rol seleccionado no existe o está desactivado.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Error("Ese nombre de usuario ya está en uso. Elija otro.");
            }

            return Ok("Usuario creado correctamente.");
        }

        public ResultadoOperacion ModificarUsuario(int idUsuario, string nombreCompleto,
            string nombreUsuario, string? correo, string nombreRol, bool activo)
        {
            if (!PuedeAdministrar(PermisosSistema.Modificar))
                return Error("Permisos insuficientes para modificar usuarios.");
            if (idUsuario <= 0)
                return Error("Seleccione un usuario para modificar.");

            nombreCompleto = nombreCompleto.Trim();
            nombreUsuario = nombreUsuario.Trim();
            correo = string.IsNullOrWhiteSpace(correo) ? null : correo.Trim();
            nombreRol = nombreRol.Trim();
            ResultadoOperacion validacion = ValidarDatos(nombreCompleto, nombreUsuario, correo, nombreRol, false);
            if (!validacion.Exitoso) return validacion;

            Usuario? anterior = repositorio.ObtenerPorId(idUsuario);
            if (anterior == null)
                return Error("El usuario ya no existe en la base de datos.");

            bool esCuentaActual = SesionActual.UsuarioActual!.IdUsuario == idUsuario;
            if (esCuentaActual && !activo)
                return Error("No puede desactivar la cuenta con la que tiene la sesión abierta.");
            if (esCuentaActual && !string.Equals(anterior.NombreRol, nombreRol, StringComparison.OrdinalIgnoreCase))
                return Error("No puede cambiar el rol de la cuenta con la que tiene la sesión abierta.");

            if (repositorio.ExisteNombreUsuario(nombreUsuario, idUsuario))
                return Error("Otro usuario ya utiliza ese nombre de usuario.");

            try
            {
                if (!repositorio.Actualizar(idUsuario, nombreCompleto, nombreUsuario, correo, nombreRol, activo))
                    return Error("No se pudo modificar: el usuario ya no existe o el rol está desactivado.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Error("Otro usuario ya utiliza ese nombre de usuario.");
            }

            if (esCuentaActual)
            {
                SesionActual.UsuarioActual!.NombreCompleto = nombreCompleto;
                SesionActual.UsuarioActual.NombreUsuario = nombreUsuario;
                SesionActual.UsuarioActual.Correo = correo;
            }

            return Ok("Usuario modificado correctamente.");
        }

        public ResultadoOperacion CambiarPassword(int idUsuario, string password, string confirmarPassword)
        {
            if (!PuedeAdministrar(PermisosSistema.Modificar))
                return Error("Permisos insuficientes para cambiar contraseñas.");
            if (idUsuario <= 0)
                return Error("Seleccione un usuario para cambiar su contraseña.");

            ResultadoOperacion validacion = ValidarPassword(password);
            if (!validacion.Exitoso) return validacion;
            if (password != confirmarPassword)
                return Error("Las contraseñas no coinciden.");

            if (!repositorio.CambiarPassword(idUsuario, SeguridadHelper.GenerarHash(password)))
                return Error("El usuario ya no existe en la base de datos.");

            return Ok("Contraseña actualizada correctamente.");
        }

        public ResultadoOperacion EliminarUsuario(int idUsuario)
        {
            if (!PuedeAdministrar(PermisosSistema.Eliminar))
                return Error("Permisos insuficientes para eliminar usuarios.");
            if (idUsuario <= 0)
                return Error("Seleccione un usuario para eliminar.");
            if (SesionActual.UsuarioActual!.IdUsuario == idUsuario)
                return Error("No puede eliminar la cuenta con la que tiene la sesión abierta.");

            try
            {
                if (!repositorio.Eliminar(idUsuario))
                    return Error("El usuario ya no existe en la base de datos.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error("No se puede eliminar: el usuario tiene citas asociadas.\n" +
                    "Puede desactivarlo desde la opción Modificar.");
            }

            return Ok("Usuario eliminado correctamente.");
        }

        private bool PuedeAdministrar(string? permisoAdicional = null)
        {
            return SesionActual.HaySesion && SesionActual.TienePermiso(PermisosSistema.CrearUsuarios) &&
                (permisoAdicional == null || SesionActual.TienePermiso(permisoAdicional));
        }

        private ResultadoOperacion ValidarDatos(string nombreCompleto, string nombreUsuario,
            string? correo, string nombreRol, bool correoObligatorio)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Length > 100)
                return Error("El nombre completo es obligatorio y no puede superar 100 caracteres.");
            if (!Regex.IsMatch(nombreUsuario, @"\A[a-zA-Z0-9._]{3,50}\z"))
                return Error("El nombre de usuario debe tener entre 3 y 50 caracteres " +
                    "y solo puede contener letras, números, punto o guion bajo.");
            if (string.IsNullOrEmpty(correo))
            {
                if (correoObligatorio) return Error("Ingrese un correo electrónico válido.");
            }
            else if (correo.Length > 100 || !CorreoValido(correo))
            {
                return Error("Ingrese un correo electrónico válido.");
            }
            if (string.IsNullOrWhiteSpace(nombreRol))
                return Error("Debe seleccionar un rol para el usuario.");

            return Ok("");
        }

        private ResultadoOperacion ValidarPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || password.Length > 100)
                return Error("La contraseña debe tener entre 6 y 100 caracteres y no puede estar vacía.");
            return Ok("");
        }

        private bool CorreoValido(string correo)
        {
            try
            {
                return new MailAddress(correo).Address == correo;
            }
            catch
            {
                return false;
            }
        }

        private ResultadoOperacion Ok(string mensaje) =>
            new ResultadoOperacion { Exitoso = true, Mensaje = mensaje };

        private ResultadoOperacion Error(string mensaje) =>
            new ResultadoOperacion { Exitoso = false, Mensaje = mensaje };
    }
}
