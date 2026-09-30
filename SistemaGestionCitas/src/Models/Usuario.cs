namespace SistemaGestionCitas.src.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string? Correo { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; } = "";
        public bool Activo { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
    }
}