﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGKPortalApp.BusinessObjectLayer.Entities.SiramatikIslemleri;

namespace SGKPortalApp.DataAccessLayer.Configurations.SiramatikIslemleri
{
    public class HubConnectionConfiguration : IEntityTypeConfiguration<HubConnection>
    {
        public void Configure(EntityTypeBuilder<HubConnection> builder)
        {
            builder.ToTable("SIR_HubConnections", "dbo");

            builder.HasKey(hc => hc.HubConnectionId);

            builder.Property(hc => hc.HubConnectionId)
                .ValueGeneratedOnAdd();

            builder.Property(hc => hc.TcKimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(hc => hc.ConnectionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(hc => hc.ConnectionStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(hc => hc.EklenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hc => hc.DuzenlenmeTarihi)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(hc => hc.SilindiMi)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(hc => new { hc.TcKimlikNo, hc.ConnectionId, hc.ConnectionStatus })
                .HasDatabaseName("IX_SIR_HubConnections_Tc_ConnId_Status");

            builder.HasIndex(hc => hc.TcKimlikNo)
                .IsUnique()
                .HasDatabaseName("IX_SIR_HubConnections_TcKimlikNo")
                .HasFilter("[SilindiMi] = 0");

            builder.HasQueryFilter(hc => !hc.SilindiMi);

            builder.HasOne(hc => hc.Personel)
                .WithOne(p => p.HubConnection)
                .HasForeignKey<HubConnection>(hc => hc.TcKimlikNo)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_SIR_HubConnections_PER_Personeller");
        }
    }
}