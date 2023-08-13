// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimension.cs" company="Exiled Team">
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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp106;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)"/> method.
    /// Adds the <see cref="Handlers.Player.EscapingPocketDimension"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.EscapingPocketDimension))]
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class EscapingPocketDimension
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(EscapingPocketDimensionEventArgs));

            Label ret = generator.DefineLabel();

            const int offset = -5;
            const int labelOffset = -2;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.ServerOverridePosition)))) + offset;

            // Replaces "ServerOverridePosition" with our logic
            newInstructions.RemoveRange(index, 6);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_1).WithLabels((Label)newInstructions[index + labelOffset].operand),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Scp106PocketExitFinder.GetBestExitPosition(fpcRole)
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Call, Method(typeof(Scp106PocketExitFinder), nameof(Scp106PocketExitFinder.GetBestExitPosition), new[] { typeof(IFpcRole) })),

                    // EscapingPocketDimensionEventArgs ev = new(Player, Vector3)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EscapingPocketDimensionEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnEscapingPocketDimension(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEscapingPocketDimension))),

                    // if (ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // ev.Player.Teleport(ev.TeleportPosition)
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.Player))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EscapingPocketDimensionEventArgs), nameof(EscapingPocketDimensionEventArgs.TeleportPosition))),
                    new(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.Teleport), new[] { typeof(Vector3) })),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}