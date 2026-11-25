using System.Data;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data;

internal sealed class ServicioRepository
{
    private readonly Conexion conexion = new();

    internal List<Servicio> Buscar(string? texto, bool? activo)
    {
        const string sql = """
            SELECT IdServicio, Nombre, Descripcion, DuracionMinutos, Precio, Activo
            FROM Servicios
            WHERE (@texto IS NULL OR CHARINDEX(@texto, Nombre) > 0)
              AND (@activo IS NULL OR Activo = @activo)
            ORDER BY Nombre, IdServicio
            """;
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@texto", SqlDbType.NVarChar, -1).Value = (object?)texto ?? DBNull.Value;
        cmd.Parameters.Add("@activo", SqlDbType.Bit).Value = (object?)activo ?? DBNull.Value;

        con.Open();
        using SqlDataReader lector = cmd.ExecuteReader();
        var servicios = new List<Servicio>();
        while (lector.Read())
        {
            servicios.Add(new Servicio
            {
                IdServicio = lector.GetInt32(0),
                Nombre = lector.GetString(1),
                Descripcion = lector.IsDBNull(2) ? null : lector.GetString(2),
                DuracionMinutos = lector.GetInt32(3),
                Precio = lector.GetDecimal(4),
                Activo = lector.GetBoolean(5)
            });
        }
        return servicios;
    }

    internal void Insertar(Servicio servicio)
    {
        const string sql = """
            INSERT INTO Servicios (Nombre, Descripcion, DuracionMinutos, Precio)
            VALUES (@nombre, @descripcion, @duracion, @precio)
            """;
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand(sql, con);
        AgregarDatos(cmd, servicio);
        con.Open();
        cmd.ExecuteNonQuery();
    }

    internal bool Actualizar(Servicio servicio)
    {
        const string sql = """
            UPDATE Servicios
            SET Nombre = @nombre, Descripcion = @descripcion, DuracionMinutos = @duracion, Precio = @precio, Activo = @activo
            WHERE IdServicio = @id
            """;
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand(sql, con);
        AgregarDatos(cmd, servicio);
        cmd.Parameters.Add("@activo", SqlDbType.Bit).Value = servicio.Activo;
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = servicio.IdServicio;
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    internal bool Eliminar(int id)
    {
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand("DELETE FROM Servicios WHERE IdServicio = @id", con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AgregarDatos(SqlCommand cmd, Servicio servicio)
    {
        cmd.Parameters.Add("@nombre", SqlDbType.VarChar, Servicio.MaxNombre).Value = servicio.Nombre;
        cmd.Parameters.Add("@descripcion", SqlDbType.VarChar, Servicio.MaxDescripcion).Value = (object?)servicio.Descripcion ?? DBNull.Value;
        cmd.Parameters.Add("@duracion", SqlDbType.Int).Value = servicio.DuracionMinutos;
        SqlParameter precio = cmd.Parameters.Add("@precio", SqlDbType.Decimal);
        precio.Precision = 10;
        precio.Scale = Servicio.DecimalesPrecio;
        precio.Value = servicio.Precio;
    }
}
