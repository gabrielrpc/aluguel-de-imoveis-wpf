using aluguel_de_imoveis_wpf.Communication.Request;
using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Services
{
    public class LocacaoService : AuthorizedHttpClient
    {

        public LocacaoService()
        {
            _httpClient.BaseAddress = new Uri(BaseUrl.Url);
        }

        public async Task<string> RegistrarLocacaoAsync(DateTime inicio, DateTime fim, Guid imovelId)
        {
            var token = TokenStorage.GetToken();
            var UsuarioId = JwtUtils.GetUserIdFromToken(token);

            if (string.IsNullOrEmpty(UsuarioId))
            {
                throw new Exception("Usuário não autenticado. Por favor, faça login novamente.");
            }

            Guid usuarioGuid = Guid.Parse(UsuarioId);

            var registrarAluguelRequestJson = new RegistrarAluguelRequestJson
            {
                DataInicio = inicio,
                DataFim = fim,
                UsuarioId = usuarioGuid,
                ImovelId = imovelId
            };

            var json = JsonSerializer.Serialize(registrarAluguelRequestJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"locacao/registrar-locacao", content);

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
                            mensagem = "Falha ao registrar o aluguel. Tente novamente.";
                        }
                    }
                    else if (root.ValueKind == JsonValueKind.String)
                    {
                        mensagem = $"- {root.GetString()}";
                    }
                    else
                    {
                        mensagem = "Falha ao registrar o aluguel. Tente novamente.";
                    }

                    throw new Exception(mensagem);
                }
                catch (JsonException)
                {
                    throw new Exception("Erro inesperado ao fazer o cadastro, Tente novamente mais tarde!");
                }
            }

            return "ok";
        }
    }
}
