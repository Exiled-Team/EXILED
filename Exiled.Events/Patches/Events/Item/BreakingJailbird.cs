// -----------------------------------------------------------------------
// <copyright file="BreakingJailbird.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;
    using HarmonyLib;
    using InventorySystem.Items.Jailbird;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="JailbirdDeteriorationTracker.RecheckUsage"/>
    /// to add <see cref="Handlers.Item.BreakingJailbird"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Item), nameof(Handlers.Item.BreakingJailbird))]
    [HarmonyPatch(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker.RecheckUsage))]
    internal class BreakingJailbird
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldsfld);

            Label continueLabel = generator.DefineLabel();

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (jailbirdWearState3 != JailbirdWearState.Broken)
                    //    goto continueLabel;
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Ldc_I4_5),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // this._jailbird
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(JailbirdDeteriorationTracker), nameof(JailbirdDeteriorationTracker._jailbird))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // BreakingJailbirdEventArgs = new(JailbirdItem, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BreakingJailbirdEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Item.OnBreakingJailbird(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnBreakingJailbird))),

                    // if (ev.IsAllowed)
                    //    goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BreakingJailbirdEventArgs), nameof(BreakingJailbirdEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // jailbirdWearState3 = JailbirdWearState.AlmostBroken;
                    new(OpCodes.Ldc_I4_4),
                    new(OpCodes.Stloc_2),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}