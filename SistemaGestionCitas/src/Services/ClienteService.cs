using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository repositorio = new ClienteRepository();

        public List<Cliente> Buscar(int? id, string? texto, bool? activo,
                                    DateTime? desde = null, DateTime? hasta = null)
        {
            return repositorio.Buscar(id, texto, activo, desde, hasta);
        }

        public ResultadoOperacion Registrar(Cliente cliente)
        {
            // Además del botón, el servicio también revisa el permiso (doble protección)
            if (!SesionActual.TienePermiso(PermisosSistema.Agregar))
                return Error("Permisos insuficientes para registrar clientes.");

            ResultadoOperacion validacion = Validar(cliente);
            if (!validacion.Exitoso) return validacion;

            if (cliente.Documento != null && repositorio.ExisteDocumento(cliente.Documento, null))
                return Error("Ya existe un cliente con ese documento.");

            repositorio.Insertar(cliente);
            return Ok("Cliente registrado correctamente.");
        }

        public ResultadoOperacion Modificar(Cliente cliente)
        {
            if (!SesionActual.TienePermiso(PermisosSistema.Modificar))
                return Error("Permisos insuficientes para modificar clientes.");

            if (cliente.IdCliente <= 0)
                return Error("Seleccione un cliente para modificar.");

            ResultadoOperacion validacion = Validar(cliente);
            if (!validacion.Exitoso) return validacion;

            if (cliente.Documento != null && repositorio.ExisteDocumento(cliente.Documento, cliente.IdCliente))
                return Error("Otro cliente ya tiene ese documento.");

            if (!repositorio.Actualizar(cliente))
                return Error("El cliente ya no existe en la base de datos.");

            return Ok("Cliente modificado correctamente.");
        }

        public ResultadoOperacion Eliminar(int idCliente)
        {
            if (!SesionActual.TienePermiso(PermisosSistema.Eliminar))
                return Error("Permisos insuficientes para eliminar clientes.");

            try
            {
                if (!repositorio.Eliminar(idCliente))
                    return Error("El cliente ya no existe en la base de datos.");
            }
            // 547 = error de SQL Server por llave foránea: el cliente tiene citas asociadas
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error("No se puede eliminar: el cliente tiene citas asociadas.\n" +
                             "Puede marcarlo como inactivo desde Modificar.");
            }

            return Ok("Cliente eliminado correctamente.");
        }

        // Valida y limpia los datos (Trim, y los vacíos opcionales pasan a null)
        private ResultadoOperacion Validar(Cliente c)
        {
            c.Nombres = c.Nombres.Trim();
            c.Apellidos = c.Apellidos.Trim();
            c.Telefono = c.Telefono.Trim();
            c.Correo = string.IsNullOrWhiteSpace(c.Correo) ? null : c.Correo.Trim();
            c.Documento = string.IsNullOrWhiteSpace(c.Documento) ? null : c.Documento.Trim();

            if (c.Nombres == "" || c.Nombres.Length > 80)
                return Error("Los nombres son obligatorios (máximo 80 caracteres).");

            if (c.Apellidos == "" || c.Apellidos.Length > 80)
                return Error("Los apellidos son obligatorios (máximo 80 caracteres).");

            if (!Regex.IsMatch(c.Telefono, @"^[0-9+()\-\s]{7,20}$"))
                return Error("El teléfono debe tener entre 7 y 20 caracteres (números, +, -, paréntesis).");

            if (c.Correo != null && (c.Correo.Length > 100 || !CorreoValido(c.Correo)))
                return Error("El correo electrónico no es válido.");

            if (c.Documento != null && c.Documento.Length > 30)
                return Error("El documento no puede superar 30 caracteres.");

            if (c.FechaNacimiento.HasValue &&
                (c.FechaNacimiento.Value.Date > DateTime.Today || c.FechaNacimiento.Value.Year < 1900))
                return Error("La fecha de nacimiento no es válida.");

            return Ok("");
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

        private ResultadoOperacion Ok(string mensaje)
        {
            return new ResultadoOperacion { Exitoso = true, Mensaje = mensaje };
        }

        private ResultadoOperacion Error(string mensaje)
        {
            return new ResultadoOperacion { Exitoso = false, Mensaje = mensaje };
        }
    }
}