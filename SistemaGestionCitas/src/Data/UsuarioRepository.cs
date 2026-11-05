using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data
{
    public class UsuarioRepository
    {
        private readonly Conexion conexion = new Conexion();

        public List<Usuario> Listar()
        {
            List<Usuario> usuarios = new List<Usuario>();
            string sql = @"SELECT U.IdUsuario, U.NombreUsuario, U.NombreCompleto,
                                  U.Correo, U.IdRol, R.Nombre AS NombreRol, U.Activo
                           FROM Usuarios U
                           INNER JOIN Roles R ON U.IdRol = R.IdRol
                           ORDER BY U.IdUsuario";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            con.Open();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(new Usuario
                {
                    IdUsuario = (int)reader["IdUsuario"],
                    NombreUsuario = (string)reader["NombreUsuario"],
                    NombreCompleto = (string)reader["NombreCompleto"],
                    Correo = reader["Correo"] == DBNull.Value ? null : (string)reader["Correo"],
                    IdRol = (int)reader["IdRol"],
                    NombreRol = (string)reader["NombreRol"],
                    Activo = (bool)reader["Activo"]
                });
            }

            return usuarios;
        }

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

        public Usuario? ObtenerPorId(int idUsuario)
        {
            string sql = @"SELECT U.IdUsuario, U.NombreUsuario, U.NombreCompleto,
                                  U.Correo, U.IdRol, R.Nombre AS NombreRol, U.Activo
                           FROM Usuarios U
                           INNER JOIN Roles R ON U.IdRol = R.IdRol
                           WHERE U.IdUsuario = @idUsuario";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Usuario
            {
                IdUsuario = (int)reader["IdUsuario"],
                NombreUsuario = (string)reader["NombreUsuario"],
                NombreCompleto = (string)reader["NombreCompleto"],
                Correo = reader["Correo"] == DBNull.Value ? null : (string)reader["Correo"],
                IdRol = (int)reader["IdRol"],
                NombreRol = (string)reader["NombreRol"],
                Activo = (bool)reader["Activo"]
            };
        }

        public bool ExisteNombreUsuario(string nombreUsuario, int? excluirId = null)
        {
            string sql = @"SELECT COUNT(*) FROM Usuarios
                           WHERE NombreUsuario = @nombreUsuario AND IdUsuario <> @excluirId";

            using (SqlConnection con = conexion.CrearConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                cmd.Parameters.AddWithValue("@excluirId", excluirId ?? 0);
                con.Open();

                int cantidad = (int)cmd.ExecuteScalar();
                return cantidad > 0;
            }
        }

        public bool CrearUsuarioConRol(string nombreUsuario, string passwordHash,
                                       string nombreCompleto, string? correo, string nombreRol, bool activo = true)
        {
            string sql = @"INSERT INTO Usuarios (NombreUsuario, PasswordHash, NombreCompleto, Correo, IdRol, Activo)
                           SELECT @nombreUsuario, @passwordHash, @nombreCompleto, @correo, IdRol, @activo
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
                cmd.Parameters.AddWithValue("@activo", activo);

                con.Open();

                int filas = cmd.ExecuteNonQuery();
                return filas > 0;
            }
        }

        public bool Actualizar(int idUsuario, string nombreCompleto, string nombreUsuario,
                               string? correo, string nombreRol, bool activo)
        {
            string sql = @"UPDATE U
                           SET NombreCompleto = @nombreCompleto, NombreUsuario = @nombreUsuario,
                               Correo = @correo, IdRol = R.IdRol, Activo = @activo
                           FROM Usuarios U
                           INNER JOIN Roles R ON R.Nombre = @nombreRol AND R.Activo = 1
                           WHERE U.IdUsuario = @idUsuario";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@nombreCompleto", nombreCompleto);
            cmd.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
            cmd.Parameters.AddWithValue("@correo", (object?)correo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@nombreRol", nombreRol);
            cmd.Parameters.AddWithValue("@activo", activo);
            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool CambiarPassword(int idUsuario, string passwordHash)
        {
            string sql = @"UPDATE Usuarios
                           SET PasswordHash = @passwordHash, IntentosFallidos = 0, BloqueadoHasta = NULL
                           WHERE IdUsuario = @idUsuario";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
            cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Eliminar(int idUsuario)
        {
            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand("DELETE FROM Usuarios WHERE IdUsuario = @idUsuario", con);
            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
