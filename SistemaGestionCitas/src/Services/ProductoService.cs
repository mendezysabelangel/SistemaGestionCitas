using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services
{
    public class ProductoService
    {
        // Un producto con stock entre 1 y este número se considera "stock bajo"
        public const int LimiteStockBajo = 5;

        private const int StockMaximo = 1000000;
        private const decimal PrecioMaximo = 99999999.99m;   // límite de DECIMAL(10,2)

        private readonly ProductoRepository repositorio = new ProductoRepository();

        public List<Producto> Buscar(FiltroProducto filtro)
        {
            return repositorio.Buscar(filtro);
        }

        public List<CategoriaProducto> ObtenerCategorias()
        {
            return repositorio.ObtenerCategorias();
        }

        public ResultadoOperacion Registrar(Producto producto)
        {
            // Además del botón, el servicio también revisa el permiso (doble protección)
            if (!SesionActual.TienePermiso(PermisosSistema.Agregar))
                return Error("Permisos insuficientes para registrar productos.");

            ResultadoOperacion validacion = Validar(producto);
            if (!validacion.Exitoso) return validacion;

            // Duplicados
            if (repositorio.ExisteCodigo(producto.Codigo, null))
                return Error("Ya existe un producto con el código " + producto.Codigo + ".");

            try
            {
                repositorio.Insertar(producto);
            }
            // 2627 y 2601 = error de SQL Server por valor duplicado (UNIQUE).
            // Pasaría si dos personas registran el mismo código al mismo tiempo.
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Error("Ya existe un producto con ese código.");
            }

            return Ok("Producto registrado correctamente.");
        }

        public ResultadoOperacion Modificar(Producto producto)
        {
            if (!SesionActual.TienePermiso(PermisosSistema.Modificar))
                return Error("Permisos insuficientes para modificar productos.");

            if (producto.IdProducto <= 0)
                return Error("Seleccione un producto para modificar.");

            ResultadoOperacion validacion = Validar(producto);
            if (!validacion.Exitoso) return validacion;

            // Se excluye el propio producto: puede conservar su código
            if (repositorio.ExisteCodigo(producto.Codigo, producto.IdProducto))
                return Error("Otro producto ya tiene el código " + producto.Codigo + ".");

            try
            {
                if (!repositorio.Actualizar(producto))
                    return Error("El producto ya no existe en la base de datos.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return Error("Otro producto ya tiene ese código.");
            }

            return Ok("Producto modificado correctamente.");
        }

        public ResultadoOperacion Eliminar(int idProducto)
        {
            if (!SesionActual.TienePermiso(PermisosSistema.Eliminar))
                return Error("Permisos insuficientes para eliminar productos.");

            try
            {
                if (!repositorio.Eliminar(idProducto))
                    return Error("El producto ya no existe en la base de datos.");
            }
            // 547 = error por llave foránea: otra tabla usa este producto
            catch (SqlException ex) when (ex.Number == 547)
            {
                return Error("No se puede eliminar: el producto está siendo utilizado.\n" +
                             "Puede marcarlo como inactivo desde Modificar.");
            }

            return Ok("Producto eliminado correctamente.");
        }

        // Valida y limpia los datos (Trim y código en mayúsculas)
        private ResultadoOperacion Validar(Producto p)
        {
            p.Codigo = p.Codigo.Trim().ToUpperInvariant();
            p.Nombre = p.Nombre.Trim();

            // Campos vacíos y formato
            if (p.Codigo == "")
                return Error("El código es obligatorio.");

            if (!Regex.IsMatch(p.Codigo, @"^[A-Z0-9][A-Z0-9_\-]{1,29}$"))
                return Error("El código debe tener entre 2 y 30 caracteres: letras, números, guion (-) o guion bajo (_).");

            if (p.Nombre == "" || p.Nombre.Length > 100)
                return Error("El nombre es obligatorio (máximo 100 caracteres).");

            // Datos inválidos
            if (p.Precio <= 0 || p.Precio > PrecioMaximo)
                return Error("El precio debe ser mayor que 0 y no puede superar 99,999,999.99.");

            if (decimal.Round(p.Precio, 2) != p.Precio)
                return Error("El precio admite como máximo 2 decimales.");

            if (p.Stock < 0 || p.Stock > StockMaximo)
                return Error("El stock debe estar entre 0 y 1,000,000.");

            // La categoría debe existir y estar activa
            if (p.IdCategoria <= 0 ||
                !repositorio.ObtenerCategorias().Any(c => c.IdCategoria == p.IdCategoria))
                return Error("Seleccione una categoría válida.");

            return Ok("");
        }

        private ResultadoOperacion Ok(string mensaje)
        {
            return new ResultadoOperacion { Exitoso = true, Mensaje = mensaje };
        }

        private ResultadoOperacion Error(string mensaje)
        {
            return new ResultadoOperacion { Exitoso = false, Mensaje = mensaje };
        }
    }
}