# E01-S01 Lista aktywów i dodawanie aktywa

> Status: w weryfikacji
> Epic: [E01 Aktywa](../50-do-zrobienia/E01-aktywa.md)

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
- Decyzje wspólne (JSON, waluty, rodzaje, brak API) są w epicu [E01](../50-do-zrobienia/E01-aktywa.md).

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


## Notatki z implementacji

### Co powstało

- `backend/PortfelInwestycji.Server/Assets/` — `Asset`, `AssetKind` z polskimi etykietami,
  `Currencies`, `Amount` (parsowanie i formatowanie kwot w `pl-PL`), `AssetInput` (dane
  z formularza z walidacją atrybutami), `AssetStore` (plik `data/assets.json`), `AssetService`
  (lista, dodawanie, sumy per waluta).
- `Pages/Assets/Index` — tabela, sumy, stan pusty i formularz na jednej stronie; link „Aktywa"
  w layoucie; style w `site.css` przez nowe zmienne `--surface`, `--danger`, `--success`
  (zdefiniowane w obu motywach).
- `Program.cs` — rejestracja serwisów, wczytanie pliku przy starcie, kultura `pl-PL` na sztywno,
  polskie komunikaty bindera modeli.
- Dokumentacja: [docs/aktywa.md](../../docs/aktywa.md); decyzje i pułapki w
  [ai/pamięć.md](../../ai/pamięć.md).

### Decyzje i odstępstwa od planu

- **Wartość nominalna wiązana jako tekst**, nie `decimal`, i parsowana przez `Amount.TryParse`.
  Dzięki temu błędny zapis kwoty jest zwykłym błędem walidacji po polsku, zgłaszanym razem
  z innymi (zgodnie ze standardem „wszystkie błędy naraz").
- **Dodatkowo przyjmowany zapis z kropką** (`1234.56`), gdy w tekście nie ma przecinka. W planie
  go nie było; łatwo usunąć, jeśli przeszkadza.
- **Wartość 0 jest dozwolona** (plan: „≥ 0"). `1,500` przechodzi jako 1,50 — liczą się
  znaczące miejsca po przecinku, nie zera na końcu.
- **Id to `Guid.CreateVersion7()`** — rośnie w czasie, więc kolejność w pliku odpowiada
  kolejności dodawania.
- **Plik w pamięci po starcie**: ręczna edycja `assets.json` przy działającej aplikacji zostanie
  nadpisana przy następnym dodaniu. Opisane w docs.
- **Nieznany `kind` w pliku** też blokuje start (JSON enum bez wartości liczbowych) — świadomie,
  żeby literówka w ręcznej edycji nie zgubiła danych.
- Komunikaty bindera modeli ustawione po polsku globalnie, bo zmanipulowana wartość `select`
  (np. `Input.Kind=Spaceship`) daje błąd bindowania, a nie walidacji.
- Testy piszą do `.tmp/tests/<losowy>/` w katalogu głównym repo (helper `TestDirectory`), nie do
  systemowego katalogu tymczasowego. Istniejące `AppPathsTests` nadal używają
  `Path.GetTempPath()` — nie ruszałem ich, bo to poza zakresem story.

### Weryfikacja

- `dotnet build` bez ostrzeżeń, `dotnet test`: 49/49 (parsowanie kwot, walidacja, zapis
  i odczyt pliku, uszkodzone pliki, 25 równoległych dodań, sortowanie po polsku, sumy).
- **Strony nie sprawdzałem w przeglądarce ani curlem** — człowiek pracował w tym czasie
  interaktywnie i prosił, żeby nie uruchamiać testów, więc aplikacji też nie startowałem.
  Do sprawdzenia ręcznie: wygląd tabeli i formularza w jasnym i ciemnym motywie, komunikat po
  dodaniu, błędy przy polach, wpisanie `1 234,56`.

### Luźne sprawy

- Brak edycji i usuwania aktywa — kandydat na E01-S02.
- Na wąskim ekranie tabela nie ma przewijania poziomego; przy długich nazwach zawija tekst.
