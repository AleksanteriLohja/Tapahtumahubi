# Vaatimusmäärittely – Tapahtumahubi

## Ydintoiminnot
- Käyttäjä voi luoda, listata, hakea, muokata ja poistaa tapahtumia.
- Tapahtumalla on: Title, StartTime, Location, Description?, MaxParticipants (oletus 50).
- Poisto pyytää vahvistuksen.
- Data säilyy paikallisesti SQLite-tietokannassa (events.db, AppDataDirectory).

## Rajaukset
- Ensivaiheessa vain Windows-työpöytä (MAUI).
- Ei käyttäjätiliä / verkkosynkkausta MVP:ssä.

## Ei-toiminnalliset
- Sovellus käynnistyy <2 s kehityskoneella.
- CI ajaa buildin ja testit jokaisesta puskuista mainiin.
- Koodi noudattaa .editorconfig-sääntöjä.