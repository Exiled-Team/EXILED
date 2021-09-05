// -----------------------------------------------------------------------
// <copyright file="InteractingShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
#pragma warning disable SA1313
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Utilities;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ShootingTarget.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Handlers.Player.InteractingShootingTarget"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.ServerInteract))]
    internal static class InteractingShootingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -7;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldarg_2) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldarg_0),

                new CodeInstruction(OpCodes.Ldarg_2),

                new CodeInstruction(OpCodes.Ldc_I4_1),

                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingShootingTargetEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingShootingTarget))),

                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingShootingTargetEventArgs), nameof(InteractingShootingTargetEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
