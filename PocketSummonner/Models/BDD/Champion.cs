using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class Champion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Image { get; set; }
        public string Splash { get; set; }

        //Relation
        public virtual List<Maitrise> Maitrises { get; set; }
    }
}