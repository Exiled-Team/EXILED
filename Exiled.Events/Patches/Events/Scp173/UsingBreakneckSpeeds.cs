// -----------------------------------------------------------------------
// <copyright file="UsingBreakneckSpeeds.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp173;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Scp173 = PlayableScps.Scp173;

    /// <summary>
    ///     Patches <see cref="PlayableScps.Scp173.ServerDoBreakneckSpeeds" />.
    ///     Adds the <see cref="Handlers.Scp173.UsingBreakneckSpeeds" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp173), nameof(Scp173.ServerDoBreakneckSpeeds))]
    internal static class UsingBreakneckSpeeds
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            int offset = -1;

            int index = newInstructions.FindIndex(
                instruction => (instruction.opcode == OpCodes.Ldfld) &&
                               ((FieldInfo)instruction.operand == Field(typeof(Scp173), nameof(Scp173._breakneckSpeedsCooldownRemaining)))) + offset;

            newInstructions.RemoveRange(index, 5);

            // var ev = new UsingBreakneckSpeedsEventArgs(Player, Scp173._breakneckSpeedsCooldownRemaining == 0);
            // Handlers.Scp173.OnUsingBreakneckSpeeds(ev);
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp173), nameof(Scp173.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Scp173._breakneckSpeedsCooldownRemaining == 0
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp173), nameof(Scp173._breakneckSpeedsCooldownRemaining))),
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Ceq),

                    // new UsingBreakneckSpeedsEventArgs(...)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingBreakneckSpeedsEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp173.OnUsingBreakneckSpeeds(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnUsingBreakneckSpeeds))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingBreakneckSpeedsEventArgs), nameof(UsingBreakneckSpeedsEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}