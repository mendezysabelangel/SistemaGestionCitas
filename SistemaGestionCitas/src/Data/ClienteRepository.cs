using System.Text;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data
{
    public class ClienteRepository
    {
        private readonly Conexion conexion = new Conexion();

        private const string Columnas =
            "IdCliente, Nombres, Apellidos, Telefono, Correo, Documento, FechaNacimiento, Activo, FechaRegistro";

        // CONSULTA DINÁMICA: cada filtro que sea null se ignora.
        public List<Cliente> Buscar(int? idCliente, string? texto, bool? activo,
                                    DateTime? desde = null, DateTime? hasta = null)
        {
            List<Cliente> lista = new List<Cliente>();

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            // "WHERE 1 = 1" siempre es verdadero; permite agregar cada condición con AND
            StringBuilder sql = new StringBuilder("SELECT " + Columnas + " FROM Clientes WHERE 1 = 1");

            if (idCliente.HasValue)
            {
                sql.Append(" AND IdCliente = @idCliente");
                cmd.Parameters.AddWithValue("@idCliente", idCliente.Value);
            }

            if (!string.IsNullOrWhiteSpace(texto))
            {
                // Busca en nombres, en apellidos y en el nombre completo
                sql.Append(" AND (Nombres LIKE @texto OR Apellidos LIKE @texto" +
                           " OR (Nombres + ' ' + Apellidos) LIKE @texto)");
                cmd.Parameters.AddWithValue("@texto", "%" + texto.Trim() + "%");
            }

            if (activo.HasValue)
            {
                sql.Append(" AND Activo = @activo");
                cmd.Parameters.AddWithValue("@activo", activo.Value);
            }

            if (desde.HasValue)
            {
                sql.Append(" AND FechaRegistro >= @desde");
                cmd.Parameters.AddWithValue("@desde", desde.Value.Date);
            }

            if (hasta.HasValue)
            {
                // "< día siguiente" incluye todo el día "hasta"
                sql.Append(" AND FechaRegistro < @hasta");
                cmd.Parameters.AddWithValue("@hasta", hasta.Value.Date.AddDays(1));
            }

            sql.Append(" ORDER BY IdCliente");
            cmd.CommandText = sql.ToString();

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(Mapear(reader));
            }

            return lista;
        }

        // Inserta un cliente y devuelve el Id que le asignó la base de datos
        public int Insertar(Cliente c)
        {
            string sql = @"INSERT INTO Clientes
                              (Nombres, Apellidos, Telefono, Correo, Documento, FechaNacimiento, Activo)
                           OUTPUT INSERTED.IdCliente
                           VALUES
                              (@nombres, @apellidos, @telefono, @correo, @documento, @fechaNacimiento, @activo)";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            AgregarParametros(cmd, c);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Devuelve true si se actualizó una fila
        public bool Actualizar(Cliente c)
        {
            string sql = @"UPDATE Clientes
                           SET Nombres = @nombres, Apellidos = @apellidos, Telefono = @telefono,
                               Correo = @correo, Documento = @documento,
                               FechaNacimiento = @fechaNacimiento, Activo = @activo
                           WHERE IdCliente = @idCliente";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            AgregarParametros(cmd, c);
            cmd.Parameters.AddWithValue("@idCliente", c.IdCliente);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Devuelve true si se borró una fila
        public bool Eliminar(int idCliente)
        {
            string sql = "DELETE FROM Clientes WHERE IdCliente = @idCliente";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idCliente", idCliente);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // ¿Otro cliente ya tiene este documento? (excluirId = el cliente que se está editando)
        public bool ExisteDocumento(string documento, int? excluirId)
        {
            string sql = "SELECT COUNT(1) FROM Clientes WHERE Documento = @documento AND IdCliente <> @excluir";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@documento", documento);
            cmd.Parameters.AddWithValue("@excluir", excluirId ?? 0);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        // ----- Métodos auxiliares (evitan repetir código) -----

        private void AgregarParametros(SqlCommand cmd, Cliente c)
        {
            cmd.Parameters.AddWithValue("@nombres", c.Nombres);
            cmd.Parameters.AddWithValue("@apellidos", c.Apellidos);
            cmd.Parameters.AddWithValue("@telefono", c.Telefono);
            // Para guardar NULL en la BD se envía DBNull.Value
            cmd.Parameters.AddWithValue("@correo", (object?)c.Correo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@documento", (object?)c.Documento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fechaNacimiento", (object?)c.FechaNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@activo", c.Activo);
        }

        // Convierte la fila actual del reader en un objeto Cliente
        private Cliente Mapear(SqlDataReader r)
        {
            return new Cliente
            {
                IdCliente = (int)r["IdCliente"],
                Nombres = (string)r["Nombres"],
                Apellidos = (string)r["Apellidos"],
                Telefono = (string)r["Telefono"],
                Correo = r["Correo"] == DBNull.Value ? null : (string)r["Correo"],
                Documento = r["Documento"] == DBNull.Value ? null : (string)r["Documento"],
                FechaNacimiento = r["FechaNacimiento"] == DBNull.Value ? null : (DateTime)r["FechaNacimiento"],
                Activo = (bool)r["Activo"],
                FechaRegistro = (DateTime)r["FechaRegistro"]
            };
        }
    }
}