using aluguel_de_imoveis.Utils.Enums;
using aluguel_de_imoveis_wpf.Communication.Response;
using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace aluguel_de_imoveis_wpf.ViewModel
{
    public class PainelViewModel : INotifyPropertyChanged
    {
        private readonly ImovelService _imovelService;
        private readonly LocacaoService _locacaoService;
        private readonly Action _abrirLogin;
        private readonly Action<Imovel, Func<Task>> _abrirDetalhes;

        public ObservableCollection<Imovel> ListaImoveisDisponiveis { get; } = new();
        public ObservableCollection<ListarLocacaoResponseJson> ListaMinhasLocacoes { get; } = new();

        public TipoImovel TipoSelecionado { get; set; }

        public string _titulo = string.Empty;
        public string _descricao = string.Empty;
        public string _valorAluguelTexto = string.Empty;
        public string _logradouro = string.Empty;
        public string _numeroTexto = string.Empty;
        public string _bairro = string.Empty;
        public string _cidade = string.Empty;
        public string _uf = string.Empty;
        public string _cep = string.Empty;

        public Array TiposImovel { get; } = Enum.GetValues(typeof(TipoImovel));

        public ICommand CadastrarImovelCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand AbrirDetalhesCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public PainelViewModel(Action<Imovel, Func<Task>> abrirDetalhes, Action abrirLogin)
        {
            _imovelService = new ImovelService();
            _locacaoService = new LocacaoService();
            _abrirLogin = abrirLogin;
            _abrirDetalhes = abrirDetalhes;

            CadastrarImovelCommand = new RelayCommand(async (_) => await CadastrarImovelAsync());
            LogoutCommand = new RelayCommand(_ => RealizarLogout());
            AbrirDetalhesCommand = new RelayCommand<Imovel>(AbrirDetalhes);

            _ = CarregarDados();
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

        public string Uf
        {
            get => _uf;
            set { _uf = value; OnPropertyChanged(); }
        }

        public string Cep
        {
            get => _cep;
            set
            {
                var digits = new string(value.Where(char.IsDigit).ToArray());

                if (digits.Length > 8)
                    digits = digits.Substring(0, 8);

                if (digits.Length > 5)
                    _cep = $"{digits.Substring(0, 5)}-{digits.Substring(5)}";
                else
                    _cep = digits;

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
                var digits = new string(value.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(digits))
                {
                    _valorAluguelTexto = "";
                    OnPropertyChanged();
                    return;
                }

                if (decimal.TryParse(digits, out decimal parsed))
                {
                    parsed /= 100; 
                    _valorAluguelTexto = parsed.ToString("N2"); 
                }

                OnPropertyChanged();
            }
        }

        private async Task CarregarDados()
        {
            await CarregarImoveisAsync();
            await CarregarMinhasLocacoes();
        }

        private async Task CarregarImoveisAsync()
        {
            try
            {
                var imoveis = await _imovelService.ListarImoveisDisponiveis();
                ListaImoveisDisponiveis.Clear();

                foreach (var imovel in imoveis.Where(i => i != null))
                    ListaImoveisDisponiveis.Add(imovel!);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task CarregarMinhasLocacoes()
        {
            try
            {
                var locacoes = await _locacaoService.ListarMinhasLocacoes();
                ListaMinhasLocacoes.Clear();

                foreach (var locacao in locacoes.Where(l => l != null))
                {
                    locacao!.DiasEmAndamentoTratado = TratarDiasEmAndamento(locacao.DiasEmAndamento);
                    locacao!.DiasRestantesTratado = TratarDiasRestantes(locacao.DiasEmAndamento, locacao.DiasRestantes);
                    ListaMinhasLocacoes.Add(locacao!);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task CadastrarImovelAsync()
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

                var novoImovel = new Imovel
                {
                    Titulo = Titulo,
                    Descricao = Descricao,
                    ValorAluguel = valor,
                    Disponivel = true,
                    Tipo = TipoSelecionado,
                    UsuarioId = Guid.Parse(UsuarioId),
                    Endereco = new Endereco
                    {
                        Logradouro = Logradouro,
                        Numero = numero,
                        Bairro = Bairro,
                        Cidade = Cidade,
                        Uf = Uf,
                        Cep = Cep
                    }
                };

                await _imovelService.CadastrarImovel(novoImovel);
                MessageBox.Show("Imóvel cadastrado com sucesso!");

                LimparCampos();
                await CarregarImoveisAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LimparCampos()
        {
            Titulo = Descricao = ValorAluguelTexto = Logradouro = NumeroTexto = Bairro = Cidade = Uf = Cep = "";
            TipoSelecionado = (TipoImovel)TiposImovel.GetValue(0)!;
            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(Descricao));
            OnPropertyChanged(nameof(ValorAluguelTexto));
            OnPropertyChanged(nameof(Logradouro));
            OnPropertyChanged(nameof(NumeroTexto));
            OnPropertyChanged(nameof(Bairro));
            OnPropertyChanged(nameof(Cidade));
            OnPropertyChanged(nameof(Uf));
            OnPropertyChanged(nameof(Cep));
            OnPropertyChanged(nameof(TipoSelecionado));
        }

        private void RealizarLogout()
        {
            if (MessageBox.Show("Deseja realmente sair?", "Confirmação", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TokenStorage.ClearToken();
                _abrirLogin();
            }
        }

        private void AbrirDetalhes(Imovel imovel)
        {
            _abrirDetalhes(imovel, CarregarDados);
        }

        private string TratarDiasEmAndamento(int atual)
        {
            if (atual < 0) return $"Início em {Math.Abs(atual)} dias";
            else return $"Dia {atual + 1} da contagem";
        }

        private string TratarDiasRestantes(int atual, int resto)
        {
            if (resto == 0) return "Locação finalizada";
            else if (atual < 0) return $"{atual + resto} dias faltando";
            else return $"Faltam {resto} dias";
        }

        protected void OnPropertyChanged([CallerMemberName] string nome = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
