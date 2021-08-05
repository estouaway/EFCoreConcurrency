using EFCoreConcurrency.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCoreConcurrency.DbContext.Configurations
{
    internal class NotConcurrentEntityTypeConfigurationSqlite : IEntityTypeConfiguration<NotConcurrent>
    {
        public void Configure(EntityTypeBuilder<NotConcurrent> builder)
        {
            builder.ToTable("NotConcurrent");
            builder.HasKey(x => x.Id);
        }
    }

    internal class NotConcurrentEntityTypeConfiguration : IEntityTypeConfiguration<NotConcurrent>
    {
        public void Configure(EntityTypeBuilder<NotConcurrent> builder)
        {
            builder.ToTable("NotConcurrent");
            builder.HasKey(x => x.Id);
        }
    }
}
