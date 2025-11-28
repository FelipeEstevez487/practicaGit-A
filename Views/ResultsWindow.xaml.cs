using System.Threading.Tasks;
using System.Windows;
using JuegoDeMemoria.Services;

namespace JuegoDeMemoria.Views
{
    public partial class ResultsWindow : Window
    {
        private readonly ArchivoService _archivoService;

        public ResultsWindow()
        {
            InitializeComponent();
            _archivoService = new ArchivoService();
            _ = CargarResultadosAsync();
        }

        private async Task CargarResultadosAsync()
        {
            var resultados = await _archivoService.LeerResultadosAsync();
            dataGridResultados.ItemsSource = resultados;
        }

        private async void btnExportarCSV_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog { FileName = "resultados", DefaultExt = ".csv", Filter = "CSV files (*.csv)|*.csv" };
            if (dlg.ShowDialog() == true)
            {
                _archivoService.ExportarResultadosACSV(dlg.FileName);
                MessageBox.Show("Exportación realizada con éxito.", "Exportar CSV", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void btnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            await CargarResultadosAsync();
        }
    }
}
