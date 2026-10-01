using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data
{
    public class UsuarioRepository
    {
        private readonly Conexion conexion = new Conexion();

        public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
        {
            string sql = @"SELECT U.IdUsuario, U.NombreUsuario, U.PasswordHash, U.NombreCompleto,
                                  U.Correo, U.IdRol, R.Nombre AS NombreRol, U.Activo,
                                  U.IntentosFallidos, U.BloqueadoHasta
                           FROM Usuarios U
                           INNER JOIN Roles R ON U.IdRol = R.IdRol
                           WHERE U.NombreUsuario = @nombreUsuario";

            using (SqlConnection con = conexion.CrearConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new Usuario
                    {
                        IdUsuario = (int)reader["IdUsuario"],
                        NombreUsuario = (string)reader["NombreUsuario"],
                        PasswordHash = (string)reader["PasswordHash"],
                        NombreCompleto = (string)reader["NombreCompleto"],
                        Correo = reader["Correo"] == DBNull.Value ? null : (string)reader["Correo"],
                        IdRol = (int)reader["IdRol"],
                        NombreRol = (string)reader["NombreRol"],
                        Activo = (bool)reader["Activo"],
                        IntentosFallidos = (int)reader["IntentosFallidos"],
                        BloqueadoHasta = reader["BloqueadoHasta"] == DBNull.Value
                            ? null
                            : (DateTime)reader["BloqueadoHasta"]
                    };
                }
            }
        }

        public void ActualizarIntentos(int idUsuario, int intentosFallidos, DateTime? bloqueadoHasta)
        {
            string sql = @"UPDATE Usuarios
                           SET IntentosFallidos = @intentos,
                               BloqueadoHasta = @bloqueadoHasta
                           WHERE IdUsuario = @idUsuario";

            using (SqlConnection con = conexion.CrearConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@intentos", intentosFallidos);
                cmd.Parameters.AddWithValue("@bloqueadoHasta", (object?)bloqueadoHasta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ReiniciarIntentos(int idUsuario)
        {
            ActualizarIntentos(idUsuario, 0, null);
        }

        public bool ExisteNombreUsuario(string nombreUsuario)
        {
            string sql = "SELECT COUNT(*) FROM Usuarios WHERE NombreUsuario = @nombreUsuario";

            using (SqlConnection con = conexion.CrearConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                con.Open();

                int cantidad = (int)cmd.ExecuteScalar();
                return cantidad > 0;
            }
        }

        public bool CrearUsuarioConRol(string nombreUsuario, string passwordHash,
                                       string nombreCompleto, string? correo, string nombreRol)
        {
            string sql = @"INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, Correo, IdRol)
                           SELECT @nombreUsuario, @passwordHash, @nombreCompleto, @correo, IdRol
                           FROM Roles
                           WHERE Nombre = @nombreRol AND Activo = 1";

            using (SqlConnection con = conexion.CrearConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                cmd.Parameters.AddWithValue("@nombreCompleto", nombreCompleto);
                cmd.Parameters.AddWithValue("@correo", (object?)correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@nombreRol", nombreRol);

                con.Open();

                int filas = cmd.ExecuteNonQuery();
                return filas > 0;
            }
        }
    }
}