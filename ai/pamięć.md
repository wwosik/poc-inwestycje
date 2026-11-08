# Pamięć projektu

Zgromadzone fakty o tym kodzie: podjęte decyzje, stosowane wzorce, pułapki warte zapamiętania.
Wspólne tutaj, a nie w lokalnej pamięci jakiegokolwiek narzędzia, żeby każdy agent na każdym
komputerze czytał to samo.

Trzymaj to rzeczowo i aktualnie. Kiedy coś tu zapisanego przestaje być prawdą, popraw to w tej
samej zmianie, która to unieważniła — nieaktualny fakt jest gorszy niż jego brak, bo się mu ufa.

## Przegląd

- Jeden projekt serwera `backend/PortfelInwestycji.Server`; kod domenowy w folderach według
  obszaru (`Assets/`), strony w `Pages/<Obszar>/`.
- Dane trwałe: pliki JSON w `AppPaths.DataDirectory`, dziś tylko `assets.json`
  (zob. [docs/aktywa.md](../docs/aktywa.md)).

## Decyzje

- **Zapis w JSON, nie w bazie** (E01). Decyzja człowieka: na start wystarczy plik. Każdy plik
  ma pole `version`, a jego magazyn (`AssetStore`) jest singletonem z jedną blokadą na odczyt
  i zapis i zapisuje atomowo (`.tmp` + `File.Move(..., overwrite: true)`). Gdy dojdą transakcje
  i zapytania przekrojowe, wrócić do pytania o SQLite.
- **Plik wczytywany przy starcie** (`await AssetStore.LoadAsync()` w `Program.cs`), żeby
  uszkodzony plik zatrzymał aplikację od razu, a nie przy pierwszym żądaniu — i żeby nie został
  nadpisany pustą listą.
- **Kwoty z formularza wiązane jako `string`**, parsowane przez `Assets/Amount.cs`. Binder
  `decimal` zwracałby angielskie komunikaty i własne reguły kultury; jako tekst błąd parsowania
  jest zwykłym błędem walidacji, zgłaszanym razem z pozostałymi.
- **Kultura `pl-PL` na sztywno**, bez dostawców z `Accept-Language` (`UseRequestLocalization`
  w `Program.cs`). Komunikaty bindera modeli są po polsku (`ModelBindingMessageProvider`).

## Pułapki

- `pl-PL` grupuje tysiące twardą spacją U+00A0, nie zwykłą. Wynik `ToString("N2", pl)` zawiera
  więc U+00A0 — w testach porównuj z `\u00A0`, nie ze spacją.
- Właściwości DTO z przycinaniem tekstu używają słowa kluczowego `field` z C# 14
  (`set => field = value?.Trim()`); `[Required]` widzi już przyciętą wartość.
