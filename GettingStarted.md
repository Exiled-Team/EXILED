# Exiled Low-Level Documentation
*(Written by [KadeDev](https://github.com/KadeDev) for the community)*

## Getting Started
### Intro
Exiled is a low-level API meaning that you can call functions from the game without needing a bunch of API bloatware.

This allows Exiled to be updated quite easily, and Exiled can be updated even before the update hits the game.

It also allows plugin developers to not have to change their code after every update to Exiled or to SCP:SL. In fact, they don't even have to update their plugins!

This documentation will show you the bare basics of making an Exiled Plugin. From here you can start showing the world what creative things you can make with this framework!

### Example Plugin
The [Example Plugin](https://github.com/galaxy119/EXILED/tree/master/Exiled.Example) which is a simple plugin that shows off events and how to properly make them. Using this example will help you learn how to properly use Exiled. There are a couple of things in that plugin that are important, lets talk about them

#### On Enable + On Disable Dynamic Updates
Exiled is a framework that has a **Reload** command which can be used to reload all the plugins and get new ones. This means you must make your plugins **Dynamically Updatable.** This means that every variable, event, coroutine, etc *must* be assigned when enabled and nullified when disabled. The **On Enable** method should enable it all, and the **On Disable** method should disable it all. But you might be wondering what about **On Reload**? That void is meant to carry over static variables, as in every static constant you make won't be wiped. So you could do something like this:
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

And the output would be:
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
(Of course excluding anything besides the actual responses)
Without doing this it would have just went to 1 and then to 2 again.

### Players + Events
Now that we are done with getting our plugins **Dynamically Updatable** we can focus on trying to interact with players with events!

An event is pretty cool, it allows SCP:SL to communicate with Exiled and then with Exiled to all the plugins!

You can listen to events for your plugin by add this to the top of your main plugin source file:
```csharp
using EXILED;
```
And then you have to reference the `Exiled.Events.dll` file for you to actually get events.

To reference an event we will be using a new class we create; called "EventHandlers". The event handler is not provided by default; you must create it.


We can reference it in the OnEnable and OnDisable void like this:

`MainClass.cs`
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    // Register the event handler class. And add the event,
    // to the EXILED_Events event listener so we get the event.
    EventHandler = new EventHandlers();
    Player.Verified += EventHandler.PlayerVerified;
}

public override OnDisable()
{
    // Make it dynamically updatable.
    // We do this by removing the listener for the event and then nulling the event handler.
    // This process must be repeated for each event.
    Player.Verified -= EventHandler.PlayerVerified;
    EventHandler = null;
}
```

And in the EventHandlers class we would do:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {

    }
}
```
Now we have successfully hooked to a player verified event which fires when ever a player is authenticated after joining the server! It is important to note that every event has different event arguments, and each type of event argument has different properties associated with it.

EXILED already provides a broadcast function, so let's use it in our event:

```csharp
public class EventHandlers
{
    public void PlayerVerified(VerifiedEventArgs ev)
    {
        ev.Player.Broadcast(5, "<color=lime>Welcome to my cool server!</color>");
    }
}
```

As stated above, every event has different arguments. Below is a different event that turns tesla gates off for Nine-Tailed Fox players.

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
    // Don't forget, events must be disconnected and nullified on the disable method.
    Player.TriggeringTesla -= EventHandler.TriggeringTesla;
    EventHandler = null;
}
```

And in the EventHandlers class.

`EventHandlers.cs`
```csharp
public class EventHandlers
{
    public void TriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        // Disable the event for nine tailed fox players.
        // This can be accomplished by checking the player's team.
        if (ev.Player.Team == Team.MTF) {
            // Disable the tesla trigger by setting ev.IsTriggerable to false.
            // Players who have a MTF ranking will no longer trigger tesla gates.
            ev.IsTriggerable = false;
        }
    }
}
```


### Configs
The majority of Exiled plugins contain configs. Configs allow server maintainers to modify plugins to their desire, although this is limited to the configuration the plugin developer provides.

First create a `config.cs` class, and change your plugin inheritance from `Plugin<>` to `Plugin<Config>`

Now you need to make that config inherit `IConfig`. After inheriting from `IConfig`, add a property to the class titled `IsEnabled` and `Debug`. Your Config class should now look like this:

```csharp
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
```

You can add any config option in there and reference it like so:

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

And then congratulations! You have made your very first Exiled Plugin! It is important to note that all plugins **must** have an IsEnabled configuration. This config allows server owners to enable and disable the plugin at their own accord. The IsEnabled config will be read by the Exiled loader (your plugin does not need to check if `IsEnabled == true` or not.).

### What now?
If you want more information you should join our [discord!](https://discord.gg/PyUkWTg)

We have a #resources channel that you might find useful, as well as exiled contributors and plugin developers who would be willing to assist you in the creation of your plugin(s).

Or you could read all the events that we have! If you want to check them out [here!](https://github.com/galaxy119/EXILED/tree/master/Exiled.Events/EventArgs)
