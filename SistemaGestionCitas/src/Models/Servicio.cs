namespace SistemaGestionCitas.src.Models;

public class Servicio
{
    internal const int MaxNombre = 100;
    internal const int MaxDescripcion = 250;
    internal const decimal PrecioMaximo = 99_999_999.99m;
    internal const byte DecimalesPrecio = 2;

    public int IdServicio { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public int DuracionMinutos { get; set; }
    public decimal Precio { get; set; }
    public bool Activo { get; set; } = true;

    public string Estado => Activo ? "Activo" : "Inactivo";
}
