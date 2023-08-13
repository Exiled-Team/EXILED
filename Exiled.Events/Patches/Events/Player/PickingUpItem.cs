// -----------------------------------------------------------------------
// <copyright file="PickingUpItem.cs" company="Exiled Team">
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

    using InventorySystem.Items.Pickups;
    using InventorySystem.Searching;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ItemSearchCompletor.Complete" />.
    ///     Adds the <see cref="Handlers.Player.PickingUpItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickingUpItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            LocalBuilder info = generator.DeclareLocal(typeof(PickupSyncInfo));
            LocalBuilder ev = generator.DeclareLocal(typeof(PickingUpItemEventArgs));

            const int index = 0;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this.TargetPickup
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PickingUpItemEventArgs ev = new(Player, ItemPickupBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnPickingUpItem(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPickingUpItem))),

                    // if (ev.IsAllowed)
                    //    goto continueLabel;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpItemEventArgs), nameof(PickingUpItemEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // PickupSyncInfo info = this.TargetPickup.Info;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
                    new(OpCodes.Ldfld, Field(typeof(ItemPickupBase), nameof(ItemPickupBase.Info))),
                    new(OpCodes.Stloc_S, info.LocalIndex),

                    // info.InUse = false;
                    new(OpCodes.Ldloca_S, info.LocalIndex),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Callvirt, PropertySetter(typeof(PickupSyncInfo), nameof(PickupSyncInfo.InUse))),

                    // this.TargetPickup.NetworkInfo = info
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
                    new(OpCodes.Ldloc_S, info.LocalIndex),
                    new(OpCodes.Callvirt, PropertySetter(typeof(ItemPickupBase), nameof(ItemPickupBase.NetworkInfo))),

                    // return
                    new(OpCodes.Br_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}