﻿using aluguel_de_imoveis_wpf.Communication.Response;
using aluguel_de_imoveis_wpf.Model;
using aluguel_de_imoveis_wpf.Security;
using aluguel_de_imoveis_wpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

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

        public async Task<string> CadastrarImovel(Imovel imovel)
        {
            var json = JsonSerializer.Serialize(imovel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("imovel/cadastrar", content);
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
