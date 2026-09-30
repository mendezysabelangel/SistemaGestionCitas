using SistemaGestionCitas.src.Forms;

namespace SistemaGestionCitas
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormLogin());
        }
    }
}