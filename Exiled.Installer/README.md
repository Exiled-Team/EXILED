### TL;DR

Exiled.Installer - EXILED online installer. Downloads the latest release from the GitHub repository and installs it.

#### Usage

```
Usage:
  Exiled.Installer [options] [[--] <additional arguments>...]]

Options:
  -p, --path <path> (REQUIRED)         Path to the folder with the SL server [default: YourWorkingFolder]
  --appdata <appdata> (REQUIRED)       Forces the folder to be the AppData folder (useful for containers when pterodactyl runs as root) [default: YourAppDataPath]
  --exiled <exiled> (REQUIRED)         Indicates the Exiled root folder [default: YourAppDataPath]
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
user@user:~/SCP# ./Exiled.Installer-Linux --pre-releases
Exiled.Installer-Linux-3.2.3.0
AppData folder: YourAppDataPath
Exiled folder: YourAppDataPath
Receiving releases...
Prereleases included - True
Target release version - (null)
Searching for the latest release that matches the parameters...
Trying to find release..
Release found!
PRE: True | ID: 87710626 | TAG: 6.0.0-beta.18
Asset found!
ID: 90263995 | NAME: Exiled.tar.gz | SIZE: 1027928 | URL: https://api.github.com/repos/Exiled-Team/Exiled-EA/releases/assets/90263995 | DownloadURL: https://github.com/Exiled-Team/Exiled-EA/releases/download/6.0.0-beta.18/Exiled.tar.gz
Processing 'EXILED/Plugins/dependencies/0Harmony.dll'
Extracting '0Harmony.dll' into 'YourAppDataPath/EXILED/Plugins/dependencies/0Harmony.dll'...
Processing 'EXILED/Plugins/dependencies/Exiled.API.dll'
Extracting 'Exiled.API.dll' into 'YourAppDataPath/EXILED/Plugins/dependencies/Exiled.API.dll'...
Processing 'EXILED/Plugins/dependencies/SemanticVersioning.dll'
Extracting 'SemanticVersioning.dll' into 'YourAppDataPath/EXILED/Plugins/dependencies/SemanticVersioning.dll'...
Processing 'EXILED/Plugins/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into 'YourAppDataPath/EXILED/Plugins/dependencies/YamlDotNet.dll'...
Processing 'EXILED/Plugins/Exiled.CreditTags.dll'
Extracting 'Exiled.CreditTags.dll' into 'YourAppDataPath/EXILED/Plugins/Exiled.CreditTags.dll'...
Processing 'EXILED/Plugins/Exiled.CustomItems.dll'
Extracting 'Exiled.CustomItems.dll' into 'YourAppDataPath/EXILED/Plugins/Exiled.CustomItems.dll'...
Processing 'EXILED/Plugins/Exiled.CustomRoles.dll'
Extracting 'Exiled.CustomRoles.dll' into 'YourAppDataPath/EXILED/Plugins/Exiled.CustomRoles.dll'...
Processing 'EXILED/Plugins/Exiled.Events.dll'
Extracting 'Exiled.Events.dll' into 'YourAppDataPath/EXILED/Plugins/Exiled.Events.dll'...
Processing 'EXILED/Plugins/Exiled.Permissions.dll'
Extracting 'Exiled.Permissions.dll' into 'YourAppDataPath/EXILED/Plugins/Exiled.Permissions.dll'...
Processing 'EXILED/Plugins/Exiled.Updater.dll'
Extracting 'Exiled.Updater.dll' into 'YourAppDataPath/EXILED/Plugins/Exiled.Updater.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Exiled.API.dll'
Extracting 'Exiled.API.dll' into 'YourAppDataPath/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Exiled.API.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into 'YourAppDataPath/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/Exiled.Loader.dll'
Extracting 'Exiled.Loader.dll' into 'YourAppDataPath/SCP Secret Laboratory/PluginAPI/plugins/7777/Exiled.Loader.dll'...
Installation complete
```

- ##### Installation in a specific folder, specific version and specific appdata folder

```
user@user:~/SCP# ./Exiled.Installer-Linux --appdata /user/SCP --exiled /user/SCP
Exiled.Installer-Linux-3.2.3.0
AppData folder: /user/SCP
Exiled folder: /user/SCP
Receiving releases...
Prereleases included - False
Target release version - (null)
Searching for the latest release that matches the parameters...
Trying to find release..
Release found!
PRE: False | ID: 87710626 | TAG: 6.0.0-beta.18
Asset found!
ID: 90263995 | NAME: Exiled.tar.gz | SIZE: 1027928 | URL: https://api.github.com/repos/Exiled-Team/Exiled-EA/releases/assets/90263995 | DownloadURL: https://github.com/Exiled-Team/Exiled-EA/releases/download/6.0.0-beta.18/Exiled.tar.gz
Processing 'EXILED/Plugins/dependencies/0Harmony.dll'
Extracting '0Harmony.dll' into '/user/SCP/EXILED/Plugins/dependencies/0Harmony.dll'...
Processing 'EXILED/Plugins/dependencies/Exiled.API.dll'
Extracting 'Exiled.API.dll' into '/user/SCP/EXILED/Plugins/dependencies/Exiled.API.dll'...
Processing 'EXILED/Plugins/dependencies/SemanticVersioning.dll'
Extracting 'SemanticVersioning.dll' into '/user/SCP/EXILED/Plugins/dependencies/SemanticVersioning.dll'...
Processing 'EXILED/Plugins/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into '/user/SCP/EXILED/Plugins/dependencies/YamlDotNet.dll'...
Processing 'EXILED/Plugins/Exiled.CreditTags.dll'
Extracting 'Exiled.CreditTags.dll' into '/user/SCP/EXILED/Plugins/Exiled.CreditTags.dll'...
Processing 'EXILED/Plugins/Exiled.CustomItems.dll'
Extracting 'Exiled.CustomItems.dll' into '/user/SCP/EXILED/Plugins/Exiled.CustomItems.dll'...
Processing 'EXILED/Plugins/Exiled.CustomRoles.dll'
Extracting 'Exiled.CustomRoles.dll' into '/user/SCP/EXILED/Plugins/Exiled.CustomRoles.dll'...
Processing 'EXILED/Plugins/Exiled.Events.dll'
Extracting 'Exiled.Events.dll' into '/user/SCP/EXILED/Plugins/Exiled.Events.dll'...
Processing 'EXILED/Plugins/Exiled.Permissions.dll'
Extracting 'Exiled.Permissions.dll' into '/user/SCP/EXILED/Plugins/Exiled.Permissions.dll'...
Processing 'EXILED/Plugins/Exiled.Updater.dll'
Extracting 'Exiled.Updater.dll' into '/user/SCP/EXILED/Plugins/Exiled.Updater.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Exiled.API.dll'
Extracting 'Exiled.API.dll' into '/user/SCP/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/Exiled.API.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'
Extracting 'YamlDotNet.dll' into '/user/SCP/SCP Secret Laboratory/PluginAPI/plugins/7777/dependencies/YamlDotNet.dll'...
Processing 'SCP Secret Laboratory/PluginAPI/plugins/7777/Exiled.Loader.dll'
Extracting 'Exiled.Loader.dll' into '/user/SCP/SCP Secret Laboratory/PluginAPI/plugins/7777/Exiled.Loader.dll'...
Installation complete
```
