
@echo off
REM Script de compilation - Calculatrice de Marge
REM Génère les exécutables en mode Debug, Release et Standalone

setlocal enabledelayedexpansion

set MODE=%1
if "%MODE%"=="" set MODE=all

set PROJECTPATH=%~dp0
set PROJECTNAME=CalculatriceMargeWPF

echo ================================================
echo   Compilation - %PROJECTNAME%
echo ================================================
echo.

echo 🧹 Nettoyage du projet...
call dotnet clean "%PROJECTPATH%%PROJECTNAME%.sln" -q >nul 2>&1
echo.

if "%MODE%"=="all" goto build_all
if "%MODE%"=="debug" goto build_debug
if "%MODE%"=="release" goto build_release
if "%MODE%"=="standalone" goto build_standalone
goto end

:build_all
echo 🔨 Compilation en mode Debug...
call dotnet build "%PROJECTPATH%%PROJECTNAME%.sln" -c Debug
if %errorlevel% equ 0 (echo ✅ Debug réussi) else (echo ❌ Debug échoué)
echo.
echo 🔨 Compilation en mode Release...
call dotnet build "%PROJECTPATH%%PROJECTNAME%.sln" -c Release
if %errorlevel% equ 0 (echo ✅ Release réussi) else (echo ❌ Release échoué)
echo.
goto build_standalone

:build_debug
echo 🔨 Compilation en mode Debug...
call dotnet build "%PROJECTPATH%%PROJECTNAME%.sln" -c Debug
goto end

:build_release
echo 🔨 Compilation en mode Release...
call dotnet build "%PROJECTPATH%%PROJECTNAME%.sln" -c Release
goto end

:build_standalone
echo 📦 Publication Standalone (self-contained)...
echo    ℹ️  Inclut SQLite natif pour la gestion de l'historique
call dotnet publish "%PROJECTPATH%%PROJECTNAME%.sln" -c Release -r win-x64 /p:PublishSingleFile=true /p:SelfContained=true /p:PublishReadyToRun=true /p:EnableCompressionInSingleFile=true

if %errorlevel% equ 0 (
    echo ✅ Publication Standalone réussie
    echo.
    echo 📦 Préparation du dossier Distribution...
    
    powershell -Command "& {$distFolder = '%PROJECTPATH%Distribution'; $publishFolder = '%PROJECTPATH%bin\Release\net10.0-windows\win-x64\publish'; $readmePath = '%PROJECTPATH%README.md'; $sqlitePath = '%PROJECTPATH%docs\SQLITE_IMPLEMENTATION.md'; if (Test-Path $distFolder) { Remove-Item $distFolder -Recurse -Force }; New-Item -ItemType Directory -Path $distFolder -Force | Out-Null; Get-ChildItem \"$publishFolder\*\" -Exclude *.pdb | Copy-Item -Destination $distFolder -Force -Recurse; if (Test-Path $readmePath) { Copy-Item $readmePath -Destination \"$distFolder\README.md\" -Force }; if (Test-Path $sqlitePath) { Copy-Item $sqlitePath -Destination \"$distFolder\SQLITE_IMPLEMENTATION.md\" -Force }; Write-Host '✅ Dossier Distribution préparé'; Write-Host \"   📁 $distFolder\"; Write-Host \"   📄 README.md copié\"; Write-Host \"   📄 SQLITE_IMPLEMENTATION.md copié\"}"
) else (
    echo ❌ Publication Standalone échouée
)

:end
echo.
echo ================================================
echo   Emplacements des fichiers
echo ================================================
echo   Debug:        bin\Debug\net10.0-windows\CalculatriceMargeWPF.exe
echo   Release:      bin\Release\net10.0-windows\CalculatriceMargeWPF.exe
echo   Standalone:   bin\Release\net10.0-windows\win-x64\publish\CalculatriceMargeWPF.exe
echo   Distribution: Distribution\
echo.
echo   ℹ️  Base de données SQLite: %%APPDATA%%\CalculatriceMarge\Historique\historique.db
echo.
