using aluguel_de_imoveis_wpf.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using aluguel_de_imoveis_wpf.Utils;

namespace aluguel_de_imoveis_wpf.View
{
    public class UsuarioVisibilidadeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var usuarioIdDoImovel = (Guid)value;

            var usuarioLogadoId = JwtUtils.GetUserIdFromToken(TokenStorage.GetToken());

            if (usuarioIdDoImovel.ToString() == usuarioLogadoId)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
