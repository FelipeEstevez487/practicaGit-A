using System.Windows;
using JuegoDeMemoria.Services;

namespace JuegoDeMemoria.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UsuarioService _usuarioService;

        public RegisterWindow()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService(new AuthService(), new ArchivoService());
        }

        private async void btnCrearCuenta_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtNuevoUsuario.Text?.Trim() ?? string.Empty;
            string pass = pwdNuevaContrasena.Password ?? string.Empty;
            string confirm = pwdConfirmar.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Rellena todos los campos.", "Registro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Registro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool creado = await _usuarioService.CrearCuentaAsync(usuario, pass);
            if (creado)
            {
                MessageBox.Show("Cuenta creada con éxito.", "Registro", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("No se pudo crear la cuenta (usuario ya existe).", "Registro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
