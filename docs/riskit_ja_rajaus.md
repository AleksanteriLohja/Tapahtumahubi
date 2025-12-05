# Riskit ja rajaukset

## Rajaus (MVP)
- Alusta: Windows (.NET MAUI)
- Toiminnot: lista/haku, CRUD, paikallinen SQLite

## Riskit ja hallinta
1. **Workload-riippuvuudet** → Build/testi vain Windows-TFM:lle; release ci pois päältä.
2. **Tietomallimuutokset** → Migraatiot versionhallintaan; PR-katselmointi.
3. **Aikataulu** → MVP ensin; sprinttitaulu tärkeimmät edellä.
4. **Testien puute** → Yksikkötestit ja kattavuus CI:ssä.
5. **Tietosuoja** → Ei turhaa henkilötietoa; data paikallisesti.