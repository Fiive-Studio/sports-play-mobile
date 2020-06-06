using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Model
{
    public class AuthCustomerModel
    {
        public string Token { get; set; }
        public string ExpiresIn { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string IdUser { get; set; }
        public string UserName { get; set; }
        public string IdPlace { get; set; }
    }
}
