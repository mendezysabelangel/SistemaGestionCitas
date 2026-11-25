using System.Data;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Data;

internal sealed class EmpleadoRepository
{
    private readonly Conexion conexion = new();

    internal List<Empleado> Buscar(string? texto, bool? activo)
    {
        const string sql = """
            SELECT IdEmpleado, Nombres, Apellidos, Telefono, Correo, Activo, FechaRegistro
            FROM Empleados
            WHERE (@texto IS NULL OR CHARINDEX(@texto, Nombres + ' ' + Apellidos) > 0)
              AND (@activo IS NULL OR Activo = @activo)
            ORDER BY Nombres, Apellidos, IdEmpleado
            """;
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@texto", SqlDbType.NVarChar, -1).Value = (object?)texto ?? DBNull.Value;
        cmd.Parameters.Add("@activo", SqlDbType.Bit).Value = (object?)activo ?? DBNull.Value;

        con.Open();
        using SqlDataReader lector = cmd.ExecuteReader();
        var empleados = new List<Empleado>();
        while (lector.Read())
        {
            empleados.Add(new Empleado
            {
                IdEmpleado = lector.GetInt32(0),
                Nombres = lector.GetString(1),
                Apellidos = lector.GetString(2),
                Telefono = lector.GetString(3),
                Correo = lector.IsDBNull(4) ? null : lector.GetString(4),
                Activo = lector.GetBoolean(5),
                FechaRegistro = lector.GetDateTime(6)
            });
        }
        return empleados;
    }

    internal void Insertar(Empleado empleado)
    {
        const string sql = """
            INSERT INTO Empleados (Nombres, Apellidos, Telefono, Correo)
            VALUES (@nombres, @apellidos, @telefono, @correo)
            """;
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand(sql, con);
        AgregarDatos(cmd, empleado);
        con.Open();
        cmd.ExecuteNonQuery();
    }

    internal bool Actualizar(Empleado empleado)
    {
        const string sql = """
            UPDATE Empleados
            SET Nombres = @nombres, Apellidos = @apellidos, Telefono = @telefono, Correo = @correo, Activo = @activo
            WHERE IdEmpleado = @id
            """;
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand(sql, con);
        AgregarDatos(cmd, empleado);
        cmd.Parameters.Add("@activo", SqlDbType.Bit).Value = empleado.Activo;
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = empleado.IdEmpleado;
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    internal bool Eliminar(int id)
    {
        using SqlConnection con = conexion.CrearConexion();
        using var cmd = new SqlCommand("DELETE FROM Empleados WHERE IdEmpleado = @id", con);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
        con.Open();
        return cmd.ExecuteNonQuery() > 0;
    }

    private static void AgregarDatos(SqlCommand cmd, Empleado empleado)
    {
        cmd.Parameters.Add("@nombres", SqlDbType.VarChar, Empleado.MaxNombres).Value = empleado.Nombres;
        cmd.Parameters.Add("@apellidos", SqlDbType.VarChar, Empleado.MaxApellidos).Value = empleado.Apellidos;
        cmd.Parameters.Add("@telefono", SqlDbType.VarChar, Empleado.MaxTelefono).Value = empleado.Telefono;
        cmd.Parameters.Add("@correo", SqlDbType.VarChar, Empleado.MaxCorreo).Value = (object?)empleado.Correo ?? DBNull.Value;
    }
}
