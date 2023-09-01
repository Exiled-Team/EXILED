// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1005
#pragma warning disable SA1515
#pragma warning disable SA1513
#pragma warning disable SA1512
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Enums;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using PlayerRoles;

    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)" />.
    ///     Adds the <see cref="Handlers.Player.InteractingDoor" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingDoor))]
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingDoorEventArgs));
            Label jmp = generator.DefineLabel();
            Label skip = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // InteractingDoorEventArgs ev = new(Player.Get(ply), __instance, true, DoorBeepType.InteractionAllowed);
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ldc_I4_3),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingDoorEventArgs))[0]),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                });

            int offset = -5;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(DoorVariant), nameof(DoorVariant.LockBypassDenied)))) + offset;
            newInstructions[index] = new CodeInstruction(OpCodes.Brtrue_S, jmp);

            offset = 2;
            index += offset;

            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // jmp
                    // ev.InteractionResult = DoorBeepType.LockBypassDenied;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jmp),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.InteractionResult))),

                    // InteractingDoor.Process(__instance, ply, colliderId, ev);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(InteractingDoor), nameof(InteractingDoor.Process))),
                });

            offset = 2;
            index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(DoorVariant), nameof(DoorVariant.AllowInteracting)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                        // ev.IsAllowed = false;
                        new(OpCodes.Ldloc_S, ev.LocalIndex),
                        new(OpCodes.Ldc_I4_0),
                        new(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),

                        // ev.InteractionResult = DoorBeepType.InteractionDenied;
                        new(OpCodes.Ldloc_S, ev.LocalIndex),
                        new(OpCodes.Ldc_I4_2),
                        new(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.InteractionResult))),

                        // InteractingDoor.Process(__instance, ply, colliderId, ev);
                        new(OpCodes.Ldarg_0),
                        new(OpCodes.Ldarg_1),
                        new(OpCodes.Ldarg_2),
                        new(OpCodes.Ldloc_S, ev.LocalIndex),
                        new(OpCodes.Call, Method(typeof(InteractingDoor), nameof(InteractingDoor.Process))),
                });

            offset = -6;
            index = newInstructions.FindIndex(instruction => instruction.Calls(PropertySetter(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState)))) + offset;

            newInstructions.RemoveRange(index, 19);
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                        // if (flag)
                        // goto skip
                        new(OpCodes.Brtrue_S, skip),

                        // ev.InteractionResult = DoorBeepType.PermissionDenied;
                        new(OpCodes.Ldloc_S, ev.LocalIndex),
                        new(OpCodes.Ldc_I4_0),
                        new(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.InteractionResult))),

                        // skip
                        // InteractingDoor.Process(__instance, ply, colliderId, ev);
                        new CodeInstruction(OpCodes.Ldarg_0).WithLabels(skip),
                        new(OpCodes.Ldarg_1),
                        new(OpCodes.Ldarg_2),
                        new(OpCodes.Ldloc_S, ev.LocalIndex),
                        new(OpCodes.Call, Method(typeof(InteractingDoor), nameof(InteractingDoor.Process))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void Process(DoorVariant __instance, ReferenceHub ply, byte colliderId, InteractingDoorEventArgs ev)
        {
            Handlers.Player.OnInteractingDoor(ev);
            if (ev.IsAllowed)
            {
                switch (ev.InteractionResult)
                {
                    case DoorBeepType.PermissionDenied:
                        __instance.PermissionsDenied(ply, colliderId);
                        DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
                        break;
                    case DoorBeepType.LockBypassDenied:
                        __instance.LockBypassDenied(ply, colliderId);
                        break;
                    default:
                        __instance.NetworkTargetState = !__instance.TargetState;
                        __instance._triggerPlayer = ply;
                        break;
                }
            }
        }
    }
}