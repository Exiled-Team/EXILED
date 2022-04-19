// -----------------------------------------------------------------------
// <copyright file="RestartingRound.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using GameCore;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using RoundRestarting;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoundRestart.InitiateRoundRestart"/>.
    /// Adds the RestartingRound event.
    /// </summary>
    [HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))]
    internal static class RestartingRound
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRestartingRound))),
                new(OpCodes.Call, Method(typeof(RestartingRound), nameof(RestartingRound.ShowDebugLine))),
            });

            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Brfalse);

            newInstructions.InsertRange(index + 1, new CodeInstruction[]
            {
                // ServerStatic.StopNextRound == 1 (restarting)
                new(OpCodes.Ldsfld, Field(typeof(ServerStatic), nameof(ServerStatic.StopNextRound))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ceq),

                // if (prev) -> goto normal round restart
                new(OpCodes.Brtrue, newInstructions[index].operand),

                // ShouldServerRestart()
                new(OpCodes.Call, Method(typeof(RestartingRound), nameof(RestartingRound.ShouldServerRestart))),

                // if (prev) -> goto normal round restart
                new(OpCodes.Brtrue, newInstructions[index].operand),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void ShowDebugLine()
        {
            API.Features.Log.Debug("Round restarting", Loader.Loader.ShouldDebugBeShown);
        }

        private static bool ShouldServerRestart()
        {
            bool flag = false;

            try
            {
                int num = ConfigFile.ServerConfig.GetInt("restart_after_rounds");
                flag = num > 0 && RoundRestart.UptimeRounds >= num;
            }
            catch (Exception ex)
            {
                ServerConsole.AddLog("Failed to check the restart_after_rounds config value: " + ex.Message, ConsoleColor.Red);
            }

            return flag;
        }
    }
}
