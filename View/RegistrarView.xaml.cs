using aluguel_de_imoveis_wpf.ViewModel;
using System.Windows.Controls;

namespace aluguel_de_imoveis_wpf.View
{
    public partial class RegistrarView : UserControl
    {
        public RegistrarView(MainWindow mainWindow)
        {
            InitializeComponent();
            DataContext = new RegistrarViewModel(mainWindow.AbrirLogin);
        }
    }
}
