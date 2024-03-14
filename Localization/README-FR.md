<h1 align="center">EXILED - Bibliothèque d'exécution étendue pour le développement externe</h1>
<div align="center">
    
[<img src="https://img.shields.io/github/actions/workflow/status/Exiled-Team/EXILED/main.yml?style=for-the-badge&logo=githubactions&label=build" alt="CI"/>](https://github.com/Exiled-Team/EXILED/actions/workflows/main.yml/badge.svg?branch=master)
<a href="https://github.com/Exiled-Team/EXILED/releases"><img src="https://img.shields.io/github/v/release/Exiled-Team/EXILED?display_name=tag&style=for-the-badge&logo=gitbook&label=Release" href="https://github.com/Exiled-Team/EXILED/releases" alt="GitHub Releases"></a>
<img src="https://img.shields.io/github/downloads/Exiled-Team/EXILED/total?style=for-the-badge&logo=github" alt="Downloads">
![Github Commits](https://img.shields.io/github/commit-activity/w/Exiled-Team/EXILED/apis-rework?style=for-the-badge&logo=git)
<a href="https://discord.gg/PyUkWTg">
    <img src="https://img.shields.io/discord/656673194693885975?style=for-the-badge&logo=discord" alt="Chat on Discord">
</a>    

</div>

EXILED est un framework de plugins de haut niveau pour les serveurs SCP: Secret Laboratory. Il offre un système d'événements aux développeurs afin de manipuler ou de modifier le code du jeu ou de mettre en œuvre leurs propres fonctions. Tous les événements EXILED sont codés avec Harmony, ce qui signifie qu'ils n'ont pas besoin d'être directement modifiés dans les "server assemblies" pour fonctionner, ce qui permet deux avantages uniques.


 - Premièrement, l'ensemble du code du framework peut être librement publié et partagé, permettant aux développeurs de mieux comprendre son fonctionnement et d'offrir des suggestions pour ajouter ou modifier ses fonctionnalités.
 - Deuxièmement, puisque tout le code lié au framework est réalisé en dehors du "server assemblies", des mises à jour mineures du jeu auront peu, voire aucun, effet sur le framework. Cela le rend très probablement compatible avec les futures mises à jour du jeu, ainsi que plus facile à mettre à jour lorsque cela est nécessaire.

# Localized READMEs
- [Русский](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-Русский.md)
- [中文](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-中文.md)
- [Español](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-ES.md)
- [Polski](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-PL.md)
- [Português-BR](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-BR.md)
- [Italiano](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-IT.md)
- [Čeština](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-CS.md)
- [Dansk](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DK.md)
- [Türkçe](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-TR.md)
- [German](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-DE.md)
- [Français](https://github.com/Exiled-Team/EXILED/blob/master/Localization/README-FR.md)

# Installation
L'installation d'EXILED est assez simple. Il se charge via l'API de plugin de Northwood. C'est pourquoi il y a deux dossiers à l'intérieur du fichier ``Exiled.tar.gz`` dans les fichiers de release. ``SCP Secret Laboratory`` contient les fichiers nécessaires pour charger les fonctionnalités EXILED dans le dossier ``EXILED`` . Tout ce que vous avez à faire est de déplacer ces deux dossiers dans le chemin approprié, qui est expliqué ci-dessous, et vous avez terminé !

Si vous choisissez d'utiliser l'installateur, il se chargera, s'il est exécuté correctement, d'installer toutes les fonctionnalités d'EXILED.

# Windows
### Installation automatique ([plus d'informations](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))
**Note**: Assurez-vous d'être connecté en tant qu'utilisateur exécutant le serveur, ou que vous disposez de privilèges Admin avant d'exécuter l'installateur.

  - Téléchargez **`Exiled.Installer-Win.exe` [ici](https://github.com/Exiled-Team/EXILED/releases)** (cliquez sur "Assets" -> cliquez sur "Installer")
  - Placez-le dans le dossier de votre serveur (téléchargez le serveur dédié si vous ne l'avez pas encore fait)
  - Double-cliquez sur **`Exiled.Installer.exe`** ou **[téléchargez ce .bat](https://www.dropbox.com/s/xny4xus73ze6mq9/install-prerelease.bat?dl=1)** et placez-le dans le dossier du serveur pour installer la dernière pré-version
  - Pour obtenir et installer des plugins, consultez la section [Installing plugins](#installing-plugins) ci-dessous.
**Note:** Si vous installez EXILED sur un serveur distant, assurez-vous d'exécuter le .exe en tant que même utilisateur qui exécute vos serveurs SCP:SL (ou un utilisateur avec des privilèges d'administration)

### Installation manuelle
  - Téléchargez **`Exiled.tar.gz` [ici](https://github.com/Exiled-Team/EXILED/releases)**
  - Extrayez son contenu avec [7Zip](https://www.7-zip.org/) ou [WinRar](https://www.win-rar.com/download.html?&L=6)
  - Déplacez le dossier **``EXILED``** vers **`%appdata%`** *Note: Ce dossier doit être placé dans ``C:\Users\%UserName%\AppData\Roaming``, et ***NON*** ``C:\Users\%UserName%\AppData\Roaming\SCP Secret Laboratory``, et **IL DOIT** être dans (...)\AppData\Roaming, et non (...)\AppData\!*
  - Déplacez **``SCP Secret Laboratory``** vers **`%appdata%`**.
    - Windows 10 & 11:
      Écrivez `%appdata%` dans Cortana / l'icône de recherche, ou la barre d'exploration de fichiers Windows.
    - Toute autre version de Windows : 
      Appuyez sur Win + R et tapez `%appdata%`

### Installation de plugin
C'est tout, EXILED devrait maintenant être installé et actif la prochaine fois que vous démarrez votre serveur. Notez que EXILED par lui-même ne fera presque rien, alors assurez-vous d'obtenir de nouveaux plugins depuis **[notre serveur Discord](https://discord.gg/PyUkWTg)**
- Pour installer un plugin, il suffit de :
  - Télécharger un plugin depuis [*cette page*](https://i.imgur.com/u34wgPD.jpg) (**il DOIT s'agir d'un fichier `.dll`!**)
  - Déplacer vers: ``C:\Users\%UserName%\AppData\Roaming\EXILED\Plugins`` (pour y accéder, appuyez sur Win + R, puis écrivez `%appdata%`)

# Linux
### Installation automatique ([plus d'informations](https://github.com/Exiled-Team/EXILED/blob/master/Exiled.Installer/README.md))

**Note:** Si vous installez EXILED sur un serveur distant, assurez-vous d'exécuter l'installateur en tant que même utilisateur qui exécute vos serveurs SCP:SL (ou en tant que root).

  - Téléchargez **`Exiled.Installer-Linux` [ici](https://github.com/Exiled-Team/EXILED/releases)** (cliquez sur "Assets" -> téléchargez l'installateur)
  - Installez-le en tapant soit **`./Exiled.Installer-Linux --path /path/to/server`** ou déplacez-le directement à l'intérieur du dossier du serveur, accédez-y avec le terminal (`cd`) et tapez: **`./Exiled.Installer-Linux`**.
  - Si vous souhaitez la dernière pré-version, ajoutez simplement **`--pre-releases`**. Exemple: **`./Exiled.Installer-Linux /home/scp/server --pre-releases`**
  - Un autre exemple, si vous avez placé `Exiled.Installer-Linux` dans votre dossier de serveur: **`/home/scp/server/Exiled.Installer-Linux --pre-releases`**
  - Pour obtenir et installer des plugins, consultez la section [Installing plugins](#installing-plugins-1) ci-dessous.

### Installation manuelle
  - **Assurez-vous** d'être connecté en tant qu'utilisateur qui exécute le(s) serveur(s) SCP.
  - Téléchargez **`Exiled.tar.gz` [ici](https://github.com/Exiled-Team/EXILED/releases)** (SSH : faites un clic droit et obtenez le lien `Exiled.tar.gz` puis tapez: **`wget (link_to_download)`**)
  - Pour l'extraire dans votre dossier actuel, tapez **``tar -xzvf EXILED.tar.gz``**
  - Déplacez le dossier **`EXILED`** vers **``~/.config``**. *Remarque : Ce dossier doit être placé dans ``~/.config``, et ***NON*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv EXILED ~/.config/`**)
  - Déplacez le dossier **`SCP Secret Laboratory`** vers **``~/.config``**. *Remarque : Ce dossier doit être placé dans ``~/.config``, et ***NON*** ``~/.config/SCP Secret Laboratory``* (SSH: **`mv SCP Secret Laboratory ~/.config/`**)

### Installation de plugins
C'est tout, EXILED devrait maintenant être installé et actif la prochaine fois que vous démarrez votre serveur. Notez que EXILED par lui-même ne fera presque rien, alors assurez-vous d'obtenir des plugins depuis **[depuis notre serveur Discord](https://discord.gg/PyUkWTg)**
- Pour installer un plugin, simplement :
  - Téléchargez un plugin depuis [*cette page*](https://i.imgur.com/u34wgPD.jpg) (**il DOIT s'agir d'un fichier `.dll`!**)
  - Déplacez-le vers: ``~/.config/EXILED/Plugins`` (si vous utilisez SSH en tant que root, alors recherchez le `.config` correct qui sera à l'intérieur de `/home/(SCP Server User)`)

# Config
EXILED offre en soi quelques options de configuration.
Toutes sont générées automatiquement au démarrage du serveur et se trouvent dans le fichier  ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` (``%AppData%\EXILED\Configs\(ServerPortHere)-config.yml`` on Windows).

Les configurations des plugins ***NE seront PAS*** dans le fichier ``config_gameplay.txt`` mentionné précédemment. Au lieu de cela, les configurations des plugins sont définies dans le fichier ``~/.config/EXILED/Configs/(ServerPortHere)-config.yml`` (``%AppData%\EXILED\(ServerPortHere)-config.yml`` sur Windows).
Cependant, certains plugins peuvent obtenir leurs paramètres de configuration à partir d'autres emplacements par eux-mêmes. Ceci est simplement l'emplacement par défaut d'EXILED pour eux, donc référez-vous au plugin individuel s'il y a des problèmes.

# Pour les Développeurs

Si vous souhaitez créer un plugin pour EXILED, c'est relativement simple à faire. Si vous avez besoin ou souhaitez consulter un tutoriel, suivez les instructions sur notre page [Bien Commencer](https://github.com/Exiled-Team/EXILED/blob/master/GettingStarted.md).

Pour des tutoriels plus complets et régulièrement mis à jour, consultez [le site d'EXILED](https://exiled-team.github.io/EXILED/articles/install.html).

Mais veuillez faire attention à suivre les règles suivantes lorsque vous publiez vos plugins :

- Votre plugin doit contenir une classe appartenant à ``Exiled.API.Features.Plugin<>``, sinon EXILED ne chargera pas votre plugin lorsque le serveur démarrera.
- Lorsqu'un plugin est chargé, le code de la classe mentionnée précédemment dans ``OnEnabled()`` est immédiatement exécuté et n'attend pas que les autres plugins soient chargés. Il n'attend pas non plus que le démarrage du serveur soit terminé. ***Et n'attend en aucun cas.*** Lors de la mise en place de votre méthode ``OnEnabled()``, assurez-vous de ne pas accéder à des choses qui ne devraient pas être démarrées par le serveur à ce moment-là, comme par exemple ``ServerConsole.Port``, ou ``PlayerManager.localPlayer``.
- Si vous avez besoin d'accéder à certaines choses avant que votre plugin ne soit chargé, il est recommandé d'attendre l'événement ``WaitingForPlayers`` pour cela. Sinon, encadrez votre code d'une boucle ``while(!x)`` qui vérifie si votre variable/objet qui a besoin de ne plus être ``NULL`` avant de continuer.
- EXILED prend en charge le rechargement dynamique des assemblies de plugins en cours d'exécution, ce qui signifie que si vous devez mettre à jour un plugin, cela peut être fait sans redémarrer le serveur. Cependant, si vous mettez à jour un plugin en cours d'exécution, le plugin doit être correctement configuré pour le prendre en charge, sinon vous rencontrerez des problèmes. Consultez la section [Mise à jour Dynamiques](https://github.com/Exiled-Team/EXILED/blob/master/README-FR.md#mise-a-jour-dynamiques) pour plus d'informations et de directives à suivre.
- Il n'y a ***AUCUN*** événement ``OnUpdate``, ``OnFixedUpdate`` ou ``OnLateUpdate`` dans EXILED. Si vous devez exécuter du code aussi souvent, vous pouvez utiliser des coroutines de MEC (ou More Effective Coroutines) qui attendent une frame, 0.01f, ou utiliser une couche de synchronisation comme ``Timing.FixedUpdate`` à la place.
 ### Les coroutines d'MEC
Si vous n'êtes pas familier avec MEC, voici un bref et simple guide pour vous aider à démarrer.
Les coroutines MEC sont essentiellement des méthodes chronométrées qui prennent en charge des périodes d'attente avant de poursuivre l'exécution, sans interrompre/pausé le thread principal du jeu.
Les coroutines MEC sont sûres à utiliser avec Unity, contrairement au threading traditionnel. ***NE TENTEZ PAS de créer de nouveaux threads pour interagir avec Unity, cela FERA planter le serveur.***

Pour utiliser MEC, vous devrez référencer ``Assembly-CSharp-firstpass.dll`` depuis les fichiers du serveur et inclure ``using MEC;``.
Exemple d'appel d'une simple coroutine qui se répète avec un délai entre chaque boucle :

```cs
using MEC;
using Exiled.API.Features;

public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}

public IEnumerator<float> MyCoroutine()
{
    for (;;) //répète la fonction suivante infiniment
    {
        Log.Info("Hey Je suis une boucle infini!"); //appel à Log.Info pour écrire une ligne sur la console/les logs du serveur.
        yield return Timing.WaitForSeconds(5f); //Dit à la coroutine d'attendre 5 secondes avant de continuer. Comme c'est à la fin de la boucle, cela empêche effectivement la boucle de se répéter pendant 5 secondes.
    }
}
```

Il est ***fortement*** recommandé de faire quelques recherches sur Google ou de demander autour de vous sur Discord si vous n'êtes pas familier avec MEC et que vous souhaitez en apprendre davantage, obtenir des conseils ou de l'aide. Les questions, aussi 'bêtes' soient-elles, seront toujours répondues de manière aussi utile et claire que possible pour aider les développeurs de plugins à exceller. Un meilleur code profite à tout le monde.

### Mise a jour Dynamiques
EXILED en tant que framework prend en charge le rechargement dynamique des assemblies de plugins sans nécessiter de redémarrage du serveur.
Par exemple, si vous démarrez le serveur avec juste ``Exiled.Events`` comme seul plugin, et que vous souhaitez en ajouter un nouveau, vous n'avez pas besoin de redémarrer le serveur pour accomplir cette tâche. Vous pouvez simplement utiliser la commande de la console à distance ou de la console du serveur ``reload plugins`` pour recharger tous les plugins EXILED, y compris les nouveaux qui n'ont pas été chargés auparavant.

Cela signifie également que vous pouvez *mettre à jour* les plugins sans avoir à redémarrer complètement le serveur également. Cependant, il existe quelques directives que le développeur de plugin doit suivre pour que cela soit réalisé correctement :

***Pour l'Hôte :***
 - Si vous mettez à jour un plugin, assurez-vous que le nom de son ``assembly`` n'est pas le même que celui de la version actuellement installée (si elle existe). Le plugin doit être construit par le développeur en gardant à l'esprit les mises à jour dynamiques pour que cela fonctionne, simplement renommer le fichier ne suffira pas.
 - Si le plugin prend en charge les mises à jour dynamiques, assurez-vous que lorsque vous placez la version plus récente du plugin dans le dossier "Plugins", vous supprimez également l'ancienne version du dossier avant de recharger EXILED. L'échec à assurer cela entraînera de nombreux problèmes.
 - Tous les problèmes découlant de la mise à jour dynamique d'un plugin relèvent uniquement de la responsabilité de vous et du développeur du plugin en question. Bien qu'EXILED soutienne pleinement et encourage les mises à jour dynamiques, la seule façon pour cela de ne pas fonctionner ou de mal se dérouler est si l'hôte du serveur ou le développeur du plugin a commis une erreur. Vérifiez que tout a été fait correctement par les deux parties avant de signaler un bug aux développeurs d'EXILED concernant les mises à jour dynamiques.

***Pour les développeurs :***
 - Les plugins souhaitant prendre en charge la mise à jour dynamique doivent veiller à se désabonner de tous les événements auxquels ils sont connectés lorsqu'ils sont désactivés ou rechargés.
 - Les plugins comportant des patches ``Harmony`` personnalisés doivent utiliser une sorte de variable changeante dans le nom de l'instance ``Harmony``, et doivent appeler ``UnPatchAll()`` sur leur instance Harmony lorsque le plugin est désactivé ou rechargé.
 - Toutes les coroutines démarrées par le plugin dans ``OnEnabled()`` doivent également être arrêtées lorsque le plugin est désactivé ou rechargé.

Tout cela peut être réalisé dans les méthodes ``OnReloaded()`` ou ``OnDisabled()`` de la classe du plugin. Lorsque EXILED recharge les plugins, il appelle ``OnDisabled()``, puis ``OnReloaded()``, puis il chargera les nouvelles assemblies, et ensuite exécute ``OnEnabled()``.

Notez qu'il s'agit d'*assemblées nouvelles*. Si vous remplacez une ``assembly`` par une autre portant le même nom, elle ne sera ***PAS*** mise à jour. Cela est dû au GAC (Global Assembly Cache) ; si vous tentez de "charger" une ``assembly`` qui est déjà en cache, elle utilisera toujours l'``assembly`` mise en cache.
Pour cette raison, si votre plugin prend en charge les mises à jour dynamiques, vous devez construire chaque version avec un nom d'``assembly`` différent dans les options choisis (renommer le fichier ne fonctionnera pas). De plus, étant donné que l'ancienne ``assembly`` n'est pas "supprimé" lorsqu'elle n'est plus nécessaire, si vous ne vous désabonnez pas des événements, ne démontez pas votre instance ``Harmony``, n'arrêtez pas les coroutines, etc., ce code continuera également à s'exécuter ainsi que celui de la nouvelle version.
C'est une très mauvaise idée de laisser cela se produire.

En conséquence, les plugins prenant en charge les mises à jour dynamiques ***DOIVENT*** suivre ces directives, sinon ils seront retirés du serveur Discord en raison du risque potentiel pour les hôtes de serveurs.

Tous les plugins ne doivent pas forcément prendre en charge les mises à jour dynamiques. Si vous n'avez pas l'intention de prendre en charge les mises à jour dynamiques, c'est parfaitement acceptable. Évitez simplement de changer le nom de l'``assembly`` de votre plugin lorsque vous créez une nouvelle version. Dans de tels cas, assurez-vous que les hôtes de serveurs savent qu'ils devront redémarrer complètement leurs serveurs pour mettre à jour votre plugin.