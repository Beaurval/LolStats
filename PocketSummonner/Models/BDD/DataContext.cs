using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PocketSummonner.Models.BDD
{
    public class DataContext : DbContext
    {
        public DataContext() : base("LolStatDb")
        {

        }

        public DbSet<Invocateur> Invocateurs { get; set; }
        public DbSet<Equipement> Equipements { get; set; }
        public DbSet<Partie> Parties { get; set; }
        public DbSet<Joueur> Joueurs { get; set; }

    }
}