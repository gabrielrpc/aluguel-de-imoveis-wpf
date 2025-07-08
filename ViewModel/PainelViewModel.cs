using aluguel_de_imoveis.Utils.Enums;
using aluguel_de_imoveis_wpf.Communication.Response;
using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Relatorios;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
using aluguel_de_imoveis_wpf.Utils.Enums;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        private readonly Action<Imovel, Func<Task>> _abrirEditarAnuncio;

        public ObservableCollection<Imovel> ListaImoveisDisponiveis { get; } = new();
        public ObservableCollection<ListarLocacaoResponseJson> ListaMinhasLocacoes { get; } = new();

        public ObservableCollection<Imovel> ListaRelatorio { get; } = new();

        public TipoImovel TipoSelecionado { get; set; }
        public UF UfSelecionado { get; set; } = UF.RJ;

        public string _titulo = string.Empty;
        public string _descricao = string.Empty;
        public string _valorAluguelTexto = string.Empty;
        public string _logradouro = string.Empty;
        public string _numeroTexto = string.Empty;
        public string _bairro = string.Empty;
        public string _cidade = string.Empty;
        public string _cep = string.Empty;
        public string _valorAluguelMaxTexto = string.Empty;
        public string _valorAluguelMinTexto = string.Empty;

        public Array TiposImovel { get; } = Enum.GetValues(typeof(TipoImovel));
        public Array UfEnum { get; } = Enum.GetValues(typeof(UF));

        public TipoImovel? TipoSelecionadoRel { get; set; } = TipoImovel.Casa;

        public ICommand CadastrarImovelCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand AbrirDetalhesCommand { get; }
        public ICommand FiltrarRelatorioCommand { get; }
        public ICommand AbrirEditarAnuncioCommand { get;  }

        public ICommand ProximaCommand { get; }
        public ICommand AnteriorCommand { get; }

        public PainelViewModel(Action<Imovel, Func<Task>> abrirDetalhes, Action<Imovel, Func<Task>> abrirEditarAnuncio, Action abrirLogin)
        {
            _imovelService = new ImovelService();
            _locacaoService = new LocacaoService();
            _abrirLogin = abrirLogin;
            _abrirDetalhes = abrirDetalhes;
            _abrirEditarAnuncio = abrirEditarAnuncio;

            CadastrarImovelCommand = new RelayCommand(async (_) => await CadastrarImovelAsync());
            LogoutCommand = new RelayCommand(_ => RealizarLogout());
            AbrirDetalhesCommand = new RelayCommand<Imovel>(AbrirDetalhes);
            FiltrarRelatorioCommand = new RelayCommand(async (_) => await FiltrarRelatorios());
            ProximaCommand = new RelayCommand(async (_) => await ProximaPaginaAsync());
            AnteriorCommand = new RelayCommand(async (_) => await PaginaAnteriorAsync());
            AbrirEditarAnuncioCommand = new RelayCommand<Imovel>(AbrirEditarAnuncio);

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
                _valorAluguelTexto = FormatarValorMonetario(value);
                OnPropertyChanged();
            }
        }

        public string ValorAluguelMaxTexto
        {
            get => _valorAluguelMaxTexto;
            set
            {
                _valorAluguelMaxTexto = FormatarValorMonetario(value);
                OnPropertyChanged();
            }
        }

        public string ValorAluguelMinTexto
        {
            get => _valorAluguelMinTexto;
            set
            {
                _valorAluguelMinTexto = FormatarValorMonetario(value);
                OnPropertyChanged();
            }
        }

        private int _paginaAtual = 1;
        private const int _itensPorPagina = 10;
        private bool _podeAvancar = true;

        public bool PodeAvancar
        {
            get => _podeAvancar;
            set
            {
                if (_podeAvancar != value)
                {
                    _podeAvancar = value;
                    OnPropertyChanged(nameof(PodeAvancar));
                }
            }
        }

        public bool PodeVoltar
        {
            get => _paginaAtual > 1;
        }

        public int PaginaAtual
        {
            get => _paginaAtual;
            set { _paginaAtual = value; OnPropertyChanged(); }
        }

        private async Task ProximaPaginaAsync()
        {
            var proximaPagina = _paginaAtual + 1;
            var imoveis = await _imovelService.ListarImoveisDisponiveis(pagina: proximaPagina);

            if (imoveis != null && imoveis.Any())
            {
                _paginaAtual = proximaPagina;
                AtualizarLista(imoveis);
            }

            else
            {
                MessageBox.Show("Não há mais imóveis disponíveis para exibição.");
                PodeAvancar = false;
            }

            OnPropertyChanged(nameof(PodeVoltar));
            OnPropertyChanged(nameof(PaginaAtual));
        }

        private async Task PaginaAnteriorAsync()
        {
            if (_paginaAtual > 1)
            {
                _paginaAtual--;
                var imoveis = await _imovelService.ListarImoveisDisponiveis(pagina: _paginaAtual);
                AtualizarLista(imoveis);

                OnPropertyChanged(nameof(PaginaAtual));
                OnPropertyChanged(nameof(PodeVoltar));
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

        private async Task CarregarDados()
        {
            await CarregarImoveisAsync();
            await CarregarMinhasLocacoes();
        }

        private void AtualizarLista(List<Imovel?> imoveis)
        {
            ListaImoveisDisponiveis.Clear();

            foreach (var imovel in imoveis.Where(i => i != null))
                ListaImoveisDisponiveis.Add(imovel!);

            PodeAvancar = imoveis.Count == _itensPorPagina;
        }

        private async Task CarregarImoveisAsync()
        {
            try
            {
                var imoveis = await _imovelService.ListarImoveisDisponiveis();
                AtualizarLista(imoveis);
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
                        Uf = UfSelecionado.ToString(),
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

        private async Task FiltrarRelatorios()
        {
            try
            {
                TipoImovel? tipo = TipoSelecionadoRel;
                decimal? valorMin = null;
                decimal? valorMax = null;

                if (tipo == null && string.IsNullOrWhiteSpace(ValorAluguelMinTexto) &&  string.IsNullOrWhiteSpace(ValorAluguelMaxTexto))
                {
                    throw new Exception("Preencha ao menos um dos campos para realizar o filtro.");
                }

                if (!string.IsNullOrWhiteSpace(ValorAluguelMinTexto))
                {
                    if (!decimal.TryParse(ValorAluguelMinTexto, out var min))
                        throw new Exception("Valor mínimo do aluguel inválido.");
                    valorMin = min;
                }

                if (!string.IsNullOrWhiteSpace(ValorAluguelMaxTexto))
                {
                    if (!decimal.TryParse(ValorAluguelMaxTexto, out var max))
                        throw new Exception("Valor máximo do aluguel inválido.");
                    valorMax = max;
                }


                var imoveis = await _imovelService.ListarImoveisDisponiveis(tipo, valorMin, valorMax);

                QuestPDF.Settings.License = LicenseType.Community;
                var relatorio = new RelatorioDeImoveisDisponiveis(imoveis);
                relatorio.GeneratePdf("relatorio.pdf");
                var caminho = Path.Combine(Directory.GetCurrentDirectory(), "relatorio.pdf");
                Process.Start(new ProcessStartInfo
                {
                    FileName = caminho,
                    UseShellExecute = true
                });

                ListaRelatorio.Clear();

                foreach (var imovel in imoveis.Where(i => i != null))
                    ListaRelatorio.Add(imovel!);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void AbrirEditarAnuncio(Imovel imovel)
        {
            _abrirEditarAnuncio(imovel, CarregarDados);
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

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        protected void OnPropertyChanged([CallerMemberName] string nome = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
