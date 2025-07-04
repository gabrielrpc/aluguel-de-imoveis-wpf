using aluguel_de_imoveis.Utils.Enums;
using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.View.Assets;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace aluguel_de_imoveis_wpf.View
{

    public partial class PainelView : UserControl
    {
        public ObservableCollection<Imovel> ListaImoveisDisponiveis { get; set; }

        private readonly ImovelService _imovelService;
        private readonly MainWindow _mainWindow;

        public PainelView(MainWindow mainWindow)
        {
            InitializeComponent();

            _imovelService = new ImovelService();
            _mainWindow = mainWindow;

            ListaImoveisDisponiveis = new ObservableCollection<Imovel>();

            DataContext = this;

            Loaded += async (_, _) => await CarregarImoveisAsync();
        }

        private async Task CarregarImoveisAsync()
        {
            try
            {
                var imoveis = await _imovelService.ListarImoveisDisponiveis();

                ListaImoveisDisponiveis.Clear();

                foreach (var imovel in imoveis.Where(i => i != null))
                {
                    ListaImoveisDisponiveis.Add(imovel!);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MaisClick(object sender, RoutedEventArgs e)
        {
            var imovel = ((Hyperlink)sender).DataContext as Imovel;
            if (imovel != null)
            {
                _mainWindow.AbrirDetalhes(imovel);
            }
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado = MessageBox.Show(
            "Deseja realmente sair do sistema?",
            "Confirmação de Logout",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                TokenStorage.ClearToken();
                _mainWindow.AbrirLogin();
            }
        }
    }
}
