using Microsoft.EntityFrameworkCore;
using SGKPortalApp.BusinessObjectLayer.Entities.Legacy;

namespace SGKPortalApp.DataAccessLayer.Context
{
    public class MysqlDbContext : DbContext
    {
        public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options)
        {
        }

        public DbSet<LegacyKullanici> Kullanicilar { get; set; }
        public DbSet<LegacyBirim> Birimler { get; set; }
        public DbSet<LegacyServis> Servisler { get; set; }
        public DbSet<LegacyUnvan> Unvanlar { get; set; }
        public DbSet<LegacyBina> Binalar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LegacyKullanici>().ToTable("kullanici");
            modelBuilder.Entity<LegacyBirim>().ToTable("birim");
            modelBuilder.Entity<LegacyServis>().ToTable("servisler");
            modelBuilder.Entity<LegacyUnvan>().ToTable("unvanlar");
            modelBuilder.Entity<LegacyBina>().ToTable("binalar");
        }
    }
}
