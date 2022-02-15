// -----------------------------------------------------------------------
// <copyright file="Blinking.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Scp173
{
#pragma warning disable SA1118

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using SEXiled.API.Features;
    using SEXiled.Events.EventArgs;

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
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.Hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._observingPlayers))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Blinking), nameof(GetObservingPlayers))),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(BlinkingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnBlinking))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.BlinkPosition))),
                new CodeInstruction(OpCodes.Starg, 1),
            });

            offset = 1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand ==
                Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._blinkCooldownRemaining))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(BlinkingEventArgs), nameof(BlinkingEventArgs.BlinkCooldown))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._blinkCooldownRemaining))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static List<Player> GetObservingPlayers(IEnumerable<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();
    }
}
