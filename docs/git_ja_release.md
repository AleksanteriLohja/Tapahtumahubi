# Git-käytännöt ja julkaisu

## Haaramalli
- `main` on julkaistava.
- Uusi työ: `feature/<kuvaus>` tai `fix/<kuvaus>`.
- PR → review → merge (CI:n mentävä läpi).

## Commit-viestit
Conventional Commits: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`.

## Versionointi
SemVer: `vMAJOR.MINOR.PATCH`.

## Tagit ja release (manuaali)
git checkout main
git pull
git tag -a v0.x.y -m "v0.x.y: kuvaus"
git push origin v0.x.y


*(Release-workflow on toistaiseksi pois päältä. Voi ottaa käyttöön myöhemmin Windows-MSIXille.)*

## Suojaus
- PR-hyväksyntä vaaditaan.
- CI vihreäksi ennen mergeä.