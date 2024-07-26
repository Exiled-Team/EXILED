# EXILED - EXtended In-runtime Library for External Development

Join our [[Discord](https://discord.gg/835XQcCCVv)]!

![EXILED CI](https://github.com/galaxy119/EXILED/workflows/EXILED%20CI/badge.svg?branch=2.0.0)
 

EXILED is a low-level plugin framework for SCP: Secret Laboratory servers. It offers an event system for developers to hook in order to manipulate or change game code, or implement their own functions.
All EXILED events are coded with Harmony, meaning they require no direct editing of server Assemblies to function, which allows for two unique benefits.

 - Firstly, the entirety of the frameworks code can be freely published and shared, allowing developers to better understand *how* it works, aswell as offer suggestions for adding to or changing it's features.
 - Secondly, since all of the code related to the framework are done outside of the server assembly, things like small game updates will have little, if any, effect on the framework. Making it most likely to be compatible with future game updates, aswell as making it easier to update when it *is* necessary to do so.


# Installation
Installation of EXILED may seem more involed or complicated than other frameworks, but it is infact quite simple.
As mentioned above, the vast majority of EXILED is not contained within the server's Assembly-CSharp.dll file, but within EXILED.dll and EXILED_Events.dll, however, there is a single modification needed to the Assembly-CSharp.dll file that is required to actually *load* EXILED into the server during startup, a clean game Assembly with this change already made will be provided with releases.

  - Click on the "Release" tab near the top-right, and download the latest release version of the framework. You can choose the automated EXILED_Installer.exe, or you can follow the directions below to install the framework manually, either is acceptable.
  - If choosing to use the Installer.exe, simply run the .exe to install EXILED. If you are installing EXILED on a remote server, make sure you run the .exe as the same user that runs your SCP: SL servers. (Linux server owners can still use the .exe, simply run it in a terminal with ``mono EXILED_Installer.exe``). The installer will, if run correctly, take care of installing EXILED, EXILED_Events, and ensuring your server has the proper Assembly-CSharp.dll file installer. If you are on a Windows server, or have a non-standard scp_server installation folder (default is ``/home/scp/scp_server``) you can run the .exe in a Command Prompt/Terminal and specify your server's install directory. EX: ``mono EXILED_Installer.exe /home/scp/server``(or just ``EXILED_Installer.exe path\to\server\folder`` for Windows).


If you choose to install EXILED manually:
 - Ensure you are logged in on the user that runs the SCP servers.
  - Download the EXILED.tar.gz archive from the latest release, and extract it's contents to a folder on the machine you will be installing EXILED on. (Windows users will need a 3rd party tool such as 7zip to do this) *Linux command line: ``tar -xzvf EXILED.tar.gz``*
  - Move the included ``Assembly-CSharp.dll`` file into the ``SCPSL_Data/Managed`` folder of your server installation. (Replace the existing file).
  - Navigate to ``~/.config`` (``%AppData%`` on Windows), and move both the "EXILED" and "Plugins" folders that were provided in the above mentioned archive into this location *Note: These 2 folders need to go in ``~/.config``, and ***NOT*** ``~/.config/SCP Secret Laboratory``
  - That's it, EXILED should now be installed and active the next time you boot up your server. It should be noted, that EXILED and EXILED_Events by themselves will do nothing, you must have plugins installed in order to use EXILED. Plugin .dll files go into the "Plugins" folder that should now be located at ``~/.config/Plugins`` (``%AppData%\Plugins`` on Windows). 
  - It is important that you do not rename "EXILED_Events.dll", as this may cause EXILED to fail to load that library before any plugins, which will likely result in and error on server startup.

# Config
EXILED by itself offers very little in the way of config options, as it, by itself, does nothing.
The only thing offered is the ``exiled_debug`` value, which should go into your ``~/.config/EXILED/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\(ServerPortHere)-config.yml`` on Windows). This value is false by default, setting it to true will print additional lines from plugins and EXILED itself to help developers/hosts fix misbehaving code. *Note: This value will not be automatically added to the config file, and if you wish to set it to true, you will need to add it to the file yourself.*

Plugin configs will ***NOT*** be in the aforementioned ``config_gameplay.txt`` file, instead, plugin configs are set in the ``~/.config/EXILED/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\(ServerPortHere)-config.yml`` on Windows).
However, plugins might get their config settings from other locations on their own, this is simply the default EXILED location for them, so refer to the individual plugin if there are issues.

This text you see here is *actually* written in Markdown! To get a feel for Markdown's syntax, type some text into the left window and watch the results in the right.

# For Developers

If you wish to make a Plugin for EXILED, it's quite simple to do so. If you would like more of a tutorial please visit our [Getting Started Page.](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md) But make sure to follow these rules when publishing your plugins:

 - Your plugin must contain a class that inherits from EXILED.Plugin, if it does not, EXILED will not load your plugin when the server starts.
 - When a plugin is loaded, the code within the aforementioned class' ``OnEnable()`` method is fired immediately, it does not wait for other plugins to be loaded. It does not wait for the server startup process to finish. ***It does not wait for anything.*** When setting up your OnEnable() method, be sure you are not accessing things which may not be initialized by the server yet, such as ServerConsole.Port, or PlayerManager.localPlayer. 
 - If you need to access things early on that are not initialized before your plugin is loaded, it is recommended to simply wait for the WaitingForPlayers event to do so, if you for some reason need to do things sooner, wrap the code in a ``` while(!x)``` loop that checks for the variable/object you need to no longer be null before continuing.
 - EXILED supports dynamically reloading plugin assemblies mid-execution. The means that, if you need to update a plugin, it can be done without rebooting the server, however, if you are updating a plugin mid-execution, the plugin needs to be properly setup to support it, or you will have a very bad time. Refer to the ``Dynamic Updates`` section for more information and guidelines to follow.
 - There is ***NO*** OnUpdate, OnFixedUpdate or OnLateUpdate event within EXILED. If you need to, for some reason, run code that often, you can use a MEC coroutine that waits for one frame, 0.01f, or uses a Timing layer like Timing.FixedUpdate instead.

### Disabling EXILED Event patches
If you wish to re-implement an event patch on your own, or you need to change the code in the same method used by an EXILED event patch, you can disable the patch from within your plugin, and re-implement it yourself quite easily.

In the said plugin's OnEnable() simply change the EXILED.Events.EventPlugin.xxxxxxPatchDisabled bool to true for the event patch you want to disable, then, to ensure your plugin doesn't absolutely break every other plugin using that event on the server, be sure to call ``EXILED.Events.Invokexxxx();`` where appropriate.
For example, if you wish to disable the PlayerJoin event and re-implement it yourself, simply
```cs
public void OnEnable()
{
    EXILED.Events.EventsPlugin.PlayerJoinPatchDisabled = true;
}
```
and then, in an appropriate location in your own patch, do ``EXILED.Events.InvokePlayerJoin();``

You should also make sure your plugin changes the PatchDisabled bool back to false in OnDisable, to avoid any potential issues that may arise from disabling your plugin mid-execution.
 ### MEC Coroutines
If you are unfamiliar with MEC, this will be a very breif and simple primer to get you started.
MEC Coroutines are basically timed methods, that support waiting periods of time before continuing execution, without interrupting/sleeping the main game thread. 
MEC coroutines are safe to use with Unity, unlike traditional threading. ***DO NOT try and make new threads to interact with Unity on, they WILL crash the server.***

To use MEC, you will need to reference ``Assembly-CSharp-firstpass.dll`` from the server files, and include ``using MEC;``.
Example of calling a simple coroutine, that repeats itself with a delay between each loop:
```cs
using MEC;
public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for(;;) //repeat the following infinitely
    {
        Plugin.Info("Hey I'm a infinite loop!"); //Call Plugin.Info to print a line to the game console/server logs
        yield return Timing.WaitForSecond(5f); //Tells the coroutine to wait 5 seconds before continuing, since this is at the end of the loop, it effectively stalls the loop from repeating for 5seconds.
    }
}
```

It is ***strongly*** recommended that you do some googling, or ask around in the Discord if you are unfamiliar with MEC and would like to learn more, get advice, or need help. Questions, no matter how 'stupid' they are, will always be answered as helpfully and clearly as possible for plugin developers to excell. Better code is better for everyone.

### Dynamic Updates
EXILED as a framework supports dynamic reloading of plugin assemblies without requiering a server reboot. 
For example, if you start the server with just EXILED_Events as the only plugin, and wish to add a new one, you do not need to reboot the server to complete this task. You can simply use the RA/ServerConsole command "reload" to reload all EXILED plugins, including new ones that weren't loaded before.

This also means that you can *update* plugins without having to fully reboot the server aswell. However there are a few guidelines that must be followed by the plugin developer in order for this to be achieved properly:

***For Hosts***
 - If you are updating a plugin, make sure that it's assembly name is not the same as the current version you have installed (if any). The plugin must be built by the developer with Dynamic Updates in mind for this to work, simply renaming the file will not.
 - If the plugin supports Dynamic Updates, be sure that when you put the newer version of the plugin into the "Plugins" folder, you also remove the older version from the folder, before reloading EXILED, failure to ensure this will result in many many bad things.
 - Any problems that arise from Dynamically Updating a plugin is soley the responsibility of you and the developer of the plugin in question. While EXILED fully supports and encourages Dynamic Updates, the only way it could fail or go wrong is if the server host or plugin dev did something wrong. Triple check that everything was done correctly by both of those parties before reporting a bug to EXILED devs regarding Dynamic Updates.

 ***For Developers***

 - Plugins that want to support Dynamic Updating need to be sure to unsubscribe from all events they are hooked into when they are Disabled or Reloaded. 
 - Plugins that have custom Harmony patches must use some kind of changing variable within the name of the Harmony Instance, and must UnPatchAll() on their harmony instance when the plugin is disabled or reloaded. 
 - Any coroutines started by the plugin in OnEnable must also be killed when the plugin is disabled or reloaded.

All of these can be achieved in either the OnReload() or OnDisabled() methods in the plugin class. When EXILED reloads plugins, it calls OnDisable(), then OnReload(), then it will load in the new assemblies, and then executes OnEnable().

Note that I said *new* assemblies. If you replace an assembly with another one of the same name, it will ***NOT*** be updated. This is due to the GAC (Global Assembly Cache), if you attempt to 'load' and assembly that is already in the cache, it will always use the cached assembly instead.
For this reason, if your plugin will support Dynamic Updates, you must build each version with a different Assembly Name in the build options (renaming the file will not work). Also, since the old assembly is not "destroyed" when it is no longer needed, if you fail to unsubscribe from events, unpatch your harmony instance, kill coroutines, etc, that code will continue to run aswell as the new version's code.
This is a very very bad idea to let happen. 

As such, plugins that support Dynamic Updates ***MUST*** follow these guidelines or they will be removed from the Discord server due to potential risk to server hosts.

But not every plugin must support Dynamic Updates. If you do not intend to support Dynamic Updates, that's perfectly fine, simply don't change the Assembly Name of your plugin when you build a new version, and you will not need to worry about any of this, just make sure server hosts know they will need to completely reboot their servers to update your plugin.

