// -----------------------------------------------------------------------
// <copyright file="StartingAndFinishingRecall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp049
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayableScps;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp049.BodyCmd_ByteAndGameObject(byte, GameObject)"/>.
    /// Adds the <see cref="Handlers.Scp049.StartingRecall"/> and <see cref="Handlers.Scp049.FinishingRecall"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp049), nameof(Scp049.BodyCmd_ByteAndGameObject))]
    internal static class StartingAndFinishingRecall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            var offset = -7;

            // Search for the first "referenceHub2.gameObject", then subtract the offset to get "ldstr".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt &&
            (MethodInfo)instruction.operand == PropertyGetter(typeof(Component), nameof(Component.gameObject))) + offset;

            // Get the first label of the last "ret".
            var returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // var ev = new StartingRecallEventArgs(Player.Get(referenceHub2), Player.Get(this.Hub), true);
            //
            // Handlers.Scp049.OnStartingRecall(ev)
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(referenceHub2)
                new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(this.Hub)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp049), nameof(Scp049.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new StartingRecallEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingRecallEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Scp049.OnStartingRecall(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnStartingRecall))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(StartingRecallEventArgs), nameof(StartingRecallEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            offset = -5;

            // Search for the first "RoundSummary.changed_into_zombies++", then subtract the offset to get "ldstr".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldsfld &&
            (FieldInfo)instruction.operand == Field(typeof(RoundSummary), nameof(RoundSummary.changed_into_zombies))) + offset;

            // var ev = new FinishingRecallEventArgs(Player.Get(referenceHub), Player.Get(this.Hub), true);
            //
            // Handlers.Scp049.OnFinishingRecall(ev)
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(referenceHub)
                new CodeInstruction(OpCodes.Ldloc_S, 8).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(this.Hub)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp049), nameof(Scp049.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new FinishingRecallEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(FinishingRecallEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Scp049.OnFinishingRecall(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp049), nameof(Handlers.Scp049.OnFinishingRecall))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FinishingRecallEventArgs), nameof(FinishingRecallEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
