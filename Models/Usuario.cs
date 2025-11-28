using System;

namespace JuegoDeMemoria.Models
{
    public class Usuario
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string HashContrasena { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
