using System;

namespace JuegoDeMemoria.Models
{
    public class ResultadoPartida
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public int Puntaje { get; set; }
        public int Movimientos { get; set; }
        public int DuracionSegundos { get; set; }
        public Nivel Nivel { get; set; }
        public DateTime Fecha { get; set; }
    }
}
