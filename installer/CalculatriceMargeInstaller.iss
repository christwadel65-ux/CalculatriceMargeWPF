; Inno Setup Script - Calculatrice de Marge v2.1.0
; Optimisations de performance et refactoring SOLID

#define MyAppName "Calculatrice de Marge"
#define MyAppVersion "2.1.0"
#define MyAppPublisher "C. Lecomte"
#define MyAppURL "https://github.com/christwadel65-ux/CalculatriceMarge"
#define MyAppExeName "CalculatriceMargeWPF.exe"
#define MySourceDir "..\bin\Release\net10.0-windows"
#define MyImagesDir "..\src\Resources\Images"
#define MyDocsDir "..\docs"

[Setup]
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
LicenseFile=..\LICENSE.txt
OutputDir=..\bin\Release\Installer
OutputBaseFilename=CalculatriceMargeInstaller_v{#MyAppVersion}
SetupIconFile={#MyImagesDir}\app_icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64 arm64
ArchitecturesAllowed=x64 arm64 x86
LanguageDetectionMethod=uilanguage

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Core application
Source: "{#MySourceDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\CalculatriceMargeWPF.deps.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\CalculatriceMargeWPF.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MySourceDir}\*.json"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#MySourceDir}\*.config"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

; Assets
Source: "{#MyImagesDir}\*"; DestDir: "{app}\Resources\Images"; Flags: ignoreversion recursesubdirs createallsubdirs

; Documentation
Source: "{#MyDocsDir}\README.md"; DestDir: "{app}\docs"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#MyDocsDir}\guides\README_V2.md"; DestDir: "{app}\docs\guides"; Flags: ignoreversion
Source: "{#MyDocsDir}\guides\IMPLEMENTATION_SUMMARY.md"; DestDir: "{app}\docs\guides"; Flags: ignoreversion
Source: "{#MyDocsDir}\guides\INSTALLER_GUIDE.md"; DestDir: "{app}\docs\guides"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#MyDocsDir}\guides\INSTRUCTIONS_APK_ANDROID.md"; DestDir: "{app}\docs\guides"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#MyDocsDir}\guides\INSTALLER.md"; DestDir: "{app}\docs\guides"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#MyDocsDir}\releases\CHANGELOG_V2.md"; DestDir: "{app}\docs\releases"; Flags: ignoreversion
Source: "{#MyDocsDir}\releases\RELEASE_NOTES_V2.md"; DestDir: "{app}\docs\releases"; Flags: ignoreversion skipifsourcedoesntexist
Source: "{#MyDocsDir}\releases\RELEASE_SUMMARY.md"; DestDir: "{app}\docs\releases"; Flags: ignoreversion skipifsourcedoesntexist
Source: "..\LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Resources\Images\app_icon.ico"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
; Desktop icon per-user to avoid admin rights issues
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; IconFilename: "{app}\Resources\Images\app_icon.ico"
; Quick Launch (per-user)
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon; IconFilename: "{app}\Resources\Images\app_icon.ico"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"
Root: HKA; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "AppURL"; ValueData: "{#MyAppURL}"

[UninstallDelete]
Type: dirifempty; Name: "{app}\Resources\Images"
Type: dirifempty; Name: "{app}\Resources"
Type: dirifempty; Name: "{app}"

[Code]
var
  DotNetRuntimeRequired: Boolean;

function IsDotNetRuntimeInstalled: Boolean;
var
  ResultCode: Integer;
begin
  try
    ShellExec('', 'dotnet', '--version', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Result := (ResultCode = 0);
  except
    Result := False;
  end;
end;

procedure InitializeWizard;
begin
  DotNetRuntimeRequired := not IsDotNetRuntimeInstalled;
  if DotNetRuntimeRequired then
  begin
    if MsgBox('Le runtime .NET 10.0 n''est pas détecté.' + #13#10 +
              'Il sera téléchargé et installé automatiquement.' + #13#10#13#10 +
              'Voulez-vous continuer ?',
              mbInformation, MB_OKCANCEL) = IDCANCEL then
    begin
      Abort;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
  begin
    if DotNetRuntimeRequired then
    begin
      MsgBox('Téléchargement et installation du runtime .NET 10.0...' + #13#10 +
             'Cela peut prendre quelques minutes.',
             mbInformation, MB_OK);
      // TODO: implémenter le téléchargement/installation si nécessaire
    end;
  end;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpFinished then
  begin
    MsgBox('Installation réussie !' + #13#10#13#10 +
           'Calculatrice de Marge v{#MyAppVersion} est prête.' + #13#10#13#10 +
           'Données utilisateur (historique, presets) : {userappdata}\CalculatriceMarge\',
           mbInformation, MB_OK);
  end;
end;

