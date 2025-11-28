using System.Security.Cryptography;
using System.Text;

namespace JuegoDeMemoria.Services
{
    public class AuthService
    {
        public string CalcularHashSHA256(string contrasena)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(contrasena);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
