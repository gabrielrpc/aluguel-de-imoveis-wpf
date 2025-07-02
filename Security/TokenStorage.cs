using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Security
{
    public static class TokenStorage
    {
        public static void SaveToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), "O token não pode ser nulo ou vazio.");

            Properties.Settings.Default.UsuarioToken = token;
            Properties.Settings.Default.Save();
        }

        public static string GetToken()
        {
            return Properties.Settings.Default.UsuarioToken;
        }

        public static void ClearToken()
        {
            Properties.Settings.Default.UsuarioToken = null;
            Properties.Settings.Default.Save();
        }
    }
}
