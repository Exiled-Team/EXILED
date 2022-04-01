// -----------------------------------------------------------------------
// <copyright file="ChangingAttachments.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item {/*
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Inventory.SyncListItemInfo.ModifyAttachments(int, int, int, int)"/>.
    /// Adds the <see cref="Handlers.Item.ChangingAttachments"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Inventory.SyncListItemInfo), nameof(Inventory.SyncListItemInfo.ModifyAttachments))]
    internal static class ChangingAttachments
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for the first "ldloca.s".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloca_S) + offset;

            // Set the return label to the last instruction.
            var returnLabel = generator.DefineLabel();

            // Declare ChangingDurabilityEventArgs, to be able to store its instance with "stloc.1".
            var ev = generator.DeclareLocal(typeof(ChangingAttachmentsEventArgs));

            // var ev = new ChangingAttachmentsEventArgs(item, sight, barrel, other, true);
            //
            // Handlers.Player.OnChangingAttachments(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // base[index] = ev.Item;
            //
            // return;
            newInstructions.InsertRange(index, new[]
            {
                 new CodeInstruction(OpCodes.Ldloc_S, 0),
                 new CodeInstruction(OpCodes.Ldarg_2),
                 new CodeInstruction(OpCodes.Ldarg_3),
                 new CodeInstruction(OpCodes.Ldarg_S, 4),
                 new CodeInstruction(OpCodes.Ldc_I4_1),
                 new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingAttachmentsEventArgs))[0]),
                 new CodeInstruction(OpCodes.Dup),
                 new CodeInstruction(OpCodes.Dup),
                 new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                 new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnChangingAttachments))),
                 new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.IsAllowed))),
                 new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                 new CodeInstruction(OpCodes.Ldarg_0),
                 new CodeInstruction(OpCodes.Ldarg_1),
                 new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                 new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingAttachmentsEventArgs), nameof(ChangingAttachmentsEventArgs.NewItem))),
                 new CodeInstruction(OpCodes.Call, Method(typeof(SyncList<Inventory.SyncItemInfo>), "set_Item")),
                 new CodeInstruction(OpCodes.Ret).WithLabels(returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }*/
}
