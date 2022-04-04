// -----------------------------------------------------------------------
// <copyright file="WeaponAttachmentDesyncFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AttachmentsServerHandler.SetupProvidedWeapon(ReferenceHub, InventorySystem.Items.ItemBase)"/>.
    /// Fixes if a plugin gives you an weapon that you do not have ammo for, your attachments will not correctly appear on said weapon.
    /// </summary>
    [HarmonyPatch(typeof(AttachmentsServerHandler), nameof(AttachmentsServerHandler.SetupProvidedWeapon))]
    internal static class WeaponAttachmentDesyncFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int insetIndex = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ret);
            int ifIndex = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_3) - 1;

            Label skipLabel = generator.DefineLabel();
            Label skipLabel2 = generator.DefineLabel();
            Label insertLabel = generator.DefineLabel();

            // Local.6(bool)
            LocalBuilder flagLocal = generator.DeclareLocal(typeof(bool));

            // if(ply.inventory.UserInventory.ReserveAmmo.TryGetValue(firearm.AmmoType, out num2))
            // {
            newInstructions[ifIndex].operand = insertLabel;

            // else
            // {
            //     bool isFlaghLightEnabled = firearm.CombinedAttachments.AdditionalPros.HasFlagFast(AttachmentDescriptiveAdvantages.Flashlight);
            //     firearm.Status = new FirearmStatus(0, isFlaghLightEnabled ? (FirearmStatusFlags.MagazineInserted | FirearmStatusFlags.FlashlightEnabled) : FirearmStatusFlags.MagazineInserted, num);
            // }
            // return;
            newInstructions.AddRange(new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).WithLabels(insertLabel),
                new(OpCodes.Ldfld, Field(typeof(InventorySystem.Items.Firearms.Firearm), nameof(InventorySystem.Items.Firearms.Firearm.CombinedAttachments))),
                new(OpCodes.Ldfld, Field(typeof(AttachmentSettings), nameof(AttachmentSettings.AdditionalPros))),
                new(OpCodes.Ldc_I4_2),
                new(OpCodes.Call, Method(typeof(AttachmentsUtils), nameof(AttachmentsUtils.HasFlagFast), new[] { typeof(AttachmentDescriptiveAdvantages), typeof(AttachmentDescriptiveAdvantages) })),
                new(OpCodes.Stloc_S, 6),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Ldloc_S, 6),
                new(OpCodes.Brtrue_S, skipLabel),
                new(OpCodes.Ldc_I4_4),
                new(OpCodes.Br_S, skipLabel2),
                new CodeInstruction(OpCodes.Ldc_I4_S, 12).WithLabels(skipLabel),
                new CodeInstruction(OpCodes.Ldloc_2).WithLabels(skipLabel2),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InventorySystem.Items.Firearms.FirearmStatus))[0]),
                new(OpCodes.Call, PropertySetter(typeof(InventorySystem.Items.Firearms.Firearm), nameof(InventorySystem.Items.Firearms.Firearm.Status))),
                new(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
