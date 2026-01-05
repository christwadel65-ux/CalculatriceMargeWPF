# Scripts de Compilation - Calculatrice de Marge

## Utilisation

### Windows (PowerShell)

```powershell
# Compiler tout (Debug, Release et Standalone)
.\build.ps1

# Compiler uniquement Debug
.\build.ps1 -Mode debug

# Compiler uniquement Release
.\build.ps1 -Mode release

# Compiler uniquement Standalone
.\build.ps1 -Mode standalone
```

### Linux / macOS (Bash)

```bash
# Compiler tout
./build.sh

# Compiler uniquement Debug
./build.sh debug

# Compiler uniquement Release
./build.sh release

# Compiler uniquement Standalone
./build.sh standalone
```

## Sorties

- **Debug** : `bin/Debug/net10.0-windows/CalculatriceMargeWPF.exe`
- **Release** : `bin/Release/net10.0-windows/CalculatriceMargeWPF.exe`
- **Standalone** : `bin/Release/net10.0-windows/win-x64/publish/CalculatriceMargeWPF.exe`

### Distribution
L'exécutable Standalone est idéal pour la distribution car il ne nécessite aucune installation préalable de .NET.
