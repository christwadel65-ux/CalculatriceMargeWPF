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
CloseApplications=yes
RestartApplications=yes

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
procedure InitializeWizard();
var
  PreviousVersionInstalled: Boolean;
  PreviousPath: string;
  UninstallExePath: string;
  UninstallString: string;
  ResultCode: Integer;
begin
  { Vérifier si une version antérieure est installée dans la registre }
  PreviousVersionInstalled := RegQueryStringValue(HKEY_LOCAL_MACHINE, 
    'Software\Microsoft\Windows\CurrentVersion\Uninstall\CalculatriceMarge_is1',
    'InstallLocation', PreviousPath);
  
  if PreviousVersionInstalled and (PreviousPath <> '') then
  begin
    { Essayer de récupérer le chemin de désinstallation }
    if not RegQueryStringValue(HKEY_LOCAL_MACHINE,
      'Software\Microsoft\Windows\CurrentVersion\Uninstall\CalculatriceMarge_is1',
      'UninstallString', UninstallString) then
    begin
      { Si pas de UninstallString, vérifier dans CommonFiles }
      UninstallExePath := ExpandConstant('{commonfil32}\CalculatriceMarge\unins000.exe');
      if not FileExists(UninstallExePath) then
        UninstallExePath := PreviousPath + 'unins000.exe';
    end
    else
    begin
      { Utiliser le chemin de UninstallString }
      UninstallExePath := UninstallString;
    end;
    
    { Avertir l'utilisateur qu'une version antérieure a été détectée }
    if MsgBox('✅ Détection de version antérieure' + #13#10 + #13#10 +
              'Une version antérieure de CalculatriceMarge a été trouvée.' + #13#10 + #13#10 +
              'Installation détectée : ' + PreviousPath + #13#10 + #13#10 +
              'La version actuelle va être désinstallée et remplacée par v2.2.0.' + #13#10 +
              'Vos données historiques seront préservées.' + #13#10 + #13#10 +
              'Voulez-vous continuer ?',
              mbConfirmation, MB_YESNO) = IDYES then
    begin
      { Désinstaller silencieusement l'ancienne version }
      if FileExists(UninstallExePath) then
      begin
        if Exec(UninstallExePath, '/SILENT /NORESTART', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
        begin
          { La désinstallation s'est bien passée }
          MsgBox('✅ Ancien programme désinstallé' + #13#10 + #13#10 +
                 'Vous pouvez maintenant continuer avec l''installation de v2.2.0.',
                 mbInformation, MB_OK);
        end
        else
        begin
          { Erreur lors de la désinstallation }
          MsgBox('⚠️ Erreur lors de la désinstallation' + #13#10 + #13#10 +
                 'Impossible de désinstaller l''ancienne version.' + #13#10 +
                 'Code d''erreur : ' + IntToStr(ResultCode) + #13#10 + #13#10 +
                 'Voulez-vous quand même continuer avec l''installation ?',
                 mbError, MB_YESNO);
        end;
      end
      else
      begin
        MsgBox('⚠️ Programme de désinstallation non trouvé' + #13#10 + #13#10 +
               'Localisation supposée : ' + UninstallExePath + #13#10 + #13#10 +
               'L''ancienne version sera remplacée par les nouveaux fichiers.',
               mbError, MB_OK);
      end;
    end
    else
    begin
      Abort();
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ApplicationPath: string;
begin
  { Avant l'installation, nettoyer les anciens fichiers }
  if CurStep = ssInstall then
  begin
    ApplicationPath := ExpandConstant('{app}\{#MyAppExeName}');
    
    { Essayer de supprimer l'ancien exécutable pour libérer la place }
    if FileExists(ApplicationPath) then
    begin
      if not DeleteFile(ApplicationPath) then
      begin
        MsgBox('⚠️ Le programme est en cours d''exécution' + #13#10 + #13#10 +
                'Impossible de remplacer le programme tant qu''il est ouvert.' + #13#10 + #13#10 +
                'Veuillez fermer CalculatriceMarge complètement et relancer l''installation.' + #13#10 +
                'Cliquez OK après avoir fermé le programme.',
                mbError, MB_OK);
        Abort();
      end;
    end;
  end;
  
  { Message de succès après l'installation }
  if CurStep = ssPostInstall then
  begin
    MsgBox('✅ Installation/Mise à jour réussie !' + #13#10 + #13#10 + 
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
                'Voulez-vous conserver votre historique après la désinstallation ?' + #13#10 + #13#10 +
                'Cliquez OUI pour conserver les données.' + #13#10 +
                'Cliquez NON pour supprimer tous les fichiers de la base de données.',
                mbConfirmation, MB_YESNO) = IDNO then
      begin
        { Confirmation supplémentaire avant suppression }
        if MsgBox('⚠️ ATTENTION - SUPPRESSION DÉFINITIVE' + #13#10 + #13#10 +
                  'Vous êtes sur le point de supprimer définitivement votre historique.' + #13#10 + #13#10 +
                  'Cette action est IRRÉVERSIBLE.' + #13#10 + #13#10 +
                  'Êtes-vous absolument sûr de vouloir supprimer la base de données ?' + #13#10 +
                  'Dossier à supprimer : ' + DataFolder,
                  mbConfirmation, MB_YESNO) = IDYES then
        begin
          { Supprimer toute la base de données et les dossiers }
          if DelTree(DataFolder, True, True, True) then
          begin
            MsgBox('✅ Suppression réussie' + #13#10 + #13#10 +
                   'Votre base de données et tous vos historiques ont été supprimés.',
                   mbInformation, MB_OK);
          end
          else
          begin
            MsgBox('⚠️ Erreur lors de la suppression' + #13#10 + #13#10 +
                   'Impossible de supprimer complètement la base de données.' + #13#10 +
                   'Vous pouvez la supprimer manuellement dans :' + #13#10 +
                   DataFolder,
                   mbError, MB_OK);
          end;
        end
        else
        begin
          { Utilisateur a annulé la suppression }
          MsgBox('Suppression annulée.' + #13#10 +
                 'Votre base de données a été conservée.' + #13#10 +
                 'Dossier : ' + DataFolder,
                 mbInformation, MB_OK);
        end;
      end
      else
      begin
        { Conserver la base de données }
        MsgBox('✅ Conservation réussie' + #13#10 + #13#10 +
               'Votre base de données a été conservée.' + #13#10 +
               'Vous pouvez la réimporter lors d''une réinstallation ultérieure.' + #13#10 + #13#10 +
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
