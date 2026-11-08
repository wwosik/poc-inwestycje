# E01 Aktywa

> Status: w trakcie

## Cel

Użytkownik prowadzi w aplikacji listę swoich aktywów i dla każdego zapisuje jego wartość
nominalną. Aktywa dodaje bezpośrednio w interfejsie, bez edycji plików ani wywołań API.

## Uzasadnienie

Aktywo to podstawowy byt portfela — transakcje, wycena i wyniki (zob. [AGENTS.md](../../AGENTS.md))
będą się do niego odwoływać. Bez listy aktywów nie ma na czym budować dalej. Ten epic wprowadza
też pierwszy trwały zapis danych w aplikacji.

## Scope

| Story | Tytuł | Stan |
| ----- | ----- | ---- |
| [E01-S01](../60-do-sprawdzenia/E01-S01-lista-i-dodawanie-aktywow.md) | Lista aktywów i dodawanie aktywa | w weryfikacji |

Poza zakresem tego epica (kandydaci na kolejne stories, jeśli będą potrzebne): edycja i usuwanie
aktywa, endpointy API, transakcje, wycena rynkowa, przeliczanie walut.

## Decyzje wspólne dla obszaru

1. **Wartość nominalna to jedna kwota wpisywana przez użytkownika** — ile według niego aktywo
   jest warte albo ile w nie włożył. Aplikacja niczego z niej nie wylicza.
2. **Trwały zapis: plik JSON** `data/assets.json` w katalogu danych z `AppPaths`. Zapis
   atomowy (plik tymczasowy obok, potem podmiana), dostęp przez jeden singleton z blokadą.
   Plik ma pole `version`, żeby późniejsza zmiana formatu albo przejście na bazę miało od czego
   się odbić.
3. **Kwoty jako `decimal`**, nigdy `double`. **Waluta per aktywo** jako trzyliterowy kod
   ISO 4217; obsługiwane: `PLN`, `EUR`, `USD`, `GBP`, `CHF`, domyślnie `PLN`. Sumy liczone
   osobno dla każdej waluty, bez przeliczania kursów.
4. **Rodzaj aktywa** z zamkniętej listy: Akcje, Obligacje, Fundusz / ETF, Lokata, Gotówka,
   Nieruchomość, Kryptowaluty, Inne.
5. **Nazwy w kodzie i w pliku danych po angielsku**: `Asset`, `AssetKind`, `Name`, `Kind`,
   `NominalValue`, `Currency`, `Identifier`, `Note`, `CreatedAt`.
6. **Format liczb w UI po polsku** (`1 234,56`), także przy wpisywaniu — kultura żądań `pl-PL`.
7. **Na razie bez API** — tylko Razor Pages. Endpointy dojdą na tym samym serwisie, gdy będą
   potrzebne.

## Kryteria akceptacji

- [ ] Użytkownik widzi listę swoich aktywów z wartością nominalną.
- [ ] Użytkownik dodaje aktywo z poziomu interfejsu i widzi je od razu na liście.
- [ ] Aktywa przetrwają restart aplikacji.

## Otwarte pytania

1. Co znaczy „wartość nominalna" w tym portfelu?

> Jedna kwota wpisywana przez użytkownika, bez wyliczania.

2. Czy SQLite + EF Core jest w porządku jako magazyn danych? Alternatywa: plik JSON w `data/`.

> Plik JSON w `data/`.

## Uwagi

