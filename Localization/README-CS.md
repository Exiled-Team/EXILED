<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/835XQcCCVv">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>    

</div>

EXILED je vysokoúrovňové rozhraní pro pluginy na servery hry SCP: Secret Laboratory. Nabízí systém "eventů", tedy událostí, které mohou vývojáři využít k manipulaci nebo změně herního kódu či implementaci vlastních funkcí.
Všechny EXILED eventy jsou kódovány pomocí Harmony, což znamená, že ke svému fungování nevyžadují přímé úpravy serverových sestav, což přináší dvě jedinečné výhody.

 - Zaprvé, celý kód rozhraní lze volně publikovat a sdílet, což vývojářům umožňuje lépe pochopit, *jak* funguje, a navrhnout doplnění nebo změnu jeho funkcí.
 - Za druhé, protože veškerý kód související s rozhraním se provádí mimo sestavu serveru, budou mít věci jako malé aktualizace hry na rozhraní jen malý vliv, pokud vůbec nějaký. Díky tomu bude s největší pravděpodobností kompatibilní s budoucími aktualizacemi hry a také bude snazší jej aktualizovat, *když* to bude nutné.

# Lokalizované README
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)

# Instalace
Instalace EXILED je poměrně jednoduchá. Načítá se prostřednictvím rozhraní NW Plugin API. Proto jsou uvnitř archivu ``Exiled.tar.gz`` ve vydaných verzích EXILED dvě složky. Složka ``SCP Secret Laboratory`` obsahuje soubory potřebné k načtení funkcí EXILED ve složce ``EXILED``. Vzhledem k tomu stačí tyto dvě složky přesunout do příslušné cesty, která je vysvětlena níže, a je hotovo!

Pokud se rozhodnete použít instalační program, postará se při správném spuštění o instalaci všech funkcí EXILED.

# Windows
### Automatická instalace ([více informací](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Poznámka:**: Před spuštěním instalačního programu se ujistěte, že jste uživatelem, který provozuje server, nebo že máte oprávnění správce.

  - Stáhněte **`Exiled.Installer-Win.exe` [odsud](https://github.com/Exiled-Team/EXILED/releases)** (Klikněte na Assets -> klikněte na Instalátor)
  - Umístěte ji do složky serveru (pokud jste ji ještě nestáhli, stáhněte si dedikovaný server).
  - Dvakrát klikněte na **`Exiled.Installer.exe`** nebo **[stáhněte tento .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** a umístěte jej do složky serveru, abyste nainstalovali nejnovější předběžnou verzi.
  - Chcete-li nainstalovat a získat pluginy, podívejte se na sekci Instalace pluginů níže.
**Poznámka:** Pokud instalujete EXILED na vzdálený server, ujistěte se, že spustíte .exe jako stejný uživatel, který spouští servery SCP:SL (nebo uživatel s právy správce).

### Manuální instalace
  - Stáhněte si **`Exiled.tar.gz` [odsud](https://github.com/Exiled-Team/EXILED/releases)**
  - Extrahujte jeho obsah pomocí [7Zip](https://www.7-zip.org/) nebo [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Přesuňte **``EXILED``** složku do **`%appdata%`** *Poznámka: Tato složka musí jít do ``C:\Users\%UserName%\AppData\Roaming``, a ***NE*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, a **MUSÍ** být v (...)\AppData\Roaming, ne (...)\AppData\!*
  - Přesuňte **``SCP Secret Laboratory``** do **`%appdata%`**.
    - Windows 10 & 11:
      Napište `%appdata%` do Cortany / vyhledávání.
    - Jakákoliv jiná verze:
      Stiskněte Win + R a napište `%appdata%`

### Instalace pluginů
To je vše, EXILED by nyní měl být nainstalován a aktivní při příštím spuštění serveru. Pozor, EXILED sám o sobě neudělá téměř nic, takže se ujistěte, že si stáhnete nové pluginy z **[našeho serveru Discord](https://discord.gg/exiledreboot)**.
- Chcete-li nainstalovat plugin, jednoduše:
  - Stáhněte si plugin z [*jeho* stránky vydání](https://i.imgur.com/u34wgPD.jpg) (**MUSÍ být `.dll`!**)
  - Přesuňte jej do: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins`` (dostaňte se sem stiskem Win + R, a pak psaním `%appdata%`)

# Linux
### Automatická instalace ([více informací](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Poznámka:** Pokud instalujete EXILED na vzdálený server, ujistěte se, že jste instalátor spustili jako stejný uživatel, který spouští servery SCP:SL (nebo jako root).

  - Stáhněte **`Exiled.Installer-Win.exe` [odsud](https://github.com/Exiled-Team/EXILED/releases)** (Klikněte na Assets -> klikněte na Instalátor)
  - Nainstalujte jej pomocí **`./Exiled.Installer-Linux --path /cesta/k/serveru`** nebo ji přesuňte přímo do složky serveru, přejděte do ní pomocí terminálu (`cd`) a zadejte: **`./Exiled.Installer-Linux`**.
  - Pokud chcete nejnovější předběžnou verzi, stačí přidat **`--pre-releases`**. Příklad: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Další příklad, pokud jste do složky serveru umístili soubor `Exiled.Installer-Linux`: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - Chcete-li nainstalovat a získat pluginy, podívejte se na sekci Instalace pluginů níže.

### Manuální instalace
  - **Ujistěte** se že jste přihlášeni jako uživatel, který spouští servery SCP: SL.
  - Stáhněte si **`Exiled.tar.gz` [odsud](https://github.com/Exiled-Team/EXILED/releases)** (SSH: klikněte pravým a zkopírujte `Exiled.tar.gz` odkaz, pak napište: **`wget (odkaz)`**)
  - Chcete-li jej rozbalit do aktuální složky, zadejte **``tar -xzvf EXILED.tar.gz``**
  - Přesuňte **`EXILED`** složku do **``~/.config``**. *Poznámka: Tuto složku je třeba umístit do ``~/.config``, a ***NE*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Přesuňte **`SCP Secret Laboratory`** složku do **``~/.config``**. *Poznámka: Tuto složku je třeba umístit do ``~/.config``, a ***NE*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Instalace pluginů
To je vše, EXILED by nyní měl být nainstalován a aktivní při příštím spuštění serveru. Pozor, EXILED sám o sobě neudělá téměř nic, takže se ujistěte, že si stáhnete nové pluginy z **[našeho serveru Discord](https://discord.gg/exiledreboot)**.
- Chcete-li nainstalovat plugin, jednoduše:
  - Stáhněte si plugin z [*jeho* stránky vydání](https://i.imgur.com/u34wgPD.jpg) (**MUSÍ být `.dll`!**)
  - Přesuňte jej do ``~/.config/EXILED/Plugins`` (pokud používáte SSH jako root, musíte najít správný `.config` který bude uvnitř `/home/(SCP Server Uživatel)`)

# Config
EXILED sám o sobě nabízí některé možnosti konfigurace.
Všechny jsou automaticky generovány při spuštění serveru a nacházejí se  v ``~/.config/EXILED/Configs/(PortServeru)-config.yml`` souboru (``%AppData%\EXILED\Configs\(PortServeru)-config.yml`` na Windows).

Plugin konfigurace ***NEBUDOU*** v již zmíněném souboru ``config_gameplay.txt``, místo toho se nacházejí v ``~/.config/EXILED/Configs/(PortServeru)-config.yml`` souboru (``%AppData%\EXILED\(ServerPortHere)-config.yml`` na Windows).
Některé pluginy však mohou získávat svá konfigurační nastavení z jiných umístění samy, toto je pro ně pouze výchozí umístění EXILED, takže v případě problémů se obraťte na jednotlivé pluginy.

# Pro vývojáře

Pokud si přejete vytvořit plugin pro EXILED, je to poměrně jednoduché. Pokud byste chtěli více návodů, navštivte prosím naši stránku [Getting Started.](https://github.com/Exiled-Team/EXILED/blob/master/GettingStarted.md).

Obsáhlejší a aktivně aktualizované tutoriály naleznete na [webové stránce EXILED](https://exiled.to).

Při publikování pluginů však dbejte na dodržování těchto pravidel:

 - Váš plugin musí obsahovat třídu která dědí z ``Exiled.API.Features.Plugin<>``, pokud tomu tak není, EXILED váš plugin při spuštění serveru nenačte.
 - Při načtení pluginu se kód v metodě ``OnEnabled()`` výše uvedené třídy spustí okamžitě, nečeká se na načtení ostatníchpluginů Nečeká na dokončení procesu spouštění serveru. ***Nečeká na nic.*** Při nastavování metody ``OnEnable()`` dbejte na to, abyste nepřistupovali  k věcem, které ještě nemusí být serverem inicializovány, jako je ``ServerConsole.Port`` nebo ``PlayerManager.localPlayer``.
 - Pokud potřebujete přistupovat k věcem, které nejsou inicializovány před načtením pluginu, doporučujeme jednoduše počkat na event ``WaitingForPlayers``, pokud z nějakého důvodu potřebujete provést věci dříve, zabalte kód do smyčky ``` while(!x)```, která před pokračováním zkontroluje, zda proměnná/objekt, který potřebujete, již není null.
 - EXILED podporuje dynamické načítání sestav pluginů během spuštění. To znamená, že pokud potřebujete aktualizovat plugin, lze to provést bez restartu serveru, nicméně pokud aktualizujete zásuvný modul uprostřed provádění, musí být plugin správně nastaven tak, aby to podporoval, jinak budete mít velmi zlé časy. Další informace a pokyny, kterými je třeba se řídit, naleznete v části ``Dynamické aktualizace``.
 - V rozhraní EXILED neexistuje žádná událost OnUpdate, OnFixedUpdate nebo OnLateUpdate. Pokud potřebujete z nějakého důvodu spouštět kód tak často, můžete místo toho použít koroutinu MEC, která čeká na jeden snímek, 0,01f, nebo použít vrstvu Timing, jako je Timing.FixedUpdate.

### Zakázání záplat EXILED eventů
***Tato funkce již není v současné době implementována.***

 ### MEC Koroutiny
Pokud se v systému MEC nevyznáte, bude to pro vás velmi stručný a jednoduchý úvod.
Koroutiny MEC jsou v podstatě časované metody, které podporují čekání po určitou dobu před pokračováním v provádění, aniž by přerušily/uspaly hlavní herní vlákno.
Na rozdíl od tradičního threadingu je použití MEC coroutines v Unity bezpečné. *** NEPOKOUŠEJTE se vytvářet nová vlákna pro interakci s Unity, DOJDE pádu serveru ***.

Chcete-li použít MEC, musíte ze souborů serveru uvést odkaz na ``Assembly-CSharp-firstpass.dll`` a zahrnout ``using MEC;```.
Příklad volání jednoduché koroutiny, která se opakuje se zpožděním mezi jednotlivými smyčkami:
```cs
using MEC;
using Exiled.API.Features;

public void NejakaMetoda()
{
    Timing.RunCoroutine(MojeKoroutina());
}

public IEnumerator<float> MojeKoroutina()
{
    for (;;) //opakuj následující navždy
    {
        Log.Info("Ahoj! Jsem nekonecna smycka!"); //Voláním Log.Info vypíšete řádek do herní konzole/logů serveru.
        yield return Timing.WaitForSeconds(5f); //Řekne koroutině, aby před pokračováním počkala 5 sekund, protože se nachází na konci smyčky, a tím efektivně zastaví opakování smyčky na 5 sekund.
    }
}
```

Pokud MEC neznáte a chcete se dozvědět více, získat radu nebo potřebujete pomoc, **silně** vám doporučujeme, abyste si něco vygooglovali nebo se zeptali v Discordu. Otázky, bez ohledu na to, jak "hloupé" jsou, budou vždy zodpovězeny co nejpomocněji a nejjasněji, aby vývojáři pluginů mohli excelovat. Lepší kód je lepší pro všechny.

### Dynamické aktualizace
EXILED jako rozhraní podporuje dynamické načítání sestav pluginů bez nutnosti restartu serveru.
Pokud například spustíte server s jediným pluginem `Exiled.Events` a budete chtít přidat nový, nemusíte pro dokončení tohoto úkolu restartovat server. Můžete jednoduše použít příkaz `reload plugins` v konzole RemoteAdmin/ServerConsole, abyste znovu načetli všechny pluginy EXILED, včetně nových, které předtím nebyly načteny.

To také znamená, že můžete *aktualizovat* pluginy, aniž byste museli plně restartovat server. Existuje však několik pokynů, které musí vývojáři zásuvných modulů dodržet, aby toho bylo dosaženo správně:

***Pro Uživatele***

 - Pokud aktualizujete plugin, ujistěte se, že název jeho sestavy není stejný jako název aktuální nainstalované verze (pokud existuje). Aby to fungovalo, musí být plugin vytvořen vývojářem s ohledem na dynamické aktualizace, pouhé přejmenování souboru nefunguje. - If the plugin supports Dynamic Updates, be sure that when you put the newer version of the plugin into the "Plugins" folder, you also remove the older version from the folder, before reloading EXILED, failure to ensure this will result in many many bad things.
 - Za případné problémy, které vzniknou při dynamické aktualizaci pluginu, nesete odpovědnost výhradně vy a vývojář daného pluginu. Přestože EXILED plně podporuje a doporučuje dynamické aktualizace, jedinou možností, jak by mohly selhat nebo se pokazit, je, že by hostitel serveru nebo vývojář pluginu udělal něco špatně. Než vývojářům EXILED nahlásíte chybu týkající se dynamických aktualizací, třikrát zkontrolujte, zda obě tyto strany provedly vše správně.

 ***Pro Vývojáře***

 - Pluginy, které chtějí podporovat dynamickou aktualizaci, se musí ujistit, že se odhlásí ze všech eventů, ke kterým jsou připojeny, když jsou vypnuty nebo znovu načteny. - Plugins that have custom Harmony patches must use some kind of changing variable within the name of the Harmony Instance, and must UnPatchAll() on their harmony instance when the plugin is disabled or reloaded.
 - Všechny koroutiny spuštěné pluginem v ``OnEnabled()`` musí být také ukončeny, když je plugin vypnut nebo znovu načten.

Toho všeho lze dosáhnout pomocí metod ``OnReloaded()``` nebo ``OnDisabled()`` ve třídě pluginu. Když EXILED načítá pluginy, volá OnDisabled(), pak ``OnReloaded()``, poté načte nové sestavy a pak provede ``OnEnabled()``.

Všimněte si, že jsem řekl *nové* sestavy. Pokud nahradíte sestavu jinou se stejným názvem, ***NEBUDE*** aktualizována. To je způsobeno GAC (Global Assembly Cache), pokud se pokusíte "načíst" sestavu, která je již v mezipaměti, vždy se místo ní použije sestava z mezipaměti.
Pokud bude váš plugin podporovat dynamické aktualizace, musíte každou verzi sestavit s jiným názvem sestavení v možnostech sestavení (přejmenování souboru nebude fungovat). Navíc, protože stará sestava není "zničena", když už není potřeba, pokud se vám nepodaří odhlásit se z událostí, odepnout instanci harmony, zabít koroutiny atd., bude tento kód běžet dál stejně jako kód nové verze.
Nechat toto se stát je velmi velmi špatný nápad.

Pluginy, které podporují dynamické aktualizace, proto ***MUSÍ*** dodržovat tyto pokyny, jinak budou ze Discord serveru odstraněny z důvodu možného rizika pro hostitele serveru.

Ne každý plugin však musí podporovat dynamické aktualizace. Pokud nemáte v úmyslu podporovat dynamické aktualizace, je to naprosto v pořádku, jednoduše při vytváření nové verze nezměňte Assembly Name svého pluginu a nemusíte se o nic z toho starat, jen se ujistěte, že hostitelé serverů vědí, že budou muset kompletně restartovat své servery, aby mohli váš plugin aktualizovat.

