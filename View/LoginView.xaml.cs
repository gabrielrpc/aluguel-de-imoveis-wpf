using aluguel_de_imoveis_wpf.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace aluguel_de_imoveis_wpf.View
{
    public partial class LoginView : UserControl
    {
        public LoginView(MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = new LoginViewModel(
                abrirPainel: mainWindow.AbrirPainel,
                abrirRegistrar: mainWindow.AbrirRegistrar
            );
        }

        private void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;

            PlaceholderText.Visibility = string.IsNullOrEmpty(passwordBox!.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (this.DataContext is LoginViewModel viewModel)
            {
                viewModel.Senha = passwordBox.Password;
            }
        }
    }
}
