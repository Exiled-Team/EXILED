// -----------------------------------------------------------------------
// <copyright file="SyncingData.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
#pragma warning restore SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;
    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AnimationController.UserCode_CmdSyncData(byte, Vector2)"/>.
    /// Adds the <see cref="Player.SyncingData"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AnimationController), nameof(AnimationController.UserCode_CmdSyncData))]
    internal static class SyncingData
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret);
            var offset = 1;

            index += offset;

            var ev = generator.DeclareLocal(typeof(SyncingDataEventArgs));

            var returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // this
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // Player.Get(__instance.gameObject)
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AnimationController), nameof(AnimationController.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),

                // Vector2 v2
                new CodeInstruction(OpCodes.Ldarg_2),

                // byte state
                new CodeInstruction(OpCodes.Ldarg_1),

                // var ev = SyncingDataEventArgs(...)
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SyncingDataEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Player.OnSyncingData(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSyncingData))),

                // if (!ev.IsAllowed) return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SyncingDataEventArgs), nameof(SyncingDataEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
