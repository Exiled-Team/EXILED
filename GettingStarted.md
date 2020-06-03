# Exiled Low-Level Documentation
*(Written by [KadeDev](https://github.com/KadeDev) for the community)*

**Table of contents**

*Want to just skip ahead to the part you want? Heres your chance!*

[Intro](#intro)

[Sample Plugin](#sample-plugin)

[Events and ReferenceHubs](#referencehubs--events)

## Getting Started
### Intro
Exiled is a low-level API meaning that you can call functions from the game without needing a bunch of api bloatwear.

This allows Exiled to be updated quite easily, and Exiled can be updated even before the update hits the game.

It also allows plugin developers to not have to change their code in order to update, in fact they don't have to even update!

This documentation will show you the bare basics of making a Exiled Plugin. From here you can start showing the world what creative things you can make with this framework!

### Sample Plugin
The [Sample Plugin](https://github.com/galaxy119/SamplePlugin) which is a simple plugin that shows off events and how to properly make them; will help you learn Exiled. There are a couple things in that plugin that are important, lets talk about them

#### On Enable + On Disable Dyanmic Updates
Exiled is a framework that has a **Reload** command which can be used to reload all the plugins and get new ones. This means you must make your plugins **Dynamicly Updatable.** What this means is assigning variables, events, and other stuff like cororutines. Must be dis-assigned and nulled in on disable. **On Enable** should enable it all, and **On Disable** should disable it all. But you might be wondering what about **On Reload**? That void is meant to carry over static variables, as in every static constant you make will not be wiped. So you could do something like this:
```csharp
class PluginClass : Plugin<IConfig>
{
	public static int StaticCount = 0; 
	public int counter = 0; 

	public override void OnEnabled() 
	{ 
		counter = StaticCount; 
		counter++; 
		Info(counter); 
	} 

	public override void OnDisabled() 
	{ 
		counter++; 
		Info(counter); 
	} 

	public override void OnReloaded() 
	{ 
		StaticCount = counter; 
	}
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
(Of course dis-including anything besides the actual responses)
Without doing this it would have just went to 1 and then to 2 again.

### ReferenceHubs + Events
Now that we are done with getting our plugins **Dynamicly Updatable** we can focus on trying to interact with players with events!

An event is pretty cool, it allows SCP:SL to communicate with Exiled and then with Exiled to all the plugins!

You can listen to events for your plugin by add this to the top of your main plugin source file:
```csharp
using EXILED;
```
And then you have to reference the `EXILED_Events.dll` file for you to actually get events.

To reference an event we will be using a new class we create; called "EventHandlers". This isn't in Exiled we have to make it ourself.

We can reference it in the OnEnable and OnDisable void like this:
```csharp
class PluginClass : Plugin<IConfig>
{
	public EventHandlers EventHandler;

	public override OnEnabled()
	{
		// Register the event handler class. And add the event,
		// to the EXILED_Events event listener so we get the event.
		EventHandler = new EventHandlers();
		Events.PlayerJoinEvent += EventHandler.PlayerJoined;
	}

	public override OnDisabled()
	{
		// Make it dynamicly updatable.
		// We do this by removing the listener for the event and then nulling the event handler.
		// The more events the more times you have to do this for each one.
		Events.PlayerJoinEvent -= EventHandler.PlayerJoined;
		EventHandler = null;
	}
}
```
And in the EventHandlers class we would do:
```csharp
public class EventHandlers
{
	public void PlayerJoined(PlayerJoinEvent ev)
	{
		
	}
}
```
Now we have successfully hooked to a player join event which fires when ever a player joins!

Now we can use the Exiled Player Class to Broadcast a message to the players!

```csharp
public class EventHandlers
{
	public void PlayerJoined(PlayerJoinEvent ev)
	{
		// ev.Player is a class called Player, it has multiple properties that are very useful
		// in plugin development!
		ev.Player.Broadcast(5, "<color=lime>Welcome to my cool server!</color>");
	}
}
```

And then congratulations! You have made your very first Exiled Plugin!

### What now?
If you want more information you should join our [Discord!](https://discord.gg/SXnFZez)

We have a [#resources](https://discord.com/channels/656673194693885975/668962626780397569) channel that you might find useful.
