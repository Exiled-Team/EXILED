// -----------------------------------------------------------------------
// <copyright file="DroppingAmmo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Firearms.Ammo;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Inventory.UserCode_CmdDropAmmo__Byte__UInt16" />.
    ///     <br>Adds the <see cref="Handlers.Player.DroppingAmmo" /> and <see cref="Handlers.Player.DroppedAmmo" /> events.</br>
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DroppingAmmo))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DroppedAmmo))]
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropAmmo__Byte__UInt16))]
    internal static class DroppingAmmo
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ammoPickups = generator.DeclareLocal(typeof(List<AmmoPickup>));
            LocalBuilder ev = generator.DeclareLocal(typeof(DroppingAmmoEventArgs));

            Label returnLabel = generator.DefineLabel();

            newInstructions.Remove(newInstructions.First(c => c.opcode == OpCodes.Pop));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(ReferenceHub);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ammoType
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Extensions.ItemExtensions), nameof(API.Extensions.ItemExtensions.GetAmmoType))),

                // amount
                new(OpCodes.Ldarg_2),

                // true
                new(OpCodes.Ldc_I4_1),

                // DroppingAmmoEventArgs ev = new(Player, AmmoType, ushort, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingAmmoEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Player.OnDroppingAmmo(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDroppingAmmo))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingAmmoEventArgs), nameof(DroppingAmmoEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // ammoPickups = InventorySystem.InventoryExtensions::ServerDropAmmo
                new(OpCodes.Stloc_S, ammoPickups.LocalIndex),

                // ev.Player
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingAmmoEventArgs), nameof(DroppingAmmoEventArgs.Player))),

                // ev.AmmoType
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingAmmoEventArgs), nameof(DroppingAmmoEventArgs.AmmoType))),

                // ev.Amount
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingAmmoEventArgs), nameof(DroppingAmmoEventArgs.Amount))),

                // ammoPickups
                new(OpCodes.Ldloc_S, ammoPickups.LocalIndex),

                // Handlers::Player::OnDroppedItem(new DroppedAmmoEventArgs(ev.Player, ev.AmmoType, ev.Amount, ammoPickups))
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppedAmmoEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDroppedItem))),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}