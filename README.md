# Portfel Inwestycji

Aplikacja do prowadzenia ewidencji własnego portfela inwestycji: aktywa, transakcje, bieżąca wycena i wyniki.

## Stos technologiczny

ASP.NET Core 10 · minimal API · OpenAPI + Scalar · Serilog

Razor Pages

## Uruchamianie lokalnie

```
./run.ps1        # albo ./run.sh — uruchamia serwer deweloperski po https
./run.ps1 test
./run.ps1 build
./run.ps1 watch
```

Raz na maszynie zaufaj lokalnemu certyfikatowi deweloperskiemu HTTPS:

```
dotnet dev-certs https --trust
```

Podgląd API jest pod `/scalar`, dokument OpenAPI pod `/openapi/v1.json`, a health check pod
`/health`.

## Dla asystentów AI i współpracowników

Zacznij od [AGENTS.md](AGENTS.md).
