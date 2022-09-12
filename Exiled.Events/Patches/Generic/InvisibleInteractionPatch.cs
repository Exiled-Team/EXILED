// -----------------------------------------------------------------------
// <copyright file="InvisibleInteractionPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using Exiled.API.Features;

    using HarmonyLib;

    using Interactables;
    using Interactables.Verification;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="StandardDistanceVerification.ServerCanInteract(ReferenceHub, InteractableCollider)"/>.
    /// Implements <see cref="Player.KeepInvisibilityOnInteracting"/> property logic.
    /// </summary>
    [HarmonyPatch(typeof(StandardDistanceVerification), nameof(StandardDistanceVerification.ServerCanInteract))]
    internal static class InvisibleInteractionPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -1;
            int index = newInstructions.FindIndex(x => x.LoadsField(Field(typeof(StandardDistanceVerification), nameof(StandardDistanceVerification._cancel268)))) + offset;

            Label skipEffectChangeLabel = generator.DefineLabel();
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, skipEffectChangeLabel),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.KeepInvisibilityOnInteracting))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
            });

            newInstructions[newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldc_I4_1)].labels.Add(skipEffectChangeLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="PlayerInteract.OnInteract"/>.
    /// Implements <see cref="Player.KeepInvisibilityOnInteracting"/> property logic.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.OnInteract))]
    internal static class InvisibleInteractionPlayerInteractPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -3;
            int index = newInstructions.FindIndex(x => x.Calls(PropertySetter(typeof(PlayerEffect), nameof(PlayerEffect.Intensity)))) + offset;

            Label label = generator.DefineLabel();
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, label),
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.KeepInvisibilityOnInteracting))),
                new CodeInstruction(OpCodes.Brtrue_S, label),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(label);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
