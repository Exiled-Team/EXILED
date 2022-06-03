// -----------------------------------------------------------------------
// <copyright file="Jumping.cs" company="Exiled Team">
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

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerMovementSync.ReceivePosition2DJump(NetworkConnection, PositionMessage2DJump)"/> and
    /// <see cref="PlayerMovementSync.ReceivePositionJump(NetworkConnection, PositionMessageJump)"/>.
    /// Adds the <see cref="Player.Jumping"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovementSync))]
    internal static class Jumping
    {
        [HarmonyPatch(nameof(PlayerMovementSync.ReceivePosition2DJump))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler2D(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(JumpingEventArgs));
            LocalBuilder pos = generator.DeclareLocal(typeof(Vector2));

            Label retLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_0);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(PositionMessage2DJump), nameof(PositionMessage2DJump.Position))),
                new(OpCodes.Stloc_S, pos.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new(OpCodes.Call, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),
                new(OpCodes.Ldloc_S, pos.LocalIndex),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JumpingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Player), nameof(Handlers.Player.OnJumping))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.Player))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, PropertySetter(typeof(API.Features.Player), nameof(API.Features.Player.IsJumping))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.Player))),
                new(OpCodes.Call, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.ReferenceHub))),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))),
                new(OpCodes.Ldloc_S, pos.LocalIndex),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Call, Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ReceivePosition2D), new[] { typeof(Vector2), typeof(bool) })),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        [HarmonyPatch(nameof(PlayerMovementSync.ReceivePositionJump))]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(JumpingEventArgs));
            LocalBuilder pos = generator.DeclareLocal(typeof(Vector3));

            Label retLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_0);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, Field(typeof(PositionMessageJump), nameof(PositionMessageJump.Position))),
                new(OpCodes.Stloc_S, pos.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new(OpCodes.Call, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(uint) })),
                new(OpCodes.Ldloc_S, pos.LocalIndex),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JumpingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Player), nameof(Handlers.Player.OnJumping))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.Player))),
                new(OpCodes.Call, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.ReferenceHub))),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))),
                new(OpCodes.Ldloc_S, pos.LocalIndex),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Call, Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ReceivePosition), new[] { typeof(Vector3), typeof(bool) })),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
