// -----------------------------------------------------------------------
// <copyright file="ProcessDisarmMessage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Disarming;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DisarmingHandlers.ServerProcessDisarmMessage"/>.
    /// Adds the <see cref="Player.Handcuffing"/> and <see cref="Player.RemovingHandcuffs"/> events.
    /// </summary>
    [HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
    internal static class ProcessDisarmMessage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -5;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Br) + offset;

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingHandcuffsEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnRemovingHandcuffs))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RemovingHandcuffsEventArgs), nameof(RemovingHandcuffsEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_1);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(HandcuffingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnHandcuffing))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HandcuffingEventArgs), nameof(HandcuffingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
