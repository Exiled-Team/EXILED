// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Respawning;
    using Respawning.NamingRules;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patch the <see cref="RespawnManager.Spawn"/>.
    /// Adds <see cref="Server.RespawningTeam"/> and <see cref="Server.DeployingTeamRole"/> events.
    /// </summary>
    [EventPatch(typeof(Server), nameof(Server.RespawningTeam))]
    [EventPatch(typeof(Server), nameof(Server.DeployingTeamRole))]
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class RespawningTeam
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder preRespawningEv = generator.DeclareLocal(typeof(PreRespawningTeamEventArgs));
            LocalBuilder respawningEv = generator.DeclareLocal(typeof(RespawningTeamEventArgs));
            LocalBuilder deployingEv = generator.DeclareLocal(typeof(DeployingTeamRoleEventArgs));

            Label continueLabel = generator.DefineLabel();
            Label ret = generator.DefineLabel();
            Label jne = generator.DefineLabel();

            int offset = -7;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Sub) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // preRespawningEv = new PreRespawningTeamEventArgs(SpawnableTeamHandlerBase, int, SpawnableTeamType, bool)
                // Handlers.Server.OnPreRespawningTeam(preRespawningEv)
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PreRespawningTeamEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, preRespawningEv.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnPreRespawningTeam))),

                // if (!preRespawningEv.IsAllowed)
                // goto ret
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreRespawningTeamEventArgs), nameof(PreRespawningTeamEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),

                // SpawnableTeamHandlerBase = preRespawningEv.SpawnableTeamHandler
                new(OpCodes.Ldloc_S, preRespawningEv.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreRespawningTeamEventArgs), nameof(PreRespawningTeamEventArgs.SpawnableTeamHandler))),
                new(OpCodes.Stloc_0),

                // this.NextKnownTeam = preRespawningEv.NextKnownTeam
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, preRespawningEv.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreRespawningTeamEventArgs), nameof(PreRespawningTeamEventArgs.NextKnownTeam))),
                new(OpCodes.Stfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),

                // maxWaveSize = preRespawningEv.MaxWaveSize
                new(OpCodes.Ldloc_S, preRespawningEv.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(PreRespawningTeamEventArgs), nameof(PreRespawningTeamEventArgs.MaxWaveSize))),
                new(OpCodes.Stloc_2),
            });

            offset = -6;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(UnitNamingRule), nameof(UnitNamingRule.TryGetNamingRule)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // GetPlayers(list);
                new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetPlayers))),

                // maxWaveSize
                new(OpCodes.Ldloc_2),

                // this.NextKnownTeam
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),

                // true
                new(OpCodes.Ldc_I4_1),

                // RespawningTeamEventArgs respawningEv = new(players, num, this.NextKnownTeam)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RespawningTeamEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, respawningEv.LocalIndex),

                // Handlers.Server.OnRespawningTeam(respawningEv)
                new(OpCodes.Call, Method(typeof(Server), nameof(Server.OnRespawningTeam))),

                // if (respawningEv.IsAllowed)
                //    goto continueLabel;
                new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new CodeInstruction(OpCodes.Ldloc_S, respawningEv.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Dup),

                // list = GetHubs(ev.Players)
                new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.Players))),
                new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetHubs))),
                new(OpCodes.Stloc_1),

                // queueToFill = ev.SpawnQueue;
                new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.SpawnQueue))),
                new(OpCodes.Stloc, 7),
            });

            offset = -6;
            newInstructions.RemoveRange(newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == Method(typeof(SpawnableTeamHandlerBase), nameof(SpawnableTeamHandlerBase.GenerateQueue))) + offset, 7);

            offset = -3;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_M1) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, 10),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DeployingTeamRoleEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnDeployingTeamRole))),
                new(OpCodes.Stloc_S, deployingEv.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DeployingTeamRoleEventArgs), nameof(DeployingTeamRoleEventArgs.Role))),
                new(OpCodes.Ldloc_S, deployingEv.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DeployingTeamRoleEventArgs), nameof(DeployingTeamRoleEventArgs.Delegate))),
                new(OpCodes.Callvirt, Method(typeof(Action), nameof(Action.Invoke))),
                new(OpCodes.Ldloc_S, deployingEv.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DeployingTeamRoleEventArgs), nameof(DeployingTeamRoleEventArgs.IsReliable))),
                new(OpCodes.Brfalse_S, jne),
            });

            offset = 5;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_M1) + offset;
            newInstructions[index].labels.Add(jne);

            offset = -3;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldc_I4_M1) + offset;

            newInstructions.RemoveRange(index, 5);

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static List<Player> GetPlayers(List<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();

        private static List<ReferenceHub> GetHubs(List<Player> players) => players.Select(player => player.ReferenceHub).ToList();
    }
}