using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services
{
    public class UsuarioService
    {
        private const string RolPorDefecto = "Ejecutor";

        private readonly UsuarioRepository repositorio = new UsuarioRepository();

        public ResultadoOperacion RegistrarUsuarioEjecutor(string nombreCompleto, string nombreUsuario,
                                                           string correo, string password)
        {
            nombreCompleto = nombreCompleto.Trim();
            nombreUsuario = nombreUsuario.Trim();
            correo = correo.Trim();

            if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Length > 100)
                return Error("El nombre completo es obligatorio y no puede superar 100 caracteres.");

            if (!Regex.IsMatch(nombreUsuario, @"^[a-zA-Z0-9._]{3,50}$"))
                return Error("El nombre de usuario debe tener entre 3 y 50 caracteres " +
                             "y solo puede contener letras, números, punto o guion bajo.");

            if (correo.Length > 100 || !CorreoValido(correo))
                return Error("Ingrese un correo electrónico válido.");

            if (string.IsNullOrEmpty(password) || password.Length < 6 || password.Length > 100)
                return Error("La contraseña debe tener entre 6 y 100 caracteres.");

            if (repositorio.ExisteNombreUsuario(nombreUsuario))
                return Error("Ese nombre de usuario ya está en uso. Elija otro.");

            string hash = SeguridadHelper.GenerarHash(password);

            try
            {
                bool creado = repositorio.CrearUsuarioConRol(
                    nombreUsuario, hash, nombreCompleto, correo, RolPorDefecto);

                if (!creado)
                    return Error($"No se encontró el rol \"{RolPorDefecto}\". Contacte al administrador.");
            }

            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Error("Ese nombre de usuario ya está en uso. Elija otro.");
            }

            return new ResultadoOperacion
            {
                Exitoso = true,
                Mensaje = "Usuario creado correctamente."
            };
        }

        private bool CorreoValido(string correo)
        {
            try
            {
                MailAddress direccion = new MailAddress(correo);
                return direccion.Address == correo;
            }
            catch
            {
                return false;
            }
        }

        private ResultadoOperacion Error(string mensaje)
        {
            return new ResultadoOperacion { Exitoso = false, Mensaje = mensaje };
        }
    }
}