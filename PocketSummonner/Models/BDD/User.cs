using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class User
    {
        public int Id { get; set; }
        [DisplayName("Nom et prénom")]
        public string UserName { get; set; }
        [DataType(DataType.Password), DisplayName("Mot de passe")]
        public string Password { get; set; }
        [DataType(DataType.EmailAddress), DisplayName("E-mail")]
        public string Mail { get; set; }
        [DataType(DataType.PhoneNumber), DisplayName("Téléphone")]
        public string Tel { get; set; }
        public virtual Invocateur invocateur { get; set; }
        public virtual List<UserSawInvocateur> Historique { get; set; }
    }
}