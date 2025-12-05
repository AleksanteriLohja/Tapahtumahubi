# Projektisuunnitelma – Tapahtumahubi

**Ryhmä:** R5  
**Versio:** 1.0  
**Päiväys:** 5.12.2025  
**Repo:** https://github.com/AleksanteriLohja/Tapahtumahubi

## 1. Tausta ja tavoitteet
Toteutetaan Windowsille optimoitu tapahtumahallinnan MAUI-sovellus (CRUD, osallistujat, kalenteri). Painotus selkeässä arkkitehtuurissa (Domain/Infrastructure/App), testauksessa ja dokumentaatiossa. Kurssin 5/5-tavoite: kaikki päätoiminnallisuudet laadukkaasti, syötevalidoinnit, asialliset virheilmoitukset, näppäimistö-navigointi (TabIndex), ei kaatumisia.

## 2. Pelisäännöt
- **Työaika:** 3 op × 27 h / opiskelija (kirjataan viikoittain).
- **Viestintä:** Discord + GitHub Issues/Projects (asynkroninen työ sallittu).
- **Työkalut:** .NET 8, MAUI, EF Core + SQLite, xUnit, Serilog, VS Code, GitHub.
- **Palaveri:** lyhyt viikkokoonti tai asynkroninen kirjallinen kooste (päätökset ylös).

## 3. Roolit ja projektipäällikön rotaatio
| Viikko | Päällikkö     | Vastuut |
|-------:|---------------|---------|
| v1     | Aleksanteri   | Kickoff, repo, backlog, roolitus |
| v2     | Jan           | Domain & Infrastructure, migraatiot |
| v3     | Antti         | UI & navigointi |
| v4     | Simo          | Testit & katselmointi |
| v5     | Aleksanteri   | Viimeistely, julkaisu, esitysvideo |

> Asynkroninen työ: päätökset ja tehtäväjaot kirjataan viikkoraportteihin/pöytäkirjoihin.

## 4. Vaihemalli (vesiputous)
1) **Määrittely** – vaatimukset & toiminnallisuus  
2) **Suunnittelu** – arkkitehtuuri, tietokanta  
3) **Toteutus** – inkrementit (featuret)  
4) **Testaus** – yksikkö + integraatio + manuaalitarkistus UI:hin  
5) **Katselmointi & julkaisu** – v1.0, dokumentit, video

### 4.1 Viikkokohtainen eteneminen
- **v1–v2:** Domain + Infra + perus UI, EF-migraatiot  
- **v3:** CRUD + osallistujat, validoinnit, Serilog  
- **v4:** Kalenteri, testit, virhepolut, TabIndex-järjestys  
- **v5:** Viimeistely, README & dokumentit, julkaisu, esitysvideo

## 5. Definition of Ready (DoR)
User story/tehtävä on “ready”, kun:
- Kuvaus, hyväksymiskriteerit ja UI-paikka on selviä
- Mahdolliset DB-muutokset ja migraatiotarve tunnistettu
- Validointisäännöt kirjattu (pakolliset kentät, rajat)
- Testitapausluonnos olemassa (onnistuu/epäonnistuu)

## 6. Definition of Done (DoD)
- Kääntyy ilman varoituksia (**TreatWarningsAsErrors**)  
- `dotnet test --settings coverage.runsettings` vihreänä  
- README ja dokumentit päivitetty  
- UI-pääpolut testattu: lisäys, muokkaus, poisto, osallistujat, kalenteri  
- Lokiin ei jää virheitä normaalikäytössä (Serilog)  
- TabIndex-järjestys ja syötevalidoinnit kunnossa, selkeät virheilmoitukset

## 7. Git-käytännöt
- **Haarat:** `main` (vakaa), `feature/<kuvaava-nimi>`  
- **PR-sääntö:** vähintään 1 katselmoija; PR:ssä kuvaus + kuvakaappaus tarvittaessa  
- **Commiteissa:** pieni, kuvaava viesti (esim. `feat(ui): lisää osallistujan validointi`)

## 8. Riskit
| #  | Riski                            | Tod.näk. | Vaikutus | Hallinta |
|----|----------------------------------|:-------:|:--------:|----------|
| R1 | Aikataulu                        |   M     |    M     | Viikkoincrementit, selkeä backlog |
| R2 | EF-migraatiot rikkoutuvat        |   M     |    M     | `IDbContextFactory`, migraatiot ennen julkaisua |
| R3 | UI-kaatuminen                    |   L     |    K     | Poikkeusten käsittely, Serilog-logit, manuaalitestit |
| R4 | Tiimin saatavuus                 |   M     |    M     | Asynkroninen työ + kirjaukset |
| R5 | Heikko tabulointi/esteettömyys   |   M     |    M     | TabIndex suunnitelma, manuaalitestaus |

## 9. Hyväksymiskriteerit (arvosana 5/5)
- Kaikki päätoiminnot valmiit: tapahtumat (CRUD), osallistujat (uniikki sähköposti / tapahtuma, kapasiteetti), kalenteri
- Syötevalidoinnit ja selkeät virheilmoitukset kaikissa lomakkeissa
- Näppäimistö-navigointi: looginen **TabIndex** järjestys lomakenäkymissä
- Ei kaatumisia virhesyötteisiin; virheet lokitetaan (Serilog)
- Dokumentit palautettu: projektikortti, vaatimukset/toiminnallinen, viikkoraportit/pöytäkirjat, loppuraportti, esitysvideo

## 10. Raportointi & palautukset
- **Viikkoraportit:** yksi koontidokumentti (tai zip) viikoista v1–v5; kirjataan tunnit/hlö ja tehdyt asiat
- **Pöytäkirjat:** lyhyt muistio tai asynkroninen kooste päätöksistä + tehtävistä
- **Loppuraportti:** tiimin ja henkilökohtainen osuus (tunnit, oma panos, itsearvio)
- **Esitysvideo:** demo hyväksymiskriteerien mukaisessa järjestyksessä

## 11. Liitteet & viitteet
- README (ajot, migraatiot, lokit, julkaisu)  
- `docs/AI-käyttö.md` (mihin AI:ta käytettiin)  
- `.vscode/` (F5-ajot), `Directory.Build.props`, `coverage.runsettings`