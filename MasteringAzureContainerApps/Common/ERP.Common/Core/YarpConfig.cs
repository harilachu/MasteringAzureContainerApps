using System;
using System.Collections.Generic;
using System.Text;

namespace ERP.Common.Core
{
    public class YarpConfig
    {
        public string YarpApiKey { get; set; }

        public TokenConfig Token { get; set; }
    }

    public class TokenConfig
    {
        public string AppSecret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
