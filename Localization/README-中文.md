<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="discord.gg/exiledreboot">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>    

</div>


EXILED是一个用于SCP: 秘密实验室服务器的高级插件框架。 它为开发者提供了一个可以改变游戏代码或实现其自己的功能的事件系统。
所有的EXILED事件都基于Harmony，意味着它不需要直接修改程序集来生效，使得其拥有两个独特的优点。
 - 首先， 所有框架内的代码都可以被发布和分享， 使得开发者可以更好的了解它是如何运作的， 以及提供增加或修改功能的建议。
 - 其次， 因为所有与框架相关的代码都是在服务器的程序集之外完成的，小的游戏更新对框架的影响会非常小。使得其更可能会与未来游戏更新兼容，以及在必要时更新时更加简单。


# 安装方法
EXILED的安装十分简单。因为是用NW插件API来将自身载入，你会在发行版（Releases）中的``Exiled.tar.gz``解压后看到两个文件夹。``SCP Secret Laboratory``文件夹中包含了加载``EXILED``框架所需的文件。
综上所述，你所需要做的也就是把这两个文件夹放到该放的地方，具体步骤将会在下面阐述。

如果你选择使用一键安装器，在运行正常的情况下它会帮你安装好所有EXILED的功能。

# Windows
### 全自动安装 ([更多消息](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))
**备注**: 在运行安装包前请确保你所使用的用户有管理员权限。

  - 下载 **[Exiled.Installer-Win.exe](https://github.com/galaxy119/EXILED/releases)** (点击 Assets -> 下载安装包)
  - 放置到你的服务器文件夹 (如果你还没有下载服务端，则需要先在Steam中下载服务端)
  - 双击 **`Exiled.Installer.exe`** 或 **[install-prerelease.bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** 并放置到服务器文件夹中来安装最新的预更新
  - 获取以及安装插件，请参考下面的[安装插件](#installing-plugins)部分。
**备注:** 如果你正在一个远程服务器上安装EXILED，请确保你运行的.exe的用户和你运行SCP:SL服务器的一致

### 手动安装
  - 下载 **[Exiled.tar.gz](https://github.com/galaxy119/EXILED/releases)**
  - 使用 [7Zip](https://www.7-zip.org/) 或 [WinRar](https://www.win-rar.com/download.html?&L=6) 解压里面的内容
  - 移动 **``EXILED``** 文件夹到 **`%appdata%`** （*备注: 这个文件夹需要放在 ``C:\用户\%UserName%\AppData\Roaming``， 而 ***不是*** ``C:\用户\%UserName%\AppData\Roaming\SCP Secret Laboratory``， 而且它 **必须** 在 (...)\AppData\Roaming， 而不是 (...)\AppData\!*）
  - 移动 **``SCP Secret Laboratory``** 文件夹到 **`%appdata%`** 中。
  - 按 Win + R 并输入 `%appdata%` 可快捷打开AppData文件夹。


# Linux
### 全自动安装 ([更多信息](https://github.com/galaxy119/EXILED/blob/master/Exiled.Installer/README.md))

**备注:** 如果你正在一个远程服务器上安装EXILED，请确保你运行的.exe的用户和你运行SCP:SL服务器的一致

  - 下载 [Exiled.Installer-Linux](https://github.com/Exiled-Team/EXILED/releases) (点击 Assets -> 下载安装包)
  - 输入 **`./Exiled.Installer-Linux --path /path/to/server`** 来安装，或者是直接把它放到服务器文件夹里， 首先在控制台（终端）中使用 (`cd`)指令移动安装器，之后再输入: **`./Exiled.Installer-Linux`运行安装程序。.
  - 如果你想要最新的预更新， 只需添加 **`--pre-releases`**. 例子: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - 另一个例子， 如果你把 `Exiled.Installer-Linux` 放到了你服务器的文件夹中，你还可以这样操作: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - 获取以及安装插件，请参考下面的[安装插件](#installing-plugins-1)部分。

### 手动安装
  - 首先**确保**你登录的是用于运行SCP服务器的用户。
  - 下载 [Exiled.tar.gz](https://github.com/Exiled-Team/EXILED/releases) (SSH: 右键获取 `Exiled.tar.gz` 的链接， 然后输入: **`wget (下载链接)`**)
  - 解压到你目前的文件夹，输入 **``tar -xzvf EXILED.tar.gz``**
  - 移动 **`EXILED`** 文件夹到 **``~/.config``**中。 *备注: EXILED文件夹应该放到 ``~/.config``， 而 ***不是*** ``~/.config/SCP Secret Laboratory``* (SSH指令例子: **`mv EXILED ~/.config/`**)
  - 移动 **``SCP Secret Laboratory``** 文件到 **``~/.config``** (SSH指令例子: **`mv SCP Secret Laboratory ~/.config/`**).

# 安装插件
现在EXILED已经安装好了，并会在下次你启动你的服务器时随之启动。请注意EXILED本身基本不会做出任何事情，所以来 **[我们的Discord服务器](https://discord.gg/PyUkWTg)** 获取最新的插件吧。
- 想要安装插件，只需要:
  - 从[**插件仓库**的Releases页面](https://i.imgur.com/u34wgPD.jpg)中下载DLL文件。(**它必须是个`.dll`!**)
## Windows
  - 移动DLL文件到: ``C:\用户\%UserName%\AppData\Roaming\EXILED\Plugins`` (可以通过按 Win + R键输入``%appdata%`` 来快速定位到AppData)
## Linux
  - 移动DLL文件到: ``~/.config/EXILED/Plugins`` (如果你的SSH用的是root用户，请搜索正确的`.config`文件夹，它会在 `/home/(SCP服务器用户)`下。)

# 配置文件（Config）
EXILED自身提供一些配置选项。
这些配置都会在服务器启动时被自动生成， 它们***不在*** ``config_gameplay.txt`` 文件中，而是在 ``~/.config/EXILED/Configs/(服务器端口)-config.yml`` 文件中。 (Windows上为``%AppData%\EXILED\Config\(服务器端口)-config.yml``。)
以上是默认的EXILED配置文件存放地点。但是，有一些插件可能会把它们的插件配置放在别的地方。如出现问题，请找该插件开发者。

# 致开发者

制作一个EXILED的插件也是一件非常轻松的事情。如果你想要一个教程，请访问我们的[入门指南](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md).

一个更加详细并且保持更新的教程，请见 [EXILED官网](https://exiled-team.github.io/EXILED/articles/install.html)。

制作插件时应遵守以下规则：

 - 你的插件必须有一个类继承``Exiled.API.Features.Plugin<>``，如果没有， EXILED将不会在服务器启动时载入你的插件。
 - 当一个插件载入后， ``OnEnabled()`` 方法中的代码将会在之前提到的类中会被首先调用， 它不会等待其他插件的加载，也不会等待服务器的启动完成。 ***它不会等待任何事物。*** 当你在设置你的OnEnable()方法时，务必确保你没有在使用任何未初始化的事物，如``ServerConsole.Port``， ``PlayerManager.localPlayer``。
 - 如果你需要使用任何可能未初始化的东西时，建议你等到``WaitingForPlayers``事件再使用，如果因为某些原因你需要在比这个事件还早之前需要执行一些东西，最好把代码放入一个``` while(!x)``` 循环来检查你所使用的变量不为null。
 - EXILED支持动态重新载入正在运行的插件程序集。也就是说，更新一个插件不需要重新启动服务器。但是，如果你要更新一个正在执行中的插件，插件本身需要支持这个功能，否则你将会非常不好过。 见 ``动态更新`` 部分以获取更多信息及规范。
 - EXILED***没有***``OnUpdate``，``OnFixedUpdate``或``OnLateUpdate``事件。如果由于某些原因你需要执行次数那么频繁的代码，你可以使用MEC协程来等待1帧，0.01秒，或``Timing.FixedUpdate``来代替。

 ### MEC协程
如果你对MEC并不了解， 这将会是一个简单的入门供你参考。
MEC协程其实就是计时方法， 它支持在执行一段代码前等待一段时间， 而不会影响/睡眠游戏主线程。
MEC协程可以用于Unity， 不像传统的线程 ***请勿尝试增加新的线程与Unity交互，它会导致炸服的。***

如需使用 MEC， 你需要从SL的服务端文件夹``SCP Secret Laboratory Dedicated Server\SCPSL_Data\Manage`` 中引用 ``Assembly-CSharp-firstpass.dll``， 并在插件中使用 ``using MEC;``。
一个简单的协程例子，循环重复之间有一个延迟:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) // 无限循环
    {
        Log.Info("Hey I'm a infinite loop!"); // 使用 Log.Info 输出一行字在控制台和日志
        yield return Timing.WaitForSeconds(5f); // 告诉协程等待5秒后再继续，鉴于这是循环的结尾，这将会有效的使得循环每次在结尾时都会停顿5秒。
    }
}
```

如果你对MEC仍然不熟悉，或是想要了解更多、得到建议、又或是寻求帮助。***强烈建议*** 你去百度或谷歌一下，或在Discord里面提问。无论问题本身有多“蠢”，我们都会尽可能的尝试帮助你解决问题。*好的代码对所有人都有益*。

### 动态更新
EXILED框架支持在不重启服务器的情况下动态重新载入插件程序集。
打个比方， 如果你启动的服务器只安装了`Exiled.Events`，并希望增加一个新的插件，你不需要重启你的服务器来达成这个目的。你只需要使用RA/服务控制台指令 `reload plugins` 来重新载入所有EXILED的插件或者使用``pmanager disable/enable 插件名`` 载入单个插件，包括那些曾经没有被载入过的新插件。

这也意味着你可以更新某些插件而不需要完全重新启动服务器。但热更新需要插件开发者遵守一些规则来避免出现问题。

***致服主***
 - 如果你想要更新一个插件， 请确保它的程序集名字和已经安装的版本（如有）的名字为不同的。 插件本身必须支持动态更新才能工作，只是重命名文件名则不会使其正常工作。
 - 如果一个插件支持动态更新，请务必确保你放置新版插件``Plugin``文件夹，以及你重新载入EXILED前移除了文件夹中旧版本的插件。否则，很多不好的事情会发生。
 - 任何由于动态更新插件所导致的问题是你与出现问题插件的作者的责任。 虽然EXILED完全支持并鼓励使用动态更新，但是它出现问题时，只可能是服主或者是插件开发者做了什么不该做的事情。 提交Bug给EXILED插件开发者最好提供关于动态更新前后的所有信息等，再三确认你们双方都没有弄错任何事情。

 ***致开发者***

 - 插件若想要支持动态更新，必须确保它在被关闭或重新载入时取消订阅所有之前订阅的事件。
 - 包含自制的Harmony补丁的插件必须使用某种形式的可变变量在Harmony Instance的名字里面，并且必须在插件被关闭或重新载入时``UnPatchAll()``。
 - 任何在``OnEnabled``方法中启动的协程都必须在插件被关闭或重载时结束。

以上所提到的可以通过在``Plugin``类的``OnReloaded()``或``OnDisabled()``方法来实现。当EXILED重新加载插件时，它会先执行``OnDisabled()``，再是``OnReloaded()``， 然后才是加载新程序集以及执行``OnEnabled()``。

请注意我说的是*新的*程序集。如果你替换了一个有着同样名字的程序集，它***不会***被重新载入。这是GAC(全局程序集缓存)所导致的，如果你尝试``载入``一个已经在缓存的程序集，它只会使用那个已经在缓存的里的程序集。
因此，如果你的插件将会支持动态更新，你必须在生成时使用不一样的程序集名（重命名文件是没有用的）。此外，不需要用的旧程序集并不会被“清除”，如果你没有正确的取消订阅事件，取消Harmony补丁，摧毁协程等。原本的代码将会同新的代码一起运行。 
让这件事发生绝对不是一个好主意。

所以，支持动态更新的插件***必须***遵从以上规则，否则，插件将会因为可能对服主造成威胁而被从Discord服务器中移除。

不是所有的插件都必须支持动态更新。如果你不打算支持动态更新，这也完全没有问题，只是需要注意不要在更新插件后改变程序集的名字，并让服主知道他会需要重启服务器来更新你的插件。

文字记录：Misaka_ZeroTwo
