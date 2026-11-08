# spec

Specyfikacje i cykl życia zadań. Sam proces opisuje
[../ai/proces-pracy.md](../ai/proces-pracy.md); ten plik jest mapą folderów.

| Folder | Zawiera |
| ------ | ------- |
| `00-pomysły/` | Surowe pomysły, pisane przez człowieka. Jeden plik na pomysł lub temat. |
| `10-analizy/` | Zamówione analizy, `RRRR-MM-DD <slug>.md`. Wynik trafia tutaj do plików, nigdy na czat. |
| `50-do-zrobienia/` | Otwarte feature epics, milestones i stories czekające na realizację. |
| `55-w-toku/<agent-id>/` | Stories claimowane przez danego agenta. Nie ruszaj folderu innego agenta ani powiązanego kodu. |
| `60-do-sprawdzenia/` | Zrealizowane stories czekające na weryfikację przez człowieka. Każde ma *Notatki z implementacji*. |
| `70-wykonane/<epic-id>-<slug>/` | Zweryfikowane stories razem z ukończonym feature epikiem. Techniczne stories używają folderu kontenera `T`. |

Prefiksy liczbowe są po to, żeby cykl czytało się w kolejności niezależnie od języka projektu.
Luki są celowe — nowy etap da się wstawić bez przenumerowywania reszty.

| Typ | ID i nazwa pliku | Plik |
| --- | ---------------- | ----- |
| Feature epic | `E01-krótki-tytuł.md` | Tak; grupuje stories obszaru produktu/domeny |
| Feature story | `E01-S01-krótki-tytuł.md` | Tak; numer story liczy się w obrębie epica |
| Techniczne story | `T01-S01-krótki-tytuł.md` | Tak; `T01` jest kontenerem bez pliku |
| Milestone | `M01-krótki-tytuł.md` | Tak; grupuje stories z kilku epics |

Używaj kolejnego wolnego ID `E`/`M` i kolejnego numeru story w kontenerze `E`/`T`. Kontener
techniczny mieści maksymalnie 99 stories; po `T01-S99` użyj `T02-S01`. Znana zewnętrzna
referencja stoi po ID story i przed slugiem. Nazwa story nie zmienia się przy przenoszeniu.

Każdy feature epic prowadzi tabelę scope ze stories i ich stanami. Story należy do jednego
feature epica albo kontenera technicznego; dostawy obejmujące kilka epics grupują milestones.

- Claimuj story przed realizacją; claimy są commitowane na `main`. Nigdy nie ruszaj story
  claimowanego przez innego agenta. Zobacz
  [../ai/proces-pracy.md#claimowanie-story](../ai/proces-pracy.md#claimowanie-story).
- Commituj skończoną pracę; po zakończonym zadaniu zrób rebase i push.
- Nigdy nie pisz analizy na czacie; napisz plik.
