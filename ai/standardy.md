# Konwencje tego repozytorium

Ten plik jest samowystarczalny: wszystko, czego potrzebuje agent, jest tutaj — nie za
odnośnikiem do innego repo ani do lokalnego dysku.

## Repozytorium

- Cała aplikacja mieści się w **jednym repo gita**.
- Cała pamięć projektu jest **wspólna**, w `ai/` — nigdy lokalna dla urządzenia.
- **Pliki tymczasowe i robocze** trafiają do `.tmp/` w katalogu głównym (ignorowanego przez
  gita) — nigdy do systemowego katalogu tymczasowego, nigdy do AppData.
- Dane programu leżą w ignorowanym przez gita katalogu `data/`.
- Logi rolują się codziennie do ignorowanego przez gita `logs/` w katalogu głównym.
- `AGENTS.md` pozostaje krótki i odsyła do `ai/`; `CLAUDE.md` odsyła wyłącznie do `AGENTS.md`.

## Struktura katalogów

| Ścieżka | Przeznaczenie |
| ------- | ------------- |
| `ai/` | Instrukcje dla AI i wspólna pamięć projektu |
| `docs/` | Dokumentacja tego, jak program naprawdę działa |
| `data/` | Dane programu (ignorowane przez gita) |
| `logs/` | Logi rolowane dziennie (ignorowane przez gita) |
| `.tmp/` | Katalog roboczy (ignorowany przez gita) |
| `spec/` | Specyfikacje i cykl życia zadań (patrz [proces-pracy.md](proces-pracy.md)) |
| `backend/` | Solucja .NET (`PortfelInwestycji.slnx`) — projekt serwera i sąsiedni projekt testów |
| `tools/` | Narzędzia pomocnicze, jeden folder na narzędzie, uruchamiane przez `run.ps1` / `run.sh` |

## Narzędzia

- Narzędzia pisane **specjalnie dla tego repo** to **jednoplikowe aplikacje .NET 10**,
  uruchamiane jako `dotnet run tools/<nazwa>/<nazwa>.cs` (zakładamy, że .NET 10 jest
  zainstalowany). Zależności NuGet idą w dyrektywach `#:package` na górze pliku.
- **Nie PowerShell, nie Python.** Jedyny `.ps1` w repo to sam launcher.
- Jeśli gotowy **pakiet npm** już to robi, użyj go bez zmian w `tools/npm` z **pnpm** — nie pisz
  tego od nowa w .NET.
- Istniejące narzędzia zewnętrzne nie wymagają przepisywania.
- **Jednorazowe narzędzia** i dane robocze trafiają do `.tmp/`, nie do `tools/`. `tools/` jest
  na to, co zostaje.
- `tools/`, `run.ps1` i `run.sh` są **w całości po angielsku** — kod, komentarze i komunikaty —
  niezależnie od języka reszty repo.

Oba launchery muszą pozostawać zgodne: czasownik dodany do `run.ps1` trafia w tej samej zmianie
także do `run.sh`.

## Konwencje .NET

- Zawsze **najnowsza** wersja .NET.
- Solucja serwera to `backend/PortfelInwestycji.slnx`, każdy projekt we własnym
  podkatalogu.
- **Centralne zarządzanie pakietami** przez `backend/Directory.Packages.props` — wersja
  pakietu jest zadeklarowana dokładnie raz, a projekty referują pakiety bez wersji.
- Wspólny **`.editorconfig`** w katalogu głównym nie jest specyficzny dla projektu: nie
  przerabiaj go pod to repo.
- **Lokalny `nuget.config`** ustala źródła pakietów, z `<clear />` na początku, żeby nie
  przeciekło żadne źródło skonfigurowane globalnie na maszynie.
- **Testy leżą w tej samej solucji**, jako projekt sąsiadujący z testowanym kodem
  (`PortfelInwestycji.Server.Tests` obok `PortfelInwestycji.Server`) — nigdy w osobnej solucji.
- **Serilog** wszędzie, z dwoma sinkami: kolorowa konsola oraz pliki rolowane dziennie do
  ignorowanego przez gita `logs/` w katalogu głównym.
- Ścieżki do danych trwałych rozwiązywane są raz przy starcie przez `AppPaths`
  (`Configuration/AppPaths.cs`), konfigurowalne przez `AppPaths:DataDirectory`
  i `AppPaths:LogsDirectory`. Nic innego nie składa ścieżki do `data/` ani `logs/` ręcznie.

### Aplikacje webowe

- API wystawiamy w developmencie po **lokalnym HTTPS** — nigdy po zwykłym HTTP.
- **Minimal API**, nie kontrolery.
- **OpenAPI** jest zawsze włączone, a **Scalar** serwuje podgląd API.
- Endpoint `/health` istnieje zawsze.
- Walidacja ma zwracać **wszystkie** błędy pól naraz, a nie po kolei:
  - Najpierw sparsuj żądanie do DTO, waliduj dopiero potem — błąd parsowania przerywa i zwraca
    tylko pierwszy problem.
  - Dlatego **nie używamy `required`** na właściwościach DTO: brakujące pole stałoby się
    problemem parsowania zamiast walidacji.
  - Preferuj walidację atrybutami; `IValidatableObject` uruchamia się dopiero po tym, jak
    walidacja atrybutami przejdzie.
  - Zamiast tablic używaj kolekcji `IReadOnly` — walidacja w .NET 10 wciąż ma problem
    z tablicami. `MinLength` na kolekcjach działa.
- Nieudana walidacja zwraca **HTTP 422**, nie 400, żeby odróżnić problemy biznesowe od
  technicznych.

## Git

- **Skończoną pracę domyślnie commituj.** Kiedy zadanie jest zrobione, zacommituj je. Praca
  zostawiona w working tree to praca, którą człowiek musi sam zauważyć, przejrzeć i zacommitować
  ręcznie.
- **Commituj to, co zadanie zmieniło — i nic więcej.** Niezwiązana poprawka to osobny commit.
- Wiadomości commitów są w języku projektu, tak jak reszta prozy.
- Nigdy nie dodawaj do wiadomości commitów stopki `Co-Authored-By`.
- **Nigdy nie commituj sekretów, wyników builda ani niczego, co obejmuje `.gitignore`.** Jeśli
  coś ignorowanego naprawdę musi trafić do repo, to pytanie do człowieka, a nie własna decyzja.
- **Nie commituj zepsutego stanu.** Jeśli build albo testy nie przechodzą, powiedz to i zatrzymaj
  się — czerwony commit kosztuje więcej, niż oszczędza.
- Nigdy nie pomijaj hooków (`--no-verify`) ani podpisywania. Hook, który nie przechodzi, to
  problem do naprawienia, a nie do obejścia.

### Po większym zadaniu

Większe zadanie to zrealizowany plan przeniesiony do `spec/60-do-sprawdzenia/` — albo, bez tego
procesu, każda samodzielna praca, o którą poprosił człowiek. Nie każdy commit po drodze.

1. **Rebase** na śledzoną gałąź upstream (`git pull --rebase`).
2. **Rozwiąż konflikty.** Jeśli właściwe rozwiązanie nie jest oczywiste, **zatrzymaj się
   i zapytaj** — nie zgaduj cudzych intencji.
3. **Push.**

`git pull --rebase` odtwarza twoje niewypchnięte commity na wierzchu upstreamu, więc wystarczy
zwykły `git push`. Rebase commitów, które **już** były wypchnięte, przepisuje historię: push
wymaga wtedy `--force-with-lease` — co jest w porządku na gałęzi, na którą pushujesz tylko ty,
i warte pytania na każdej gałęzi używanej przez kogoś jeszcze. Nigdy zwykłe `--force`.

## Odstępstwa

Miejsca, w których to repo odbiega od powyższych konwencji — bo jest od nich starsze albo bo coś
konkretnego tego wymagało. To, co tu wpisane, jest celowe: nie „naprawiaj" tego bez pytania.

| Konwencja | Co robi to repo zamiast tego | Dlaczego |
| --------- | ---------------------------- | -------- |
| — | — | — |
