using aluguel_de_imoveis.Utils.Enums;
using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
using aluguel_de_imoveis_wpf.Utils.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace aluguel_de_imoveis_wpf.ViewModel
{
    public class EditarAnuncioViewModel : INotifyPropertyChanged
    {
        public Imovel Imovel { get; }

        private readonly ImovelService _imovelService;
        private readonly Action _voltarParaPainel;
        private readonly Func<Task> _atualizarPainel;

        public string _titulo = string.Empty;
        public string _descricao = string.Empty;
        public string _valorAluguelTexto = string.Empty;
        public string _logradouro = string.Empty;
        public string _numeroTexto = string.Empty;
        public string _bairro = string.Empty;
        public string _cidade = string.Empty;
        public string _cep = string.Empty;
        public TipoImovel _tipoSelecionado;
        public UF _ufSelecionado;

        public Array TiposImovel { get; } = Enum.GetValues(typeof(TipoImovel));
        public Array UfEnum { get; } = Enum.GetValues(typeof(UF));

        public ICommand VoltarCommand { get; }
        public ICommand EditarImovelCommand { get; }

        public EditarAnuncioViewModel(Imovel imovel, Action voltarParaPainel, Func<Task> atualizarPainel)
        {
            _imovelService = new ImovelService();
            _voltarParaPainel = voltarParaPainel;
            _atualizarPainel = atualizarPainel;

            Imovel = imovel;
            _titulo = imovel.Titulo;
            _descricao = imovel.Descricao;
            _valorAluguelTexto = FormatarValorMonetario(imovel.ValorAluguel.ToString());
            _logradouro = imovel.Endereco.Logradouro;
            _numeroTexto = imovel.Endereco.Numero.ToString();
            _bairro = imovel.Endereco.Bairro;
            _cidade = imovel.Endereco.Cidade;
            _cep = FormatarCep(imovel.Endereco.Cep);
            _tipoSelecionado = imovel.Tipo;
            _ufSelecionado = (UF)Enum.Parse(typeof(UF), imovel.Endereco.Uf);

            VoltarCommand = new RelayCommand(_ => _voltarParaPainel());
            EditarImovelCommand = new RelayCommand(async _ => await EditarImovelAsync());
        }

        public string Titulo
        {
            get => _titulo;
            set { _titulo = value; OnPropertyChanged(); }
        }

        public string Descricao
        {
            get => _descricao;
            set { _descricao = value; OnPropertyChanged(); }
        }

        public string Logradouro
        {
            get => _logradouro;
            set { _logradouro = value; OnPropertyChanged(); }
        }

        public string Bairro
        {
            get => _bairro;
            set { _bairro = value; OnPropertyChanged(); }
        }

        public string Cidade
        {
            get => _cidade;
            set { _cidade = value; OnPropertyChanged(); }
        }

        public string Cep
        {
            get => _cep;
            set
            {
               _cep = FormatarCep(value);
                OnPropertyChanged();
            }
        }

        public string NumeroTexto
        {
            get => _numeroTexto;
            set
            {
                _numeroTexto = new string(value.Where(char.IsDigit).ToArray());
                OnPropertyChanged();
            }
        }

        public string ValorAluguelTexto
        {
            get => _valorAluguelTexto;
            set
            {
                _valorAluguelTexto = FormatarValorMonetario(value);
                OnPropertyChanged();
            }
        }

        public TipoImovel TipoSelecionado {
            get => _tipoSelecionado;
            set { _tipoSelecionado = value; OnPropertyChanged(); }
        }
        public UF UfSelecionado { 
            get => _ufSelecionado; 
            set { _ufSelecionado = value; OnPropertyChanged(); }
        }

        private async Task EditarImovelAsync()
        {
            try
            {
                var token = TokenStorage.GetToken();
                var UsuarioId = JwtUtils.GetUserIdFromToken(token);

                if (string.IsNullOrEmpty(UsuarioId))
                    throw new Exception("Usuário não autenticado. Por favor, faça login novamente.");

                if (!decimal.TryParse(ValorAluguelTexto, out var valor))
                    throw new Exception("Valor do aluguel inválido");

                if (!int.TryParse(NumeroTexto, out var numero))
                    throw new Exception("Número do imóvel inválido");

                var imovelId = Imovel.Id;

                var imovelAtualizado = new Imovel
                {
                    Titulo = Titulo,
                    Descricao = Descricao,
                    ValorAluguel = valor,
                    Tipo = TipoSelecionado,
                    Endereco = new Endereco
                    {
                        Logradouro = Logradouro,
                        Numero = numero,
                        Bairro = Bairro,
                        Cidade = Cidade,
                        Uf = UfSelecionado.ToString(),
                        Cep = Cep
                    }
                };

                await _imovelService.AtualizarImovel(imovelAtualizado, imovelId);

                MessageBox.Show("Anúncio alterado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                LimparCampos();

                await _atualizarPainel();

                _voltarParaPainel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar o anúncio: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatarValorMonetario(string value)
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digits))
                return "";

            if (decimal.TryParse(digits, out decimal parsed))
                return (parsed / 100).ToString("N2");

            return "";
        }

        private string FormatarCep(string value)
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());

            if (digits.Length > 8)
                digits = digits.Substring(0, 8);

            if (digits.Length >= 6)
                return $"{digits.Substring(0, 5)}-{digits.Substring(5)}";
            else
                return digits;

        }

        private void LimparCampos()
        {
            Titulo = Descricao = ValorAluguelTexto = Logradouro = NumeroTexto = Bairro = Cidade = Cep = "";
            TipoSelecionado = (TipoImovel)TiposImovel.GetValue(0)!;
            UfSelecionado = UF.RJ;
            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(Descricao));
            OnPropertyChanged(nameof(ValorAluguelTexto));
            OnPropertyChanged(nameof(Logradouro));
            OnPropertyChanged(nameof(NumeroTexto));
            OnPropertyChanged(nameof(Bairro));
            OnPropertyChanged(nameof(Cidade));
            OnPropertyChanged(nameof(Cep));
            OnPropertyChanged(nameof(TipoSelecionado));
            OnPropertyChanged(nameof(UfSelecionado));
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
