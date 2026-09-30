using Microsoft.Data.SqlClient;

namespace SistemaGestionCitas.src.Data
{
    public class Conexion
    {
        private readonly string datosConexion;
        private SqlConnection conectar;

        public Conexion()
        {
            datosConexion = "server=localhost; database=SistemaGestionCitasDB; Trusted_Connection=True; TrustConnectionCertificate=True";
            conectar = new SqlConnection(datosConexion);
        }

        public SqlConnection AbrirConexion()
        {
            try
            {
                if(conectar.State == System.Data.ConnectionState.Closed)
                {
                    conectar.Open();
                    Console.WriteLine("Conexion abierta");
                }
            } catch (Exception e) {
                Console.WriteLine("Error al conectar: " + e);
            }
            return conectar;
        }

        public void CerrarConexion()
        {
            try
            {
                if (conectar.State == System.Data.ConnectionState.Open)
                {
                    conectar.Close();
                    Console.WriteLine("Conexion cerrada");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
            }
        }
    }
}