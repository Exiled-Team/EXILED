// -----------------------------------------------------------------------
// <copyright file="StartPryingGate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp096;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches the <see cref="Scp096PrygateAbility.ServerProcessCmd(NetworkReader)" /> method.
    ///     Adds the <see cref="Handlers.Scp096.StartPryingGate" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096PrygateAbility), nameof(Scp096PrygateAbility.ServerProcessCmd))]
    internal static class StartPryingGate
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            const int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(DoorVariant), nameof(DoorVariant.TargetState)))) + offset;

            // StartPryingGateEventArgs ev = new StartPryingGateEventArgs(this, Player, Gate, true);
            //
            // Handlers.Scp096.OnStartPryingGate(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // base.ScpRole
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.ScpRole))),

                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this._syncDoor
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp096PrygateAbility), nameof(Scp096PrygateAbility._syncDoor))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // StartPryingGateEventArgs ev = new StartPryingGateEventArgs(Scp096Role, Player, PryableDoor, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StartPryingGateEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp096.OnStartPryingGate(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnStartPryingGate))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StartPryingGateEventArgs), nameof(StartPryingGateEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}