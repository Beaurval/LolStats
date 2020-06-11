using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models
{
    public class UserModel
    {
        [DisplayName("Email")]
        public string Mail { get; set; }
        [DataType(DataType.Password), DisplayName("Mot de passe")]
        public string Password { get; set; }
    }
}