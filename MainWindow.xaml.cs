using aluguel_de_imoveis_wpf.View;
using System.Windows;

namespace aluguel_de_imoveis_wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContent.Content = new LoginView();
    }
}