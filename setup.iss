; Script Inno Setup pour Calculatrice de Marge
; Génère un installateur Windows pour l'application

#define MyAppName "Calculatrice de Marge"
#define MyAppVersion "1.0.5"
#define MyAppPublisher "C.Lecomte"
#define MyAppExeName "CalculatriceMargeWPF.exe"
#define MyAppAssocName MyAppName + " File"
#define MyAppAssocExt ".cmarge"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt

[Setup]
; NOTE: La valeur de AppId identifie de manière unique cette application.
; Ne pas utiliser la même valeur AppId dans les installateurs d'autres applications.
; (Pour générer un nouveau GUID, cliquez sur Outils | Générer GUID dans l'IDE.)
AppId={{A8F9B2C1-3D4E-5F6A-7B8C-9D0E1F2A3B4C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=LICENSE.txt
InfoBeforeFile=
InfoAfterFile=
OutputDir=bin\Release\Installer
OutputBaseFilename=CalculatriceMarge_Setup_v{#MyAppVersion}
SetupIconFile=Images\app_icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Application de calcul de marge commerciale
VersionInfoCopyright=Copyright (C) 2025
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "bin\Release\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: N'utilisez pas "Flags: ignoreversion" sur les fichiers système

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{group}\Guide d'utilisation"; Filename: "{app}\README.md"
Name: "{group}\Licence"; Filename: "{app}\LICENSE.txt"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Flags: nowait postinstall skipifsilent; Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"

[Code]
function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
begin
  // Vérifier si .NET 10.0 ou supérieur est installé
  if not RegKeyExists(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost') then
  begin
    if MsgBox('Cette application nécessite .NET Desktop Runtime 10.0 ou supérieur.' + #13#10 + 
              'Voulez-vous télécharger et installer .NET maintenant ?', 
              mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 
        'https://dotnet.microsoft.com/download/dotnet/10.0', 
        '', '', SW_SHOWNORMAL, ewNoWait, ResultCode);
    end;
    Result := False;
  end
  else
    Result := True;
end;
