using aluguel_de_imoveis_wpf.Communication.Response;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aluguel_de_imoveis_wpf.View
{
    public partial class LoginView : UserControl
    {
        private readonly UsuarioService _usuarioService;
        private MainWindow _mainWindow;

        public LoginView(MainWindow mainWindow)
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _mainWindow = mainWindow;
        }

        private void PasswordBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PasswordBox.Focus();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholderText.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await _usuarioService.LoginAsync(EmailTextBox.Text, PasswordBox.Password);

                TokenStorage.SaveToken(response.Token);

                _mainWindow.AbrirPainel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRegistrarClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.AbrirRegistrar();
        }
    }
}
