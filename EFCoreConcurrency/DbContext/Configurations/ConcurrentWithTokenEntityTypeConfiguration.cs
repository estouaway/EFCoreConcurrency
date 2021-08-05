using EFCoreConcurrency.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreConcurrency.DbContext.Configurations
{
    internal class ConcurrentWithTokenEntityTypeConfiguration : IEntityTypeConfiguration<ConcurrentWithToken>
    {
        public void Configure(EntityTypeBuilder<ConcurrentWithToken> builder)
        {
            builder.ToTable("ConcurrentWithToken");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Version).IsConcurrencyToken();
        }
    }

    internal class ConcurrentWithTokenEntityTypeConfigurationSqlite : IEntityTypeConfiguration<ConcurrentWithToken>
    {
        public void Configure(EntityTypeBuilder<ConcurrentWithToken> builder)
        {
            builder.ToTable("ConcurrentWithToken");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Version).IsConcurrencyToken();
        }
    }
}
