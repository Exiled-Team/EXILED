# Exiled Low-Level Documentation
*(Written by [KadeDev](https://github.com/KadeDev) for the community)*

## Getting Started
### Intro
Exiled is a low-level API meaning that you can call functions from the game without needing a bunch of API bloatware.

This allows Exiled to be updated quite easily, and Exiled can be updated even before the update hits the game.

It also allows plugin developers to not have to change their code to update. In fact, they don't even have to update!

This documentation will show you the bare basics of making an Exiled Plugin. From here you can start showing the world what creative things you can make with this framework!

### Example Plugin
The [Example Plugin](https://github.com/galaxy119/EXILED/tree/master/Exiled.Example) which is a simple plugin that shows off events and how to properly make them; will help you learn Exiled. There are a couple of things in that plugin that are important, lets talk about them

#### On Enable + On Disable Dynamic Updates
Exiled is a framework that has a **Reload** command which can be used to reload all the plugins and get new ones. This means you must make your plugins **Dynamically Updatable.** What this means is assigning variables, events, and other stuff like coroutines. Must be dis-assigned and nulled in on disable. **On Enable** should enable it all, and **On Disable** should disable it all. But you might be wondering what about **On Reload**? That void is meant to carry over static variables, as in every static constant you make won't be wiped. So you could do something like this:
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

To reference an event we will be using a new class we create; called "EventHandlers". This isn't in Exiled we have to make it ourself.


We can reference it in the OnEnable and OnDisable void like this:
```csharp
using Player = Exiled.Events.Handlers.Player;

public EventHandlers EventHandler;

public override OnEnable()
{
    // Register the event handler class. And add the event,
    // to the EXILED_Events event listener so we get the event.
    EventHandler = new EventHandlers();
    Player.Joined += EventHandler.PlayerJoined;
}

public override OnDisable()
{
    // Make it dynamically updatable.
    // We do this by removing the listener for the event and then nulling the event handler.
    // The more events the more times you have to do this for each one.
    Player.Joined -= EventHandler.PlayerJoined;
    EventHandler = null;
}
```
And in the EventHandlers class we would do:
```csharp
public class EventHandlers
{
    public void PlayerJoined(JoinedEventArgs ev)
    {

    }
}
```
Now we have successfully hooked to a player join event which fires when ever a player joins!

EXILED already provides a broadcast function, so let's use it in our event:

```csharp
public class EventHandlers
{
    public void PlayerJoined(PlayerJoinEvent ev)
    {
        ev.Player.Broadcast(5, "<color=lime>Welcome to my cool server!</color>");
    }
}
```

### Configs

Now that you've created most of your plugin, if you wanted to create a config. Heres whatcha need to do.

First create a `config.cs` class, and change your plugin inheritance from `Plugin<>` to `Plugin<Config>`

Now you need to make that config inherit `IConfig`. Auto complete it in your IDE. And it should look like this:

```
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
    }
```

You can add any config option in there and reference it like so:

`Config.cs`
```
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; }
        public string TextThatINeed { get; set; } = "this is the default";
    }
```

`MainClass.cs`
```
   public override OnEnabled()
   {
        Log.Info(Config.TextThatINeed);
   }
```

And then congratulations! You have made your very first Exiled Plugin!

### What now?
If you want more information you should join our [Discord!](https://discord.gg/SXnFZez)

We have a #resources channel that you might find useful.

Or you could read all the events that we have! If you want to check them out [here!](https://github.com/galaxy119/EXILED/blob/master/EXILED_Events/Events/EventArgs.cs)

