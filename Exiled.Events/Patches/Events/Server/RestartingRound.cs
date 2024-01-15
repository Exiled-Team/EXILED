// -----------------------------------------------------------------------
// <copyright file="RestartingRound.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using GameCore;

    using HarmonyLib;

    using RoundRestarting;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoundRestart.InitiateRoundRestart"/>.
    /// Adds the <see cref="Handlers.Server.RestartingRound" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.RestartingRound))]
    [HarmonyPatch(typeof(RoundRestart), nameof(RoundRestart.InitiateRoundRestart))]
    internal static class RestartingRound
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Handlers.Server.OnRestartingRound()
                    new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRestartingRound))),

                    // API.Features.Log.Debug("Round restarting", Loader.ShouldDebugBeShown)
                    new(OpCodes.Ldstr, "Round restarting"),
                    new(OpCodes.Call, Method(typeof(API.Features.Log), nameof(API.Features.Log.Debug), new[] { typeof(string) })),
                });

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse);

            newInstructions.InsertRange(
                index + offset,
                new CodeInstruction[]
                {
                    // if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Restart)  -> goto normal round restart
                    new(OpCodes.Call, PropertyGetter(typeof(ServerStatic), nameof(ServerStatic.StopNextRound))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Beq_S, newInstructions[index].operand),

                    // if (ShouldServerRestart()) -> goto normal round restart
                    new(OpCodes.Call, Method(typeof(RestartingRound), nameof(ShouldServerRestart))),
                    new(OpCodes.Brtrue, newInstructions[index].operand),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
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