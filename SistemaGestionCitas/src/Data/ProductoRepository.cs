using System.Text;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data
{
    public class ProductoRepository
    {
        private readonly Conexion conexion = new Conexion();

        // CONSULTA DINÁMICA: cada filtro que sea null se ignora.
        public List<Producto> Buscar(FiltroProducto filtro)
        {
            List<Producto> lista = new List<Producto>();

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            // "WHERE 1 = 1" siempre es verdadero; permite agregar cada condición con AND
            StringBuilder sql = new StringBuilder(
                @"SELECT P.IdProducto, P.Codigo, P.Nombre, P.Precio, P.Stock,
                         P.IdCategoria, C.Nombre AS Categoria, P.Activo, P.FechaRegistro
                  FROM Productos P
                  INNER JOIN CategoriasProducto C ON P.IdCategoria = C.IdCategoria
                  WHERE 1 = 1");

            if (filtro.IdProducto.HasValue)
            {
                sql.Append(" AND P.IdProducto = @idProducto");
                cmd.Parameters.AddWithValue("@idProducto", filtro.IdProducto.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                // Busca el texto dentro del nombre o del código
                sql.Append(" AND (P.Nombre LIKE @texto OR P.Codigo LIKE @texto)");
                cmd.Parameters.AddWithValue("@texto", "%" + filtro.Texto.Trim() + "%");
            }

            if (filtro.IdCategoria.HasValue)
            {
                sql.Append(" AND P.IdCategoria = @idCategoria");
                cmd.Parameters.AddWithValue("@idCategoria", filtro.IdCategoria.Value);
            }

            if (filtro.Activo.HasValue)
            {
                sql.Append(" AND P.Activo = @activo");
                cmd.Parameters.AddWithValue("@activo", filtro.Activo.Value);
            }

            if (filtro.StockMinimo.HasValue)
            {
                sql.Append(" AND P.Stock >= @stockMinimo");
                cmd.Parameters.AddWithValue("@stockMinimo", filtro.StockMinimo.Value);
            }

            if (filtro.StockMaximo.HasValue)
            {
                sql.Append(" AND P.Stock <= @stockMaximo");
                cmd.Parameters.AddWithValue("@stockMaximo", filtro.StockMaximo.Value);
            }

            sql.Append(" ORDER BY P.IdProducto");
            cmd.CommandText = sql.ToString();

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(Mapear(reader));
            }

            return lista;
        }

        // Categorías activas, para llenar el ComboBox
        public List<CategoriaProducto> ObtenerCategorias()
        {
            List<CategoriaProducto> lista = new List<CategoriaProducto>();

            string sql = "SELECT IdCategoria, Nombre FROM CategoriasProducto WHERE Activo = 1 ORDER BY Nombre";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);

            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new CategoriaProducto
                {
                    IdCategoria = (int)reader["IdCategoria"],
                    Nombre = (string)reader["Nombre"]
                });
            }

            return lista;
        }

        // Inserta un producto y devuelve el Id que le asignó la base de datos
        public int Insertar(Producto p)
        {
            string sql = @"INSERT INTO Productos (Codigo, Nombre, Precio, Stock, IdCategoria, Activo)
                           OUTPUT INSERTED.IdProducto
                           VALUES (@codigo, @nombre, @precio, @stock, @idCategoria, @activo)";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            AgregarParametros(cmd, p);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // Devuelve true si se actualizó una fila
        public bool Actualizar(Producto p)
        {
            string sql = @"UPDATE Productos
                           SET Codigo = @codigo, Nombre = @nombre, Precio = @precio,
                               Stock = @stock, IdCategoria = @idCategoria, Activo = @activo
                           WHERE IdProducto = @idProducto";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            AgregarParametros(cmd, p);
            cmd.Parameters.AddWithValue("@idProducto", p.IdProducto);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // Devuelve true si se borró una fila
        public bool Eliminar(int idProducto)
        {
            string sql = "DELETE FROM Productos WHERE IdProducto = @idProducto";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idProducto", idProducto);

            con.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        // ¿Otro producto ya usa este código? (excluirId = el producto que se está editando)
        public bool ExisteCodigo(string codigo, int? excluirId)
        {
            string sql = "SELECT COUNT(1) FROM Productos WHERE Codigo = @codigo AND IdProducto <> @excluir";

            using SqlConnection con = conexion.CrearConexion();
            using SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@codigo", codigo);
            cmd.Parameters.AddWithValue("@excluir", excluirId ?? 0);

            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        // ----- Métodos auxiliares (evitan repetir código) -----

        private void AgregarParametros(SqlCommand cmd, Producto p)
        {
            cmd.Parameters.AddWithValue("@codigo", p.Codigo);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@precio", p.Precio);
            cmd.Parameters.AddWithValue("@stock", p.Stock);
            cmd.Parameters.AddWithValue("@idCategoria", p.IdCategoria);
            cmd.Parameters.AddWithValue("@activo", p.Activo);
        }

        // Convierte la fila actual del reader en un objeto Producto
        private Producto Mapear(SqlDataReader r)
        {
            return new Producto
            {
                IdProducto = (int)r["IdProducto"],
                Codigo = (string)r["Codigo"],
                Nombre = (string)r["Nombre"],
                Precio = (decimal)r["Precio"],
                Stock = (int)r["Stock"],
                IdCategoria = (int)r["IdCategoria"],
                Categoria = (string)r["Categoria"],
                Activo = (bool)r["Activo"],
                FechaRegistro = (DateTime)r["FechaRegistro"]
            };
        }
    }
}