using System.IO;
using System.Reflection;
using EFCoreConcurrency.DbContext.Configurations;
using EFCoreConcurrency.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreConcurrency.DbContext
{
    public class MyDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public const string DbFileName = @"EfCoreConcurrencyTest.db";
        public DbSet<ConcurrentWithToken> ConcurrentWithToken { get; protected set; }
        public DbSet<ConcurrentWithRowVersion> ConcurrentWithRowVersion { get; protected set; }
        public DbSet<NotConcurrent> NotConcurrent { get; protected set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder = optionsBuilder
                .UseLoggerFactory(Program.MyLoggerFactory)
                .UseSqlite($"Data source={DbFileName}",
                    options => { options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName); });
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Database.IsSqlite())
            {
                modelBuilder.ApplyConfiguration(new ConcurrentWithTokenEntityTypeConfigurationSqlite());
                modelBuilder.ApplyConfiguration(new ConcurrentAccountWithRowVersionEntityTypeConfigurationSqlite());
                modelBuilder.ApplyConfiguration(new NotConcurrentEntityTypeConfigurationSqlite());
            }
            else
            {
                modelBuilder.ApplyConfiguration(new ConcurrentWithTokenEntityTypeConfiguration());
                modelBuilder.ApplyConfiguration(new ConcurrentWithRowVersionEntityTypeConfiguration());
                modelBuilder.ApplyConfiguration(new NotConcurrentEntityTypeConfiguration());
            }
        }

        public static void EnsureDatabaseIsCleaned()
        {
            File.Delete(DbFileName);
        }
    }
}
