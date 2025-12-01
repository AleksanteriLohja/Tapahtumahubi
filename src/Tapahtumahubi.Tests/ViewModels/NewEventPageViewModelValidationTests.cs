using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Tapahtumahubi.App.ViewModels;
using Tapahtumahubi.Infrastructure;
using Xunit;
using System.Threading;

namespace Tapahtumahubi.Tests.ViewModels
{
    public class NewEventPageViewModelValidationTests
    {
        [Fact]
        public async Task SaveAsync_EiSalliMennyttaPaivaa()
        {
            using var factory = new TestDbContextFactory();

            var vm = new NewEventPageViewModel(factory)
            {
                TitleText = "Mennyt",
                Location  = "X",
                EventDate = DateTime.Today.AddDays(-1),
                EventTime = new TimeSpan(12,0,0),
                MaxParticipants = 10
            };

            try { await InvokePrivateAsync(vm, "SaveAsync"); } catch (NullReferenceException) { }

            using var db = await factory.CreateDbContextAsync();
            Assert.Equal(0, await db.Events.CountAsync());
        }

        [Fact]
        public async Task SaveAsync_EiSalliLiianSuurtaOsallistujamaaraa()
        {
            using var factory = new TestDbContextFactory();

            var vm = new NewEventPageViewModel(factory)
            {
                TitleText = "Iso",
                Location  = "Y",
                EventDate = DateTime.Today.AddDays(1),
                EventTime = new TimeSpan(18,0,0),
                MaxParticipants = 999999 // ylittää rajan
            };

            try { await InvokePrivateAsync(vm, "SaveAsync"); } catch (NullReferenceException) { }

            using var db = await factory.CreateDbContextAsync();
            Assert.Equal(0, await db.Events.CountAsync());
        }

        private static async Task InvokePrivateAsync(NewEventPageViewModel vm, string method)
        {
            var m = typeof(NewEventPageViewModel).GetMethod(method, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var task = (Task)m!.Invoke(vm, null)!;
            await task;
        }

        private sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>, IDisposable
        {
            private readonly SqliteConnection _c;
            private readonly DbContextOptions<AppDbContext> _opt;
            public TestDbContextFactory()
            {
                _c = new SqliteConnection("Filename=:memory:");
                _c.Open();
                _opt = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_c).Options;
                using var ctx = new AppDbContext(_opt);
                ctx.Database.EnsureCreated();
            }
            public AppDbContext CreateDbContext() => new AppDbContext(_opt);
            public Task<AppDbContext> CreateDbContextAsync(CancellationToken t = default) => Task.FromResult(new AppDbContext(_opt));
            public void Dispose() => _c.Dispose();
        }
    }
}