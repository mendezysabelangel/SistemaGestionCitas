namespace SistemaGestionCitas.src.Models;

public class Empleado
{
    internal const int MaxNombres = 80;
    internal const int MaxApellidos = 80;
    internal const int MaxTelefono = 20;
    internal const int MaxCorreo = 100;

    public int IdEmpleado { get; set; }
    public string Nombres { get; set; } = "";
    public string Apellidos { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string? Correo { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; }

    public string NombreCompleto => $"{Nombres} {Apellidos}";
    public string Estado => Activo ? "Activo" : "Inactivo";
}
