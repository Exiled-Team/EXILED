# EXILED - EXtended In-runtime Library for External Development

![EXILED CI](https://github.com/galaxy119/EXILED/workflows/EXILED%20CI/badge.svg?branch=2.0.0)
<a href="https://github.com/Exiled-Team/EXILED/releases">
  <img src="https://img.shields.io/github/release/Exiled-Team/EXILED/all.svg?style=flat" alt="GitHub Releases">
</a>
![Github All Downloads](https://img.shields.io/github/downloads/galaxy119/EXILED/total.svg?style=flat)
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/dev)
<a href="https://discord.gg/PyUkWTg">
  <img src="https://img.shields.io/discord/656673194693885975?logo=discord" alt="Chat on Discord">
</a>


EXILED是一个用于SCP: 秘密实验室服务器的低级别插件框架。 它为开发者提供了一个事件系统，以便改变游戏代码，或实现其自己的功能。
所有的EXILED事件都基于Harmony，意味着它不需要直接修改程序集来生效，使得其拥有两个独特的优点。
 - 首先， 所有框架内的代码都可以被发布和分享, 使得开发者可以更好的了解它是如何运作的, 以及提供增加或修改功能的建议。
 - 其次， 因为所有与框架相关的代码都是在服务器的程序集之外完成的，小的游戏更新对框架的影响会非常小。使得其更可能会与未来游戏更新兼容，以及在必要时更新时更加简单。


# 安装方法
EXILED的安装可能看起来比别的框架更加复杂，但其实并不复杂。
如同上面所述，EXILED的主要部分并不是在服务器的Assembly-CSharp.dll文件中, 但是, 由于需要在服务器启动时加载EXILED，Assembly-CSharp.dll文件中会有一处需要修改。一个纯净的程序集和这个改动在Releases中会被提供。
如果你使用的是安装包，并正确运行，它会帮你搞定安装`Exiled.Loader`, `Exiled.Updater`, `Exiled.Permissions`, `Exiled.API`和`Exiled.Events`, and ensuring your server has the proper Assembly-CSharp.dll file installer.

# Windows
### 全自动安装 ([more information](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))
**备注**: 在运行安装包前请确保你所使用的用户有管理员权限。

  - Download the **`Exiled.Installer-Win.exe` [from here](https://github.com/galaxy119/EXILED/releases)** (click on Assets -> click the Installer)
  - Place it on your server folder (download the dedicated server if you haven't)
  - Double click the **`Exiled.Installer.exe`** or **[download this .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** and place it in the server folder to install the latest pre-release
  - To install and get plugins, check the [Installing plugins](#installing-plugins) section down below.
**备注:** If you are installing EXILED on a remote server, make sure you run the .exe as the same user that runs your SCP:SL servers (or one with Admin privileges)

### 手动安装
  - 下载 **`Exiled.tar.gz` [从这里](https://github.com/galaxy119/EXILED/releases)**
  - 使用 [7Zip](https://www.7-zip.org/) 或 [WinRar](https://www.win-rar.com/download.html?&L=6) 解压里面的内容
  - 移动 **``Assembly-CSharp.dll``** 到: **`(服务器文件夹)\SCPSL_Data\Managed`** 并更换文件
  - 移动 **``EXILED``** 文件夹到 **`%appdata%`** *备注: 这个文件夹需要放在 ``C:\用户\(你的用户)\AppData\Roaming``, 而 ***不是*** ``C:\用户\(你的用户)\AppData\Roaming\SCP Secret Laboratory``, 而且它 **必须** 在 (...)\AppData\Roaming, 而不是 (...)\AppData\!*
    - Windows 10 & 11:
      输入 `%appdata%` 在底部的 Cortana / 搜索图标
    - 其他Windows版本:
      Press Win + R and type `%appdata%`

### 安装插件
That's it, EXILED should now be installed and active the next time you boot up your server. Note that EXILED by themselves will do almost nothing, so make sure to get new plugins from **[our Discord server](https://discord.gg/PyUkWTg)**
- To install a plugin, simply:
  - Download a plugin from [*their* releases page](https://i.imgur.com/u34wgPD.jpg) (**it MUST be a `.dll`!**)
  - Move it to: ``C:\Users\(Your_User)\AppData\Roaming\EXILED\Plugins`` (move here by pressing Win + R, then writing `%appdata%`)

# Linux
### 全自动安装 ([更多信息](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))

**备注:** If you are installing EXILED on a remote server, make sure you run the Installer as the same user that runs your SCP:SL servers (or root)

  - 下载 **`Exiled.Installer-Linux` [从这里](https://github.com/galaxy119/EXILED/releases)** (点击 Assets -> 下载安装包)
  - Install it by either typing **`./Exiled.Installer-Linux --path /path/to/server`** or move it inside the server folder directly, move to it with the terminal (`cd`) and type: **`./Exiled.Installer-Linux`**.
  - 如果你想要最新的预更新, 只需添加 **`--pre-releases`**. 例子: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - 另一个例子, 如果你把 `Exiled.Installer-Linux` 放到了你服务器的文件夹: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - 获取以及安装插件，请参考下面的[安装插件](#installing-plugins-1)部分。

### Manual installation
  - **Ensure** you are logged in on the user that runs the SCP servers.
  - Download the **`Exiled.tar.gz` [from here](https://github.com/galaxy119/EXILED/releases)** (SSH: right click and to get the `Exiled.tar.gz` link, then type: **`wget (link_to_download)`**)
  - To extract it to your current folder, type **``tar -xzvf EXILED.tar.gz``**
  - Move the included **``Assembly-CSharp.dll``** file into the **``SCPSL_Data/Managed``** folder of your server installation (SSH: **`mv Assembly-CSharp.dll (path_to_server)/SCPSL_Data/Managed`**).
  - Move the **`EXILED`** folder to **``~/.config``**. *Note: This folder needs to go in ``~/.config``, and ***NOT*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)

### 安装插件
That's it, EXILED should now be installed and active the next time you boot up your server. Note that EXILED by themselves will do almost nothing, so make sure to get new plugins from **[our Discord server](https://discord.gg/PyUkWTg)**
- To install a plugin, simply:
  - Download a plugin from [*their* releases page](https://i.imgur.com/u34wgPD.jpg) (**it MUST be a `.dll`!**)
  - Move it to: ``~/.config/EXILED/Plugins`` (if you use your SSH as root, then search for the correct `.config` which will be inside `/home/(SCP Server User)`)

# 配置文件（Config）
EXILED自身提供一些配置选项。
All of them are auto-generated at the server startup, they are located at ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\Configs\(ServerPortHere)-config.yml`` on Windows).

Plugin configs will ***NOT*** be in the aforementioned ``config_gameplay.txt`` file, instead, plugin configs are set in the ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` file (``%AppData%\EXILED\(ServerPortHere)-config.yml`` on Windows).
However, some plugins might get their config settings from other locations on their own, this is simply the default EXILED location for them, so refer to the individual plugin if there are issues.

# 致开发者

制作一个EXILED的插件也是一件非常轻松的事情。如果你想要一个教程，请访问我们的[入门指南](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md).

一个更加详细并且保持更新的教程，请见 [EXILED官网](https://exiled-team.github.io/EXILED/articles/install.html).

制作插件时应遵守以下规则：

 - 你的插件必须有一个类继承Exiled.API.Features.Plugin<>，如果没有， EXILED将不会在服务器启动时载入你的插件。
 - 当一个插件载入后， ``OnEnabled()`` 方法中的代码将会在之前提到的类中被叫到, it does not wait for other plugins to be loaded. It does not wait for the server startup process to finish. ***It does not wait for anything.*** When setting up your OnEnable() method, be sure you are not accessing things which may not be initialized by the server yet, such as ServerConsole.Port, or PlayerManager.localPlayer.
 - If you need to access things early on that are not initialized before your plugin is loaded, it is recommended to simply wait for the WaitingForPlayers event to do so, if you for some reason need to do things sooner, wrap the code in a ``` while(!x)``` loop that checks for the variable/object you need to no longer be null before continuing.
 - EXILED supports dynamically reloading plugin assemblies mid-execution. The means that, if you need to update a plugin, it can be done without rebooting the server, however, if you are updating a plugin mid-execution, the plugin needs to be properly setup to support it, or you will have a very bad time. Refer to the ``Dynamic Updates`` section for more information and guidelines to follow.
 - There is ***NO*** OnUpdate, OnFixedUpdate or OnLateUpdate event within EXILED. If you need to, for some reason, run code that often, you can use a MEC coroutine that waits for one frame, 0.01f, or uses a Timing layer like Timing.FixedUpdate instead.

 ### MEC协程
如果你对MEC并不了解, 这将会是一个简单的入门供你参考。
MEC协程 are basically timed methods, that support waiting periods of time before continuing execution, without interrupting/sleeping the main game thread.
MEC协程可以用于Unity, 不像传统的线程 ***请勿尝试增加新的线程与Unity交互，它会导致炸服的。***

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

***致服主***
 - If you are updating a plugin, make sure that it's assembly name is not the same as the current version you have installed (if any). The plugin must be built by the developer with Dynamic Updates in mind for this to work, simply renaming the file will not.
 - If the plugin supports Dynamic Updates, be sure that when you put the newer version of the plugin into the "Plugins" folder, you also remove the older version from the folder, before reloading EXILED, failure to ensure this will result in many many bad things.
 - Any problems that arise from Dynamically Updating a plugin is solely the responsibility of you and the developer of the plugin in question. While EXILED fully supports and encourages Dynamic Updates, the only way it could fail or go wrong is if the server host or plugin dev did something wrong. Triple check that everything was done correctly by both of those parties before reporting a bug to EXILED devs regarding Dynamic Updates.

 ***致开发者***

 - Plugins that want to support Dynamic Updating need to be sure to unsubscribe from all events they are hooked into when they are Disabled or Reloaded.
 - Plugins that have custom Harmony patches must use some kind of changing variable within the name of the Harmony Instance, and must UnPatchAll() on their harmony instance when the plugin is disabled or reloaded.
 - Any coroutines started by the plugin in OnEnabled must also be killed when the plugin is disabled or reloaded.

All of these can be achieved in either the OnReloaded() or OnDisabled() methods in the plugin class. When EXILED reloads plugins, it calls OnDisabled(), then OnReloaded(), then it will load in the new assemblies, and then executes OnEnabled().

Note that I said *new* assemblies. If you replace an assembly with another one of the same name, it will ***NOT*** be updated. This is due to the GAC (Global Assembly Cache), if you attempt to 'load' and assembly that is already in the cache, it will always use the cached assembly instead.
For this reason, if your plugin will support Dynamic Updates, you must build each version with a different Assembly Name in the build options (renaming the file will not work). Also, since the old assembly is not "destroyed" when it is no longer needed, if you fail to unsubscribe from events, unpatch your harmony instance, kill coroutines, etc, that code will continue to run aswell as the new version's code.
This is a very very bad idea to let happen.

As such, plugins that support Dynamic Updates ***MUST*** follow these guidelines or they will be removed from the Discord server due to potential risk to server hosts.

But not every plugin must support Dynamic Updates. If you do not intend to support Dynamic Updates, that's perfectly fine, simply don't change the Assembly Name of your plugin when you build a new version, and you will not need to worry about any of this, just make sure server hosts know they will need to completely reboot their servers to update your plugin.
