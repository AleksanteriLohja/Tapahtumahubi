# Tapahtumahubi

Tapahtumahubi on .NET 8 / .NET MAUI -harjoitusprojekti, jonka tarkoituksena on
mallintaa yksinkertaista tapahtumien ja osallistujien hallintaa. Projekti on
osa ohjelmistotuotannon kurssia ja toteutettu kerrosarkkitehtuurilla:

- **Domain** – ydindomain (Event, Participant)
- **Infrastructure** – tietokanta ja palvelut (EF Core + SQLite)
- **App** – .NET MAUI -käyttöliittymä
- **Tests** – yksikkö- ja integraatiotestit (xUnit)

Pääalusta on tällä hetkellä **Windows-työpöytä**. Android/iOS/MacCatalyst
-targetit ovat olemassa mahdollista jatkokehitystä varten, mutta niitä ei
ole pakko kääntää/ajaa.

---

## Päätoiminnot

Nykyinen toiminnallisuus keskittyy ydindomainiin ja peruspolkuihin:

- Tapahtumien mallinnus (`Event`)
- Osallistujien mallinnus (`Participant`)
- Tietokantakerros EF Coren ja SQLite-tietokannan avulla
- Tapahtumien listauksen logiikka (`MainPageViewModel`)
- Uuden tapahtuman luomisen logiikka (`NewEventPageViewModel`)
- Peruspalvelut tapahtumien ja osallistujien hakemiseen ja tallentamiseen
- Lokitus virhetilanteissa (`ILogger`)

UI-taso käyttää ViewModel-kerrosta eikä ole suorassa riippuvuudessa
tietokantaan.

---

## Teknologiat

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- .NET MAUI (WinUI, `net8.0-windows10.0.19041.0`)
- Entity Framework Core + SQLite
- xUnit (yksikkö- ja integraatiotestit)
- Microsoft.Extensions.Logging
- GitHub + GitHub Issues / Project -taulu (sprinttien hallinta)
- Dependabot (päivitysten seurantaan)

---

## Projektirakenne

```text
.
├── .github/
│   ├── ISSUE_TEMPLATE/
│   └── workflows/
│       └── dependabot.yml
├── docs/
│   ├── project-card.md       # Projektikortti
│   └── vaatimukset.md        # Vaatimusmäärittely
├── src/
│   ├── Tapahtumahubi.App/
│   │   ├── ViewModels/
│   │   │   ├── BaseViewModel.cs
│   │   │   ├── MainPageViewModel.cs
│   │   │   └── NewEventPageViewModel.cs
│   │   ├── *.xaml / *.xaml.cs (sivut: MainPage, NewEventPage, ParticipantsPage, jne.)
│   │   └── Tapahtumahubi.App.csproj
│   ├── Tapahtumahubi.Domain/
│   │   ├── Event.cs
│   │   └── Participant.cs
│   ├── Tapahtumahubi.Infrastructure/
│   │   ├── AppDbContext.cs
│   │   ├── AppDbContextSeed.cs
│   │   ├── DesignTimeDbContextFactory.cs
│   │   ├── Queries/
│   │   │   └── EventQueries.cs
│   │   ├── Services/
│   │   │   └── ParticipantService.cs
│   │   └── Migrations/
│   └── Tapahtumahubi.Tests/
│       ├── Infrastructure/
│       │   └── SqliteInMemoryFixture.cs
│       ├── Integration/
│       │   ├── EventIntegrationTests.cs
│       │   └── MigrationAndSeedTests.cs
│       ├── ViewModels/
│       │   ├── MainPageViewModelTests.cs
│       │   └── NewEventPageViewModelTests.cs
│       ├── EventTests.cs
│       ├── ParticipantTests.cs
│       └── Tapahtumahubi.Tests.csproj
├── coverage.runsettings
├── .editorconfig
├── .gitignore
├── LICENSE
├── README.md
└── Tapahtumahubi.sln

Arkkitehtuuri
Domain

Tapahtumahubi.Domain sisältää vain ydindatan:

Event – tapahtuma, jolla on perustiedot (esim. nimi, ajankohta, kuvaus).

Participant – osallistuja, joka voidaan liittää tapahtumiin.

Domain-kerros ei tunne tietokantaa, UI:ta tai infrastruktuuria.

Infrastructure

Tapahtumahubi.Infrastructure vastaa pysyväistallennuksesta:

AppDbContext – EF Core -konteksti.

Migrations – tietokantamigraatiot.

AppDbContextSeed – kehitysdatan alustaminen.

Queries- ja Services-kansiot – domain-rajapintojen toteutukset
(esim. tapahtumien haku ja osallistujapalvelu).

Integraatiotesteissä käytetään SQLite in-memory -tietokantaa
(SqliteInMemoryFixture).

App (MAUI)

Tapahtumahubi.App sisältää käyttöliittymän ja ViewModelit:

MainPageViewModel

Load-komento hakee tapahtumalistan palvelusta ja asettaa
IsBusy / Items -tilat.

NewEventPageViewModel

Save-komento luo uuden tapahtuman palvelun kautta.

Cancel-komento ei tee muutoksia domainiin.

Virhepolut kirjaavat lokiin (ILogger).

ViewModelit ovat testattavissa ilman UI-riippuvuuksia. DI-konfigurointi
tehdään MauiProgram.cs-tiedostossa.

Tests

Tapahtumahubi.Tests sisältää:

Domain-testit (EventTests, ParticipantTests)

Infrastructure-testit (palvelut ja kyselyt)

Integraatiotestit, joissa tietokantaa ajetaan in-memory SQLite -instanssilla

ViewModel-testit:

MainPageViewModelTests (Load-komento, virhepolut, tilamuutokset)

NewEventPageViewModelTests (Save/Cancel, virhepolut, lokitus)

Testien DoD:

Testit vihreinä

Keskeiset polut katettu

ViewModel-luokille tavoite ≥ 80 % kattavuus tai perusteltu poikkeus

Kehitysympäristö
Esivaatimukset

Windows 10 (19041) tai uudempi

.NET 8 SDK

(Suositeltu) Visual Studio 2022 tai VS Code + C#-laajennus

SQLite-ajurit (EF Core hoitaa käytön koodista)

Android/iOS/MacCatalyst -targetit ovat mukana, mutta kurssivaiheessa
keskitytään Windows-targettiin.

Projektin kääntäminen ja ajaminen
Visual Studio / IDE

Avaa Tapahtumahubi.sln.

Aseta Tapahtumahubi.App käynnistysprojektiksi.

Valitse targetiksi Windows Machine.

Suorita projekti (Run/Debug).

Komentoriviltä (Windows-target)

Projektin kääntäminen:
dotnet build src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0

Sovelluksen ajaminen:
dotnet run --project src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0

Huom: Ratkaisun (Tapahtumahubi.sln) ajaminen dotnet build / dotnet test
-komennolla voi yrittää kääntää myös Android-targetin, joka vaatii JDK 21
-version. Windows-targetin ajaminen yllä olevilla komennoilla riittää
tämänhetkiseen käyttöön.

Testien ajaminen

Yksikkö- ja integraatiotestit:
cd src/Tapahtumahubi.Tests
dotnet test

Tällä hetkellä testejä on:

Passed: 35

Failed: 0

Skipped: 0

Testikattavuus

Ratkaisun juuresta löytyy coverage.runsettings, jonka avulla voi generoida
testikattavuusraportin:

cd src/Tapahtumahubi.Tests
dotnet test --settings ..\..\coverage.runsettings

Raportti tallentuu TestResults-hakemistoon (tarkka polku riippuu ajosta).

Dokumentaatio

docs/vaatimukset.md – sovelluksen vaatimukset ja toiminnalliset tavoitteet

docs/project-card.md – projektikortti (kurssin vaatima kuvaus)

GitHub Issues & Project -taulu – sprintit, issuet ja DoD kullekin tehtävälle

Jatkokehitysideoita

Android-targetin buildin korjaus (JDK 21, mahdollinen mobiilikäyttö)

Käyttöliittymän täydentäminen (osallistujien hallinta, tarkemmat näkymät)

Hakutoiminnot ja suodatus tapahtumalistaukseen

Lisätestit reunatapauksille ja virhetilanteille

Lokituksen ja virheenkäsittelyn yhtenäistäminen koko sovelluksessa

Lisenssi

Projektin lisenssi löytyy tiedostosta LICENSE