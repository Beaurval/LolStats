﻿using PocketSummonner.Models.BDD;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace PocketSummonner.Helpers
{
    public static class SaveToDb
    {


        public static Invocateur SaveInvocateur(Invocateur i, DataContext db)
        {
            //Si l'invocateur n'existe pas
            if (!db.Invocateurs.Select(x => x.Id).Contains(i.Id))
            {
                db.Invocateurs.Add(i);
                db.SaveChanges();
            }
            else
                i = db.Invocateurs.Find(i.Id);

            return i;
        }


        public async static Task MajSpells(DataContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {

                List<Sort> sort = await ApiCall.GetSpells();
                db.Sorts.RemoveRange(db.Sorts.ToList());
                db.Sorts.AddRange(sort);
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Sorts ON;");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Sorts OFF");
                db.SaveChanges();
                transaction.Commit();
            }
        }
        public async static Task MajChampions(DataContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                List<Champion> champions = await ApiCall.GetChampions();
                db.Champions.RemoveRange(db.Champions.ToList());
                db.Champions.AddRange(champions);
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Champions ON;");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Champions OFF");
                db.SaveChanges();
                transaction.Commit();
            }
        }
        public async static Task MajEquipemment(DataContext db)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                List<Equipement> equipements = await ApiCall.GetEquipements();

                db.Equipements.RemoveRange(db.Equipements.ToList());
                db.Equipements.AddRange(equipements);
                db.Equipements.Add(new Equipement { Id = 1, Description = "", Nom = "empty", Image = "~/Content/img/assets/empty.png" });
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Equipements ON;");
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Equipements OFF");
                db.SaveChanges();
                transaction.Commit();
            }
        }
    }
}