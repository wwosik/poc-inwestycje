# Aktywa

Lista aktywów użytkownika z ich wartością nominalną. Strona: `/Assets` (link „Aktywa"
w nawigacji).

## Co widzi użytkownik

- Tabela aktywów posortowana po nazwie w polskiej kolejności alfabetycznej (Ć po C, Ł po L,
  bez rozróżniania wielkości liter). Pod nazwą, mniejszym krojem: ticker / ISIN i notatka, jeśli
  są.
- Pod tabelą wiersze „Razem <waluta>" — suma wartości nominalnych osobno dla każdej waluty, bez
  przeliczania kursów. Kolejność walut: PLN, EUR, USD, GBP, CHF.
- Bez aktywów: komunikat z instrukcją, jak dodać pierwsze.
- Pod listą formularz „Dodaj aktywo". Po zapisie strona przeładowuje się (Post/Redirect/Get)
  i pokazuje „Dodano aktywo <nazwa>.". Przy błędach formularz wraca z wpisanymi wartościami
  i wszystkimi błędami naraz, przy polach.

Edycji i usuwania jeszcze nie ma — poprawić aktywo można tylko w pliku danych.

## Pola i reguły

| Pole | Reguły |
| ---- | ------ |
| Nazwa | wymagana, do 200 znaków, spacje na brzegach są obcinane |
| Rodzaj | wymagany: Akcje, Obligacje, Fundusz / ETF, Lokata, Gotówka, Nieruchomość, Kryptowaluty, Inne |
| Wartość nominalna | wymagana, ≥ 0, najwyżej 2 miejsca po przecinku |
| Waluta | wymagana: PLN (domyślna), EUR, USD, GBP, CHF |
| Ticker / ISIN | opcjonalne, do 32 znaków, zapisywane wielkimi literami |
| Notatka | opcjonalna, do 1000 znaków |

Wartość nominalna to kwota, którą użytkownik sam deklaruje — aplikacja niczego z niej nie
wylicza. Przyjmowane zapisy: `1234,56`, `1 234,56` (zwykła, twarda albo wąska spacja jako
separator tysięcy) oraz `1234.56`, jeśli w tekście nie ma przecinka. Wyświetlanie zawsze po
polsku: `1 234,56 PLN`. Cała aplikacja działa w kulturze `pl-PL`, niezależnie od języka
przeglądarki.

## Gdzie leżą dane

Plik `assets.json` w katalogu danych (`AppPaths:DataDirectory`, lokalnie `data/` w katalogu
głównym repo). Brak pliku oznacza pustą listę; plik powstaje przy pierwszym dodanym aktywie.

```json
{
  "version": 1,
  "assets": [
    {
      "id": "019a…",
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

- `kind` to nazwa wartości enuma `AssetKind` (`Stock`, `Bond`, `Fund`, `Deposit`, `Cash`,
  `RealEstate`, `Crypto`, `Other`), nie liczba.
- Zapis jest atomowy: nowa treść trafia do `assets.json.tmp`, potem podmienia `assets.json`.
- Plik jest wczytywany raz przy starcie i trzymany w pamięci. Ręczna zmiana pliku przy
  działającej aplikacji nie zostanie zauważona i zostanie nadpisana przy następnym dodaniu —
  plik edytuj przy zatrzymanej aplikacji.
- Jeśli pliku nie da się odczytać (zły JSON, inna `version`, brak listy `assets`, nieznany
  `kind`), aplikacja nie wystartuje, a w logu pojawi się błąd ze ścieżką pliku. Plik nie jest
  wtedy zmieniany.

## Kod

`backend/PortfelInwestycji.Server/Assets/` — model (`Asset`, `AssetKind`, `Currencies`),
parsowanie i formatowanie kwot (`Amount`), dane z formularza z walidacją (`AssetInput`), zapis
(`AssetStore`) i logika listy (`AssetService`). Strona: `Pages/Assets/Index`.
