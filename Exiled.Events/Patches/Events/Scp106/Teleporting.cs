// -----------------------------------------------------------------------
// <copyright file="Teleporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp106;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp106;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches <see cref="Scp106HuntersAtlasAbility.GetSafePosition" />.
    ///     Adds the <see cref="Scp106.Teleporting" /> event.
    /// </summary>
    [EventPatch(typeof(Scp106), nameof(Scp106.Teleporting))]
    [HarmonyPatch(typeof(Scp106HuntersAtlasAbility), nameof(Scp106HuntersAtlasAbility.GetSafePosition))]
    internal static class Teleporting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(TeleportingEventArgs));
            Label continueLabel = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Owner);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106HuntersAtlasAbility), nameof(Scp106HuntersAtlasAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // safePosition
                    new(OpCodes.Ldloc_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // TeleportingEventArgs ev = new(player, safePosition, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TeleportingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Scp106.OnTeleporting(ev)
                    new(OpCodes.Call, Method(typeof(Scp106), nameof(Scp106.OnTeleporting))),

                    // if (ev.IsAllowed)
                    //     goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // return this.Owner.transform.position;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp106HuntersAtlasAbility), nameof(Scp106HuntersAtlasAbility.Owner))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.transform))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.position))),
                    new(OpCodes.Ret),

                    // safePosition = ev.Position;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TeleportingEventArgs), nameof(TeleportingEventArgs.Position))),
                    new(OpCodes.Stloc_0),
                });

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
