# Tapahtumahubi

[![CI](https://github.com/AleksanteriLohja/Tapahtumahubi/actions/workflows/ci.yml/badge.svg)](https://github.com/AleksanteriLohja/Tapahtumahubi/actions/workflows/ci.yml)

Yksinkertainen mutta tuotantotapainen tapahtumahallinnan sovellus (.NET 8, .NET MAUI/Windows, SQLite, EF Core, MVVM).

![Etusivu](docs/images/etusivu.png)
![Kalenteri](docs/images/kalenteri.png)

---

## Sisällys
- [Arkkitehtuuri](#arkkitehtuuri)
- [Vaatimukset ja asennus](#vaatimukset-ja-asennus)
- [Ajaminen (Windows)](#ajaminen-windows)
- [Tietokanta ja migraatiot](#tietokanta-ja-migraatiot)
- [Lokitus](#lokitus)
- [Laatu: koodi, testit ja tyyli](#laatu-koodi-testit-ja-tyyli)
- [Projektinhallinta ja dokumentaatio](#projektinhallinta-ja-dokumentaatio)
- [Julkaisu (valinnainen)](#julkaisu)
- [Tunnetut rajoitteet ja jatkokehitys](#tunnetut-rajoitteet-ja-jatkokehitys)
- [Lisenssi](#lisenssi)

## Arkkitehtuuri

**Kerrokset**
- **Tapahtumahubi.Domain** – entiteetit ja validoinnit (`Event`, `Participant`).
- **Tapahtumahubi.Infrastructure** – EF Core + SQLite: `AppDbContext`, migraatiot (`Migrations/`), siemendata, kyselyt. Käyttää `IDbContextFactory<AppDbContext>`.
- **Tapahtumahubi.App** – .NET MAUI UI (MVVM): ViewModelit ja XAML-näkymät. DI-rekisteröinti `MauiProgram.cs`:ssä. Serilog lokitukseen.
- **Tapahtumahubi.Tests** – xUnit-testit domainille ja peruspoluille.

**UX**
- Välilehdet: **Tapahtumat** ja **Kalenteri**.
- Uusi/Muokkaa: otsikko, sijainti, päivä, aika, kuvaus, maks. osallistujat.
- Osallistujat: sähköposti uniikki per tapahtuma; kapasiteettiraja huomioidaan.

## Vaatimukset ja asennus

- Windows 10 2004 / 11 (build 19041+)
- .NET 8 SDK (ja Desktop Runtime)
- (Suositus) EF Core Tools:
  ```bash
  dotnet tool install --global dotnet-ef


# Kloonaus ja palautus
git clone https://github.com/AleksanteriLohja/Tapahtumahubi.git
cd Tapahtumahubi
dotnet restore

# Ajaminen (Windows)
dotnet build src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0
dotnet run --project src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0

Ensimmäinen ajo luo paikallisen SQLite-tietokannan:
%LocalAppData%\Tapahtumahubi.App\app.db

# Tietokanta ja migraatiot
Migraatiot: src/Tapahtumahubi.Infrastructure/Migrations.
# Aja viimeisin migraatio
dotnet ef database update \
  --project src/Tapahtumahubi.Infrastructure \
  --startup-project src/Tapahtumahubi.App

# Luo uusi migraatio
dotnet ef migrations add <Nimi> \
  --project src/Tapahtumahubi.Infrastructure \
  --startup-project src/Tapahtumahubi.App

Sovellus käyttää IDbContextFactory<AppDbContext> ja tallentaa tietokannan LocalApplicationData-kansioon.

# Lokitus
Serilog kirjoittaa lokit:
%LocalAppData%\Tapahtumahubi.App\logs\app-<päivä>.log

Laatu: koodi, testit ja tyyli
# Käännös
dotnet build -c Release

# Testit + kattavuus
dotnet test --settings coverage.runsettings

Kattavuus: Coverlet (XPlat Code Coverage) + ReportGenerator (CI tuottaa HTML-raportin).

Varoitukset on nostettu virheiksi Directory.Build.props -tiedostossa.

Koodityyli: .editorconfig.

# Projektinhallinta ja dokumentaatio

Projektikortti: docs/project-card.md

Vaatimukset: docs/vaatimukset.md

Arkkitehtuuri: docs/arkkitehtuuri.md

Testaus: docs/testaus.md

Käyttöohje: docs/kayttoohje.md

Git & release -käytännöt: docs/git_ja_release.md

Riskit ja rajaukset: docs/riskit_ja_rajaus.md

Projektisuunnitelma (roolit, sprintit, DoD): docs/projektisuunnitelma.md

AI-käyttö: docs/AI-kaytto.md

# Julkaisu
Unpackaged (kansioon)

PowerShell (yksi rivi):
dotnet publish src/Tapahtumahubi.App -c Release -f net8.0-windows10.0.19041.0 -r win10-x64 -o publish/win

PowerShell (monirivinen, käyttäen ^):
dotnet publish src/Tapahtumahubi.App ^
  -c Release ^
  -f net8.0-windows10.0.19041.0 ^
  -r win10-x64 ^
  -o publish/win

  HUOM: Androidin JDK-virheilmoitukset eivät liity tähän, kun rakennat vain Windows-TFM:lle.

MSIX (pipeline-esimerkki ja ohjeet: docs/git_ja_release.md)

# Tunnetut rajoitteet ja jatkokehitys

Projekti optimoitu Windowsille; muut alustat skelettona.

Jatkokehitys: suodattimet/haku, CSV/ICS-vienti, osallistujahallinnan laajennus, VM-virhepolkujen testit.

# Lisenssi

MIT