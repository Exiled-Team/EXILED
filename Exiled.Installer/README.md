### TL;DR

EXILED.Installer - EXILED online installer. Downloads the latest release from the GitHub repository and installs it.

#### Usage

```
Usage:
  Exiled.Installer [options] [[--] <additional arguments>...]]

Options:
  -p, --path <path> (REQUIRED)         Path to the folder with the SL server [default: YourWorkingFolder]
  --appdata <appdata> (REQUIRED)       Forces the folder to be the AppData folder (useful for containers when pterodactyl runs as root) [default: YourAppDataPath]
  --pre-releases                       Includes pre-releases [default: False]
  --target-version <target-version>    Target version for installation
  --github--token <github--token>      Uses a token for auth in case the rate limit is exceeded (no permissions required)
  --exit                               Automatically exits the application anyway
  --get-versions                       Gets all possible versions for installation
  --version                            Show version information
  -?, -h, --help                       Show help and usage information

Additional Arguments:
  Arguments passed to the application that is being run.
```

-----

#### Examples

- ##### Basic installation in the folder you are in

```
PS E:\SteamLibrary\steamapps\common\SCP Secret Laboratory Dedicated Server> .\Exiled.Installer-Win --pre-releases
Exiled.Installer-Win-3.1.0.0
AppData folder: YourAppDataPath
Receiving releases...
Prereleases included - True
Target release version - 2.1.4
Searching for the latest release that matches the parameters...
Release found!
PRE: True | ID: 29240140 | TAG: 2.0.10
Asset found!
ID: 23562200 | NAME: Exiled.tar.gz | SIZE: 1171630 | URL: https://api.github.com/repos/galaxy119/EXILED/releases/assets/23562200 | DownloadURL: https://github.com/galaxy119/EXILED/releases/download/2.0.10/Exiled.tar.gz
Processing 'EXILED\Plugins\Exiled.Updater.dll'
Extracting 'Exiled.Updater.dll' into 'YourAppDataPath\EXILED\Plugins\Exiled.Updater.dll'...
Processing 'EXILED\Plugins\dependencies\0Harmony.dll'
Extracting '0Harmony.dll' into 'YourAppDataPath\EXILED\Plugins\dependencies\0Harmony.dll'...
Processing 'EXILED\Plugins\dependencies\Exiled.API.dll'
Extracting 'Exiled.API.dll' into 'YourAppDataPath\EXILED\Plugins\dependencies\Exiled.API.dll'...
Processing 'EXILED\Plugins\dependencies\YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into 'YourAppDataPath\EXILED\Plugins\dependencies\YamlDotNet.dll'...
Processing 'EXILED\Plugins\Exiled.Permissions.dll'
Extracting 'Exiled.Permissions.dll' into 'YourAppDataPath\EXILED\Plugins\Exiled.Permissions.dll'...
Processing 'EXILED\Plugins\Exiled.Events.dll'
Extracting 'Exiled.Events.dll' into 'YourAppDataPath\EXILED\Plugins\Exiled.Events.dll'...
Processing 'EXILED\Exiled.Loader.dll'
Extracting 'Exiled.Loader.dll' into 'YourAppDataPath\EXILED\Exiled.Loader.dll'...
Processing 'Assembly-CSharp.dll'
Extracting 'Assembly-CSharp.dll' into 'E:\SteamLibrary\steamapps\common\SCP Secret Laboratory Dedicated Server\SCPSL_Data\Managed\Assembly-CSharp.dll'...
Installation complete
```

- ##### Installation in a specific folder, specific version and specific appdata folder

```
irebbok@iRebbok:~$ ./Exiled.Installer-Linux -p /home/irebbok/scpsl/dedi --appdata /home/irebbok/scpsl --target-version 2.0.8
Exiled.Installer-Linux-3.1.0.0
AppData folder: /home/irebbok/scpsl
Receiving releases...
Prereleases included - False
Target release version - 2.0.8
Searching for the latest release that matches the parameters...
Release found!
PRE: True | ID: 29168825 | TAG: 2.0.8
Asset found!
ID: 23461628 | NAME: Exiled.tar.gz | SIZE: 1130061 | URL: https://api.github.com/repos/galaxy119/EXILED/releases/assets/23461628 | DownloadURL: https://github.com/galaxy119/EXILED/releases/download/2.0.8/Exiled.tar.gz
Processing 'Assembly-CSharp.dll'
Extracting 'Assembly-CSharp.dll' into '/home/irebbok/scpsl/dedi/SCPSL_Data/Managed/Assembly-CSharp.dll'...
Processing 'EXILED/Exiled.Loader.dll'
Extracting 'Exiled.Loader.dll' into '/home/irebbok/scpsl/EXILED/Exiled.Loader.dll'...
Processing 'EXILED/Plugins/dependencies/0Harmony.dll'
Extracting '0Harmony.dll' into '/home/irebbok/scpsl/EXILED/Plugins/dependencies/0Harmony.dll'...
Processing 'EXILED/Plugins/dependencies/Exiled.API.dll'
Extracting 'Exiled.API.dll' into '/home/irebbok/scpsl/EXILED/Plugins/dependencies/Exiled.API.dll'...
Processing 'EXILED/Plugins/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into '/home/irebbok/scpsl/EXILED/Plugins/dependencies/YamlDotNet.dll'...
Processing 'EXILED/Plugins/Exiled.Events.dll'
Extracting 'Exiled.Events.dll' into '/home/irebbok/scpsl/EXILED/Plugins/Exiled.Events.dll'...
Processing 'EXILED/Plugins/Exiled.Permissions.dll'
Extracting 'Exiled.Permissions.dll' into '/home/irebbok/scpsl/EXILED/Plugins/Exiled.Permissions.dll'...
Processing 'EXILED/Plugins/Exiled.Updater.dll'
Extracting 'Exiled.Updater.dll' into '/home/irebbok/scpsl/EXILED/Plugins/Exiled.Updater.dll'...
Installation complete
```
