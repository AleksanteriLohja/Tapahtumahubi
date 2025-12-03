# Projektisuunnitelma – Tapahtumahubi

## 1. Yhteenveto
Tapahtumahubi on .NET 8 / .NET MAUI -pohjainen työpöytäsovellus tapahtumien hallintaan. 
Tavoite: kurssiprojektina tuottaa rakenteeltaan selkeä, testattu ja dokumentoitu sovellus,
joka demonstroi kerrosarkkitehtuuria (Domain/Infrastructure/App), MVVM-mallia, EF Core -migraatioita,
lokitusta ja projektityöskentelyä.

**Valmis, kun:** alla oleva *Definition of Done* täyttyy kaikille toteutettaville ominaisuuksille.

---

## 2. Tavoitteet ja laajuus
**In scope**
- Tapahtumien CRUD (otsikko, sijainti, päivä + kellonaika, kuvaus, maks. osallistujat).
- Osallistujat per tapahtuma (uniikki sähköposti/tapahtuma, kapasiteettiraja).
- Kalenterin selaus ja tapahtumien näkyminen päivittäin.
- Lokitus (Serilog), tietokanta (SQLite), migraatiot (EF Core).
- Testit: domain-validoinnit, palvelut, kyselyt, keskeiset VM-polut.
- Dokumentaatio (README, tämä suunnitelma, AI-käyttö).

**Out of scope**
- Mobiilialustojen tuotantotuki.
- Synkronointi pilveen / käyttäjähallinta.
- Monikielisyys.

---

## 3. Aikataulu ja virstanpylväät
| Viikko | Virstanpylväs | Tuotokset |
|------:|----------------|-----------|
| 1 | Projektin käynnistys | Repo valmis, backlog, roolit, arkkitehtuuripäätökset |
| 2 | Perus-CRUD ja kalenteri | Event/Participant-peruspolut, ensimmäiset testit |
| 3 | Validoinnit ja virheenkäsittely | Domain-validoinnit, kapasiteetti/uniikkius, lokitus |
| 4 | Siistiminen ja testikattavuus | Testien laajennus, README, tähän suunnitelmaan päivitykset |
| 5 | Demo ja loppuraportti | Käyttöohje, AI-käyttöraportti, tuntikooste |

(Tarkenna kurssin todellisen kalenterin mukaan.)

---

## 4. Roolit ja vastuut
| Jakso | Projektipäällikkö | Vastuut |
|------:|-------------------|---------|
| vk 1–2 | Nimi | Sprintin suunnittelu, daily/weekly, katselmoinnit |
| vk 3–4 | Nimi | Priorisointi, PR-käytännöt, riskiseuranta |
| vk 5 | Nimi | Loppuraportti, demo, viimeistely |

**Yleiset vastuut**
- **Tekninen vastuu:** arkkitehtuuri, EF-migraatiot, CI (tarvittaessa).
- **Testausvastuu:** testikattavuus, runsettings.
- **Dokumentointivastuu:** README, käyttöohje, AI-käyttö, tämä suunnitelma.

---

## 5. Työnjako ja työskentelytavat
- **Git-käytäntö:** feature-haarat → PR → koodikatselmointi → main.
- **Branch-nimeäminen:** `feature/<aihe>`, `fix/<aihe>`, `docs/<aihe>`.
- **Commit-viestit:** lyhyt imperatiivi + tarvittaessa tarkenne (issue/PR-viite).
- **Project-taulu:** To Do → In Progress → Done, omistaja & tavoitepäivä jokaiselle kortille.
- **Palaverit:** 1×/vko synkronointi (30–45 min), ad hoc tarpeen mukaan.
- **Viestintä:** Discord (ryhmä), Moodle (opettajaviestit).

---

## 6. Laatukäytännöt (Definition of Done)
- Koodi noudattaa `.editorconfig`-tyyliä, build puhdas.
- Ominaisuudella on **testit** (onnistuneet + virhepolut, jos järkevää).
- Validoinnit domainissa; virheet näkyvät UI:ssa hallitusti (ei kaatumista).
- Tietokantamuutokset migraatioilla; `dotnet ef database update` toimii.
- README/käyttöohje päivitetty, lokin sijainti dokumentoitu.
- PR on katselmoitu ja hyväksytty, Project-kortti siirretty Doneen.

---

## 7. Riskit ja hallinta
| Riski | Todennäköisyys | Vaikutus | Hallintatoimi |
|------|:--------------:|:--------:|---------------|
| Aikataulu venyy | M | H | Priorisoi vähimmäisominaisuudet, tee demo-kelpoinen ensin |
| Tietokantalukitus / korruptio | L | M | `IDbContextFactory` UI:ssa, yksikkötestit, varmuuskopiointi tarvittaessa |
| DI/VM instansiointi rikkoo navigaation | M | M | Parametrittomat sivukonstruktorit + `ServiceHelper` |
| Testikattavuus jää ohueksi | M | M | Lisää testit jokaisesta bugista ja tärkeästä polusta |
| Tiimin saatavuus | M | M | Roolivarahenkilöt, viikkopalaverit |

---

## 8. Työkalut ja ympäristö
- **Tekniikat:** .NET 8, .NET MAUI (Windows), EF Core + SQLite, xUnit, Serilog.
- **IDE/Editor:** VS Code (F5=build+run), vaihtoehtona Visual Studio 2022.
- **OS:** Windows 10/11 (19041+).
- **Komennot:** ks. README (build/run/test/migraatiot).

---

## 9. Tuntiseuranta (81 h/opiskelija)
| Päivä | Tekijä | Tehtävä | kesto (h) | Kuvaus |
|------:|--------|---------|----------:|--------|
|      |        |         |           |        |

Tunnit kootaan loppuraporttiin (suunnitellut vs. toteutuneet, poikkeamasyyt).

---

## 10. Loppuraportin runko
- Johdanto: tavoite, tiimi, roolit, aikataulu.
- Tekniset ratkaisut ja perustelut.
- Toteutuneet ominaisuudet ja rajaukset.
- Laatu: testit, validoinnit, lokitus, käyttökokemus.
- Projektinhallinta: tunnit, riskit, viestintä.
- **Tekoälyn käyttö:** mitä, miksi, vaikutus, opit (linkki `AI-käyttö.md`:hen).
- Johtopäätökset ja jatkokehitys.
