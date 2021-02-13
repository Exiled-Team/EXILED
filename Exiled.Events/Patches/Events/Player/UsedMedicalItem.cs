// -----------------------------------------------------------------------
// <copyright file="UsedMedicalItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1118 // Parameter should not span multiple lines

    /// <summary>
    /// Patches a method, the class in which it's defined, is compiler-generated, <see cref="ConsumableAndWearableItems"/>.
    /// Adds the <see cref="Handlers.Player.MedicalItemUsed"/> event.
    /// </summary>
    internal static class UsedMedicalItem
    {
        private static readonly HarmonyMethod PatchMethod = new HarmonyMethod(typeof(UsedMedicalItem), nameof(Transpiler));

        private static Type type;
        private static MethodInfo method;

        internal static void Patch()
        {
            const string nestedName = "<UseMedicalItem>d__24";
            const string methodName = "MoveNext";

            type = Array.Find(typeof(ConsumableAndWearableItems).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public), t => t.Name == nestedName);
            method = type != null ? AccessTools.Method(type, methodName) : null;
            if (type == null || method == null)
            {
                Log.Error($"Couldn't find `{nestedName}::{methodName}` ({type == null}, {method == null}) inside `ConsumableAndWearableItems`: Player::MedicalItemUsed event won't fire");
                return;
            }

            Exiled.Events.Events.Instance.Harmony.Patch(method, transpiler: PatchMethod);
        }

        internal static void Unpatch()
        {
            if (type != null && method != null)
            {
                Exiled.Events.Events.Instance.Harmony.Unpatch(method, PatchMethod.method);

                type = null;
                method = null;
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var index = newInstructions.FindLastIndex(op => op.opcode == OpCodes.Blt);
            newInstructions.InsertRange(++index, new[]
            {
                // Player.Get(ConsumableAndWearableItems._hub)
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems._hub))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // ConsumableAndWearableItems.usableItems[mid].inventoryID
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.usableItems))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(type, "mid")),
                new CodeInstruction(OpCodes.Ldelem, typeof(ConsumableAndWearableItems.UsableItem)),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ConsumableAndWearableItems.UsableItem), nameof(ConsumableAndWearableItems.UsableItem.inventoryID))),

                // Player.OnMedicalItemDequipped(new UsedMedicalItemEventArgs(arg1, arg2))
                new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(UsedMedicalItemEventArgs), new[] { typeof(Player), typeof(ItemType) })),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Handlers.Player), nameof(Handlers.Player.OnMedicalItemDequipped))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
