// -----------------------------------------------------------------------
// <copyright file="InteractingScp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Interactables.Interobjects;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp330Interobject.ServerInteract"/> method to add the <see cref="Handlers.Scp330.InteractingScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]
    internal static class InteractingScp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingScp330EventArgs));

            // Find the only ldnull and insert our event code 2 instructions above it.
            int offset = -1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldnull) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // var ev = new InteractingScp330EventArgs(Player.Get(ply), num);
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp330EventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Scp330.InInteractingScp330(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnInteractingScp330))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
            });

            // if (num > 2)  ->   if (ev.ShouldSever)
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldloc_2);

            // Remove existing if check instructions
            newInstructions.RemoveRange(index, 3);

            // Add our new instructions.
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldSever))),
                new(OpCodes.Brfalse, continueLabel),
            });

            // Find the instruction the base-code if check points to when false, and add our own label.
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldarg_0);
            newInstructions[index].labels.Add(continueLabel);

            // Add a return label to the end of the method.
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
