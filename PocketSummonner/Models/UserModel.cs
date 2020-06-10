using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models
{
    public class UserModel
    {
        [DisplayName("Email")]
        public string Mail { get; set; }
        [DisplayName("Mot de passe")]
        public string Password { get; set; }
    }
}