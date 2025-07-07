using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace aluguel_de_imoveis_wpf.Converters
{
    public class TelefoneFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var numero = value?.ToString();

            if (string.IsNullOrWhiteSpace(numero))
                return string.Empty;

            var digitos = new string(numero.Where(char.IsDigit).ToArray());

            if (digitos.Length == 11)
                return $"({digitos[..2]}) {digitos[2]} {digitos.Substring(3, 4)}-{digitos[7..]}";
            else if (digitos.Length == 10)
                return $"({digitos[..2]}) {digitos.Substring(2, 4)}-{digitos[6..]}";

            return numero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new string(value?.ToString()?.Where(char.IsDigit).ToArray());
        }
    }
}
