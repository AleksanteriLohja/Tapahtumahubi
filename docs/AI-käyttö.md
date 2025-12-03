# Tekoälyn (AI) käyttö – Tapahtumahubi

## 1. Mitä tehtiin AI:n avulla
- DI/MVVM-rakenteen ongelman diagnosointi (sivujen parametrittomat konstruktorit + VM DI:stä).
- `ParticipantService` → `IDbContextFactory<AppDbContext>`-malli UI-sovellukselle.
- `MauiProgram.cs`-rekisteröinnit (sivut, VM:t, palvelut) kuntoon.
- Serilog-lokin sijainnin ja hyödyntämisen ohjeistus.
- README:n ja projektisuunnitelman mallit, EF-migraatiokäytännöt.

## 2. Miksi AI:ta käytettiin
- Nopeuttaa virheiden paikantamista ja rakenteen siistimistä.
- Tuottaa dokumenttipohjat ja kehitystyön “parhaat käytännöt” yhtenäisesti.

## 3. Hyödyt
- Käynnistysbugi paikannettiin ja korjattiin nopeasti.
- DI/VM-sidonta selkeytyi ja toistuvat virheet vähenivät.
- Dokumentaation ja ohjeiden laatu parani.

## 4. Rajoitteet ja varotoimet
- AI-ehdotukset **tarkistettiin** ja sovitettiin repon todelliseen sisältöön.
- Kurssin vaatimukset varmistettiin tiimin toimesta (AI ei “päätä” vaatimuksia).
- Koodiin ei hyväksytty sokeasti muutoksia ilman testausta.

## 5. Esimerkkejä (ennen → jälkeen)
- **Sivut:** `MainPage(CalendarPageViewModel vm)` → `MainPage()` + `BindingContext = ServiceHelper.GetRequiredService<...>()`
- **Palvelut:** ctor `AppDbContext` → `IDbContextFactory<AppDbContext>` + `using var db = await _factory.CreateDbContextAsync();`
- **DI:** lisätty `Services.AddTransient<MainPageViewModel>(); ...` + kaikki sivut/VM:t rekisteriin.

## 6. Arvio vaikutuksesta
- Arvioitu ajansäästö: 4–8 h.
- Laadullinen hyöty: selkeämpi arkkitehtuuri, parempi virheensietokyky, dokumentaation kattavuus.

## 7. Lähestymistapa jatkossa
- Käytä AI:ta ideointiin, rakenteen refaktorointiin ja dokumenttipohjiin.
- Pidä päätösvalta kehittäjällä: tarkista ehdotukset, testaa, dokumentoi.
