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

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patch the <see cref="CharacterClassManager.AllowContain"/> to implement <see cref="Scp106Container"/> properties.
    /// </summary>
    internal static class CheckForLurePatch
    {
        [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.AllowContain))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();
            newInstructions.Insert(0, new(OpCodes.Call, Method(typeof(CheckForLurePatch), nameof(CheckPlayers))));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void CheckPlayers()
        {
            foreach (Player player in Player.List)
            {
                if (Scp106Container.CanBeKilled(player))
                {
                    player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.UsedAs106Bait, null));
                    Scp106Container.Base.SetState(false, true);
                    return;
                }
            }
        }
    }
}
