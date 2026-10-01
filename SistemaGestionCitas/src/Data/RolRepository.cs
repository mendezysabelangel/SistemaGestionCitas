using Microsoft.Data.SqlClient;

namespace SistemaGestionCitas.src.Data
{
    public class RolRepository
    {
        private readonly Conexion conexion = new Conexion();

        public bool ExisteRol(string nombre)
        {
            string sql =
                @"SELECT COUNT(1)
                  FROM Roles
                  WHERE Nombre = @nombre";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@nombre", nombre);

            con.Open();

            int cantidad = Convert.ToInt32(cmd.ExecuteScalar());

            return cantidad > 0;
        }

        public void CrearRol(
            string nombre,
            string descripcion,
            List<string> permisos)
        {
            using SqlConnection con = conexion.CrearConexion();

            con.Open();

            using SqlTransaction transaccion = con.BeginTransaction();

            try
            {
                int idRol;

                // Crear el rol
                string sqlRol =
                    @"INSERT INTO Roles
                        (Nombre, Descripcion)
                      OUTPUT INSERTED.IdRol
                      VALUES
                        (@nombre, @descripcion);";

                using (SqlCommand cmdRol =
                    new SqlCommand(sqlRol, con, transaccion))
                {
                    cmdRol.Parameters.AddWithValue(
                        "@nombre",
                        nombre);

                    cmdRol.Parameters.AddWithValue(
                        "@descripcion",
                        string.IsNullOrWhiteSpace(descripcion)
                            ? (object)DBNull.Value
                            : descripcion);

                    idRol = Convert.ToInt32(
                        cmdRol.ExecuteScalar());
                }

                // Asignar los permisos seleccionados
                foreach (string permiso in permisos)
                {
                    string sqlPermiso =
                        @"INSERT INTO RolPermiso
                            (IdRol, IdPermiso)
                          SELECT
                            @idRol,
                            IdPermiso
                          FROM Permisos
                          WHERE Nombre = @nombrePermiso;";

                    using SqlCommand cmdPermiso =
                        new SqlCommand(
                            sqlPermiso,
                            con,
                            transaccion);

                    cmdPermiso.Parameters.AddWithValue(
                        "@idRol",
                        idRol);

                    cmdPermiso.Parameters.AddWithValue(
                        "@nombrePermiso",
                        permiso);

                    cmdPermiso.ExecuteNonQuery();
                }

                transaccion.Commit();
            }
            catch
            {
                transaccion.Rollback();
                throw;
            }
        }
    }
}