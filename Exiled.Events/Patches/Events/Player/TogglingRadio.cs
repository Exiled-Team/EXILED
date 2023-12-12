// -----------------------------------------------------------------------
// <copyright file="TogglingRadio.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
namespace Exiled.Events.Patches.Events.Player
{

    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Radio;
    using PluginAPI.Events;

    /// <summary>
    ///     Patches <see cref="RadioItem.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Player.TogglingRadio" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.TogglingRadio))]
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.ServerProcessCmd))]
    internal static class TogglingRadio
    {
        private static bool Prefix(RadioItem __instance, RadioMessages.RadioCommand command)
        {
            if (__instance.Owner.HasBlock(BlockedInteraction.ItemUsage))
                return false;

            switch (command)
            {
                case RadioMessages.RadioCommand.Enable:
                    {
                        if (__instance._enabled || __instance._battery <= 0f || !EventManager.ExecuteEvent(new PlayerRadioToggleEvent(__instance.Owner, __instance, true)))
                            return false;

                        TogglingRadioEventArgs ev = new(Player.Get(__instance.Owner), __instance, true);
                        Handlers.Player.OnTogglingRadio(ev);
                        if (!ev.IsAllowed)
                            return false;

                        __instance._enabled = ev.NewState == __instance._enabled ? true : ev.NewState;
                        break;
                    }

                case RadioMessages.RadioCommand.Disable:
                    {
                        if (!__instance._enabled || !EventManager.ExecuteEvent(new PlayerRadioToggleEvent(__instance.Owner, __instance, false)))
                            return false;

                        TogglingRadioEventArgs ev = new(Player.Get(__instance.Owner), __instance, false);
                        Handlers.Player.OnTogglingRadio(ev);
                        if (!ev.IsAllowed)
                            return false;

                        __instance._enabled = ev.NewState == __instance._enabled ? false : ev.NewState;
                        break;
                    }

                case RadioMessages.RadioCommand.ChangeRange:
                    {
                        byte b = (byte)(__instance._rangeId + 1);
                        if (b >= __instance.Ranges.Length)
                        {
                            b = 0;
                        }

                        PlayerChangeRadioRangeEvent playerChangeRadioRangeEvent = new PlayerChangeRadioRangeEvent(__instance.Owner, __instance, (RadioMessages.RadioRangeLevel)b);
                        if (!EventManager.ExecuteEvent(playerChangeRadioRangeEvent))
                        {
                            return false;
                        }

                        b = __instance._rangeId = (byte)playerChangeRadioRangeEvent.Range;
                        break;
                    }
            }

            __instance.SendStatusMessage();
            return false;
        }
    }
}