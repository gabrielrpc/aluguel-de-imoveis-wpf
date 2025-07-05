using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace aluguel_de_imoveis_wpf.View.Assets
{
    public partial class DetalhesImovelView : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly LocacaoService _locacaoService;
        private readonly ImovelService _imovelService;

        private Guid _imovelId;
        private DateTime _dataInicio;
        private DateTime _dataFim;

        public DetalhesImovelView(MainWindow mainWindow, Imovel imovel)
        {
            InitializeComponent();
            _locacaoService = new LocacaoService();
            _imovelService = new ImovelService();
            _mainWindow = mainWindow;
            _imovelId = imovel.Id;
            DataContext = imovel;
        }

        private void VoltarClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.VoltarParaPainel();
        }

        private void SelectDateInicio(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker picker && picker.SelectedDate is DateTime data)
            {
                _dataInicio = data;
            }
        }

        private void SelectDateFim(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker picker && picker.SelectedDate is DateTime data)
            {
                _dataFim = data;
            }
        }

        private async void RegistrarAluguel(object sender, RoutedEventArgs e)
        {

            try
            {

                await _locacaoService.RegistrarLocacaoAsync(_dataInicio, _dataFim, _imovelId);

                MessageBox.Show("Aluguel registrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                await _imovelService.ListarImoveisDisponiveis();
                await _locacaoService.ListarMinhasLocacoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
