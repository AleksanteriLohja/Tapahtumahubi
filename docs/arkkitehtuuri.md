# Arkkitehtuuri

## Yleiskuva
Tapahtumahubi on .NET MAUI -sovellus, jossa on selkeä kerrosjako:

- **App (UI & MVVM)** – MAUI-näkymät, ViewModelit, navigointi ja DI-konfigurointi.
- **Domain** – entiteetit ja liiketoimintasäännöt. Ei riippuvuuksia kehyksiin.
- **Infrastructure** – EF Core, repositoriot ja integraatiot.

Ratkaisurakenne:
src/
├─ Tapahtumahubi.App/
├─ Tapahtumahubi.Domain/
├─ Tapahtumahubi.Infrastructure/
└─ Tapahtumahubi.Tests/


## MVVM
- **View**: XAML-sivut (esim. `MainPage`).
- **ViewModel**: tilanhallinta, komennot, validointi.
- **Model**: Domain-entiteetit (`Event` jne.).

## Pysyvyys
- EF Core (SQLite devissä).
- Migraatiot versionhallinnassa; skeemamuutokset PR:ien kautta.

## DI
`MauiProgram.CreateMauiApp()`:
- `AddDbContext<...>()`
- `AddScoped<IEventRepository, EventRepository>()`
- `AddTransient<ViewModelit, Viewit>()`

## Validointi
- Domain-invariantit (päivämäärät, kapasiteetti).
- Virheviestit poikkeuksina/tulosolioina.

## Testattavuus ja julkaisu
- Unit-testit ViewModel/Domain, SQLite-in-memory infra-testeihin.
- Ensijulkaisu: Windows (MSIX myöhemmin).