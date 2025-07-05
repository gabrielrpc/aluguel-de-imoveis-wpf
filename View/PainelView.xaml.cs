using aluguel_de_imoveis.Utils.Enums;
using aluguel_de_imoveis_wpf.Communication.Response;
using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
using aluguel_de_imoveis_wpf.View.Assets;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace aluguel_de_imoveis_wpf.View
{

    public partial class PainelView : UserControl
    {
        public ObservableCollection<Imovel> ListaImoveisDisponiveis { get; set; }
        public ObservableCollection<ListarLocacaoResponseJson> ListaMinhasLocacoes { get; set; }

        public TipoImovel TipoSelecionado { get; set; }

        private readonly ImovelService _imovelService;
        private readonly LocacaoService _locacaoService;
        private readonly MainWindow _mainWindow;

        public PainelView(MainWindow mainWindow)
        {
            InitializeComponent();

            _imovelService = new ImovelService();
            _locacaoService = new LocacaoService();
            _mainWindow = mainWindow;

            ComboTipoImovel.ItemsSource = Enum.GetValues(typeof(TipoImovel));
            ComboTipoImovel.SelectedIndex = 0;

            ListaImoveisDisponiveis = new ObservableCollection<Imovel>();

            ListaMinhasLocacoes = new ObservableCollection<ListarLocacaoResponseJson>();

            DataContext = this;

            Loaded += async (_, _) => await CarregarImoveisAsync();

            Loaded += async (_, _) => await CarregarMinhasLocacoes();
        }

        private async Task CarregarImoveisAsync()
        {
            try
            {
                var imoveis = await _imovelService.ListarImoveisDisponiveis();

                ListaImoveisDisponiveis.Clear();

                foreach (var imovel in imoveis.Where(i => i != null))
                {
                    ListaImoveisDisponiveis.Add(imovel!);
                }
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

        private async void CadastrarImovel(object sender, RoutedEventArgs e)
        {
            try
            {
                var token = TokenStorage.GetToken();
                var UsuarioId = JwtUtils.GetUserIdFromToken(token);

                if (string.IsNullOrEmpty(UsuarioId))
                {
                    throw new Exception("Usuário não autenticado. Por favor, faça login novamente.");
                }

                Guid usuarioGuid = Guid.Parse(UsuarioId);

                var valorAluguel = decimal.TryParse(ValorAluguelTextBox.Text, out decimal valor);
                var numeroImovel = int.TryParse(NumeroTextBox.Text, out int numero);

                var novoImovel = new Imovel
                {

                    Titulo = TituloTextBox.Text,
                    Descricao = DescricaoTextBox.Text,
                    ValorAluguel = valor,
                    Disponivel = true,
                    Tipo = TipoSelecionado,
                    Endereco = new Endereco
                    {
                        Logradouro = LogradouroTextBox.Text,
                        Numero = numero,
                        Bairro = BairroTextBox.Text,
                        Cidade = CidadeTextBox.Text,
                        Uf = UfTextBox.Text,
                        Cep = CepTextBox.Text
                    },
                    UsuarioId = usuarioGuid
                };

                await _imovelService.CadastrarImovel(novoImovel);

                MessageBox.Show("Imóvel cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                ResetarCampos();

                await CarregarImoveisAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResetarCampos()
        {
            TituloTextBox.Text = "";
            DescricaoTextBox.Text = "";
            ValorAluguelTextBox.Text = "";
            ComboTipoImovel.SelectedIndex = 0;
            LogradouroTextBox.Text = "";
            NumeroTextBox.Text = "";
            BairroTextBox.Text = "";
            CidadeTextBox.Text = "";
            UfTextBox.Text = "";
            CepTextBox.Text = "";
     }

        private void MaisClick(object sender, RoutedEventArgs e)
        {
            var imovel = ((Hyperlink)sender).DataContext as Imovel;
            if (imovel != null)
            {
                _mainWindow.AbrirDetalhes(imovel);
            }
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resultado = MessageBox.Show(
            "Deseja realmente sair do sistema?",
            "Confirmação de Logout",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                TokenStorage.ClearToken();
                _mainWindow.AbrirLogin();
            }
        }
   
        private string TratarDiasEmAndamento(int atual)
        {
            if (atual < 0)
            {
                return $"Início em {Math.Abs(atual)} dias";
            }
            else if (atual >= 0)
            {
                return $"Dia {atual + 1} da contagem ";
            }

            return string.Empty;
        }

        private string TratarDiasRestantes(int atual, int resto)
        {
            if (atual < 0)
            {
                return $"{atual + resto} dias faltando";
            }
            else if (atual >= 0)
            {
                return $"Faltam {resto} dias";
            }else if (resto == 0)
            {
                return "Locação finalizada";
            }

            return string.Empty;
        }
    }
}
