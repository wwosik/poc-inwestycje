# Jak wygląda praca w tym repozytorium

Pomysł → epic/story → iteracja → claim → realizacja → weryfikacja → archiwum. Każde przejście
uruchamia człowiek; nic nie przesuwa się samo.

## 1. Pomysły — `spec/00-pomysły/`

Pisane przez człowieka, jeden plik na pomysł lub temat, swobodnie. Pomysł z zewnętrznego systemu
zadań zachowuje swoją referencję, żeby droga powrotna nigdy nie zginęła.

## 2. Plan — `spec/50-do-zrobienia/`

Zaakceptowany pomysł rozpisz jako feature epic i jego stories albo jako techniczne story, jeśli
nie należy do obszaru funkcjonalnego. Używaj języka projektu; szablonem stories jest
[../spec/_szablon.md](../spec/_szablon.md).

- **Feature epic:** `E01-krótki-tytuł.md`. Dla każdego obszaru produktu/domeny użyj kolejnego
   wolnego numeru `E`. Epic zawiera cel, uzasadnienie, tabelę scope ze stories i ich stanem,
   decyzje wspólne dla obszaru, kryteria akceptacji i otwarte pytania. Decyzje przekrojowe należą
   do epica, nie do pojedynczego story.
- **Feature story:** `E01-S01-krótki-tytuł.md`. Numer story liczy się w obrębie epica; użyj
   kolejnego wolnego. Każde story należy do jednego feature epica albo technicznego kontenera.
   Nagłówek wskazuje epic: `> Epic: [E01 Tytuł](E01-krótki-tytuł.md)`.
- **Techniczne story:** `T01-S01-krótki-tytuł.md`, z `> Epic: T01 (techniczne)`. `T01` to
   kontener, nie plik ani feature epic. Po `Tnn-S99` zaczyna się następny kontener; każdy mieści
   maksymalnie 99 stories. Praca techniczna służąca konkretnemu feature epicowi należy do niego.
- **Milestone:** `M01-krótki-tytuł.md`. Grupuje stories z kilku epics dostarczane razem. Stories
   pozostają w swoich epicach i wskazują milestone w nagłówku:
   `> Milestone: [M01 Tytuł](M01-krótki-tytuł.md)`.

Znana zewnętrzna referencja `W-nnnn` stoi po ID story i przed slugiem. ID pozostają
stałe przy przenoszeniu między folderami i są wspólne dla wszystkich języków projektu.

Każde story kończy się rozdziałami **Otwarte pytania** (po każdym pytaniu pusta linia cytatu na
odpowiedź) oraz **Uwagi** (wolne miejsce na komentarze niezwiązane z pytaniami).

## 3. Iteracja

Człowiek wpisuje odpowiedzi; poprawiaj ten sam plik, aż zaakceptuje plan. Nie realizuj planu,
który nie został zaakceptowany.

## 4. Claim story

Przed realizacją story należy je claimować — zobacz [Claimowanie story](#claimowanie-story).
Epics i milestones są dokumentami planistycznymi; claimuje się stories.

## 5. Realizacja

Gdy plan jest zaakceptowany, story zostało claimowane przez tę sesję i pada prośba o realizację:

1. Zrealizuj story, najlepiej w worktree utworzonym na bazie commita claimu.
2. W branchu roboczym **przenieś** story z `spec/55-w-toku/<agent-id>/` do
   `spec/60-do-sprawdzenia/` — ta sama nazwa. Usuń pusty folder agenta i popraw linki w story,
   epicu i milestone.
3. Ustaw stan w tabeli scope epica na `w weryfikacji` i dopisz **Notatki z implementacji**:
   decyzje, kompromisy, odstępstwa, ostrzeżenia, punkty do weryfikacji i luźne sprawy.
4. Zacommituj implementację i przeniesione story razem, zgodnie z zasadami Git repozytorium.
5. Po zakończeniu zadania zrób rebase i push; rozwiązuj konflikty, pytając, gdy wynik nie jest
   oczywisty.
6. Na czacie napisz tylko, że zadanie jest skończone. **Bez podsumowania** — szczegóły są w
   pliku.

## 6. Weryfikacja

Człowiek weryfikuje. Po akceptacji **przenieś** story do
`spec/70-wykonane/<epic-id>-<slug>/` i ustaw status `ukończone`. Stories feature epica są razem.
Gdy wszystkie są ukończone, przenieś tam również plik epica. Techniczne stories trafiają do
`T01-techniczne/` (albo odpowiedniego kontenera `T`).

## Claimowanie story

Kilka agentów może pracować w osobnych worktree. Claim informuje pozostałych, że story i jego
obszar kodu są zajęte.

**ID agenta.** Dla wszystkich claimów w sesji używaj jednego krótkiego ID zapisanego małymi
literami i z myślnikami (np. `nebel-2`). Jeśli sesja nie ma jeszcze ID, przed pierwszym claimem
poproś człowieka o jego wybór. Nowa sesja pyta ponownie; człowiek może ponownie użyć ID, aby
wznowić claimy agenta.

**Claimuj przed jakąkolwiek implementacją:**

1. Pracuj na `main` w głównym checkoutie, nie w worktree; pliki objęte claimem nie mogą mieć
   niezatwierdzonych zmian. Sprawdź, czy story nadal jest w `spec/50-do-zrobienia/`. Jeśli leży
   już w `spec/55-w-toku/<inne-id>/`, zatrzymaj się i zapytaj człowieka; nie przejmuj go.
2. Przenieś story z tą samą nazwą do `spec/55-w-toku/<agent-id>/`. W nagłówku ustaw
   `> Status: w trakcie (<agent-id>)`.
3. Popraw linki zepsute przez przeniesienie. Story jest poziom głębiej, więc zmieniają się jego
   linki względne; zaktualizuj też linki w tabeli scope epica i milestone. W epicu ustaw stan
   story na `w trakcie (<agent-id>)`.
4. Zacommituj na `main` tylko claim, np. `Zajmij E01-S01: <agent-id>`. Nie dołączaj innych
   zmian.
5. Rozpocznij realizację, najlepiej w worktree opartym na tym commicie.

**Podczas claimu:**

- Nie edytuj, nie przenoś ani nie realizuj story claimowanego przez innego agenta; pozostaw mu
   też kod, którego dotyczy. Jeśli potrzebna jest tam zmiana, zapytaj człowieka.
- Claim kończy się, gdy story zostanie przeniesione do `spec/60-do-sprawdzenia/` w branchu agenta
   i ten branch trafi na `main`.
- Aby zwolnić claim bez realizacji, przenieś story z powrotem do `spec/50-do-zrobienia/`,
  przywróć status i linki oraz zacommituj na `main` tylko zwolnienie, np.
  `Zwolnij E01-S01: <agent-id>`.

## Epics, kontenery techniczne i milestones

- **Feature epics `E01`, `E02`, ...** grupują stories obszaru produktu lub domeny. Plik epica
   leży w `spec/50-do-zrobienia/`; po ukończeniu wszystkich stories przenieś go z nimi do
   `spec/70-wykonane/<epic-id>-<slug>/`. Używaj kolejnego wolnego numeru `E`.
- **Kontenery techniczne `T01`, `T02`, ...** grupują stories bez feature epica, np. hosting,
  storage, deployment, uwierzytelnianie i narzędzia. Nie mają pliku ani własnych wymagań; decyzje
  są w stories. Maksymalnie 99 stories w kontenerze; po `T01-S99` użyj `T02-S01`.
- **Milestones `M01`, `M02`, ...** grupują stories z kilku epics dostarczane wspólnie. Ich plik
   leży w `spec/50-do-zrobienia/`, ale stories pozostają przypisane do swoich epics.
- Do stories odwołuj się przez ID (`E02-S03`, `E02-S03 §2.4`), do epics przez ID (`E04`).

## Analizy — `spec/10-analizy/`

Analiza trafia do pliku `spec/10-analizy/RRRR-MM-DD <slug>.md`, nigdy na czat. Napisz tylko,
że jest gotowa i gdzie leży. Analiza ustala, jak jest; plan proponuje, co zmienić.

## Zasady bezwzględne

- Nigdy nie realizuj bezpośrednio z pomysłu; iteruj z człowiekiem i najpierw claimuj story.
- Nigdy nie ruszaj story ani kodu objętego claimem innego agenta.
- Commituj skończoną pracę i po zakończonym zadaniu zrób rebase i push — zobacz sekcję Git w
   [standardy.md](standardy.md).
- Pliki tymczasowe i robocze trafiają do `.tmp/` w katalogu głównym repo, nigdy do systemowego
   katalogu tymczasowego ani do AppData.
- Cała pamięć projektu zostaje wspólna, w `ai/`.
