; Inno Setup Script for CalculatriceMarge WPF v2.1.0
; Author: C. Lecomte
; Description: Professional WPF application for calculating commercial margins
; GitHub: https://github.com/christwadel65-ux/CalculatriceMarge

#define MyAppName "CalculatriceMarge"
#define MyAppVersion "2.1.0"
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
DefaultDirName={pf32}\{#MyAppName}
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
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"

[CustomMessages]
en.WelcomeText=Welcome to {#MyAppName} Setup!
en.WelcomeSubText=This will install {#MyAppName} v{#MyAppVersion} on your computer.
fr.WelcomeText=Bienvenue dans l'installation de {#MyAppName}!
fr.WelcomeSubText=Cela va installer {#MyAppName} v{#MyAppVersion} sur votre ordinateur.

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startmenu"; Description: "Create Start Menu shortcut"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
; Main application files from self-contained Release build (win-x64 folder)
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
; License file
Source: "..\LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
; Documentation files
Source: "..\docs\*"; DestDir: "{app}\docs"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "Launch {#MyAppName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName} now"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: dirifempty; Name: "{app}"
