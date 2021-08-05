using System;
using System.Threading;
using System.Threading.Tasks;
using EFCoreConcurrency.DbContext;
using EFCoreConcurrency.Models;
using EFCoreConcurrency.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCoreConcurrency
{
    internal class Program
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning)
                    .AddConsole();
            });

        private static async Task Main()
        {
            MyDbContext.EnsureDatabaseIsCleaned();
            using (var dbContext = new MyDbContext())
            {
                dbContext.Database.Migrate();
                if (dbContext.Database.IsSqlite())
                {
                    await dbContext.Database.ExecuteSqlRawAsync(
                       @"
                            CREATE TRIGGER SetTimestampOnUpdate
                            AFTER UPDATE ON ConcurrentWithRowVersion
                            BEGIN
                                UPDATE ConcurrentWithRowVersion
                                SET Timestamp = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END
                        ");
                    await dbContext.Database.ExecuteSqlRawAsync(
                        @"
                            CREATE TRIGGER SetTimestampOnInsert
                            AFTER INSERT ON ConcurrentWithRowVersion
                            BEGIN
                                UPDATE ConcurrentWithRowVersion
                                SET Timestamp = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END
                        ");
                }
                await dbContext.NotConcurrent.AddAsync(new NotConcurrent { Id = 1, Title = "Artigo 1" });
                await dbContext.ConcurrentWithToken.AddAsync(new ConcurrentWithToken { Id = 1, Title = "Artigo 1" });
                await dbContext.ConcurrentWithRowVersion.AddAsync(new ConcurrentWithRowVersion { Id = 1, Title = "Artigo 1" });
                await dbContext.SaveChangesAsync();
            }

            Console.WriteLine("NOT CONCURRENT TEST");
            await TestWithoutConcurrencyControl();

            Console.WriteLine("\n\nCONCURRENT TEST WITH TOKEN");
            await ConcurrencyControlByConcurrencyToken();

            Console.WriteLine("\n\nCONCURRENT TEST WITH ROW VERSION");
            await ConcurrencyControlByRowVersion();
        }

        private static async Task TestWithoutConcurrencyControl()
        {
            using (var dbContext = new MyDbContext())
            {
                var article = await dbContext.NotConcurrent.FindAsync(1);
                ConsoleUtils.WriteInf($"Article title was: {article.Title}");
            }

            var threads = new Thread[2];
            threads[0] = new Thread(async () =>
            {
                using (var dbContext = new MyDbContext())
                {
                    var article = await dbContext.NotConcurrent.FindAsync(1);
                    var newTitle = "New Title 0";
                    ConsoleUtils.WriteInf($"Trying to change article title to: {newTitle}");
                    article.Title = newTitle;
                    await dbContext.SaveChangesAsync();
                }
            });
            threads[1] = new Thread(async () =>
            {
                using (var dbContext = new MyDbContext())
                {
                    var article = await dbContext.NotConcurrent.FindAsync(1);
                    var newTitle = "New Title 1";
                    ConsoleUtils.WriteInf($"Trying to change article title to: {newTitle}");
                    article.Title = newTitle;
                    await dbContext.SaveChangesAsync();
                }
            });

            foreach (var t in threads)
            {
                t.Start();
            }

            Thread.Sleep(1000); 
            using (var dbContext = new MyDbContext())
            {
                var article = await dbContext.NotConcurrent.FindAsync(1);
                ConsoleUtils.WriteInf($"Article title now is: {article.Title}");
            }
        }

        private static async Task ConcurrencyControlByConcurrencyToken()
        {
            using (var dbContext = new MyDbContext())
            {
                var article = await dbContext.ConcurrentWithToken.FindAsync(1);
                ConsoleUtils.WriteInf($"Article title was: {article.Title}");
            }

            var threads = new Thread[2];
            threads[0] = new Thread(async () =>
            {
                using (var dbContext = new MyDbContext())
                {
                    var article = await dbContext.ConcurrentWithToken.FindAsync(1);
                    var newTitle = "New Title 0";
                    ConsoleUtils.WriteInf($"Trying to change article title to: {newTitle}");
                    article.Title = newTitle;
                    article.Version = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ConsoleUtils.WriteErr(e.Message);
                    }
                }
            });
            threads[1] = new Thread(async () =>
            {
                using (var dbContext = new MyDbContext())
                {
                    var article = await dbContext.ConcurrentWithToken.FindAsync(1);
                    var newTitle = "New Title 1";
                    ConsoleUtils.WriteInf($"Trying to change article title to: {newTitle}");
                    article.Title = newTitle;
                    article.Version = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ConsoleUtils.WriteErr(e.Message);
                    }
                }
            });

            foreach (var t in threads)
            {
                t.Start();
            }

            Thread.Sleep(1000);
            using (var dbContext = new MyDbContext())
            {
                var article = await dbContext.ConcurrentWithToken.FindAsync(1);
                ConsoleUtils.WriteInf($"Article title now is: {article.Title}");
            }
        }

        private static async Task ConcurrencyControlByRowVersion()
        {
            using (var dbContext = new MyDbContext())
            {
                var article = await dbContext.ConcurrentWithRowVersion.FindAsync(1);
                ConsoleUtils.WriteInf($"Article title was: {article.Title}");
            }

            var threads = new Thread[2];
            threads[0] = new Thread(async () =>
            {
                using (var dbContext = new MyDbContext())
                {
                    var article = await dbContext.ConcurrentWithRowVersion.FindAsync(1);
                    var newTitle = "New Title 0";
                    ConsoleUtils.WriteInf($"Trying to change article title to: {newTitle}");
                    article.Title = newTitle;
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ConsoleUtils.WriteErr(e.Message);
                    }
                }
            });
            threads[1] = new Thread(async () =>
            {
                using (var dbContext = new MyDbContext())
                {
                    var article = await dbContext.ConcurrentWithRowVersion.FindAsync(1);
                    var newTitle = "New Title 1";
                    ConsoleUtils.WriteInf($"Trying to change article title to: {newTitle}");
                    article.Title = newTitle;
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        ConsoleUtils.WriteErr(e.Message);
                    }
                }
            });

            foreach (var t in threads)
            {
                t.Start();
            }

            Thread.Sleep(1000);
            using (var dbContext = new MyDbContext())
            {
                var article = await dbContext.ConcurrentWithRowVersion.FindAsync(1);
                ConsoleUtils.WriteInf($"Article title now is: {article.Title}");
            }
        }
    }
}
