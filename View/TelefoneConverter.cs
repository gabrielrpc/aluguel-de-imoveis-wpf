using System;
using System.Globalization;
using System.Windows.Data;

namespace aluguel_de_imoveis_wpf.Converters
{
    public class TelefoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string telefone && !string.IsNullOrWhiteSpace(telefone))
            {
                telefone = new string(telefone.Where(char.IsDigit).ToArray());
                if (telefone.Length == 11)
                    return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 1)} {telefone.Substring(3, 4)}-{telefone.Substring(7, 4)}";
                else if (telefone.Length == 10)
                    return $"({telefone.Substring(0, 2)}) {telefone.Substring(2, 4)}-{telefone.Substring(6, 4)}";
                else if (telefone.Length == 8)
                    return $"{telefone.Substring(0, 4)}-{telefone.Substring(4, 4)}";
            }
            return "Telefone inválido";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}