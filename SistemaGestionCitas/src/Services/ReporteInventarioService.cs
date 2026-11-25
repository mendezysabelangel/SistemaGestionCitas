using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services
{
    public class ReporteInventarioService
    {
        public static readonly string[] EncabezadosExportacion =
        {
            "ID",
            "Código",
            "Producto",
            "Categoría",
            "Precio",
            "Existencias",
            "Valor inventario",
            "Nivel de existencias",
            "Estado",
            "Fecha de registro"
        };

        private readonly Func<FiltroProducto, List<Producto>> buscarProductos;
        private readonly Func<List<CategoriaProducto>> obtenerCategorias;

        public ReporteInventarioService()
            : this(new ProductoService())
        {
        }

        public ReporteInventarioService(ProductoService productoService)
        {
            ArgumentNullException.ThrowIfNull(productoService);
            buscarProductos = productoService.Buscar;
            obtenerCategorias = productoService.ObtenerCategorias;
        }

        internal ReporteInventarioService(
            Func<FiltroProducto, List<Producto>> buscarProductos,
            Func<List<CategoriaProducto>> obtenerCategorias)
        {
            this.buscarProductos = buscarProductos ?? throw new ArgumentNullException(nameof(buscarProductos));
            this.obtenerCategorias = obtenerCategorias ?? throw new ArgumentNullException(nameof(obtenerCategorias));
        }

        public static bool TienePermisoConsulta() =>
            SesionActual.HaySesion && SesionActual.TienePermiso(PermisosSistema.Consultar);

        public static NivelExistencias ClasificarStock(int stock) => stock switch
        {
            <= 0 => NivelExistencias.SinExistencias,
            <= ProductoService.LimiteStockBajo => NivelExistencias.StockBajo,
            _ => NivelExistencias.Disponible
        };

        public static ResumenInventario CalcularResumen(IReadOnlyList<ItemReporteInventario> items)
        {
            if (items == null || items.Count == 0)
                return ResumenInventario.Vacio;

            int sinExistencias = 0;
            int stockBajo = 0;
            int disponibles = 0;
            long unidadesTotales = 0;
            decimal valorInventario = 0m;

            foreach (ItemReporteInventario item in items)
            {
                switch (item.NivelExistencias)
                {
                    case NivelExistencias.SinExistencias:
                        sinExistencias++;
                        break;
                    case NivelExistencias.StockBajo:
                        stockBajo++;
                        break;
                    default:
                        disponibles++;
                        break;
                }

                unidadesTotales += item.Stock;
                valorInventario += item.ValorInventario;
            }

            return new ResumenInventario(
                items.Count,
                sinExistencias,
                stockBajo,
                disponibles,
                unidadesTotales,
                valorInventario);
        }

        public List<CategoriaProducto> ObtenerCategorias()
        {
            ValidarPermisoConsulta();
            return obtenerCategorias();
        }

        public ResultadoReporteInventario Consultar(FiltroReporteInventario? filtro = null)
        {
            ValidarPermisoConsulta();

            FiltroReporteInventario filtroAplicado = filtro ?? new FiltroReporteInventario();
            List<Producto> productos = buscarProductos(filtroAplicado.AFiltroProducto());
            List<ItemReporteInventario> items = productos.Select(p => new ItemReporteInventario(p)).ToList();
            ResumenInventario resumen = CalcularResumen(items);

            return new ResultadoReporteInventario(items, resumen, filtroAplicado, DateTime.Now);
        }

        public void ExportarCsv(ResultadoReporteInventario reporte, string rutaArchivo)
        {
            ValidarPermisoConsulta();
            ValidarExportacion(reporte, rutaArchivo);

            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", EncabezadosExportacion.Select(EscaparCsv)));

            foreach (ItemReporteInventario item in reporte.Items)
            {
                string[] fila =
                {
                    item.IdProducto.ToString(CultureInfo.InvariantCulture),
                    item.Codigo,
                    item.Nombre,
                    item.Categoria,
                    item.Precio.ToString("0.00", CultureInfo.InvariantCulture),
                    item.Stock.ToString(CultureInfo.InvariantCulture),
                    item.ValorInventario.ToString("0.00", CultureInfo.InvariantCulture),
                    item.NivelExistenciasTexto,
                    item.Estado,
                    item.FechaRegistro.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                };

                sb.AppendLine(string.Join(",", fila.Select(EscaparCsv)));
            }

            // UTF-8 con BOM para que Excel interprete correctamente tildes y caracteres especiales.
            File.WriteAllText(rutaArchivo, sb.ToString(), new UTF8Encoding(true));
        }

        public void ExportarExcel(ResultadoReporteInventario reporte, string rutaArchivo, string? usuario = null)
        {
            ValidarPermisoConsulta();
            ValidarExportacion(reporte, rutaArchivo);

            string nombreUsuario = string.IsNullOrWhiteSpace(usuario)
                ? SesionActual.UsuarioActual?.NombreCompleto ?? "-"
                : usuario.Trim();

            using var libro = new XLWorkbook();
            IXLWorksheet hoja = libro.Worksheets.Add("Inventario");

            const string formatoMoneda = "\"RD$ \"#,##0.00";
            const string formatoEntero = "#,##0";

            // Encabezado del reporte
            hoja.Cell(1, 1).Value = "Reporte de Inventario de Productos";
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontSize = 16;
            hoja.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#082D50");

            hoja.Cell(2, 1).Value = reporte.Filtro.Descripcion();
            hoja.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml("#485E77");

            hoja.Cell(3, 1).Value =
                $"Generado: {reporte.FechaGeneracion:dd/MM/yyyy HH:mm}  ·  Usuario: {nombreUsuario}";
            hoja.Cell(3, 1).Style.Font.FontColor = XLColor.FromHtml("#485E77");

            // Resumen ejecutivo
            string[] etiquetasResumen =
            {
                "Cantidad de productos",
                "Sin existencias (0)",
                $"Stock bajo (1–{ProductoService.LimiteStockBajo})",
                "Unidades totales",
                "Valor del inventario"
            };

            for (int i = 0; i < etiquetasResumen.Length; i++)
                hoja.Cell(5, i + 1).Value = etiquetasResumen[i];

            IXLRange rangoEtiquetasResumen = hoja.Range(5, 1, 5, etiquetasResumen.Length);
            rangoEtiquetasResumen.Style.Font.Bold = true;
            rangoEtiquetasResumen.Style.Font.FontColor = XLColor.FromHtml("#082D50");
            rangoEtiquetasResumen.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F1FC");
            rangoEtiquetasResumen.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangoEtiquetasResumen.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rangoEtiquetasResumen.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D0DEED");
            rangoEtiquetasResumen.Style.Border.InsideBorderColor = XLColor.FromHtml("#D0DEED");

            hoja.Cell(6, 1).Value = reporte.Resumen.CantidadProductos;
            hoja.Cell(6, 1).Style.NumberFormat.Format = formatoEntero;

            hoja.Cell(6, 2).Value = reporte.Resumen.ProductosSinExistencias;
            hoja.Cell(6, 2).Style.NumberFormat.Format = formatoEntero;
            if (reporte.Resumen.ProductosSinExistencias > 0)
            {
                hoja.Cell(6, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#FCE5E8");
                hoja.Cell(6, 2).Style.Font.FontColor = XLColor.FromHtml("#882B39");
            }

            hoja.Cell(6, 3).Value = reporte.Resumen.ProductosStockBajo;
            hoja.Cell(6, 3).Style.NumberFormat.Format = formatoEntero;
            if (reporte.Resumen.ProductosStockBajo > 0)
            {
                hoja.Cell(6, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#FDEFD3");
                hoja.Cell(6, 3).Style.Font.FontColor = XLColor.FromHtml("#6F4411");
            }

            hoja.Cell(6, 4).Value = reporte.Resumen.UnidadesTotales;
            hoja.Cell(6, 4).Style.NumberFormat.Format = formatoEntero;

            hoja.Cell(6, 5).Value = reporte.Resumen.ValorInventario;
            hoja.Cell(6, 5).Style.NumberFormat.Format = formatoMoneda;
            hoja.Cell(6, 5).Style.Font.FontColor = XLColor.FromHtml("#007077");

            IXLRange rangoValoresResumen = hoja.Range(6, 1, 6, etiquetasResumen.Length);
            rangoValoresResumen.Style.Font.Bold = true;
            rangoValoresResumen.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangoValoresResumen.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rangoValoresResumen.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D0DEED");
            rangoValoresResumen.Style.Border.InsideBorderColor = XLColor.FromHtml("#D0DEED");

            // Tabla de productos filtrados
            const int filaEncabezado = 8;
            int totalColumnas = EncabezadosExportacion.Length;

            for (int i = 0; i < totalColumnas; i++)
                hoja.Cell(filaEncabezado, i + 1).Value = EncabezadosExportacion[i];

            IXLRange rangoEncabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, totalColumnas);
            rangoEncabezado.Style.Font.Bold = true;
            rangoEncabezado.Style.Font.FontColor = XLColor.White;
            rangoEncabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#082D50");
            rangoEncabezado.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int fila = filaEncabezado + 1;
            foreach (ItemReporteInventario item in reporte.Items)
            {
                hoja.Cell(fila, 1).Value = item.IdProducto;
                hoja.Cell(fila, 2).Value = item.Codigo;
                hoja.Cell(fila, 3).Value = item.Nombre;
                hoja.Cell(fila, 4).Value = item.Categoria;

                hoja.Cell(fila, 5).Value = item.Precio;
                hoja.Cell(fila, 5).Style.NumberFormat.Format = formatoMoneda;

                hoja.Cell(fila, 6).Value = item.Stock;
                hoja.Cell(fila, 6).Style.NumberFormat.Format = formatoEntero;

                hoja.Cell(fila, 7).Value = item.ValorInventario;
                hoja.Cell(fila, 7).Style.NumberFormat.Format = formatoMoneda;

                hoja.Cell(fila, 8).Value = item.NivelExistenciasTexto;
                hoja.Cell(fila, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                hoja.Cell(fila, 9).Value = item.Estado;
                hoja.Cell(fila, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                hoja.Cell(fila, 10).Value = item.FechaRegistro;
                hoja.Cell(fila, 10).Style.DateFormat.Format = "dd/MM/yyyy";
                hoja.Cell(fila, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                IXLCell celdaNivel = hoja.Cell(fila, 8);
                if (item.EsSinExistencias)
                {
                    hoja.Range(fila, 1, fila, totalColumnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#FEF6F7");
                    celdaNivel.Style.Fill.BackgroundColor = XLColor.FromHtml("#FCE5E8");
                    celdaNivel.Style.Font.FontColor = XLColor.FromHtml("#882B39");
                    celdaNivel.Style.Font.Bold = true;
                }
                else if (item.EsStockBajo)
                {
                    hoja.Range(fila, 1, fila, totalColumnas).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFFBF2");
                    celdaNivel.Style.Fill.BackgroundColor = XLColor.FromHtml("#FDEFD3");
                    celdaNivel.Style.Font.FontColor = XLColor.FromHtml("#6F4411");
                    celdaNivel.Style.Font.Bold = true;
                }
                else
                {
                    celdaNivel.Style.Fill.BackgroundColor = XLColor.FromHtml("#DDF2EA");
                    celdaNivel.Style.Font.FontColor = XLColor.FromHtml("#005243");
                }

                fila++;
            }

            int ultimaFilaDatos = fila - 1;
            hoja.Range(filaEncabezado, 1, ultimaFilaDatos, totalColumnas).SetAutoFilter();

            // Fila de totales
            hoja.Cell(fila, 1).Value = "Total";
            hoja.Cell(fila, 3).Value = reporte.Resumen.CantidadProductos == 1
                ? "1 producto"
                : $"{reporte.Resumen.CantidadProductos:N0} productos";
            hoja.Cell(fila, 6).Value = reporte.Resumen.UnidadesTotales;
            hoja.Cell(fila, 6).Style.NumberFormat.Format = formatoEntero;
            hoja.Cell(fila, 7).Value = reporte.Resumen.ValorInventario;
            hoja.Cell(fila, 7).Style.NumberFormat.Format = formatoMoneda;

            IXLRange rangoTotales = hoja.Range(fila, 1, fila, totalColumnas);
            rangoTotales.Style.Font.Bold = true;
            rangoTotales.Style.Font.FontColor = XLColor.FromHtml("#082D50");
            rangoTotales.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F1FC");
            rangoTotales.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            rangoTotales.Style.Border.TopBorderColor = XLColor.FromHtml("#082D50");

            for (int col = 1; col <= totalColumnas; col++)
            {
                hoja.Column(col).AdjustToContents(5, fila);
                hoja.Column(col).Width = Math.Max(hoja.Column(col).Width + 2, 10);
            }

            libro.SaveAs(rutaArchivo);
        }

        public static string MensajeErrorConsulta(Exception ex)
        {
            if (ex is UnauthorizedAccessException)
                return "No tiene permiso para consultar el reporte de inventario.";

            if (ex is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    -2 => "Tiempo de espera agotado al consultar la base de datos. Intente nuevamente.",
                    -1 or 2 or 53 or 40 or 10053 or 10054 or 10060 or 10061 =>
                        "No se pudo conectar con SQL Server. Verifique que el servidor esté disponible e intente nuevamente.",
                    4060 or 18456 =>
                        "No se pudo acceder a la base de datos del sistema. Verifique las credenciales y permisos de conexión.",
                    _ => "Ocurrió un error al consultar el inventario en la base de datos. Intente nuevamente."
                };
            }

            return "No se pudo consultar el inventario. Verifique la conexión e intente nuevamente.";
        }

        private static void ValidarPermisoConsulta()
        {
            if (!TienePermisoConsulta())
                throw new UnauthorizedAccessException("Permisos insuficientes para consultar el reporte de inventario.");
        }

        private static void ValidarExportacion(ResultadoReporteInventario reporte, string rutaArchivo)
        {
            ArgumentNullException.ThrowIfNull(reporte);

            if (reporte.Items.Count == 0)
                throw new InvalidOperationException("No hay productos en el reporte para exportar.");

            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException("Debe indicar una ruta válida para guardar el archivo.", nameof(rutaArchivo));
        }

        private static string EscaparCsv(string valor)
        {
            if (valor.Contains(',') || valor.Contains('"') || valor.Contains('\n') || valor.Contains('\r'))
                return "\"" + valor.Replace("\"", "\"\"") + "\"";

            return valor;
        }
    }
}
