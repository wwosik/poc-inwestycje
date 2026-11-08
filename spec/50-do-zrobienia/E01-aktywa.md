# E01 Aktywa

> Status: do zrobienia

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
| [E01-S01](E01-S01-lista-i-dodawanie-aktywow.md) | Lista aktywów i dodawanie aktywa | do zrobienia |

Poza zakresem tego epica (kandydaci na kolejne stories, jeśli będą potrzebne): edycja i usuwanie
aktywa, transakcje, wycena rynkowa, przeliczanie walut.

## Decyzje wspólne dla obszaru

Propozycje — do potwierdzenia w otwartych pytaniach.

1. **Trwały zapis: SQLite przez EF Core.** Plik bazy `portfel.db` w katalogu danych z `AppPaths`
   (`data/`). Schemat zarządzany migracjami EF Core, aplikowanymi automatycznie przy starcie.
   SQLite nie wymaga osobnego serwera i pasuje do aplikacji jednoosobowej; EF Core daje migracje
   na kolejne epics (transakcje, wyceny).
2. **Kwoty jako `decimal`**, nigdy `double`. Waluta jako trzyliterowy kod ISO 4217 (`PLN`, `USD`,
   `EUR`).
3. **Nazwy w kodzie i bazie po angielsku**: encja `Asset`, tabela `Assets`, kolumny `Name`,
   `Kind`, `NominalValue`, `Currency`, `CreatedAt`.
4. **Format liczb w UI po polsku** (`1 234,56`), także przy wpisywaniu — kultura żądań `pl-PL`.

## Kryteria akceptacji

- [ ] Użytkownik widzi listę swoich aktywów z wartością nominalną.
- [ ] Użytkownik dodaje aktywo z poziomu interfejsu i widzi je od razu na liście.
- [ ] Aktywa przetrwają restart aplikacji.

## Otwarte pytania

1. Co znaczy „wartość nominalna" w tym portfelu? Propozycja: kwota, którą użytkownik sam wpisuje
   jako wartość aktywa (np. ile włożył albo ile jest warte według niego), bez żadnego
   automatycznego wyliczania. Alternatywa: wartość nominalna w sensie papierów dłużnych (np.
   obligacja o nominale 100 zł × liczba sztuk) — wtedy potrzebne byłyby dwa pola.

>

2. Czy SQLite + EF Core jest w porządku jako magazyn danych? Alternatywa: plik JSON w `data/` —
   prostszy dziś, ale bez migracji i zapytań, gdy dojdą transakcje.

>

## Uwagi

