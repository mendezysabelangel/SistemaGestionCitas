using System.Security.Cryptography;

namespace SistemaGestionCitas.src.Services
{
    public static class SeguridadHelper
    {
        private const int Iteraciones = 100000; 
        private const int TamanoSalt = 16;   
        private const int TamanoHash = 32;

        public static string GenerarHash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(TamanoSalt);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);

            return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerificarPassword(string password, string hashGuardado)
        {
            string[] partes = hashGuardado.Split('.');
            if (partes.Length != 3) return false;

            if (!int.TryParse(partes[0], out int iteraciones)) return false;

            byte[] salt = Convert.FromBase64String(partes[1]);
            byte[] hashEsperado = Convert.FromBase64String(partes[2]);

            byte[] hashCalculado = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
    }
}