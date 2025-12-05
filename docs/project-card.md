# Projektikortti – Tapahtumahubi

**Tavoite:** Pienorganisaatioiden tapahtumien hallinta (.NET 8, MAUI, SQLite EF Core).

## MVP (valmis)
- [x] Tapahtumien lista + haku
- [x] Luo / Muokkaa / Poista
- [x] EF Core + SQLite + migraatiot
- [x] Windows-debug (unpackaged)

## Seuraavat sprintit
1. **Osallistujat**
   - CRUD osallistujille per tapahtuma
   - Maksimikapasiteetin validointi

2. **UX viimeistely**
   - Tyylit ja tyhjien listojen tilaviestit
   - Päiväys-/aikavalitsimien parannukset

3. **CI/CD**
   - CI: build + test (VALMIS tässä paketissa)
   - (Valinnainen) Release-workflow Windowsille

4. **Laatu**
   - Yksikkötestit Domainiin
   - Testikattavuusraportti (coverlet)

## Teknologia
- .NET 8, .NET MAUI (Windows)
- EF Core (SQLite)