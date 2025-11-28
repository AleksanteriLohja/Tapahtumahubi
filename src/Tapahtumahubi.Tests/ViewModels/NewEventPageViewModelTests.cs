using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Tapahtumahubi.App.ViewModels;
using Tapahtumahubi.Domain;
using Tapahtumahubi.Infrastructure;
using Xunit;

namespace Tapahtumahubi.Tests.ViewModels
{
    public class NewEventPageViewModelTests
    {
        [Fact]
        public async Task InitializeAsync_IsEditTrue_LataaTapahtumanTietokannasta()
        {
            // Arrange
            using var factory = new TestDbContextFactory();

            int eventId;
            var now = DateTime.Now.Date.AddHours(18);

            using (var db = await factory.CreateDbContextAsync())
            {
                var ev = new Event
                {
                    Title = "Alkuperäinen tapahtuma",
                    Location = "Savonia",
                    StartTime = now,
                    Description = "Kuvaus",
                    MaxParticipants = 42
                };
                db.Events.Add(ev);
                await db.SaveChangesAsync();
                eventId = ev.Id;
            }

            var vm = new NewEventPageViewModel(factory);
            vm.SetEditingId(eventId);

            // Act
            await vm.InitializeAsync();

            // Assert
            Assert.True(vm.IsEdit);
            Assert.Equal("Alkuperäinen tapahtuma", vm.TitleText);
            Assert.Equal("Savonia", vm.Location);
            Assert.Equal(42, vm.MaxParticipants);
            Assert.Equal(now.Date, vm.EventDate.Date);
            Assert.Equal(now.TimeOfDay, vm.EventTime);
        }

        [Fact]
        public async Task SaveAsync_KelvollinenData_LuoUudenTapahtuman()
        {
            // Arrange
            using var factory = new TestDbContextFactory();

            var vm = new NewEventPageViewModel(factory)
            {
                TitleText = "Uusi kurssitapahtuma",
                Location = "Kuopio",
                EventDate = DateTime.Today,
                EventTime = new TimeSpan(18, 0, 0),
                Description = "Testikuvaus",
                MaxParticipants = 10
            };

            // Act
            try
            {
                // Kutsutaan private SaveAsync-metodia suoraan heijastuksella.
                // Navigointi (Shell.Current.GoToAsync) saattaa aiheuttaa NullReferenceExceptionin
                // testissä – se ohitetaan.
                await InvokePrivateAsync(vm, "SaveAsync");
            }
            catch (NullReferenceException)
            {
                // UI-riippuvuus (Shell.Current) ei ole testin fokus – ok testissä.
            }

            // Assert – tarkistetaan, että tietokantaan on luotu yksi tapahtuma oikeilla arvoilla
            using var db = await factory.CreateDbContextAsync();
            var ev = await db.Events.SingleAsync();

            Assert.Equal("Uusi kurssitapahtuma", ev.Title);
            Assert.Equal("Kuopio", ev.Location);
            Assert.Equal(10, ev.MaxParticipants);
            Assert.Equal(vm.EventDate.Date, ev.StartTime.Date);
            Assert.Equal(vm.EventTime, ev.StartTime.TimeOfDay);
            Assert.Equal("Testikuvaus", ev.Description);
        }

        [Fact]
        public async Task SaveAsync_KunValidointiEpaonnistuu_EiLuoTapahtumaa()
        {
            // Arrange
            using var factory = new TestDbContextFactory();

            var vm = new NewEventPageViewModel(factory)
            {
                TitleText = "", // virhe: otsikko vaaditaan
                Location = "Kuopio",
                MaxParticipants = 10
            };

            // Act
            try
            {
                await InvokePrivateAsync(vm, "SaveAsync");
            }
            catch (NullReferenceException)
            {
                // Shell.Current.DisplayAlert aiheuttaa NullReferencen testissä – ohitetaan.
            }

            // Assert – tietokannassa ei ole yhtään tapahtumaa
            using var db = await factory.CreateDbContextAsync();
            var count = await db.Events.CountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task CancelAsync_EiMuutaTietokantaa()
        {
            // Arrange
            using var factory = new TestDbContextFactory();

            // Luodaan yksi tapahtuma, jonka ei pitäisi muuttua
            using (var db = await factory.CreateDbContextAsync())
            {
                db.Events.Add(new Event
                {
                    Title = "Testi",
                    Location = "Kuopio",
                    StartTime = DateTime.Now.AddDays(1),
                    MaxParticipants = 5
                });
                await db.SaveChangesAsync();
            }

            var vm = new NewEventPageViewModel(factory);

            int beforeCount;
            using (var db = await factory.CreateDbContextAsync())
            {
                beforeCount = await db.Events.CountAsync();
            }

            // Act
            try
            {
                await InvokePrivateAsync(vm, "CancelAsync");
            }
            catch (NullReferenceException)
            {
                // Shell.Current.GoToAsync aiheuttaa NullReferencen – ohitetaan.
            }

            // Assert – rivimäärä kannassa sama kuin ennen Cancelia
            using (var db = await factory.CreateDbContextAsync())
            {
                var afterCount = await db.Events.CountAsync();
                Assert.Equal(beforeCount, afterCount);
            }
        }

        /// <summary>
        /// Kutsuu private async -metodia NewEventPageViewModelistä heijastuksella.
        /// </summary>
        private static async Task InvokePrivateAsync(NewEventPageViewModel vm, string methodName)
        {
            var method = typeof(NewEventPageViewModel).GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
                throw new InvalidOperationException(
                    $"Metodia '{methodName}' ei löytynyt NewEventPageViewModelistä.");

            var task = (Task)method.Invoke(vm, null)!;
            await task;
        }

        /// <summary>
        /// Sama in-memory SQLite -tehdas kuin MainPageViewModel-testeissä, mutta
        /// kopioituna tähän tiedostoon, jotta testit ovat riippumattomia toisistaan.
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