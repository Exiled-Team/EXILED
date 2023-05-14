// -----------------------------------------------------------------------
// <copyright file="DroppingAmmo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Inventory.UserCode_CmdDropAmmo__Byte__UInt16" />.
    ///     Adds the <see cref="DroppingAmmo" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropAmmo__Byte__UInt16))]
    internal static class DroppingAmmo
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player.Get(ReferenceHub);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // ammoType
                    new(OpCodes.Ldarg_1),

                    // amount
                    new(OpCodes.Ldarg_2),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // DroppingAmmoEventArgs ev = new(Player, AmmoType, ushort, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingAmmoEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnDroppingAmmo(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDroppingAmmo))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingAmmoEventArgs), nameof(DroppingAmmoEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}