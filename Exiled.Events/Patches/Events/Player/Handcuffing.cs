// -----------------------------------------------------------------------
// <copyright file="Handcuffing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Disarming;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Events = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="DisarmedPlayers.SetDisarmedStatus"/>.
    /// Adds the <see cref="Handlers.Player.Handcuffing"/> event.
    /// </summary>
    [HarmonyPatch(typeof(DisarmedPlayers), nameof(DisarmedPlayers.SetDisarmedStatus))]
    internal static class Handcuffing
    {
        private static bool Prefix(Inventory inv, Inventory disarmer)
        {
            bool flag;
            do
            {
                flag = true;
                for (int index = 0; index < DisarmedPlayers.Entries.Count; ++index)
                {
                    if ((int)DisarmedPlayers.Entries[index].DisarmedPlayer == (int)inv.netId)
                    {
                        var uncuffing =
                            new RemovingHandcuffsEventArgs(Player.Get(DisarmedPlayers.Entries[index].Disarmer), Player.Get(inv._hub));
                        Handlers.Player.OnRemovingHandcuffs(uncuffing);
                        if (!uncuffing.IsAllowed)
                            return false;
                        DisarmedPlayers.Entries.RemoveAt(index);
                        flag = false;
                        break;
                    }
                }
            }
            while (!flag);
            if (!(disarmer != null))
                return false;
            var disarming = new HandcuffingEventArgs(Player.Get(disarmer._hub), Player.Get(inv._hub));
            Handlers.Player.OnHandcuffing(disarming);
            if (!disarming.IsAllowed)
                return false;
            DisarmedPlayers.Entries.Add(new DisarmedPlayers.DisarmedEntry(inv.netId, disarmer.netId));

            return false;
        }
    }
}
