// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)" />.
    /// Adds the <see cref="Handlers.Player.InteractingDoor" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingDoor))]
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(InteractingDoorEventArgs));

            List<Label> jmp = null;
            Label interactionAllowed = generator.DefineLabel();
            Label permissionDenied = generator.DefineLabel();
            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(InteractingDoor), nameof(InteractingDoor.CanStateChange))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // InteractingDoorEventArgs ev = new(Player.Get(ply), __instance, false);
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingDoorEventArgs))[0]),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                });

            int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(DoorVariant), nameof(DoorVariant.LockBypassDenied)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Handlers.Player.OnInteractingDoor(ev);
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingDoor))),

                    // if (ev.IsAllowed)
                    //    go to interactionAllowed;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, interactionAllowed),
                });

            // attaching permission denied label
            offset = -3;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(DoorVariant), nameof(DoorVariant.PermissionsDenied)))) + offset;
            newInstructions[index].WithLabels(permissionDenied);

            // replace the condition check
            offset = -7;
            index = newInstructions.FindIndex(instruction => instruction.Calls(PropertySetter(typeof(DoorVariant), nameof(DoorVariant.NetworkTargetState)))) + offset;
            jmp = newInstructions[index].ExtractLabels();
            newInstructions.RemoveRange(index, 2);

            // insert interaction Allowed label
            newInstructions[index].WithLabels(interactionAllowed);
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.IsAllowed = flag;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jmp),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Callvirt, PropertySetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),

                    // Handlers.Player.OnInteractingDoor(ev);
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingDoor))),

                    // if (!ev.IsAllowed)
                    //    go to permission denied;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingDoorEventArgs), nameof(InteractingDoorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, permissionDenied),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool CanStateChange(DoorVariant variant)
        {
            return !(variant.GetExactState() > 0f && variant.GetExactState() < 1f);
        }
    }
}