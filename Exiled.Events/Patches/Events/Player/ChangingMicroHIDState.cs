// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using InventorySystem.Items.MicroHID;

    /// <summary>
    /// Patches <see cref="MicroHIDItem.State"/>.
    /// Adds the <see cref="Handlers.Player.OnChangingMicroHIDState"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MicroHIDItem), nameof(MicroHIDItem.ExecuteServerside))]
    internal static class ChangingMicroHIDState
    {
        private static bool Prefix(MicroHIDItem __instance, ref byte value)
        {
            try
            {
                ChangingMicroHidStateEventArgs ev = new ChangingMicroHidStateEventArgs(API.Features.Player.Get(__instance.Owner), __instance, HidState.Idle, HidState.PoweringUp, true);

                Player.OnChangingMicroHIDState(ev);

                if (!ev.IsAllowed)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                API.Features.Log.Error($"{typeof(ChangingMicroHidStateEventArgs).FullName}.{nameof(Prefix)}:\n{e}");
                return true;
            }
        }
    }
}
