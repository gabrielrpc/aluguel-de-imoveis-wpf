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
    public class ImovelService : AuthorizedHttpClient
    {

        public ImovelService()
        {
            _httpClient.BaseAddress = new Uri(BaseUrl.Url);
        }

        public async Task<List<Imovel?>> ListarImoveisDisponiveis()
        {
            var response = await _httpClient.GetAsync($"imovel/listar-imoveis-disponiveis");

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

                    throw new Exception("Falha ao listar os imóveis, tente novamente mais tarde!");
                }
                catch (JsonException)
                {
                    throw new Exception("Erro inesperado ao listar os imvóveis.");
                }
            }

            var imoveis = JsonSerializer.Deserialize<List<Imovel?>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return imoveis ?? new List<Imovel?>();
        }
    }
}
