; Inno Setup Script for CalculatriceMarge WPF v2.2.0
; Author: C. Lecomte
; Description: Professional WPF application for calculating commercial margins
; GitHub: https://github.com/christwadel65-ux/CalculatriceMarge
;
; IMPORTANT: Run "build.bat standalone" first to create the publish folder
; Or use "installer\build_installer.bat" to automate the entire process

#define MyAppName "CalculatriceMarge"
#define MyAppVersion "2.2.0"
#define MyAppPublisher "C. Lecomte"
#define MyAppURL "https://github.com/christwadel65-ux/CalculatriceMarge"
#define MyAppExeName "CalculatriceMargeWPF.exe"
#define SourceDir "..\bin\Release\net10.0-windows\win-x64\publish"

[Setup]
AppId={{7F3B9C2A-8D4E-4B2C-9F1E-6D5A3C8B2E1F}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={commonpf32}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename=CalculatriceMargeWPF-{#MyAppVersion}-Setup
WizardStyle=modern
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
UsePreviousAppDir=yes
ShowLanguageDialog=auto
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

[Languages]
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startmenu"; Description: "Créer un raccourci Menu Démarrer"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Main application files from self-contained Release build (win-x64 folder)
; Includes SQLite native libraries embedded in the executable
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
; License file
Source: "..\LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
; Main documentation
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion
; Release notes and technical documentation
Source: "..\RELEASE_NOTES_V2.2.0.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\docs\SQLITE_IMPLEMENTATION.md"; DestDir: "{app}\docs"; Flags: ignoreversion
; User guides
Source: "..\docs\guides\*"; DestDir: "{app}\docs\guides"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "Lancer {#MyAppName}"
Name: "{group}\Documentation"; Filename: "{app}\README.md"; Comment: "Documentation complète"
Name: "{group}\Nouveautés v2.2.0"; Filename: "{app}\RELEASE_NOTES_V2.2.0.md"; Comment: "Notes de version SQLite"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Lancer {#MyAppName} maintenant"; Flags: nowait postinstall skipifsilent

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    MsgBox('Installation terminée !' + #13#10 + #13#10 + 
           'Version 2.2.0 avec base de données SQLite intégrée.' + #13#10 + #13#10 +
           'Nouveautés :' + #13#10 +
           '• Historique robuste et performant' + #13#10 +
           '• Migration automatique des données' + #13#10 +
           '• Recherche et statistiques améliorées' + #13#10 +
           '• Gestionnaire de base de données intégré' + #13#10 +
           '  (Menu Outils > Gestion base de données)' + #13#10 + #13#10 +
           'Base de données : %APPDATA%\CalculatriceMarge\Historique\historique.db', 
           mbInformation, MB_OK);
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  DataFolder: string;
  DatabaseFile: string;
begin
  if CurUninstallStep = usUninstall then
  begin
    { Vérifier si la base de données existe dans %APPDATA% }
    DataFolder := ExpandConstant('{commonappdata}\CalculatriceMarge\Historique');
    DatabaseFile := DataFolder + '\historique.db';
    
    if FileExists(DatabaseFile) then
    begin
      { Demander à l'utilisateur s'il veut conserver la base de données }
      if MsgBox('La base de données de votre historique a été trouvée.' + #13#10 + #13#10 +
                'Emplacement : ' + DatabaseFile + #13#10 + #13#10 +
                'Voulez-vous conserver votre historique après la désinstallation ?' + #13#10 +
                'Cliquez OUI pour conserver les données.' + #13#10 +
                'Cliquez NON pour supprimer tous les fichiers.',
                mbConfirmation, MB_YESNO) = IDNO then
      begin
        { Supprimer toute la base de données et les dossiers }
        DelTree(DataFolder, True, True, True);
      end
      else
      begin
        { Conserver la base de données, supprimer uniquement les fichiers temporaires }
        MsgBox('Votre base de données a été conservée.' + #13#10 +
               'Vous pouvez la réimporter lors d''une réinstallation ultérieure.' + #13#10 +
               'Dossier : ' + DataFolder,
               mbInformation, MB_OK);
      end;
    end;
  end;
end;

[Registry]
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"

[UninstallDelete]
Type: filesandordirs; Name: "{app}\docs\guides"
Type: filesandordirs; Name: "{app}\docs\releases"
Type: filesandordirs; Name: "{app}\docs"
Type: filesandordirs; Name: "{app}\resources"
Type: dirifempty; Name: "{app}"
