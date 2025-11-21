# Tapahtumahubi

![CI](https://github.com/AleksanteriLohja/Tapahtumahubi/actions/workflows/ci.yml/badge.svg)

C# / .NET MAUI -sovellus pienorganisaatioiden tapahtumien hallintaan.  
Kurssiprojekti (ET00BP84 – Ohjelmistotuotanto, Savonia AMK, 2025).

---

## Sisältö

- [Stack](#stack)
- [Kansiorakenne](#kansiorakenne)
- [Kehitysympäristö](#kehitysympäristö)
- [Käynnistys (Windows, Debug)](#käynnistys-windows-debug)
- [Tietokanta & migraatiot](#tietokanta--migraatiot)
- [Testit](#testit)
- [CI (GitHub Actions)](#ci-github-actions)
- [Sovelluksen käyttö](#sovelluksen-käyttö)
- [Tietomalli](#tietomalli)
- [Lisenssi](#lisenssi)

---

## Stack

- .NET 8
- .NET MAUI (Windows)
- EF Core + SQLite (paikallinen tiedosto `events.db`)
- xUnit (yksikkötestit)

---

## Kansiorakenne

```txt
Tapahtumahubi/
├─ .github/
│  └─ workflows/
│     └─ ci.yml                       # GitHub Actions (build + test)
├─ docs/                               # Projektikortti, vaatimusmäärittely, yms.
├─ src/
│  ├─ Tapahtumahubi.App/               # MAUI UI + DI + navigointi (startup)
│  ├─ Tapahtumahubi.Domain/            # Domain-mallit (Event, Participant)
│  ├─ Tapahtumahubi.Infrastructure/    # EF Core DbContext + migraatiot
│  └─ Tapahtumahubi.Tests/             # xUnit-yksikkötestit (Domain)
├─ .gitignore
├─ LICENSE
├─ README.md
└─ Tapahtumahubi.sln

---

Kehitysympäristö

Windows 10/11

.NET SDK 8.0

Visual Studio 2022 tai VS Code

---

Käynnistys (Windows, Debug)

Vaihtoehto A: build + exe

# 1) Rakenna MAUI Windowsille (Debug, unpackaged)
dotnet build -c Debug -f net8.0-windows10.0.19041.0 .\src\Tapahtumahubi.App\Tapahtumahubi.App.csproj

# 2) Aja binaari
.\src\Tapahtumahubi.App\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\Tapahtumahubi.App.exe

Vaihtoehto B: dotnet run

dotnet run -c Debug -f net8.0-windows10.0.19041.0 --project .\src\Tapahtumahubi.App\Tapahtumahubi.App.csproj

Huom. Debugissa sovellus ajetaan “unpackaged”-tilassa, mikä helpottaa kehitystä Windowsissa.

---

Tietokanta & migraatiot

Tietokanta: SQLite

Oletussijainti:

%LOCALAPPDATA%\events.db

Sovellus ajaa käynnistyksessä db.Database.Migrate() → puuttuvat migraatiot ajetaan automaattisesti.

---

EF Core -työkalut (tarvittaessa):

dotnet tool install --global dotnet-ef
dotnet tool update  --global dotnet-ef

---

Migraation luonti (kun skeemaa muutetaan):

# Suorita repo-juuresta
dotnet ef migrations add InitialCreate `
  --project .\src\Tapahtumahubi.Infrastructure `
  --startup-project .\src\Tapahtumahubi.App `
  --framework net8.0-windows10.0.19041.0 `
  --output-dir Migrations

---

Tietokannan päivittäminen:

dotnet ef database update `
  --project .\src\Tapahtumahubi.Infrastructure `
  --startup-project .\src\Tapahtumahubi.App `
  --framework net8.0-windows10.0.19041.0
  
---

Testit
# (Valinnainen) Siivoa testiprojektin build-artefaktit
Remove-Item .\src\Tapahtumahubi.Tests\bin -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item .\src\Tapahtumahubi.Tests\obj -Recurse -Force -ErrorAction SilentlyContinue

# Palauta paketit ja suorita testit (Release)
dotnet restore
dotnet build .\src\Tapahtumahubi.Tests\Tapahtumahubi.Tests.csproj -c Release
dotnet test  .\src\Tapahtumahubi.Tests\Tapahtumahubi.Tests.csproj -c Release `
  --logger "trx;LogFileName=test-results.trx"


TRX-raportti: src/Tapahtumahubi.Tests/TestResults/test-results.trx
  
---

CI (GitHub Actions)

Työnkulku: .github/workflows/ci.yml

Käynnistyy push / pull_request haaraan main

Asentaa .NET 8, tekee restore, buildaa Domain, Infrastructure, Tests (MAUI-appia ei ajeta CI:ssä)

Ajaa xUnit-testit ja julkaisee raportit artefakteina

Status näkyy README:n yläosan badge-kuvakkeessa.
  
---

Sovelluksen käyttö

Tapahtumien lista: Etusivulla kaikki tapahtumat aikajärjestyksessä. Haku suodattaa otsikon ja paikan perusteella.

Uusi tapahtuma: Uusi tapahtuma → täytä kentät → Tallenna.

Muokkaus/poisto: pyyhkäise listariviltä vasemmalle → Muokkaa tai Poista.
  
---

Tietomalli

Event

int Id

string Title

DateTime StartTime

string Location

string? Description

int MaxParticipants

List<Participant> Participants
  
---

Participant

int Id

string Name

string Email

int EventId (FK)

### Debug-seed
Debug-ajoissa sovellus lisää 2–3 demotapahtumaa, jos tietokanta on tyhjä.
Toteutus: `AppDbContextSeed.Seed()` (kutsutaan `MauiProgram.cs`issa).

## Lataa release (Windows)
Viimeisimmät Windows-asennuspaketit (.msix/.msixbundle) löytyvät GitHubin **Releases**-osiosta tai suoraan Actions-artifakteina tagien ajosta.

- Windows 10/11: lataa uusin MSIX, suorita ja hyväksy asennus (tarvittaessa "Sideload apps" on oltava sallittu).

