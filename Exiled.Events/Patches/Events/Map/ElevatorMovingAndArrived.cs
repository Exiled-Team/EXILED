// -----------------------------------------------------------------------
// <copyright file="ElevatorMovingAndArrived.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;
    using HarmonyLib;
    using Interactables.Interobjects;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ElevatorChamber.TrySetDestination"/>
    /// to add <see cref="Handlers.Map.ElevatorArrived"/> and <see cref="Handlers.Map.ElevatorMoving"/> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.ElevatorArrived))]
    [EventPatch(typeof(Handlers.Map), nameof(Handlers.Map.ElevatorMoving))]
    [HarmonyPatch(typeof(ElevatorChamber), nameof(ElevatorChamber.TrySetDestination))]
    internal class ElevatorMovingAndArrived
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(NetworkServer), nameof(NetworkServer.active))));

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Lift.Get(this);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Lift), nameof(Lift.Get), new[] { typeof(ElevatorChamber) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ElevatorMovingEventArgs ev = new(Lift, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ElevatorMovingEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Map.OnElevatorMoving(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnElevatorMoving))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ElevatorMovingEventArgs), nameof(ElevatorMovingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            int offset = 1;
            index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(ElevatorChamber), nameof(ElevatorChamber.SetInnerDoor)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Lift.Get(this);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Lift), nameof(Lift.Get), new[] { typeof(ElevatorChamber) })),

                    // ElevatorArrivedEventArgs ev = new(Lift);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ElevatorArrivedEventArgs))[0]),

                    // Handlers.Map.OnElevatorArrived(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Map), nameof(Handlers.Map.OnElevatorArrived))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}