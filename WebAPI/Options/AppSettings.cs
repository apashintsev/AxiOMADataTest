using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Options
{
    public class AppSettings
    {
        public string AuthenticationJwtSecret { get; set; }
        public string AuthenticationJwtSubject { get; set; }
        public string AuthenticationJwtIssuer { get; set; }
        public string AuthenticationJwtAudience { get; set; }
        public string AuthenticationGoogleClientId { get; set; }
    }
}
