// -----------------------------------------------------------------------
// <copyright file="DamagingShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using BaseTarget = InventorySystem.Items.Firearms.Utilities.ShootingTarget;

    /// <summary>
    /// Patches <see cref="BaseTarget.Damage(float, InventorySystem.Items.IDamageDealer, Footprinting.Footprint, UnityEngine.Vector3)"/>.
    /// Adds the <see cref="Handlers.Player.DamagingShootingTarget"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BaseTarget), nameof(BaseTarget.Damage))]
    internal static class DamagingShootingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_0) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(ReferenceHub)
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // item
                new CodeInstruction(OpCodes.Ldarg_2),

                // attackerFootprint
                new CodeInstruction(OpCodes.Ldarg_3),

                // hitLocation
                new CodeInstruction(OpCodes.Ldarg, 4),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // OnDamagingShootingTarget(DamagingShootingTargetEventArgs)
                // if (!IsAllowed)
                //   return
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingShootingTargetEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDamagingShootingTarget))),

                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DamagingShootingTargetEventArgs), nameof(DamagingShootingTargetEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
