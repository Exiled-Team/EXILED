// -----------------------------------------------------------------------
// <copyright file="ChangingAmmo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Item;

    using Handlers;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Firearm.Status" />.
    /// Adds the <see cref="Item.ChangingAmmo" /> event.
    /// </summary>
    [EventPatch(typeof(Item), nameof(Item.ChangingAmmo))]
    [HarmonyPatch(typeof(Firearm), nameof(Firearm.Status), MethodType.Setter)]
    internal static class ChangingAmmo
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 3;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAmmoEventArgs));
            LocalBuilder ammo = generator.DeclareLocal(typeof(byte));

            Label cdc = generator.DefineLabel();
            Label jmp = generator.DefineLabel();
            Label jcc = generator.DefineLabel();

            newInstructions[index].labels.Add(cdc);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // value.Ammo
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),

                    // this._status.Ammo
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),

                    // if (value.Ammo == this._status.Ammo)
                    //   goto cdc;
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, cdc),

                    // this
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Dup),

                    // this._status
                    new(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),

                    // this.Ammo
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),

                    // value.Ammo
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingDurabilityEventArgs ev = new(ItemBase, byte, byte, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAmmoEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Item.ChangingAmmoEventArgs(ev)
                    new(OpCodes.Call, Method(typeof(Item), nameof(Item.OnChangingAmmo))),

                    // if (ev.Firearm == null)
                    //   goto cdc;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAmmoEventArgs), nameof(ChangingAmmoEventArgs.Firearm))),
                    new(OpCodes.Brfalse_S, cdc),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),

                    // if (!ev.IsAllowed)
                    //   goto jmp;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAmmoEventArgs), nameof(ChangingAmmoEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, jmp),

                    // ev.NewDurability
                    // goto jcc;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAmmoEventArgs), nameof(ChangingAmmoEventArgs.NewAmmo))),
                    new(OpCodes.Stloc_S, ammo.LocalIndex),
                    new(OpCodes.Br_S, jcc),

                    // ev.OldAmmo
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jmp),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAmmoEventArgs), nameof(ChangingAmmoEventArgs.OldAmmo))),
                    new(OpCodes.Stloc_S, ammo.LocalIndex),

                    // this
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(jcc),

                    // ammo
                    new(OpCodes.Ldloc_S, ammo.LocalIndex),

                    // value.Flags
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Flags))),

                    // value.Attachments
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Attachments))),

                    // this.value = new(byte, FirearmStatusFlags, uint)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(FirearmStatus))[0]),
                    new(OpCodes.Stfld, Field(typeof(Firearm), nameof(Firearm._status))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}