﻿// -----------------------------------------------------------------------
// <copyright file="Attacked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp939ClawAbility.ServerProcessCmd(Mirror.NetworkReader)"/>
    /// to add <see cref="Handlers.Scp939.Attacked"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939ClawAbility), nameof(Scp939ClawAbility.ServerProcessCmd))]
    internal class Attacked
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(continueLabel);

            newInstructions.InsertRange(
                0, new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp939ClawAbility), nameof(Scp939ClawAbility.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AttackedEventArgs))[0]),

                    new(OpCodes.Call, Method(typeof(Handlers.Scp939), nameof(Handlers.Scp939.OnAttacked))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}