using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using JuegoDeMemoria.Services;

namespace JuegoDeMemoria.Views
{
    public partial class ImageManagerWindow : Window
    {
        private readonly ImagenService _imagenService;

        public ImageManagerWindow()
        {
            InitializeComponent();
            _imagenService = new ImagenService();
            CargarThumbnails();
        }

        private void CargarThumbnails()
        {
            wrapPanelThumbnails.Children.Clear();
            var rutas = _imagenService.ObtenerRutasImagenesDisponibles();
            foreach (var ruta in rutas)
            {
                var img = new System.Windows.Controls.Image
                {
                    Width = 90,
                    Height = 90,
                    Margin = new Thickness(6),
                    Source = _imagenService.ObtenerMiniatura(ruta, 90, 90),
                    Tag = ruta
                };
                var border = new Border { Child = img, BorderThickness = new Thickness(1), Margin = new Thickness(4) };
                wrapPanelThumbnails.Children.Add(border);
            }
        }

        private async void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Multiselect = true, Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.gif" };
            if (dlg.ShowDialog() == true)
            {
                await _imagenService.CopiarImagenesSeleccionadasAsync(dlg.FileNames);
                MessageBox.Show("Imágenes copiadas correctamente.", "Imágenes", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarThumbnails();
            }
        }

        private void btnEliminarSeleccionadas_Click(object sender, RoutedEventArgs e)
        {
            // Simplificado: elimina las imágenes que estén "marcadas" mediante pasar el mouse (demo)
            foreach (Border b in wrapPanelThumbnails.Children)
            {
                if (b.Child is System.Windows.Controls.Image img && img.Tag is string ruta && img.IsMouseOver)
                {
                    try { File.Delete(ruta); } catch { }
                }
            }
            CargarThumbnails();
            MessageBox.Show("Eliminadas las seleccionadas.", "Eliminar", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
