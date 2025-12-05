# Käyttöohje (dev & julkaisu)

## Vaatimukset
- Windows 10 19041+ / Windows 11
- .NET SDK 8.0
- (VS Code / Visual Studio 2022)

## Käynnistys dev-tilassa
dotnet restore
dotnet build
dotnet run --project src/Tapahtumahubi.App

## Testit
dotnet test

## MSIX (paikallinen, valinnainen)
dotnet publish src/Tapahtumahubi.App -c Release -f net8.0-windows10.0.19041.0 -r win10-x64 -p:WindowsPackageType=MSIX -p:AppxPackageSigningEnabled=false -o publish/win

Tuotos: `publish/win/*.msix`

## Tunnetut rajoitteet
- Ensijulkaisu vain Windowsille.
- Tietokanta paikallinen (SQLite).