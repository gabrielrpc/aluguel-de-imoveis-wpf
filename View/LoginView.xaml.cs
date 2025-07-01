using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aluguel_de_imoveis_wpf.View
{
    public partial class LoginView : UserControl
    {
        private readonly HttpClient _httpClient;
        public LoginView()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://your-api-url/");
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

        private void OnRegistrarClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new RegistrarView();
            }
        }
    }
}
