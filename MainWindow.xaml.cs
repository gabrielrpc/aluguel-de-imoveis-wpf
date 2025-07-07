using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.View;
using aluguel_de_imoveis_wpf.View.Assets;
using System.Windows;
using System.Windows.Controls;

namespace aluguel_de_imoveis_wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AbrirLogin();
    }
    public void AbrirPainel()
    {
        MainContent.Visibility = Visibility.Collapsed;
        MainFrame.Visibility = Visibility.Visible;

        MainFrame.Navigate(new PainelView(this));
    }

    public void AbrirDetalhes(Imovel imovel, Func<Task> atualizarPainel)
    {
        MainFrame.Navigate(new DetalhesImovelView(this, imovel, atualizarPainel));
    }

    public void AbrirRegistrar()
    {
        MainContent.Content = new RegistrarView(this);
    }

    public void AbrirLogin()
    {
        MainFrame.Visibility = Visibility.Collapsed;
        MainContent.Visibility = Visibility.Visible;
        MainContent.Content = new LoginView(this);
    }

    public void VoltarParaPainel()
    {
        if (MainFrame.CanGoBack)
            MainFrame.GoBack();
    }
}