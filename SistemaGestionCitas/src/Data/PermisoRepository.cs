using Microsoft.Data.SqlClient;

namespace SistemaGestionCitas.src.Data
{
    public class PermisoRepository
    {
        private readonly Conexion conexion = new Conexion();

        public List<string> ObtenerPermisosPorRol(int idRol)
        {
            List<string> permisos = new List<string>();

            string sql = @"SELECT P.Nombre
                           FROM RolPermiso RP
                           INNER JOIN Permisos P ON RP.IdPermiso = P.IdPermiso
                           WHERE RP.IdRol = @idRol AND P.Activo = 1
                           ORDER BY P.Nombre";

            using (SqlConnection con = conexion.CrearConexion())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@idRol", idRol);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        permisos.Add((string)reader["Nombre"]);
                    }
                }
            }

            return permisos;
        }
    }
}