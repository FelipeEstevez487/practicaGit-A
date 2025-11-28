namespace JuegoDeMemoria.Models
{
    public class Carta
    {
        public int Id { get; set; }
        public string ImagenPath { get; set; } = string.Empty;
        public bool EstaRevelada { get; set; }
        public bool EstaEmparejada { get; set; }
        public string ParId { get; set; } = string.Empty;
    }
}
