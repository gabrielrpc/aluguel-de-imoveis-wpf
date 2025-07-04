using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using aluguel_de_imoveis_wpf.Security;

namespace aluguel_de_imoveis_wpf.Services
{
    public class AuthorizedHttpClient
    {
        protected readonly HttpClient _httpClient;

        public AuthorizedHttpClient()
        {
            _httpClient = new HttpClient();
            var token = TokenStorage.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
