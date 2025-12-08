; Inno Setup Script - Calculatrice de Marge v2.0
; ===============================================
; Script pour créer l'installateur Windows

#define MyAppName "Calculatrice de Marge"
#define MyAppVersion "2.0.0"
#define MyAppPublisher "C. Lecomte"
#define MyAppURL "https://github.com/christwadel65-ux/CalculatriceMarge"
#define MyAppExeName "CalculatriceMargeWPF.exe"
#define MySourceDir "bin\Release\net10.0-windows"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{9A8E4F9B-7C3D-4E2A-B1F6-3C9D8E7A2B4}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=LICENSE
OutputDir=bin\Release\Installer
OutputBaseFilename=CalculatriceMargeInstaller_v{#MyAppVersion}
SetupIconFile=Images\app_icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64 arm64
ArchitecturesAllowed=x64 arm64 ia32

; Langue par défaut
DefaultLanguage=french

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked and SkipIfNotExists
Name: "startup"; Description: "Lancer l'application au démarrage"; GroupDescription: "Options de démarrage"

[Files]
; Application principale
Source: "{#MySourceDir}\CalculatriceMargeWPF.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\CalculatriceMargeWPF.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MySourceDir}\*.exe"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; Assets et ressources
Source: "Images\*"; DestDir: "{app}\Images"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs

; Documentation
Source: "README_V2.md"; DestDir: "{app}"; Flags: ignoreversion isreadme
Source: "CHANGELOG_V2.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "LICENSE"; DestDir: "{app}"; Flags: ignoreversion
Source: "IMPLEMENTATION_SUMMARY.md"; DestDir: "{app}"; Flags: ignoreversion

; Runtime .NET (si nécessaire - optionnel si déjà installé)
; Source: "dotnet-runtime-10.0-windows-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall

; NOTE: Don't use "Flags: ignoreversion" on any shared system files.

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Images\app_icon.ico"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; IconFilename: "{app}\Images\app_icon.ico"
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon; IconFilename: "{app}\Images\app_icon.ico"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
; Création des clés de registre pour intégration Windows
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "AppURL"; ValueData: "{#MyAppURL}"

; Association de fichiers (optionnel - pour historique)
; Root: HKA; Subkey: "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.marge"; ValueType: string; ValueName: "ProgID"; ValueData: "CalculatriceMarge.Historique"

[UninstallDelete]
; Supprimer les dossiers créés lors de l'installation
Type: dirifempty; Name: "{app}\Images"
Type: dirifempty; Name: "{app}\Resources"
Type: dirifempty; Name: "{app}"

; Garder le dossier AppData (historique et presets de l'utilisateur)
; Type: dirifempty; Name: "{userappdata}\CalculatriceMarge"

[Code]
// Variables globales
var
  DotNetRuntimeRequired: Boolean;

// Fonction de détection .NET Runtime
function IsDotNetRuntimeInstalled: Boolean;
var
  ResultCode: Integer;
begin
  try
    // Vérifier si .NET runtime 10.0 est installé
    ShellExec('', 'dotnet', '--version', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Result := (ResultCode = 0);
  except
    Result := False;
  end;
end;

// Initialisation du wizard
procedure InitializeWizard;
begin
  DotNetRuntimeRequired := not IsDotNetRuntimeInstalled;
  
  if DotNetRuntimeRequired then
  begin
    if MsgBox('Le runtime .NET 10.0 n''est pas détecté sur ce système.' + #13#10 +
              'Il sera téléchargé et installé automatiquement.' + #13#10#13#10 +
              'Voulez-vous continuer?',
              mbInformation, MB_OKCANCEL) = IDCANCEL then
    begin
      Abort;
    end;
  end;
end;

// Avant l'installation
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
  begin
    // Vérifier .NET Runtime
    if DotNetRuntimeRequired then
    begin
      MsgBox('Téléchargement et installation du runtime .NET 10.0...' + #13#10 +
             'Cela peut prendre quelques minutes.',
             mbInformation, MB_OK);
      
      // Télécharger et installer .NET Runtime
      // (À implémenter avec WinHttpReq ou similaire si nécessaire)
    end;
  end;
end;

// Après l'installation
procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpFinished then
  begin
    MsgBox('Installation réussie!' + #13#10#13#10 +
           'Calculatrice de Marge v{#MyAppVersion} est prête à être utilisée.' + #13#10#13#10 +
           'Les données d''historique et presets sont sauvegardés dans:' + #13#10 +
           '{userappdata}\CalculatriceMarge\' + #13#10#13#10 +
           'Merci d''utiliser cette application!',
           mbInformation, MB_OK);
  end;
end;

// Message de désinstallation
procedure DeinitializeSetup;
begin
  if not UsingWizard and (ExitCode <> 0) then
  begin
    MsgBox('L''installation a été annulée ou a échoué.', mbInformation, MB_OK);
  end;
end;
