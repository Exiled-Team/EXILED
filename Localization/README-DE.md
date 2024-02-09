<h1 align="center">EXILED - Erweiterte Laufzeitbibliothek für externe Development</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/PyUkWTg">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat auf Discord">
</a>    

</div>

EXILED ist ein hochrangiges Plugin-Framework für SCP: Secret Laboratory Server. Es bietet ein Event System, in das Entwickler einhaken können, um Spielcodes zu manipulieren oder zu ändern oder ihre eigenen Funktionen zu implementieren.
Alle EXILED-EVENTS sind mit Harmony kodiert, was bedeutet, dass sie keine direkte Bearbeitung der Serverassemblies benötigen, um zu funktionieren, was zwei einzigartige Vorteile ermöglicht.

 - Erstens kann der gesamte Code des Frameworks frei veröffentlicht und geteilt werden, was es Entwicklern ermöglicht, besser zu verstehen, *wie* es funktioniert und Vorschläge für das Hinzufügen oder Ändern seiner Funktionen zu bieten.
 - Zweitens, da der gesamte Code im Zusammenhang mit dem Framework außerhalb der Serverassembly erfolgt, haben Dinge wie kleine Spielupdates wenig oder gar keinen Einfluss auf das Framework. Dies macht es höchstwahrscheinlich mit zukünftigen Spielupdates kompatibel, sowie einfacher zu aktualisieren, wenn es *notwendig* ist.

# Lokalisierte READMEs
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Italiano](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-IT.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)
- [Türkçe](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-TR.md)

# Installation
Die Installation von EXILED ist ziemlich einfach. Es lädt sich selbst über Northwoods Plugin API. Deshalb gibt es zwei Ordner in den ``Exiled.tar.gz`` Release-Dateien. ``SCP Secret Laboratory`` enthält die notwendigen Dateien, um EXILED-Funktionen im ``EXILED`` Ordner zu laden. Alles, was Sie tun müssen, ist, diese beiden Ordner in den entsprechenden Pfad zu verschieben, der unten erklärt wird, und Sie sind fertig!

Wenn Sie den Installer verwenden, wird er, wenn er korrekt ausgeführt wird, sich um die Installation aller EXILED-Funktionen kümmern.

# Windows
### Automatische Installation ([mehr Informationen](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Hinweis**: Stellen Sie sicher, dass Sie als Benutzer, der den Server ausführt, angemeldet sind, oder Sie Adminrechte haben, bevor Sie den Installer ausführen.

  - Laden Sie den **`Exiled.Installer-Win.exe` [hier](https://github.com/Exiled-Team/EXILED/releases)** herunter (klicken Sie auf Assets -> klicken Sie auf den Installer)
  - Platzieren Sie ihn in Ihrem Serverordner (laden Sie den dedizierten Server herunter, falls Sie ihn noch nicht haben)
  - Doppelklicken Sie auf die **`Exiled.Installer.exe`** oder **[laden Sie diese .bat herunter](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** und platzieren Sie sie im Serverordner, um die neueste Vorabversion zu installieren
  - Um Plugins zu erhalten und zu installieren, überprüfen Sie den Abschnitt [Plugins installieren](#plugins-installieren) weiter unten.
**Hinweis:** Wenn Sie EXILED auf einem Remote-Server installieren, stellen Sie sicher, dass Sie die .exe als denselben Benutzer ausführen, der Ihre SCP:SL-Server ausführt (oder einen mit Adminrechten)

### Manuelle Installation
  - Laden Sie die **`Exiled.tar.gz` [hier](https://github.com/Exiled-Team/EXILED/releases)** herunter
  - Extrahieren Sie dessen Inhalt mit [7Zip](https://www.7-zip.org/) oder [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Verschieben Sie den **``EXILED``** Ordner nach **`%appdata%`** *Hinweis: Dieser Ordner muss in ``C:\Users\%UserName%\AppData\Roaming`` gehen, und ***NICHT*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, und **ER MUSS** in (...)\AppData\Roaming sein, nicht (...)\AppData\!*
  - Verschieben Sie **``SCP Secret Laboratory``** nach **`%appdata%`**.
    - Windows 10 & 11:
      Schreiben Sie `%appdata%` in Cortana / das Suchsymbol oder die Windows Explorer-Leiste.
    - Jede andere Windows-Version:
      Drücken Sie Win + R und tippen Sie `%appdata%`

### Plugins installieren
Das war's, EXILED sollte nun installiert sein und beim nächsten Start Ihres Servers aktiv sein. Beachten Sie, dass EXILED allein fast nichts tut, also stellen Sie sicher, dass Sie neue Plugins von **[unserem Discord-Server](https://discord.gg/PyUkWTg)** bekommen
- Um ein Plugin zu installieren, einfach:
  - Laden Sie ein Plugin von [*deren* Release-Seite](https://i.imgur.com/u34wgPD.jpg) herunter (**es MUSS eine `.dll` sein!**)
  - Verschieben Sie es nach: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins`` (bewegen Sie sich hierhin, indem Sie Win + R drücken, dann `%appdata%` schreiben)

# Linux
### Automatische Installation ([mehr Informationen](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Hinweis:** Wenn Sie EXILED auf einem Remote-Server installieren, stellen Sie sicher, dass Sie den Installer als denselben Benutzer ausführen, der Ihre SCP:SL-Server ausführt (oder root)

  - Laden Sie den **`Exiled.Installer-Linux` [hier](https://github.com/Exiled-Team/EXILED/releases)** herunter (klicken Sie auf Assets -> laden Sie den Installer herunter)
  - Installieren Sie ihn, indem Sie entweder **`./Exiled.Installer-Linux --path /path/to/server`** eingeben oder ihn direkt in den Serverordner verschieben, mit dem Terminal dorthin wechseln (`cd`) und tippen: **`./Exiled.Installer-Linux`**.
  - Wenn Sie die neueste Vorabversion möchten, fügen Sie einfach **`--pre-releases`** hinzu. Beispiel: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Ein weiteres Beispiel, wenn Sie `Exiled.Installer-Linux` in Ihren Serverordner gelegt haben: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - Um Plugins

 zu erhalten und zu installieren, überprüfen Sie den Abschnitt [Plugins installieren](#plugins-installieren-1) weiter unten.

### Manuelle Installation
  - **Stellen Sie sicher**, dass Sie als Benutzer angemeldet sind, der die SCP-Server ausführt.
  - Laden Sie die **`Exiled.tar.gz` [hier](https://github.com/Exiled-Team/EXILED/releases)** herunter (SSH: Rechtsklick und um den `Exiled.tar.gz` Link zu bekommen, dann tippen: **`wget (link_zum_herunterladen)`**)
  - Um es in Ihren aktuellen Ordner zu extrahieren, tippen Sie **``tar -xzvf EXILED.tar.gz``**
  - Verschieben Sie den **`EXILED`** Ordner nach **``~/.config``**. *Hinweis: Dieser Ordner muss in ``~/.config`` gehen, und ***NICHT*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Verschieben Sie den **`SCP Secret Laboratory`** Ordner nach **``~/.config``**. *Hinweis: Dieser Ordner muss in ``~/.config`` gehen, und ***NICHT*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Plugins installieren
Das war's, EXILED sollte nun installiert sein und beim nächsten Start Ihres Servers aktiv sein. Beachten Sie, dass EXILED allein fast nichts tut, also stellen Sie sicher, dass Sie Plugins von **[unserem Discord-Server](https://discord.gg/PyUkWTg)** bekommen
- Um ein Plugin zu installieren, einfach:
  - Laden Sie ein Plugin von [*deren* Release-Seite](https://i.imgur.com/u34wgPD.jpg) herunter (**es MUSS eine `.dll` sein!**)
  - Verschieben Sie es nach: ``~/.config/EXILED/Plugins`` (wenn Sie Ihr SSH als Root verwenden, dann suchen Sie nach dem richtigen `.config`, das sich in `/home/(SCP Server Benutzer)` befindet)

# Konfiguration
EXILED bietet selbst einige Konfigurationsoptionen.
Alle davon werden beim Serverstart automatisch generiert, sie befinden sich in der Datei ``~/.config/EXILED/Configs/(ServerPortHier)-config.yml`` (``%AppData%\EXILED\Configs\(ServerPortHier)-config.yml`` auf Windows).

Plugin-Konfigurationen werden ***NICHT*** in der oben genannten ``config_gameplay.txt`` Datei sein, stattdessen werden Plugin-Konfigurationen in der ``~/.config/EXILED/Configs/(ServerPortHier)-config.yml`` Datei gesetzt (``%AppData%\EXILED\(ServerPortHier)-config.yml`` auf Windows).
Einige Plugins erhalten jedoch ihre Konfigurationseinstellungen aus anderen Standorten auf eigene Faust, dies ist einfach der Standardort von EXILED für sie, daher beziehen Sie sich auf das einzelne Plugin, wenn es Probleme gibt.

# Für Entwickler

Wenn Sie ein Plugin für EXILED erstellen möchten, ist es ziemlich einfach. Wenn Sie mehr ein Tutorial möchten, besuchen Sie bitte unsere [Getting Started Seite.](https://github.com/Exiled-Team/EXILED/blob/master/GettingStarted.md).

Für umfassendere und aktuell gehaltene Tutorials siehe [die EXILED-Website](https://exiled-team.github.io/EXILED/articles/install.html).

Aber stellen Sie sicher, dass Sie diese Regeln befolgen, wenn Sie Ihre Plugins veröffentlichen:

 - Ihr Plugin muss eine Klasse enthalten, die von ``Exiled.API.Features.Plugin<>`` erbt, wenn nicht, wird EXILED Ihr Plugin beim Serverstart nicht laden.
 - Wenn ein Plugin geladen wird, wird der Code innerhalb der besagten Klasse ``OnEnabled()`` Methode sofort ausgeführt, es wartet nicht darauf, dass andere Plugins geladen werden. Es wartet nicht darauf, dass der Serverstartprozess abgeschlossen ist. ***Es wartet auf nichts.*** Beim Einrichten Ihrer ``OnEnabled()`` Methode stellen Sie sicher, dass Sie nicht auf Dinge zugreifen, die vom Server noch nicht initialisiert sind, wie ``ServerConsole.Port`` oder ``PlayerManager.localPlayer``.
 - Wenn Sie frühzeitig auf Dinge zugreifen müssen, die vor dem Laden Ihres Plugins nicht initialisiert wurden, wird empfohlen, auf das ``WaitingForPlayers`` Ereignis zu warten, um dies zu tun. Wenn Sie Dinge früher tun müssen, umschließen Sie

 den Code in einer ``` while(!x)``` Schleife, die überprüft, ob die Variable/das Objekt, das Sie benötigen, nicht mehr null ist, bevor Sie fortfahren.
 - EXILED unterstützt das dynamische Nachladen von Plugin-Assemblys während der Ausführung, was bedeutet, dass wenn Sie ein Plugin aktualisieren müssen, dies ohne Neustart des Servers erfolgen kann. Wenn Sie jedoch ein Plugin während der Ausführung aktualisieren, muss das Plugin richtig eingerichtet sein, um dies zu unterstützen, oder Sie werden eine sehr schlechte Zeit haben. Beziehen Sie sich auf den Abschnitt ``Dynamische Updates`` für weitere Informationen und Richtlinien zum Befolgen.
 - Es gibt ***KEIN*** OnUpdate, OnFixedUpdate oder OnLateUpdate Ereignis innerhalb von EXILED. Wenn Sie Code ausführen müssen, der oft läuft, können Sie eine MEC-Coroutine verwenden, die auf ein Frame, 0.01f wartet, oder eine Timing-Schicht wie Timing.FixedUpdate stattdessen verwenden.
 ### MEC-Coroutines
Wenn Sie mit MEC nicht vertraut sind, wird dies ein sehr kurzer und einfacher Primer sein, um Ihnen den Einstieg zu erleichtern.
MEC-Coroutines sind im Grunde genommen zeitgesteuerte Methoden, die Wartezeiten unterstützen, bevor sie die Ausführung fortsetzen, ohne den Hauptspielfaden zu unterbrechen/schlafen zu legen.
MEC-Coroutines sind sicher in der Verwendung mit Unity, im Gegensatz zu traditionellem Threading. ***VERSUCHEN Sie NICHT, neue Threads zu erstellen, um mit Unity zu interagieren, sie WERDEN den Server abstürzen lassen.***

Um MEC zu verwenden, müssen Sie ``Assembly-CSharp-firstpass.dll`` aus den Serverdateien referenzieren und ``using MEC;`` einbeziehen.
Beispiel für den Aufruf einer einfachen Coroutine, die sich mit einer Verzögerung zwischen jeder Schleife wiederholt:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //wiederhole das Folgende unendlich
    {
        Log.Info("Hey, ich bin eine unendliche Schleife!"); //Rufe Log.Info auf, um eine Zeile in die Spielkonsole/Serverprotokolle zu drucken.
        yield return Timing.WaitForSeconds(5f); //Sagt der Coroutine, 5 Sekunden zu warten, bevor sie fortfährt, da dies am Ende der Schleife steht, hält es effektiv die Schleife davon ab, sich für 5 Sekunden zu wiederholen.
    }
}
```

Es wird ***stark*** empfohlen, etwas zu googeln oder im Discord herumzufragen, wenn Sie mit MEC nicht vertraut sind und mehr erfahren möchten, Rat suchen oder Hilfe benötigen. Fragen, egal wie 'dumm' sie sind, werden immer so hilfreich und klar wie möglich beantwortet, um Plugin-Entwicklern zu helfen, sich zu verbessern. Besserer Code ist besser für alle.

### Dynamische Updates
EXILED als Framework unterstützt das dynamische Nachladen von Plugin-Assemblys ohne einen Serverneustart zu benötigen.
Zum Beispiel, wenn Sie den Server nur mit `Exiled.Events` als einziges Plugin starten und ein neues hinzufügen möchten, müssen Sie den Server nicht neu starten, um diese Aufgabe zu erfüllen. Sie können einfach den Remote Admin oder Server Console Befehl `reload plugins` verwenden, um alle EXILED-Plugins neu zu laden, einschließlich neuer, die vorher nicht geladen wurden.

Das bedeutet auch, dass Sie Plugins aktualisieren können, ohne den Server vollständig neu starten zu müssen. Es gibt jedoch einige Richtlinien, die vom Plugin-Entwickler befolgt werden müssen, damit dies ordnungsgemäß erreicht werden kann:

***Für Hosts***
 - Wenn Sie ein Plugin aktualisieren, stellen Sie sicher, dass sein Assembly-Name nicht derselbe ist wie die aktuelle Version, die Sie installiert haben (falls vorhanden). Das Plugin muss vom Entwickler mit dynamischen Updates im Hinterkopf gebaut worden sein, damit dies funktioniert, einfach das Umbenennen der Datei wird nicht funktionieren.
 - Wenn das Plugin dynamische Updates unterstützt, stellen Sie sicher, dass Sie, wenn Sie die neuere Version des Plugins in den "Plugins"-Ordner legen, auch die ältere Version aus dem Ordner entfernen, bevor Sie EX

ILED neu laden. Das Versäumnis, dies zu gewährleisten, wird in vielen schlechten Dingen resultieren.
 - Alle Probleme, die aus dem dynamischen Aktualisieren eines Plugins entstehen, sind ausschließlich die Verantwortung von Ihnen und dem Entwickler des betreffenden Plugins. Während EXILED dynamische Updates vollständig unterstützt und fördert, könnte der einzige Weg, wie es fehlschlagen oder schiefgehen könnte, sein, wenn der Serverhost oder Plugin-Entwickler etwas falsch gemacht hat. Überprüfen Sie, dass alles korrekt von beiden Parteien gemacht wurde, bevor Sie einen Fehler den EXILED-Entwicklern bezüglich dynamischer Updates melden.

 ***Für Entwickler***

 - Plugins, die dynamische Updates unterstützen möchten, müssen sicherstellen, dass sie sich von allen Ereignissen, in die sie eingehakt sind, abmelden, wenn sie deaktiviert oder neu geladen werden.
 - Plugins, die benutzerdefinierte Harmony-Patches haben, müssen irgendeine Art von wechselnder Variablen innerhalb des Namens der Harmony-Instanz verwenden und müssen ``UnPatchAll()`` auf ihrer Harmony-Instanz ausführen, wenn das Plugin deaktiviert oder neu geladen wird.
 - Alle Coroutinen, die vom Plugin in ``OnEnabled()`` gestartet wurden, müssen auch getötet werden, wenn das Plugin deaktiviert oder neu geladen wird.

All dies kann in den Methoden ``OnReloaded()`` oder ``OnDisabled()`` in der Plugin-Klasse erreicht werden. Wenn EXILED Plugins neu lädt, ruft es ``OnDisabled()``, dann ``OnReloaded()``, dann lädt es die neuen Assemblys und führt dann ``OnEnabled()`` aus.

Beachten Sie, dass es *neue* Assemblys sind. Wenn Sie eine Assembly durch eine andere mit demselben Namen ersetzen, wird ***sie NICHT*** aktualisiert. Dies liegt am GAC (Global Assembly Cache), wenn Sie versuchen, eine Assembly zu 'laden', die bereits im Cache ist, wird immer die gecachte Assembly verwendet.
Aus diesem Grund, wenn Ihr Plugin dynamische Updates unterstützt, müssen Sie jede Version mit einem anderen Assembly-Namen in den Build-Optionen bauen (das Umbenennen der Datei wird nicht funktionieren). Da die alte Assembly nicht "zerstört" wird, wenn sie nicht mehr benötigt wird, wenn Sie sich nicht von Ereignissen abmelden, Ihre Harmony-Instanz nicht unpatchen, Coroutinen nicht töten usw., wird dieser Code weiterhin ausgeführt sowie der Code der neuen Version.
Dies ist eine extrem schlechte Idee, dies zuzulassen.

Daher müssen Plugins, die dynamische Updates unterstützen, ***diesen Richtlinien folgen*** oder sie werden wegen potenzieller Risiken für Serverhosts vom Discord-Server entfernt.

Nicht jedes Plugin muss dynamische Updates unterstützen. Wenn Sie nicht beabsichtigen, dynamische Updates zu unterstützen, ist das völlig in Ordnung. Vermeiden Sie einfach, den Assembly-Namen Ihres Plugins zu ändern, wenn Sie eine neue Version bauen. In solchen Fällen stellen Sie sicher, dass Serverhosts wissen, dass sie ihre Server vollständig neu starten müssen, um Ihr Plugin zu aktualisieren.