# EXILED - EXtended In-runtime Library for External Development

![EXILED CI](https://github.com/galaxy119/EXILED/workflows/EXILED%20CI/badge.svg?branch=2.0.0)
<a href="https://github.com/Exiled-Team/EXILED/releases">
  <img src="https://img.shields.io/github/release/Exiled-Team/EXILED/all.svg?style=flat" alt="GitHub Releases">
</a>
![Github All Downloads](https://img.shields.io/github/downloads/Exiled-Team/EXILED/total.svg?style=flat)
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/dev)
<a href="https://discord.gg/PyUkWTg">
  <img src="https://img.shields.io/discord/656673194693885975?logo=discord" alt="Chat on discord">
</a>


EXILED est un framework pour des plug-ins de bas niveau pour les servers SCP : Secret Laboratory. Il offre un système d'événements auquel les développeurs peuvent attacher leurs codes afin de manipuler ou modifier le code du jeu, ou implémenter leurs propres fonctions.
Tous les événements d’EXILED sont codés avec Harmony, ce qui signifie qu'ils ne nécessitent aucune modification directe des bibliothèques  du server pour fonctionner, ce qui offre deux avantages uniques.

 – Premièrement, l'intégralité du code du framework est publiée et partagée, permettant aux développeurs de mieux comprendre *comment* cela fonctionne, ainsi que de proposer des ajouts ou modifications.
 - Deuxièmement, étant donné que tout le code lié au framework est fait en dehors des bibliothèques du server, des choses comme de petites mises à jour auront peu ou pas d'effet sur le framework. Cela le rendant plus compatible avec les futures mises à jour du jeu, ainsi que de faciliter les mises à jour lorsqu'il *est* nécessaire.

# READMEs traduit
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)

# Installation
L'installation d'EXILED peut sembler très compliquée contrairement à d'autres plateformes, mais il est en fait simple.
Comme mentionné précédemment, la majeure partie d'EXILED ne se trouve pas dans le fichier "Assembly-CSharp.dll" du server. Cependant, une modification à ce fichier est nécessaire pour *démarrer* EXILED avec le server, la version du fichier avec cette modification est incluse avec le programme d'installation.

Si vous choisissez d'utiliser le programme d'installation (et s'il est exécuté correctement). Alors, il s'occupera d'installer `Exiled.Loader`, `Exiled.Updater`, `Exiled.Permissions`, `Exiled.API` et `Exiled.Events`, et il s'assurera que votre server a le bon fichier Assembly-CSharp.dll installé.


# Windows
### Installation automatique ([plus d'info](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Note :** assurez-vous que vous êtes sur l'utilisateur qui exécute le server ou que vous disposez des privilèges d'administrateur avant d'exécuter le programme d'installation.

  - Télécharger **`Exiled.Installer-Win.exe` [ici](https://github.com/Exiled-Team/EXILED/releases)** (cliquer sur Assets puis sur l'installeur Windows)
  - placez-le dans votre dossier du server (téléchargez le server dédié si vous ne l'avez pas)
  - Double-cliquez sur **`Exiled.Installer.exe`** ou **[télécharger ce .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** et placez-le dans le dossier du server pour installer la dernière version préliminaire
  - Pour installer et obtenir des plug-ins, consultez la section [Installation des plug-ins](#installing-plugins) ci-dessous.
**Note :** si vous installez EXILED sur un server distant, assurez-vous d'exécuter le .exe sous le même utilisateur que celui qui exécute vos servers SCP:SL (ou un avec des privilèges d'administrateur)

### Manual installation
  - Téléchargez **`Exiled.tar.gz` [ici](https://github.com/Exiled-Team/EXILED/releases)**
  - Extractez son contenu avec [7Zip](https://www.7-zip.org/) ou [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Déplacez l'**``Assembly-CSharp.dll``** à **`(Votre_Dossier_server)\SCPSL_Data\Managed`** et remplacer le fichier.
  - Déplacez le dossier **``EXILED``** dans **`%appdata%`** *Note : Ce dossier doit aller dans``C:\Users\(Votre_Utilisateur)\AppData\Roaming``, et ***NON PAS*** ``C:\Users\(Votre_Utilisateur)\AppData\Roaming\SCP Secret Laboratory``, et **IL EST IMPÉRATIF** qu'il est dans (...)\AppData\Roaming, et non pas (...)\AppData\ !*
    - Pour Windows 10 :
      Écrivez `%appdata%` dans Cortana/l'icône de recherche, ou la barre de l'Explorateur Windows
    - Pour toute autre version de Windows :
      Pressez Win + R et tapez `%appdata%`

### Installation des plug-ins
Voilà, EXILED devrait maintenant être installé et actif au prochain démarrage de votre server. Notez qu'EXILED par lui-même ne fera presque rien, alors assurez-vous d'obtenir de nouveaux plug-ins sur notre **[server discord](https://discord.gg/PyUkWTg)**.
- Pour installer un plug-in, il suffit de :
  - Télécharger le plug-in de [*ça* page des versions](https://i.imgur.com/u34wgPD.jpg) (**ça DOIT être un fichier `.dll` !**)
  - Le déplacer dans : ``C:\Users\(Votre_Utilisateur)\AppData\Roaming\EXILED\Plugins`` (déplacez-vous vers ce dossier en appuyant sur Win + R, puis en tapant `%appdata%`)

# Linux
### Installation automatique ([plus d'info](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Note :** si vous installez EXILED sur un server distant, assurez-vous d'exécuter le programme d'installation sous le même utilisateur que celui qui exécute vos servers SCP:SL (ou root)

  - Téléchargez **`Exiled.Installer-Linux` [ici](https://github.com/Exiled-Team/EXILED/releases)** (cliquer sur Assets puis sur l'installeur Windows)
  - Installez-le soit en tapant **`./Exiled.Installer-Linux --path/path/to/server`** ou déplacez-le directement dans le dossier du server, accédez-y avec le terminal (`cd`) et tapez : **`./Exiled.Installer-Linux`**.
  - Si vous voulez la dernière version préliminaire, ajoutez simplement **`--préreleases`**. Example : **`./Exiled.Installer-Linux/home/scp/server--préreleases`**
  - Autre exemple, si vous placez `Exiled.Installer-Linux` dans votre dossier server : **`/home/scp/server/Exiled.Installer-Linux --préreleases`**
  - Pour installer et obtenir des plug-ins, consultez la section [Installation des plug-ins](#installing-plugins -1) ci-dessous.


### Installation manuelle
  - **Assurez-vous** que vous êtes connecté sur l'utilisateur qui exécute les servers SCP.
  - Téléchargez **`Exiled.tar.gz` [ici](https://github.com/Exiled-Team/EXILED/releases)** (SSH : clique droit et pour obtenir le lien `Exiled.tar.gz`, puis tapez  **`wget (Lien_Pour_Télécharger)`**)
  - Pour l'extraire dans votre dossier actuel, tapez **``tar -xzvf EXILED.tar.gz``**
  - Déplacez l'**``Assembly-CSharp.dll``** et déposez-le dans le dossier **``SCPSL_Data/Managed``** de votre server (SSH : **`mv Assembly-CSharp.dll (Chemin_Vers_Votre_server)/SCPSL_Data/Managed`**).
  - Déplacez le dossier **`EXILED`** vers **``~/.config``**. *Note : Ce dossier doit aller dans ``~/.config``, et ***NON PAS*** ``~/.config/SCP Secret Laboratory``* (SSH : **`mv EXILED ~/.config/`**)

### Installation des plug-ins
Voilà, EXILED devrait maintenant être installé et actif au prochain démarrage de votre server. Notez qu'EXILED par lui-même ne fera presque rien, alors assurez-vous d'obtenir de nouveaux plug-ins sur notre **[server discord](https://discord.gg/PyUkWTg)**
- Pour installer un plug-in, il suffit de :
  - Télécharger le plug-in de [*ça* page des versions](https://i.imgur.com/u34wgPD.jpg) (**ça DOIT être un fichier `.dll`!**)
  - Le déplacer vers : ``~/.config/EXILED/Plugins`` (si vous utilisez votre SSH en tant que root, recherchez le bon `.config` qui sera à l'intérieur `/home/(SCP_Server_Utilisateur)`)

# Configs
EXILED par lui-même offre quelques options de configuration.
Tous sont générés automatiquement au démarrage du server, ils sont situés dans les dossiers ``~/.config/EXILED/Configs/(PortServer)-config.yml`` (quant à Windows ce sont les dossiers ``%AppData%\EXILED\Configs\(PortServer)-config.yml``).

Les configurations des plug-ins ***NE SONT POINT*** dans le fichier ``config_gameplay.txt`` ! Mais, les configurations de plug-in sont définies dans les fichiers ``~/.config/EXILED/Configs/(PortServer)-config.yml`` (et pour Windows les fichiers ``%AppData%\EXILED\(PortServer)-config.yml``).
Cependant, certains plug-ins peuvent avoir leurs paramètres de configuration à d'autres emplacements, les locations présentées ci-dessus sont celles par défaut des plug-ins EXILED, en cas de problèmes référez-vous au plug-in.

# Pour Les Devloppeurs

Si vous souhaitez créer un Plugin pour EXILED, et obtenir un rapide aperçu, veuillez visiter notre page [Premiers pas.](https://github.com/galaxy119/EXILED/blob/master/GettingStarted.md).

Pour des tutoriels plus complets, allez voir [le site web EXILED](https://exiled-team.github.io/EXILED/articles/install.html).

Assurez-vous de suivre ces règles avant de publier un plug-in :

 - Votre plug-in doit contenir une classe qui hérite de Exiled.API.Features.Plugin<>, si ce n'est pas le cas, EXILED ne chargera pas votre plug-in au démarrage du server.
 - Lorsqu'un plug-in est chargé le code de la méthode ``OnEnabled()`` de la classe susmentionnée ci-dessus est déclenchée immédiatement, il n'attend pas que d'autres plug-ins soient chargés. Ni la fin du processus de démarrage du server. ***Il n’attend rien.*** Lors de l'écriture de votre méthode OnEnable(), assurez-vous que vous n'accédez pas à des éléments qui ne sont peut-être pas encore initialisés par le server, tels que ServerConsole.Port ou PlayerManager.localPlayer.
 - Si vous avez besoin d'accéder très tôt à des éléments qui ne sont pas initialisés avant le chargement de votre plug-in, il est recommandé d'user l'événement WaitingForPlayers, si pour une raison quelconque vous avez besoin de faire les choses plus tôt, enveloppez le code dans un ```while(!x)``` qui vérifie que la variable/l'objet dont vous avez besoin n'est plus nul avant de continuer.
 - EXILED prend en charge le rechargement dynamique des plug-ins en cours d'exécution. Cela signifie que, si vous devez mettre à jour un plug-in, cela peut être fait sans redémarrer le server. Cependant, si vous mettez à jour un plug-in en cours d'exécution, le plug-in doit être correctement conçu pour le prendre en charge ! Reportez-vous à la section ``Mise à jour dynamique`` pour plus d'informations, ainsi  que les directives à suivre.
 - Il n'y a ***AUCUN*** événement OnUpdate, OnFixedUpdate ou OnLateUpdate dans EXILED. Si vous devez, pour une raison quelconque, exécuter du code à chaque frame, vous pouvez utiliser une coroutine MEC qui attend la prochaine frame (0.01f) ou une couche de synchronisation comme Timing.FixedUpdate.

### Désactivation des correctifs d'événement EXILED
***Cette fonctionnalité n'est actuellement plus implémentée.***

### Coroutines MEC
Si vous n'êtes pas familier avec MEC, ce sera une introduction très brève et simple pour vous aider à démarrer.
Les coroutines MEC sont essentiellement des méthodes « timées », qui prennent en charge des périodes d'attente avant de poursuivre l'exécution, sans interrompre le thread principal du jeu.
Les coroutines MEC peuvent être utilisées en toute sécurité avec Unity, contrairement au threading traditionnel. ***N'essayez PAS de créer de nouveaux threads pour interagir avec Unity, ils planteraient le server.***

Pour utiliser MEC, vous devrez référencer ``Assembly-CSharp-firstpass.dll`` (présent dans bibliothèque du server) et inclure ``using MEC;``.
Exemple d'appel d'une coroutine simple, qui se répète avec un délai entre chaque itération :
```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //répéter ce qui suit à l'infini
    {
        Log.Info("Hey, je suis une boucle infinie !"); //Appel Log.Info pour renvoyer une ligne dans la console de jeu/du server.
        yield return Timing.WaitForSeconds(5f); //indique à la coroutine d'attendre 5 secondes avant de continuer la boucle.
    }
}
```

Il est ***fortement*** recommandé de chercher sur Google ou de demander sur Discord si vous n'êtes pas familier avec MEC et que vous souhaitez en savoir plus, obtenir des conseils ou recevoir de l'aide. Les questions, aussi « stupides » soient-elles, recevront toujours une réponse aussi utile et claire que possible. Un meilleur code est meilleur pour tout le monde.

### Mises à jour dynamiques
EXILED en tant que framework prend en charge le rechargement dynamique des assemblys de plug-ins sans nécessiter de redémarrage du server.
Par exemple, si vous démarrez le server avec uniquement plug-in `Exiled.Events` et que vous souhaitez en ajouter un nouveau, vous n'avez pas besoin de redémarrer le server. Pour cella, vous pouvez simplement utiliser la commande RemoteAdmin/ServerConsole `reload plug-ins` pour recharger tous les plug-ins EXILED, y compris les nouveaux qui n'ont pas été chargés auparavant.

Cela signifie également que vous pouvez *mettre à jour* les plug-ins sans avoir à redémarrer complètement le server. Cependant, il y a quelques directives qui doivent être suivies par le développeur du plug-in pour que cela soit réalisé correctement :

***Pour les hôtes***
 - Si vous mettez à jour un plug-in, assurez-vous que son nom d'assembly n'est pas le même que la version actuelle que vous avez installée. Le plug-in doit avoir été conçu par le développeur avec l'idée d'une mise à jour dynamique pour que cela fonctionne. Renommer le fichier ne suffira pas !
 - Si le plug-in prend en charge les mises à jour dynamiques, assurez-vous que lorsque vous placez la version la plus récente du plug-in dans le dossier « Plugins », vous supprimez également l'ancienne version du dossier, avant de recharger EXILED, faute de quoi cela entraînera de nombreux problèmes.
 - Tout problème résultant de la mise à jour dynamique d'un plug-in relève uniquement de la responsabilité de vous et du développeur du plug-in en question. Bien qu'EXILED prenne pleinement en charge et encourage les mises à jour dynamiques, la seule façon dont cela pourrait échouer ou mal tourner est que l'hôte du server ou le développeur du plug-in a fait quelque chose qu'il n'aurait pas dû. Vérifiez trois fois que tout a été fait correctement avant de signaler un bug aux développeurs EXILED concernant les mises à jour dynamiques.

 ***Pour les développeurs***

 - Les plug-ins qui souhaitent prendre en charge la mise à jour dynamique doivent s'assurer de se désinscrire de tous les événements auxquels ils sont connectés lorsqu'ils sont désactivés ou rechargés.
 - Les plug-ins qui ont des « patchs » Harmony doivent stoker l'instance Harmony et appeler UnPatchAll() de l'instance lorsque le plug-in est désactivé ou rechargé.
- Toutes les coroutines démarrées par le plug-in dans OnEnabled doivent également être arrêtées lorsque le plug-in est désactivé ou rechargé.

Tout cela peut être réalisé dans les méthodes OnReloaded() ou OnDisabled() dans la classe plug-in. Quand EXILED recharge les plug-ins, il appelle OnDisabled(), puis OnReloaded() et il finit par charger les nouvelles assemblys et exécute OnEnabled().

Note : il est bien question de *nouvelles* assemblys. Si vous remplacez une assembly par un autre du même nom, elle ne sera ***PAS*** mise à jour. Cela est dû au GAC (Global Assembly Cache), si vous essayez de « charger » une assembly qui est déjà dans le cache, il utilisera toujours l'assembly mise en cache.
Pour cette raison, si votre plug-in prend en charge les mises à jour dynamiques, vous devez construire chaque version avec un nom d'assemblage différent dans les options de construction (renommer le fichier ne fonctionnera pas). De plus, puisque l'ancienne assembly n'est pas « détruit » lorsqu'elle est plus nécessaire, si vous ne parvenez pas à vous désabonner des événements, à dépatcher votre instance d'harmonie, à arrêter les coroutines..., ce code continuera à s'exécuter avec le code de la nouvelle version.

En tant que tels, les plug-ins qui prennent en charge les mises à jour dynamiques ***DOIVENT*** suivre ces directives ou ils seront supprimés du server discord en raison du risque pour les hôtes du server.

Mais tous les plug-ins ne doivent pas prendre en charge les mises à jour dynamiques. Si vous n'avez pas l'intention de prendre en charge les mises à jour dynamiques, ne changez pas le nom d'assembly de votre plug-in lorsque vous créez une nouvelle version, assurez-vous simplement que les hôtes de server savent qu'ils devront redémarrer complètement leurs servers pour mettre à jour votre plug-in.

Traduction par Warquys
