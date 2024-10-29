---
sidebar_position: 2
---

# Automatic Linux Installation

Download `Exiled.Installer-Linux` from [here](https://github.com/Exiled-Team/EXILED/releases).

Move it into your **server directory** and run it using `./Exiled.Installer-Linux`
- Make sure the server directory is the one where LocalAdmin executable is found.

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

- ##### Installation in a specific folder, specific version and specific appdata folder
```powershell title="Basic installation in the folder you are in"
.\Exiled.Installer-Linux --pre-releases
```

```powershell title="Installation in a specific folder, specific version and specific appdata folder"
.\Exiled.Installer-Linux -p /home/user/scpsl/server --appdata /home/user/scpsl --target-version 2.0.8
```