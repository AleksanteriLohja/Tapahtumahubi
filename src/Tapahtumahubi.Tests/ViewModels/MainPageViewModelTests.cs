using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Tapahtumahubi.App.ViewModels;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;
using Xunit;

namespace Tapahtumahubi.Tests.ViewModels
{
    public class MainPageViewModelTests
    {
        [Fact]
        public async Task InitializeAsync_LataaTapahtumatJaPaivittaaTilat()
        {
            // Arrange
            using var factory = new TestDbContextFactory();

            var now = DateTime.Now;

            // Seedataan testidataa
            using (var db = await factory.CreateDbContextAsync())
            {
                db.Events.AddRange(
                    new Event
                    {
                        Title = "Rock-ilta",
                        Location = "Kuopio",
                        StartTime = now.AddDays(1),
                        MaxParticipants = 100
                    },
                    new Event
                    {
                        Title = "Jooga-aamu",
                        Location = "Helsinki",
                        StartTime = now.AddDays(2),
                        MaxParticipants = 20
                    }
                );
                await db.SaveChangesAsync();
            }

            var vm = new MainPageViewModel(factory);

            // Ennen latausta lista on tyhjä
            Assert.True(vm.IsListEmpty);
            Assert.False(vm.HasItems);

            // Act
            await vm.InitializeAsync(); // kutsuu LoadAsync-komentoa

            // Assert
            Assert.False(vm.IsBusy);
            Assert.True(vm.HasItems);
            Assert.False(vm.IsListEmpty);
            Assert.Equal(2, vm.Events.Count);
        }

        [Fact]
        public async Task InitializeAsync_KunDbFactoryHeittaa_IsBusyPalautuuFalseen()
        {
            // Arrange
            var factoryMock = new Mock<IDbContextFactory<AppDbContext>>();
            factoryMock
                .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Testivirhe"));

            var vm = new MainPageViewModel(factoryMock.Object);

            // Act + Assert – varsinainen virhe saa levitä ulos
            await Assert.ThrowsAsync<InvalidOperationException>(() => vm.InitializeAsync());

            // Mutta IsBusy palautuu falseksi finally-lohkossa
            Assert.False(vm.IsBusy);
            Assert.True(vm.IsListEmpty);
        }

        [Fact]
        public async Task SearchText_SuodattaaTapahtumatOtsikonTaiSijainninPerusteella()
        {
            // Arrange
            using var factory = new TestDbContextFactory();

            var now = DateTime.Now;

            using (var db = await factory.CreateDbContextAsync())
            {
                db.Events.AddRange(
                    new Event
                    {
                        Title = "Rock-ilta Savoniassa",
                        Location = "Kuopio",
                        StartTime = now.AddDays(1),
                        MaxParticipants = 50
                    },
                    new Event
                    {
                        Title = "Jooga-aamu",
                        Location = "Helsinki",
                        StartTime = now.AddDays(2),
                        MaxParticipants = 20
                    }
                );
                await db.SaveChangesAsync();
            }

            var vm = new MainPageViewModel(factory);

            await vm.InitializeAsync();
            Assert.Equal(2, vm.Events.Count); // varmistetaan lähtötilanne

            // Act – haku "rock" pitäisi jättää listaan vain rock-tapahtuman
            vm.SearchText = "rock";

            // Assert
            Assert.Single(vm.Events);
            Assert.Equal("Rock-ilta Savoniassa", vm.Events[0].Title);
        }

        /// <summary>
        /// Yksinkertainen in-memory SQLite -tehdas, joka toteuttaa IDbContextFactory&lt;AppDbContext&gt;.
        /// Jokainen CreateDbContext(_Async) -kutsu saa uuden AppDbContextin samaan in-memory-kantaan.
        /// </summary>
        private sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>, IDisposable
        {
            private readonly SqliteConnection _connection;
            private readonly DbContextOptions<AppDbContext> _options;

            public TestDbContextFactory()
            {
                _connection = new SqliteConnection("Filename=:memory:");
                _connection.Open();

                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite(_connection)
                    .Options;

                using var context = new AppDbContext(_options);
                context.Database.EnsureCreated();
            }

            public AppDbContext CreateDbContext()
                => new AppDbContext(_options);

            public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
                => Task.FromResult(new AppDbContext(_options));

            public void Dispose()
            {
                _connection.Dispose();
            }
        }
    }
}