---
sidebar_position: 3
---

:::caution

This tutorial assumes that you are familiar with C# and with setting up a plugin in the EXILED framework. See the tutorial if you are unfamiliar with setting up a plugin using EXILED.
:::

# Events: What are they?
**Events** play a key role in the EXILED framework and all of the plugins utilizing it. Almost every plugin created using the EXILED framework uses events in one way or another. So, what are they? An event is a simple way of being informed when *something* happens. Events range from the round ending, to a player throwing an item or opening a door, to even SCP-096 being enraged! Events allow you to attach code that executes when something occurs before, during, or at the conclusion of a round.

For example, say that you have the following method.

```cs
public void OnDead()
{
    // Show hint to player.
}
```

With EXILED, it is possible to achieve the desired result: Showing a hint to a player who dies.

## Event structure
The EXILED framework consists of two different types of events: Events that can be disallowed, and those that cannot. Events that can be disallowed can prevent certain events from happening; as an example, preventing a player from dying when they normally should. The ability to prevent certain events from happening is what gives EXILED its beauty.

All events are part of a static class called a **handler**. All handlers can be found in the `Exiled.Events` namespace. Every handler is related to a specific feature in the game (eg. `Exiled.Events.Scp096` contains SCP-096 related events).

Almost all events have a corresponding **event argument** class. The event argument provides the data of an event, as well as the ability to prevent it from occurring. All event arguments can be found in the `Exiled.Events.EventArgs` namespace, and all event arguments inherit from `System.EventArgs`.

### Example: Enraging event
The following is the structure of the `Exiled.Events.EventArgs.EnragingEventArgs`.
```cs
public class EnragingEventArgs : System.EventArgs
{
    // Note: Constructor omitted.
    public Scp096 Scp096 { get; } // The SCP-096 instance.
    public Player Player { get; } // The player controlling SCP-096.
    public bool IsAllowed { get; set; } // Whether or not SCP-096 can be enraged.
}
```
Notice the `IsAllowed` property of the event. This property, which defaults to `true`, can be set to `false` to prevent SCP-096 from being enraged. For most events that can be disallowed, `IsAllowed` is set to `true` by default, and plugins can set it to `false` to prevent the event from occurring. However, in some cases, `IsAllowed` defaults to false and plugins can set it to `true` to *allow* the event to occur. An example of this behavior is the `InteractingDoor` event. `IsAllowed` will default to `false` in this event if a player cannot open a door, however plugins may set it to `true` to allow the player to open it regardless.

## Connecting events
Events can be connected and disconnected by using the `+=` and `-=` operators. These can be used in the plugin's `OnEnabled` and `OnDisabled` methods, respectively.
```cs
// Base plugin class
// This example assumes a method called "OnEnraging" exists in this class. For best practice, you should create a new class to handle events.
using Exiled.Events;
public override void OnEnabled()
{
    Scp096.Enraging += OnEnraging; // Scp096 is the event handler, while Enraging is the name of the event. The += operator connects this event to the provided method.
}
public override void OnDisabled()
{
    Scp096.Enraging -= OnEnraging; // The -= operator disconnects this event from the provided method.
}
// Some other class
using Exiled.Events.EventArgs;
public void OnEnraging(EnragingEventArgs ev) // ev is the arguments for the event. Every event has a different argument class with different parameters, so make sure to check its documentation.
{
    Log.Info(ev.Player.Nickname + " has just been enraged!");
}
```

## Async events

_Async events allow you to seamlessly integrate coroutines and event functionalities.
You can find more information about MEC coroutines [here](https://github.com/Exiled-Team/EXILED#mec-coroutines)._
More info about MEC coroutines can be found [here](https://github.com/Exiled-Team/EXILED#mec-coroutines).
```cs
// Base plugin class
// This example assumes a method called "OnEnraging" exists in this class. For best practice, you should create a new class to handle events.
using Exiled.Events;
public override void OnEnabled()
{
    Scp096.Enraging += OnEnraging; // Scp096 is the event handler, while Enraging is the name of the event. The += operator connects this event to the provided method.
}
public override void OnDisabled()
{
    Scp096.Enraging -= OnEnraging; // The -= operator disconnects this event from the provided method.
}
// Some other class
using Sustem.Collections.Generic;

using Exiled.Events.EventArgs;
using MEC;
public IEnumerator<float> OnEnraging(EnragingEventArgs ev) // ev is the arguments for the event. Every event has a different argument class with different parameters, so make sure to check its documentation.
{
    yield return Timing.WaitForSeconds(1f);
    Log.Info(ev.Player.Nickname + " has just been enraged!");
}
```
