using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using JuegoDeMemoria.Services;
using JuegoDeMemoria.Models;

namespace JuegoDeMemoria.Views
{
    public partial class GameWindow : Window
    {
        private readonly string _usuario;
        private readonly Nivel _nivel;
        private readonly JuegoService _juegoService;
        private readonly ImagenService _imagenService;
        private readonly ArchivoService _archivoService;
        private readonly DispatcherTimer _uiTimer;
        private Carta _primeraSeleccionada;
        private bool _bloqueado = false;

        public GameWindow(string usuario, Nivel nivel)
        {
            InitializeComponent();
            _usuario = usuario;
            _nivel = nivel;
            _juegoService = new JuegoService();
            _imagenService = new ImagenService();
            _archivoService = new ArchivoService();

            _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _uiTimer.Tick += UiTimer_Tick;

            InicializarJuego();
        }

        private void InicializarJuego()
        {
            int filas = _nivel == Nivel.Facil ? 4 : _nivel == Nivel.Medio ? 6 : 8;
            TableroGrid.Rows = filas;
            TableroGrid.Columns = filas;

            var rutas = _imagenService.ObtenerRutasImagenesDisponibles();
            var cartas = _juegoService.GenerarTablero(_nivel, rutas);

            TableroGrid.Children.Clear();
            foreach (var carta in cartas)
            {
                var btn = new Button { Tag = carta, Padding = new Thickness(2), Margin = new Thickness(6) };
                btn.Click += Carta_Click;
                btn.Content = CrearContenidoReverso();
                TableroGrid.Children.Add(btn);
            }

            lblMovimientos.Content = "Movimientos: 0";
            lblTiempo.Content = "Tiempo: 0s";

            _juegoService.Inicializar(_nivel, _usuario);
            _uiTimer.Start();
        }

        private UIElement CrearContenidoReverso()
        {
            return new TextBlock { Text = "?", FontSize = 24, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        }

        private UIElement CrearContenidoFrente(Carta carta)
        {
            if (!string.IsNullOrEmpty(carta.ImagenPath) && System.IO.File.Exists(carta.ImagenPath))
            {
                var img = new System.Windows.Controls.Image
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(carta.ImagenPath)),
                    Stretch = System.Windows.Media.Stretch.Uniform
                };
                return img;
            }
            else
            {
                return new TextBlock { Text = carta.ParId, FontSize = 28, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            }
        }

        private async void Carta_Click(object sender, RoutedEventArgs e)
        {
            if (_bloqueado) return;
            if (!(sender is Button btn) || !(btn.Tag is Carta carta)) return;
            if (carta.EstaEmparejada || carta.EstaRevelada) return;

            carta.EstaRevelada = true;
            btn.Content = CrearContenidoFrente(carta);

            if (_primeraSeleccionada == null)
            {
                _primeraSeleccionada = carta;
                return;
            }

            _bloqueado = true;
            _juegoService.IncrementarMovimiento();
            lblMovimientos.Content = $"Movimientos: {_juegoService.Movimientos}";

            var segunda = carta;
            bool coinciden = _juegoService.CompararPareja(_primeraSeleccionada, segunda);

            if (coinciden)
            {
                MarcaParejadaEnUI(_primeraSeleccionada);
                MarcaParejadaEnUI(segunda);
            }
            else
            {
                await Task.Delay(_juegoService.TiempoEsperaEmparejadoMs);
                OcultarCartaEnUI(_primeraSeleccionada);
                OcultarCartaEnUI(segunda);
            }

            _primeraSeleccionada = null;
            _bloqueado = false;

            if (_juegoService.EstaTerminado())
            {
                _uiTimer.Stop();
                int segundos = _juegoService.SegundosTranscurridos;
                int movs = _juegoService.Movimientos;
                int puntaje = _juegoService.CalcularPuntajeFinal(movs, segundos, _nivel);

                var resultado = new Models.ResultadoPartida
                {
                    NombreUsuario = _usuario,
                    Movimientos = movs,
                    DuracionSegundos = segundos,
                    Puntaje = puntaje,
                    Nivel = _nivel,
                    Fecha = DateTime.Now
                };
                await _archivoService.GuardarResultadosAsync(resultado);

                MessageBox.Show($"Juego finalizado!\nPuntaje: {puntaje}\nMovimientos: {movs}\nTiempo: {segundos}s", "Fin del juego", MessageBoxButton.OK, MessageBoxImage.Information);
                new MainMenuWindow(_usuario).Show();
                this.Close();
            }
        }

        private void MarcaParejadaEnUI(Carta carta)
        {
            foreach (Button b in TableroGrid.Children)
            {
                if (b.Tag is Carta c && c.Id == carta.Id)
                {
                    c.EstaEmparejada = true;
                    b.IsEnabled = false;
                    break;
                }
            }
        }

        private void OcultarCartaEnUI(Carta carta)
        {
            foreach (Button b in TableroGrid.Children)
            {
                if (b.Tag is Carta c && c.Id == carta.Id)
                {
                    c.EstaRevelada = false;
                    b.Content = CrearContenidoReverso();
                    break;
                }
            }
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            lblTiempo.Content = $"Tiempo: {_juegoService.SegundosTranscurridos}s";
        }

        private void btnAbandonar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Abandonar la partida? Se perderá el progreso.", "Abandonar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _uiTimer.Stop();
                new MainMenuWindow(_usuario).Show();
                this.Close();
            }
        }
    }
}
