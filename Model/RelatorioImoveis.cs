using aluguel_de_imoveis.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Model
{
    public class RelatorioImoveis
    {
        public string Proprieario { get; set; } = string.Empty;
        public string Contato { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string TituloImovel { get; set; } = string.Empty;
        public TipoImovel Tipo { get; set; }
        public decimal ValorAluguel { get; set; }

    }
}
