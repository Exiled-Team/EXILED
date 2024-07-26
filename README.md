# EXILED - EXtended In-runtime Library for External Development

![EXILED CI](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases">
  <img src="https://img.shields.io/github/release/Exiled-Team/EXILED/all.svg?style=flat" alt="GitHub Releases">
</a>
![Github All Downloads](https://img.shields.io/github/downloads/Exiled-Team/EXILED/total.svg?style=flat)
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/dev)
<a href="https://discord.gg/835XQcCCVv">
  <img src="https://img.shields.io/discord/1261714360854646905?logo=discord" alt="Chat on Discord">
</a>


EXILED is a low-level plugin framework for SCP: Secret Laboratory servers. It offers an event system for developers to hook in order to manipulate or change game code, or implement their own functions.
All EXILED events are coded with Harmony, meaning they require no direct editing of server Assemblies to function, which allows for two unique benefits.

 - Firstly, the entirety of the frameworks code can be freely published and shared, allowing developers to better understand *how* it works, as well as offer suggestions for adding to or changing it's features.
 - Secondly, since all of the code related to the framework are done outside of the server assembly, things like small game updates will have little, if any, effect on the framework. Making it most likely to be compatible with future game updates, as well as making it easier to update when it *is* necessary to do so.


# Installation
Installation of EXILED may seem more involved or complicated than other frameworks, but it is in fact quite simple.
As mentioned above, the vast majority of EXILED is not contained within the server's Assembly-CSharp.dll file, however, there is a single modification needed to the Assembly-CSharp.dll file that is required to actually *load* EXILED into the server during startup, a clean game Assembly with this change already made will be provided with releases.

If you choose to use the installer it will, if run correctly, take care of installing `Exiled.Loader`, `Exiled.Updater`, `Exiled.Permissions`, `Exiled.API` and `Exiled.Events`, and ensuring your server has the proper Assembly-CSharp.dll file installer.

# Windows
### Automatic installation ([more information](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))
**Note**: Make sure you're on the user that runs the server or you have Admin privileges before running the Installer.

  - Download the **`Exiled.Installer-Win.exe` [from here](https://github.com/galaxy119/EXILED/releases)** (click on Assets -> click the Installer)
  - Place it on your server folder (download the dedicated server if you haven't)
  - Double click the **`Exiled.Installer.exe`** or **[download this .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** and place it in the server folder to install the latest pre-release
  - To install and get plugins, check the [Installing plugins](#installing-plugins) section down below.
**Note:** If you are installing EXILED on a remote server, make sure you run the .exe as the same user that runs your SCP:SL servers (or one with Admin privileges)

### Manual installation
  - Download the **`Exiled.tar.gz` [from here](https://github.com/galaxy119/EXILED/releases)**
  - Extract its contents with [7Zip](https://www.7-zip.org/) or [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Move **``Assembly-CSharp.dll``** to: **`(Your Server Folder)\SCPSL_Data\Managed`** and replace the file.
  - Move the **``EXILED``** folder to **`%appdata%`** *Note: This folder needs to go in ``C:\Users\(Your_User)\AppData\Roaming``, and ***NOT*** ``C:\Users\(Your_User)\AppData\Roaming\SCP Secret Laboratory``, and **IT MUST** be in (...)\AppData\Roaming, not (...)\AppData\!*
    - Windows 10:
      Write `%appdata%` in Cortana / the search icon, or the Windows Explorer bar
    - Any other Windows version:
      Press Win + R and type `%appdata%`

### Installing plugins
That's it, EXILED should now be installed and active the next time you boot up your server. Note that EXILED by themselves will do almost nothing, so make sure to get new plugins from **[our Discord server](https://discord.gg/PyUkWTg)**
- To install a plugin, simply:
  - Download a plugin from [*their* releases page](https://i.imgur.com/u34wgPD.jpg) (**it MUST be a `.dll`!**)
  - Move it to: ``C:\Users\(Your_User)\AppData\Roaming\EXILED\Plugins`` (move here by pressing Win + R, then writing `%appdata%`)

# Linux
### Automatic installation ([more information](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))

**Note:** If you are installing EXILED on a remote server, make sure you run the Installer as the same user that runs your SCP:SL servers (or root)

  - Download the **`Exiled.Installer-Linux` [from here](https://github.com/galaxy119/EXILED/releases)** (click on Assets -> download the Installer)
  - Install it by either typing **`./Exiled.Installer-Linux --path /path/to/server`** or move it inside the server folder directly, move to it with the terminal (`cd`) and type: **`./Exiled.Installer-Linux`**.
  - If you want the latest pre-release, simply add **`--pre-releases`**. Example: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Another example, if you placed `Exiled.Installer-Linux` in your server folder: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - To install and get plugins, check the [Installing plugins](#installing-plugins-1) section down below.

### Manual installation
  - **Ensure** you are logged in on the user that runs the SCP servers.
  - Download the **`Exiled.tar.gz` [from here](https://github.com/galaxy119/EXILED/releases)** (SSH: right click and to get the `Exiled.tar.gz` link, then type: **`wget (link_to_download)`**)
  - To extract it to your current folder, type **``tar -xzvf EXILED.tar.gz``**
  - Move the included **``Assembly-CSharp.dll``** file into the **``SCPSL_Data/Managed``** folder of your server installation (SSH: **`mv Assembly-CSharp.dll (path_to_server)/SCPSL_Data/Managed`**).
  - Move the **`EXILED`** folder to **``~/.config``**. *Note: This folder needs to go in ``~/.config``, and ***NOT*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)

### Installing plugins
That's it, EXILED should now be installed and active the next time you boot up your server. Note that EXILED by themselves will do almost nothing, so make sure to get new plugins from **[our Discord server](https://discord.gg/PyUkWTg)**
- To install a plugin, simply:
  - Download a plugin from [*their* releases page](https://i.imgur.com/u34wgPD.jpg) (**it MUST be a `.dll`!**)
  - Move it to: ``~/.config/EXILED/Plugins`` (if you use your SSH as root, then search for the correct `.config` which will be inside `/home/(SCP Server User)`)

# Config
EXILED by itself offers some config options.
All of them are auto-generated at the server startup, they are located at ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\Configs\(ServerPortHere)-config.yml`` on Windows).

Plugin configs will ***NOT*** be in the aforementioned ``config_gameplay.txt`` file, instead, plugin configs are set in the ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\(ServerPortHere)-config.yml`` on Windows).
However, some plugins might get their config settings from other locations on their own, this is simply the default EXILED location for them, so refer to the individual plugin if there are issues.

# For Developers

If you wish to make a Plugin for EXILED, it's quite simple to do so. If you would like more of a tutorial please visit our [Getting Started Page.](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md), or you can just watch a [video tutorial](https://www.youtube.com/watch?v=gx67ziYldvk) on YouTube.

But make sure to follow these rules when publishing your plugins:

 - Your plugin must contain a class that inherits from Exiled.API.Features.Plugin<>, if it does not, EXILED will not load your plugin when the server starts.
 - When a plugin is loaded, the code within the aforementioned class' ``OnEnabled()`` method is fired immediately, it does not wait for other plugins to be loaded. It does not wait for the server startup process to finish. ***It does not wait for anything.*** When setting up your OnEnable() method, be sure you are not accessing things which may not be initialized by the server yet, such as ServerConsole.Port, or PlayerManager.localPlayer.
 - If you need to access things early on that are not initialized before your plugin is loaded, it is recommended to simply wait for the WaitingForPlayers event to do so, if you for some reason need to do things sooner, wrap the code in a ``` while(!x)``` loop that checks for the variable/object you need to no longer be null before continuing.
 - EXILED supports dynamically reloading plugin assemblies mid-execution. The means that, if you need to update a plugin, it can be done without rebooting the server, however, if you are updating a plugin mid-execution, the plugin needs to be properly setup to support it, or you will have a very bad time. Refer to the ``Dynamic Updates`` section for more information and guidelines to follow.
 - There is ***NO*** OnUpdate, OnFixedUpdate or OnLateUpdate event within EXILED. If you need to, for some reason, run code that often, you can use a MEC coroutine that waits for one frame, 0.01f, or uses a Timing layer like Timing.FixedUpdate instead.

### Disabling EXILED Event patches
***This feature is currently no longer implemented.***

 ### MEC Coroutines
If you are unfamiliar with MEC, this will be a very brief and simple primer to get you started.
MEC Coroutines are basically timed methods, that support waiting periods of time before continuing execution, without interrupting/sleeping the main game thread.
MEC coroutines are safe to use with Unity, unlike traditional threading. ***DO NOT try and make new threads to interact with Unity on, they WILL crash the server.***

To use MEC, you will need to reference ``Assembly-CSharp-firstpass.dll`` from the server files, and include ``using MEC;``.
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

It is ***strongly*** recommended that you do some googling, or ask around in the Discord if you are unfamiliar with MEC and would like to learn more, get advice, or need help. Questions, no matter how 'stupid' they are, will always be answered as helpfully and clearly as possible for plugin developers to excell. Better code is better for everyone.

### Dynamic Updates
EXILED as a framework supports dynamic reloading of plugin assemblies without requiring a server reboot.
For example, if you start the server with just `Exiled.Events` as the only plugin, and wish to add a new one, you do not need to reboot the server to complete this task. You can simply use the RemoteAdmin/ServerConsole command `reload plugins` to reload all EXILED plugins, including new ones that weren't loaded before.

This also means that you can *update* plugins without having to fully reboot the server as well. However there are a few guidelines that must be followed by the plugin developer in order for this to be achieved properly:

***For Hosts***
 - If you are updating a plugin, make sure that it's assembly name is not the same as the current version you have installed (if any). The plugin must be built by the developer with Dynamic Updates in mind for this to work, simply renaming the file will not.
 - If the plugin supports Dynamic Updates, be sure that when you put the newer version of the plugin into the "Plugins" folder, you also remove the older version from the folder, before reloading EXILED, failure to ensure this will result in many many bad things.
 - Any problems that arise from Dynamically Updating a plugin is solely the responsibility of you and the developer of the plugin in question. While EXILED fully supports and encourages Dynamic Updates, the only way it could fail or go wrong is if the server host or plugin dev did something wrong. Triple check that everything was done correctly by both of those parties before reporting a bug to EXILED devs regarding Dynamic Updates.

 ***For Developers***

 - Plugins that want to support Dynamic Updating need to be sure to unsubscribe from all events they are hooked into when they are Disabled or Reloaded.
 - Plugins that have custom Harmony patches must use some kind of changing variable within the name of the Harmony Instance, and must UnPatchAll() on their harmony instance when the plugin is disabled or reloaded.
 - Any coroutines started by the plugin in OnEnabled must also be killed when the plugin is disabled or reloaded.

All of these can be achieved in either the OnReloaded() or OnDisabled() methods in the plugin class. When EXILED reloads plugins, it calls OnDisabled(), then OnReloaded(), then it will load in the new assemblies, and then executes OnEnabled().

Note that I said *new* assemblies. If you replace an assembly with another one of the same name, it will ***NOT*** be updated. This is due to the GAC (Global Assembly Cache), if you attempt to 'load' and assembly that is already in the cache, it will always use the cached assembly instead.
For this reason, if your plugin will support Dynamic Updates, you must build each version with a different Assembly Name in the build options (renaming the file will not work). Also, since the old assembly is not "destroyed" when it is no longer needed, if you fail to unsubscribe from events, unpatch your harmony instance, kill coroutines, etc, that code will continue to run aswell as the new version's code.
This is a very very bad idea to let happen.

As such, plugins that support Dynamic Updates ***MUST*** follow these guidelines or they will be removed from the Discord server due to potential risk to server hosts.

But not every plugin must support Dynamic Updates. If you do not intend to support Dynamic Updates, that's perfectly fine, simply don't change the Assembly Name of your plugin when you build a new version, and you will not need to worry about any of this, just make sure server hosts know they will need to completely reboot their servers to update your plugin.

