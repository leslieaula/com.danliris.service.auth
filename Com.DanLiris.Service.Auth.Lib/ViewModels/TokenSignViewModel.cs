using System.Collections.Generic;
using System.Dynamic;

namespace Com.DanLiris.Service.Auth.Lib.ViewModels
{
    public class TokenSignViewModel
    {
        public string username { get; set; }
        public Dictionary<string, object> permission { get; set; }
        public TokenSignProfileViewModel profile { get; set; }
    }

    public class TokenSignProfileViewModel
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string gender { get; set; }
    }
}
