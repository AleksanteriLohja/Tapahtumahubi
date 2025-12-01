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

- Tapahtumien mallinnus (`Event`)
- Osallistujien mallinnus (`Participant`)
- Tietokantakerros EF Coren ja SQLite-tietokannan avulla
- Tapahtumien listauksen logiikka (`MainPageViewModel`)
- Uuden tapahtuman luomisen logiikka (`NewEventPageViewModel`)
- Peruspalvelut tapahtumien ja osallistujien hakemiseen ja tallentamiseen
- Lokitus virhetilanteissa (`ILogger`)

UI-taso käyttää ViewModel-kerrosta eikä ole suorassa riippuvuudessa tietokantaan.

---

## Teknologiat

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- .NET MAUI (WinUI, `net8.0-windows10.0.19041.0`)
- Entity Framework Core + SQLite
- xUnit (yksikkö- ja integraatiotestit)
- Microsoft.Extensions.Logging
- GitHub Actions (CI)
- Dependabot

---

## Projektirakenne

```text
.
├── .github/
│   └── workflows/
│       └── ci.yml
├── docs/
│   ├── project-card.md
│   └── vaatimukset.md
├── src/
│   ├── Tapahtumahubi.App/
│   │   ├── ViewModels/
│   │   │   ├── BaseViewModel.cs
│   │   │   ├── MainPageViewModel.cs
│   │   │   └── NewEventPageViewModel.cs
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

Event – tapahtuma (nimi, ajankohta, kuvaus, …)

Participant – osallistuja (liitettävissä tapahtumiin)

Domain ei tunne tietokantaa, UI:ta tai infrastruktuuria.


Infrastructure

AppDbContext – EF Core -konteksti

Migrations – tietokantamigraatiot

AppDbContextSeed – kehitysdatan alustaminen

Queries/Services – domain-rajapintojen toteutuksia

Integraatiotesteissä käytetään SQLite in-memory -kantaa.


App (MAUI)

ViewModelit: MainPageViewModel, NewEventPageViewModel

ViewModelit testattavissa ilman UI-riippuvuuksia (DI MauiProgram.cs:ssa)


Tests

Domain- ja Infrastructure-testit

Integraatiotestit (SQLite in-memory)

ViewModel-testit (keskeiset polut, virhepolut, tilamuutokset)


Kehitysympäristö

Esivaatimukset

Windows 10 (19041) tai uudempi

.NET 8 SDK

(Suositus) Visual Studio 2022 tai VS Code + C#-laajennus

Rakentaminen & ajo (Windows)

dotnet build src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0
dotnet run  --project src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0

Huom: Ratkaisun (.sln) rakentaminen voi yrittää kääntää myös Android-targetin
(JDK 21). Yllä olevat komennot riittävät tämänhetkiseen käyttöön.


Testit & kattavuus

Paikallisesti

cd src/Tapahtumahubi.Tests
dotnet test --settings ..\..\coverage.runsettings


Tällä hetkellä:

Passed: 35 / Failed: 0 / Skipped: 0

CI (GitHub Actions)

Rakentaa Domain, Infrastructure, Tests

Ajaa xUnit-testit ja kerää Cobertura-kattavuuden (artefaktit: test-results, coverage)

MAUI App ei kuulu CI-buildiin


Dokumentaatio

docs/vaatimukset.md – vaatimukset

docs/project-card.md – projektikortti

GitHub Issues/Project – sprintit, issuet ja DoD


Jatkokehitysideoita

Android-targetin buildin korjaus (JDK 21) ja mobiiliajo

UI:n täydentäminen (osallistujien hallinta, näkymät)

Haku- ja suodatustoiminnat listaukseen

Lisää testejä reunatapauksiin

Lokituksen ja virheenkäsittelyn yhtenäistäminen


Lisenssi

Katso LICENSE