using System;
using System.Collections.Generic;
using System.Linq;
using JuegoDeMemoria.Models;

namespace JuegoDeMemoria.Services
{
    public class JuegoService
    {
        private List<Carta> _cartas = new();
        private int _movimientos = 0;
        private DateTime _inicio;
        public int Movimientos => _movimientos;
        public int SegundosTranscurridos => (int)(DateTime.Now - _inicio).TotalSeconds;
        public int TiempoEsperaEmparejadoMs { get; } = 800;

        public void Inicializar(Nivel nivel, string usuario)
        {
            _inicio = DateTime.Now;
            _movimientos = 0;
        }

        public List<Carta> GenerarTablero(Nivel nivel, IEnumerable<string> rutas)
        {
            int numParejas = nivel == Nivel.Facil ? 8 : nivel == Nivel.Medio ? 18 : 32;
            var listaRutas = rutas.ToList();

            if (listaRutas.Count == 0)
            {
                // usar emojis como respaldo
                var emojis = new[] { "🍎","🍌","🍇","🍓","🍊","🍑","🍍","🥝","🍉","🍒","🥥","🍐","🥭","🍋","🍈","🍓","🍓","🍌","🍎","🍇","🍊","🍑","🍍","🥝","🍉","🍒","🥥","🍐","🥭","🍋","🍈","🍓" };
                listaRutas = emojis.Take(numParejas).ToList();
            }

            // duplicar si menos imágenes
            while (listaRutas.Count < numParejas)
            {
                var add = listaRutas.Take(Math.Min(listaRutas.Count, numParejas - listaRutas.Count)).ToList();
                listaRutas.AddRange(add);
            }

            var seleccion = listaRutas.OrderBy(x => Guid.NewGuid()).Take(numParejas).ToList();

            var pares = new List<Carta>();
            int id = 1;
            foreach (var r in seleccion)
            {
                var parId = Guid.NewGuid().ToString();
                pares.Add(new Carta { Id = id++, ImagenPath = r, ParId = parId });
                pares.Add(new Carta { Id = id++, ImagenPath = r, ParId = parId });
            }

            // mezclar
            pares = pares.OrderBy(x => Guid.NewGuid()).ToList();
            _cartas = pares;
            return pares;
        }

        public void IncrementarMovimiento() => _movimientos++;

        public bool CompararPareja(Carta a, Carta b)
        {
            if (a.ParId == b.ParId)
            {
                a.EstaEmparejada = b.EstaEmparejada = true;
                return true;
            }
            return false;
        }

        public bool EstaTerminado() => _cartas.All(c => c.EstaEmparejada);

        public int CalcularPuntajeFinal(int movimientos, int segundos, Nivel nivel)
        {
            double mult = nivel == Nivel.Facil ? 1.0 : nivel == Nivel.Medio ? 1.5 : 2.0;
            int baseScore = 1000 - (movimientos * 10) - segundos;
            if (baseScore < 0) baseScore = 0;
            return (int)Math.Round(baseScore * mult);
        }
    }
}
