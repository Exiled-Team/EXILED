// -----------------------------------------------------------------------
// <copyright file="Escaping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1402 // File may only contain a single type

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Enums;
    using API.Features;
    using API.Features.Pools;

    using EventArgs.Player;

    using HarmonyLib;

    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Escape.ServerHandlePlayer(ReferenceHub)"/> for <see cref="Handlers.Player.Escaping" />.
    /// </summary>
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerHandlePlayer))]
    internal static class Escaping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingEventArgs));

            LocalBuilder teamToGrantTickets = generator.DeclareLocal(typeof(SpawnableTeamType));
            LocalBuilder ticketsToGrant = generator.DeclareLocal(typeof(float));

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldc_I4_M1),
                new (OpCodes.Stloc_S, teamToGrantTickets.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_M1),
                new (OpCodes.Stloc_S, teamToGrantTickets.LocalIndex),
            });

            // prevent calling RespawnTokensManager.GrantTokens, but save the values
            for (int i = newInstructions.Count - 1; i >= 0; --i)
            {
                CodeInstruction instruction = newInstructions[i];
                if (instruction.opcode != OpCodes.Call ||
                    instruction.operand is not MethodBase { Name: nameof(RespawnTokensManager.GrantTokens) })
                    continue;

                newInstructions.RemoveAt(i);
                newInstructions.InsertRange(i, new[]
                {
                    new CodeInstruction(OpCodes.Stloc, ticketsToGrant.LocalIndex),
                    new (OpCodes.Stloc, teamToGrantTickets.LocalIndex),
                });
            }

            int offset = -2;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

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
                    new(OpCodes.Ldloc_S, teamToGrantTickets.LocalIndex),

                    // ticketsToGrant
                    new(OpCodes.Ldloc_S, ticketsToGrant.LocalIndex),

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

                    // GrantAllTickets(ev)
                    new(OpCodes.Ldloc, ev.LocalIndex),
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
    [HarmonyPatch(typeof(Escape), nameof(Escape.ServerGetScenario))]
    internal static class GetScenario
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int e = 0;
            for (int i = 0; i < newInstructions.Count; i++)
            {
                CodeInstruction codeInstruction = newInstructions[i];
                if (codeInstruction.opcode == OpCodes.Ldc_I4_0)
                {
                    e++;
                    if (e > 3)
                    {
                        newInstructions[i].opcode = OpCodes.Ldc_I4_5;
                    }
                }
            }

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}