// -----------------------------------------------------------------------
// <copyright file="Escaping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable IDE0060

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Enums;
    using API.Features;
    using API.Features.Core.Generic.Pools;

    using EventArgs.Player;
    using Exiled.Events.Attributes;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Escape.ServerHandlePlayer(ReferenceHub)"/> for <see cref="Handlers.Player.Escaping" />.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Escaping))]
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerHandlePlayer))]
    internal static class Escaping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            List<Label> labels;
            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingEventArgs));

            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // roleTypeId
                    new(OpCodes.Ldloc_0),

                    // escapeScenario
                    new(OpCodes.Ldloc_1),

                    // teamToGrantTickets
                    new(OpCodes.Ldloc_2),

                    // ticketsToGrant
                    new(OpCodes.Ldloc_3),

                    // EscapingEventArgs ev = new(Player, RoleTypeId, EscapeScenario, SpawnableTeamType, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingEventArgs)).First(cctor => cctor.GetParameters().Any(param => param.ParameterType == typeof(SpawnableTeamType)))),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnEscaping(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscaping))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, returnLabel),

                    // roleTypeId = ev.NewRole
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingEventArgs), nameof(EscapingEventArgs.NewRole))),
                    new(OpCodes.Stloc_0),
                });

            // replace base-game grant token logic
            offset = -2;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(RespawnTokensManager), nameof(RespawnTokensManager.GrantTokens)))) + offset;
            labels = newInstructions[index].ExtractLabels();
            newInstructions.RemoveRange(index, 3);
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // GrantAllTickets(ev)
                    new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex).WithLabels(labels),
                    new(OpCodes.Call, Method(typeof(Escaping), nameof(GrantAllTickets))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void GrantAllTickets(EscapingEventArgs ev)
        {
            if (Enum.IsDefined(typeof(SpawnableTeamType), ev.RespawnTickets.Key) && ev.RespawnTickets.Key != SpawnableTeamType.None)
                RespawnTokensManager.ModifyTokens(ev.RespawnTickets.Key, ev.RespawnTickets.Value);
        }
    }

    /// <summary>
    /// Patches <see cref="Escape.ServerGetScenario(ReferenceHub)"/> for <see cref="Handlers.Player.Escaping"/>.
    /// Replaces last returned <see cref="EscapeScenario.None"/> to <see cref="EscapeScenario.CustomEscape"/>.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Escaping))]
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerGetScenario))]
    internal static class GetScenario
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder fpcRole = generator.DeclareLocal(typeof(FpcStandardRoleBase));

            // replace HumanRole to FpcStandardRoleBase
            newInstructions.Find(x => x.opcode == OpCodes.Isinst).operand = typeof(FpcStandardRoleBase);

            // after this index all invalid exit are considered Custom
            int customExit = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0);
            for (int i = 0; i < newInstructions.Count; i++)
            {
                OpCode opcode = newInstructions[i].opcode;
                if (opcode == OpCodes.Stloc_0)
                    newInstructions[i] = new CodeInstruction(OpCodes.Stloc_S, fpcRole.LocalIndex).WithLabels(newInstructions[i].labels);
                else if (opcode == OpCodes.Ldloc_0)
                    newInstructions[i] = new CodeInstruction(OpCodes.Ldloc_S, fpcRole.LocalIndex).WithLabels(newInstructions[i].labels);
                else if (opcode == OpCodes.Ldc_I4_0 && i > customExit)
                    newInstructions[i].opcode = OpCodes.Ldc_I4_5;
            }

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
