using aluguel_de_imoveis_wpf.Communication.Request;
using aluguel_de_imoveis_wpf.Communication.Response;
using aluguel_de_imoveis_wpf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(BaseUrl.Url);
        }

        public async Task<LoginResponseJson?> LoginAsync(string email, string senha)
        {
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                {
                    throw new ArgumentException("Email e senha devem ser informados.");
                }

                var loginRequest = new LoginRequestJson
                {
                    Email = email,
                    Senha = senha
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"usuario/login", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                        if (errorResponse != null && errorResponse.ContainsKey("erro"))
                        {
                            var mensagem = errorResponse["erro"];
                            throw new Exception(mensagem);
                        }

                        throw new Exception("Falha no login. Verifique suas credenciais.");
                    }
                    catch (JsonException)
                    {
                        throw new Exception("Erro inesperado ao fazer login.");
                    }
                }

                var loginResponse = JsonSerializer.Deserialize<LoginResponseJson>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return loginResponse;
            }
        }

        public async Task<string> RegistrarAsync(string nome, string email, string cpf, string telefone, string senha)
        {
            var usuarioRequest = new Usuario
            {
                Nome = nome,
                Email = email,
                Cpf = cpf,
                Telefone = telefone,
                Senha = senha
            };

            var json = JsonSerializer.Serialize(usuarioRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"usuario/cadastrar", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var jsonDocument = JsonDocument.Parse(responseContent);
                    var root = jsonDocument.RootElement;

                    string mensagem;
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        if (root.TryGetProperty("erros", out var errosProp) && errosProp.ValueKind == JsonValueKind.Array)
                        {
                            var erros = errosProp.Deserialize<string[]>();
                            mensagem = erros?.Length > 0 ? string.Join("\n- ", erros) : "Falha no cadastro. Tente novamente.";
                            mensagem = $"- {mensagem}";
                        }
                        else if (root.TryGetProperty("erro", out var erroProp) && erroProp.ValueKind == JsonValueKind.String)
                        {
                            mensagem = $"- {erroProp.GetString()}";
                        }
                        else
                        {
                            mensagem = "Falha no cadastro. Tente novamente.";
                        }
                    }
                    else if (root.ValueKind == JsonValueKind.String)
                    {
                        mensagem = $"- {root.GetString()}";
                    }
                    else
                    {
                        mensagem = "Falha no cadastro. Tente novamente.";
                    }

                    throw new Exception(mensagem);
                }
                catch (JsonException)
                {
                    throw new Exception("Erro inesperado ao fazer o cadastro, Tente novamente mais tarde!");
                }
            }

            return "created";
        }
    }
}
