// -----------------------------------------------------------------------
// <copyright file="ExplodingFragGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Grenades;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FragGrenade.ServersideExplosion()"/>.
    /// Adds the <see cref="Handlers.Map.OnExplodingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
    internal static class ExplodingFragGrenade
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Declare Dictionary<Player, float> local variable.
            var players = generator.DeclareLocal(typeof(Dictionary<Player, float>));

            // Declare Dictionary<Player, float>.Enumerator local variable.
            var playerEnumerator = generator.DeclareLocal(typeof(Dictionary<Player, float>.Enumerator));

            // Declare KeyValuePair<Player, float> local variable.
            var playerKeyValuePair = generator.DeclareLocal(typeof(KeyValuePair<Player, float>));

            // Define the return label.
            var returnLabel = generator.DefineLabel();

            // Define the foreach labels.
            var foreachFirstLabel = generator.DefineLabel();
            var foreachSecondLabel = generator.DefineLabel();

            // var players = Dictionary<Player, float>();
            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(Dictionary<Player, float>))[0]),
                new CodeInstruction(OpCodes.Stloc_S, players.LocalIndex),
            });

            ////////// ENABLE EFFECTS INSTRUCTIONS //////////

            var startOffset = -1;
            var finishOffset = 0;

            // Search for the first index of instructions to remove, inside the foreach.
            var firstIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld &&
            (FieldInfo)instruction.operand == Field(typeof(PlayerStats), nameof(PlayerStats.ccm))) + startOffset;

            // Search for the last index of instructions to remove, inside the foreach.
            var lastIndex = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt &&
            (MethodInfo)instruction.operand == Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.EnableEffect), new[] { typeof(PlayerEffect), typeof(float), typeof(bool) })) + finishOffset;

            // Save enable effects instructions and remove them all.
            var enableEffectsInstructions = newInstructions.GetRange(firstIndex, lastIndex - firstIndex + 1);
            newInstructions.RemoveRange(firstIndex, lastIndex - firstIndex + 1);

            // Clear all labels of the first instruction.
            enableEffectsInstructions[0].labels.Clear();

            // Save the label of the "brtrue.s" instruction, and change it to redirect to the new foreach.
            var oldForeachFirstLabel = enableEffectsInstructions[3].operand;
            enableEffectsInstructions[3].operand = foreachFirstLabel;

            /////////////// HURT INSTRUCTIONS ///////////////

            startOffset = 2;
            finishOffset = 1;

            // Search for the first index of instructions to remove, inside the foreach.
            firstIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(Physics), nameof(Physics.Linecast), new[] { typeof(Vector3), typeof(Vector3), typeof(int) })) + startOffset;

            // Search for the last index of instructions to remove, inside the foreach.
            lastIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt &&
            (MethodInfo)instruction.operand == Method(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer), new[] { typeof(PlayerStats.HitInfo), typeof(GameObject), typeof(bool) })) + finishOffset;

            // Redirect "br.s" instruction (break) to the old foreach.
            newInstructions[lastIndex + 1].operand = oldForeachFirstLabel;

            // Save "component2.HurtPlayer()" instructions and remove them all.
            var hurtPlayerInstructions = newInstructions.GetRange(firstIndex, lastIndex - firstIndex + 1);
            newInstructions.RemoveRange(firstIndex, lastIndex - firstIndex + 1);

            // players.Add(Player.Get(gameObject), damage);
            newInstructions.InsertRange(firstIndex, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, players.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, 11),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldloc_S, 13),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<Player, float>), nameof(Dictionary<Player, float>.Add), new[] { typeof(Player), typeof(float) })),
            });

            ///////////////////////////////////////////////

            // Get the index of the penultimate instruction;
            var index = newInstructions.Count - 2;

            // Get the count to find the previous index
            var oldCount = newInstructions.Count;

            // var ev = new ExplodingGrenadeEventArgs(players, true, grenadeGameObject, true);
            //
            // Handlers.Map.OnExplodingGrenade(ev);
            //
            // if (!ev.IsAllowed)
            //   return result;
            //
            // foreach (var player in players)
            // {
            //   foreachBody;
            //   hurtPlayerInstructions;
            //   enableEffectsInstructions;
            // }
            var explodingGrenadeEvent = new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Grenade), nameof(Grenade.throwerGameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Ldloc_S, players.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExplodingGrenadeEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnExplodingGrenade))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplodingGrenadeEventArgs), nameof(ExplodingGrenadeEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            };

            var foreachStart = new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, players.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<Player, float>), nameof(Dictionary<Player, float>.GetEnumerator))),
                new CodeInstruction(OpCodes.Stloc_S, playerEnumerator.LocalIndex),

                new CodeInstruction(OpCodes.Br_S, foreachFirstLabel).WithBlocks(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock)),
                new CodeInstruction(OpCodes.Ldloca_S, playerEnumerator.LocalIndex).WithLabels(foreachSecondLabel),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Dictionary<Player, float>.Enumerator), nameof(Dictionary<Player, float>.Enumerator.Current))),
                new CodeInstruction(OpCodes.Stloc_S, playerKeyValuePair.LocalIndex),
            };

            // Fill local variables of hurtPlayerInstructions
            //
            // damage = playerKeyValuePair.Value;
            // playerStats = playerKeyValuePair.Key.ReferenceHub.playerStats;
            // gameObject = playerKeyValuePair.Key.ReferenceHub.gameObject;
            var foreachBody = new[]
            {
                new CodeInstruction(OpCodes.Ldloca_S, playerKeyValuePair.LocalIndex),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(KeyValuePair<Player, float>), nameof(KeyValuePair<Player, float>.Value))),
                new CodeInstruction(OpCodes.Stloc_S, 13),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(KeyValuePair<Player, float>), nameof(KeyValuePair<Player, float>.Key))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.ReferenceHub))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerStats))),
                new CodeInstruction(OpCodes.Stloc_S, 12),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Stloc_S, 11),
            };

            var foreachEnd = new[]
            {
                new CodeInstruction(OpCodes.Ldloca_S, playerEnumerator.LocalIndex).WithLabels(foreachFirstLabel),
                new CodeInstruction(OpCodes.Call, Method(typeof(Dictionary<Player, float>.Enumerator), nameof(Dictionary<Player, float>.Enumerator.MoveNext))),
                new CodeInstruction(OpCodes.Brtrue_S, foreachSecondLabel),
                new CodeInstruction(OpCodes.Leave_S, returnLabel),

                // --- Clean up ---
                new CodeInstruction(OpCodes.Ldloca_S, playerEnumerator.LocalIndex).WithBlocks(new ExceptionBlock(ExceptionBlockType.BeginFinallyBlock)),
                new CodeInstruction(OpCodes.Constrained, typeof(Dictionary<Player, float>.Enumerator)),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(IDisposable), nameof(IDisposable.Dispose))),
                new CodeInstruction(OpCodes.Endfinally).WithBlocks(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock)),
            };

            // Insert all instructions.
            newInstructions.InsertRange(index, explodingGrenadeEvent
                .Concat(foreachStart)
                .Concat(foreachBody)
                .Concat(hurtPlayerInstructions)
                .Concat(enableEffectsInstructions)
                .Concat(foreachEnd));

            // Add the starting labels to the first injected instruction.
            // Calculate the difference and get the valid index - is better and easy than using a list
            newInstructions[index].MoveLabelsFrom(newInstructions[newInstructions.Count - oldCount + index]);

            // Add the return label to the penultimate instruction.
            newInstructions[newInstructions.Count - 2].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
