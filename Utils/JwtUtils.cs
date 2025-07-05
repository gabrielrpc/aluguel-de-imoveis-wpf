using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace aluguel_de_imoveis_wpf.Utils
{
    public static class JwtUtils
    {
        public static string? GetUserIdFromToken(string token)
        {
            try
            {
                var payload = token.Split('.')[1];
                var jsonBytes = Convert.FromBase64String(PadBase64(payload));
                var json = Encoding.UTF8.GetString(jsonBytes);

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("sub", out var usuarioIdElement))
                {
                    return usuarioIdElement.GetString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string PadBase64(string base64)
        {
            return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        }
    }
}
