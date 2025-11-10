# Tapahtumahubi

![CI](https://github.com/AleksanteriLohja/Tapahtumahubi/actions/workflows/ci.yml/badge.svg)

C# / .NET MAUI -sovellus pienorganisaatioiden tapahtumien hallintaan.  
Kurssiprojekti (ET00BP84 – Ohjelmistotuotanto, Savonia AMK, 2025).

## Stack
- .NET 8
- .NET MAUI (Windows)
- EF Core + SQLite (paikallinen tiedosto `events.db`)

## Kehitysympäristö
- Windows 10/11
- .NET SDK 8.0
- Visual Studio 2022 / VS Code

## Käynnistys (Windows, Debug)
```powershell
dotnet build -f net8.0-windows10.0.19041.0 .\src\Tapahtumahubi.App\Tapahtumahubi.App.csproj
.\src\Tapahtumahubi.App\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\Tapahtumahubi.App.exe