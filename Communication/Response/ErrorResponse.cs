using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Communication.Response
{
    public class ErrorResponse
    {
        public string Erros { get; set; } = string.Empty;
    }
}
