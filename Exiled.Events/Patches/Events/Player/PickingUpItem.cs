// -----------------------------------------------------------------------
// <copyright file="PickingUpItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ItemSearchCompletor.Complete"/>.
    /// Adds the <see cref="Handlers.Player.PickingUpItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickingUpItem {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();
            Label cdcLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(PickingUpItemEventArgs));

            newInstructions[0].labels.Add(cdcLabel);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpItemEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPickingUpItem))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(ItemPickupBase), nameof(ItemPickupBase.Info))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpItemEventArgs), nameof(PickingUpItemEventArgs.Pickup))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Items.Pickup), nameof(API.Features.Items.Pickup.Serial))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(PickupSyncInfo), nameof(PickupSyncInfo.Serial))),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpItemEventArgs), nameof(PickingUpItemEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue_S, cdcLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
                new CodeInstruction(OpCodes.Ldflda, Field(typeof(ItemPickupBase), nameof(ItemPickupBase.Info))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(PickupSyncInfo), nameof(PickupSyncInfo.InUse))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ItemPickupBase), nameof(ItemPickupBase.Info))),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(ItemPickupBase), nameof(ItemPickupBase.NetworkInfo))),
                new CodeInstruction(OpCodes.Br, retLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
