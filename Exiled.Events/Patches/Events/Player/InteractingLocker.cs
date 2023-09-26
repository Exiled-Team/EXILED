// -----------------------------------------------------------------------
// <copyright file="InteractingLocker.cs" company="Exiled Team">
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
    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Locker.ServerInteract(ReferenceHub, byte)" />.
    ///     Adds the <see cref="Handlers.Player.InteractingLocker" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.InteractingLocker))]
    [HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
    internal static class InteractingLocker
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -9;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ply);
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // this.Chambers[colliderId]
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Locker), nameof(Locker.Chambers))),
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Ldelem_Ref),

                    // colliderId
                    new(OpCodes.Ldarg_2),

                    // !flag
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),

                    // InteractingLockerEventArgs ev = new(Player, Locker, LockerChamber, byte, bool)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingLockerEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnInteractingLocker(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnInteractingLocker))),

                    // flag = !ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingLockerEventArgs), nameof(InteractingLockerEventArgs.IsAllowed))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ceq),
                    new(OpCodes.Stloc_0),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}