### TL;DR
EXILED.Installer - EXILED online installer. Downloads the latest release from the GitHub repository and installs it.

#### Usage
```
Usage:
  Exiled.Installer [options] [[--] <additional arguments>...]]

Options:
  -p, --path <path>    Path to the folder with the SL server [default: ]
  --pre-releases       Includes pre-releases [default: False]
  --version            Show version information
  -?, -h, --help       Show help and usage information

Additional Arguments:
  Arguments passed to the application that is being run.
```

**You can just run EXILED.Installer in the game directory.**

#### Arguments
`-p`/`--path` - path to the game directory, in case the application is launched from another directory.

`--pre-releases` - Includes pre-releases EXILED (if launched without arguments (from the game folder) true by default, after release 2.0.0 it'll be false).
