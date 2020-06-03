// -----------------------------------------------------------------------
// <copyright file="UpgradingScp914Items.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using System.Linq;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp914.Scp914Machine.ProcessItems"/>.
    /// Adds the <see cref="Map.UpgradingScp914Items"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914.Scp914Machine), nameof(Scp914.Scp914Machine.ProcessItems))]
    public class UpgradingScp914Items
    {
        /// <summary>
        /// Prefix of <see cref="Scp914.Scp914Machine.ProcessItems"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp914.Scp914Machine"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp914.Scp914Machine __instance)
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

            var ev = new UpgradingScp914ItemsEventArgs(__instance, __instance.players.Select(player => API.Features.Player.Get(player.gameObject)).ToList(), __instance.items, __instance.knobState);

            Map.OnUpgradingScp914Items(ev);

            var players = ev.Players.Select(player => player.ReferenceHub.characterClassManager).ToList();

            __instance.MoveObjects(ev.Items, players);

            if (ev.IsAllowed)
                __instance.UpgradeObjects(ev.Items, players);

            return false;
        }
    }
}
