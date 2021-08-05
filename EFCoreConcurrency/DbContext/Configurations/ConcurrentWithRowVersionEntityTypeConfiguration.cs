using EFCoreConcurrency.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreConcurrency.DbContext.Configurations
{
    internal class ConcurrentWithRowVersionEntityTypeConfiguration : IEntityTypeConfiguration<ConcurrentWithRowVersion>
    {
        public void Configure(EntityTypeBuilder<ConcurrentWithRowVersion> builder)
        {
            builder.ToTable("ConcurrentWithRowVersion");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Timestamp).IsRowVersion();
        }
    }

    internal class ConcurrentAccountWithRowVersionEntityTypeConfigurationSqlite : IEntityTypeConfiguration<ConcurrentWithRowVersion>
    {
        public void Configure(EntityTypeBuilder<ConcurrentWithRowVersion> builder)
        {
            builder.ToTable("ConcurrentWithRowVersion");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Timestamp).HasColumnName("Timestamp")
                .HasColumnType("BLOB")
                .IsRowVersion();
        }
    }
}
