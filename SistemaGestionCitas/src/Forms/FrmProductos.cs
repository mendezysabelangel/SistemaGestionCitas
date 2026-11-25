using System.Drawing.Drawing2D;
using System.Globalization;
using SistemaGestionCitas.src.Models;
using SistemaGestionCitas.src.Services;

namespace SistemaGestionCitas.src.Forms
{
    public class FrmProductos : Form
    {
        private static readonly Color FondoIcono = Color.FromArgb(238, 243, 248);
        private readonly TextBox buscarId = ControlesRegistro.Texto(9);
        private readonly ComboBox buscarCategoria = ControlesRegistro.Opciones("Todas");
        private readonly ComboBox buscarEstado = ControlesRegistro.Opciones("Todos", "Activo", "Inactivo");
        private readonly ComboBox buscarStock = ControlesRegistro.Opciones("Todos", "Agotado", "Stock bajo", "Disponible");
        private readonly Font nombreProducto = new("Segoe UI", 11F, FontStyle.Bold);
        private readonly VistaRegistros<Producto> vista = new("Productos", "Catálogo y control de inventario",
            "Buscar por nombre o código", "producto", p => p.IdProducto);
        private List<CategoriaProducto>? categorias;

        public FrmProductos()
        {
            SuspendLayout();
            Text = "Productos";
            Name = "FrmProductos";
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoScaleDimensions = new SizeF(96F, 96F);
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(1040, 650);
            vista.Tabla.ColumnHeadersVisible = false;
            vista.Tabla.CellBorderStyle = DataGridViewCellBorderStyle.None;
            vista.Tabla.RowTemplate.Height = 67;
            vista.Tabla.AgregarColumna("Producto y categoría", 46, 145);
            vista.Tabla.AgregarColumna("Precio", 21, 82);
            vista.Tabla.AgregarColumna("Existencias", 13, 42);
            vista.Tabla.AgregarColumna("Inventario", 20, 82);
            vista.Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            vista.Tabla.CellPainting += PintarCatalogo;
            vista.ValorCelda = (p, indice) => indice switch
            {
                0 => p.Nombre + "\n" + p.Categoria,
                1 => Precio(p.Precio),
                2 => $"{p.Stock:N0} uds.",
                3 => EstadoInventario(p),
                _ => null
            };
            vista.MostrarSeleccion = MostrarFicha;
            vista.AgregarFiltros(("ID de producto", buscarId), ("Categoría", buscarCategoria),
                ("Estado", buscarEstado), ("Inventario", buscarStock));
            vista.Consultar += () => Consultar();
            vista.Restablecer += () =>
            {
                buscarId.Clear();
                vista.Busqueda.Clear();
                buscarCategoria.SelectedIndex = 0;
                buscarEstado.SelectedIndex = 0;
                buscarStock.SelectedIndex = 0;
                Consultar();
            };
            vista.Nuevo += () => AbrirEditor(null);
            vista.Editar += AbrirEditor;
            vista.Eliminar += async producto => await vista.EliminarConfirmadoAsync($"{producto.Nombre} ({producto.Codigo})",
                () => new ProductoService().Eliminar(producto.IdProducto), () => Consultar(sinSeleccion: true));
            Controls.Add(vista);
            vista.ActualizarPermisos();
            Shown += (_, _) =>
            {
                vista.Tabla.RowTemplate.Height = LogicalToDeviceUnits(67);
                Consultar();
            };
            ResumeLayout(true);
        }

        private void Consultar(int? seleccionado = null, bool sinSeleccion = false)
        {
            if (!vista.LeerId(buscarId, out int? id)) return;
            int? idCategoria = buscarCategoria.SelectedValue is int categoria && categoria > 0 ? categoria : null;
            int nivel = buscarStock.SelectedIndex;
            var filtro = new FiltroProducto
            {
                IdProducto = id, Texto = vista.Busqueda.Text.Trim(), IdCategoria = idCategoria,
                Activo = ControlesRegistro.Estado(buscarEstado.SelectedIndex),
                StockMinimo = nivel switch { 0 => null, 1 => 0, 2 => 1, _ => ProductoService.LimiteStockBajo + 1 },
                StockMaximo = nivel switch { 1 => 0, 2 => ProductoService.LimiteStockBajo, _ => null }
            };
            bool conFiltros = id.HasValue || idCategoria.HasValue || filtro.Activo.HasValue || nivel > 0
                || !string.IsNullOrEmpty(filtro.Texto);
            bool cargarCategorias = categorias == null;
            vista.Cargar(() =>
            {
                var servicio = new ProductoService();
                List<CategoriaProducto>? obtenidas = cargarCategorias ? servicio.ObtenerCategorias() : null;
                return (servicio.Buscar(filtro), obtenidas == null ? null : () => EstablecerCategorias(obtenidas));
            }, conFiltros, seleccionado, sinSeleccion);
        }

        private void EstablecerCategorias(List<CategoriaProducto> obtenidas)
        {
            categorias = obtenidas;
            VincularCategorias(buscarCategoria, obtenidas, "Todas");
        }

        private static void VincularCategorias(ComboBox control, List<CategoriaProducto> valores, string opcion)
        {
            control.DisplayMember = nameof(CategoriaProducto.Nombre);
            control.ValueMember = nameof(CategoriaProducto.IdCategoria);
            control.DataSource = new List<CategoriaProducto> { new() { Nombre = opcion } }.Concat(valores).ToList();
        }

        private static string Precio(decimal precio) => $"RD$ {precio.ToString("N2", CultureInfo.CurrentCulture)}";

        private static string NivelStock(int stock) => stock switch
        {
            0 => "Agotado",
            <= ProductoService.LimiteStockBajo => "Stock bajo",
            _ => "Disponible"
        };

        private static string EstadoInventario(Producto producto) => producto.Activo ? NivelStock(producto.Stock) : "Inactivo";

        private void MostrarFicha(Producto? producto)
        {
            if (producto == null)
            {
                vista.PresentarFicha("Selecciona un producto para ver sus detalles");
                return;
            }
            vista.PresentarFicha(producto.Nombre,
                ("Código", producto.Codigo),
                ("Categoría", ControlesRegistro.Disponible(producto.Categoria)),
                ("Precio", Precio(producto.Precio)),
                ("Existencias", $"{producto.Stock:N0} unidades"),
                ("Nivel de stock", producto.Activo ? NivelStock(producto.Stock) : $"{NivelStock(producto.Stock)} en inventario"),
                ("Estado administrativo", producto.Estado),
                ("Fecha de registro", producto.FechaRegistro.ToString("dd/MM/yyyy")));
        }

        private void PintarCatalogo(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || vista.Registro(e.RowIndex) is not { } p || e.Graphics is not { } g) return;
            bool seleccionado = (e.State & DataGridViewElementStates.Selected) != 0;
            var textoFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter;
            Rectangle bordes = e.CellBounds;
            GraphicsState estadoDibujo = g.Save();
            g.SetClip(Rectangle.Intersect(bordes, e.ClipBounds));
            using SolidBrush fondoFila = new(seleccionado ? EstiloAgenda.Seleccion : Color.White);
            using Pen separador = new(EstiloAgenda.Borde);
            g.FillRectangle(fondoFila, bordes);
            int margen = vista.LogicalToDeviceUnits(8);
            if (e.ColumnIndex == 0)
            {
                int lado = vista.LogicalToDeviceUnits(40);
                var icono = new Rectangle(bordes.X + margen, bordes.Y + (bordes.Height - lado) / 2, lado, lado);
                using GraphicsPath forma = EstiloAgenda.RectanguloRedondo(icono, vista.LogicalToDeviceUnits(8));
                using SolidBrush fondo = new(FondoIcono);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(fondo, forma);
                int dibujo = vista.LogicalToDeviceUnits(22);
                IconosAgenda.Dibujar(g, SimboloAgenda.Productos, EstiloAgenda.Tinta,
                    new(icono.X + (lado - dibujo) / 2, icono.Y + (lado - dibujo) / 2, dibujo, dibujo));
                int x = icono.Right + margen;
                int altoLinea = Math.Max(nombreProducto.Height, vista.Font.Height);
                int y = bordes.Y + (bordes.Height - 2 * altoLinea) / 2;
                var superior = new Rectangle(x, y, Math.Max(1, bordes.Right - x - margen), altoLinea);
                var inferior = new Rectangle(superior.X, superior.Bottom, superior.Width, altoLinea);
                TextRenderer.DrawText(g, p.Nombre, nombreProducto, superior, EstiloAgenda.Tinta, textoFlags);
                TextRenderer.DrawText(g, p.Categoria, vista.Font, inferior, EstiloAgenda.Secundario, textoFlags);
            }
            else if (e.ColumnIndex == 3)
            {
                string estado = EstadoInventario(p);
                (Color color, Color tinta) = estado switch
                {
                    "Agotado" => (Color.FromArgb(252, 229, 232), Color.FromArgb(136, 43, 57)),
                    "Stock bajo" => (Color.FromArgb(253, 239, 211), Color.FromArgb(111, 68, 17)),
                    "Disponible" => (Color.FromArgb(221, 242, 234), Color.FromArgb(0, 82, 67)),
                    _ => (FondoIcono, EstiloAgenda.Secundario)
                };
                int alto = vista.LogicalToDeviceUnits(26);
                int ancho = Math.Max(1, Math.Min(bordes.Width - margen, TextRenderer.MeasureText(estado, vista.Font).Width + margen));
                var etiqueta = new Rectangle(bordes.X + (bordes.Width - ancho) / 2, bordes.Y + (bordes.Height - alto) / 2, ancho, alto);
                using GraphicsPath forma = EstiloAgenda.RectanguloRedondo(etiqueta, alto / 2);
                using SolidBrush fondo = new(color);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(fondo, forma);
                TextRenderer.DrawText(g, estado, vista.Font, etiqueta, tinta, textoFlags | TextFormatFlags.HorizontalCenter);
            }
            else
            {
                TextRenderer.DrawText(g, e.FormattedValue?.ToString() ?? "", vista.Font, bordes, EstiloAgenda.Tinta,
                    textoFlags | TextFormatFlags.HorizontalCenter);
            }
            g.SmoothingMode = SmoothingMode.None;
            g.DrawLine(separador, bordes.Left, bordes.Bottom - 1, bordes.Right, bordes.Bottom - 1);
            if (seleccionado && vista.Tabla.Focused && vista.Tabla.CurrentCellAddress == new Point(e.ColumnIndex, e.RowIndex))
                ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(bordes, -2, -2), EstiloAgenda.Tinta, EstiloAgenda.Seleccion);
            g.Restore(estadoDibujo);
            e.Handled = true;
        }

        private void AbrirEditor(Producto? producto)
        {
            string permiso = producto == null ? PermisosSistema.Agregar : PermisosSistema.Modificar;
            if (!SesionActual.VerificarPermiso(permiso, producto == null ? "registrar productos" : "modificar productos"))
                return;
            if (categorias == null)
            {
                MessageBox.Show(this, "Primero deben cargarse las categorías. Pulse Consultar para volver a intentarlo.",
                    "Categorías de productos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var editor = new EditorRegistro(producto == null ? "Nuevo producto" : "Editar producto", permiso, producto != null);
            var codigo = ControlesRegistro.Texto(30, producto?.Codigo ?? "");
            var nombre = ControlesRegistro.Texto(100, producto?.Nombre ?? "");
            var categoria = ControlesRegistro.Opciones();
            VincularCategorias(categoria, categorias, "Seleccione una categoría");
            var precio = ControlesRegistro.Texto(12, producto?.Precio.ToString("0.00", CultureInfo.CurrentCulture) ?? "");
            var stock = ControlesRegistro.Texto(7, producto?.Stock.ToString(CultureInfo.InvariantCulture) ?? "0");
            var activo = new CheckBox { Text = "Producto activo", Checked = producto?.Activo ?? true, AutoSize = true };
            editor.AgregarCampo("Código", codigo);
            editor.AgregarCampo("Nombre", nombre);
            editor.AgregarCampo("Categoría", categoria);
            editor.AgregarCampo("Precio (RD$)", precio);
            editor.AgregarCampo("Existencias", stock);
            editor.AgregarCampo("Estado", activo);
            if (producto != null && categorias.Any(c => c.IdCategoria == producto.IdCategoria))
            {
                // La lista enlazada ya tiene su contexto cuando se abre el diálogo.
                editor.Load += (_, _) => categoria.SelectedValue = producto.IdCategoria;
            }

            editor.PrepararGuardado = () =>
            {
                if (string.IsNullOrWhiteSpace(codigo.Text)) return editor.Advertir("Ingrese el código del producto.", codigo);
                if (string.IsNullOrWhiteSpace(nombre.Text)) return editor.Advertir("Ingrese el nombre del producto.", nombre);
                if (categoria.SelectedValue is not int idCategoria || idCategoria <= 0)
                    return editor.Advertir("Seleccione una categoría válida.", categoria);
                if (!decimal.TryParse(precio.Text.Trim(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.CurrentCulture, out decimal monto))
                    return editor.Advertir($"Ingrese un precio válido, por ejemplo {650.50m.ToString("0.00")}.", precio);
                if (!int.TryParse(stock.Text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int existencias))
                    return editor.Advertir("El stock debe ser un número entero (0 o más).", stock);
                var datos = new Producto
                {
                    IdProducto = producto?.IdProducto ?? 0, Codigo = codigo.Text, Nombre = nombre.Text,
                    Precio = monto, Stock = existencias, IdCategoria = idCategoria, Activo = activo.Checked
                };
                return producto == null ? () => new ProductoService().Registrar(datos) : () => new ProductoService().Modificar(datos);
            };
            if (editor.ShowDialog(this) == DialogResult.OK) Consultar(producto?.IdProducto, producto == null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) nombreProducto.Dispose();
        }
    }
}
