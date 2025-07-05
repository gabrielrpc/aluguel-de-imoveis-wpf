using aluguel_de_imoveis.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Communication.Response
{
    public class ListarLocacaoResponseJson
    {
        public Guid Id { get; set; }

        public string DiasEmAndamentoTratado { get; set; } = string.Empty;
        public string DiasRestantesTratado { get; set; } = string.Empty;

        public decimal ValorFinal { get; set; }
        public int DiasEmAndamento { get; set; }
        public int DiasRestantes { get; set; }

        public TipoImovel TipoImovel { get; set; }

        public string NomeProprietario { get; set; } = string.Empty;
        public string EmailProprietario { get; set; } = string.Empty;
    }
}
