<div align="center">
  <img src="/assets/logo.svg" alt="Logo" width="96px" />
</div>
<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
  <a href="https://github.com/Exiled-Team/EXILED/releases/latest" target="_blank">
    <img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="Build" />
  </a>
  <a href="https://github.com/Exiled-Team/EXILED/releases" target="_blank">
    <img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" alt="Releases" />
  </a>
  <a href="https://github.com/Exiled-Team/EXILED/releases/latest" target="_blank">
    <img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads" />
  </a>
  <a href="https://github.com/Exiled-Team/EXILED/commits/dev" target="_blank">
    <img src="https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/dev?style=for-the-badge&logo=git" alt="Commits" />
  </a>
  <a href="https://discord.gg/exiledreboot" target="_blank">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Discord" />
  </a>
</div>

EXILED is a high-level plugin framework for SCP: Secret Laboratory servers. It offers an event system for developers to hook into in order to manipulate or change game code or implement their own functions.
All EXILED events are coded with Harmony, meaning they require no direct editing of server assemblies to function, which allows for two unique benefits.

 - Firstly, the entirety of the framework’s code can be freely published and shared, allowing developers to better understand *how* it works and offer suggestions for adding to or changing its features.
 - Secondly, since all the code related to the framework is done outside of the server assembly, things like small game updates will have little, if any, effect on the framework. Making it most likely to be compatible with future game updates, as well as making it easier to update when it *is* necessary to do so.

# Localized READMEs
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Italiano](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-IT.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)
- [Türkçe](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-TR.md)
- [German](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DE.md)
- [Français](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-FR.md)
- [한국어](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-KR.md)
- [ไทย](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ไทย.md)

# Installation
Installation of EXILED is quite simple. It loads itself through Northwood’s Plugin API. That's why there are two folders inside the ``Exiled.tar.gz`` in release files. ``SCP Secret Laboratory`` contains the necessary files to load EXILED features in ``EXILED`` folder. All you need to do is move these two folders into the appropriate path, which are explained below, and you are done! 

If you choose to use the installer it will, if run correctly, take care of installing all EXILED features.

# Windows
### Automatic installation ([more information](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Note**: Make sure you're on the user that runs the server, or you have Admin privileges before running the Installer.

  - Download the **`Exiled.Installer-Win.exe` [from here](https://github.com/Exiled-Team/EXILED/releases)** (click on Assets -> click the Installer)
  - Place it on your server folder (download the dedicated server if you haven't)
  - Double click the **`Exiled.Installer.exe`** or **[download this .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** and place it in the server folder to install the latest pre-release
  - To get and install plugins, check the [Installing plugins](#installing-plugins) section down below.
**Note:** If you are installing EXILED on a remote server, make sure you run the .exe as the same user that runs your SCP:SL servers (or one with Admin privileges)

### Manual installation
  - Download the **`Exiled.tar.gz` [from here](https://github.com/Exiled-Team/EXILED/releases)**
  - Extract its contents with [7Zip](https://www.7-zip.org/) or [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Move the **``EXILED``** folder to **`%appdata%`** *Note: This folder needs to go in ``C:\Users\%UserName%\AppData\Roaming``, and ***NOT*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, and **IT MUST** be in (...)\AppData\Roaming, not (...)\AppData\!*
  - Move **``SCP Secret Laboratory``** to **`%appdata%`**.
    - Windows 10 & 11:
      Write `%appdata%` in Cortana / the search icon, or the Windows Explorer bar.
    - Any other Windows version:
      Press Win + R and type `%appdata%`

### Installing plugins
That's it, EXILED should now be installed and active the next time you boot up your server. Note that EXILED by themselves will do almost nothing, so make sure to get new plugins from **[our Discord server](https://discord.gg/PyUkWTg)**
- To install a plugin, simply:
  - Download a plugin from [*their* releases page](https://i.imgur.com/u34wgPD.jpg) (**it MUST be a `.dll`!**)
  - Move it to: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins`` (move here by pressing Win + R, then writing `%appdata%`)

# Linux
### Automatic installation ([more information](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Note:** If you are installing EXILED on a remote server, make sure you run the Installer as the same user that runs your SCP:SL servers (or root)

  - Download the **`Exiled.Installer-Linux` [from here](https://github.com/Exiled-Team/EXILED/releases)** (click on Assets -> download the Installer)
  - Install it by either typing **`./Exiled.Installer-Linux --path /path/to/server`** or move it inside the server folder directly, move to it with the terminal (`cd`) and type: **`./Exiled.Installer-Linux`**.
  - If you want the latest pre-release, simply add **`--pre-releases`**. Example: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Another example, if you placed `Exiled.Installer-Linux` in your server folder: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - To get and install plugins, check the [Installing plugins](#installing-plugins-1) section down below.

### Manual installation
  - **Ensure** you are logged in on the user that runs the SCP servers.
  - Download the **`Exiled.tar.gz` [from here](https://github.com/Exiled-Team/EXILED/releases)** (SSH: right click and to get the `Exiled.tar.gz` link, then type: **`wget (link_to_download)`**)
  - To extract it to your current folder, type **``tar -xzvf EXILED.tar.gz``**
  - Move the **`EXILED`** folder to **``~/.config``**. *Note: This folder needs to go in ``~/.config``, and ***NOT*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Move the **`SCP Secret Laboratory`** folder to **``~/.config``**. *Note: This folder needs to go in ``~/.config``, and ***NOT*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Installing plugins
That's it, EXILED should now be installed and active the next time you boot up your server. Note that EXILED by themselves will do almost nothing, so make sure to get plugins from **[our Discord server](https://discord.gg/PyUkWTg)**
- To install a plugin, simply:
  - Download a plugin from [*their* releases page](https://i.imgur.com/u34wgPD.jpg) (**it MUST be a `.dll`!**)
  - Move it to: ``~/.config/EXILED/Plugins`` (if you use your SSH as root, then search for the correct `.config` which will be inside `/home/(SCP Server User)`)

# Config
EXILED by itself offers some config options.
All of them are auto-generated at the server startup, they are located at ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\Configs\(ServerPortHere)-config.yml`` on Windows).

Plugin configs will ***NOT*** be in the aforementioned ``config_gameplay.txt`` file, instead, plugin configs are set in the ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\(ServerPortHere)-config.yml`` on Windows).
However, some plugins might get their config settings from other locations on their own, this is simply the default EXILED location for them, so refer to the individual plugin if there are issues.

# For Developers

If you wish to make a plugin for EXILED, it's quite simple to do so. If you would like more of a tutorial please visit our [Getting Started Page.](https://exiled.to/Archive/GettingStarted).

For more comprehensive and actively updated tutorials, see [the EXILED website](https://exiled.to/Archive/Documentation).

But make sure to follow these rules when publishing your plugins:

 - Your plugin must contain a class that inherits from ``Exiled.API.Features.Plugin<>``, if it does not, EXILED will not load your plugin when the server starts.
 - When a plugin is loaded, the code within the aforementioned class' ``OnEnabled()`` method is executed immediately, it does not wait for other plugins to be loaded. It does not wait for the server startup process to finish. ***It does not wait for anything.*** When setting up your ``OnEnabled()`` method, be sure you are not accessing things which may not be initialized by the server yet, such as ``ServerConsole.Port``, or ``PlayerManager.localPlayer``.
 - If you need to access things early on that are not initialized before your plugin is loaded, it is recommended to wait for the ``WaitingForPlayers`` event to do so, if you need to do things sooner, wrap the code in a ``` while(!x)``` loop that checks for the variable/object you need to no longer be null before continuing.
 - EXILED supports dynamically reloading plugin assemblies mid-execution, meaning that if you need to update a plugin, it can be done without rebooting the server, however, if you are updating a plugin mid-execution, the plugin needs to be properly setup to support it, or you will have a very bad time. Refer to the ``Dynamic Updates`` section for more information and guidelines to follow.
 - There is ***NO*** OnUpdate, OnFixedUpdate or OnLateUpdate event within EXILED. If you need to run code that often, you can use a MEC coroutine that waits for one frame, 0.01f, or use a Timing layer like Timing.FixedUpdate instead.
 ### MEC Coroutines
If you are unfamiliar with MEC, this will be a very brief and simple primer to get you started.
MEC Coroutines are basically timed methods, that support waiting periods of time before continuing execution, without interrupting/sleeping the main game thread.
MEC coroutines are safe to use with Unity, unlike traditional threading. ***DO NOT try and make new threads to interact with Unity on, they WILL crash the server.***

To use MEC, you will need to reference ``Assembly-CSharp-firstpass.dll`` from the server files and include ``using MEC;``.
Example of calling a simple coroutine, that repeats itself with a delay between each loop:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //repeat the following infinitely
    {
        Log.Info("Hey I'm a infinite loop!"); //Call Log.Info to print a line to the game console/server logs.
        yield return Timing.WaitForSeconds(5f); //Tells the coroutine to wait 5 seconds before continuing, since this is at the end of the loop, it effectively stalls the loop from repeating for 5 seconds.
    }
}
```

It is ***strongly*** recommended that you do some googling or ask around in the Discord if you are unfamiliar with MEC and would like to learn more, get advice, or need help. Questions, no matter how 'stupid' they are, will always be answered as helpfully and clearly as possible to help plugin developers to excel. Better code is better for everyone.

### Dynamic Updates
EXILED as a framework supports dynamic reloading of plugin assemblies without requiring a server reboot.
For example, if you start the server with just `Exiled.Events` as the only plugin, and wish to add a new one, you do not need to reboot the server to complete this task. You can simply use the Remote Admin or Server Console command `reload plugins` to reload all EXILED plugins, including new ones that weren't loaded before.

This also means that you can *update* plugins without having to fully reboot the server as well. However, there are a few guidelines that must be followed by the plugin developer for this to be achieved properly:

***For Hosts***
 - If you are updating a plugin, make sure that its assembly name is not the same as the current version you have installed (if any). The plugin must be built by the developer with Dynamic Updates in mind for this to work, simply renaming the file will not.
 - If the plugin supports dynamic updates, be sure that when you put the newer version of the plugin into the "Plugins" folder, you also remove the older version from the folder before reloading EXILED, failure to ensure this will result in many bad things.
 - Any problems that arise from dynamically updating a plugin are solely the responsibility of you and the developer of the plugin in question. While EXILED fully supports and encourages dynamic updates, the only way it could fail or go wrong is if the server host or plugin dev did something wrong. Verify that everything was done correctly by both of those parties before reporting a bug to EXILED developers regarding dynamic updates.

 ***For Developers***

 - Plugins that want to support dynamic updating need to be sure to unsubscribe from all events they are hooked into when they are disabled or reloaded.
 - Plugins that have custom Harmony patches must use some kind of changing variable within the name of the Harmony Instance, and must ``UnPatchAll()`` on their harmony instance when the plugin is disabled or reloaded.
 - Any coroutines started by the plugin in ``OnEnabled()`` must also be killed when the plugin is disabled or reloaded.

All of these can be achieved in either the ``OnReloaded()`` or ``OnDisabled()`` methods in the plugin class. When EXILED reloads plugins, it calls ``OnDisabled()``, then ``OnReloaded()``, then it will load in the new assemblies, and then executes ``OnEnabled()``.

Note that it’s *new* assemblies. If you replace an assembly with another one of the same name, it will ***NOT*** be updated. This is due to the GAC (Global Assembly Cache), if you attempt to 'load' and assembly that is already in the cache, it will always use the cached assembly instead.
For this reason, if your plugin supports dynamic updates, you must build each version with a different assembly name in the build options (renaming the file will not work). Also, since the old assembly is not "destroyed" when it is no longer needed, if you fail to unsubscribe from events, unpatch your harmony instance, kill coroutines, etc., that code will continue to run as well as the new version's code.
This is an extremely bad idea to let happen.

As such, plugins that support dynamic updates ***MUST*** follow these guidelines or they will be removed from the discord server due to potential risk to server hosts.

Not every plugin must support dynamic updates. If you do not intend to support dynamic updates, that's perfectly fine. Just avoid changing the assembly name of your plugin when you build a new version. In such cases make sure server hosts know they will need to completely reboot their servers to update your plugin.
