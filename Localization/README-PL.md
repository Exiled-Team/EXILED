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

EXILED to wysoko poziomowy framework do tworzenia pluginów dla serwerów w grze SCP: Secret Laboratory. Oferuje on system zdarzeń, do którego programiści mogą podpinać swój kod w celu manipulacji bądź zmiany działania gry, lub implementowania własnych funkcji.
Wszystkie zdarzenia EXILED'a są zaprogromowane za pomocą Harmony, co oznacza że nie wymagają bezpośredniego modyfikowania serwerowych plików Assembly, co daje dwie wyjątkowe korzyści.

 - Po pierwsze, cały kod framework'a może być swobodnie publikowany i udostępniany co pozwala programistom lepiej zrozumieć *jak* on działa, oraz dawać sugestie dotyczące dodawania i zmieniania jego funkcji.
 - Po drugie, ponieważ cały kod związany z framework'iem jest wykonywany poza plikami Assembly serwera, rzeczy takie jak małe aktualizacje gry będą miały niewielki (jeżeli jakikolwiek) wpływ na działanie framework'a, sprawiając że najprawdopodobniej będzie on kompatybilny z przyszłymi aktualizacjami gry oraz łatwiejszy do aktualizacji gdy *faktycznie* jest to potrzebne.

# Przetłumaczone README
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)

# Instalacja
Instalacja EXILED'a może wydawać się bardziej skomplikowana i wymagająca niż innych framework'ów ale tak naprawdę jest to całkiem proste.
Jak wymieniono wyżej, większość EXILED'a jest zawarta poza plikiem Asembly-CSharp.dll serwera, aczkolwiek istnieja jedna ważna zmiana w tym pliku wymagana aby faktycznie *załadować* EXILED'a na serwer podczas uruchamiania, czysta wersja pliku Assembly z tą zmianą już wprowadzoną będzie dostarczona razem z kolejnymi wydaniami framework'a.

Jeżeli zdecydujecie się na skorzystanie z instalatora, ten - jeżeli użyty poprawnie, zajmie się instalacją EXILED'a ze wszystkimi jego funkcjami.

# Windows
### Automatyczna instalacja ([więcej informacji](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**UWAGA**: Przed uruchomieniem instalatora upewnijcie się, że jesteście zalogowani jako użytkownik, który będzie uruchamiał serwer albo ma uprawnienia Administratora.

  - Pobierzcie **`Exiled.Installer-Win.exe` kilkając [tutaj](https://github.com/Exiled-Team/EXILED/releases)** (Naciśnijcie na zasoby (Assets) -> naciśnijcie na instalatora)
  - Umieśćcie go w folderze waszego serwera (pobierzcie serwer dedykowany jeżeli jeszcze tego nie zrobiliście)
  - Naciśnijcie 2 razy na **`Exiled.Installer.exe`** albo **[pobierzcie ten plik .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** i umieśćcie go w folderze waszego serwera aby zainstalować najnowsze wydanie 'beta'.
  - Aby zdobyć i zainstalować pluginy, sprawdźcie sekcję [Instalowanie pluginów](#installing-plugins) poniżej.
**UWAGA:** Jeżeli instalujecie EXILED'a na serwerze, upewnijcie się, że uruchamiacie plik .exe jako ten sam użytkownik, który będzie uruchamiał serwer (albo ma uprawnienia Administratora)

### Ręczna instalacja
  - Pobierzcie **`Exiled.tar.gz` klikając [tutaj](https://github.com/Exiled-Team/EXILED/releases)**
  - Wypakujcie jego zawartośc za pomocą [7Zip'a](https://www.7-zip.org/) albo [WinRar'a](https://www.win-rar.com/download.html?&L=6)
  - Przenieście folder **``EXILED``** do **`%appdata%`** *UWAGA: Folder EXILED musi znaleźć się w ścieżce ``C:\Users\%UserName%\AppData\Roaming``, ***A NIE*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, i **MUSI** być w (...)\AppData\Roaming, a nie (...)\AppData\!*
    - Windows 10:
      Wpiszcie `%appdata%` w Cortanie / ikonce wyszukiwania, albo w Eksploratorze plików Windows
    - Jakakolwiek inna wersja windows:
      Naciśnijcie kombinację klawiszy Win + R i wpiszcie `%appdata%`

### Instalowanie pluginów
To tyle, EXILED powinien być juz zainstalowany i aktywowany podczas następnego uruchomienia serwera. Pamiętajcie, że sam EXILED nie będzie robił prawie nic, aby zdobyć pluginy udajcie się na **[nasz serwer Discord](https://discord.gg/exiledreboot)**
- Aby zainstalować plugin wystarczy:
  - Pobrać go z [*jego* strony wydań](https://i.imgur.com/u34wgPD.jpg) (**MUSI być to plik `.dll`!**)
  - Przenieść go do folderu: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins`` (aby się tutaj dostać wystarczy wcisnąć Win + R i wpisać `%appdata%`)

# Linux
### Automatyczna instalacja ([więcej informacji](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**UWAGA:** Jeżeli instalujecie EXILED'a na serwerze zdalnym, upewnijcie się, że uruchamiacie instalator jako ten sam użytkownik, który będzie uruchamiał serwer (albo jako root).

  - Pobierzcie **`Exiled.Installer-Linux` klikając [tutaj](https://github.com/Exiled-Team/EXILED/releases)** (naciśnijcie na zasoby (Assets) -> naciśnijcie na instalator)
  - Zainstulujcie go wpisując **`./Exiled.Installer-Linux --path /path/to/server`** albo przenieście go bezpośrednio do folderu serwera, przejdźcie do niego za pomocą terminala (komenda `cd`) i wpiszcie: **`./Exiled.Installer-Linux`**.
  - Jeżeli chcecie zainstalować najnowsze wydanie 'beta', po prostu dodajcie **`--pre-releases`** na końcu komendy. Na przykład: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Kolejny przykład, dla tych którzy umieścili `Exiled.Installer-Linux` w swoim folderze serwera: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - Aby zdobyć i zainstalować pluginy sprawdźcie sekcję [Instalowanie pluginów](#installing-plugins-1) poniżej.

### Ręczna instalacja
  - **Upewnijcie się**, że jesteście zalogowani jako ten sam użytkownik, który będzie uruchamiał serwer.
  - Pobierzcie **`Exiled.tar.gz` kilkając [tutaj](https://github.com/Exiled-Team/EXILED/releases)** (SSH: naciśnijcie prawym przyciskiem myszy na `Exiled.tar.gz` aby skopiować link, a następnie wpiszcie: **`wget (link_do_pobrania)`**)
  - Aby wypakować pliki do obecnego folderu **``tar -xzvf EXILED.tar.gz``**
  - Przenieście folder **`EXILED`** do ścieżki **``~/.config``**. *UWAGA: Folder EXILED musi znaleźć się w folderze ``~/.config``, ***A NIE*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Przenieście folder **`SCP Secret Laboratory`** do ścieżki **``~/.config``**. *UWAGA: Folder musi znaleźć się w ``~/.config``, **A *NIE*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Instalowanie pluginów
To tyle, EXILED powinien być juz zainstalowany i aktywowany podczas następnego uruchomienia serwera. Pamiętajcie, że sam EXILED nie będzie robił prawie nic, aby zdobyć nowe pluginy udajcie się na **[nasz serwer Discord](https://discord.gg/exiledreboot)**
- Aby zainstalować plugin wystarczy:
  - Pobrać go z [*jego* strony wydań](https://i.imgur.com/u34wgPD.jpg) (**MUSI być to plik `.dll`!**)
  - Przenieść go do folderu: ``~/.config/EXILED/Plugins`` (jeżeli używacie SSH jako root, musicie znaleźć poprawny folder `.config`, który będzie znajdować się pod ścieżką `/home/(użytkownik_uruchamiający_serwer)`)

# Config
Sam EXILED oferuje kilka opcji configu.
Wszystkie configi są automatycznie generowane podczas uruchamiania serwera, znajdują się w pliku ``~/.config/EXILED/Configs/(port_serwera_tutaj)-config.yml`` (``%AppData%\EXILED\Configs\(port_serwera_tutaj)-config.yml`` na Windowsie).

Configi pluginów ***NIE BĘDĄ*** znajdować się w pliku ``config_gameplay.txt``, zamiast tego configi pluginów można znaleźć pod ścieżką ``~/.config/EXILED/Configs/(port_serwera_tutaj)-config.yml`` (``%AppData%\EXILED\(port_serwera_tutaj)-config.yml`` na Windowsie).
Niektóre pluginy mogą brać swoje configi z innych miejsc, wyżej wymieniona lokalizacja to domyślne miejsce przechowywania configów pluginów. W razie jakichkolwiek problemów proszę zwracać się do poszczególnych pluginów.

# Dla programistów

Tworzenie pluginów za pomocą EXILED'a jest całkiem proste. Jeżeli chcecie dowiedzieć się więcej na ten temat odwiedźcie naszą stronę [dla początkujących](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md).

Aby uzyskać dokładniejsze i bardziej aktualne poradniki odwiedźcie [stronę EXILED'a](https://exiled.to).

Podczas publikowania swoich pluginów pamiętajcie o tych zasadach:

 - Wasz plugin musi zawierać klasę która dziedziczy od ``Exiled.API.Features.Plugin<>``, jeżeli tak nie jest EXILED nie załaduje waszego pluginu podczas startu serwera!
 - Gdy plugin jest załadowany kod zawarty w metodzie ``OnEnabled()`` nie czeka aż pozostałe pluginy zostaną załadowane, nie czeka aż proces uruchamiania serwera się zakończy. ***Nie czeka na nic.*** Podczas konfigurowania waszej metody ``OnEnabled()``, upewnijcie się że ***nie*** próbujecie korzystać z rzeczy, które nie zostały jeszcze zainicjalizowane przez serwer, takich jak: ``ServerConsole.Port``, albo ``PlayerManager.localPlayer``.
 - Jeżeli musicie wcześnie skorzystać z rzeczy, które nie zostały jeszcze zainicjalizowane przez serwer, zalecane jest aby poczekać na zdarzenie ``WaitingForPlayers``, jeżeli z jakiegoś powodu musicie zrobić coś jeszcze wcześniej, po prostu owińcie wasz kod w pętli ``` while(!x)``` która czeka aż objekt/zmienna której potrzebujecie nie będzie miała już wartości null, a potem kontynuuje.
 - EXILED wspiera dynamiczne ładowanie assembly pluginów podczas egzekucji kodu. To oznacza, że jeżeli musicie zaktualizować plugin, może się to wydarzyć bez restartowania serwera, jednakże żeby zaktualizować plugin podczas egzekucji kodu, plugin musi być opdowiednio przygotowany aby wspierać takie działanie. Odnoście się do sekcji ``Dynamiczne aktualizacje`` po więcej informacji i wytycznych.
 - ***NIE MA*** zdarzeń takich jak: OnUpdate, OnFixedUpdate albo OnLateUpdate zawartych w EXILED'zie. Jeżeli z jakiegoś powodu musicie wykonywać kod *aż tak* często, możecie skorzystać z korutyny MEC, która czeka jedną klatkę, 0.01f, albo używa warstwy Timing takiej jak Timing.FixedUpdate.

### Wyłączanie patche'ów zdarzeń EXILED'a
***Ta funkcja już nie istnieje.***

 ### Korutyny MEC
Jeżeli nie jesteście zaznajomienie z MEC'em, poniższe wyjaśnienie posłuży wam jako podstawa.
Korutyny MEC to po prostu metody z licznikiem czasu, wspierają one czekanie konkretną ilość czasu przed kontynuowaniem egzekucji bez przerywania/usypiania głównego wątku gry.
Wątki MEC można używać bezpiecznie w połączeniu z Unity, w odróżnieniu od tradycyjnych wątków. ***NIE próbujcie tworzyć nowych wątków w celu interakcji z Unity, mają one 100% szansę na zcrashowanie serwera.***

Aby skorzystać z MEC'a będziecie musieli stworzyć odwołanie do pliku ``Assembly-CSharp-firstpass.dll`` znajdującego się w plikach serwera, oraz umieścić ``using MEC;`` na początku waszego pliku.
Przykład prostej korutyny, która powtarza się z opóźnieniem między każdą iteracja pętli.
```cs
using MEC;
using Exiled.API.Features;

public void JakasMetoda()
{
    Timing.RunCoroutine(MojaKorutyna());
}

public IEnumerator<float> MojaKorutyna()
{
    for (;;) //powtórz następujący kod w nieskończoność
    {
        Log.Info("Hej, jestem nieskończoną pętlą!"); //Ta część wzywa funkcję Log.Info(), aby wypisać tą samą, ustaloną linijkę do konsoli serwera/logów serwera
        yield return Timing.WaitForSeconds(5f); //Ta część mówi korutynie aby zaczekała 5 sekund przed wykonaniem reszty kodu, ponieważ znajduje się to na końcu pętli przeciąga to korutynę o 5 sekund.
    }
}
```

Jest ***mocno*** zalecane abyście wykonali trochę starego dobrego 'googlowania', albo popytali na naszym serwerze Discord jeżeli nie jesteście zaznajomieni z MEC'em i chcielibyście dowiedzieć się więcej, poszukać rad, albo potrzebujecie pomocy. Wszelkie pytania, nie ważne jak głupie, nigdy nie pozostaną bez pomocnej i jasnej odpowiedzi. Lepszy i bardziej czytelny kod ułatwia życie każdemu.

### Dynamiczne aktualizacje
EXILED wspiera dynamiczne aktualizowanie plików assembly pluginów bez wymagania restartu serwera.
Na przykład, jeżeli uruchomicie serwer z plikiem `Exiled.Events` jako jedynym pluginem, ale chielibyście dodać nowe, nie musicie restartować serwera aby to osiągnąć. Możecie po prostu użyć komendy Remote Admin'a/Konsoli serwera `reload plugins` aby ponownie załadować wszystkie pluginy EXILED'a, wliczając w to nowe, które nie były wcześniej obecne.

Oznacza to także, że możecie *aktualizować* pluginy bez restartowania serwera, jednakże istnieje kilka wytycznych, które ***MUSZĄ*** zostać spełnione aby zostało to poprawnie osiągnięte:

***Dla hostów***
 - Jeżeli aktualizujecie jakiś plugin, upewnijcie się, że nazwa jego assembly nie jest taka sama jak obecna zainstalowana wersja (jeżeli jakakolwiek). Plugin musi być stworzony przez programistę z zamysłem Dynamicznych Aktualizacji, aby to zadziałało. Zwykłe zmienianie nazwy pliku nic nie da.
 - Jeżeli plugin wspiera Dynamiczne aktualizacje, upewnijcie się, że gdy przenosicie nowszą wersję pluginu do folderu "Plugins", usuniecie także starszą wersję tegoż pluginu przed ponownym załadnowaniem EXILED'a. Nie usunięcie starszej wersji pliku spowoduje wiele, *wiele* nieprzyjemnych rzeczy.
 - Jakiekolwiek problemy powstałe w wyniku Dynamicznego aktualizowania pluginu są tylko i wyłącznie waszą winą, bądź winą programisty tworzącego dany plugin. EXILED wspiera i zachęca do korzystania z Dynamicznych Aktualizacji, jedyny sposób na stworzenie tutaj błędów to pomyłka hosta serwera bądź programisty danego pluginu. Sprawdźcie 3, 4, a nawet 5 razy, że wszystko zostało wykonane poprawnie przez programistę i hosta serwera zanim zgłosicie jakikolwiek błąd związany z Dynamicznymi aktualizacjami do deweloperów EXILED'a.

 ***Dla programistów***

 - Pluginy, które chcą wspierać Dynamiczne aktualizacje ***MUSZĄ*** odpiąć się od wszystkch zdarzeń, do których były wcześniej podpięte gdy są wyłączane albo ponownie załadowywane.
 - Pluginy, które korzystają z własnych patchy Harmony muszą zawierać jakąś zmieniającą się zmienną (masło maślane ale jednak) w swojej nazwie instancji Harmony, oraz muszą użyć metody UnPatchAll() na swojej instancji Harmony gdy plugin jest wyłączany bądź ponownie załadowywany.
 - Wszelkie korutyny rozpoczynające się w metodzie ``OnEnabled`` muszą być zabijane ~~zanim złożą jaja~~ gdy plugin jest wyłączany bądź ponownie załadowywany.

Wszystkie z powyższych mogą zostać osiągnięte w metodach ``OnReloaded()`` albo ``OnDisabled()`` w waszym głównym pliku Pluginu z klasy Plugin. Gdy EXILED załadowywuje plugin ponownie, najpierw wzywa metodę OnDisabled(), a potem ``OnReloaded()``, potem ładuje nowe assembly, a potem wykonuje metodę ``OnEnabled()``.

Zauważcie, że powiedziałem *nowe* assembly. Jeżeli zamienicie stare assembly na nowe o tej samej nazwie, to ***NIE*** zostanaie ono zaktualizowane. Jest tak z powodu GAC'a (Global Assembly Cache). Jeżeli spróbujecie 'załadować' assembly, które jest już zcachowane, zcachowane assembly zawsze będzie użyte zamiast nowego.
Z tego powodu, jeżeli wasz plugin chce wspierać Dynamiczne aktualizacje, musicie kompilować każdą wersję z inną nazwą assembly w opcjach builda (zmiana nazwy pliku nie zadziała). Oprócz tego, stare assembly nie jest "niszczone" gdy nie jest już potrzebne, z tego powodu jeżeli nie odepniecie się od wszystkich zdarzeń, nie odpatchujecie waszej instancji Harmony, nie zabijecie korutyn itp. kod starej wersji pluginu będzie wykonywany obok nowego. Jeżeli na to pozwolicie czekają was bardzo nieprzyjemne niespodzianki.

Z tych powodów pluginy wspierające Dynamiczne aktualizacje ***MUSZĄ*** spełniać te wytyczne, w przeciwnym wypadku zostaną usunięte z naszego serwera Discord ze względu na potencjalne ryzyko dla hostów serwerów.

Nie każdy plugin musi wpsierać Dynamiczne aktualizacje. Jeżeli nie chcecie tego robić, jest to w 100% okej, po prostu nie zmieniajcie nazwy waszego assembly podczas kompilowania nowych wersji, a nie będziecie musieli się tym martwić. Upewnijcie się, że serwer hości wiedzą o tym, że muszą zrestartować serwer za każdym razem gdy chcą zaktualizować wasz plugin.

Polskie tłumaczenie stworzone przez: Mikihero.
