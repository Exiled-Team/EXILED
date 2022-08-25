// -----------------------------------------------------------------------
// <copyright file="CheckForLurePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patch the <see cref="CharacterClassManager.AllowContain"/> to implement <see cref="Scp106Container"/> properties.
    /// Adds the <see cref="Handlers.Player.EnteringFemurBreaker"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.AllowContain))]
    internal static class CheckForLurePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            yield return new CodeInstruction(OpCodes.Call, Method(typeof(CheckForLurePatch), nameof(CheckPlayers)));
            yield return new(OpCodes.Ret);
        }

        private static void CheckPlayers()
        {
            foreach (Player player in Player.List)
            {
                if (Scp106Container.CanBeKilled(player))
                {
                    EnteringFemurBreakerEventArgs ev = new(player);
                    Handlers.Player.OnEnteringFemurBreaker(ev);

                    if (!ev.IsAllowed)
                        return;
                    player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.UsedAs106Bait));
                    Scp106Container.Base.SetState(false, true);
                    return;
                }
            }
        }
    }
}
