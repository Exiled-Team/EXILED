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

    using API.Features.Core.Generic.Pools;
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

            const int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingAmmoEventArgs));

            Label ret = generator.DefineLabel();
            Label jump = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // value.Ammo
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),

                    // this._status.Ammo
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),
                    new(OpCodes.Ldfld, Field(typeof(FirearmStatus), nameof(FirearmStatus.Ammo))),

                    // if (value.Ammo == this._status.Ammo)
                    //   goto jump;
                    new(OpCodes.Ceq),
                    new(OpCodes.Brtrue_S, jump),

                    // this
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Dup),

                    // this._status (oldStatus)
                    new(OpCodes.Ldfld, Field(typeof(Firearm), nameof(Firearm._status))),

                    // value (newStatus)
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingAmmoEventArgs ev = new(BaseFirearm, FirearmStatus, FirearmStatus, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAmmoEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Item.OnChangingAmmo(ev)
                    new(OpCodes.Call, Method(typeof(Item), nameof(Item.OnChangingAmmo))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAmmoEventArgs), nameof(ChangingAmmoEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // value = ev.NewStatus;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAmmoEventArgs), nameof(ChangingAmmoEventArgs.NewStatus))),
                    new(OpCodes.Starg_S, 1),

                    // jump:
                    new CodeInstruction(OpCodes.Nop).WithLabels(jump),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
