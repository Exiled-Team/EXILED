<h1 align="center">EXILED - EXtended In-runtime Library for External Development</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/PyUkWTg">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>    

</div>

EXILED er et plugin-framework på højt niveau til SCP: Secret Laboratory-servere. Det tilbyder et event-system, som udviklere kan bruge til at manipulere eller ændre spilkoden eller implementere deres egne funktioner.
Alle EXILED-hændelser er kodet med Harmony, hvilket betyder, at de ikke kræver direkte redigering af server Assemblies for at fungere, hvilket giver to unikke fordele.

 - For det første kan hele frameworks-koden frit offentliggøres og deles, så udviklere bedre kan forstå, *hvordan* det fungerer, og komme med forslag til at tilføje eller ændre dets funktioner.
 - For det andet, da al kode relateret til frameworket er lavet uden for serversamlingen, vil ting som små spilopdateringer have lille, hvis nogen, effekt på frameworket. Det gør det mere sandsynligt, at det er kompatibelt med fremtidige spilopdateringer, og gør det nemmere at opdatere, når det *er* nødvendigt at gøre det.

# Localized READMEs
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Italiano](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-IT.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)

# Installation
Installation af EXILED er ganske enkel. Den indlæser sig selv gennem NW Plugin API. Det er derfor, der er to mapper inde i ``Exiled.tar.gz`` i udgivelsesfiler. ``SCP Secret Laboratory`` indeholder de nødvendige filer til at indlæse EXILED-funktioner i ``EXILED``-mappen. Når det er sagt, er alt, hvad du skal gøre, at flytte disse to mapper til den rette sti, som er forklaret nedenfor, og du er færdig! 

Hvis du vælger at bruge installationsprogrammet, vil det, hvis det køres korrekt, sørge for at installere alle EXILED-funktioner.

# Windows
### Automatisk installation ([mere information](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Note**: Sørg for, at du er på den bruger, der kører serveren, eller at du har administratorrettigheder, før du kører installationsprogrammet.

  - Download **`Exiled.Installer-Win.exe` [herfra](https://github.com/Exiled-Team/EXILED/releases)** (klik på Assets -> klik på Installer)  - Placer den i din servermappe (download den dedikerede server, hvis du ikke har gjort det).
  - Dobbeltklik på **`Exiled.Installer.exe`** eller **[download denne .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** og placer den i servermappen for at installere den seneste pre-release
  - For at installere og hente plugins, tjek afsnittet [Installing plugins](#installing-plugins) nedenfor.
**Bemærk:** Hvis du installerer EXILED på en fjernserver, skal du sørge for at køre .exe som den samme bruger, der kører dine SCP:SL-servere (eller en med administratorrettigheder).

### Manuel installation
  - Download **`Exiled.tar.gz` [herfra](https://github.com/Exiled-Team/EXILED/releases)**
  - Udpak indholdet med [7Zip](https://www.7-zip.org/) eller [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Flyt mappen **``EXILED``** til **`%appdata%`** *Bemærk: Denne mappe skal ligge i ``C:\Users\%UserName%\AppData\Roaming``, og ***IKKE*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, og **Den SKAL** ligge i (...)\AppData\Roaming\SCP Secret Laboratory``. ..)\AppData\Roaming, ikke (...)\AppData\!*  - Flyt **``SCP Secret Laboratory``** til **`%appdata%`**.
    - Windows 10 og 11:
      Skriv `%appdata%` i Cortana / søgeikonet eller Windows Explorer-linjen
    - Alle andre Windows-versioner:
      Tryk på Win + R og skriv `%appdata%`.

### Installation af plugins
Det var det, EXILED skulle nu være installeret og aktiv, næste gang du starter din server op. Bemærk, at EXILED i sig selv næsten ikke gør noget, så sørg for at hente nye plugins fra **[vores Discord-server](https://discord.gg/PyUkWTg)**.
- For at installere et plugin skal du blot:
  - Download et plugin fra [*deres* udgivelsesside](https://i.imgur.com/u34wgPD.jpg) (**det SKAL være en `.dll`!**)  - Flyt det til: ``C:\Users\(Your_User)\AppData\Roaming\EXILED\Plugins`` (flyt den hertil ved at trykke Win + R, og skriv derefter `%appdata%`)

# Linux
### Automatisk installation ([mere information](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Bemærk:** Hvis du installerer EXILED på en fjernserver, skal du sørge for at køre installationsprogrammet som den samme bruger, der kører dine SCP:SL-servere (eller root).

  - Download **`Exiled.Installer-Linux` [herfra](https://github.com/Exiled-Team/EXILED/releases)** (klik på Assets -> download Installer)
  - Installer den ved enten at skrive **`./Exiled.Installer-Linux --path /path/to/server`** eller flyt den direkte ind i servermappen, gå til den med terminalen (`cd`) og skriv: **`./Exiled.Installer-Linux`**.
  - Hvis du vil have den seneste pre-release, skal du blot tilføje **`--pre-releases`**. Eksempel: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**.
  - Et andet eksempel, hvis du placerede `Exiled.Installer-Linux` i din servermappe: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - For at installere og hente plugins, se afsnittet [Installation af plugins](#installing-plugins-1) nedenfor.

### Manuel installation
  - Sørg for, at du er logget ind på den bruger, der kører SCP-serverne.
  - Download **`Exiled.tar.gz` [herfra](https://github.com/Exiled-Team/EXILED/releases)** (SSH: højreklik og hent linket `Exiled.tar.gz`, skriv derefter: **`wget (link_til_download)`**)
  - For at udpakke den til din nuværende mappe, skriv **``tar -xzvf EXILED.tar.gz``**.
  - Flyt mappen **`EXILED`** til **`~/.config``**. *Bemærk: Denne mappe skal ligge i ``~/.config``, og ***IKKE*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Flyt mappen **`SCP Secret Laboratory`** til **`~/.config``**. *Bemærk: Denne mappe skal være i ``~/.config``, og ***IKKE*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Installation af plugins
Det var det, EXILED skulle nu være installeret og aktiv, næste gang du starter din server op. Bemærk, at EXILED i sig selv næsten ikke gør noget, så sørg for at hente nye plugins fra **[vores Discord-server](https://discord.gg/PyUkWTg)**.
- For at installere et plugin skal du blot:
  - Download et plugin fra [*deres* udgivelsesside](https://i.imgur.com/u34wgPD.jpg) (**det SKAL være en `.dll`!**)
  - Flyt det til: ``~/.config/EXILED/Plugins`` (hvis du bruger din SSH som root, så søg efter den korrekte `.config`, som vil være inde i `/home/(SCP Server Bruger)`)

# Konfig
EXILED tilbyder i sig selv nogle konfigurationsmuligheder.
De genereres alle automatisk ved opstart af serveren og findes i filen ``~/.config/EXILED/Configs/(ServerPortHer)-config.yml`` (``%AppData%\EXILED\Configs\(ServerPortHer)-config.yml`` på Windows).

Plugin-konfigurationer vil ***IKKE*** være i den førnævnte ``config_gameplay.txt``-fil, i stedet indstilles plugin-konfigurationer i ``~/.config/EXILED/Configs/(ServerPortHer)-config.yml``-filen (``%AppData%\EXILED\(ServerPortHer)-config.yml`` på Windows).
Nogle plugins kan dog selv hente deres konfigurationsindstillinger fra andre steder, dette er blot standard EXILED-placeringen for dem, så henvis til det enkelte plugin, hvis der er problemer.

# For udviklere

Hvis du ønsker at lave et plugin til EXILED, er det ganske enkelt at gøre det. Hvis du gerne vil have mere vejledning, kan du besøge vores [Start Side](https://github.com/Exiled-Team/EXILED/blob/master/GettingStarted.md).

For mere omfattende og aktivt opdaterede vejledninger, se [EXILED-webstedet](https://exiled-team.github.io/EXILED/articles/install.html).

Men sørg for at følge disse regler, når du udgiver dine plugins:

 - Dit plugin skal indeholde en klasse, der arver fra ``Exiled.API.Features.Plugin<>``, hvis den ikke gør, vil EXILED ikke indlæse dit plugin, når serveren starter.
 - Når et plugin indlæses, udløses koden i den førnævnte klasses ``OnEnabled()``-metode med det samme, den venter ikke på, at andre plugins bliver indlæst. Den venter ikke på, at serverens opstartsproces er færdig. ***Når du opsætter din ``OnEnabled()``-metode, skal du være sikker på, at du ikke tilgår ting, som måske ikke er initialiseret af serveren endnu, såsom ``ServerConsole.Port`` eller ``PlayerManager.localPlayer``.
 - Hvis du har brug for at tilgå ting tidligt, som ikke er initialiseret, før dit plugin er indlæst, anbefales det blot at vente på ``WaitingForPlayers``-hændelsen for at gøre det, hvis du af en eller anden grund har brug for at gøre tingene hurtigere, skal du pakke koden ind i en `` while(!x)```-løkke, der kontrollerer, at den variabel/det objekt, du har brug for, ikke længere er null, før du fortsætter.
 - EXILED understøtter dynamisk genindlæsning af plugin-samlinger midt i udførelsen. Det betyder, at hvis du har brug for at opdatere et plugin, kan det gøres uden at genstarte serveren, men hvis du opdaterer et plugin midt i udførelsen, skal pluginnet være korrekt konfigureret til at understøtte det, ellers vil du få en meget dårlig tid. Se afsnittet ``Dynamiske opdateringer`` for flere oplysninger og retningslinjer, der skal følges.
 - Der er ***INGEN*** OnUpdate-, OnFixedUpdate- eller OnLateUpdate-begivenheder i EXILED. Hvis du af en eller anden grund har brug for at køre kode så ofte, kan du bruge en MEC coroutine, der venter på en frame, 0.01f, eller bruge et Timing-lag som Timing.FixedUpdate i stedet.

### Deaktivering af EXILED Event-patches
***Denne funktion er i øjeblikket ikke længere implementeret.***

 ### MEC Coroutines
Hvis du ikke er bekendt med MEC, vil dette være en meget kort og enkel introduktion til at komme i gang.
MEC Coroutines er dybest set tidsindstillede metoder, der understøtter venteperioder, før de fortsætter udførelsen, uden at afbryde/sove spillets hovedtråd.
MEC coroutines er sikre at bruge med Unity, i modsætning til traditionel threading. ***forsøg IKKE at lave nye tråde til at interagere med Unity på, de VIL få serveren til at gå ned ***.

For at bruge MEC skal du henvise til ``Assembly-CSharp-firstpass.dll`` fra serverfilerne og inkludere ``using MEC;``.
Eksempel på kald af en simpel coroutine, der gentager sig selv med en forsinkelse mellem hver loop:
```cs
using MEC;
using Exiled.API.Features;

public void Metode()
{
    Timing.RunCoroutine(MinCoroutine());
}

public IEnumerator<float> MinCoroutine()
{
    for (;;) //gentag følgende i det uendelige
    {
        Log.Info("Jeg er en uendelig løkke!"); //Kald Log.Info for at udskrive en linje til spilkonsollen/serverens logfiler.
        yield return Timing.WaitForSeconds(5f); //Beskriver, at coroutinen skal vente 5 sekunder, før den fortsætter, og da dette er i slutningen af løkken, forhindrer det effektivt løkken i at gentage sig i 5 sekunder.
    }
}
```

Det anbefales ***stærkt***, at du googler lidt eller spørger rundt i Discord, hvis du ikke er bekendt med MEC og gerne vil lære mere, få råd eller brug for hjælp. Spørgsmål, uanset hvor "dumme" de er, vil altid blive besvaret så hjælpsomt og klart som muligt, så plugin-udviklere kan udmærke sig. Bedre kode er bedre for alle.

### Dynamiske opdateringer
EXILED som framework understøtter dynamisk genindlæsning af plugin-samlinger uden at kræve en genstart af serveren.
Hvis du for eksempel starter serveren med `Exiled.Events` som det eneste plugin og ønsker at tilføje et nyt, behøver du ikke at genstarte serveren for at fuldføre denne opgave. Du kan blot bruge RemoteAdmin/ServerConsole-kommandoen `reload plugins` til at genindlæse alle EXILED-plugins, inklusive de nye, der ikke var indlæst før.

Det betyder også, at du kan *opdatere* plugins uden også at skulle genstarte serveren helt. Der er dog et par retningslinjer, der skal følges af plugin-udvikleren, for at dette kan opnås korrekt:

***For server værter***
 - Hvis du opdaterer et plugin, skal du sørge for, at dets samlingsnavn ikke er det samme som den aktuelle version, du har installeret (hvis nogen). Plugin'et skal være bygget af udvikleren med dynamiske opdateringer i tankerne, for at dette kan fungere, det er ikke nok at omdøbe filen.
 - Hvis pluginet understøtter dynamiske opdateringer, skal du sørge for, at når du lægger den nyere version af pluginet i mappen "Plugins", fjerner du også den ældre version fra mappen, før du genindlæser EXILED, hvis du ikke sørger for dette, vil det resultere i mange mange dårlige ting.
 - Eventuelle problemer, der opstår som følge af dynamisk opdatering af et plugin, er udelukkende dit og udvikleren af det pågældende plugins ansvar. Mens EXILED fuldt ud støtter og opfordrer til dynamiske opdateringer, er den eneste måde, det kan mislykkes eller gå galt på, hvis serverværten eller plugin-udvikleren gjorde noget forkert. Tjek tre gange, at alt blev gjort korrekt af begge parter, før du rapporterer en fejl til EXILEDs udviklere vedrørende dynamiske opdateringer.

 ***For udviklere***
 - Plugins, der ønsker at understøtte Dynamisk Updatering, skal sørge for at afmelde alle events, de er hooked til, når de deaktiveres eller genindlæses.
 - Plugins, der har tilpassede Harmony-patches, skal bruge en form for skiftende variabel i navnet på Harmony-instansen og skal ``UnPatchAll()`` på deres Harmony-instans, når pluginnet deaktiveres eller genindlæses.
 - Alle coroutines startet af plugin'et i ``OnEnabled()`` skal også dræbes, når plugin'et deaktiveres eller genindlæses.

Alt dette kan opnås i enten ``OnReloaded()`` eller ``OnDisabled()`` metoderne i plugin-klassen. Når EXILED genindlæser plugins, kalder den ``OnDisabled()``, derefter ``OnReloaded()``, så vil den indlæse de nye assemblies, og så udfører den ``OnEnabled()``.

Bemærk, at jeg sagde *nye* assemblies. Hvis du erstatter en assembly med en anden af samme navn, vil den ***IKKE*** blive opdateret. Dette skyldes GAC (Global Assembly Cache), og hvis du forsøger at "indlæse" en assembly, der allerede er i cachen, vil den altid bruge den cachelagrede assembly i stedet.
Hvis dit plugin understøtter dynamiske opdateringer, skal du derfor bygge hver version med et andet Assembly Name i build-indstillingerne (omdøbning af filen virker ikke). Og da den gamle assembly ikke "ødelægges", når den ikke længere er nødvendig, vil den kode fortsætte med at køre sammen med den nye versions kode, hvis du undlader at afmelde dig fra events, unpatch din harmony-instans, dræbe coroutines osv.
Det er en meget, meget dårlig ide at lade det ske.

Derfor **SKAL** plugins, der understøtter dynamiske opdateringer, følge disse retningslinjer, ellers vil de blive fjernet fra Discord-serveren på grund af potentiel risiko for serverværterne.

Men ikke alle plugins behøver at understøtte dynamiske opdateringer. Hvis du ikke har tænkt dig at understøtte dynamiske opdateringer, er det helt fint, du skal bare ikke ændre Assembly Name på dit plugin, når du bygger en ny version, så behøver du ikke bekymre dig om noget af dette, bare sørg for, at serverværterne ved, at de bliver nødt til at genstarte deres servere fuldstændigt for at opdatere dit plugin.

**Translator: @misfiy**
