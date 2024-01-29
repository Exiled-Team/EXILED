// -----------------------------------------------------------------------
// <copyright file="HurtingFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using HarmonyLib;

    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="CustomNetworkManager.OnServerDisconnect(NetworkConnectionToClient)" />.
    /// </summary>
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), typeof(NetworkConnectionToClient))]
    internal static class HurtingFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            Label skip = generator.DefineLabel();

            int offset = 2;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(PlayerStatsSystem.PlayerStats), nameof(PlayerStatsSystem.PlayerStats.DealDamage)))) + offset;

            // Attach a skip label for skipping the DealDamge call
            newInstructions[index].WithLabels(skip);

            offset = -8;
            index += offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (!Player.Get(hub).IsAlive)
                    //   skip;
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, player.LocalIndex),
                    new(OpCodes.Brfalse_S, skip),
                    new(OpCodes.Ldloc, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsAlive))),
                    new(OpCodes.Brfalse_S, skip),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}