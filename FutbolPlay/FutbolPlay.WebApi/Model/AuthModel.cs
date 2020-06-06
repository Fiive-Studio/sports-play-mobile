using FutbolPlay.WebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Model
{
    public class AuthModel
    {
        public string Token { get; set; }
        public string ExpiresIn { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string IdUser { get; set; }
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string IdSocialNetwork { get; set; }
        public AuthType AuthType { get; set; }
    }
}
