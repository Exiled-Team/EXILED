# Installer/Updater Improvements
The current experience of using EXILED can feel clucky at times. For example, during an SL beta, a user has to figure out two different branches (most likely release and pre-release), and has to reinstall EXILED. In addition to this, if a release isn't marked correctly, the updater will break, which is an all-too-easy mistake to make given the fact that it's only a single button. 

The purpose of these changes is to provide a near seamless experience while using EXILED, by managing and correctly updating the EXILED version, especially between updates.

In order to accomplish this, an EXILED "launcher" will be created, which makes sure that the game assembly is always bootstrapped, so between game versions, there's no need for someone to update the assembly. Along with this, the updater will work aware of the current game version (for example, it won't download an EXILED version for SL 11 while on SL 10). Both of these will be based on a new installer, which accepts a specific version to install.

## Installer
The installer will work similarity to the current version, although it will accept an argument of which version to install. If none is given, it will be identical to the current installer.

## Launcher
The launcher's one purpose is to ensure that EXILED starts. It bootstraps EXILED if the assembly changed (by saving the digest of the file), and installs it if it doesn't exist (using the latest version through the installer).

The launcher is essentially a drop-in replacement for LocalAdmin. It does the above actions, and then launches LocalAdmin with the arguments given to it.

By using the launcher, EXILED will always be loaded, although it may not be the correct version (it has no way of determining the game version without injecting).

## Updater
The updater functions similarly to the current updater, although it will act version aware and only installs the EXILED version with the game version closest to its game version.

It does this by reading a json file stored in the EXILED repository, which contains a list of EXILED version, game version, and whether it is a pre-release. This file is read by the updater to determine the most applicable game version, and then it installs it via the installer.

## Usage
The standard usage of this would be as follows:
 1. User places EXILED launcher into the server directory, then runs it.
 2. The launcher detects that EXILED is not installed, and installs the latest version.
 3. The server is started with LocalAdmin.
 4. EXILED updater determines that a better version is available, and so installs it.
 5. The server is restarted, and EXILED is correctly installed.
 
Then, if a game version comes out and the user updates:
 1. The EXILED launcher is ran.
 2. The launcher detects that the assembly changed, and bootstraps it.
 3. The server is started.
 4. EXILED updater determines that a better version is available, and so installs it.
 5. The server is restarted, and EXILED is updated.

Then, when the server next restarts:
 1. The EXILED launcher is ran.
 2. The server is started.
 3. EXILED is installed correctly.

In the case of someone switching to the launcher, the following might occur.
 1. The EXILED launcher is ran.
 2. The assembly is bootstrapped.
 3. The server is started, and EXILED is installed correctly.

If someone does not use the launcher, EXILED will still need to be reinstalled between game updates.
