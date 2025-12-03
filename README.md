Tapahtumahubi

Yksinkertainen mutta tuotantotapainen tapahtumahallinnan sovellus (.NET 8 + .NET MAUI, SQLite, EF Core, MVVM).
Tavoitteena on kurssiprojektina toteuttaa selkeästi rakennettu, testattu ja dokumentoitu työpöytäsovellus.

Sisällys

Arkkitehtuuri

Vaatimukset ja asennus

Ajaminen (Windows)

Tietokanta ja migraatiot

Lokitus

Laatu: koodi, testit ja tyyli

Projektinhallinta: roolit, tunnit, AI-käyttö

Käyttöohje (pika)

Tunnetut rajoitteet ja jatkokehitys

Lisenssi


Arkkitehtuuri

Ratkaisun kerrokset

Tapahtumahubi.Domain
Entiteetit ja niiden sisäinen validointi (Event, Participant).
Domain-logiikan tarkoitus on olla teknologianeutraali.

Tapahtumahubi.Infrastructure

EF Core/SQLite: AppDbContext, migraatiot ja kyselyt (Queries/), alustus (Seed/).

Palvelut: esim. ParticipantService, joka käyttää IDbContextFactory<AppDbContext> (turvallinen malli UI-sovelluksille).

Testeissä käytetään in-memory/SQLite-strategioita.

Tapahtumahubi.App (.NET MAUI)

MVVM: ViewModels/ + näkymät (*.xaml).

DI: kaikki sivut ja viewmodelit rekisteröidään MauiProgram.cs:ssä.

ServiceHelper hakee DI:stä VM:n sivujen parametrittomissa konstruktoreissa.

Serilog lokittaa tiedostoon.

Windows-target: net8.0-windows10.0.19041.0.

Tapahtumahubi.Tests
Yksikkö- ja integraatiotestit (xUnit). Kattaa domain-validoinnit, kyselyt, palvelut ja perus-VM-polut.

Navigaatio/UX

Tabit: Tapahtumat, Kalenteri.

Uusi/Muokkaa tapahtumaa -näkymässä otsikko, sijainti, päivä + kellonaika, kuvaus ja maks. osallistujat.

Osallistujille uniikki sähköposti tapahtumaa kohti; kapasiteettirajat huomioidaan.


Vaatimukset ja asennus

Windows 10 2004 / 11 (19041+)

.NET 8 SDK ja .NET 8 Desktop Runtime

(Suositus) EF Core Tools: dotnet tool install --global dotnet-ef

Kloonaus:
git clone https://github.com/AleksanteriLohja/Tapahtumahubi.git
cd Tapahtumahubi
dotnet restore


Ajaminen (Windows)
dotnet build src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0
dotnet run   --project src/Tapahtumahubi.App/Tapahtumahubi.App.csproj -f net8.0-windows10.0.19041.0

Ensimmäisellä ajolla sovellus luo paikallisen SQLite-tietokannan:
%LocalAppData%\Tapahtumahubi.App\app.db


Tietokanta ja migraatiot

Migraatiot ovat projektissa Tapahtumahubi.Infrastructure (src/Tapahtumahubi.Infrastructure/Migrations).

Tyypilliset komennot:
# Aja viimeisin migraatio (kehitys)
dotnet ef database update --project src/Tapahtumahubi.Infrastructure --startup-project src/Tapahtumahubi.App

# Luo uusi migraatio
dotnet ef migrations add <Nimi> --project src/Tapahtumahubi.Infrastructure --startup-project src/Tapahtumahubi.App

Huom: App käyttää IDbContextFactory<AppDbContext> ja sijoittaa db-tiedoston käyttäjän LocalApplicationData-kansioon. Tämä vähentää lukituksia ja sopii UI-sovelluksille.


Lokitus

Serilog kirjoittaa lokitiedostot:
%LocalAppData%\Tapahtumahubi.App\logs\app-<päivä>.log

Lokista löytyy mahdolliset käynnistys-/run-aikaiset virheilmoitukset (hyödyllinen debugissa).

Laatu: koodi, testit ja tyyli

Koodityyli: .editorconfig repojuuressa; varatut sanat ja nimet selkeästi.

Analysointi: projektit kääntyvät puhtaasti .NET 8:lla; varoitukset pidetään minimissä.

Testit:
dotnet test

(Valinnaisesti voi kerätä kattavuuden Coverletilla / runsettingsillä.)

Pieni siivous tehty: tarpeettomat duplikaatit .gitignoressa poistettu, DI-rekisteröinnit täydelliset, sivujen konstruktorit parametrittomiksi ja VM-sidonta DI:stä (ServiceHelper).


Käyttöohje (pika)

Uusi tapahtuma: Tapahtumat-välilehdeltä Uusi. Täytä otsikko, sijainti, päivä, kellonaika, kuvaus ja maks. osallistujat. Tallenna.

Muokkaus/poisto: valitse tapahtuma listasta.

Osallistujat: lisää nimi + sähköposti. Sähköposti on uniikki per tapahtuma; kapasiteettiraja estää ylitäytön.

Kalenteri: selaa päivämääriä Kalenteri-välilehdeltä; päivän tapahtumat näkyvät listana.


Tunnetut rajoitteet ja jatkokehitys

MAUI-projekti on optimoitu Windowsille (kurssin demot). Muiden alustojen tuki on skelettona, ei testattu.

Jatkossa:

haku/suodatus tapahtumalistaan

testikattavuuden nosto (VM-komennot, virhepolut)

vienti CSV/ICS

per-event lokit/telemetria


Lisenssi

MIT (katso LICENSE)