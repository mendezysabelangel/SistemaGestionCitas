namespace SistemaGestionCitas.src.Models
{
    // Representa una fila de la tabla Clientes
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombres { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string? Correo { get; set; }
        public string? Documento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; }

        // Propiedades calculadas: no existen en la BD, se arman con otros datos.
        // La tabla de pantalla mostrará "Activo/Inactivo" en vez de True/False.
        public string NombreCompleto => Nombres + " " + Apellidos;
        public string Estado => Activo ? "Activo" : "Inactivo";
    }
}