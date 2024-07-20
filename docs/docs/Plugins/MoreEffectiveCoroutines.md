---
sidebar_position: 2
---

:::caution

This tutorial assumes that you are familiar with C# and with setting up a plugin in the EXILED framework. See the [Plugin Structure](/docs/plugins/Plugin%20Structure) tutorial if you are unfamiliar with setting up a plugin using **EXILED**.

:::

# MEC (More Effective Coroutines)
If you are unfamiliar with MEC, this will be a very brief and simple primer to get you started. **MEC Coroutines** are basically timed methods, that support waiting periods of time before continuing execution, without interrupting/sleeping the main game thread. MEC coroutines are safe to use with Unity, unlike traditional threading, which *will* crash the server.

MEC is useful for plugins which require a pre-defined timeout between execution. As an example, an automatic nuke plugin would want to pause for a certain amount of seconds before activating the warhead. A supply drop plugin would want to wait in certain intervals before executing a supply drop. Both of these are possible with MEC.

## Setup
Unlike other API provided by SCP:SL, MEC requires a reference to the `Assembly-CSharp-firstpass` DLL file. After referencing this file, a `using MEC;` statement allows MEC to be used.

## Coroutine
MEC offers [tons of features](http://trinary.tech/category/mec/free/) for controlling threads. For this tutorial, we are going to look at two of them: coroutines, and delayed calls. A coroutine is a method that is executed by MEC and supports delays. These methods must return type `IEnumerator<float>` and must be called by `Timing.RunCoroutine(Method())`. An example can be seen below, using an infinite loop with a 5 second delay.

```cs
using MEC;
using Exiled.API.Features;
public void SomeMethod()
{
    Timing.RunCoroutine(MyCoroutine());
}
public IEnumerator<float> MyCoroutine()
{
    for (;;) //repeat the loop infinitely
    {
        Log.Info("Hey, I'm a infinite loop!"); //Call Log.Info to print a line to the game console/server logs.
        yield return Timing.WaitForSeconds(5f); //Tells the coroutine to wait 5 seconds before continuing. Since this is at the end of the loop, it effectively stalls the loop from repeating for 5 seconds.
    }
}
```
This example prints, "Hey, I'm an infinite loop!" every 5 seconds infinitely. Coroutines can have multiple `yield return` statements.

## Delayed Calls
A simpler method of running an action after a delay is using `Timing.CallDelayed(float, Action)`, which executes code after a given number of seconds passes. This method does not require a coroutine to be created, hence why it's useful. An example can be seen below, logging a message 5 seconds after the method is called.
```cs
using MEC;
using Exiled.API.Features;
public void SomeMethod()
{
    Timing.CallDelayed(5f, () => // Execute the provided method 5 seconds late.
    {
        Log.Info("This log was printed 5 seconds late!");
    })
}
```
It is ***strongly*** recommended that you do some googling, or ask around **[in the EXILED Discord server](https://discord.gg/exiledreboot)** if you are unfamiliar with MEC and would like to learn more, get advice, or need help. Questions, no matter how 'stupid' they are, will always be answered as helpfully and clearly as possible for plugin developers to excel. Better code is better for everyone.
