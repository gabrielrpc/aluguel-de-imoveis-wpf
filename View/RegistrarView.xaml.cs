using System.Windows;
using System.Windows.Controls;

namespace aluguel_de_imoveis_wpf.View
{
    public partial class RegistrarView : UserControl
    {
        public RegistrarView()
        {
            InitializeComponent();
        }

        private void NomeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           NomePlaceholderText.Visibility = string.IsNullOrEmpty(NomeTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholderText.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CpfTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new string(CpfTextBox.Text.Where(char.IsDigit).ToArray());

            if (text.Length > 11)
                text = text.Substring(0, 11);

            if (text.Length <= 3)
                CpfTextBox.Text = text;
            else if (text.Length <= 6)
                CpfTextBox.Text = $"{text.Substring(0, 3)}.{text.Substring(3)}";
            else if (text.Length <= 9)
                CpfTextBox.Text = $"{text.Substring(0, 3)}.{text.Substring(3, 3)}.{text.Substring(6)}";
            else
                CpfTextBox.Text = $"{text.Substring(0, 3)}.{text.Substring(3, 3)}.{text.Substring(6, 3)}-{text.Substring(9)}";

            CpfTextBox.CaretIndex = CpfTextBox.Text.Length;
            CpfPlaceholderText.Visibility = string.IsNullOrEmpty(CpfTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TelefoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new string(TelefoneTextBox.Text.Where(char.IsDigit).ToArray());

            if (text.Length > 11)
                text = text.Substring(0, 11);

            if (text.Length <= 2)
                TelefoneTextBox.Text = $"{text}";
            else if (text.Length <= 7)
                TelefoneTextBox.Text = $"({text.Substring(0, 2)}) {text.Substring(2)}";
            else if (text.Length <= 11)
            {
                if (text[2] == '9')
                    TelefoneTextBox.Text = $"({text.Substring(0, 2)}) {text.Substring(2, 1)} {text.Substring(3, 4)}-{text.Substring(7)}";
                else 
                    TelefoneTextBox.Text = $"({text.Substring(0, 2)}) {text.Substring(2, 4)}-{text.Substring(6)}";
            }

            TelefoneTextBox.CaretIndex = TelefoneTextBox.Text.Length;
            TelefonePlaceholderText.Visibility = string.IsNullOrEmpty(TelefoneTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SenhaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SenhaPlaceholderText.Visibility = string.IsNullOrEmpty(SenhaTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new LoginView();
            }
        }
    }
}
