using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Communication.Response
{
    public class LoginResponseJson
    {
        public string Token { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
}
