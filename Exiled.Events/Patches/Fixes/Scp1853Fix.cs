// -----------------------------------------------------------------------
// <copyright file="Scp1853Fix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;

    using CustomPlayerEffects;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using HarmonyLib;

    

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="Scp1853.OnEffectUpdate"/>.
    /// Fix Spamming EnableEffect.
    /// </summary>
    [HarmonyPatch(typeof(Scp1853), nameof(Scp1853.OnEffectUpdate))]
    internal static class Scp1853Fix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Brfalse_S) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (Player.Get(this.Hub).GetEffect(EffectType.Poisoned).IsEnabled)
                    //    return;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1853), nameof(Scp1853.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldc_I4, (int)EffectType.Poisoned),
                    new(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.GetEffect))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StatusEffectBase), nameof(StatusEffectBase.IsEnabled))),
                    new(OpCodes.Brtrue_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}