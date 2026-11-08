# E01-S01 Lista aktywów i dodawanie aktywa

> Status: do zrobienia
> Epic: [E01 Aktywa](E01-aktywa.md)

## Podsumowanie

Strona z listą aktywów i ich wartością nominalną oraz formularzem dodawania nowego aktywa.
Pierwszy pionowy przekrój aplikacji: od pliku danych po interfejs.

## Kontekst

- Aplikacja ma dziś tylko pustą stronę główną ([Index.cshtml](../../backend/PortfelInwestycji.Server/Pages/Index.cshtml))
  i layout z samym logo w nawigacji ([_Layout.cshtml](../../backend/PortfelInwestycji.Server/Pages/Shared/_Layout.cshtml)).
- Nie ma żadnego trwałego zapisu ani modelu domeny. Katalog danych jest rozwiązywany przez
  [AppPaths](../../backend/PortfelInwestycji.Server/Configuration/AppPaths.cs).
- Konwencje UI: Razor Pages, post formularza, bez frameworków CSS/JS, `PageModel` bez logiki
  biznesowej ([ai/ui.md](../../ai/ui.md)).
- Decyzje wspólne (JSON, waluty, rodzaje, brak API) są w epicu [E01](E01-aktywa.md).

## Proponowane podejście

### Model

Rekord `Asset`:

| Pole | Typ | Reguły |
| ---- | --- | ------ |
| `Id` | `Guid` | nadawany przy zapisie |
| `Name` | `string` | wymagane, 1–200 znaków po przycięciu spacji |
| `Kind` | enum `AssetKind` | wymagane |
| `NominalValue` | `decimal` | wymagane, ≥ 0, maks. 2 miejsca po przecinku |
| `Currency` | `string` | wymagane, jeden z: `PLN`, `EUR`, `USD`, `GBP`, `CHF`; domyślnie `PLN` |
| `Identifier` | `string?` | opcjonalne (ticker albo ISIN), maks. 32 znaki po przycięciu, zapisywane wielkimi literami |
| `Note` | `string?` | opcjonalne, maks. 1000 znaków po przycięciu |
| `CreatedAt` | `DateTimeOffset` | ustawiane przy zapisie, UTC |

`AssetKind` (etykieta w UI): `Stock` (Akcje), `Bond` (Obligacje), `Fund` (Fundusz / ETF),
`Deposit` (Lokata), `Cash` (Gotówka), `RealEstate` (Nieruchomość), `Crypto` (Kryptowaluty),
`Other` (Inne). W JSON zapisywany jako tekst (`"Stock"`), nie liczba.

Format pliku `data/assets.json`:

```json
{
  "version": 1,
  "assets": [
    {
      "id": "…",
      "name": "Obligacje skarbowe EDO",
      "kind": "Bond",
      "nominalValue": 10000.00,
      "currency": "PLN",
      "identifier": "EDO0934",
      "note": null,
      "createdAt": "2026-11-08T10:00:00+00:00"
    }
  ]
}
```

### Kroki

1. **`AssetStore`** (singleton) — czyta i zapisuje `assets.json` w `AppPaths.DataDirectory`.
   Brak pliku = pusta lista. Zapis atomowy: serializacja do `assets.json.tmp` obok, potem
   `File.Move(..., overwrite: true)`. Odczyty i zapisy pod `SemaphoreSlim`, żeby dwa równoległe
   posty nie zgubiły aktywa. Uszkodzony plik albo nieznana `version` → błąd przy starcie
   z jasnym komunikatem w logu; pliku nie nadpisujemy.
2. **`AssetService`** — `ListAsync()` (sortowanie po nazwie, kultura `pl-PL`) i `AddAsync(...)`.
   Walidacja w DTO atrybutami, wszystkie błędy naraz (zgodnie z
   [ai/standardy.md](../../ai/standardy.md)). Sumy per waluta jako osobna metoda, żeby dało się
   je testować.
3. **Strona `Pages/Assets/Index`** (`/Assets`):
   - Tabela: nazwa (pod nią mniejszym krojem ticker/ISIN i notatka, jeśli są), rodzaj, wartość
     nominalna (format `1 234,56 PLN`, wyrównana do prawej). Pod tabelą suma dla każdej waluty
     osobno.
   - Stan pusty: „Nie masz jeszcze żadnych aktywów. Dodaj pierwsze w formularzu poniżej."
   - Pod tabelą formularz „Dodaj aktywo": nazwa, rodzaj (select), wartość nominalna, waluta
     (select, domyślnie PLN), ticker / ISIN (opcjonalne), notatka (opcjonalna, textarea).
     Przycisk **Dodaj aktywo**.
   - Post → walidacja → zapis → redirect na `/Assets` (Post/Redirect/Get) z komunikatem
     „Dodano aktywo <nazwa>." (TempData). Przy błędach formularz wraca z wpisanymi wartościami
     i komunikatami przy polach, po polsku.
4. **Kultura `pl-PL`** dla żądań (`UseRequestLocalization`), żeby wpisywanie `1234,56` i
   `1 234,56` działało, a wyświetlanie było po polsku.
5. **Nawigacja**: link „Aktywa" w `_Layout.cshtml`. Strona główna bez zmian.
6. **Style** w `site.css` przez istniejące zmienne: tabela (kwoty do prawej, cyfry
   tabularyczne), formularz, komunikat sukcesu, błędy walidacji — w obu motywach.
7. **Testy** w `PortfelInwestycji.Server.Tests`: walidacja (puste pola, wartość ujemna, za dużo
   miejsc po przecinku, zła waluta, wiele błędów naraz), `AssetStore` na katalogu w `.tmp/`
   (brak pliku, zapis i odczyt, zachowanie `decimal` i enuma), `AssetService` (sortowanie,
   sumy per waluta).
8. **Dokumentacja**: `docs/` — gdzie leży plik danych, jego format i jak działa lista aktywów;
   `ai/pamięć.md` — decyzja o JSON zamiast bazy i o zapisie atomowym pod blokadą.

## Kryteria akceptacji

- [ ] `/Assets` pokazuje wszystkie aktywa posortowane po nazwie, z rodzajem, wartością
      nominalną w polskim formacie oraz tickerem/ISIN i notatką, jeśli są.
- [ ] Pod listą jest suma wartości dla każdej waluty osobno.
- [ ] Przy braku aktywów widać stan pusty z instrukcją.
- [ ] Formularz dodaje aktywo; po zapisie użytkownik wraca na listę i widzi komunikat i nowe
      aktywo.
- [ ] Błędne dane pokazują wszystkie błędy naraz, przy polach, po polsku, bez utraty wpisanych
      wartości.
- [ ] Wartość `1 234,56` i `1234,56` jest przyjmowana; wartość ujemna jest odrzucana.
- [ ] Dane przetrwają restart; leżą w `data/assets.json`.
- [ ] Link „Aktywa" w nawigacji.
- [ ] Build i testy przechodzą.

## Otwarte pytania

1. Czy lista rodzajów aktywów pasuje? Czy rodzaj w ogóle jest potrzebny na start?

> Tak, ta lista, wymagana.

2. Waluta per aktywo z sumą osobno dla każdej waluty — czy wystarczy sam PLN?

> Waluta per aktywo.

3. Czy potrzebne są dodatkowe pola na start, np. notatka, ticker/ISIN, konto/rachunek maklerski?

> Notatka oraz ticker / ISIN. Bez konta.

4. Czy wystawić też endpointy API (`GET`/`POST /api/assets`)?

> Nie teraz.

5. Formularz pod listą na tej samej stronie, czy osobna strona `/Assets/Create`?

> Pod listą, na tej samej stronie.

## Uwagi

