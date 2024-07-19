<div align="center">
  <img src="../assets/logo.svg" alt="Logo" width="96px" />
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
<br />

EXILED è un framework di alto livello per i server di SCP: Secret Laboratory. Offre un sistema di eventi per gli sviluppatori per modificare il codice di gioco o implementare le proprie funzioni.
Tutti gli eventi di EXILED sono scritti con Harmony, il che significa che non è necessaria alcuna modifica diretta agli assembly del server per farli funzionare, il che offre due vantaggi unici.

  - In primo luogo, l'intero codice del framework può essere liberamente pubblicato e condiviso, consentendo agli sviluppatori di capire meglio come funziona e di offrire suggerimenti per aggiungere o modificare le sue funzionalità.
  - In secondo luogo, poiché tutto il codice relativo al framework è realizzato al di fuori degli assembly del server, cose come piccoli aggiornamenti del gioco avranno scarso o nessun effetto sul framework. Questo lo rende molto probabilmente compatibile con i futuri aggiornamenti del gioco, nonché più facile da aggiornare quando è necessario farlo.

# README localizzati
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)
- [Türkçe](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-TR.md)
- [German](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DE.md)
- [Français](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-FR.md)
- [한국어](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-KR.md)
- [ไทย](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ไทย.md)

# Installation
L'installazione di EXILED è piuttosto semplice. Si carica tramite l'API del plugin NW. Per questo motivo, ci sono due cartelle all'interno del file  ``Exiled.tar.gz `` nei file di rilascio.  ``SCP Secret Laboratory `` contiene i file necessari per caricare le funzionalità di EXILED nella cartella  ``EXILED ``. Detto questo, tutto ciò che devi fare è spostare queste due cartelle nel percorso appropriato, che viene spiegato di seguito, ed è fatto!

# Windows
### Installazione automatica ([maggiori informazioni](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Nota**: Assicurati di essere connesso con l'utente che esegue il server o di avere i privilegi di amministratore prima di eseguire l'Installer.

  - Scarica **`Exiled.Installer-Win.exe` [da qui](https://github.com/Exiled-Team/EXILED/releases)** (clicca su Assets -> clicca sull'Installer)
  - Mettilo nella cartella del tuo server (scarica il server dedicato se non l'hai già fatto).
  - Fai doppio clic su **`Exiled.Installer.exe`** o **[scarica questo file .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** e mettilo nella cartella del server per installare l'ultima versione preliminare.
  - Per installare e ottenere plugin, consulta la sezione [Installare plugins](#installazione-plugins) qui sotto.

**Nota**: Se stai installando EXILED su un server remoto, assicurati di eseguire il file .exe come lo stesso utente che avvia i tuoi server SCP:SL (o un utente con privilegi di amministratore).

### Installazione manuale
  - Scarica **`Exiled.tar.gz` [da qui](https://github.com/Exiled-Team/EXILED/releases)**
  - Estrai i suoi contenuti con [7Zip](https://www.7-zip.org/) o [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Sposta la cartella **``EXILED``** su **`%appdata%`** *Nota: Questa cartella deve essere posizionata su ``C:\Users\%UserName%\AppData\Roaming``, e ***NON*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, e  **DEVE** essere su (...)\AppData\Roaming, non (...)\AppData\!*
  - Sposta **``SCP Secret Laboratory``** su **`%appdata%`**.
    - Windows 10 & 11:
      Scrivi `%appdata%` su Cortana / l'icona della ricerca o nella barra di esplorazione di Windows.
    - Qualsiasi altra versione di Windows:
      Premi Win + R e scrivi  `%appdata%`

### Installazione plugins
Con questo, EXILED dovrebbe ora essere installato e attivo la prossima volta che avvierai il tuo server. Tieni presente che EXILED da solo farà quasi nulla, quindi assicurati di ottenere nuovi plugin dal nostro **[server Discord](https://discord.gg/exiledreboot)**
- Per installare un plugin, semplicemente:
  - Scarica un plugin dalla [*loro* releases page](https://i.imgur.com/u34wgPD.jpg) (**DEVE essere un `.dll`!**)
  - Spostalo su: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins`` (sposta qui premendo Win + R, quindi scrivendo `%appdata%`)

# Linux
### Installazione automatica ([maggiori informazioni](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Nota:** Se stai installando EXILED su un server remoto, assicurati di eseguire l'Installer come lo stesso utente che avvia i tuoi server SCP:SL (o con privilegi di root).

  - Scarica **`Exiled.Installer-Linux` [da qui](https://github.com/Exiled-Team/EXILED/releases)** (clicca su Assets -> scarica l'Installer)
  - Installalo digitando **`./Exiled.Installer-Linux --path /path/to/server`** o spostalo direttamente nella cartella del server, muoviti nella cartella con il terminale (`cd`) e digita: **`./Exiled.Installer-Linux`**.
  - Se desideri la versione preliminare più recente, aggiungi semplicemente **`--pre-releases`**. Esempio: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Un altro esempio, se hai inserito `Exiled.Installer-Linux` nella cartella del tuo server: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - Per installare ed scaricare plugins, dirigiti sulla sezione [Installare plugins](#installazione-plugins-1) qui sotto.

### Installazione manuale
  - **Assicurati** di essere connesso con l'utente che avvia i server SCP.
  - Scarica **`Exiled.tar.gz` [da qui](https://github.com/Exiled-Team/EXILED/releases)** (SSH: fai clic con il tasto destro e ottieni il link per `Exiled.tar.gz`, di conseguenza digita: **`wget (link_per_il_download)`**)
  - Per estrarlo nella tua cartella corrente, digita **``tar -xzvf EXILED.tar.gz``**
  - Sposta la cartella **`EXILED`** su **``~/.config``**. *Nota: Questa cartella deve essere posizionata su ``~/.config``, e ***NON*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Sposta la cartella **`SCP Secret Laboratory`** su **``~/.config``**. *Nota: Questa cartella andare su ``~/.config``, e ***NON*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Installazione plugins
Con questo, EXILED dovrebbe ora essere installato e attivo la prossima volta che avvierai il tuo server. Tieni presente che EXILED da solo farà quasi nulla, quindi assicurati di ottenere nuovi plugin dal nostro **[server Discord](https://discord.gg/exiledreboot)**
- Per installare un plugin, semplicemente:
  - Scarica un plugin dalla [*loro* releases page](https://i.imgur.com/u34wgPD.jpg) (**DEVE essere un `.dll`!**)
  - Spostalo su: ``~/.config/EXILED/Plugins`` (se utilizzi SSH come root, cerca il file .config corretto che si troverà su `/home/(SCP Server User)`)

## Configurazione
EXILED offre alcune opzioni di configurazione di base.
Tutte queste sono generate automaticamente all'avvio del server e si trovano nel file ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` (``%AppData%\EXILED\Configs\(ServerPortHere)-config.yml`` su Windows).

Le configurazioni dei plugin ***NON*** si trovano nel file "config_gameplay.txt" menzionato sopra, ma le configurazioni dei plugin vengono impostate nel file ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` (``%AppData%\EXILED\(ServerPortHere)-config.yml`` su Windows).
Tuttavia, alcuni plugin possono ottenere le loro impostazioni di configurazione da altre posizioni, quindi questa è semplicemente la posizione predefinita di EXILED per loro; quindi fai riferimento al singolo plugin se ci sono problemi.

# Per gli Sviluppatori

Se desideri creare un Plugin per EXILED, è piuttosto semplice farlo. Se vuoi un tutorial più dettagliato, visita la nostra [Pagina di Inizio](https://exiled.to/Archive/GettingStarted).

Per tutorial più completi e costantemente aggiornati, consulta [il sito web di EXILED](https://exiled.to).

Ma assicurati di seguire queste regole quando pubblichi i tuoi plugin:

- Il tuo plugin deve contenere una classe che eredita da ``Exiled.API.Features.Plugin<>``; se non lo fa, EXILED non caricherà il tuo plugin quando il server si avvia.
- Quando un plugin viene caricato, il codice all'interno del metodo ``OnEnabled()`` della classe menzionata sopra viene eseguito immediatamente, non attende che altri plugin vengano caricati. Non attende nemmeno che il processo di avvio del server sia completato. ***Non attende nulla.*** Quando imposti il tuo metodo ``OnEnabled()``, assicurati di non accedere a elementi che potrebbero non essere ancora stati inizializzati dal server, come ``ServerConsole.Port``, o ``PlayerManager.localPlayer``.
- Se hai bisogno di accedere a elementi prima che il tuo plugin venga caricato, è consigliabile attendere l'evento ``WaitingForPlayers`` per farlo; se per qualche motivo hai bisogno di farlo prima, avvolgi il codice in un ciclo ``` while(!x)``` che verifica che la variabile/oggetto che stai cercando di ottenere non sia più nullo prima di continuare.
- EXILED supporta il ricaricamento dinamico degli assembly dei plugin durante l'esecuzione. Ciò significa che se devi aggiornare un plugin, puoi farlo senza dover riavviare il server; tuttavia, se stai aggiornando un plugin durante l'esecuzione, il plugin deve essere configurato correttamente per supportare questa funzionalità, altrimenti potresti avere problemi. Consulta la sezione "Dynamic Updates" per ulteriori informazioni e linee guida da seguire.
- Non esistono eventi come OnUpdate, OnFixedUpdate o OnLateUpdate in EXILED. Se per qualche motivo hai bisogno di eseguire codice in modo frequente, puoi utilizzare una coroutine MEC che attende un frame, 0.01f, o utilizzare un livello di sincronizzazione come Timing.FixedUpdate.



### Disattivazione delle patch degli eventi di EXILED
***Questa funzionalità non è attualmente implementata.***

 ### MEC Coroutines
Se non sei familiare con le MEC, questa sarà una breve e semplice introduzione per aiutarti a iniziare.
Le coroutine MEC sono essenzialmente metodi temporizzati che supportano periodi di attesa prima di continuare l'esecuzione, senza interrompere o sospendere il thread principale del gioco.
Le coroutine MEC sono sicure da utilizzare con Unity, a differenza della programmazione tradizionale con i thread. ***NON cercare di creare nuovi thread per interagire con Unity, FARANNO crashare il server.***

Per utilizzare le MEC, dovrai fare riferimento a ``Assembly-CSharp-firstpass.dll`` dai file del server e includere ``using MEC;``.
Ecco un esempio di come chiamare una semplice coroutine che si ripete con un ritardo tra ogni ciclo:
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) // ripeti quanto segue all'infinito
    {
        Log.Info("Ehi, sono un loop infinito!"); // Chiama Log.Info per stampare una riga sulla console di gioco/server.
        yield return Timing.WaitForSeconds(5f); // Dice alla coroutine di attendere 5 secondi prima di continuare; poiché questa istruzione è alla fine del ciclo, essa effettivamente sospende il ciclo per 5 secondi prima di ripetere.
    }
}
```

È ***fortemente***  consigliato fare qualche ricerca o chiedere nel Discord se non sei familiare con le MEC e desideri saperne di più, ottenere consigli o assistenza. Le domande, indipendentemente da quanto possano sembrare "stupide", saranno sempre risposte nel modo più utile e chiaro possibile per gli sviluppatori di plugin. Un codice migliore è vantaggioso per tutti.

### Dynamic Updates
EXILED come framework supporta il ricaricamento dinamico degli assembly dei plugin senza richiedere un riavvio del server.
Ad esempio, se avvii il server solo con `Exiled.Events` come unico plugin e desideri aggiungerne uno nuovo, non è necessario riavviare il server per completare questa operazione. Puoi semplicemente utilizzare il comando RemoteAdmin/ServerConsole `reload plugins` per ricaricare tutti i plugin di EXILED, inclusi quelli nuovi che non erano stati caricati in precedenza.

Ciò significa anche che puoi *aggiornare* i plugin senza dover riavviare completamente il server. Tuttavia, ci sono alcune linee guida che il developer del plugin deve seguire affinché ciò avvenga correttamente:

***Per gli Host***
 - Se stai aggiornando un plugin, assicurati che il nome dell'assembly non sia lo stesso della versione attuale che hai installato (se presente). Il plugin deve essere costruito dallo sviluppatore tenendo presente gli Aggiornamenti Dinamici, semplicemente rinominare il file non funzionerà.
 - Se il plugin supporta gli Aggiornamenti Dinamici, assicurati che quando inserisci la versione più recente del plugin nella cartella "Plugins", rimuovi anche la versione precedente dalla cartella, prima di ricaricare EXILED; se non lo fai, ciò potrebbe causare problemi gravi.
 - Qualsiasi problema che sorga dal Ricaricamento Dinamico di un plugin è di responsabilità tua e dello sviluppatore del plugin in questione. Sebbene EXILED supporti pienamente e promuova gli Aggiornamenti Dinamici, l'unico modo in cui potrebbero fallire o andare storti è se l'host del server o lo sviluppatore del plugin hanno commesso un errore. Verifica tre volte che tutto sia stato fatto correttamente da entrambe le parti prima di segnalare un bug agli sviluppatori di EXILED riguardo agli Aggiornamenti Dinamici.

 ***Per gli Sviluppatori***

 - I plugin che desiderano supportare gli Aggiornamenti Dinamici devono assicurarsi di annullare la registrazione da tutti gli eventi a cui sono collegati quando vengono Disabilitati o Ricaricati.
 - I plugin che hanno patch Harmony personalizzate devono utilizzare una variabile che cambia nel nome dell'istanza Harmony e devono chiamare ``UnPatchAll()`` sulla loro istanza Harmony quando il plugin viene disabilitato o ricaricato.
 - Qualsiasi coroutine avviata dal plugin in ``OnEnabled()`` deve anche essere terminata quando il plugin viene disabilitato o ricaricato.

Tutti questi punti possono essere realizzati nei metodi ``OnReloaded()`` o ``OnDisabled()`` nella classe del plugin. Quando EXILED ricarica i plugin, chiama ``OnDisabled()``, poi ``OnReloaded()``, quindi caricherà le nuove assembly e infine eseguirà ``OnEnabled()``.

Nota che ho detto *nuove* assembly. Se sostituisci un'assembly con un'altra con lo stesso nome, essa ***NON*** verrà aggiornata. Questo è dovuto alla GAC (Global Assembly Cache); se tenti di 'caricare' un'assembly che è già in cache, verrà sempre utilizzata l'assembly in cache. Per questo motivo, se il tuo plugin supporterà gli Aggiornamenti Dinamici, dovrai costruire ciascuna versione con un nome di assembly diverso nelle opzioni di compilazione (rinominare il file non funzionerà). Inoltre, poiché la vecchia assembly non viene "distrutta" quando non è più necessaria, se non annulli la registrazione dagli eventi, non rimuovi le patch Harmony, non termini le coroutine, ecc., quel codice continuerà a essere eseguito insieme al codice della nuova versione. Questo è un grave errore da evitare assolutamente.

Pertanto, i plugin che supportano gli Aggiornamenti Dinamici ***DEVONO*** seguire queste linee guida o verranno rimossi dal server Discord a causa del potenziale rischio per gli host del server.

Ma non tutti i plugin devono supportare gli Aggiornamenti Dinamici. Se non hai intenzione di supportare gli Aggiornamenti Dinamici, va benissimo, basta che non cambi il nome dell'Assembly del tuo plugin quando crei una nuova versione e non dovrai preoccuparti di nulla. Assicurati solo che gli host del server siano consapevoli che dovranno riavviare completamente i loro server per aggiornare il tuo plugin.
