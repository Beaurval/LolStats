using PocketSummonner.Migrations;
using System.Data.Entity;

namespace PocketSummonner.Models.BDD
{
    public class DataContext : DbContext
    {
        public DataContext() : base("LolStatDb")
        {

        }

        public DbSet<Invocateur> Invocateurs { get; set; }
        public DbSet<Champion> Champions { get; set; }
        public DbSet<Equipement> Equipements { get; set; }
        public DbSet<Partie> Parties { get; set; }
        public DbSet<Joueur> Joueurs { get; set; }
        public DbSet<Sort> Sorts { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
            
            modelBuilder.Entity<Equipement>()
                .HasMany<Joueur>(j => j.Joueurs)
                .WithMany(e => e.Equipements)
                .Map(je =>
                {
                    je.MapLeftKey("EquipementId");
                    je.MapRightKey("JoueurId");
                    je.ToTable("EquipementJoueur");
                });


        }
    }
}