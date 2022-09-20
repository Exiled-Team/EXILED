// -----------------------------------------------------------------------
// <copyright file="InteractingElevator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Lift = Lift;

    /// <summary>
    ///     Patches <see cref="PlayerInteract.UserCode_CmdUseElevator(UnityEngine.GameObject)" />.
    ///     Adds the <see cref="Handlers.Player.InteractingElevator" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUseElevator))]
    internal class InteractingElevator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -2;
            int index = newInstructions.FindLastIndex(i => i.Calls(Method(typeof(Lift), nameof(Lift.UseLift)))) + offset;

            Label continueLabel = newInstructions[index + 6].labels[0];

            // InteractingElevatorEventArgs ev = new(API.Features.Player.Get(hub), elevator, lift);
            // Handlers.Player.OnInteractingElevator(ev);
            // if (!ev.IsAllowed)
            //     continue;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(PlayerInteract), nameof(PlayerInteract._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldloc_3),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingElevatorEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingElevator))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingElevatorEventArgs), nameof(InteractingElevatorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}