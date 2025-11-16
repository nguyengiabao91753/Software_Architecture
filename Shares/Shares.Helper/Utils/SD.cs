using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Helper.Utils
{
    public class SD
    {

        public static string AuthAPIBase { get; set; }

        public static string DocumentAPIBase { get; set; }

        public const string TokenCookie = "JWTToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE

        }

        public enum FileAccess
        {
            Read,
            Dowload,
            All

        }
    }
}
