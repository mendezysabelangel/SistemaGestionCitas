using System.Diagnostics;
using System.Globalization;

namespace SistemaGestionCitas.src.Forms;

internal static class PreferenciaTema
{
    internal static bool Leer(int idUsuario)
    {
        try
        {
            string ruta = Ruta(idUsuario);
            return File.Exists(ruta) && File.ReadAllText(ruta) == "oscuro";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Trace.TraceWarning("Citorna, leer tema: {0}", ex.GetType().Name);
            return false;
        }
    }

    internal static void Guardar(int idUsuario, bool oscuro)
    {
        string ruta = Ruta(idUsuario);
        string temporal = ruta + "." + Guid.NewGuid().ToString("N");
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
            try
            {
                File.WriteAllText(temporal, oscuro ? "oscuro" : "claro");
                File.Move(temporal, ruta, overwrite: true);
            }
            finally
            {
                File.Delete(temporal);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Trace.TraceWarning("Citorna, guardar tema: {0}", ex.GetType().Name);
        }
    }

    private static string Ruta(int idUsuario) => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Citorna", "Preferencias", idUsuario.ToString(CultureInfo.InvariantCulture) + ".tema");
}
