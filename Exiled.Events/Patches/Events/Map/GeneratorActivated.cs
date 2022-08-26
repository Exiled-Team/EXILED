// -----------------------------------------------------------------------
// <copyright file="GeneratorActivated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp079Generator.Engaged" />.
    ///     Adds the <see cref="Map.GeneratorActivated" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.GeneratorActivated))]
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
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GeneratorActivatedEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnGeneratorActivated))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratorActivatedEventArgs), nameof(GeneratorActivatedEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retModLabel),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex).WithLabels(retModLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GeneratorActivatedEventArgs), nameof(GeneratorActivatedEventArgs.IsAllowed))),
                new(OpCodes.Brtrue, returnLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp079Generator), nameof(Scp079Generator._leverStopwatch))),
                new(OpCodes.Callvirt, Method(typeof(Stopwatch), nameof(Stopwatch.Restart))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
