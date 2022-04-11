// -----------------------------------------------------------------------
// <copyright file="Blinking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.ServerHandleBlinkMessage"/>.
    /// Adds the <see cref="Handlers.Scp173.Blinking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.ServerHandleBlinkMessage))]
    internal static class Blinking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = -13;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand ==
                Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ForcePosition), new[] { typeof(Vector3) })) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(BlinkingEventArgs));
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._observingPlayers))),
                new(OpCodes.Call, Method(typeof(Blinking), nameof(GetObservingPlayers))),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(BlinkingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnBlinking))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.BlinkPosition))),
                new(OpCodes.Starg, 1),
            });

            offset = 1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand ==
                Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._blinkCooldownRemaining))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(returnLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.BlinkCooldown))),
                new(OpCodes.Stfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._blinkCooldownRemaining))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static List<Player> GetObservingPlayers(IEnumerable<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();
    }
}
