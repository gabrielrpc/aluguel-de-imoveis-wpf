using aluguel_de_imoveis_wpf.Services;
using aluguel_de_imoveis_wpf.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace aluguel_de_imoveis_wpf.ViewModel
{
    public class RegistrarViewModel : INotifyPropertyChanged
    {
        private string _nome = string.Empty;
        private string _email = string.Empty;
        private string _cpf = string.Empty;
        private string _telefone = string.Empty;
        private string _senha = string.Empty;

        private readonly UsuarioService _usuarioService;
        private readonly Action _abrirLogin;

        public RegistrarViewModel(Action abrirLogin)
        {
            _usuarioService = new UsuarioService();
            _abrirLogin = abrirLogin;

            RegistrarCommand = new RelayCommand(async (_) => await RegistrarAsync());
            IrParaLoginCommand = new RelayCommand(_ => _abrirLogin());
        }

        public string Nome
        {
            get => _nome;
            set { _nome = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Cpf
        {
            get => _cpf;
            set {
                string digits = new string(value.Where(char.IsDigit).ToArray());

                if (digits.Length > 11)
                    digits = digits.Substring(0, 11);

                if (digits.Length <= 3)
                    _cpf = digits;
                else if (digits.Length <= 6)
                    _cpf = $"{digits.Substring(0, 3)}.{digits.Substring(3)}";
                else if (digits.Length <= 9)
                    _cpf = $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6)}";
                else
                    _cpf = $"{digits.Substring(0, 3)}.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-{digits.Substring(9)}";

                OnPropertyChanged();
            }
        }

        public string Telefone
        {
            get => _telefone;
            set {
                string digits = new string(value.Where(char.IsDigit).ToArray());

                if (digits.Length > 11)
                    digits = digits.Substring(0, 11);

                if (digits.Length <= 2)
                    _telefone = $"{digits}";
                else if (digits.Length <= 7)
                    _telefone = $"({digits.Substring(0, 2)}) {digits.Substring(2)}";
                else if (digits.Length <= 11)
                {
                    if (digits[2] == '9')
                        _telefone = $"({digits.Substring(0, 2)}) {digits.Substring(2, 1)} {digits.Substring(3, 4)}-{digits.Substring(7)}";
                    else
                        _telefone = $"({digits.Substring(0, 2)}) {digits.Substring(2, 4)}-{digits.Substring(6)}";
                }
            }
        }

        public string Senha
        {
            get => _senha;
            set { _senha = value; OnPropertyChanged(); }
        }

        public ICommand RegistrarCommand { get; }
        public ICommand IrParaLoginCommand { get; }

        private async Task RegistrarAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Cpf) || string.IsNullOrEmpty(Telefone) || string.IsNullOrEmpty(Senha))
                {
                    throw new ArgumentException("É necessário preencher todos os dados para finalizar o cadastro!");
                }

                await _usuarioService.RegistrarAsync(Nome, Email, Cpf, Telefone, Senha);
                MessageBox.Show("Usuário registrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                _abrirLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ocorreu um erro!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged([CallerMemberName] string nome = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
        }
    }
}
