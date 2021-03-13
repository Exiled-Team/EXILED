// -----------------------------------------------------------------------
// <copyright file="Interacting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1118
#pragma warning disable SA1123
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.CallCmdInteract(string, GameObject)"/>.
    /// Adds the <see cref="InteractingTeslaEventArgs"/>, <see cref="InteractingDoorEventArgs"/>, <see cref="Handlers.Scp079.StartingSpeaker"/> and <see cref="Handlers.Scp079.StoppingSpeaker"/> event for SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallCmdInteract))]
    internal static class Interacting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            #region InteractingTeslaEventArgs

            // Index offset.
            var offset = 5;

            // Find first "ldstr Tesla Gate Burst", then add the offset to get "ldloc.3".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldstr &&
            (string)instruction.operand == "Tesla Gate Burst") + offset;

            // Get the return label.
            var returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // Declare a local variable of the type "InteractingTeslaEventArgs";
            var interactingTeslaEv = generator.DeclareLocal(typeof(InteractingTeslaEventArgs));

            // var ev = new InteractingTeslaEventArgs(Player.Get(this.gameObject), teslaGameObject.GetComponent<TeslaGate>(), manaFromLabel, this.curMana > manaFromLabel);
            //
            // Handlers.Map.OnInteractingTesla(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            var instructionsToInsert = new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // teslaGameObject.GetComponent<TeslaGate>();
                new CodeInstruction(OpCodes.Ldloc_S, 4),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(GameObject), nameof(GameObject.GetComponent), generics: new[] { typeof(TeslaGate) })),

                // manaFromLabel
                new CodeInstruction(OpCodes.Ldloc_3),

                // this.curMana > manaFromLabel
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.curMana))),
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Cgt),

                // var ev = new InteractingTeslaEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingTeslaEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, interactingTeslaEv.LocalIndex),

                // Handlers.Map.OnInteractingTesla(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnInteractingTesla))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingTeslaEventArgs), nameof(InteractingTeslaEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // manaFromLabel = ev.AuxiliaryPowerCost
                new CodeInstruction(OpCodes.Ldloc_S, interactingTeslaEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingTeslaEventArgs), nameof(InteractingTeslaEventArgs.AuxiliaryPowerCost))),
                new CodeInstruction(OpCodes.Stloc_3),
            };

            newInstructions.InsertRange(index, instructionsToInsert);

            var instructionsToMoveOffset = 10;
            var instructionsToMoveCount = 13;

            // GameObject teslaGameObject = GameObject.Find(this.currentZone + "/" + this.currentRoom + "/Gate");
            //
            // if (gameObject == null)
            //   return;
            var instructionsToMove = newInstructions.GetRange(index + instructionsToInsert.Length + instructionsToMoveOffset, instructionsToMoveCount);

            // Move the instructions block to the start of the transpiler and and remove it.
            newInstructions.RemoveRange(index + instructionsToInsert.Length + instructionsToMoveOffset, instructionsToMoveCount);
            newInstructions.InsertRange(index, instructionsToMove);

            // New index offset.
            var newOffest = -2;

            // Find first "teslaGate.RpcInstantBurst" method, then add the offset to get "ldloc.s 4".
            var newIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt &&
            (MethodInfo)instruction.operand == Method(typeof(TeslaGate), nameof(TeslaGate.RpcInstantBurst))) + newOffest;

            // Move all labels from the first moved instruction.
            newInstructions[newIndex].MoveLabelsFrom(newInstructions[index]);

            #endregion

            #region TriggeringDoorEventArgs

            // Declare a local variable of the type "TriggeringDoorEventArgs";
            var interactingDoorEv = generator.DeclareLocal(typeof(TriggeringDoorEventArgs));

            offset = 10;

            // Find the first ',', then add the offset to get "ldloc.3".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_S &&
            (sbyte)instruction.operand == ',') + offset;

            // var ev = new TriggeringDoorEventArgs(Player.Get(this.gameObject), doorVariant, manaFromLabel, this.curMana > manaFromLabel);
            //
            // Handlers.Scp079.OnTriggeringDoor(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // doorVariant
                new CodeInstruction(OpCodes.Ldloc_1),

                // manaFromLabel
                new CodeInstruction(OpCodes.Ldloc_3),

                // __instance.curMana > manaFromLabel
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.curMana))),
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Cgt),

                // var ev = new TriggeringDoorEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(TriggeringDoorEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, interactingDoorEv.LocalIndex),

                // Handlers.Scp079.OnTriggeringDoor(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnTriggeringDoor))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringDoorEventArgs), nameof(TriggeringDoorEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // manaFromLabel = ev.AuxiliaryPowerCost
                new CodeInstruction(OpCodes.Ldloc_S, interactingDoorEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(TriggeringDoorEventArgs), nameof(TriggeringDoorEventArgs.AuxiliaryPowerCost))),
                new CodeInstruction(OpCodes.Stloc_3),
            });

            #endregion

            #region StartingSpeakerEventArgs

            // Declare a local variable of the type "StartingSpeakerEventArgs";
            var startingSpeakerEv = generator.DeclareLocal(typeof(StartingSpeakerEventArgs));

            offset = -1;

            // Find the first 1.5f, then add the offset to get "ldloc.3".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_R4 &&
            (float)instruction.operand == 1.5f) + offset;

            // var ev = new StartingSpeakerEventArgs(Player.Get(this.gameObject), Map.FindParentRoom(this.currentCamera.gameObject), manaFromLabel, this.curMana > manaFromLabel * 1.5f);
            //
            // Handlers.Scp079.OnStartingSpeaker(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // Map.FindParentRoom(this.currentCamera.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.currentCamera))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.FindParentRoom))),

                // manaFromLabel
                new CodeInstruction(OpCodes.Ldloc_3),

                // __instance.curMana > manaFromLabel * 1.5f
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.curMana))),
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Ldc_R4, 1.5f),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Cgt),

                // var ev = new StartingSpeakerEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartingSpeakerEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, startingSpeakerEv.LocalIndex),

                // Handlers.Scp079.OnStartingSpeaker(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnStartingSpeaker))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(StartingSpeakerEventArgs), nameof(StartingSpeakerEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // manaFromLabel = ev.AuxiliaryPowerCost
                new CodeInstruction(OpCodes.Ldloc_S, startingSpeakerEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(StartingSpeakerEventArgs), nameof(StartingSpeakerEventArgs.AuxiliaryPowerCost))),
                new CodeInstruction(OpCodes.Stloc_3),
            });

            #endregion

            #region StoppingSpeakerEventArgs

            offset = -1;

            // Find the first string.Empty, then add the offset to get "ldarg.0".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldsfld &&
            (FieldInfo)instruction.operand == Field(typeof(string), nameof(string.Empty))) + offset;

            // if (string.IsNullOrEmpty(this.Speaker)
            //   return;
            //
            // var ev = new StoppingSpeakerEventArgs(Player.Get(this.gameObject), Map.FindParentRoom(this.currentCamera.gameObject), true);
            //
            // Handlers.Scp079.OnStoppingSpeaker(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // if (string.IsNullOrEmpty(this.Speaker)
                //   return;
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.Speaker))),
                new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.IsNullOrEmpty))),
                new CodeInstruction(OpCodes.Brtrue, returnLabel),

                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // Map.FindParentRoom(this.currentCamera.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.currentCamera))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.FindParentRoom))),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new StartingSpeakerEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingSpeakerEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Scp079.OnStartingSpeaker(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnStoppingSpeaker))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(StoppingSpeakerEventArgs), nameof(StoppingSpeakerEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            #endregion

            #region ElevatorTeleportingEventArgs

            // Index offset.
            offset = 5;

            // Find first "ldstr Elevator Teleport", then add the offset to get "ldloc.3".
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldstr &&
            (string)instruction.operand == "Elevator Teleport") + offset;

            // Declare a local variable of the type "ElevatorTeleportingEventArgs";
            var elevatorTeleportEv = generator.DeclareLocal(typeof(ElevatorTeleportingEventArgs));

            // var ev = new ElevatorTeleportingEventArgs(Player.Get(this.gameObject), camera, manaFromLabel, this.curMana > manaFromLabel);
            //
            // Handlers.Scp079.OnElevatorTeleporting(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            instructionsToInsert = new[]
            {
                // Player.Get(this.gameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // camera
                new CodeInstruction(OpCodes.Ldloc_S, 12),

                // manaFromLabel
                new CodeInstruction(OpCodes.Ldloc_3),

                // this.curMana > manaFromLabel
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.curMana))),
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Cgt),

                // var ev = new ElevatorTeleportingEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ElevatorTeleportingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, elevatorTeleportEv.LocalIndex),

                // Handlers.Map.OnElevatorTeleporting(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnElevatorTeleporting))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ElevatorTeleportingEventArgs), nameof(ElevatorTeleportingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // manaFromLabel = ev.AuxiliaryPowerCost
                new CodeInstruction(OpCodes.Ldloc_S, elevatorTeleportEv.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ElevatorTeleportingEventArgs), nameof(ElevatorTeleportingEventArgs.AuxiliaryPowerCost))),
                new CodeInstruction(OpCodes.Stloc_3),
            };

            newInstructions.InsertRange(index, instructionsToInsert);

            instructionsToMoveOffset = 10;
            instructionsToMoveCount = 30;

            // Camera079 camera = null;
            //
            // foreach (global::Scp079Interactable scp079Interactable in this.nearbyInteractables)
            // {
            //   if (scp079Interactable.type == Scp079Interactable.InteractableType.ElevatorTeleport)
            //     camera = scp079Interactable.optionalObject.GetComponent<Camera079>();
            // }
            //
            // if (camera != null)
            //   return;
            instructionsToMove = newInstructions.GetRange(index + instructionsToInsert.Length + instructionsToMoveOffset, instructionsToMoveCount);

            // Move the instructions block to the start of the transpiler and and remove it.
            newInstructions.RemoveRange(index + instructionsToInsert.Length + instructionsToMoveOffset, instructionsToMoveCount);
            newInstructions.InsertRange(index, instructionsToMove);

            // New index offset.
            newOffest = -4;

            // Find first "teslaGate.RpcInstantBurst" method, then add the offset to get "ldarg.0".
            newIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.RpcSwitchCamera))) + newOffest;

            // Move all labels from the first moved instruction.
            newInstructions[newIndex].MoveLabelsFrom(newInstructions[index]);

            #endregion

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
