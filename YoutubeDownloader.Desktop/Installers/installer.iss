#define MyAppName "Youtube Downloader"
#define MyAppVersion "1.0.0"
#define MyAppExeName "YoutubeDownloader.Desktop.exe"
#define PublishDir "C:\Users\Tiago\source\repos\YoutubeDownloader\YoutubeDownloader.Desktop\bin\Release\net9.0-windows10.0.19041.0\win-x64\publish\"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={localappdata}\Programs\YoutubeDownloader
DefaultGroupName={#MyAppName}
OutputDir=..\..\output
OutputBaseFilename=YoutubeDownloaderSetup
Compression=lzma
SolidCompression=yes

PrivilegesRequired=lowest
DisableDirPage=no

[Files]
Source: "{#PublishDir}*.*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs ignoreversion
Source: "{#PublishDir}wwwroot\*"; DestDir: "{app}\wwwroot"; Flags: recursesubdirs createallsubdirs ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Abrir {#MyAppName}"; Flags: nowait postinstall skipifsilent