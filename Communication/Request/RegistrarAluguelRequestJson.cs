using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Communication.Request
{
    public class RegistrarAluguelRequestJson
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public Guid UsuarioId { get; set; }
        public Guid ImovelId { get; set; }
    }
}
