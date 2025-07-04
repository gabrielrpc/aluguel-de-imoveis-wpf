using aluguel_de_imoveis_wpf.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace aluguel_de_imoveis_wpf.View.Assets
{
    public partial class DetalhesImovelView : Page
    {
        private readonly MainWindow _mainWindow;

        public DetalhesImovelView(MainWindow mainWindow, Imovel imovel)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            DataContext = imovel;
        }

        private void VoltarClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.VoltarParaPainel();
        }

    }
}
