using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Models
{
    public enum NivelExistencias
    {
        Todos = 0,
        SinExistencias = 1,
        StockBajo = 2,
        Disponible = 3
    }

    public class FiltroReporteInventario
    {
        public string? Texto { get; set; }
        public int? IdCategoria { get; set; }
        public string? NombreCategoria { get; set; }
        public bool? Activo { get; set; }
        public NivelExistencias NivelExistencias { get; set; } = NivelExistencias.Todos;

        public bool TieneFiltros =>
            !string.IsNullOrWhiteSpace(Texto)
            || IdCategoria.HasValue
            || Activo.HasValue
            || NivelExistencias != NivelExistencias.Todos;

        public FiltroProducto AFiltroProducto()
        {
            return new FiltroProducto
            {
                Texto = string.IsNullOrWhiteSpace(Texto) ? null : Texto.Trim(),
                IdCategoria = IdCategoria,
                Activo = Activo,
                StockMinimo = NivelExistencias switch
                {
                    NivelExistencias.SinExistencias => 0,
                    NivelExistencias.StockBajo => 1,
                    NivelExistencias.Disponible => ProductoService.LimiteStockBajo + 1,
                    _ => null
                },
                StockMaximo = NivelExistencias switch
                {
                    NivelExistencias.SinExistencias => 0,
                    NivelExistencias.StockBajo => ProductoService.LimiteStockBajo,
                    _ => null
                }
            };
        }

        public string Descripcion()
        {
            var partes = new List<string>();

            if (!string.IsNullOrWhiteSpace(Texto))
                partes.Add($"Búsqueda: \"{Texto.Trim()}\"");

            if (IdCategoria.HasValue)
                partes.Add($"Categoría: {(string.IsNullOrWhiteSpace(NombreCategoria) ? $"#{IdCategoria.Value}" : NombreCategoria)}");

            if (Activo.HasValue)
                partes.Add($"Estado: {(Activo.Value ? "Activo" : "Inactivo")}");

            if (NivelExistencias != NivelExistencias.Todos)
                partes.Add($"Existencias: {EtiquetaNivel(NivelExistencias)}");

            return partes.Count == 0
                ? "Filtros: Todos los productos"
                : "Filtros: " + string.Join("  ·  ", partes);
        }

        public static string EtiquetaNivel(NivelExistencias nivel) => nivel switch
        {
            NivelExistencias.SinExistencias => "Sin existencias (0)",
            NivelExistencias.StockBajo => $"Stock bajo (1–{ProductoService.LimiteStockBajo})",
            NivelExistencias.Disponible => $"Disponible (> {ProductoService.LimiteStockBajo})",
            _ => "Todos"
        };
    }

    public class ItemReporteInventario
    {
        public int IdProducto { get; }
        public string Codigo { get; }
        public string Nombre { get; }
        public int IdCategoria { get; }
        public string Categoria { get; }
        public decimal Precio { get; }
        public int Stock { get; }
        public bool Activo { get; }
        public string Estado { get; }
        public DateTime FechaRegistro { get; }
        public decimal ValorInventario { get; }
        public NivelExistencias NivelExistencias { get; }

        public bool EsSinExistencias => NivelExistencias == NivelExistencias.SinExistencias;
        public bool EsStockBajo => NivelExistencias == NivelExistencias.StockBajo;

        public string NivelExistenciasTexto => NivelExistencias switch
        {
            NivelExistencias.SinExistencias => "Sin existencias",
            NivelExistencias.StockBajo => "Stock bajo",
            _ => "Disponible"
        };

        public ItemReporteInventario(Producto producto)
        {
            ArgumentNullException.ThrowIfNull(producto);

            IdProducto = producto.IdProducto;
            Codigo = producto.Codigo;
            Nombre = producto.Nombre;
            IdCategoria = producto.IdCategoria;
            Categoria = producto.Categoria;
            Precio = producto.Precio;
            Stock = producto.Stock;
            Activo = producto.Activo;
            Estado = producto.Estado;
            FechaRegistro = producto.FechaRegistro;
            ValorInventario = producto.Precio * producto.Stock;
            NivelExistencias = ReporteInventarioService.ClasificarStock(producto.Stock);
        }
    }

    public sealed record ResumenInventario(
        int CantidadProductos,
        int ProductosSinExistencias,
        int ProductosStockBajo,
        int ProductosDisponibles,
        long UnidadesTotales,
        decimal ValorInventario)
    {
        public static ResumenInventario Vacio { get; } = new(0, 0, 0, 0, 0, 0m);
    }

    public sealed record ResultadoReporteInventario(
        IReadOnlyList<ItemReporteInventario> Items,
        ResumenInventario Resumen,
        FiltroReporteInventario Filtro,
        DateTime FechaGeneracion);
}
