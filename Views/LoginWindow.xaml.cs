using System.Windows;
using JuegoDeMemoria.Services;
using JuegoDeMemoria.Models;

namespace JuegoDeMemoria.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UsuarioService _usuarioService;

        public LoginWindow()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService(new AuthService(), new ArchivoService());
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text?.Trim() ?? string.Empty;
            string pass = pwdContrasena.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Ingrese usuario y contraseña.", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = await _usuarioService.LoginAsync(usuario, pass);
            if (user != null)
            {
                new MainMenuWindow(user.NombreUsuario).Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrecta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }
    }
}
