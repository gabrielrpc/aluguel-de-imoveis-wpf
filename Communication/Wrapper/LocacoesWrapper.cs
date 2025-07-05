using aluguel_de_imoveis_wpf.Communication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Communication.Wrapper
{
    public class LocacoesWrapper
    {
        public List<ListarLocacaoResponseJson?> Locacoes { get; set; } = new();
    }
}
