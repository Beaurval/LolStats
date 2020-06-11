using PocketSummonner.Migrations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DbSet<Maitrise> Maitrises { get; set; }
        public DbSet<UserSawInvocateur> Vues { get; set; }


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

            modelBuilder.Entity<UserSawInvocateur>()
                .HasKey(ui => new { ui.UserId, ui.InvocateurId });

            modelBuilder.Entity<UserSawInvocateur>()
                .HasRequired(ui => ui.User).WithMany(i => i.Historique).HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<UserSawInvocateur>()
                .HasRequired(ui => ui.Invocateur).WithMany(i => i.ConsulteParUser).HasForeignKey(ui => ui.InvocateurId);

        }

        public System.Data.Entity.DbSet<PocketSummonner.Models.BDD.User> Users { get; set; }
    }
}