// -----------------------------------------------------------------------
// <copyright file="JailbirdPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Item;
    using Handlers;
    using HarmonyLib;
    using InventorySystem.Items.Jailbird;
    using Mirror;
    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches
    ///     <see cref="JailbirdItem.ServerProcessCmd(NetworkReader)" />.
    ///     Adds the <see cref="Item.Swinging" /> event.
    /// </summary>
    [HarmonyPatch(typeof(JailbirdItem), nameof(JailbirdItem.ServerProcessCmd))]
    internal static class JailbirdPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label retLabel = generator.DefineLabel();

            const int offset = 2;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(NetworkReader), nameof(NetworkReader.ReadByte)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this (JailbirdItem)
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // header (JailbirdMessageType)
                    new CodeInstruction(OpCodes.Ldloc_0),

                    // HandleJailbird(JailbirdItem, JailbirdMessageType)
                    new(OpCodes.Call, Method(typeof(JailbirdPatch), nameof(HandleJailbird))),

                    // return false if not allowed
                    new(OpCodes.Brfalse_S, retLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Processes Jailbird statuses.
        /// </summary>
        /// <param name="instance"> <see cref="JailbirdItem"/> instance. </param>
        /// <param name="messageType"> <see cref="JailbirdMessageType"/> type. </param>
        /// <returns> <see cref="bool"/>. </returns>
        private static bool HandleJailbird(JailbirdItem instance, JailbirdMessageType messageType)
        {
            switch (messageType)
            {
                case JailbirdMessageType.AttackTriggered:
                {
                    SwingingEventArgs ev = new(instance.Owner, instance);

                    Item.OnSwinging(ev);

                    return ev.IsAllowed;
                }

                case JailbirdMessageType.ChargeStarted:
                {
                    ChargingJailbirdEventArgs ev = new(instance.Owner, instance);

                    Item.OnChargingJailbird(ev);

                    return ev.IsAllowed;
                }

                default:
                    return true;
            }
        }
    }
}