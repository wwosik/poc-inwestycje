# E01-S01 Lista aktywów i dodawanie aktywa

> Status: do zrobienia
> Epic: [E01 Aktywa](E01-aktywa.md)

## Podsumowanie

Strona z listą aktywów i ich wartością nominalną oraz formularzem dodawania nowego aktywa.
Pierwszy pionowy przekrój aplikacji: od bazy danych po interfejs.

## Kontekst

- Aplikacja ma dziś tylko pustą stronę główną ([Index.cshtml](../../backend/PortfelInwestycji.Server/Pages/Index.cshtml))
  i layout z samym logo w nawigacji ([_Layout.cshtml](../../backend/PortfelInwestycji.Server/Pages/Shared/_Layout.cshtml)).
- Nie ma żadnego trwałego zapisu ani modelu domeny. Katalog danych jest rozwiązywany przez
  [AppPaths](../../backend/PortfelInwestycji.Server/Configuration/AppPaths.cs).
- Konwencje UI: Razor Pages, post formularza, bez frameworków CSS/JS, `PageModel` bez logiki
  biznesowej ([ai/ui.md](../../ai/ui.md)).

## Proponowane podejście

### Model

Encja `Asset`:

| Pole | Typ | Reguły |
| ---- | --- | ------ |
| `Id` | `int` | klucz, autoinkrementacja |
| `Name` | `string` | wymagane, 1–200 znaków po przycięciu spacji |
| `Kind` | enum `AssetKind` | wymagane; wartości poniżej |
| `NominalValue` | `decimal` | wymagane, ≥ 0, maks. 2 miejsca po przecinku |
| `Currency` | `string` | wymagane, kod ISO 4217 z listy obsługiwanych; domyślnie `PLN` |
| `CreatedAt` | `DateTimeOffset` | ustawiane przy zapisie, UTC |

`AssetKind` (etykieta w UI): `Stock` (Akcje), `Bond` (Obligacje), `Fund` (Fundusz / ETF),
`Deposit` (Lokata), `Cash` (Gotówka), `RealEstate` (Nieruchomość), `Crypto` (Kryptowaluty),
`Other` (Inne). Obsługiwane waluty na start: `PLN`, `EUR`, `USD`, `GBP`, `CHF`.

### Kroki

1. **Pakiety** w `Directory.Packages.props`: `Microsoft.EntityFrameworkCore.Sqlite`,
   `Microsoft.EntityFrameworkCore.Design`; lokalny manifest narzędzi z `dotnet-ef`.
2. **`PortfolioDbContext`** z `DbSet<Asset>`; connection string budowany z
   `AppPaths.DataDirectory` (`portfel.db`). Rejestracja w `Program.cs`, `Database.Migrate()` przy
   starcie. Pierwsza migracja `AddAssets`.
3. **`AssetService`** — `ListAsync()` (sortowanie po nazwie) i `AddAsync(...)`. Walidacja w DTO
   atrybutami, wszystkie błędy naraz (zgodnie z [ai/standardy.md](../../ai/standardy.md)).
4. **Strona `Pages/Assets/Index`** (`/Assets`):
   - Tabela: nazwa, rodzaj, wartość nominalna (format `1 234,56 PLN`). Pod tabelą suma wartości
     **osobno dla każdej waluty** — bez przeliczania kursów.
   - Stan pusty: „Nie masz jeszcze żadnych aktywów. Dodaj pierwsze w formularzu poniżej."
   - Pod tabelą formularz „Dodaj aktywo": nazwa, rodzaj (select), wartość nominalna, waluta
     (select, domyślnie PLN). Przycisk **Dodaj aktywo**.
   - Post → walidacja → zapis → redirect na `/Assets` (Post/Redirect/Get) z komunikatem
     „Dodano aktywo <nazwa>.". Przy błędach formularz wraca z wpisanymi wartościami i komunikatami
     przy polach, po polsku.
5. **Kultura `pl-PL`** dla żądań (`UseRequestLocalization`), żeby wpisywanie `1234,56` i
   `1 234,56` działało, a wyświetlanie było po polsku.
6. **Nawigacja**: link „Aktywa" w `_Layout.cshtml`. Strona główna bez zmian.
7. **Style** w `site.css` przez istniejące zmienne: tabela (kwoty wyrównane do prawej, cyfry
   tabularyczne), formularz, komunikat sukcesu, błędy walidacji — w obu motywach.
8. **Testy** w `PortfelInwestycji.Server.Tests`: walidacja (puste pola, wartość ujemna, zła
   waluta, wiele błędów naraz), `AssetService` na SQLite in-memory (zapis, odczyt, sortowanie),
   sumy per waluta.
9. **Dokumentacja**: `docs/` — gdzie leży baza i jak działa lista aktywów; `ai/pamięć.md` —
   decyzja o SQLite/EF Core i pułapka: SQLite nie sumuje ani nie sortuje `decimal` natywnie
   (EF zapisuje go jako TEXT), więc sumy liczymy w pamięci.

## Kryteria akceptacji

- [ ] `/Assets` pokazuje wszystkie aktywa posortowane po nazwie, z rodzajem i wartością
      nominalną w polskim formacie.
- [ ] Pod listą jest suma wartości dla każdej waluty osobno.
- [ ] Przy braku aktywów widać stan pusty z instrukcją.
- [ ] Formularz dodaje aktywo; po zapisie użytkownik wraca na listę i widzi komunikat i nowe aktywo.
- [ ] Błędne dane pokazują wszystkie błędy naraz, przy polach, po polsku, bez utraty wpisanych
      wartości.
- [ ] Wartość `1 234,56` i `1234,56` jest przyjmowana; wartość ujemna jest odrzucana.
- [ ] Dane przetrwają restart; baza leży w `data/portfel.db`.
- [ ] Link „Aktywa" w nawigacji.
- [ ] Build i testy przechodzą.

## Otwarte pytania

1. Czy lista rodzajów aktywów (`Akcje`, `Obligacje`, `Fundusz / ETF`, `Lokata`, `Gotówka`,
   `Nieruchomość`, `Kryptowaluty`, `Inne`) pasuje? Czy rodzaj w ogóle jest potrzebny na start?

>

2. Waluta per aktywo z sumą osobno dla każdej waluty — czy wystarczy sam PLN?

>

3. Czy potrzebne są dodatkowe pola na start, np. notatka, ticker/ISIN, konto/rachunek maklerski?

>

4. Czy wystawić też endpointy API (`GET`/`POST /api/assets`, widoczne w Scalarze)? Propozycja:
   nie teraz — UI ich nie potrzebuje; łatwo dodać później na tym samym serwisie.

>

5. Formularz pod listą na tej samej stronie, czy osobna strona `/Assets/Create`? Propozycja: ta
   sama strona — mniej klikania przy dodawaniu kilku aktywów pod rząd.

>

## Uwagi

