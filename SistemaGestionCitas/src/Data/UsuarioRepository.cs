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
    }
}