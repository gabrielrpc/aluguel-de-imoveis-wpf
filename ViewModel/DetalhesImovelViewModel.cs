﻿using aluguel_de_imoveis_wpf.Model;
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

        private DateTime? _dataInicio;
        private DateTime? _dataFim;

        public Imovel Imovel { get; }

        public DetalhesImovelViewModel(Imovel imovel, Action voltarParaPainel)
        {
            _locacaoService = new LocacaoService();
            _imovelService = new ImovelService();
            _voltarParaPainel = voltarParaPainel;

            Imovel = imovel;
            RegistrarCommand = new RelayCommand(async (_) => await RegistrarAsync());
            VoltarCommand = new RelayCommand(_ => _voltarParaPainel());
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

        public ICommand RegistrarCommand { get; }
        public ICommand VoltarCommand { get; }

        private async Task RegistrarAsync()
        {
            if (DataInicio == null || DataFim == null)
            {
                MessageBox.Show("Selecione uma data de início e fim válidas.");
                return;
            }

            try
            {
                await _locacaoService.RegistrarLocacaoAsync(DataInicio.Value, DataFim.Value, Imovel.Id);

                MessageBox.Show("Aluguel registrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                await _imovelService.ListarImoveisDisponiveis();
                await _locacaoService.ListarMinhasLocacoes();

                _voltarParaPainel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
