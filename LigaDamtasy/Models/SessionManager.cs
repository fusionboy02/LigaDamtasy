using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigaDAMtasy.Models
{
    internal static class SessionManager
    {
        public static string Token { get; set; } = string.Empty;
        public static string UserEmail { get; set; } = string.Empty;

        public static void ClearSession()
        {
            Token = string.Empty;
            UserEmail = string.Empty;
        }
    }
}
