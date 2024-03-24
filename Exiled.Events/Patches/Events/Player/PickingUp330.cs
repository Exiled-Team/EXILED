// -----------------------------------------------------------------------
// <copyright file="PickingUp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    /// Patches the <see cref="Scp330Bag.ServerProcessPickup" /> method to add the
    /// <see cref="Handlers.Player.PickingUpItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PickingUpItem))]
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup))]
    internal static class PickingUp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(PickingUpItemEventArgs));

            Label continueLabel = generator.DefineLabel();

            // We put the event code right at the beginning of the method.
            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (pickup == null)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Brfalse, continueLabel),

                    // Player.Get(ply)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // pickup
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PickingUpItemEventArgs ev = new(Player.Get(ply), pickup, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Scp330.OnPickingUpScp330(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPickingUpItem))),

                    // if (!ev.IsAllowed)
                    //    return false;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpItemEventArgs), nameof(PickingUpItemEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue, continueLabel),

                    // We need to load false onto the stack before returning, since the method returns a bool.
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ret),
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}