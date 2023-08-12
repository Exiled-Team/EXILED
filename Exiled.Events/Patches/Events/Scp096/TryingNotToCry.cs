// -----------------------------------------------------------------------
// <copyright file="TryingNotToCry.cs" company="Exiled Team">
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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp096;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches the <see cref="Scp096TryNotToCryAbility.ServerProcessCmd(NetworkReader)" /> method.
    ///     Adds the <see cref="Handlers.Scp096.TryingNotToCry" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp096), nameof(Handlers.Scp096.TryingNotToCry))]
    [HarmonyPatch(typeof(Scp096TryNotToCryAbility), nameof(Scp096TryNotToCryAbility.ServerProcessCmd))]
    internal static class TryingNotToCry
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            // TryingNotToCryEventArgs ev = new(Scp096 scp096, Player player, door, true);
            //
            // Handlers.Scp096.OnTryingNotToCry(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // base.ScpRole
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.ScpRole))),

                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp096Role>), nameof(ScpStandardSubroutine<Scp096Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // TryingNotToCryEventArgs ev = new(Scp096Role, Player, DoorVariant, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(TryingNotToCryEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Scp096.OnTryingNotToCry(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnTryingNotToCry))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(TryingNotToCryEventArgs), nameof(TryingNotToCryEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}