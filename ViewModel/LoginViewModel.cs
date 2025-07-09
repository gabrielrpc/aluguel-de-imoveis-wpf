using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace aluguel_de_imoveis_wpf.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _email = string.Empty;
        private string _senha = string.Empty;
        private readonly UsuarioService _usuarioService;
        private readonly Action _abrirPainel;
        private readonly Action _abrirRegistrar;

        public LoginViewModel(Action abrirPainel, Action abrirRegistrar)
        {
            _usuarioService = new UsuarioService();
            _abrirPainel = abrirPainel;
            _abrirRegistrar = abrirRegistrar;

            LoginCommand = new RelayCommand(async (_) => await LoginAsync());
            RegistrarCommand = new RelayCommand(_ => _abrirRegistrar());
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Senha
        {
            get => _senha;
            set
            {
                _senha = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegistrarCommand { get; }

        private async Task LoginAsync()
        {
            try
            {
                var response = await _usuarioService.LoginAsync(Email, Senha);
                if (response == null)
                    throw new Exception("Falha no login. Verifique suas credenciais.");

                TokenStorage.SaveToken(response.Token);
                _abrirPainel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ocorreu um erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        protected void OnPropertyChanged([CallerMemberName] string nome = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
