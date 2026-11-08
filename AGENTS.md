# Portfel Inwestycji

Aplikacja do prowadzenia ewidencji własnego portfela inwestycji: aktywa, transakcje, bieżąca wycena i wyniki.

ASP.NET Core 10 na .NET 10, minimal API, OpenAPI ze Scalarem jako podglądem API, Serilog.

Interfejs jest renderowany po stronie serwera jako Razor Pages — nie ma osobnego projektu
frontendu.

Zacznij tutaj, potem idź za odnośnikami:

- [ai/standardy.md](ai/standardy.md) — konwencje, których trzyma się to repo.
- [ai/pamięć.md](ai/pamięć.md) — zgromadzone fakty o tym kodzie.
- [ai/proces-pracy.md](ai/proces-pracy.md) — jak wygląda tu praca: pomysły, plany, realizacja,
  weryfikacja.
- [spec/](spec/) — bieżąca i zakończona praca.
- [ai/ui.md](ai/ui.md) — konwencje UI: układ, komponenty, formułowanie tekstów.
- [docs/](docs/) — dokumentacja tego, jak aplikacja działa naprawdę.

## Język

Językiem tego repo jest **polski**. Dotyczy to całej prozy, planów, dokumentacji, tekstów
interfejsu i wiadomości commitów.

Szkielet techniczny jest angielski w każdym projekcie z tej rodziny i nie jest wyborem
językowym: `src/`, `tools/`, `deploy/`, `logs/`, `data/`, `.tmp/`, `run.ps1`, `run.sh` oraz
wszystkie pliki konfiguracyjne, razem z ich zawartością. Nazwy folderów `ai/`, `spec/`, `docs/`,
`.github/` i pliki `AGENTS.md`, `CLAUDE.md`, `README.md` też są stałe — ale wszystko w środku
jest w języku projektu.

W kodzie i w bazie danych **wszystko jest po angielsku** — identyfikatory, nazwy tabel
i kolumn, komentarze, komunikaty logów, łącznie ze słownictwem dziedzinowym. Po polsku jest
tylko to, co czyta użytkownik.

## Zasady bezwzględne

- **Skończoną pracę domyślnie commituj.** Kiedy zadanie jest zrobione, zacommituj to, co
  zmieniło — nie zostawiaj jej w working tree do znalezienia przez człowieka. Szczegóły
  w [ai/standardy.md](ai/standardy.md).
- **Po większym zadaniu: rebase, potem push.** Konflikty rozwiązuj po drodze, a gdy właściwe
  rozwiązanie nie jest oczywiste — pytaj.
- **Pliki tymczasowe i robocze trafiają do `.tmp/`** w katalogu głównym repo (ignorowanego przez
  gita) — nigdy do systemowego katalogu tymczasowego ani do AppData.
- **Cała pamięć projektu jest wspólna, w `ai/`** — nigdy w lokalnej pamięci żadnego narzędzia na
  żadnym komputerze, żeby każdy agent czytał to samo.
- **Nigdy nie realizuj prosto z pomysłu.** Pomysły stają się planami w `spec/50-do-zrobienia/`,
  plany są iterowane z człowiekiem, a realizowany jest tylko plan zaakceptowany.
- **Analizy trafiają do plików, nigdy na czat.** Kiedy padnie prośba o analizę, zapisz ją
  w `spec/10-analizy/` i napisz tylko, że jest gotowa.
- **Zgłaszaj zakończenie bez podsumowania.** Po realizacji zadania napisz, że jest skończone —
  szczegóły należą do rozdziału *Notatki z implementacji* w pliku planu, nie na czat.
- **Claimuj stories przed realizacją.** Nigdy nie realizuj story objętego claimem innego agenta
  ani nie zmieniaj jego kodu; stosuj procedurę claimowania i zwalniania z `ai/proces-pracy.md`.

## Polecenia

`./run.ps1` (albo `./run.sh`) bez argumentów wypisuje dostępne czasowniki i narzędzia.
