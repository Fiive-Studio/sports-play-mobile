using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Model
{
    public class UserModel
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string IdSocialNetwork { get; set; }
        public int IdUserType { get; set; }
    }
}
