using SistemaGestionCitas.src.Data;
using SistemaGestionCitas.src.Models;

namespace SistemaGestionCitas.src.Services;

internal sealed class BitacoraCitasService
{
    internal const int LimiteEventos = 100;
    private readonly BitacoraCitasRepository repositorio = new();

    internal static bool PuedeConsultar() => SesionActual.HaySesion
        && SesionActual.TienePermiso(PermisosSistema.Consultar)
        && SesionActual.TienePermiso(PermisosSistema.CrearUsuarios);

    internal Task<List<EventoBitacoraCita>> ConsultarAsync(CancellationToken cancelacion = default)
    {
        if (!PuedeConsultar())
            throw new UnauthorizedAccessException("No tiene permiso para consultar la bitácora de Citas.");
        return repositorio.ConsultarRecientesAsync(LimiteEventos, cancelacion);
    }
}
