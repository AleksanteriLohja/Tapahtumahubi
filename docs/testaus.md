# Testausstrategia

## Tavoitteet
- Suojaa domain- ja ViewModel-logiikka regressioilta.
- CI ajaa testit jokaisesta main-pushista.
- Kattavuustavoite 70–80 % ydinkoodista.

## Työkalut
- xUnit
- FluentAssertions (valinnainen)
- Coverlet (XPlat Code Coverage)

## Paikallinen ajo
dotnet test

## Kattavuus
dotnet test --settings coverage.runsettings --collect:"XPlat Code Coverage"


## Rakenne ja käytännöt
- AAA (Arrange-Act-Assert).
- Nimeä: `Method_WhenCondition_ShouldOutcome`.
- Testaa: domain-säännöt, ViewModel-komennot/validointi, repositoryn CRUD.