# Vaatimusmäärittely – Tapahtumahubi

## 1. Tarkoitus ja kohderyhmä
Sovellus tukee pienten yhdistysten, harrasteporukoiden ja tiimien tapahtumien hallintaa ilman pilviylläpitoa. Ensijulkaisu rajataan Windows-työpöydälle (.NET MAUI).

## 2. Käsitteet
- **Tapahtuma**: Title, StartTime, Location, Description?, MaxParticipants (oletus 50).
- **Osallistuja** (jatkokehitys): Name, Email?, Status (Confirmed/Cancelled).

## 3. Toiminnalliset vaatimukset
F-1: Käyttäjä voi luoda tapahtuman (pakolliset: Title, StartTime; valinnaiset: Location, Description, MaxParticipants).  
F-2: Käyttäjä voi listata ja hakea tapahtumia otsikon/paikan perusteella.  
F-3: Käyttäjä voi muokata ja poistaa tapahtumia (poisto pyytää vahvistuksen).  
F-4: Data tallentuu paikalliseen SQLite-tietokantaan (`events.db`).  
F-5: (Jatkokehitys) Tapahtumalle voi lisätä osallistujia; MaxParticipants-raja estää ylitäytön.  
F-6: (Jatkokehitys) Listan tyhjätila näyttää ohjetilan (“Ei tapahtumia – lisää uusi”).

## 4. Ei-toiminnalliset vaatimukset
N-1: Sovellus käynnistyy kehityskoneella < 2 s.  
N-2: Build ja testit ajetaan CI:ssä jokaisesta main-pushista.  
N-3: Koodi noudattaa `.editorconfig`-sääntöjä ja nimentää.  
N-4: Pysyvyys toteutetaan EF Corella (migrations lähdekoodissa).  
N-5: Käyttöliittymä toimii näppäimistöllä ja hiirellä (perusesteettömyys).

## 5. Rajoitteet ja rajaukset
R-1: Ensiversio vain Windows (MSIX toimitus mahdollista myöhemmin).  
R-2: Ei käyttäjähallintaa eikä verkkosynkkausta MVP:ssä.  
R-3: Ei monikielisyyttä MVP:ssä (UI suomi/englanti kooditasolla ok).

## 6. Hyväksymiskriteerit (esimerkit)
A-1: Kun luon tapahtuman pakollisilla tiedoilla, se ilmestyy listaan ja löytyy haulla otsikolla.  
A-2: Kun muokkaan tapahtuman otsikkoa, muutos päivittyy listaan.  
A-3: Kun poistan tapahtuman ja vahvistan, se poistuu pysyvästi tietokannasta.  
A-4: (Jatkokehitys) Kun lisään osallistujia yli rajan, UI estää tallennuksen selkeällä virheilmoituksella.

## 7. Tietomalli (MVP)
Event

Id (int, PK)
Title (string, req)
StartTime (DateTime, req)
Location (string?)
Description (string?)
MaxParticipants (int, default 50)


## 8. Käyttötapaukset (tiivistetty)
UC-1: Luo tapahtuma → syötä tiedot → Tallenna → listaan ilmestyy uusi rivi.  
UC-2: Hae tapahtumia → kirjoita hakusana → listaus suodattuu.  
UC-3: Muokkaa → avaa tapahtuma → muuta kenttiä → Tallenna.  
UC-4: Poista → vahvista → rivi poistuu.  
UC-5: (Jatkokehitys) Lisää osallistuja → syötä nimi → tallenna → kapasiteettisääntö tarkistuu.

## 9. Laadunvarmistus
- Yksikkötestit Domain- ja ViewModel-tasolla, kattavuustavoite 70–80 % ydinkoodista.
- PR-katselmointi ennen mergeä mainiin.