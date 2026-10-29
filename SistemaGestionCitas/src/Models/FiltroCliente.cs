namespace SistemaGestionCitas.src.Models
{
    // Guarda los filtros elegidos antes de generar el reporte.
    // Un valor null significa "no filtrar por este campo".
    public class FiltroCliente
    {
        public int? IdCliente { get; set; }
        public string? Nombre { get; set; }
        public bool? Activo { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }

        // Texto que resume los filtros (se muestra en el reporte, en la impresión y en Excel)
        public string Descripcion()
        {
            List<string> partes = new List<string>();

            if (IdCliente.HasValue)
                partes.Add("ID: " + IdCliente.Value);

            if (!string.IsNullOrWhiteSpace(Nombre))
                partes.Add("Nombre contiene \"" + Nombre.Trim() + "\"");

            if (Activo.HasValue)
                partes.Add("Estado: " + (Activo.Value ? "Activo" : "Inactivo"));

            if (Desde.HasValue)
                partes.Add("Registrados desde " + Desde.Value.ToString("dd/MM/yyyy"));

            if (Hasta.HasValue)
                partes.Add("hasta " + Hasta.Value.ToString("dd/MM/yyyy"));

            if (partes.Count == 0)
                return "Filtros: ninguno (todos los registros)";

            return "Filtros: " + string.Join("  ·  ", partes);
        }
    }
}