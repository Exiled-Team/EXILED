// -----------------------------------------------------------------------
// <copyright file="PickingUpAmmo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Searching;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AmmoSearchCompletor.Complete" /> for the <see cref="Handlers.Player.PickingUpItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.PickingUpItem))]
    [HarmonyPatch(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.Complete))]
    internal static class PickingUpAmmo
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brfalse) + offset;

            newInstructions[index].labels.Add(continueLabel);
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this.TargetPickup
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(AmmoSearchCompletor), nameof(AmmoSearchCompletor.TargetPickup))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PickingUpAmmoEventArgs ev = new(Player, ItemPickupBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PickingUpItemEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnPickingUpAmmo(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPickingUpItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PickingUpItemEventArgs), nameof(PickingUpItemEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // PickupSyncInfo info = this.TargetPickup.Info;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(SearchCompletor), nameof(SearchCompletor.TargetPickup))),
                    new(OpCodes.Ldfld, Field(typeof(ItemPickupBase), nameof(ItemPickupBase.Info))),
                    new(OpCodes.Stloc_3),

                    // info.InUse = false;
                    new(OpCodes.Ldloca_S, 3),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Call, PropertySetter(typeof(PickupSyncInfo), nameof(PickupSyncInfo.InUse))),

                    // this.TargetPickup.NetworkInfo = info;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(SearchCompletor), nameof(SearchCompletor.TargetPickup))),
                    new(OpCodes.Ldloc_3),
                    new(OpCodes.Call, PropertySetter(typeof(ItemPickupBase), nameof(ItemPickupBase.NetworkInfo))),

                    // return;
                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}