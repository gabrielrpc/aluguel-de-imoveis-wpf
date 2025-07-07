using aluguel_de_imoveis_wpf.ViewModel;
using System.Windows.Controls;

namespace aluguel_de_imoveis_wpf.View
{
    public partial class PainelView : UserControl
    {
        public PainelView(MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = new PainelViewModel((imovel, func) => mainWindow.AbrirDetalhes(imovel, func), mainWindow.AbrirLogin);
        }
    }
}
