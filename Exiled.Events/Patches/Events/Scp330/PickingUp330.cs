// -----------------------------------------------------------------------
// <copyright file="PickingUp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Scp330;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patches the <see cref="Scp330Bag.ServerProcessPickup" /> method to add the
    ///     <see cref="Handlers.Scp330.PickingUpScp330" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup))]
    internal static class PickingUp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            LocalBuilder ev = generator.DeclareLocal(typeof(PickingUpScp330EventArgs));
            Label continueLabel = generator.DefineLabel();

            // We put the event code right at the beginning of the method.
            newInstructions.InsertRange(0, new[]
            {
                // var ev = new PickingUpScp330EventArgs(Player.Get(ply), pickup);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Brfalse, continueLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpScp330EventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Scp330.OnPickingUpScp330(ev);
                new(OpCodes.Call, Method(typeof(Scp330), nameof(Scp330.OnPickingUp330))),

                // if (!ev.IsAllowed)
                //    return false;
                new(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpScp330EventArgs), nameof(PickingUpScp330EventArgs.IsAllowed))),
                new(OpCodes.Brtrue, continueLabel),

                // We need to load false onto the stack before returning, since the method returns a bool.
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ret),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++) yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
