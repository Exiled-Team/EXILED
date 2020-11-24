// -----------------------------------------------------------------------
// <copyright file="Blinking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
#pragma warning disable SA1313
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Scp173PlayerScript.CallRpcBlinkTime"/>.
    /// Adds the <see cref="Handlers.Scp173.Blinking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp173PlayerScript), nameof(Scp173PlayerScript.CallRpcBlinkTime))]
    internal static class Blinking
    {
        private static bool Prefix(Scp173PlayerScript __instance)
        {
            List<Player> triggers = new List<Player>();

            foreach (var player in Player.List)
            {
                if (player.Team != Team.SCP && player.Team != Team.RIP)
                {
                    Scp173PlayerScript playerScript = player.ReferenceHub.characterClassManager.Scp173;

                    if ((player.Role != RoleType.Tutorial || Exiled.Events.Events.Instance.Config.CanTutorialBlockScp173) && playerScript.LookFor173(__instance.gameObject, true) && __instance.LookFor173(player.GameObject, false))
                        triggers.Add(player);
                }
            }

            if (triggers.Count > 0)
                Handlers.Scp173.OnBlinking(new BlinkingEventArgs(Player.Get(__instance.gameObject), triggers.ToArray(), __instance.blinkDuration_notsee));

            return true;
        }
    }
}
