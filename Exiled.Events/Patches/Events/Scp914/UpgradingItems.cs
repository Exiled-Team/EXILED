// -----------------------------------------------------------------------
// <copyright file="UpgradingItems.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
#pragma warning disable SA1313
    using System.Linq;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using global::Scp914;
    using HarmonyLib;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp914Machine.ProcessItems"/>.
    /// Adds the <see cref="Scp914.UpgradingItems"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Machine), nameof(Scp914Machine.ProcessItems))]
    internal class UpgradingItems
    {
        private static bool Prefix(Scp914Machine __instance)
        {
            if (!NetworkServer.active)
                return true;
            Collider[] colliderArray = Physics.OverlapBox(__instance.intake.position, __instance.inputSize / 2f);
            __instance.players.Clear();
            __instance.items.Clear();
            foreach (Collider collider in colliderArray)
            {
                CharacterClassManager component1 = collider.GetComponent<CharacterClassManager>();
                if (component1 != null)
                {
                    __instance.players.Add(component1);
                }
                else
                {
                    Pickup component2 = collider.GetComponent<Pickup>();
                    if (component2 != null)
                        __instance.items.Add(component2);
                }
            }

            var ev = new UpgradingItemsEventArgs(__instance, __instance.players.Select(player => API.Features.Player.Get(player.gameObject)).ToList(), __instance.items, __instance.knobState);

            Scp914.OnUpgradingItems(ev);

            var players = ev.Players.Select(player => player.ReferenceHub.characterClassManager).ToList();

            __instance.MoveObjects(ev.Items, players);

            if (ev.IsAllowed)
                __instance.UpgradeObjects(ev.Items, players);

            return false;
        }
    }
}
