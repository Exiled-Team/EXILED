// -----------------------------------------------------------------------
// <copyright file="StartingByServer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="AlphaWarheadController.Update"/>.
    /// Adds the <see cref="Handlers.Warhead.Starting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Update))]
    internal static class StartingByServer
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = -3;

            // Search for the only call AlphaWarheadController.StartDetonation
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))) + offset;

            // Get the count to find the previous index
            int oldCount = newInstructions.Count;

            // Define a return label for us to use.
            Label returnLabel = generator.DefineLabel();

            // var ev = new StartingEventArgs(Server.Host, true);
            //
            // Handlers.Warhead.OnStarting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(Server), nameof(Server.Host))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Warhead), nameof(Handlers.Warhead.OnStarting))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StartingEventArgs), nameof(StartingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            // Add the starting labels to the first injected instruction.
            // Calculate the difference and get the valid index - is better and easy than using a list
            newInstructions[index].MoveLabelsFrom(newInstructions[newInstructions.Count - oldCount + index]);

            // Add our return label to the method's natural ret instruction.
            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
