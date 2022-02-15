// -----------------------------------------------------------------------
// <copyright file="GeneratorActivated.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Generator.Engaged"/>.
    /// Adds the <see cref="Map.GeneratorActivated"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.Engaged), MethodType.Setter)]
    internal static class GeneratorActivated
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Search for the third "ldarg.0".
            const int index = 0;
            Label retModLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(GeneratorActivatedEventArgs));

            // var ev = new GeneratorActivatedEventArgs(this, true);
            //
            // Map.OnGeneratorActivated(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(GeneratorActivatedEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnGeneratorActivated))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GeneratorActivatedEventArgs), nameof(GeneratorActivatedEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, retModLabel),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex).WithLabels(retModLabel),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GeneratorActivatedEventArgs), nameof(GeneratorActivatedEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue, returnLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079Generator), nameof(Scp079Generator._leverStopwatch))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Stopwatch), nameof(Stopwatch.Restart))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
