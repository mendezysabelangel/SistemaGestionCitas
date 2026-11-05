namespace SistemaGestionCitas.src.Models
{
    public class FiltroProducto
    {
        public int? IdProducto { get; set; }       // búsqueda por ID
        public string? Texto { get; set; }         // búsqueda por nombre o código
        public int? IdCategoria { get; set; }      // filtro por categoría
        public bool? Activo { get; set; }          // filtro por estado
        public int? StockMinimo { get; set; }      // filtro por nivel de stock (desde)
        public int? StockMaximo { get; set; }      // filtro por nivel de stock (hasta)
    }
}