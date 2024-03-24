# Exiled Documentation pour les bas niveau
*(écrit par [KadeDev](https://github.com/KadeDev) pour la communautée)*
*(traduit par [Crazy](https://github.com/CrazyMega02))

## Savoir bien commencer
### Intro
Exiled est une API de bas niveau, c'est à dire que vous pouvez faire appel à des fonction du jeux sans avoir besoin d'API de type bloatware (c'est à dire pré installé dans le jeux).

Cela permet des mise à jour relativement facile d'Exiled et des mises à jour avant même que le jeux soit mise à jour lui-même.

Cela permet aussi au developpeurs de plugin à ne pas avoir à constament mettre à jour leur code avec chaque mise à jour d'Exiled ou d'SCP:SL. Il n'y a même pas besoin de mètre à jour leur plugins!

Ce document ci-joint va vous apprendre les bases pour la création d'un plugin Exiled.D'ici là vous pouvez motrer au monde entier toute la créativitée que vous avez en vous et pourrez créé grace à ce-ci!

### Example de Plugin
Un [Exemple de Plugin](https://github.com/galaxy119/EXILED/tree/master/Exiled.Example) qui est un simple plugin montrant les différents évenements et comment les mettre en place proprement. Cette exemple vous permettra d'apprende à utiliser correctement Exiled. Plusieurs choses sont important dans ce plugin, nous allons donc les voir.

#### On Enable + On Disable Mise à jour Dynamique
Exiled est un framework qui dispose d'une commande de **Rechargement** qui peut être utilisée pour recharger tous les plugins et en obtenir de nouveaux. Cela signifie que vous devez rendre vos plugins **Dynamiquement à jour.** Cela signifie que chaque variable, événement, coroutine, etc. *doit* être assigné lorsqu'il est activé et annulé lorsqu'il est désactivé. La méthode **On Enable** devrait tout activer, et la méthode **On Disable** devrait tout désactiver. Mais vous vous demandez peut-être ce qu'il en est de **On Reload**? Cette fonction est destinée à transférer les variables statiques, c'est-à-dire que toutes les constantes statiques que vous créez ne seront pas effacées. Vous pouvez donc faire quelque chose comme cela :
```csharp
public static int StaticCount = 0;
public int counter = 0;

public override void OnEnable()
{
    counter = StaticCount;
    counter++;
    Info(counter);
}

public override void OnDisable()
{
    counter++;
    Info(counter);
}

public override void OnReload()
{
    StaticCount = counter;
}
```

Et le résultat serait :
```bash
# On enable fires
1
# Reload command
# On Disable fires
2
# On Reload fires
# On Enable fires again
3

```
Bien sûr, excluant tout ce qui est autre que les réponses réelles. Sans cela, il serait simplement passé à 1 puis à 2 à nouveau.

### Joueurs + Events
Maintenant que nous avons terminé de rendre nos plugins **Dynamiquement à jour**, nous pouvons nous concentrer sur la tentative d'interaction avec les joueurs grâce aux événements !

Un événement est assez cool, il permet à SCP:SL de communiquer avec Exiled, puis à Exiled avec tous les plugins !

Vous pouvez écouter les événements pour votre plugin en ajoutant ceci en haut de votre fichier source principal du plugin :
```csharp
using EXILED;
```
Ensuite, vous devez référencer le fichier `Exiled.Events.dll` pour réellement obtenir des événements.

Pour référencer un événement, nous utiliserons une nouvelle classe que nous créons ; appelée "EventHandlers". Le gestionnaire d'événements n'est pas fourni par défaut ; vous devez le créer.

Nous pouvons le référencer dans les méthodes OnEnable et OnDisable de la manière suivante :

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    // Enregistrez la classe gestionnaire d'événements. Et ajoutez l'événement,
    // à l'écouteur d'événements EXILED_Events pour obtenir l'événement.
    EventHandler = new EventHandlers();
    Player.Verified += EventHandler.PlayerVerified;
}

public override OnDisable()
{
    // Rendez-le dynamiquement mis à jour.
    // Nous faisons cela en supprimant l'écouteur de l'événement, puis en annulant le gestionnaire d'événements.
    // Ce processus doit être répété pour chaque événement.
    Player.Verified -= EventHandler.PlayerVerified;
    EventHandler = null;
}
```

Et dans la classe EventHandlers, nous ferions :

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {

    }
}
```
Maintenant, nous nous sommes correctement connectés à un événement de joueur vérifié qui se déclenche chaque fois qu'un joueur est authentifié après avoir rejoint le serveur ! Il est important de noter que chaque événement a des arguments d'événement différents, et chaque type d'argument d'événement a des propriétés différentes qui lui sont associées.

EXILED fournit déjà une fonction de diffusion, alors utilisons-la dans notre événement :

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        ev.Player.Broadcast(5, "<color=lime>Welcome to my cool server!</color>");
    }
}
```

Comme indiqué ci-dessus, chaque événement a des arguments différents. Ci-dessous se trouve un événement différent qui désactive les portes Tesla pour les joueurs Nine-Tailed Fox.

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    EventHandler = new EventHandlers();
    Player.TriggeringTesla += EventHandler.TriggeringTesla;
}

public override OnDisable()
{
    // N'oubliez pas, les événements doivent être déconnectés et annulés dans la méthode de désactivation.
    Player.TriggeringTesla -= EventHandler.TriggeringTesla;
    EventHandler = null;
}
```

Et dans la classe EventHandlers.

`EventHandlers.cs`
```csharp
public class EventHandlers
{
    public void TriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        // Désactiver l'événement pour les joueurs du personnel de la Fondation.
        // Cela peut être accompli en vérifiant le côté du joueur.
        if (ev.Player.Role.Side == Side.Mtf) {
            // Désactivez le déclencheur de la porte Tesla en définissant ev.IsTriggerable sur false.
            // Les joueurs ayant un rang MTF ne déclencheront plus les portes Tesla.
            ev.IsTriggerable = false;
        }
    }
}
```


### Configs
La majorité des plugins Exiled contiennent des configurations. Les configurations permettent aux administrateurs de serveur de modifier les plugins selon leurs besoins, bien que cela soit limité à la configuration fournie par le développeur du plugin.

Commencez par créer une classe `config.cs`, et changez l'héritage de votre plugin de `Plugin<>` à `Plugin<Config>`.

Maintenant, vous devez faire en sorte que cette configuration hérite de `IConfig`. Après avoir hérité de `IConfig`, ajoutez une propriété à la classe intitulée `IsEnabled` et `Debug`. Votre classe Config devrait maintenant ressembler à ceci :

```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
```

Vous pouvez ajouter n'importe quelle option de configuration là-dedans et y faire référence comme ceci :

`Config.cs`
```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
        public string TextThatINeed { get; set; } = "this is the default";
    }
```

`MainClass.cs`
```csharp
   public override OnEnabled()
   {
        Log.Info(Config.TextThatINeed);
   }
```

Et félicitations ! Vous avez créé votre tout premier plugin Exiled ! Il est important de noter que tous les plugins **doivent** avoir une configuration IsEnabled. Cette configuration permet aux propriétaires de serveurs d'activer et de désactiver le plugin à leur guise. La configuration IsEnabled sera lue par le chargeur Exiled (votre plugin n'a pas besoin de vérifier si `IsEnabled == true` ou non).

### Que faire Maintenant ?
Si vous voulez plus d'informations, vous devriez rejoindre notre [discord!](https://discord.gg/PyUkWTg)

Nous avons un canal #resources que vous pourriez trouver utile, ainsi que des contributeurs Exiled et des développeurs de plugins qui seraient prêts à vous aider dans la création de votre/vos plugin(s).

Ou vous pourriez consulter tous les événements que nous avons ! Si vous voulez les consulter, [ici!](https://github.com/galaxy119/EXILED/tree/master/Exiled.Events/EventArgs)
