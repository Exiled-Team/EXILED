// -----------------------------------------------------------------------
// <copyright file="ChangingGroup.cs" company="Exiled Team">
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

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ServerRoles.SetGroup(UserGroup, bool, bool)" />.
    ///     Adds the <see cref="Handlers.Player.ChangingGroup" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingGroup))]
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup))]
    internal static class ChangingGroup
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // ChangingGroupEventArgs ev = new(Player.Get(this.gameObject), group, true);
            //
            // Handlers.Player.OnChangingGroup(ev);
            // group = ev.NewGroup;
            //
            // if (!ev.IsAllowed)
            //     return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.gameObject)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ServerRoles), nameof(ServerRoles.gameObject))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                    // group
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingGroupEventArgs ev = new(Player, UserGroup, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingGroupEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnChangingGroup(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingGroup))),

                    // group = ev.NewGroup;
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingGroupEventArgs), nameof(ChangingGroupEventArgs.NewGroup))),
                    new(OpCodes.Starg_S, 1),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingGroupEventArgs), nameof(ChangingGroupEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}