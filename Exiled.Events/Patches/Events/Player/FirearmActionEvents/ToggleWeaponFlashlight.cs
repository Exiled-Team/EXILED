// -----------------------------------------------------------------------
// <copyright file="ToggleWeaponFlashlight.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player.FirearmActionEvents
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Attachments;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FlashlightAttachment.ServerSendStatus" />.
    /// Adds <see cref="Handlers.Player.TogglingWeaponFlashlight" /> & <see cref="Handlers.Player.ToggledWeaponFlashlight" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.TogglingWeaponFlashlight))]
    [HarmonyPatch(typeof(FlashlightAttachment), nameof(FlashlightAttachment.ServerSendStatus))]
    internal static class ToggleWeaponFlashlight
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnToggleWeaponFlashlight(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(TogglingWeaponFlashlightEventArgs));
            LocalBuilder firearm = generator.DeclareLocal(typeof(Firearm));
            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder status = generator.DeclareLocal(typeof(bool));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // Player.Get(this.Firearm.Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FlashlightAttachment), nameof(FlashlightAttachment.Firearm))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(InventorySystem.Items.Firearms.Firearm), nameof(InventorySystem.Items.Firearms.Firearm.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Stloc_S, player.LocalIndex),

                // (Firearm)Item.Get(this.Firearm)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(FlashlightAttachment), nameof(FlashlightAttachment.Firearm))),
                new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(InventorySystem.Items.Firearms.Firearm) })),
                new(OpCodes.Castclass, typeof(Firearm)),
                new(OpCodes.Stloc_S, firearm.LocalIndex),

                // status
                new(OpCodes.Ldarg_1),

                // true
                new(OpCodes.Ldc_I4_1),

                // TogglingWeaponFlashlightEventArgs args = new(Player, Firearm, bool, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TogglingWeaponFlashlightEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnTogglingWeaponFlashlight))),

                // if (!args.IsAllowed)
                //     return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), nameof(TogglingWeaponFlashlightEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // status = NewState
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(TogglingWeaponFlashlightEventArgs), nameof(TogglingWeaponFlashlightEventArgs.NewState))),
                new(OpCodes.Starg_S, 1),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Stloc_S, status.LocalIndex),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // Player
                new(OpCodes.Ldloc_S, player.LocalIndex),

                // Firearm
                new(OpCodes.Ldloc_S, firearm.LocalIndex),

                // status
                new(OpCodes.Ldloc_S, status.LocalIndex),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ToggledWeaponFlashlightEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnToggledWeaponFlashlight))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}