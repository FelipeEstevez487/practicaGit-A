using System;
using System.Windows;

namespace JuegoDeMemoria.Views
{
    public partial class MainMenuWindow : Window
    {
        private readonly string _usuario;

        public MainMenuWindow(string usuario)
        {
            InitializeComponent();
            _usuario = usuario;
            lblBienvenida.Content = $"Bienvenido, {usuario}";
            cmbNivel.ItemsSource = Enum.GetValues(typeof(Models.Nivel));
            cmbNivel.SelectedItem = Models.Nivel.Facil;
        }

        private void btnJugar_Click(object sender, RoutedEventArgs e)
        {
            var nivel = (Models.Nivel)cmbNivel.SelectedItem;
            new GameWindow(_usuario, nivel).Show();
            this.Close();
        }

        private void btnGestionImagenes_Click(object sender, RoutedEventArgs e)
        {
            new ImageManagerWindow().ShowDialog();
        }

        private void btnVerResultados_Click(object sender, RoutedEventArgs e)
        {
            new ResultsWindow().ShowDialog();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}
