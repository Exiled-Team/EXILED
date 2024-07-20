---
sidebar_position: 1
---

# Automatic Windows Installation

Download `Exiled.Installer-Win.exe` from [here](https://github.com/Exiled-Team/EXILED/releases).

Move it into your **server directory** and double click the .exe.
- Make sure the server directory is the one where LocalAdmin.exe is found.

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
Using powershell.

```powershell title="Basic installation in the folder you are in"
.\Exiled.Installer-Win --pre-releases
```

```powershell title="Installation in a specific folder, specific version and specific appdata folder"
.\Exiled.Installer-Win -p D:\Games\SCPSL\Server --appdata C --target-version 2.0.8
```