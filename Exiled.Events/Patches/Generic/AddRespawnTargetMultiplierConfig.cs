// -----------------------------------------------------------------------
// <copyright file="AddRespawnTargetMultiplierConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic.Scp079API
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoundSummary.ServerOnRespawned(Respawning.SpawnableTeamType, List{ReferenceHub})"/>.
    /// Adds the <see cref="RoundSummary.RespawnTargetMultiplier" /> as NW config.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.ServerOnRespawned))]
    internal class AddRespawnTargetMultiplierConfig
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // replace "this.ChaosTargetCount += (int)((double)respawnedPlayers.Count * 0.75);"
            // with " this.ChaosTargetCount += (int)((double)respawnedPlayers.Count * ConfigFile.ServerConfig.GetDouble("respawn_target_multiplier", 0.75);"
            int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_R8) + offset;
            newInstructions.RemoveAt(index);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ConfigFile.ServerConfig.GetDouble("respawn_target_multiplier", 0.75);
                    new(OpCodes.Ldsfld, Field(typeof(GameCore.ConfigFile), nameof(GameCore.ConfigFile.ServerConfig))),
                    new(OpCodes.Ldstr, "respawn_target_multiplier"),
                    new(OpCodes.Ldc_R8, RoundSummary.RespawnTargetMultiplier),
                    new(OpCodes.Call, Method(typeof(YamlConfig), nameof(YamlConfig.GetDouble))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}