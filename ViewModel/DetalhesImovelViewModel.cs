using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace aluguel_de_imoveis_wpf.ViewModel
{
    public class DetalhesImovelViewModel : INotifyPropertyChanged
    {
        private readonly LocacaoService _locacaoService;
        private readonly ImovelService _imovelService;
        private readonly Action _voltarParaPainel;
        private readonly Func<Task> _atualizarPainel;

        private DateTime? _dataInicio;
        private DateTime? _dataFim;

        public Imovel Imovel { get; }

        public ICommand RegistrarLocacaoCommand { get; }
        public ICommand VoltarCommand { get; }
        public ICommand RemoverAnuncioCommand { get; }

        public DetalhesImovelViewModel(Imovel imovel, Action voltarParaPainel, Func<Task> atualizarPainel)
        {
            _locacaoService = new LocacaoService();
            _imovelService = new ImovelService();
            _voltarParaPainel = voltarParaPainel;
            _atualizarPainel = atualizarPainel;

            Imovel = imovel;
            RegistrarLocacaoCommand = new RelayCommand(async (_) => await RegistrarLocacaoAsync());
            VoltarCommand = new RelayCommand(_ => _voltarParaPainel());
            RemoverAnuncioCommand = new RelayCommand(async _ => await RemoverAnuncio());
        }

        public DateTime? DataInicio
        {
            get => _dataInicio;
            set { _dataInicio = value; OnPropertyChanged(); }
        }

        public DateTime? DataFim
        {
            get => _dataFim;
            set { _dataFim = value; OnPropertyChanged(); }
        }

        private async Task RegistrarLocacaoAsync()
        {
            if (DataInicio == null || DataFim == null)
            {
                MessageBox.Show("Selecione uma data de início e fim válidas.");
                return;
            }

            try
            {
                await _locacaoService.RegistrarLocacaoAsync(DataInicio.Value, DataFim.Value, Imovel.Id);

                await _atualizarPainel();

                MessageBox.Show("Aluguel registrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                _voltarParaPainel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task RemoverAnuncio()
        {
            if (MessageBox.Show("Deseja realmente remover o anuncio?", "Confirmação", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    await _imovelService.RemoverImovel(Imovel.Id);

                    await _atualizarPainel();

                    MessageBox.Show("Anuncio removido com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    _voltarParaPainel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
