namespace SistemaGestionCitas.src.Models
{
    // Representa una fila de la tabla Productos
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; } = "";
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }          // decimal: el tipo correcto para dinero
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; } = ""; // nombre de la categoría (viene del JOIN)
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; }

        // Propiedad calculada: la tabla muestra "Activo/Inactivo" en vez de True/False
        public string Estado => Activo ? "Activo" : "Inactivo";
    }
}