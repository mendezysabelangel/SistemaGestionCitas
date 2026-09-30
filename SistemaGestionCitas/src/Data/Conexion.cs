using Microsoft.Data.SqlClient;

namespace SistemaGestionCitas.src.Data
{
    public class Conexion
    {
       
        private readonly string cadenaConexion =
            "Server=localhost; Database=SistemaGestionCitasDB; Trusted_Connection=True; TrustServerCertificate=True;";

        public SqlConnection CrearConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}