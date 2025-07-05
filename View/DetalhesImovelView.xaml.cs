using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.ViewModel;
using System.Windows.Controls;

namespace aluguel_de_imoveis_wpf.View.Assets
{
    public partial class DetalhesImovelView : Page
    {
        public DetalhesImovelView(MainWindow mainWindow, Imovel imovel)
        {
            InitializeComponent();
            DataContext = new DetalhesImovelViewModel(imovel, mainWindow.VoltarParaPainel);
        }
    }
}
